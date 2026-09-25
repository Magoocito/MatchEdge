using System.Text.Json;
using System.Text.RegularExpressions;
using MatchEdge.Application.Clients.FootyMetrics;
using MatchEdge.Infrastructure.Clients;
using Microsoft.Extensions.Logging;

namespace MatchEdge.Infrastructure.Services;

public sealed record FmResolveRequest(string? Date, IReadOnlyList<string>? FixtureIds);

public sealed record FmOutcomeLine(
    long SignalId,
    string SubjectType,
    string SubjectName,
    string? Market,
    double? Line,
    string Status,
    double? ActualValue,
    int? Hit,
    string Source,
    string? Motivo,
    string? UnavailableReason = null,
    bool SourceConflict = false);

public sealed record FmFixtureOutcomeReport(
    string FixtureId,
    string Home,
    string Away,
    string KickoffUtc,
    int Signals,
    int Resolved,
    int AlreadyResolved,
    List<FmOutcomeLine> Lines);

public sealed record FmResolveReport(
    string Date,
    int FinishedFixtures,
    int NavigationsUsed,
    int Budget,
    List<FmFixtureOutcomeReport> Fixtures,
    List<string> Warnings);

public interface IFmOutcomeResolver
{
    Task<FmResolveReport> ResolveAsync(FmResolveRequest request, CancellationToken ct = default);
}

public sealed class FmOutcomeResolver : IFmOutcomeResolver
{
    public const string StatusResolved = "RESOLVED";
    public const string StatusNotPlayed = "NOT_PLAYED";
    public const string StatusUnavailable = "UNAVAILABLE";
    public const string StatusAmbiguous = "AMBIGUOUS";

    public const string ReasonNoHistoryElement = "NO_HISTORY_ELEMENT";
    public const string ReasonNoDateMatch = "NO_DATE_MATCH";
    public const string ReasonOther = "OTHER";

    private static readonly Regex StatsRow = new(
        @">([^<>]+)</span><span class=""text-xs font-medium text-text-secondary"">([^<>]+)</span><span class=""text-sm font-semibold tabular-nums text-text-primary"">([^<>]+)</span>",
        RegexOptions.Compiled);

