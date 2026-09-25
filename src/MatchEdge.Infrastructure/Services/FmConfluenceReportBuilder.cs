using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MatchEdge.Infrastructure.Services;

// P7 E1: descriptive 360° confluence report. Reads only persisted tables
// (fm_signal, fm_outcome, fm_team_matches, fm_player_matches, fm_market_odds).
// No probability/EV/ranking is ever produced here: markets are ordered by
// sample size only (see SortCriteria).
public sealed record FmReportInput(
    IReadOnlyList<FmSignalDetailRow> Signals,
    IReadOnlyDictionary<long, FmOutcomeRecord> Outcomes,
    IReadOnlyList<FmTeamMatchRow> FixtureRows,
    IReadOnlyDictionary<long, IReadOnlyList<FmTeamMatchRow>> TeamRows,
    IReadOnlyDictionary<long, IReadOnlyList<FmPlayerMatchRow>> PlayerRows,
    IReadOnlyList<FmReportOddsRow> Odds,
    bool LeakageFlag);

public sealed record FmReportFixture(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("home")] string? Home,
    [property: JsonPropertyName("away")] string? Away,
    [property: JsonPropertyName("kickoffUtc")] string? KickoffUtc,
    [property: JsonPropertyName("competition")] string? Competition,
    [property: JsonPropertyName("leakage_flag")] bool LeakageFlag);

public sealed record FmReportBestWindow(
    [property: JsonPropertyName("window")] string Window,
    [property: JsonPropertyName("hits")] int? Hits,
    [property: JsonPropertyName("n")] int? N,
    [property: JsonPropertyName("rate")] double? Rate);

public sealed record FmReportFmReported(
    [property: JsonPropertyName("hits")] int? Hits,
    [property: JsonPropertyName("sample")] int? Sample,
    [property: JsonPropertyName("bestWindow")] FmReportBestWindow? BestWindow);

// own_windows entry: hits/n/rate/mean/median/min/max.
public sealed record FmReportWindow(
    [property: JsonPropertyName("hits")] int? Hits,
    [property: JsonPropertyName("n")] int N,
    [property: JsonPropertyName("rate")] double? Rate,
    [property: JsonPropertyName("mean")] double? Mean,
    [property: JsonPropertyName("median")] double? Median,
    [property: JsonPropertyName("min")] double? Min,
    [property: JsonPropertyName("max")] double? Max);

// venue_split entry: hits/n/rate/mean/median (no min/max, per brief).
public sealed record FmReportVenue(
    [property: JsonPropertyName("hits")] int? Hits,
    [property: JsonPropertyName("n")] int N,
    [property: JsonPropertyName("rate")] double? Rate,
    [property: JsonPropertyName("mean")] double? Mean,
    [property: JsonPropertyName("median")] double? Median);

// own_windows: {"last5","last10","all"} -> FmReportWindow | "INSUFFICIENT_SAMPLE"
// venue_split: {"home","away"} -> FmReportVenue | "NO_DATA"
public sealed record FmReportAttack(
    [property: JsonPropertyName("fm_reported")] FmReportFmReported? FmReported,
    [property: JsonPropertyName("own_windows")] Dictionary<string, object> OwnWindows,
    [property: JsonPropertyName("venue_split")] Dictionary<string, object> VenueSplit);

public sealed record FmReportOpponentContext(
    [property: JsonPropertyName("conceded_equivalent")] FmReportAttack ConcededEquivalent);

public sealed record FmReportDataQuality(
    [property: JsonPropertyName("suspect")] bool Suspect,
    [property: JsonPropertyName("source_conflict")] bool SourceConflict,
    [property: JsonPropertyName("motivo")] string Motivo);

public sealed record FmReportOdds(
    [property: JsonPropertyName("bookmaker")] string Bookmaker,
    [property: JsonPropertyName("side")] string? Side,
    [property: JsonPropertyName("value")] double Value,
    [property: JsonPropertyName("implied_prob")] double ImpliedProb,
    [property: JsonPropertyName("captured_at")] string CapturedAt);

