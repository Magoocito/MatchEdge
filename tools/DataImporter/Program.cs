using System.Globalization;
using MatchEdge.Domain.Odds;
using MatchEdge.Infrastructure.Data;
using MatchEdge.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

var baseDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
var dataDir = Path.Combine(baseDir, "data");
var dbPath = Path.Combine(baseDir, "src", "MatchEdge.Api", "matchedge.db");

Console.WriteLine($"Database: {dbPath}");
Console.WriteLine($"Data directory: {dataDir}");

var options = new DbContextOptionsBuilder<MatchEdgeDbContext>()
    .UseSqlite($"Data Source={dbPath}")
    .Options;

using var db = new MatchEdgeDbContext(options);
db.Database.EnsureCreated();

var csvFiles = Directory.GetFiles(dataDir, "footystats-results-*.csv");
Console.WriteLine($"\nFound {csvFiles.Length} CSV files to import:");

int totalImported = 0;
int totalSkipped = 0;

foreach (var csvFile in csvFiles)
{
    var fileName = Path.GetFileName(csvFile);
    Console.WriteLine($"\n--- Importing {fileName} ---");

    var lines = File.ReadAllLines(csvFile);
    if (lines.Length < 2)
    {
        Console.WriteLine("  Skipped: empty or header-only file");
        continue;
    }

    var header = lines[0];
    var headers = header.Split(',');
    var dateIdx = Array.FindIndex(headers, h => h.Trim().Equals("Date", StringComparison.OrdinalIgnoreCase));
    var homeTeamIdx = Array.FindIndex(headers, h => h.Trim().Equals("HomeTeam", StringComparison.OrdinalIgnoreCase));
    var awayTeamIdx = Array.FindIndex(headers, h => h.Trim().Equals("AwayTeam", StringComparison.OrdinalIgnoreCase));
    var homeScoreIdx = Array.FindIndex(headers, h => h.Trim().Equals("HomeScore", StringComparison.OrdinalIgnoreCase));
    var awayScoreIdx = Array.FindIndex(headers, h => h.Trim().Equals("AwayScore", StringComparison.OrdinalIgnoreCase));
    var homeOddsIdx = Array.FindIndex(headers, h => h.Trim().Equals("HomeOdds", StringComparison.OrdinalIgnoreCase));
    var drawOddsIdx = Array.FindIndex(headers, h => h.Trim().Equals("DrawOdds", StringComparison.OrdinalIgnoreCase));
    var awayOddsIdx = Array.FindIndex(headers, h => h.Trim().Equals("AwayOdds", StringComparison.OrdinalIgnoreCase));

    if (dateIdx < 0 || homeTeamIdx < 0 || awayTeamIdx < 0 || homeOddsIdx < 0)
    {
        Console.WriteLine($"  Skipped: missing required columns");
        continue;
    }

    int fileImported = 0;
    int fileSkipped = 0;

    for (int i = 1; i < lines.Length; i++)
    {
        var line = lines[i].Trim();
        if (string.IsNullOrEmpty(line)) continue;

        var values = line.Split(',');
        if (values.Length < headers.Length) continue;

        if (!DateTime.TryParse(values[dateIdx].Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var matchDate))
        {
            fileSkipped++;
            continue;
        }

        var homeTeam = values[homeTeamIdx].Trim();
        var awayTeam = values[awayTeamIdx].Trim();

        if (!double.TryParse(values[homeOddsIdx].Trim(), CultureInfo.InvariantCulture, out var homeOdds) ||
            !double.TryParse(values[drawOddsIdx].Trim(), CultureInfo.InvariantCulture, out var drawOdds) ||
            !double.TryParse(values[awayOddsIdx].Trim(), CultureInfo.InvariantCulture, out var awayOdds))
        {
            fileSkipped++;
            continue;
        }

        if (homeOdds <= 0 || drawOdds <= 0 || awayOdds <= 0)
        {
            fileSkipped++;
            continue;
        }

        int? homeScore = null, awayScore = null;
        if (homeScoreIdx >= 0 && int.TryParse(values[homeScoreIdx].Trim(), out var hs))
            homeScore = hs;
        if (awayScoreIdx >= 0 && int.TryParse(values[awayScoreIdx].Trim(), out var as2))
            awayScore = as2;

        var sourceMatchId = $"{matchDate:yyyyMMdd}_{homeTeam}_{awayTeam}".GetHashCode() & 0x7FFFFFFF;
        var existing = db.HistoricalOdds.FirstOrDefault(o => o.Source == "FootyStats" && o.SourceMatchId == sourceMatchId);
        if (existing != null)
        {
            fileSkipped++;
            continue;
        }

        db.HistoricalOdds.Add(new HistoricalOddsEntity
        {
            Source = "FootyStats",
            SourceMatchId = sourceMatchId,
            MatchDate = matchDate,
            TournamentId = 1,
            Round = 0,
            HomeTeamId = 0,
            HomeTeamName = homeTeam,
            AwayTeamId = 0,
            AwayTeamName = awayTeam,
            HomeWinOdds = homeOdds,
            DrawOdds = drawOdds,
            AwayWinOdds = awayOdds,
            CreatedAt = DateTime.UtcNow
        });

        fileImported++;
    }

    Console.WriteLine($"  Imported: {fileImported}, Skipped: {fileSkipped}");
    totalImported += fileImported;
    totalSkipped += fileSkipped;
}

db.SaveChanges();

Console.WriteLine($"\n=== Import Complete ===");
Console.WriteLine($"Total imported: {totalImported}");
Console.WriteLine($"Total skipped: {totalSkipped}");
Console.WriteLine($"Database: {dbPath}");
