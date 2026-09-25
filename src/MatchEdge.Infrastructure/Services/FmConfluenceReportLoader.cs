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
}
