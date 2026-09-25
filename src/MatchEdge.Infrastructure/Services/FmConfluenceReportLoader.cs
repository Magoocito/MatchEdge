namespace MatchEdge.Infrastructure.Services;

// P7 E2: loads everything the report needs from persisted tables only
// (0 navigations, read-only). Returns null when the fixture has no signals.
public static class FmConfluenceReportLoader
{
    public static async Task<FmReportInput?> LoadAsync(
        FmSnapshotStore store,
        FmOutcomeStore outcomeStore,
        string fixtureId,
        CancellationToken ct = default)
    {
        var signals = await store.GetLatestSignalDetailsAsync(fixtureId, ct);
        if (signals.Count == 0) return null;

        var outcomes = await outcomeStore.GetByFixtureAsync(fixtureId, ct);
        outcomes = await RekeyOutcomesAsync(store, fixtureId, signals, outcomes);
        var fixtureRows = await store.GetTeamMatchesByFixtureAsync(fixtureId, ct);
        var odds = await store.GetOddsDetailAsync(fixtureId, ct);
        var leakage = await store.HasLeakageAsync(fixtureId, ct);

        var teamRows = new Dictionary<long, IReadOnlyList<FmTeamMatchRow>>();
        var playerRows = new Dictionary<long, IReadOnlyList<FmPlayerMatchRow>>();
        foreach (var apid in fixtureRows.Select(r => r.TeamApid).Distinct().OrderBy(a => a))
        {
            teamRows[apid] = await store.GetTeamMatchesAsync(apid, null, null, false, ct);
            playerRows[apid] = await store.GetPlayerMatchesAsync(apid, null, null, null, ct);
        }

        return new FmReportInput(
            signals, outcomes, fixtureRows, teamRows, playerRows, odds, leakage);
    }

    // P7-T3 bugfix: fm_signal stores one copy per snapshot (ids drift with every
    // capture) while fm_outcome keeps the signal id from resolution time, so a
    // join by raw id silently dropped source_conflict/motivo from data_quality.
    // Outcomes are re-keyed onto the latest signal copy with the same identity
    // (subject_type, subject_name, market, line) - no data is invented.
    private static async Task<Dictionary<long, FmOutcomeRecord>> RekeyOutcomesAsync(
        FmSnapshotStore store,
        string fixtureId,
        IReadOnlyList<FmSignalDetailRow> signals,
        Dictionary<long, FmOutcomeRecord> outcomes,
        CancellationToken ct = default)
    {
        if (outcomes.Count == 0) return outcomes;

        var latestIds = new HashSet<long>(signals.Select(s => s.Id));
        if (outcomes.Keys.All(latestIds.Contains)) return outcomes;

        var reportIdByIdentity = signals
            .GroupBy(s => (s.SubjectType, s.SubjectName, s.Market, s.Line))
            .ToDictionary(g => g.Key, g => g.Min(s => s.Id));
        var identities = await store.GetSignalIdentitiesAsync(fixtureId, ct);

        var rekeyed = new Dictionary<long, FmOutcomeRecord>(outcomes);
        foreach (var (signalId, record) in outcomes)
        {
            if (latestIds.Contains(signalId)) continue;
            if (!identities.TryGetValue(signalId, out var identity)) continue;
            if (!reportIdByIdentity.TryGetValue(identity, out var target)) continue;
            if (!rekeyed.ContainsKey(target)) rekeyed[target] = record;
        }
        return rekeyed;
    }
}
