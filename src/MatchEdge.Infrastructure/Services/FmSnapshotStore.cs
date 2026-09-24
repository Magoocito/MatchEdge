using Microsoft.Data.Sqlite;

namespace MatchEdge.Infrastructure.Services;

public sealed class FmSnapshotStore
{
    private readonly string _connectionString;
    private readonly object _ensureLock = new();
    private Task? _ensureTask;

    public FmSnapshotStore(string connectionString)
    {
        _connectionString = connectionString;
    }

    private Task EnsureOnceAsync(CancellationToken ct)
    {
        lock (_ensureLock)
        {
            if (_ensureTask == null)
                _ensureTask = EnsureSchemaAsync(ct);
            return _ensureTask;
        }
    }

    private void ResetEnsureFailure()
    {
        lock (_ensureLock)
        {
            if (_ensureTask != null && _ensureTask.IsFaulted) _ensureTask = null;
        }
    }

    public async Task EnsureSchemaAsync(CancellationToken ct = default)
    {
        await using var conn = Open();
        await conn.OpenAsync(ct);
        await ExecAsync(conn, @"
CREATE TABLE IF NOT EXISTS fm_snapshot (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    fixture_id TEXT NOT NULL,
    tab TEXT NOT NULL,
    url TEXT NOT NULL,
    source_timestamp_utc TEXT NOT NULL,
    raw_path TEXT NOT NULL,
    raw_sha256 TEXT NOT NULL,
    parser_version TEXT NOT NULL,
    status TEXT NOT NULL
);
CREATE INDEX IF NOT EXISTS ix_fm_snapshot_fixture_tab ON fm_snapshot(fixture_id, tab, id);
CREATE TABLE IF NOT EXISTS fm_signal (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    snapshot_id INTEGER NOT NULL,
    fixture_id TEXT NOT NULL,
    subject_type TEXT NOT NULL,
    subject_name TEXT NOT NULL,
    market TEXT,
    line REAL,
    direction TEXT,
    hits INTEGER,
    sample_size INTEGER,
    observed_hit_rate REAL,
    opp_hits REAL,
    opp_sample_size INTEGER,
    venue_scope TEXT,
    competition_scope TEXT,
    confidence_score REAL,
    recent_values_json TEXT,
    source_timestamp_utc TEXT NOT NULL,
    FOREIGN KEY(snapshot_id) REFERENCES fm_snapshot(id)
);
CREATE INDEX IF NOT EXISTS ix_fm_signal_fixture ON fm_signal(fixture_id, snapshot_id);
CREATE INDEX IF NOT EXISTS ix_fm_signal_subject ON fm_signal(fixture_id, subject_type, subject_name);", ct);
    }

    public async Task<long> InsertSnapshotAsync(
        string fixtureId, string tab, string url, DateTime sourceTimestampUtc,
        string rawPath, string rawSha256, string parserVersion, string status,
        CancellationToken ct = default)
    {
        try { await EnsureOnceAsync(ct); }
        catch { ResetEnsureFailure(); throw; }
        await using var conn = Open();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
INSERT INTO fm_snapshot (fixture_id, tab, url, source_timestamp_utc, raw_path, raw_sha256, parser_version, status)
VALUES ($fixtureId, $tab, $url, $ts, $rawPath, $sha, $parserVersion, $status);
SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$fixtureId", fixtureId);
        cmd.Parameters.AddWithValue("$tab", tab);
        cmd.Parameters.AddWithValue("$url", url);
        cmd.Parameters.AddWithValue("$ts", sourceTimestampUtc.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        cmd.Parameters.AddWithValue("$rawPath", rawPath);
        cmd.Parameters.AddWithValue("$sha", rawSha256);
        cmd.Parameters.AddWithValue("$parserVersion", parserVersion);
        cmd.Parameters.AddWithValue("$status", status);
        var id = await cmd.ExecuteScalarAsync(ct);
        return Convert.ToInt64(id);
    }

    public async Task<int> InsertSignalsAsync(
        long snapshotId,
        string fixtureId,
        IReadOnlyList<FmSignalDraft> signals,
        string venueScope,
        string competitionScope,
        DateTime sourceTimestampUtc,
        CancellationToken ct = default)
    {
        if (signals.Count == 0) return 0;

        try { await EnsureOnceAsync(ct); }
        catch { ResetEnsureFailure(); throw; }
        await using var conn = Open();
        await conn.OpenAsync(ct);
        await using var tx = (SqliteTransaction)await conn.BeginTransactionAsync(ct);
        var inserted = 0;
        var ts = sourceTimestampUtc.ToString("yyyy-MM-dd HH:mm:ss.fff");

        foreach (var s in signals)
        {
            await using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = @"
INSERT INTO fm_signal (snapshot_id, fixture_id, subject_type, subject_name, market, line, direction,
    hits, sample_size, observed_hit_rate, opp_hits, opp_sample_size, venue_scope, competition_scope,
    confidence_score, recent_values_json, source_timestamp_utc)
VALUES ($snapshotId, $fixtureId, $subjectType, $subjectName, $market, $line, $direction,
    $hits, $sample, $observed, $oppHits, $oppSample, $venueScope, $competitionScope,
    $confidence, $recent, $ts);";
            cmd.Parameters.AddWithValue("$snapshotId", snapshotId);
            cmd.Parameters.AddWithValue("$fixtureId", fixtureId);
            cmd.Parameters.AddWithValue("$subjectType", s.SubjectType);
            cmd.Parameters.AddWithValue("$subjectName", s.SubjectName);
            cmd.Parameters.AddWithValue("$market", (object?)s.Market ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$line", (object?)s.Line ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$direction", (object?)s.Direction ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$hits", (object?)s.Hits ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$sample", (object?)s.SampleSize ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$observed", (object?)s.ObservedHitRate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$oppHits", (object?)s.OppHits ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$oppSample", (object?)s.OppSampleSize ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$venueScope", venueScope);
            cmd.Parameters.AddWithValue("$competitionScope", competitionScope);
            cmd.Parameters.AddWithValue("$confidence", (object?)s.ConfidenceScore ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$recent", (object?)s.RecentValuesJson ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$ts", ts);
            inserted += await cmd.ExecuteNonQueryAsync(ct);
        }

        await tx.CommitAsync(ct);
        return inserted;
    }

    public async Task<string?> GetLastSnapshotUrlAsync(string fixtureId, CancellationToken ct = default)
    {
        try { await EnsureOnceAsync(ct); }
        catch { ResetEnsureFailure(); throw; }
        await using var conn = Open();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT url FROM fm_snapshot WHERE fixture_id = $fixtureId ORDER BY id DESC LIMIT 1;";
        cmd.Parameters.AddWithValue("$fixtureId", fixtureId);
        var result = await cmd.ExecuteScalarAsync(ct);
        return result as string;
    }

    public async Task<Dictionary<string, int>> GetLastOkSignalCountsAsync(
        string fixtureId, string tab, CancellationToken ct = default)
    {
        try { await EnsureOnceAsync(ct); }
        catch { ResetEnsureFailure(); throw; }
        await using var conn = Open();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT s.market, COUNT(*)
FROM fm_signal s
JOIN fm_snapshot sn ON sn.id = s.snapshot_id
WHERE sn.fixture_id = $fixtureId AND sn.tab = $tab AND sn.status = 'OK'
  AND sn.id = (SELECT MAX(id) FROM fm_snapshot WHERE fixture_id = $fixtureId AND tab = $tab AND status = 'OK')
GROUP BY s.market;";
        cmd.Parameters.AddWithValue("$fixtureId", fixtureId);
        cmd.Parameters.AddWithValue("$tab", tab);
        var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var market = reader.IsDBNull(0) ? "(null)" : reader.GetString(0);
            counts[market] = reader.GetInt32(1);
        }
        return counts;
    }

    private SqliteConnection Open() => new(_connectionString);

    private static async Task ExecAsync(SqliteConnection conn, string sql, CancellationToken ct)
    {
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
