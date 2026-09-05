using MatchEdge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var dbPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "src", "MatchEdge.Api", "matchedge.db");
Console.WriteLine($"Database: {dbPath}");

var options = new DbContextOptionsBuilder<MatchEdgeDbContext>()
    .UseSqlite($"Data Source={dbPath}")
    .Options;

using var db = new MatchEdgeDbContext(options);

var odds = db.HistoricalOdds.AsNoTracking().ToList();
Console.WriteLine($"\nTotal odds in database: {odds.Count}");

var uniqueDates = odds.Select(o => o.MatchDate.Date).Distinct().OrderBy(d => d).ToList();
Console.WriteLine($"Unique match dates: {uniqueDates.Count}");

Console.WriteLine("\n--- Date Distribution by Year ---");
var byYear = odds.GroupBy(o => o.MatchDate.Year).OrderBy(g => g.Key);
foreach (var year in byYear)
{
    var dates = year.Select(o => o.MatchDate.Date).Distinct().Count();
    Console.WriteLine($"  {year.Key}: {year.Count()} matches, {dates} unique dates");
}

Console.WriteLine("\n--- Unique Teams ---");
var homeTeams = odds.Select(o => o.HomeTeamName).Distinct().OrderBy(t => t).ToList();
var awayTeams = odds.Select(o => o.AwayTeamName).Distinct().OrderBy(t => t).ToList();
var allTeams = homeTeams.Union(awayTeams).Distinct().OrderBy(t => t).ToList();
Console.WriteLine($"Total unique teams: {allTeams.Count}");
foreach (var team in allTeams)
{
    Console.WriteLine($"  - {team}");
}

Console.WriteLine("\n--- Sample Matches (first 10) ---");
foreach (var o in odds.OrderBy(o => o.MatchDate).Take(10))
{
    Console.WriteLine($"  {o.MatchDate:yyyy-MM-dd} | {o.HomeTeamName} vs {o.AwayTeamName} | {o.HomeWinOdds}/{o.DrawOdds}/{o.AwayWinOdds}");
}

Console.WriteLine("\n--- Odds by Season ---");
var bySeason = odds.GroupBy(o =>
{
    var month = o.MatchDate.Month;
    return month <= 6 ? $"{o.MatchDate.Year} Clausura" : $"{o.MatchDate.Year} Apertura";
}).OrderBy(g => g.Key);
foreach (var season in bySeason)
{
    Console.WriteLine($"  {season.Key}: {season.Count()} matches");
}
