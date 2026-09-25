using Microsoft.Data.Sqlite;

namespace MatchEdge.Infrastructure.Services;

public sealed record FmOutcomeDraft(
    long SignalId,
    string FixtureId,
    double? ActualValue,
    int? Hit,
    string Status,
    string Source,
    string? Motivo,
    string? UnavailableReason = null);

public sealed record FmOutcomeRecord(
    long SignalId,
    double? ActualValue,
    int? Hit,
    string Status,
    string Source,
    string? Motivo,
    string? UnavailableReason,
    bool SourceConflict);

public sealed class FmOutcomeStore
{
    private readonly string _connectionString;
    private readonly object _ensureLock = new();
    private Task? _ensureTask;

    public FmOutcomeStore(string connectionString)
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

    public async Task EnsureSchemaAsync(CancellationToken ct = default)
    {
        await using var conn = Open();
        await conn.OpenAsync(ct);
        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS fm_outcome (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    signal_id INTEGER NOT NULL UNIQUE,
    fixture_id TEXT NOT NULL,
    resolved_at_utc TEXT NOT NULL,
    actual_value REAL,
    hit INTEGER,
    status TEXT NOT NULL,
    source TEXT NOT NULL,
    motivo TEXT,
    unavailable_reason TEXT,
    source_conflict INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS ix_fm_outcome_fixture ON fm_outcome(fixture_id);";
            await cmd.ExecuteNonQueryAsync(ct);
        }

        await EnsureColumnAsync(conn, "fm_outcome", "unavailable_reason", "TEXT", ct);
        await EnsureColumnAsync(conn, "fm_outcome", "source_conflict", "INTEGER NOT NULL DEFAULT 0", ct);
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

    public async Task<Dictionary<long, FmOutcomeRecord>> GetByFixtureAsync(
        string fixtureId, CancellationToken ct = default)
    {
        try { await EnsureOnceAsync(ct); }
        catch { lock (_ensureLock) { _ensureTask = null; } throw; }
        await using var conn = Open();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT signal_id, actual_value, hit, status, source, motivo, unavailable_reason, source_conflict
FROM fm_outcome WHERE fixture_id = $fixtureId;";
        cmd.Parameters.AddWithValue("$fixtureId", fixtureId);
        var result = new Dictionary<long, FmOutcomeRecord>();
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var signalId = reader.GetInt64(0);
            result[signalId] = new FmOutcomeRecord(
                signalId,
                reader.IsDBNull(1) ? null : reader.GetDouble(1),
                reader.IsDBNull(2) ? null : reader.GetInt32(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.IsDBNull(5) ? null : reader.GetString(5),
                reader.IsDBNull(6) ? null : reader.GetString(6),
                !reader.IsDBNull(7) && reader.GetInt32(7) != 0);
        }
        return result;
    }

    public async Task<int> WriteAsync(
        IReadOnlyList<FmOutcomeDraft> drafts, DateTime resolvedAtUtc, CancellationToken ct = default)
    {
        if (drafts.Count == 0) return 0;
        try { await EnsureOnceAsync(ct); }
        catch { lock (_ensureLock) { _ensureTask = null; } throw; }
        await using var conn = Open();
        await conn.OpenAsync(ct);
        await using var tx = (SqliteTransaction)await conn.BeginTransactionAsync(ct);
        var written = 0;
        var ts = resolvedAtUtc.ToString("yyyy-MM-dd HH:mm:ss.fff");

        foreach (var d in drafts)
        {
            await using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            // A1: UNAVAILABLE rows are retryable (upsert over them); final statuses
            // (RESOLVED/NOT_PLAYED/AMBIGUOUS) keep INSERT OR IGNORE semantics.
            cmd.CommandText = @"
INSERT INTO fm_outcome (signal_id, fixture_id, resolved_at_utc, actual_value, hit, status, source, motivo,
    unavailable_reason, source_conflict)
VALUES ($signalId, $fixtureId, $ts, $actual, $hit, $status, $source, $motivo, $reason, 0)
ON CONFLICT(signal_id) DO UPDATE SET
    resolved_at_utc = excluded.resolved_at_utc,
    actual_value = excluded.actual_value,
    hit = excluded.hit,
    status = excluded.status,
    source = excluded.source,
    motivo = excluded.motivo,
    unavailable_reason = excluded.unavailable_reason,
    source_conflict = excluded.source_conflict
WHERE fm_outcome.status = 'UNAVAILABLE';";
            cmd.Parameters.AddWithValue("$signalId", d.SignalId);
            cmd.Parameters.AddWithValue("$fixtureId", d.FixtureId);
            cmd.Parameters.AddWithValue("$ts", ts);
            cmd.Parameters.AddWithValue("$actual", (object?)d.ActualValue ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$hit", (object?)d.Hit ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$status", d.Status);
            cmd.Parameters.AddWithValue("$source", d.Source);
            cmd.Parameters.AddWithValue("$motivo", (object?)d.Motivo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$reason", (object?)d.UnavailableReason ?? DBNull.Value);
            written += await cmd.ExecuteNonQueryAsync(ct);
        }

        await tx.CommitAsync(ct);
        return written;
    }

    public async Task<int> MarkSourceConflictsAsync(
        IReadOnlyCollection<long> signalIds, CancellationToken ct = default)
    {
        if (signalIds.Count == 0) return 0;
        try { await EnsureOnceAsync(ct); }
        catch { lock (_ensureLock) { _ensureTask = null; } throw; }
        await using var conn = Open();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        var ids = string.Join(",", signalIds.Select(id => id.ToString()));
        cmd.CommandText = $"UPDATE fm_outcome SET source_conflict = 1 WHERE signal_id IN ({ids});";
        return await cmd.ExecuteNonQueryAsync(ct);
    }

    private SqliteConnection Open() => new(_connectionString);
}