public sealed record FmReportManualOdds(
    [property: JsonPropertyName("bookmaker")] string Bookmaker,
    [property: JsonPropertyName("value")] double? Value,
    [property: JsonPropertyName("implied_prob")] double? ImpliedProb,
    [property: JsonPropertyName("captured_at")] string? CapturedAt);

public sealed record FmReportMarket(
    [property: JsonPropertyName("market")] string Market,
    [property: JsonPropertyName("line")] double? Line,
    [property: JsonPropertyName("subject")] string Subject,
    [property: JsonPropertyName("subject_role")] string? SubjectRole,
    [property: JsonPropertyName("basis")] string Basis,
    [property: JsonPropertyName("team_attack")] FmReportAttack TeamAttack,
    [property: JsonPropertyName("opponent_context")] FmReportOpponentContext OpponentContext,
    [property: JsonPropertyName("overlap_flags")] List<string> OverlapFlags,
    [property: JsonPropertyName("data_quality")] FmReportDataQuality DataQuality,
    [property: JsonPropertyName("market_odds_fm")] List<FmReportOdds> MarketOddsFm,
    [property: JsonPropertyName("manual_odds")] FmReportManualOdds ManualOdds);

public sealed record FmReportPlayerSignal(
    [property: JsonPropertyName("market")] string Market,
    [property: JsonPropertyName("line")] double? Line,
    [property: JsonPropertyName("subject")] string Subject,
    [property: JsonPropertyName("basis")] string Basis,
    [property: JsonPropertyName("fm_reported")] FmReportFmReported? FmReported,
    [property: JsonPropertyName("own_windows")] Dictionary<string, object> OwnWindows,
    [property: JsonPropertyName("overlap_flags")] List<string> OverlapFlags,
    [property: JsonPropertyName("data_quality")] FmReportDataQuality DataQuality);

public sealed record FmReport(
    [property: JsonPropertyName("fixture")] FmReportFixture Fixture,
    [property: JsonPropertyName("sort_criteria")] string SortCriteria,
    [property: JsonPropertyName("markets")] List<FmReportMarket> Markets,
    [property: JsonPropertyName("player_signals")] List<FmReportPlayerSignal> PlayerSignals,
    [property: JsonPropertyName("notes")] List<string> Notes);

public static class FmConfluenceReportBuilder
{
    public const string SortCriteria = "sample_size_desc";

    public const string NoteDescriptive =
        "Esta sección es puramente descriptiva. Ninguna cifra debe interpretarse " +
        "como probabilidad de resultado futuro sin validación out-of-sample.";

    public const string StatusInsufficient = "INSUFFICIENT_SAMPLE";
    public const string StatusNoData = "NO_DATA";
    public const string WindowAll = "all";

    public const string BasisTeamOwn = "team_own_stats";
    public const string BasisMatchTotalGoals = "match_total_goals";
    public const string BasisNotReproducible = "not_reproducible";
    public const string BasisPlayerOwn = "player_own_stats";
    public const string BasisUnmapped = "unmapped";
    public const string BasisUnresolvedSubject = "subject_not_resolved";

    // market (after stripping home_/away_) -> fm_team_matches.team_stats_json stat.
    private static readonly Dictionary<string, string> TeamStats = new(StringComparer.Ordinal)
    {
        ["goals"] = "goals",
        ["corners"] = "corners",
        ["shots"] = "sh",
        ["shots_on_target"] = "sot",
        ["saves"] = "saves",
        ["goalkeeper_saves"] = "saves",
        ["cards"] = "cards",
        ["tackles"] = "tackles",
        ["offsides"] = "offsides",
        ["fouls_committed"] = "foulsC",
        ["fouls_drawn"] = "foulsD",
        ["shots_created"] = "shotsCreated",
        ["score_assist"] = "assists",
        ["assists"] = "assists"
    };

