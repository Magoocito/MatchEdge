using System.Text;
using System.Text.Json;
using Microsoft.Data.Sqlite;

var conn = new SqliteConnection("Data Source=src/MatchEdge.Api/matchedge.db");
conn.Open();

// Step 1: Get 25 random matches from SQLite
var cmd = conn.CreateCommand();
cmd.CommandText = @"
    SELECT MatchDate, HomeTeamName, AwayTeamName, HomeWinOdds, DrawOdds, AwayWinOdds
    FROM HistoricalOdds
    WHERE HomeWinOdds > 0 AND DrawOdds > 0 AND AwayWinOdds > 0
    ORDER BY RANDOM()
    LIMIT 25";

var sqliteMatches = new List<(DateTime Date, string Home, string Away)>();
using (var reader = cmd.ExecuteReader())
{
    while (reader.Read())
    {
        sqliteMatches.Add((
            reader.GetDateTime(0),
            reader.GetString(1),
            reader.GetString(2)
        ));
    }
}
Console.WriteLine($"Muestra de SQLite: {sqliteMatches.Count} partidos");

// Step 2: Load verified CSVs to get FootyStats scores
var csvScores = new Dictionary<string, (int HomeScore, int AwayScore)>(StringComparer.OrdinalIgnoreCase);
foreach (var year in new[] { 2023, 2024, 2025 })
{
    var csvPath = $"data/footystats-results-{year}.csv";
    if (!File.Exists(csvPath)) continue;
    var lines = File.ReadAllLines(csvPath).Skip(1);
    foreach (var line in lines)
    {
        var parts = line.Split(',');
        if (parts.Length < 7) continue;
        if (!DateTime.TryParse(parts[0], out var date)) continue;
        var home = parts[1].Trim();
        var away = parts[3].Trim();
        if (!int.TryParse(parts[5], out var hs) || !int.TryParse(parts[6], out var as2)) continue;
        var key = $"{date:yyyy-MM-dd}|{Normalize(home)}|{Normalize(away)}";
        csvScores[key] = (hs, as2);
    }
}
Console.WriteLine($"CSV scores loaded: {csvScores.Count} matches");

// Step 3: Team name mappings (FootyStats -> SofaScore IDs)
var teamMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
{
    ["ADT"] = 335557, ["Academia Cantolao"] = 245083, ["Alianza Atlético"] = 2307,
    ["Alianza Lima"] = 2311, ["Alianza Universidad"] = 306660, ["Atlético Grau"] = 282538,
    ["Ayacucho"] = 33894, ["Carlos Manucci"] = 252245, ["Cienciano"] = 2301,
    ["Comerciantes Unidos"] = 213609, ["César Vallejo"] = 5281,
    ["Deportivo Binacional"] = 275839, ["Deportivo Garcilaso"] = 458584,
    ["Deportivo Municipal"] = 7031, ["FC Cajamarca"] = 87854,
    ["Juan Pablo II College"] = 511206, ["Los Chankas"] = 252254, ["Melgar"] = 2308,
    ["Real Garcilaso"] = 458584, ["Sport Boys"] = 2312, ["Sport Huancayo"] = 33895,
    ["Sporting Cristal"] = 2302, ["UCV Moquegua"] = 0, ["UTC Cajamarca"] = 87854,
    ["Universitario"] = 2305, ["Unión Comercio"] = 48431,
};

// Season IDs
var seasonIds = new Dictionary<int, int> { [2023] = 48078, [2024] = 57741, [2025] = 70962 };

// Step 4: Fetch SofaScore events for each season (by round)
Console.WriteLine("Fetching SofaScore events...");
var ssEvents = new List<(DateTime Date, int HomeId, int AwayId, int HomeScore, int AwayScore, string HomeName, string AwayName)>();
var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };

foreach (var (year, seasonId) in seasonIds)
{
    foreach (var prefix in new[] { "Apertura", "Clausura" })
    {
        for (int round = 1; round <= 20; round++)
        {
            try
            {
                var resp = await httpClient.GetAsync($"/api/BrowserTest/fetch?apiPath=unique-tournament/406/season/{seasonId}/events/round/{round}/prefix/{prefix}");
                if (!resp.IsSuccessStatusCode) continue;
                var json = await resp.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                if (!doc.RootElement.TryGetProperty("events", out var events)) continue;

                foreach (var ev in events.EnumerateArray())
                {
                    if (!ev.TryGetProperty("startTimestamp", out var ts)) continue;
                    var date = DateTimeOffset.FromUnixTimeSeconds(ts.GetInt64()).UtcDateTime;
                    if (!ev.TryGetProperty("homeTeam", out var ht) || !ev.TryGetProperty("awayTeam", out var at)) continue;
                    var homeId = ht.GetProperty("id").GetInt32();
                    var awayId = at.GetProperty("id").GetInt32();
                    var homeName = ht.TryGetProperty("name", out var hn) ? hn.GetString() ?? "" : "";
                    var awayName = at.TryGetProperty("name", out var an) ? an.GetString() ?? "" : "";

                    int homeScore = -1, awayScore = -1;
                    if (ev.TryGetProperty("homeScore", out var hs) && hs.TryGetProperty("current", out var hc) && hc.ValueKind == JsonValueKind.Number)
                        homeScore = hc.GetInt32();
                    if (ev.TryGetProperty("awayScore", out var asEl) && asEl.TryGetProperty("current", out var ac) && ac.ValueKind == JsonValueKind.Number)
                        awayScore = ac.GetInt32();

                    if (homeScore >= 0 && awayScore >= 0) // Only finished matches
                        ssEvents.Add((date, homeId, awayId, homeScore, awayScore, homeName, awayName));
                }
            }
            catch { }
            await Task.Delay(300);
        }
    }
    var yearEvents = ssEvents.Count(e => e.Date.Year == year || (e.Date.Month >= 1 && e.Date.Month <= 3 && e.Date.Year == year + 1));
    Console.WriteLine($"  {year}: {yearEvents} finished events loaded");
}
Console.WriteLine($"Total SofaScore events: {ssEvents.Count}");

