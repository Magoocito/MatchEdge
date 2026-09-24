using Microsoft.Data.Sqlite;

namespace MatchEdge.Infrastructure.Services;

public sealed record FmOutcomeDraft(
    long SignalId,
    string FixtureId,
    double? ActualValue,
    int? Hit,
    string Status,
    string Source,
    string? Motivo);

public sealed record FmOutcomeRecord(
    long SignalId,
    double? ActualValue,
    int? Hit,
    string Status,
    string Source,
    string? Motivo);

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
        await using var cmd = conn.CreateCommand();
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
    motivo TEXT
);
CREATE INDEX IF NOT EXISTS ix_fm_outcome_fixture ON fm_outcome(fixture_id);";
        await cmd.ExecuteNonQueryAsync(ct);
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
SELECT signal_id, actual_value, hit, status, source, motivo
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
                reader.IsDBNull(5) ? null : reader.GetString(5));
        }
        return result;
    }

    public async Task<int> InsertIgnoreAsync(
        IReadOnlyList<FmOutcomeDraft> drafts, DateTime resolvedAtUtc, CancellationToken ct = default)
    {
        if (drafts.Count == 0) return 0;
        try { await EnsureOnceAsync(ct); }
        catch { lock (_ensureLock) { _ensureTask = null; } throw; }
        await using var conn = Open();
        await conn.OpenAsync(ct);
        await using var tx = (SqliteTransaction)await conn.BeginTransactionAsync(ct);
        var inserted = 0;
        var ts = resolvedAtUtc.ToString("yyyy-MM-dd HH:mm:ss.fff");

        foreach (var d in drafts)
        {
            await using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = @"
INSERT OR IGNORE INTO fm_outcome (signal_id, fixture_id, resolved_at_utc, actual_value, hit, status, source, motivo)
VALUES ($signalId, $fixtureId, $ts, $actual, $hit, $status, $source, $motivo);";
            cmd.Parameters.AddWithValue("$signalId", d.SignalId);
            cmd.Parameters.AddWithValue("$fixtureId", d.FixtureId);
            cmd.Parameters.AddWithValue("$ts", ts);
            cmd.Parameters.AddWithValue("$actual", (object?)d.ActualValue ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$hit", (object?)d.Hit ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$status", d.Status);
            cmd.Parameters.AddWithValue("$source", d.Source);
            cmd.Parameters.AddWithValue("$motivo", (object?)d.Motivo ?? DBNull.Value);
            inserted += await cmd.ExecuteNonQueryAsync(ct);
        }

        await tx.CommitAsync(ct);
        return inserted;
    }

    private SqliteConnection Open() => new(_connectionString);
}