    // player market -> fm_player_matches.stats_json stat.
    private static readonly Dictionary<string, string> PlayerStats = new(StringComparer.Ordinal)
    {
        ["goals"] = "goals",
        ["shots"] = "sh",
        ["shots_on_target"] = "sot",
        ["score_assist"] = "assists",
        ["assists"] = "assists",
        ["offsides"] = "offsides",
        ["shots_created"] = "shotsCreated",
        ["chances_created"] = "chancesCreated",
        ["cards"] = "cards",
        ["yellow_cards"] = "yellowCards",
        ["penalties"] = "penalties"
    };

    public static FmReport Build(string fixtureId, FmReportInput input)
    {
        var fixtureRows = DedupByFixtureTeam(input.FixtureRows);
        var homeRow = fixtureRows.FirstOrDefault(r =>
            string.Equals(r.Location, "home", StringComparison.OrdinalIgnoreCase));
        var awayRow = fixtureRows.FirstOrDefault(r =>
            string.Equals(r.Location, "away", StringComparison.OrdinalIgnoreCase));

        // fm_team_matches stores the row's own name only as the *opponent* of the
        // other side, so home/away names come from the opposite row.
        var homeName = awayRow?.Opponent;
        var awayName = homeRow?.Opponent;
        var homeApid = homeRow?.TeamApid;
        var awayApid = awayRow?.TeamApid;
        var kickoff = homeRow?.TsUtc ?? awayRow?.TsUtc;
        var competition = homeRow?.League ?? awayRow?.League ?? input.Signals
            .Select(s => s.CompetitionScope)
            .FirstOrDefault(c => !string.IsNullOrWhiteSpace(c));

        var roles = new Dictionary<string, (long Apid, string Role)>(StringComparer.OrdinalIgnoreCase);
        if (homeApid is long ha && !string.IsNullOrWhiteSpace(homeName))
            roles[homeName!] = (ha, "home");
        if (awayApid is long aa && !string.IsNullOrWhiteSpace(awayName))
            roles[awayName!] = (aa, "away");

        var teamSignals = input.Signals
            .Where(s => s.SubjectType == "team" && !string.IsNullOrEmpty(s.Market))
            .GroupBy(s => (s.Market!, s.Line, s.SubjectName))
            .Select(g => g.OrderBy(s => s.Id).First())
            .ToList();
        var playerSignals = input.Signals
            .Where(s => s.SubjectType == "player" && !string.IsNullOrEmpty(s.Market))
            .GroupBy(s => (s.Market!, s.Line, s.SubjectName))
            .Select(g => g.OrderBy(s => s.Id).First())
            .ToList();

        var markets = new List<(FmReportMarket Market, int OwnN)>();
        foreach (var s in teamSignals)
        {
            roles.TryGetValue(s.SubjectName, out var role);
            var subjectRows = role.Apid != 0
                ? RowsFor(input, role.Apid)
                : Array.Empty<FmTeamMatchRow>();

            var (points, basis, skipReason) = BuildTeamSeries(
                subjectRows, s.Market!, s.Line, s.Direction);
            var ownWindows = BuildOwnWindows(points, "team");
            var venueSplit = BuildVenueSplit(subjectRows, s.Market!, s.Line, s.Direction);

            var opponentApid = role.Role == "home" ? awayApid : homeApid;
            var opponentName = role.Role == "home" ? awayName : homeName;
            var opponentRows = opponentApid is long oa ? RowsFor(input, oa) : Array.Empty<FmTeamMatchRow>();
            var (oppPoints, _, oppSkip) = BuildTeamSeries(
                opponentRows, s.Market!, s.Line, s.Direction);
            var opponentReported = FindSignal(teamSignals, opponentName, s.Market, s.Line) is { } oppSig
                ? BuildFmReported(oppSig)
                : null;
            var opponentAttack = new FmReportAttack(
                opponentReported,
                BuildOwnWindows(oppPoints, "team"),
                BuildVenueSplit(opponentRows, s.Market!, s.Line, s.Direction));

            var dataQuality = BuildDataQuality(s, input.Outcomes, skipReason ?? oppSkip);
            var market = new FmReportMarket(
                s.Market!,
                s.Line,
                s.SubjectName,
                role.Apid != 0 ? role.Role : null,
                basis,
                new FmReportAttack(BuildFmReported(s), ownWindows, venueSplit),
                new FmReportOpponentContext(opponentAttack),
                BuildOverlapFlags(subjectRows, opponentApid, opponentName),
                dataQuality,
                BuildFmOdds(input.Odds, s.Market!, s.Line, s.SubjectName),
                BuildManualOdds(input.Odds, s.Market!, s.Line));
            markets.Add((market, WindowN(ownWindows)));
        }

        var playerList = new List<(FmReportPlayerSignal Signal, int OwnN)>();
        foreach (var s in playerSignals)
        {
            roles.TryGetValue(s.SubjectName, out var role);
            var teamApid = role.Apid != 0
                ? role.Apid
                : FindPlayerTeamApid(input, s.SubjectName);
            var subjectRole = role.Role
                ?? (homeApid is long h && teamApid == h ? "home"
                    : awayApid is long a && teamApid == a ? "away"
                    : null);
            var playerRows = teamApid is long ta
                ? (input.PlayerRows.TryGetValue(ta, out var pr)
                    ? pr.Where(p => NameMatches(p.PlayerName, s.SubjectName)).ToList()
                    : new List<FmPlayerMatchRow>())
                : new List<FmPlayerMatchRow>();
            playerRows = DedupPlayerByFixture(playerRows);

            var (points, basis, skipReason) = BuildPlayerSeries(
                playerRows, s.Market!, s.Line, s.Direction);
            var ownWindows = BuildOwnWindows(points, "player");
            var opponentApid = subjectRole == "home" ? awayApid
                : subjectRole == "away" ? homeApid : null;
            var opponentName = subjectRole == "home" ? awayName
                : subjectRole == "away" ? homeName : null;
            var h2h = H2HDates(
                teamApid is long ta2 ? RowsFor(input, ta2) : Array.Empty<FmTeamMatchRow>(),
                opponentApid, opponentName);

            playerList.Add((new FmReportPlayerSignal(
                s.Market!,
                s.Line,
                s.SubjectName,
                basis,
                BuildFmReported(s),
                ownWindows,
                BuildPlayerOverlapFlags(playerRows, h2h),
                BuildDataQuality(s, input.Outcomes, skipReason)),
                WindowN(ownWindows)));
        }

        var fixture = new FmReportFixture(
            fixtureId,
            homeName,
            awayName,
            kickoff is null ? null : FormatUtc(kickoff.Value),
            competition,
            input.LeakageFlag);

        var orderedMarkets = markets
            .OrderByDescending(m => m.OwnN)
            .ThenBy(m => m.Market.Market, StringComparer.Ordinal)
            .ThenBy(m => m.Market.Line)
            .ThenBy(m => m.Market.Subject, StringComparer.Ordinal)
            .Select(m => m.Market)
            .ToList();
        var orderedPlayers = playerList
            .OrderByDescending(p => p.OwnN)
            .ThenBy(p => p.Signal.Market, StringComparer.Ordinal)
            .ThenBy(p => p.Signal.Line)
            .ThenBy(p => p.Signal.Subject, StringComparer.Ordinal)
            .Select(p => p.Signal)
            .ToList();

        return new FmReport(
            fixture,
            SortCriteria,
            orderedMarkets,
            orderedPlayers,
            new List<string> { NoteDescriptive });
    }

