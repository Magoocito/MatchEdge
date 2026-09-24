using Microsoft.Data.Sqlite;

namespace MatchEdge.Infrastructure.Services;

public sealed record FmSignalInsertResult(int Inserted, int Suspect, string? FirstMotivo);

public sealed record FmSignalRow(
    long Id,
    string SubjectType,
    string SubjectName,
    string? Market,
    double? Line,
    string? Direction);

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
    status TEXT NOT NULL,
    leakage_flag INTEGER NOT NULL DEFAULT 0
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
    params_json TEXT,
    status TEXT NOT NULL DEFAULT 'OK',
    motivo TEXT,
    source_timestamp_utc TEXT NOT NULL,
    FOREIGN KEY(snapshot_id) REFERENCES fm_snapshot(id)
);
CREATE INDEX IF NOT EXISTS ix_fm_signal_fixture ON fm_signal(fixture_id, snapshot_id);
CREATE INDEX IF NOT EXISTS ix_fm_signal_subject ON fm_signal(fixture_id, subject_type, subject_name);", ct);

        await EnsureColumnAsync(conn, "fm_snapshot", "leakage_flag", "INTEGER NOT NULL DEFAULT 0", ct);
        await EnsureColumnAsync(conn, "fm_signal", "params_json", "TEXT", ct);
        await EnsureColumnAsync(conn, "fm_signal", "status", "TEXT NOT NULL DEFAULT 'OK'", ct);
        await EnsureColumnAsync(conn, "fm_signal", "motivo", "TEXT", ct);
    }

    private static async Task EnsureColumnAsync(
        SqliteConnection conn, string table, string column, string ddl, CancellationToken ct)
    {
        var found = false;
        await using (var readerCmd = conn.CreateCommand())
        {
            readerCmd.CommandText = $"PRAGMA table_info({table});";
            await using var reader = await readerCmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                if (string.Equals(reader.GetString(1), column, StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                    break;
                }
            }
        }

        if (found) return;
        await using var alter = conn.CreateCommand();
        alter.CommandText = $"ALTER TABLE {table} ADD COLUMN {column} {ddl}";
        await alter.ExecuteNonQueryAsync(ct);
    }

    public async Task<long> InsertSnapshotAsync(
        string fixtureId, string tab, string url, DateTime sourceTimestampUtc,
        string rawPath, string rawSha256, string parserVersion, string status,
        bool leakageFlag = false,
        CancellationToken ct = default)
    {
        try { await EnsureOnceAsync(ct); }
        catch { ResetEnsureFailure(); throw; }
        await using var conn = Open();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
INSERT INTO fm_snapshot (fixture_id, tab, url, source_timestamp_utc, raw_path, raw_sha256, parser_version, status, leakage_flag)
VALUES ($fixtureId, $tab, $url, $ts, $rawPath, $sha, $parserVersion, $status, $leakage);
SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$fixtureId", fixtureId);
        cmd.Parameters.AddWithValue("$tab", tab);
        cmd.Parameters.AddWithValue("$url", url);
        cmd.Parameters.AddWithValue("$ts", sourceTimestampUtc.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        cmd.Parameters.AddWithValue("$rawPath", rawPath);
        cmd.Parameters.AddWithValue("$sha", rawSha256);
        cmd.Parameters.AddWithValue("$parserVersion", parserVersion);
        cmd.Parameters.AddWithValue("$status", status);
        cmd.Parameters.AddWithValue("$leakage", leakageFlag ? 1 : 0);
        var id = await cmd.ExecuteScalarAsync(ct);
        return Convert.ToInt64(id);
    }

    public async Task<FmSignalInsertResult> InsertSignalsAsync(
        long snapshotId,
        string fixtureId,
        IReadOnlyList<FmSignalDraft> signals,
        string venueScope,
        string competitionScope,
        DateTime sourceTimestampUtc,
        string? paramsJson = null,
        CancellationToken ct = default)
    {
        if (signals.Count == 0) return new FmSignalInsertResult(0, 0, null);

        try { await EnsureOnceAsync(ct); }
        catch { ResetEnsureFailure(); throw; }
        await using var conn = Open();
        await conn.OpenAsync(ct);
        await using var tx = (SqliteTransaction)await conn.BeginTransactionAsync(ct);
        var inserted = 0;
        var suspect = 0;
        string? firstMotivo = null;
        var ts = sourceTimestampUtc.ToString("yyyy-MM-dd HH:mm:ss.fff");

        foreach (var s in signals)
        {
            var (validationStatus, motivo) = FmSignalValidator.Validate(s);
            if (validationStatus == FmSignalValidator.StatusSuspect)
            {
                suspect++;
                firstMotivo ??= motivo;
            }

            await using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = @"
INSERT INTO fm_signal (snapshot_id, fixture_id, subject_type, subject_name, market, line, direction,
    hits, sample_size, observed_hit_rate, opp_hits, opp_sample_size, venue_scope, competition_scope,
    confidence_score, recent_values_json, params_json, status, motivo, source_timestamp_utc)
VALUES ($snapshotId, $fixtureId, $subjectType, $subjectName, $market, $line, $direction,
    $hits, $sample, $observed, $oppHits, $oppSample, $venueScope, $competitionScope,
    $confidence, $recent, $params, $validationStatus, $motivo, $ts);";
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
            cmd.Parameters.AddWithValue("$params", (object?)paramsJson ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$validationStatus", validationStatus);
            cmd.Parameters.AddWithValue("$motivo", (object?)motivo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$ts", ts);
            inserted += await cmd.ExecuteNonQueryAsync(ct);
        }

        await tx.CommitAsync(ct);
        return new FmSignalInsertResult(inserted, suspect, firstMotivo);
    }

    public async Task<IReadOnlyList<FmSignalRow>> GetLatestSignalsAsync(
        string fixtureId, CancellationToken ct = default)
    {
        try { await EnsureOnceAsync(ct); }
        catch { ResetEnsureFailure(); throw; }
        await using var conn = Open();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT s.id, s.subject_type, s.subject_name, s.market, s.line, s.direction
FROM fm_signal s
JOIN (
    SELECT tab, MAX(id) AS mid FROM fm_snapshot
    WHERE fixture_id = $fixtureId AND status IN ('OK', 'SUSPECT')
    GROUP BY tab
) l ON s.snapshot_id = l.mid
WHERE s.fixture_id = $fixtureId
ORDER BY s.id;";
        cmd.Parameters.AddWithValue("$fixtureId", fixtureId);
        var rows = new List<FmSignalRow>();
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            rows.Add(new FmSignalRow(
                reader.GetInt64(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.IsDBNull(3) ? null : reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetDouble(4),
                reader.IsDBNull(5) ? null : reader.GetString(5)));
        }
        return rows;
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