// Step 5: Compare each SQLite match
Console.WriteLine();
Console.WriteLine($"=== AUDITORÍA REAL: SQLite (FootyStats) vs SofaScore — Comparación de Marcadores ===");
Console.WriteLine();

int totalMatched = 0;
int scoreMatch = 0;
int scoreMismatch = 0;
int notFoundInCSV = 0;
int notFoundInSS = 0;
var mismatches = new List<string>();

foreach (var m in sqliteMatches)
{
    var csvKey = $"{m.Date:yyyy-MM-dd}|{Normalize(m.Home)}|{Normalize(m.Away)}";

    // Find in CSV
    if (!csvScores.TryGetValue(csvKey, out var csvScore))
    {
        // Try date+1 (timezone differences)
        var keyPlus1 = $"{m.Date.AddDays(1):yyyy-MM-dd}|{Normalize(m.Home)}|{Normalize(m.Away)}";
        var keyMinus1 = $"{m.Date.AddDays(-1):yyyy-MM-dd}|{Normalize(m.Home)}|{Normalize(m.Away)}";
        if (!csvScores.TryGetValue(keyPlus1, out csvScore) && !csvScores.TryGetValue(keyMinus1, out csvScore))
        {
            Console.WriteLine($"  NOT IN CSV  | {m.Date:yyyy-MM-dd} | {m.Home} vs {m.Away}");
            notFoundInCSV++;
            continue;
        }
    }

    // Find in SofaScore
    if (!teamMap.TryGetValue(m.Home, out var homeId) || !teamMap.TryGetValue(m.Away, out var awayId) || homeId == 0 || awayId == 0)
    {
        Console.WriteLine($"  NO MAPPING  | {m.Date:yyyy-MM-dd} | {m.Home} vs {m.Away}");
        continue;
    }

    var yearForMatch = m.Date.Month >= 8 ? m.Date.Year : m.Date.Year - 1;
    var ssEvent = ssEvents.FirstOrDefault(e =>
        e.Date.Date == m.Date.Date &&
        ((e.HomeId == homeId && e.AwayId == awayId) || (e.HomeId == awayId && e.AwayId == homeId)));

    if (ssEvent.HomeId == 0 && ssEvent.AwayId == 0)
    {
        // Try date +/- 1 day
        ssEvent = ssEvents.FirstOrDefault(e =>
            (e.Date.Date == m.Date.AddDays(1).Date || e.Date.Date == m.Date.AddDays(-1).Date) &&
            ((e.HomeId == homeId && e.AwayId == awayId) || (e.HomeId == awayId && e.AwayId == homeId)));
    }

    if (ssEvent.HomeId == 0 && ssEvent.AwayId == 0)
    {
        Console.WriteLine($"  NOT IN SS   | {m.Date:yyyy-MM-dd} | {m.Home} vs {m.Away}");
        notFoundInSS++;
        continue;
    }

    totalMatched++;
    var csvHome = csvScore.HomeScore;
    var csvAway = csvScore.AwayScore;
    var ssHome = ssEvent.HomeScore;
    var ssAway = ssEvent.AwayScore;

    // Handle home/away swap (if SofaScore has them reversed)
    if (ssEvent.HomeId == awayId && ssEvent.AwayId == homeId)
    {
        (ssHome, ssAway) = (ssAway, ssHome);
    }

    if (csvHome == ssHome && csvAway == ssAway)
    {
        scoreMatch++;
        Console.WriteLine($"  OK           | {m.Date:yyyy-MM-dd} | {m.Home} vs {m.Away} | Score: {csvHome}-{csvAway}");
    }
    else
    {
        scoreMismatch++;
        var msg = $"{m.Date:yyyy-MM-dd} | {m.Home} vs {m.Away} | CSV: {csvHome}-{csvAway} | SS: {ssHome}-{ssAway}";
        mismatches.Add(msg);
        Console.WriteLine($"  MISMATCH     | {msg}");
    }
}

Console.WriteLine();
Console.WriteLine($"=== RESUMEN AUDITORÍA DE MARCADORES ===");
Console.WriteLine($"Total muestra SQLite: {sqliteMatches.Count}");
Console.WriteLine($"Encontrados en CSV: {sqliteMatches.Count - notFoundInCSV}");
Console.WriteLine($"Encontrados en SofaScore: {totalMatched}");
Console.WriteLine($"Marcador coincide: {scoreMatch}/{totalMatched} ({(totalMatched > 0 ? scoreMatch * 100.0 / totalMatched : 0):F1}%)");
Console.WriteLine($"Marcador difiere: {scoreMismatch}/{totalMatched}");
Console.WriteLine($"No encontrado en CSV: {notFoundInCSV}");
Console.WriteLine($"No encontrado en SofaScore: {notFoundInSS}");

if (mismatches.Count > 0)
{
    Console.WriteLine();
    Console.WriteLine("Mismatches:");
    foreach (var m in mismatches) Console.WriteLine($"  {m}");
}

conn.Close();
return;

static string Normalize(string s)
{
    return s.ToLower()
        .Replace("á", "a").Replace("é", "e").Replace("í", "i")
        .Replace("ó", "o").Replace("ú", "u").Replace("ñ", "n")
        .Replace(" ", "").Trim();
}