    private static IReadOnlyList<FmTeamMatchRow> RowsFor(FmReportInput input, long teamApid) =>
        input.TeamRows.TryGetValue(teamApid, out var rows) ? rows : Array.Empty<FmTeamMatchRow>();

    private static FmSignalDetailRow? FindSignal(
        IEnumerable<FmSignalDetailRow> signals, string? subject, string? market, double? line) =>
        subject is null
            ? null
            : signals.FirstOrDefault(s =>
                NameMatches(s.SubjectName, subject) &&
                string.Equals(s.Market, market, StringComparison.Ordinal) &&
                LineEquals(s.Line, line));

    private static bool NameMatches(string? a, string? b) =>
        a is not null && b is not null &&
        string.Equals(a.Trim(), b.Trim(), StringComparison.OrdinalIgnoreCase);

    private static bool LineEquals(double? a, double? b) =>
        a is null ? b is null : b is not null && Math.Abs(a.Value - b.Value) < 1e-9;

    private static List<FmTeamMatchRow> DedupByFixture(IReadOnlyList<FmTeamMatchRow> rows)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var result = new List<FmTeamMatchRow>();
        foreach (var row in rows) // store order: ts DESC, fixture_id
        {
            if (seen.Add(row.FixtureId)) result.Add(row);
        }
        return result;
    }

    // The fixture block needs BOTH sides of the current fixture, which share
    // fixture_id, so dedup here is per (fixture, team), not per fixture.
    private static List<FmTeamMatchRow> DedupByFixtureTeam(IReadOnlyList<FmTeamMatchRow> rows)
    {
        var seen = new HashSet<(string, long)>();
        var result = new List<FmTeamMatchRow>();
        foreach (var row in rows) // store order: location DESC, team_apid
        {
            if (seen.Add((row.FixtureId, row.TeamApid))) result.Add(row);
        }
        return result;
    }

    private static List<FmPlayerMatchRow> DedupPlayerByFixture(List<FmPlayerMatchRow> rows)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var result = new List<FmPlayerMatchRow>();
        foreach (var row in rows) // store order: ts DESC, player_apid
        {
            if (seen.Add(row.FixtureId)) result.Add(row);
        }
        return result;
    }

    private static (List<FmWindowPoint> Points, string Basis, string? SkipReason) BuildTeamSeries(
        IReadOnlyList<FmTeamMatchRow> rows, string market, double? line, string? direction)
    {
        var points = new List<FmWindowPoint>();
        string basis = BasisTeamOwn;
        string? skip = null;
        var deduped = DedupByFixture(rows);

        if (deduped.Count == 0)
            return (points, BasisUnresolvedSubject, "no fm_team_matches rows for this team");

        foreach (var row in deduped)
        {
            if (!TryTeamValue(market, row, out var value, out basis, out var reason))
            {
                skip ??= reason;
                continue;
            }
            points.Add(new FmWindowPoint(value, Met(line, direction, value)));
        }
        return (points, basis, skip);
    }

    private static (List<FmWindowPoint> Points, string Basis, string? SkipReason) BuildPlayerSeries(
        IReadOnlyList<FmPlayerMatchRow> rows, string market, double? line, string? direction)
    {
        var points = new List<FmWindowPoint>();
        if (rows.Count == 0)
            return (points, BasisUnresolvedSubject, "player not in fm_player_matches for this fixture's teams");

        string basis = BasisPlayerOwn;
        string? skip = null;
        foreach (var row in rows)
        {
            if (!TryPlayerValue(market, row, out var value, out basis, out var reason))
            {
                skip ??= reason;
                continue;
            }
            points.Add(new FmWindowPoint(value, Met(line, direction, value)));
        }
        return (points, basis, skip);
    }

    private static bool? Met(double? line, string? direction, double value)
    {
        if (line is null || direction is null) return null;
        return string.Equals(direction, "under", StringComparison.OrdinalIgnoreCase)
            ? value < line.Value
            : value > line.Value;
    }

    private static bool TryTeamValue(
        string market, FmTeamMatchRow row, out double value,
        out string basis, out string? reason)
    {
        value = 0;
        var m = market.Trim().ToLowerInvariant();

        if (m.StartsWith("total_", StringComparison.Ordinal))
        {
            var core = m["total_".Length..];
            if (core == "goals")
            {
                basis = BasisMatchTotalGoals;
                if (row.HGoals is int h && row.AGoals is int a)
                {
                    value = h + a;
                    reason = null;
                    return true;
                }
                reason = "hgoals/agoals are null in fm_team_matches";
                return false;
            }
            basis = BasisNotReproducible;
            reason = "match-total basis not reproducible from fm_team_matches " +
                     "(team_stats_json stores team-own stats only)";
            return false;
        }

        if (m.StartsWith("home_", StringComparison.Ordinal)) m = m["home_".Length..];
        else if (m.StartsWith("away_", StringComparison.Ordinal)) m = m["away_".Length..];

        if (!TeamStats.TryGetValue(m, out var stat))
        {
            basis = BasisUnmapped;
            reason = $"market '{market}' has no mapping to fm_team_matches.team_stats_json";
            return false;
        }

        basis = BasisTeamOwn;
        if (!TryReadStat(row.TeamStatsJson, stat, out value))
        {
            reason = $"stat '{stat}' missing or non-numeric in fm_team_matches.team_stats_json";
            return false;
        }
        reason = null;
        return true;
    }

    private static bool TryPlayerValue(
        string market, FmPlayerMatchRow row, out double value,
        out string basis, out string? reason)
    {
        value = 0;
        var m = market.Trim().ToLowerInvariant();
        if (m.StartsWith("home_", StringComparison.Ordinal)) m = m["home_".Length..];
        else if (m.StartsWith("away_", StringComparison.Ordinal)) m = m["away_".Length..];

        if (!PlayerStats.TryGetValue(m, out var stat))
        {
            basis = BasisUnmapped;
            reason = $"market '{market}' has no mapping to fm_player_matches.stats_json";
            return false;
        }

        basis = BasisPlayerOwn;
        if (!TryReadStat(row.StatsJson, stat, out value))
        {
            reason = $"stat '{stat}' missing or non-numeric in fm_player_matches.stats_json";
            return false;
        }
        reason = null;
        return true;
    }

    private static bool TryReadStat(string? json, string stat, out double value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(json)) return false;
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Object) return false;
            if (!doc.RootElement.TryGetProperty(stat, out var el))
            {
                el = default;
                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    if (string.Equals(prop.Name, stat, StringComparison.OrdinalIgnoreCase))
                    {
                        el = prop.Value;
                        break;
                    }
                }
                if (el.ValueKind == JsonValueKind.Undefined) return false;
            }
            switch (el.ValueKind)
            {
                case JsonValueKind.Number:
                    value = el.GetDouble();
                    return true;
                case JsonValueKind.String when double.TryParse(
                    el.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed):
                    value = parsed;
                    return true;
                default:
                    return false;
            }
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static Dictionary<string, object> BuildOwnWindows(
        IReadOnlyList<FmWindowPoint> points, string subjectType)
    {
        var windows = FmWindowCalculator.ComputeFromPoints(subjectType, points);
        var result = new Dictionary<string, object>(StringComparer.Ordinal);
        foreach (var w in windows)
        {
            var key = w.Window switch
            {
                "5" => "last5",
                "10" => "last10",
                _ => WindowAll
            };
            result[key] = w.Status == FmWindowCalculator.StatusOk
                ? new FmReportWindow(
                    w.Hits, w.N,
                    Round(w.ObservedRate, 4), Round(w.Mean, 4),
                    Round(w.Median, 4), Round(w.Min, 4), Round(w.Max, 4))
                : (object)StatusInsufficient;
        }
        return result;
    }

    private static Dictionary<string, object> BuildVenueSplit(
        IReadOnlyList<FmTeamMatchRow> rows, string market, double? line, string? direction)
    {
        var deduped = DedupByFixture(rows);
        var result = new Dictionary<string, object>(StringComparer.Ordinal);
        foreach (var location in new[] { "home", "away" })
        {
            var subset = deduped
                .Where(r => string.Equals(r.Location, location, StringComparison.OrdinalIgnoreCase))
                .ToList();
            var values = new List<(double V, bool? Met)>();
            foreach (var row in subset)
            {
                if (TryTeamValue(market, row, out var v, out _, out _))
                    values.Add((v, Met(line, direction, v)));
            }
            result[location] = values.Count == 0
                ? (object)StatusNoData
                : BuildVenue(values);
        }
        return result;
    }

    private static FmReportVenue BuildVenue(List<(double V, bool? Met)> values)
    {
        var ordered = values.Select(x => x.V).OrderBy(v => v).ToList();
        var hasMet = values.All(x => x.Met.HasValue);
        var hits = values.Count(x => x.Met == true);
        var median = ordered.Count % 2 == 1
            ? ordered[ordered.Count / 2]
            : (ordered[ordered.Count / 2 - 1] + ordered[ordered.Count / 2]) / 2.0;
        return new FmReportVenue(
            hasMet ? hits : null,
            ordered.Count,
            hasMet ? Round((double)hits / ordered.Count, 4) : null,
            Round(ordered.Average(), 4),
            Round(median, 4));
    }

    private static FmReportFmReported? BuildFmReported(FmSignalDetailRow s)
    {
        FmReportBestWindow? best = null;
        if (s.Hits is not null && s.SampleSize is not null)
        {
            foreach (var w in FmWindowCalculator.Compute(
                         s.SubjectType, s.RecentValuesJson, s.Line, s.Direction))
            {
                if (w.Status == FmWindowCalculator.StatusOk &&
                    w.Hits == s.Hits && w.N == s.SampleSize)
                {
                    best = new FmReportBestWindow(w.Window, w.Hits, w.N, Round(w.ObservedRate, 4));
                    break;
                }
            }
        }
        return new FmReportFmReported(s.Hits, s.SampleSize, best);
    }

    private static FmReportDataQuality BuildDataQuality(
        FmSignalDetailRow s,
        IReadOnlyDictionary<long, FmOutcomeRecord> outcomes,
        string? seriesReason)
    {
        outcomes.TryGetValue(s.Id, out var outcome);
        var motivo = new List<string>();
        if (!string.IsNullOrWhiteSpace(s.Motivo)) motivo.Add(s.Motivo!);
        if (outcome is not null && !string.IsNullOrWhiteSpace(outcome.Motivo))
            motivo.Add(outcome.Motivo!);
        if (!string.IsNullOrWhiteSpace(outcome?.UnavailableReason))
            motivo.Add(outcome!.UnavailableReason!);
        if (!string.IsNullOrWhiteSpace(seriesReason)) motivo.Add(seriesReason!);

        return new FmReportDataQuality(
            string.Equals(s.Status, "SUSPECT", StringComparison.OrdinalIgnoreCase),
            outcome?.SourceConflict ?? false,
            string.Join("; ", motivo));
    }

    private static List<string> BuildOverlapFlags(
        IReadOnlyList<FmTeamMatchRow> subjectRows, long? opponentApid, string? opponentName)
    {
        var h2h = H2HDates(subjectRows, opponentApid, opponentName);
        var last5 = LastDates(subjectRows, 5);
        return OverlapFlags("team", last5, h2h);
    }

    private static List<string> BuildPlayerOverlapFlags(
        IReadOnlyList<FmPlayerMatchRow> playerRows, HashSet<string> h2h)
    {
        var last5 = LastDates(playerRows.Select(r => r.TsUtc), 5);
        return OverlapFlags("player", last5, h2h);
    }

    private static List<string> OverlapFlags(
        string kind, List<string> last5, HashSet<string> h2h)
    {
        var flags = new List<string>();
        if (last5.Count == 0 || h2h.Count == 0) return flags;
        var intersection = last5.Count(d => h2h.Contains(d));
        if (last5.Count > 0 &&
            (double)intersection / last5.Count > FmConfluenceBuilder.OverlapThreshold)
        {
            flags.Add(
                $"{kind}_last5 shares >50% matches with H2H ({intersection}/{last5.Count})");
        }
        return flags;
    }

    private static HashSet<string> H2HDates(
        IReadOnlyList<FmTeamMatchRow> rows, long? opponentApid, string? opponentName)
    {
        var dates = new HashSet<string>(StringComparer.Ordinal);
        foreach (var row in rows)
        {
            var isH2h = (opponentApid is long oa && row.OpponentApid == oa) ||
                        (opponentName is not null && NameMatches(row.Opponent, opponentName));
            if (isH2h) dates.Add(row.TsUtc.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        }
        return dates;
    }

    private static List<string> LastDates(IReadOnlyList<FmTeamMatchRow> rows, int take) =>
        LastDates(rows.Select(r => r.TsUtc), take);

    private static List<string> LastDates(IEnumerable<DateTime> dates, int take) =>
        dates.Select(d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
            .Distinct(StringComparer.Ordinal)
            .Take(take)
            .ToList();

    private static List<FmReportOdds> BuildFmOdds(
        IReadOnlyList<FmReportOddsRow> odds, string market, double? line, string subject)
    {
        var rows = odds.Where(o =>
            string.Equals(o.Source, "fm", StringComparison.OrdinalIgnoreCase) &&
            NameMatches(o.Market, market) &&
            LineEquals(o.Line, line) &&
            (o.SubjectName is null || NameMatches(o.SubjectName, subject)));

        var latest = new Dictionary<(string Bookmaker, string Side), FmReportOddsRow>();
        foreach (var o in rows)
        {
            var key = (o.Bookmaker, o.Side);
            if (!latest.TryGetValue(key, out var current) ||
                string.CompareOrdinal(o.SourceTimestampUtc, current.SourceTimestampUtc) > 0 ||
                (string.CompareOrdinal(o.SourceTimestampUtc, current.SourceTimestampUtc) == 0 &&
                 o.Id > current.Id))
            {
                latest[key] = o;
            }
        }

        return latest.Values
            .OrderBy(o => o.BookmakerName ?? o.Bookmaker, StringComparer.Ordinal)
            .ThenBy(o => o.Side, StringComparer.Ordinal)
            .Select(o => new FmReportOdds(
                o.BookmakerName ?? o.Bookmaker,
                o.Side,
                o.OddsValue,
                Math.Round(1.0 / o.OddsValue, 3),
                FormatCapturedAt(o.SourceTimestampUtc)))
            .ToList();
    }

    private static FmReportManualOdds BuildManualOdds(
        IReadOnlyList<FmReportOddsRow> odds, string market, double? line)
    {
        var row = odds
            .Where(o =>
                string.Equals(o.Source, "manual", StringComparison.OrdinalIgnoreCase) &&
                NameMatches(o.Market, market) &&
                LineEquals(o.Line, line))
            .OrderBy(o => o.SourceTimestampUtc, StringComparer.Ordinal)
            .ThenBy(o => o.Id)
            .LastOrDefault();

        if (row is null) return new FmReportManualOdds("Betano", null, null, null);
        return new FmReportManualOdds(
            row.BookmakerName ?? row.Bookmaker,
            row.OddsValue,
            Math.Round(1.0 / row.OddsValue, 3),
            FormatCapturedAt(row.SourceTimestampUtc));
    }

    private static long? FindPlayerTeamApid(FmReportInput input, string? playerName)
    {
        foreach (var kv in input.PlayerRows)
        {
            if (kv.Value.Any(p => NameMatches(p.PlayerName, playerName)))
                return kv.Key;
        }
        return null;
    }

    private static int WindowN(Dictionary<string, object> windows) =>
        windows.TryGetValue(WindowAll, out var all) && all is FmReportWindow w ? w.N : 0;

    private static double? Round(double? value, int decimals) =>
        value is null ? null : Math.Round(value.Value, decimals);

    private static string FormatUtc(DateTime value) =>
        value.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture) + "Z";

    private static string FormatCapturedAt(string sourceTimestampUtc)
    {
        var normalized = sourceTimestampUtc.Trim().Replace(' ', 'T');
        return normalized.EndsWith("Z", StringComparison.Ordinal) ? normalized : normalized + "Z";
    }
}
