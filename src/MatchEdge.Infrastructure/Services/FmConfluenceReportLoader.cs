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

        // P12 B: a fixture that has not been played has no fm_team_matches row
        // for this fixture, so home/away are resolved from the signals'
        // venue_role plus the name -> apid map of older fixtures. Played
        // fixtures keep reading the rows (null sides, unchanged behaviour).
        var sides = await ResolveSidesAsync(store, signals, fixtureRows, ct);

        var apids = new SortedSet<long>(fixtureRows.Select(r => r.TeamApid));
        if (sides?.HomeApid is long homeApid) apids.Add(homeApid);
        if (sides?.AwayApid is long awayApid) apids.Add(awayApid);

        var teamRows = new Dictionary<long, IReadOnlyList<FmTeamMatchRow>>();
        var playerRows = new Dictionary<long, IReadOnlyList<FmPlayerMatchRow>>();
        foreach (var apid in apids)
        {
            teamRows[apid] = await store.GetTeamMatchesAsync(apid, null, null, false, ct);
            playerRows[apid] = await store.GetPlayerMatchesAsync(apid, null, null, null, ct);
        }

        return new FmReportInput(
            signals, outcomes, fixtureRows, teamRows, playerRows, odds, leakage, sides);
    }

    // P12 B: returns null when both sides already come from fixture rows.
    // Every field is looked up only when its row is missing - nothing is
    // guessed: a missing name/apid stays null and the report marks it.
    private static async Task<FmReportFixtureSides?> ResolveSidesAsync(
        FmSnapshotStore store,
        IReadOnlyList<FmSignalDetailRow> signals,
        IReadOnlyList<FmTeamMatchRow> fixtureRows,
        CancellationToken ct)
    {
        var homeRow = fixtureRows.FirstOrDefault(r =>
            string.Equals(r.Location, "home", StringComparison.OrdinalIgnoreCase));
        var awayRow = fixtureRows.FirstOrDefault(r =>
            string.Equals(r.Location, "away", StringComparison.OrdinalIgnoreCase));
        if (homeRow is not null && awayRow is not null) return null;

        var homeName = awayRow?.Opponent ?? SignalSideName(signals, "home");
        var awayName = homeRow?.Opponent ?? SignalSideName(signals, "away");
        if (homeName is null && awayName is null) return null;

        var homeApid = homeRow?.TeamApid
            ?? (homeName is null ? null : await store.GetTeamApidByNameAsync(homeName, ct));
        var awayApid = awayRow?.TeamApid
            ?? (awayName is null ? null : await store.GetTeamApidByNameAsync(awayName, ct));

        var competition = homeRow?.League ?? awayRow?.League
            ?? (homeApid is long ha ? await store.GetLatestLeagueAsync(ha, ct) : null)
            ?? (awayApid is long aa ? await store.GetLatestLeagueAsync(aa, ct) : null);

        return new FmReportFixtureSides(homeName, homeApid, awayName, awayApid, competition);
    }

    // The team subject of one side, taken from the signals captured for it.
    // Match-total signals (total_*) describe the fixture, so they never name
    // a side; ties break by name so the result is deterministic.
    private static string? SignalSideName(
        IReadOnlyList<FmSignalDetailRow> signals, string venueRole) =>
        signals
            .Where(s => s.SubjectType == "team" &&
                        !string.IsNullOrEmpty(s.Market) &&
                        !s.Market!.StartsWith("total_", StringComparison.Ordinal) &&
                        string.Equals(s.VenueRole, venueRole, StringComparison.OrdinalIgnoreCase) &&
                        !string.IsNullOrWhiteSpace(s.SubjectName))
            .Select(s => s.SubjectName.Trim())
            .GroupBy(n => n, StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key, StringComparer.Ordinal)
            .Select(g => g.Key)
            .FirstOrDefault();

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
