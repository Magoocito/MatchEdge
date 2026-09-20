using System.Globalization;
using System.Text.RegularExpressions;
using System.Text.Json;
using MatchEdge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var dbPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "src", "MatchEdge.Api", "matchedge.db");
var dataDir = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "data");
var options = new DbContextOptionsBuilder<MatchEdgeDbContext>()
    .UseSqlite($"Data Source={dbPath}")
    .Options;

using var db = new MatchEdgeDbContext(options);
var allOdds = db.HistoricalOdds.AsNoTracking().ToList();

// Load all CSVs
var csvMatches = new List<(DateTime Date, string Home, string Away, int HomeScore, int AwayScore, double HomeOdds, double DrawOdds, double AwayOdds)>();
foreach (var csvFile in Directory.GetFiles(dataDir, "footystats-results-*.csv"))
{
    var lines = File.ReadAllLines(csvFile);
    for (int i = 1; i < lines.Length; i++)
    {
        var line = lines[i].Trim();
        if (string.IsNullOrEmpty(line)) continue;
        var values = line.Split(',');
        if (values.Length < 10) continue;

        if (DateTime.TryParse(values[0].Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) &&
            int.TryParse(values[5].Trim(), out var hs) &&
            int.TryParse(values[6].Trim(), out var as2) &&
            double.TryParse(values[7].Trim(), CultureInfo.InvariantCulture, out var ho) &&
            double.TryParse(values[8].Trim(), CultureInfo.InvariantCulture, out var do_) &&
            double.TryParse(values[9].Trim(), CultureInfo.InvariantCulture, out var ao))
        {
            csvMatches.Add((date, values[1].Trim(), values[3].Trim(), hs, as2, ho, do_, ao));
        }
    }
}

string Normalize(string name) => Regex.Replace(name.ToLowerInvariant().Trim()
    .Replace("á", "a").Replace("é", "e").Replace("í", "i")
    .Replace("ó", "o").Replace("ú", "u").Replace("ñ", "n"), @"\s+", " ");

// Take 25 random from SQLite
var random = new Random(42);
var sample = allOdds.OrderBy(_ => random.Next()).Take(25).OrderBy(o => o.MatchDate).ToList();

Console.WriteLine("=== AUDITORÍA DE CALIDAD DE DATOS ===");
Console.WriteLine($"Total en SQLite: {allOdds.Count}");
Console.WriteLine($"Total en CSVs: {csvMatches.Count}");
Console.WriteLine($"Muestra: {sample.Count}");
Console.WriteLine();

int matched = 0;
int oddsMatch = 0;
int oddsMismatch = 0;
int notInCsv = 0;
var discrepancies = new List<string>();

foreach (var s in sample)
{
    var csvCandidate = csvMatches.FirstOrDefault(c =>
        c.Date.Date == s.MatchDate.Date &&
        Normalize(c.Home) == Normalize(s.HomeTeamName) &&
        Normalize(c.Away) == Normalize(s.AwayTeamName));

    if (csvCandidate.Date == default)
    {
        Console.WriteLine($"  NOT_IN_CSV | {s.MatchDate:yyyy-MM-dd} | {s.HomeTeamName} vs {s.AwayTeamName}");
        notInCsv++;
        continue;
    }

    matched++;

    var hoDiff = Math.Abs(csvCandidate.HomeOdds - s.HomeWinOdds);
    var doDiff = Math.Abs(csvCandidate.DrawOdds - s.DrawOdds);
    var aoDiff = Math.Abs(csvCandidate.AwayOdds - s.AwayWinOdds);

    if (hoDiff < 0.01 && doDiff < 0.01 && aoDiff < 0.01)
    {
        oddsMatch++;
        Console.WriteLine($"  OK | {s.MatchDate:yyyy-MM-dd} | {s.HomeTeamName} vs {s.AwayTeamName} | CSV:{csvCandidate.HomeScore}-{csvCandidate.AwayScore} | Odds:{s.HomeWinOdds}/{s.DrawOdds}/{s.AwayWinOdds}");
    }
    else
    {
        oddsMismatch++;
        var msg = $"{s.MatchDate:yyyy-MM-dd} | {s.HomeTeamName} vs {s.AwayTeamName} | CSV odds: {csvCandidate.HomeOdds}/{csvCandidate.DrawOdds}/{csvCandidate.AwayOdds} vs SQLite: {s.HomeWinOdds}/{s.DrawOdds}/{s.AwayWinOdds}";
        discrepancies.Add(msg);
        Console.WriteLine($"  MISMATCH | {msg}");
    }
}

Console.WriteLine();
Console.WriteLine("=== RESULTADOS ===");
Console.WriteLine($"Encontrados en CSV: {matched}/25 ({matched*100/25}%)");
Console.WriteLine($"No encontrados en CSV: {notInCsv}/25");
Console.WriteLine($"Odds coinciden: {oddsMatch}/{matched}");
Console.WriteLine($"Odds difieren: {oddsMismatch}/{matched}");
Console.WriteLine();
if (discrepancies.Count > 0)
{
    Console.WriteLine("=== DISCREPANCIAS ===");
    foreach (var d in discrepancies) Console.WriteLine($"  {d}");
}