    private static readonly Dictionary<string, (string Label, int Side)> TeamStatsMarkets =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["total_corners"] = ("Corners", 0),
            ["home_corners"] = ("Corners", 1),
            ["away_corners"] = ("Corners", 2),
            ["total_shots"] = ("Total shots", 0),
            ["home_shots"] = ("Total shots", 1),
            ["away_shots"] = ("Total shots", 2),
            ["total_shots_on_target"] = ("Shots on target", 0),
            ["home_shots_on_target"] = ("Shots on target", 1),
            ["away_shots_on_target"] = ("Shots on target", 2),
            ["fouls_committed"] = ("Fouls committed", 0),
            ["total_fouls"] = ("Fouls committed", 0),
            ["goalkeeper_saves"] = ("Goalkeeper saves", 0),
            ["total_saves"] = ("Goalkeeper saves", 0),
            ["home_saves"] = ("Goalkeeper saves", 1),
            ["away_saves"] = ("Goalkeeper saves", 2),
            ["offsides"] = ("Offsides", 0),
            ["total_offsides"] = ("Offsides", 0)
        };

    private const string TeamTrendsPath =
        "/api/front/trends/fixtures/{0}/teams?page=1&sort=hr-h&min_opp_hits=0&min_h2h_hits=0" +
        "&min_hit=70&min_matches=4&directions=over&parties=match%2Cteam" +
        "&markets=1%2C2%2C3%2C4%2C5%2C6%2C7%2C8%2C9%2C10&bookmakers=1%2C2%2C3%2C4%2C5&league_only=false";

    private const string PlayerTrendsPath =
        "/api/front/trends/fixtures/{0}/players?page=1&sort=hr-h&min_hit=60&min_matches=3" +
        "&lineup=all&lines=&pos=1%2C2%2C3%2C4&bookmakers=1%2C2%2C3%2C4%2C5" +
        "&markets=1%2C2%2C3%2C4%2C5%2C14%2C6%2C12%2C7%2C8%2C9%2C10%2C15%2C11%2C13&league_only=false";

    private readonly IFmFixtureResolver _fetcher;
    private readonly FootyMetricsBrowserManager _browserManager;
    private readonly FmSnapshotStore _signals;
    private readonly FmOutcomeStore _outcomes;
    private readonly ILogger<FmOutcomeResolver> _logger;

    public FmOutcomeResolver(
        IFmFixtureResolver fetcher,
        FootyMetricsBrowserManager browserManager,
        FmSnapshotStore signals,
        FmOutcomeStore outcomes,
        ILogger<FmOutcomeResolver> logger)
    {
        _fetcher = fetcher;
        _browserManager = browserManager;
        _signals = signals;
        _outcomes = outcomes;
        _logger = logger;
    }

    public async Task<FmResolveReport> ResolveAsync(
        FmResolveRequest request, CancellationToken ct = default)
    {
        var date = ParseDate(request.Date);
        var dateKey = date.ToString("yyyy-MM-dd");
        var warnings = new List<string>();

        var globalJson = await FetchUnderGateAsync($"/api/front/fixtures?date={dateKey}", ct);
        var fixtures = ParseGlobalFixtures(globalJson, warnings);
        var finished = fixtures.Where(f => f.Finished).ToList();

        if (request.FixtureIds is { Count: > 0 })
        {
            var wanted = new HashSet<string>(request.FixtureIds, StringComparer.Ordinal);
            finished = finished.Where(f => wanted.Contains(f.Id)).ToList();
        }

        var reports = new List<FmFixtureOutcomeReport>();
        foreach (var fixture in finished)
        {
            ct.ThrowIfCancellationRequested();
            reports.Add(await ResolveFixtureAsync(fixture, warnings, ct));
        }

        _logger.LogInformation(
            "Fm outcomes resolved for {Date}: {Count} finished fixture(s), {Resolved} newly resolved.",
            dateKey, finished.Count, reports.Sum(r => r.Resolved));

        return new FmResolveReport(
            dateKey,
            finished.Count,
            NavigationsUsed: 0,
            Budget: FmNavigator.MaxNavigationsPerRun,
            reports,
            warnings);
    }

    private async Task<FmFixtureOutcomeReport> ResolveFixtureAsync(
        GlobalFixture fixture, List<string> warnings, CancellationToken ct)
    {
        var signals = await _signals.GetLatestSignalsAsync(fixture.Id, ct);
        var existing = await _outcomes.GetByFixtureAsync(fixture.Id, ct);

        var lines = new List<FmOutcomeLine>();
        var drafts = new List<FmOutcomeDraft>();
        var resolvedAt = DateTime.UtcNow;

        if (signals.Count == 0)
        {
            warnings.Add($"{fixture.Id}: no signals (no OK/SUSPECT snapshot); nothing to resolve.");
            return new FmFixtureOutcomeReport(
                fixture.Id, fixture.Home, fixture.Away, fixture.Timestamp ?? "",
                0, 0, 0, lines);
        }

        // D0 order 1: history[] of a later snapshot (fixture already ingested by FM).
        var (historyHits, historyReasons) = await TryHistoryAsync(fixture, signals, warnings, ct);

        bool NeedsAttempt(FmSignalRow s) =>
            !historyHits.ContainsKey(s.Id) &&
            (!existing.TryGetValue(s.Id, out var prior) || prior.Status == StatusUnavailable);

        // D0 order 2: overview stats panel (server-rendered HTML) for leftovers.
        Dictionary<string, (double Home, double Away)>? stats = null;
        if (signals.Any(s => s.SubjectType == "team" && NeedsAttempt(s)))
        {
            var html = await FetchUnderGateAsync($"/fixtures/{fixture.Slug}", ct);
            stats = ParseStats(html);
        }

        var resolved = 0;
        var already = 0;
        foreach (var s in signals)
        {
            // A1: only final statuses are idempotent; UNAVAILABLE is retried every run.
            if (existing.TryGetValue(s.Id, out var prior) && prior.Status != StatusUnavailable)
            {
                already++;
                lines.Add(new FmOutcomeLine(
                    s.Id, s.SubjectType, s.SubjectName, s.Market, s.Line,
                    prior.Status, prior.ActualValue, prior.Hit, prior.Source,
                    prior.Motivo ?? "already resolved (idempotent skip)",
                    prior.UnavailableReason, prior.SourceConflict));
                continue;
            }

            FmOutcomeDraft draft;
            if (historyHits.TryGetValue(s.Id, out var fromHistory))
            {
                draft = fromHistory;
            }
            else
            {
                historyReasons.TryGetValue(s.Id, out var historyReason);
                draft = ComputeOutcome(
                    fixture, s, stats, historyReason.Code, historyReason.Detail, warnings);
            }

            drafts.Add(draft);
            lines.Add(new FmOutcomeLine(
                draft.SignalId, s.SubjectType, s.SubjectName, s.Market, s.Line,
                draft.Status, draft.ActualValue, draft.Hit, draft.Source, draft.Motivo,
                draft.UnavailableReason, false));
            if (draft.Status == StatusResolved) resolved++;
        }

        var written = await _outcomes.WriteAsync(drafts, resolvedAt, ct);
        if (written != drafts.Count)
            warnings.Add($"{fixture.Id}: {drafts.Count - written} outcome(s) kept final status (not overwritten).");

        // A4: cross-field consistency of FM-derived RESOLVED values (document, never fix).
        var conflictMarkets = DetectSourceConflicts(signals, drafts, existing);
        if (conflictMarkets.Count > 0)
        {
            var resolvedIds = new HashSet<long>(
                drafts.Where(d => d.Status == StatusResolved).Select(d => d.SignalId));
            foreach (var (id, rec) in existing)
                if (rec.Status == StatusResolved) resolvedIds.Add(id);
            var conflictIds = signals
                .Where(s => s.Market != null && conflictMarkets.Contains(s.Market) &&
                            resolvedIds.Contains(s.Id))
                .Select(s => s.Id)
                .ToList();
            var marked = await _outcomes.MarkSourceConflictsAsync(conflictIds, ct);
            warnings.Add(
                $"{fixture.Id}: source_conflict [{string.Join(", ", conflictMarkets.OrderBy(m => m))}] " +
                $"marked on {marked} outcome(s).");
            for (var i = 0; i < lines.Count; i++)
                if (conflictIds.Contains(lines[i].SignalId))
                    lines[i] = lines[i] with { SourceConflict = true };
        }

        return new FmFixtureOutcomeReport(
            fixture.Id, fixture.Home, fixture.Away, fixture.Timestamp ?? "",
            signals.Count, resolved, already, lines);
    }

    public static HashSet<string> DetectSourceConflicts(
        IReadOnlyList<FmSignalRow> signals,
        IReadOnlyList<FmOutcomeDraft> drafts,
        IReadOnlyDictionary<long, FmOutcomeRecord> existing)
    {
        var marketById = signals.Where(s => s.Market != null)
            .ToDictionary(s => s.Id, s => s.Market!);
        var values = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        void Take(long id, double? actual, string status)
        {
            if (status != StatusResolved || actual is null) return;
            if (marketById.TryGetValue(id, out var m)) values[m] = actual.Value;
        }
        foreach (var d in drafts) Take(d.SignalId, d.ActualValue, d.Status);
        foreach (var (id, rec) in existing) Take(id, rec.ActualValue, rec.Status);

        var conflicts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        bool Has(string m) => values.ContainsKey(m);
        double Get(string m) => values[m];

        // Brief example: home GK saves 0 while the rival scored.
        if (Has("home_saves") && Get("home_saves") == 0 &&
            Has("away_goals") && Get("away_goals") > 0)
        {
            conflicts.Add("home_saves");
            conflicts.Add("away_goals");
        }
        if (Has("away_saves") && Get("away_saves") == 0 &&
            Has("home_goals") && Get("home_goals") > 0)
        {
            conflicts.Add("away_saves");
            conflicts.Add("home_goals");
        }

        // Identity: opponent_saves + opponent_goals == side_shots_on_target.
        if (Has("home_saves") && Has("away_goals") && Has("away_shots_on_target") &&
            Math.Abs(Get("home_saves") + Get("away_goals") - Get("away_shots_on_target")) > 0.001)
        {
            conflicts.Add("home_saves");
            conflicts.Add("away_goals");
            conflicts.Add("away_shots_on_target");
        }
        if (Has("away_saves") && Has("home_goals") && Has("home_shots_on_target") &&
            Math.Abs(Get("away_saves") + Get("home_goals") - Get("home_shots_on_target")) > 0.001)
        {
            conflicts.Add("away_saves");
            conflicts.Add("home_goals");
            conflicts.Add("home_shots_on_target");
        }

        // Shots on target cannot exceed shots (total and per side).
        if (Has("total_shots_on_target") && Has("total_shots") &&
            Get("total_shots_on_target") > Get("total_shots"))
        {
            conflicts.Add("total_shots_on_target");
            conflicts.Add("total_shots");
        }
        if (Has("home_shots_on_target") && Has("home_shots") &&
            Get("home_shots_on_target") > Get("home_shots"))
        {
            conflicts.Add("home_shots_on_target");
            conflicts.Add("home_shots");
        }
        if (Has("away_shots_on_target") && Has("away_shots") &&
            Get("away_shots_on_target") > Get("away_shots"))
        {
            conflicts.Add("away_shots_on_target");
            conflicts.Add("away_shots");
        }

        return conflicts;
    }

    private static FmOutcomeDraft ComputeOutcome(
        GlobalFixture fixture, FmSignalRow s,
        Dictionary<string, (double Home, double Away)>? stats,
        string? historyReason, string? historyDetail,
        List<string> warnings)
    {
        if (s.SubjectType == "player")
        {
            var reason = string.IsNullOrEmpty(historyReason) ? ReasonOther : historyReason;
            var detail = !string.IsNullOrEmpty(historyDetail)
                ? $"{historyDetail} (stats panel is team-level, no player fallback)"
                : reason switch
                {
                    ReasonNoDateMatch =>
                        "history[] has no element within kickoff±1 day (FM ingestion lag); " +
                        "stats panel is team-level.",
                    ReasonNoHistoryElement =>
                        "signal has no history[] elements to match against; stats panel is team-level.",
                    _ =>
                        "D0 order 1 unresolved (fetch/parse/opponent mismatch); " +
                        "stats panel is team-level."
                };
            return new FmOutcomeDraft(
                s.Id, fixture.Id, null, null, StatusUnavailable, "none", detail, reason);
        }

        if (s.Line is null || s.Direction is null)
        {
            return new FmOutcomeDraft(
                s.Id, fixture.Id, null, null, StatusAmbiguous, "none",
                s.Line is null ? "signal has no line" : "signal has no direction");
        }

        double actual;
        string source;
        if (s.Market is "home_goals" or "away_goals" or "total_goals")
        {
            if (fixture.HomeGoals is null || fixture.AwayGoals is null)
            {
                return new FmOutcomeDraft(
                    s.Id, fixture.Id, null, null, StatusAmbiguous, "none",
                    "fixtures-api has no homeGoals/awayGoals");
            }
            actual = s.Market switch
            {
                "home_goals" => fixture.HomeGoals.Value,
                "away_goals" => fixture.AwayGoals.Value,
                _ => fixture.HomeGoals.Value + fixture.AwayGoals.Value
            };
            source = "fixtures-api";
        }
        else if (s.Market is "tackles" or "home_cards" or "away_cards" or "total_cards")
        {
            var reason = s.Market is "tackles"
                ? "ambiguous stats label (Total tackles vs Tackles won)"
                : "ambiguous stats label (Yellow cards vs Yellow+Red cards)";
            return new FmOutcomeDraft(
                s.Id, fixture.Id, null, null, StatusAmbiguous, "none", reason);
        }
        else if (s.Market != null &&
                 TeamStatsMarkets.TryGetValue(s.Market, out var mapping))
        {
            if (stats == null)
            {
                return new FmOutcomeDraft(
                    s.Id, fixture.Id, null, null, StatusUnavailable, "none",
                    "overview stats panel unavailable (fetch failed or no team signals)",
                    ReasonOther);
            }
            if (!stats.TryGetValue(mapping.Label, out var pair))
            {
                return new FmOutcomeDraft(
                    s.Id, fixture.Id, null, null, StatusUnavailable, "none",
                    $"label '{mapping.Label}' not present in stats panel", ReasonOther);
            }
            actual = mapping.Side switch { 1 => pair.Home, 2 => pair.Away, _ => pair.Home + pair.Away };
            source = "stats-panel";
        }
        else
        {
            return new FmOutcomeDraft(
                s.Id, fixture.Id, null, null, StatusUnavailable, "none",
                $"market '{s.Market}' not mapped to stats panel", ReasonOther);
        }

        var under = string.Equals(s.Direction, "under", StringComparison.OrdinalIgnoreCase);
        var hit = under
            ? actual < s.Line!.Value
            : actual > s.Line!.Value;
        return new FmOutcomeDraft(
            s.Id, fixture.Id, actual, hit ? 1 : 0, StatusResolved, source, null);
    }

    private async Task<(
        Dictionary<long, FmOutcomeDraft> Drafts,
        Dictionary<long, (string Code, string Detail)> Reasons)> TryHistoryAsync(
        GlobalFixture fixture, IReadOnlyList<FmSignalRow> signals,
        List<string> warnings, CancellationToken ct)
    {
        var drafts = new Dictionary<long, FmOutcomeDraft>();
        var reasons = new Dictionary<long, (string, string)>();

        if (fixture.Apid is null || fixture.Timestamp is null || fixture.Timestamp.Length < 10 ||
            !DateOnly.TryParse(fixture.Timestamp[..10], out var kickoff))
        {
            foreach (var s in signals)
                reasons[s.Id] = (ReasonOther, "fixture has no parseable kickoff timestamp.");
            return (drafts, reasons);
        }

        foreach (var subjectType in signals.Select(s => s.SubjectType).Distinct())
        {
            var subset = signals.Where(s => s.SubjectType == subjectType).ToList();

            string json;
            try
            {
                var path = subjectType == "player"
                    ? string.Format(PlayerTrendsPath, fixture.Apid)
                    : string.Format(TeamTrendsPath, fixture.Apid);
                json = await FetchUnderGateAsync(path, ct);
            }
            catch (Exception ex)
            {
                warnings.Add($"{fixture.Id}: D0 order 1 ({subjectType}) fetch failed: {ex.Message}");
                foreach (var s in subset)
                    reasons[s.Id] = (ReasonOther, $"trends fetch failed: {ex.Message}");
                continue;
            }

            List<JsonElement> rows;
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (!doc.RootElement.TryGetProperty("data", out var data) ||
                    data.ValueKind != JsonValueKind.Array)
                {
                    foreach (var s in subset)
                        reasons[s.Id] = (ReasonOther, "trends payload has no data array.");
                    continue;
                }
                rows = data.EnumerateArray().Select(e => e.Clone()).ToList();
            }
            catch (JsonException ex)
            {
                warnings.Add($"{fixture.Id}: D0 order 1 ({subjectType}) unparseable: {ex.Message}");
                foreach (var s in subset)
                    reasons[s.Id] = (ReasonOther, $"trends payload unparseable: {ex.Message}");
                continue;
            }

            foreach (var s in subset)
            {
                if (drafts.ContainsKey(s.Id)) continue;
                var row = rows.FirstOrDefault(r =>
                    string.Equals(GetString(r, "market") ?? GetString(r, "key"), s.Market,
                        StringComparison.OrdinalIgnoreCase));
                if (row.ValueKind != JsonValueKind.Object)
                {
                    reasons[s.Id] = (ReasonOther,
                        $"market row '{s.Market}' not present in trends payload.");
                    continue;
                }
                if (!row.TryGetProperty("history", out var hist) ||
                    hist.ValueKind != JsonValueKind.Array)
                {
                    reasons[s.Id] = (ReasonNoHistoryElement, "history[] missing for market row.");
                    continue;
                }

                var elements = hist.EnumerateArray().ToList();
                var (draft, reason) = MatchHistory(
                    fixture.Id, fixture.Home, fixture.Away, s, kickoff, elements);
                if (draft != null)
                    drafts[s.Id] = draft;
                else if (reason != null)
                    reasons[s.Id] = reason.Value;
            }
        }

        return (drafts, reasons);
    }

    // A2: pure history classification — public static for unit tests.
    public static (FmOutcomeDraft? Draft, (string Code, string Detail)? Reason) MatchHistory(
        string fixtureId, string home, string away, FmSignalRow s, DateOnly kickoff,
        List<JsonElement> elements)
    {
        if (elements.Count == 0)
            return (null, (ReasonNoHistoryElement, "history[] empty for market row."));

        FmOutcomeDraft? matched = null;
        var hasOppElement = false;
        var nearestDeltaDays = double.MaxValue;
        var nearestDate = "";
        var sameOppNearestDeltaDays = double.MaxValue;
        var sameOppNearestDate = "";
        var inWindowOpp = false;

        foreach (var el in elements)
        {
            var t = GetString(el, "t");
            if (t == null || t.Length < 10 || !DateOnly.TryParse(t[..10], out var elDate))
                continue;
            var delta = Math.Abs(elDate.DayNumber - kickoff.DayNumber);
            if (delta < nearestDeltaDays)
            {
                nearestDeltaDays = delta;
                nearestDate = elDate.ToString("yyyy-MM-dd");
            }

            var opp = el.TryGetProperty("opp", out var oppEl) && oppEl.ValueKind == JsonValueKind.Object
                ? GetString(oppEl, "name") : null;
            var isFixtureOpp = opp == home || opp == away;
            if (isFixtureOpp && delta < sameOppNearestDeltaDays)
            {
                hasOppElement = true;
                sameOppNearestDeltaDays = delta;
                sameOppNearestDate = elDate.ToString("yyyy-MM-dd");
            }
            if (delta > 1 || !isFixtureOpp) continue;
            inWindowOpp = true;

            if (s.SubjectType == "player")
            {
                var minutes = GetInt(el, "m");
                if (minutes is 0)
                {
                    matched = new FmOutcomeDraft(
                        s.Id, fixtureId, 0, null, StatusNotPlayed, "history",
                        "player did not play (minutes=0 in history element)");
                }
                else
                {
                    var value = GetDouble(el, "v");
                    matched = HitFromValue(fixtureId, s, value, "history");
                }
            }
            else
            {
                var value = GetDouble(el, "vt");
                matched = HitFromValue(fixtureId, s, value, "history");
            }
            break;
        }

        if (matched != null)
            return (matched, null);
        if (hasOppElement)
            return (null, (ReasonNoDateMatch,
                $"same-opponent element exists but none within kickoff±1d " +
                $"(nearest same-opponent t={sameOppNearestDate}, " +
                $"Δ={sameOppNearestDeltaDays:0}d; kickoff={kickoff:yyyy-MM-dd})."));
        if (inWindowOpp)
            return (null, (ReasonOther,
                "in-window fixture element matched but value could not be extracted."));
        return (null, (ReasonNoHistoryElement,
            $"no history element for this fixture: 0/{elements.Count} elements " +
            $"with opponents [{home} / {away}] " +
            $"(nearest t={nearestDate}, Δ={nearestDeltaDays:0}d from kickoff)."));
    }

    private static FmOutcomeDraft HitFromValue(
        string fixtureId, FmSignalRow s, double? value, string source)
    {
        if (value is null || s.Line is null || s.Direction is null)
        {
            return new FmOutcomeDraft(
                s.Id, fixtureId, null, null, StatusAmbiguous, "none",
                value is null ? "history element has no value" : "signal has no line/direction");
        }

        var under = string.Equals(s.Direction, "under", StringComparison.OrdinalIgnoreCase);
        var hit = under
            ? value.Value < s.Line.Value
            : value.Value > s.Line.Value;
        return new FmOutcomeDraft(
            s.Id, fixtureId, value, hit ? 1 : 0, StatusResolved, source, null);
    }

    public static Dictionary<string, (double Home, double Away)> ParseStats(string html)
    {
        // Values are rendered as `39<!-- -->%`; strip the comment artifacts so the
        // value/label spans can be matched without crossing tags.
        var clean = html.Replace("<!-- -->", string.Empty);
        var occurrences = new Dictionary<string, List<(double Home, double Away)>>(
            StringComparer.OrdinalIgnoreCase);

        foreach (Match m in StatsRow.Matches(clean))
        {
            var v1 = ParseStatValue(m.Groups[1].Value);
            var label = m.Groups[2].Value.Trim();
            var v2 = ParseStatValue(m.Groups[3].Value);
            if (v1 is null || v2 is null || label.Length == 0) continue;
            if (!occurrences.TryGetValue(label, out var list))
            {
                list = new List<(double, double)>();
                occurrences[label] = list;
            }
            list.Add((v1.Value, v2.Value));
        }

        // The server HTML renders the panel more than once (desktop + mobile, and
        // sometimes an empty placeholder). Identical repeats are fine; divergent
        // repeats resolve to the most frequent pair (deterministic), and a tie
        // drops the label so the caller reports AMBIGUOUS/UNAVAILABLE instead of guessing.
        var result = new Dictionary<string, (double Home, double Away)>(
            StringComparer.OrdinalIgnoreCase);
        foreach (var (label, list) in occurrences)
        {
            var best = list
                .GroupBy(p => p)
                .OrderByDescending(g => g.Count())
                .ThenBy(g => g.Key)
                .ToList();
            if (best.Count == 1 || best[0].Count() > best[1].Count())
                result[label] = best[0].Key;
        }
        return result;
    }

    private static double? ParseStatValue(string raw)
    {
        var cleaned = raw.Trim().TrimEnd('%');
        return double.TryParse(cleaned, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : null;
    }

    private static List<GlobalFixture> ParseGlobalFixtures(string json, List<string> warnings)
    {
        var result = new List<GlobalFixture>();
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.ValueKind != JsonValueKind.Array)
        {
            warnings.Add("fixtures-api: unexpected payload shape (not an array).");
            return result;
        }

        foreach (var league in doc.RootElement.EnumerateArray())
        {
            if (!league.TryGetProperty("fixtures", out var list) ||
                list.ValueKind != JsonValueKind.Array) continue;
            foreach (var fx in list.EnumerateArray())
            {
                var id = GetString(fx, "id");
                var slug = GetString(fx, "slug");
                if (id == null || slug == null) continue;
                var state = GetInt(fx, "state");
                var status = GetString(fx, "status");
                var home = fx.TryGetProperty("Home", out var h) && h.ValueKind == JsonValueKind.Object
                    ? GetString(h, "name") : null;
                var away = fx.TryGetProperty("Away", out var a) && a.ValueKind == JsonValueKind.Object
                    ? GetString(a, "name") : null;
                result.Add(new GlobalFixture(
                    id,
                    slug,
                    GetInt(fx, "apid"),
                    GetString(fx, "timestamp"),
                    state == 5 || status == "FT",
                    GetInt(fx, "homeGoals"),
                    GetInt(fx, "awayGoals"),
                    home ?? "?",
                    away ?? "?"));
            }
        }
        return result;
    }

    private async Task<string> FetchUnderGateAsync(string pathAndQuery, CancellationToken ct)
    {
        var gate = _browserManager.NavigationGate;
        await gate.WaitAsync(ct);
        try
        {
            return await _fetcher.FetchTextAsync(pathAndQuery, ct);
        }
        finally
        {
            gate.Release();
        }
    }

    private static DateOnly ParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return DateOnly.FromDateTime(DateTime.UtcNow);
        if (DateOnly.TryParseExact(value, "yyyy-MM-dd", out var date))
            return date;
        throw new FmResolverException("date must be yyyy-MM-dd.");
    }

    private static string? GetString(JsonElement el, string prop) =>
        el.ValueKind == JsonValueKind.Object &&
        el.TryGetProperty(prop, out var v) &&
        v.ValueKind == JsonValueKind.String ? v.GetString() : null;

    private static int? GetInt(JsonElement el, string prop)
    {
        if (el.ValueKind != JsonValueKind.Object || !el.TryGetProperty(prop, out var v)) return null;
        return v.ValueKind switch
        {
            JsonValueKind.Number when v.TryGetInt32(out var i) => i,
            JsonValueKind.String when int.TryParse(v.GetString(), out var i) => i,
            _ => null
        };
    }

    private static double? GetDouble(JsonElement el, string prop)
    {
        if (el.ValueKind != JsonValueKind.Object || !el.TryGetProperty(prop, out var v)) return null;
        return v.ValueKind switch
        {
            JsonValueKind.Number => v.GetDouble(),
            JsonValueKind.String when double.TryParse(
                v.GetString(), System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out var d) => d,
            _ => null
        };
    }

    private sealed record GlobalFixture(
        string Id,
        string Slug,
        int? Apid,
        string? Timestamp,
        bool Finished,
        int? HomeGoals,
        int? AwayGoals,
        string Home,
        string Away);
}
