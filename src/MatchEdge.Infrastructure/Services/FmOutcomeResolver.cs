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
    string? Motivo);

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

    private static readonly Regex StatsRow = new(
        @">(.*?)</span><span class=""text-xs font-medium text-text-secondary"">(.*?)</span><span class=""text-sm font-semibold tabular-nums text-text-primary"">(.*?)</span>",
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
        var historyHits = await TryHistoryAsync(fixture, signals, warnings, ct);

        // D0 order 2: overview stats panel (server-rendered HTML) for leftovers.
        Dictionary<string, (double Home, double Away)>? stats = null;
        var needsStats = signals.Any(s =>
            s.SubjectType == "team" &&
            !historyHits.ContainsKey(s.Id) &&
            !existing.ContainsKey(s.Id));
        if (needsStats)
        {
            var html = await FetchUnderGateAsync($"/fixtures/{fixture.Slug}", ct);
            stats = ParseStats(html);
        }

        var resolved = 0;
        var already = 0;
        foreach (var s in signals)
        {
            if (existing.TryGetValue(s.Id, out var prior))
            {
                already++;
                lines.Add(new FmOutcomeLine(
                    s.Id, s.SubjectType, s.SubjectName, s.Market, s.Line,
                    prior.Status, prior.ActualValue, prior.Hit, prior.Source,
                    prior.Motivo ?? "already resolved (idempotent skip)"));
                continue;
            }

            FmOutcomeDraft draft;
            if (historyHits.TryGetValue(s.Id, out var fromHistory))
            {
                draft = fromHistory;
            }
            else
            {
                draft = ComputeOutcome(fixture, s, stats, warnings);
            }

            drafts.Add(draft);
            lines.Add(new FmOutcomeLine(
                draft.SignalId, s.SubjectType, s.SubjectName, s.Market, s.Line,
                draft.Status, draft.ActualValue, draft.Hit, draft.Source, draft.Motivo));
            if (draft.Status == StatusResolved) resolved++;
        }

        var inserted = await _outcomes.InsertIgnoreAsync(drafts, resolvedAt, ct);
        if (inserted != drafts.Count)
            warnings.Add($"{fixture.Id}: {drafts.Count - inserted} outcome(s) already existed (race); kept originals.");

        return new FmFixtureOutcomeReport(
            fixture.Id, fixture.Home, fixture.Away, fixture.Timestamp ?? "",
            signals.Count, resolved, already, lines);
    }

    private static FmOutcomeDraft ComputeOutcome(
        GlobalFixture fixture, FmSignalRow s,
        Dictionary<string, (double Home, double Away)>? stats,
        List<string> warnings)
    {
        if (s.SubjectType == "player")
        {
            return new FmOutcomeDraft(
                s.Id, fixture.Id, null, null, StatusUnavailable, "none",
                "D0 order 1: fixture not in player history yet (FM ingestion lag); " +
                "stats panel is team-level. Player actual pending next history refresh.");
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
        else if (s.Market is "tackles")
        {
            return new FmOutcomeDraft(
                s.Id, fixture.Id, null, null, StatusAmbiguous, "none",
                "ambiguous stats label (Total tackles vs Tackles won)");
        }
        else if (s.Market != null &&
                 TeamStatsMarkets.TryGetValue(s.Market, out var mapping))
        {
            if (stats == null)
            {
                return new FmOutcomeDraft(
                    s.Id, fixture.Id, null, null, StatusUnavailable, "none",
                    "overview stats panel unavailable (fetch failed or no team signals)");
            }
            if (!stats.TryGetValue(mapping.Label, out var pair))
            {
                return new FmOutcomeDraft(
                    s.Id, fixture.Id, null, null, StatusUnavailable, "none",
                    $"label '{mapping.Label}' not present in stats panel");
            }
            actual = mapping.Side switch { 1 => pair.Home, 2 => pair.Away, _ => pair.Home + pair.Away };
            source = "stats-panel";
        }
        else
        {
            return new FmOutcomeDraft(
                s.Id, fixture.Id, null, null, StatusUnavailable, "none",
                $"market '{s.Market}' not mapped to stats panel");
        }

        var under = string.Equals(s.Direction, "under", StringComparison.OrdinalIgnoreCase);
        var hit = under
            ? actual < s.Line!.Value
            : actual > s.Line!.Value;
        return new FmOutcomeDraft(
            s.Id, fixture.Id, actual, hit ? 1 : 0, StatusResolved, source, null);
    }

    private async Task<Dictionary<long, FmOutcomeDraft>> TryHistoryAsync(
        GlobalFixture fixture, IReadOnlyList<FmSignalRow> signals,
        List<string> warnings, CancellationToken ct)
    {
        var result = new Dictionary<long, FmOutcomeDraft>();
        if (fixture.Apid is null || fixture.Timestamp is null || fixture.Timestamp.Length < 10)
            return result;
        var kickDate = fixture.Timestamp[..10];

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
                continue;
            }

            List<JsonElement> rows;
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (!doc.RootElement.TryGetProperty("data", out var data) ||
                    data.ValueKind != JsonValueKind.Array)
                    continue;
                rows = data.EnumerateArray().ToList();
            }
            catch (JsonException ex)
            {
                warnings.Add($"{fixture.Id}: D0 order 1 ({subjectType}) unparseable: {ex.Message}");
                continue;
            }

            foreach (var s in subset)
            {
                if (result.ContainsKey(s.Id)) continue;
                var row = rows.FirstOrDefault(r =>
                    string.Equals(GetString(r, "market") ?? GetString(r, "key"), s.Market,
                        StringComparison.OrdinalIgnoreCase));
                if (row.ValueKind != JsonValueKind.Object) continue;
                if (!row.TryGetProperty("history", out var hist) ||
                    hist.ValueKind != JsonValueKind.Array) continue;

                foreach (var el in hist.EnumerateArray())
                {
                    var t = GetString(el, "t");
                    if (t == null || !t.StartsWith(kickDate, StringComparison.Ordinal)) continue;
                    var opp = el.TryGetProperty("opp", out var oppEl) && oppEl.ValueKind == JsonValueKind.Object
                        ? GetString(oppEl, "name") : null;
                    if (opp != fixture.Home && opp != fixture.Away) continue;

                    if (subjectType == "player")
                    {
                        var minutes = GetInt(el, "m");
                        if (minutes is 0)
                        {
                            result[s.Id] = new FmOutcomeDraft(
                                s.Id, fixture.Id, 0, null, StatusNotPlayed, "history",
                                "player did not play (minutes=0 in history element)");
                        }
                        else
                        {
                            var value = GetDouble(el, "v");
                            result[s.Id] = HitFromValue(fixture.Id, s, value, "history");
                        }
                    }
                    else
                    {
                        var value = GetDouble(el, "vt");
                        result[s.Id] = HitFromValue(fixture.Id, s, value, "history");
                    }
                    break;
                }
            }
        }

        return result;
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
        var result = new Dictionary<string, (double Home, double Away)>(
            StringComparer.OrdinalIgnoreCase);
        var ambiguous = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var lastLabel = "";
        var lastValues = (0.0, 0.0);

        foreach (Match m in StatsRow.Matches(html))
        {
            var v1 = ParseStatValue(m.Groups[1].Value);
            var label = m.Groups[2].Value;
            var v2 = ParseStatValue(m.Groups[3].Value);
            if (v1 is null || v2 is null) continue;

            // Server HTML renders the panel twice (desktop + mobile columns);
            // identical repeats are fine, divergent repeats are ambiguous.
            if (label == lastLabel &&
                (Math.Abs(lastValues.Item1 - v1.Value) > 0.001 ||
                 Math.Abs(lastValues.Item2 - v2.Value) > 0.001))
            {
                ambiguous.Add(label);
                continue;
            }
            lastLabel = label;
            lastValues = (v1.Value, v2.Value);

            if (ambiguous.Contains(label)) continue;
            if (result.ContainsKey(label))
            {
                var existing = result[label];
                if (Math.Abs(existing.Home - v1.Value) > 0.001 ||
                    Math.Abs(existing.Away - v2.Value) > 0.001)
                {
                    result.Remove(label);
                    ambiguous.Add(label);
                }
                continue;
            }
            result[label] = (v1.Value, v2.Value);
        }

        foreach (var label in ambiguous)
            result.Remove(label);
        return result;
    }

    private static double? ParseStatValue(string raw)
    {
        var cleaned = raw.Replace("<!-- -->", string.Empty).Trim().TrimEnd('%');
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
