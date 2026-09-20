using System.Text.Json;
using System.Net.Http;
using System.Net.Security;
using System.Text.RegularExpressions;
using System.Globalization;

var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
};
var httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(60) };
var baseUrl = "https://localhost:7000/api/BrowserTest/fetch?apiPath=";

var seasons = new Dictionary<int, string> {
    {48078, "Liga 1 2023"},
    {57741, "Liga 1 2024"},
    {70962, "Liga 1 2025"},
    {88529, "Liga 1 2026"}
};

var allEvents = new List<(DateTime Date, string Home, string Away, int HomeScore, int AwayScore)>();

foreach (var (seasonId, seasonName) in seasons)
{
    Console.Write($"Fetching {seasonName}...");
    
    var roundsResp = await Fetch($"unique-tournament/406/season/{seasonId}/rounds");
    var roundsDoc = JsonDocument.Parse(roundsResp);
    var roundsArray = roundsDoc.RootElement.GetProperty("rounds").EnumerateArray().ToList();
    
    int eventCount = 0;
    foreach (var round in roundsArray)
    {
        var roundNum = round.GetProperty("round").GetInt32();
        var prefix = round.GetProperty("prefix").GetString()!;
        
        try {
            var eventsResp = await Fetch($"unique-tournament/406/season/{seasonId}/events/round/{roundNum}/prefix/{Uri.EscapeDataString(prefix)}");
            var eventsDoc = JsonDocument.Parse(eventsResp);
            
            if (!eventsDoc.RootElement.TryGetProperty("events", out var eventsArray))
                continue;
            
            foreach (var evt in eventsArray.EnumerateArray())
            {
                var startTimestamp = evt.GetProperty("startTimestamp").GetInt64();
                var date = DateTimeOffset.FromUnixTimeSeconds(startTimestamp).DateTime.Date;
                
                var homeTeam = evt.GetProperty("homeTeam").GetProperty("name").GetString()!;
                var awayTeam = evt.GetProperty("awayTeam").GetProperty("name").GetString()!;
                
                int homeScore = 0, awayScore = 0;
                if (evt.TryGetProperty("homeScore", out var hs) && hs.TryGetProperty("current", out var hc))
                    homeScore = hc.GetInt32();
                if (evt.TryGetProperty("awayScore", out var as2) && as2.TryGetProperty("current", out var ac))
                    awayScore = ac.GetInt32();
                
                allEvents.Add((date, homeTeam, awayTeam, homeScore, awayScore));
                eventCount++;
            }
        } catch { }
    }
    Console.WriteLine($" {eventCount} events (total: {allEvents.Count})");
}

Console.WriteLine($"\nTotal SofaScore events: {allEvents.Count}");

// Save all events
var json = JsonSerializer.Serialize(allEvents.Select(e => new { date = e.Date.ToString("yyyy-MM-dd"), home = e.Home, away = e.Away, homeScore = e.HomeScore, awayScore = e.AwayScore }), new JsonSerializerOptions { WriteIndented = true });
File.WriteAllText("sofascore-events.json", json);

// Now compare the 25 sample matches
Console.WriteLine("\n=== COMPARANDO MUESTRA DE 25 ===\n");

string Normalize(string name) => Regex.Replace(name.ToLowerInvariant().Trim()
    .Replace("á", "a").Replace("é", "e").Replace("í", "i")
    .Replace("ó", "o").Replace("ú", "u").Replace("ñ", "n"), @"\s+", " ");

// The 25 sample matches (from the earlier SQLite query)
var sample = new (DateTime Date, string Home, string Away)[] {
    (new DateTime(2023,2,25), "Deportivo Garcilaso", "Atlético Grau"),
    (new DateTime(2023,3,24), "Universitario", "Cienciano"),
    (new DateTime(2023,3,26), "Deportivo Municipal", "Unión Comercio"),
    (new DateTime(2023,5,28), "Unión Comercio", "ADT"),
    (new DateTime(2023,7,3), "César Vallejo", "Cienciano"),
    (new DateTime(2023,8,16), "Carlos Manucci", "Real Garcilaso"),
    (new DateTime(2023,8,16), "Deportivo Garcilaso", "Sport Boys"),
    (new DateTime(2023,8,17), "Deportivo Binacional", "César Vallejo"),
    (new DateTime(2023,8,19), "Universitario", "Deportivo Garcilaso"),
    (new DateTime(2023,8,26), "Carlos Manucci", "Cienciano"),
    (new DateTime(2023,9,17), "ADT", "Sport Huancayo"),
    (new DateTime(2024,2,2), "Real Garcilaso", "UTC Cajamarca"),
    (new DateTime(2024,2,12), "Unión Comercio", "Universitario"),
    (new DateTime(2024,2,26), "ADT", "Deportivo Municipal"),
    (new DateTime(2024,3,26), "Deportivo Municipal", "Unión Comercio"),
    (new DateTime(2024,9,17), "ADT", "Sport Huancayo"),
    (new DateTime(2024,9,24), "Melgar", "Unión Comercio"),
    (new DateTime(2025,2,17), "Ayacucho", "Alianza Universidad"),
    (new DateTime(2025,2,27), "ADT", "Sport Boys"),
    (new DateTime(2025,3,7), "Alianza Lima", "Ayacucho"),
    (new DateTime(2025,5,18), "Melgar", "Atlético Grau"),
    (new DateTime(2025,10,16), "Alianza Lima", "Sport Boys"),
    (new DateTime(2026,2,15), "Universitario", "ADT"),
    (new DateTime(2026,2,23), "UCV Moquegua", "Deportivo Garcilaso"),
    (new DateTime(2026,2,28), "Sport Boys", "UCV Moquegua"),
};

int found = 0, scoreMatch = 0, scoreMismatch = 0, notFound = 0;
var mismatches = new List<string>();

foreach (var s in sample)
{
    var sofa = allEvents.FirstOrDefault(e =>
        e.Date == s.Date &&
        Normalize(e.Home) == Normalize(s.Home) &&
        Normalize(e.Away) == Normalize(s.Away));

    if (sofa.Date == default)
    {
        Console.WriteLine($"  NOT_FOUND | {s.Date:yyyy-MM-dd} | {s.Home} vs {s.Away}");
        notFound++;
        continue;
    }
    found++;

    // Score comparison would need CSV data; here we just confirm match exists
    Console.WriteLine($"  FOUND | {s.Date:yyyy-MM-dd} | {s.Home} vs {s.Away} | SofaScore: {sofa.HomeScore}-{sofa.AwayScore}");
}

Console.WriteLine();
Console.WriteLine("=== RESULTADOS AUDITORÍA vs SofaScore ===");
Console.WriteLine($"Muestra: 25");
Console.WriteLine($"Encontrados en SofaScore: {found}/25 ({found*100/25}%)");
Console.WriteLine($"No encontrados: {notFound}/25");
Console.WriteLine($"Tasa de coincidencia: {found*100/25}%");

async Task<string> Fetch(string path)
{
    var response = await httpClient.GetAsync(baseUrl + path);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsStringAsync();
}
