using Microsoft.Data.Sqlite;
var conn = new SqliteConnection("Data Source=src/MatchEdge.Api/matchedge.db");
conn.Open();
var cmd = conn.CreateCommand();

// Get all unique team names from HistoricalOdds
var oddsTeams = new HashSet<string>();
cmd.CommandText = "SELECT DISTINCT HomeTeamName FROM HistoricalOdds UNION SELECT DISTINCT AwayTeamName FROM HistoricalOdds";
using (var r = cmd.ExecuteReader()) while (r.Read()) oddsTeams.Add(r.GetString(0));

// Get all mapped team names
var mappedTeams = new HashSet<string>();
cmd.CommandText = "SELECT SourceTeamName FROM TeamMappings WHERE Source = 'FootyStats'";
using (var r = cmd.ExecuteReader()) while (r.Read()) mappedTeams.Add(r.GetString(0));

Console.WriteLine("=== TEAMS IN ODDS WITHOUT MAPPING ===");
foreach (var t in oddsTeams.OrderBy(x => x))
{
    if (!mappedTeams.Contains(t))
        Console.WriteLine($"  UNMAPPED: {t}");
}

// Check how many Real Garcilaso matches exist
Console.WriteLine();
Console.WriteLine("=== REAL GARCILASO IN HISTORICAL_ODDS ===");
cmd.CommandText = "SELECT Id, MatchDate, HomeTeamName, AwayTeamName FROM HistoricalOdds WHERE HomeTeamName = 'Real Garcilaso' OR AwayTeamName = 'Real Garcilaso' ORDER BY MatchDate";
using (var r = cmd.ExecuteReader()) {
    int cnt = 0;
    while (r.Read()) { Console.WriteLine($"  {r.GetInt64(0)} | {r.GetDateTime(1):yyyy-MM-dd} | {r.GetString(2)} vs {r.GetString(3)}"); cnt++; }
    Console.WriteLine($"  Total: {cnt}");
}

// Check Deportivo Garcilaso
Console.WriteLine();
Console.WriteLine("=== DEPORTIVO GARCILASO IN HISTORICAL_ODDS ===");
cmd.CommandText = "SELECT Id, MatchDate, HomeTeamName, AwayTeamName FROM HistoricalOdds WHERE HomeTeamName = 'Deportivo Garcilaso' OR AwayTeamName = 'Deportivo Garcilaso' ORDER BY MatchDate";
using (var r = cmd.ExecuteReader()) {
    int cnt = 0;
    while (r.Read()) { Console.WriteLine($"  {r.GetInt64(0)} | {r.GetDateTime(1):yyyy-MM-dd} | {r.GetString(2)} vs {r.GetString(3)}"); cnt++; }
    Console.WriteLine($"  Total: {cnt}");
}

// Check UCV Moquegua
Console.WriteLine();
Console.WriteLine("=== UCV MOQUEGUA IN HISTORICAL_ODDS ===");
cmd.CommandText = "SELECT Id, MatchDate, HomeTeamName, AwayTeamName FROM HistoricalOdds WHERE HomeTeamName = 'UCV Moquegua' OR AwayTeamName = 'UCV Moquegua' ORDER BY MatchDate";
using (var r = cmd.ExecuteReader()) {
    int cnt = 0;
    while (r.Read()) { Console.WriteLine($"  {r.GetInt64(0)} | {r.GetDateTime(1):yyyy-MM-dd} | {r.GetString(2)} vs {r.GetString(3)}"); cnt++; }
    Console.WriteLine($"  Total: {cnt}");
}

// Check Carlos Manucci
Console.WriteLine();
Console.WriteLine("=== CARLOS MANUCCI IN HISTORICAL_ODDS ===");
cmd.CommandText = "SELECT Id, MatchDate, HomeTeamName, AwayTeamName FROM HistoricalOdds WHERE HomeTeamName = 'Carlos Manucci' OR AwayTeamName = 'Carlos Manucci' ORDER BY MatchDate";
using (var r = cmd.ExecuteReader()) {
    int cnt = 0;
    while (r.Read()) { Console.WriteLine($"  {r.GetInt64(0)} | {r.GetDateTime(1):yyyy-MM-dd} | {r.GetString(2)} vs {r.GetString(3)}"); cnt++; }
    Console.WriteLine($"  Total: {cnt}");
}

conn.Close();
