using Microsoft.Data.Sqlite;

var conn = new SqliteConnection("Data Source=src/MatchEdge.Api/matchedge.db");
conn.Open();

Console.WriteLine("=== RE-CREATING TEAM MAPPINGS ===");

var mappings = new (string Name, int SsId, string SsName)[]
{
    ("ADT", 335557, "Asociación Deportiva Tarma"),
    ("Academia Cantolao", 245083, "Academia Deportiva Cantolao"),
    ("Alianza Atlético", 2307, "Alianza Atlético de Sullana"),
    ("Alianza Lima", 2311, "Alianza Lima"),
    ("Alianza Universidad", 306660, "Alianza Universidad"),
    ("Atlético Grau", 282538, "Club Atlético Grau"),
    ("Ayacucho", 33894, "Ayacucho FC"),
    ("Carlos Manucci", 252245, "Carlos A. Mannucci"),
    ("Cienciano", 2301, "Cienciano"),
    ("Comerciantes Unidos", 213609, "Comerciantes Unidos"),
    ("César Vallejo", 5281, "Universidad César Vallejo"),
    ("Deportivo Binacional", 275839, "Deportivo Binacional"),
    ("Deportivo Garcilaso", 458584, "Deportivo Garcilaso"),
    ("Deportivo Municipal", 7031, "Deportivo Municipal"),
    ("FC Cajamarca", 87854, "Universidad Técnica de Cajamarca"),
    ("Juan Pablo II College", 511206, "CD Juan Pablo II"),
    ("Los Chankas", 252254, "Los Chankas CYC"),
    ("Melgar", 2308, "Melgar"),
    ("Real Garcilaso", 63760, "Cusco FC"),  // CORRECTED: Real Garcilaso = Cusco FC, NOT Deportivo Garcilaso
    ("Sport Boys", 2312, "Sport Boys"),
    ("Sport Huancayo", 33895, "Sport Huancayo"),
    ("Sporting Cristal", 2302, "Club Sporting Cristal"),
    ("UCV Moquegua", 0, "Unknown"),
    ("UTC Cajamarca", 87854, "Universidad Técnica de Cajamarca"),
    ("Universitario", 2305, "Universitario de Deportes"),
    ("Unión Comercio", 48431, "Unión Comercio"),
};

foreach (var (name, ssId, ssName) in mappings)
{
    if (ssId == 0) continue;
    var cmd = conn.CreateCommand();
    cmd.CommandText = @"
        INSERT OR REPLACE INTO TeamMappings (Source, SourceTeamId, SourceTeamName, SofaScoreTeamId, SofaScoreTeamName, Confidence, CreatedAt)
        VALUES ('FootyStats', @sourceId, @sourceName, @ssId, @ssName, 1.0, @now)";
    cmd.Parameters.AddWithValue("@sourceId", name.ToLower().GetHashCode() & 0x7FFFFFFF);
    cmd.Parameters.AddWithValue("@sourceName", name);
    cmd.Parameters.AddWithValue("@ssId", ssId);
    cmd.Parameters.AddWithValue("@ssName", ssName);
    cmd.Parameters.AddWithValue("@now", DateTime.UtcNow.ToString("o"));
    cmd.ExecuteNonQuery();
}
Console.WriteLine($"Created {mappings.Length} team mappings (including corrected Real Garcilaso -> Cusco FC)");

// Delete Carlos Manucci 2025
Console.WriteLine();
Console.WriteLine("=== EXCLUDING CARLOS MANUCCI 2025 ===");
var cmd2 = conn.CreateCommand();
cmd2.CommandText = @"
    DELETE FROM HistoricalOdds 
    WHERE (HomeTeamName = 'Carlos Manucci' OR AwayTeamName = 'Carlos Manucci')
    AND MatchDate >= '2025-01-01' AND MatchDate < '2026-01-01'";
var deleted = cmd2.ExecuteNonQuery();
Console.WriteLine($"Deleted {deleted} Carlos Manucci 2025 records");

// Final counts
Console.WriteLine();
Console.WriteLine("=== FINAL COUNTS ===");
cmd2 = conn.CreateCommand();
cmd2.CommandText = "SELECT COUNT(*) FROM HistoricalOdds";
Console.WriteLine($"HistoricalOdds: {cmd2.ExecuteScalar()}");

cmd2 = conn.CreateCommand();
cmd2.CommandText = "SELECT COUNT(*) FROM TeamMappings";
Console.WriteLine($"TeamMappings: {cmd2.ExecuteScalar()}");

cmd2 = conn.CreateCommand();
cmd2.CommandText = @"
    SELECT strftime('%Y', MatchDate) as Year, COUNT(*) 
    FROM HistoricalOdds 
    GROUP BY strftime('%Y', MatchDate) 
    ORDER BY Year";
using (var reader = cmd2.ExecuteReader())
{
    Console.WriteLine("By year:");
    while (reader.Read())
    {
        Console.WriteLine($"  {reader.GetString(0)}: {reader.GetInt32(1)}");
    }
}

conn.Close();
