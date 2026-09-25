using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using MatchEdge.Application.Clients.FootyMetrics;
using MatchEdge.Infrastructure.Clients;
using Microsoft.Extensions.Logging;

namespace MatchEdge.Infrastructure.Services;

public sealed class FmFixtureSnapshotService : IFmFixtureSnapshotService
{
    public const string HtmlParserVersion = "html-v1";
    private const string SiteOrigin = "https://www.footymetrics.com";
    private static readonly TimeSpan NavTimeout = TimeSpan.FromSeconds(20);

    private static readonly (string Tab, string ReadyPattern, FmReadySignalKind Kind, string Ext)[] Tabs =
    {
        ("overview", "h1", FmReadySignalKind.DomSelector, "html"),
        ("player-trends", @"/api/front/trends/fixtures/\d+/players", FmReadySignalKind.ResponsePattern, "json"),
        ("team-trends", @"/api/front/trends/fixtures/\d+/teams", FmReadySignalKind.ResponsePattern, "json")
    };

    private readonly FmNavigator _navigator;
    private readonly FootyMetricsBrowserManager _browserManager;
    private readonly FootyMetricsBrowserCollector _collector;
    private readonly FmSnapshotStore _store;
    private readonly ILogger<FmFixtureSnapshotService> _logger;

    public FmFixtureSnapshotService(
        FmNavigator navigator,
        FootyMetricsBrowserManager browserManager,
        FootyMetricsBrowserCollector collector,
        FmSnapshotStore store,
        ILogger<FmFixtureSnapshotService> logger)
    {
        _navigator = navigator;
        _browserManager = browserManager;
        _collector = collector;
        _store = store;
        _logger = logger;
    }

    public async Task<FmSnapshotOutcome> SnapshotAsync(FmFixtureRef fixture, CancellationToken ct = default)
    {
        var baseUrl = fixture.Url.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? fixture.Url
            : SiteOrigin + fixture.Url;

        var tabs = new List<FmTabOutcome>();
        var rawPaths = new List<string>();
        var warnings = new List<string>();
        var trendUrls = new Dictionary<string, string>();

        foreach (var (tab, readyPattern, kind, ext) in Tabs)
        {
            ct.ThrowIfCancellationRequested();
            var tabUrl = tab == "overview" ? baseUrl : $"{baseUrl}?tab={tab}";
            var sourceTs = DateTime.UtcNow;

            try
            {
                Func<FmNavOutcome, CancellationToken, Task<FmNavOutcome>>? capture =
                    kind == FmReadySignalKind.DomSelector
                        ? async (o, _) => new FmNavOutcome(
                            o.Url, await PageHtmlAsync(), o.ResponseUrl)
                        : null;

                var outcome = await _navigator.GoToAsync(
                    tabUrl,
                    new FmReadySignal(kind, readyPattern),
                    capture,
                    NavTimeout,
                    ct);

                string raw;
                string status;
                int signalCount = 0;

                if (kind == FmReadySignalKind.DomSelector)
                {
                    raw = outcome.ResponseBody ?? string.Empty;
                    status = raw.Length >= 1000 ? "OK" : "NO_DATA";
                }
                else
                {
                    raw = outcome.ResponseBody ?? string.Empty;
                    if (!string.IsNullOrEmpty(outcome.ResponseUrl))
                        trendUrls[tab] = outcome.ResponseUrl!;
                    var (statusComputed, count, suspectWarning) =
                        await ComputeTrendsTabAsync(fixture.FixtureId, tab, raw, sourceTs, ct);
                    status = statusComputed;
                    signalCount = count;
                    if (suspectWarning != null) warnings.Add(suspectWarning);
                }

                string? rawPath = null;
                string sha;
                try
                {
                    rawPath = await WriteRawAsync(fixture.FixtureId, tab, ext, raw, ct);
                    rawPaths.Add(rawPath);
                    sha = Sha256Hex(raw);
                }
                catch (Exception ex)
                {
                    warnings.Add($"raw write failed for {tab}: {ex.Message}");
                    sha = string.Empty;
                }

                try
                {
                    var leakage = fixture.KickoffUtc.HasValue &&
                                  sourceTs > fixture.KickoffUtc.Value;
                    var snapshotId = await _store.InsertSnapshotAsync(
                        fixture.FixtureId, tab, tabUrl, sourceTs,
                        rawPath ?? string.Empty, sha,
                        kind == FmReadySignalKind.ResponsePattern
                            ? FmTrendsJsonParser.ParserVersion
                            : HtmlParserVersion,
                        status, leakage, ct);

                    if (kind == FmReadySignalKind.ResponsePattern)
                    {
                        if (signalCount > 0)
                        {
                            var drafts = FmTrendsJsonParser.Parse(raw, TabSubjectType(tab));
                            var (venueScope, compScope) = ScopesFromUrl(outcome.ResponseUrl);
                            var insertResult = await _store.InsertSignalsAsync(
                                snapshotId, fixture.FixtureId, drafts,
                                venueScope, compScope, sourceTs,
                                ParamsJsonFromUrl(outcome.ResponseUrl), ct);
                            if (insertResult.Suspect > 0)
                            {
                                status = "SUSPECT";
                                warnings.Add(
                                    $"validation: {insertResult.Suspect} signal(s) SUSPECT for {tab}: {insertResult.FirstMotivo}");
                            }
                        }

                        // C2: capture FM odds (data[].odds = array of {bk, over, under}).
                        var oddsDrafts = FmTrendsJsonParser.ParseOdds(
                            raw, TabSubjectType(tab));
                        var oddsInserted = await _store.InsertOddsAsync(
                            fixture.FixtureId, oddsDrafts, snapshotId, sourceTs, ct);
                        if (oddsInserted > 0)
                            warnings.Add($"{tab}: {oddsInserted} odds row(s) captured");
                    }
                }
                catch (Exception ex)
                {
                    warnings.Add($"db insert failed for {tab}: {ex.Message}");
                    status = "ERROR";
                }

                tabs.Add(new FmTabOutcome(tab, status, signalCount, rawPath, null));
            }
            catch (FmNavigationException ex)
            {
                _logger.LogWarning("Snapshot tab {Tab} failed for {FixtureId}: {Code} {Msg}",
                    tab, fixture.FixtureId, ex.Code, ex.Message);
                var mapped = MapCode(ex.Code);
                if (kind == FmReadySignalKind.ResponsePattern &&
                    ex.Code is FmNavigationErrorCode.Timeout
                        or FmNavigationErrorCode.NoData
                        or FmNavigationErrorCode.PageNotReady &&
                    await PaywallMarkersAsync())
                {
                    mapped = "DEGRADED";
                    warnings.Add(
                        $"paywall markers (Upgrade/Sign in) with trends API silent for {tab}: non-premium profile suspected");
                }
                tabs.Add(new FmTabOutcome(tab, mapped, 0, null, ex.Message));
            }
        }

        if (trendUrls.Count > 0)
            await CaptureMatchScopeAsync(fixture, trendUrls, tabs, rawPaths, warnings, ct);

        var partial = tabs.Any(t => t.Status is not ("OK" or "SUSPECT" or "EMPTY"));
        return new FmSnapshotOutcome(fixture.FixtureId, tabs, rawPaths, warnings, partial);
    }

    // B4: same-venue scope (location=match) captured via direct API fetch —
    // 0 extra navigations: the SPA drops ?location= on soft navigations
    // (evidence tmp/fm/p4_b1_location_discovery.json), and the API itself
    // answers location=match with data:[] for every fixture tested (D5 + B1).
    private async Task CaptureMatchScopeAsync(
        FmFixtureRef fixture, IReadOnlyDictionary<string, string> trendUrls,
        List<FmTabOutcome> tabs, List<string> rawPaths, List<string> warnings,
        CancellationToken ct)
    {
        foreach (var (tab, responseUrl) in trendUrls)
        {
            var dbTab = $"{tab}+loc=match";
            var apiPath = WithLocationMatch(ToPathAndQuery(responseUrl));
            try
            {
                string? raw;
                var gate = _browserManager.NavigationGate;
                await gate.WaitAsync(ct);
                try
                {
                    raw = await _collector.FetchJsonAsync(apiPath, ct);
                }
                finally
                {
                    gate.Release();
                }

                if (string.IsNullOrEmpty(raw))
                {
                    warnings.Add($"{tab} location=match: no response from API fetch");
                    tabs.Add(new FmTabOutcome(dbTab, "NO_DATA", 0, null, null));
                    continue;
                }

                var sourceTs = DateTime.UtcNow;
                var (status, count, suspectWarning) =
                    await ComputeTrendsTabAsync(fixture.FixtureId, tab, raw, sourceTs, ct);
                if (suspectWarning != null) warnings.Add(suspectWarning);
                warnings.Add(
                    $"{tab} location=match scope: {count} row(s) " +
                    $"(direct API fetch, response location={ScopesFromUrl(apiPath).VenueScope})");

                string? rawPath = null;
                string sha;
                try
                {
                    rawPath = await WriteRawAsync(fixture.FixtureId, dbTab, "json", raw, ct);
                    rawPaths.Add(rawPath);
                    sha = Sha256Hex(raw);
                }
                catch (Exception ex)
                {
                    warnings.Add($"raw write failed for {dbTab}: {ex.Message}");
                    sha = string.Empty;
                }

                try
                {
                    var leakage = fixture.KickoffUtc.HasValue &&
                                  sourceTs > fixture.KickoffUtc.Value;
                    var snapshotId = await _store.InsertSnapshotAsync(
                        fixture.FixtureId, dbTab, apiPath, sourceTs,
                        rawPath ?? string.Empty, sha,
                        FmTrendsJsonParser.ParserVersion, status, leakage, ct);

                    var (venueScope, compScope) = ScopesFromUrl(apiPath);
                    var paramsJson = ParamsJsonFromUrl(apiPath);
                    if (count > 0)
                    {
                        var drafts = FmTrendsJsonParser.Parse(raw, TabSubjectType(tab));
                        var insertResult = await _store.InsertSignalsAsync(
                            snapshotId, fixture.FixtureId, drafts,
                            venueScope, compScope, sourceTs, paramsJson, ct);
                        if (insertResult.Suspect > 0)
                        {
                            warnings.Add(
                                $"validation: {insertResult.Suspect} signal(s) SUSPECT for {dbTab}: {insertResult.FirstMotivo}");
                        }
                    }

                    var oddsDrafts = FmTrendsJsonParser.ParseOdds(raw, TabSubjectType(tab));
                    await _store.InsertOddsAsync(
                        fixture.FixtureId, oddsDrafts, snapshotId, sourceTs, ct);
                }
                catch (Exception ex)
                {
                    warnings.Add($"db insert failed for {dbTab}: {ex.Message}");
                    status = "ERROR";
                }

                tabs.Add(new FmTabOutcome(dbTab, status, count, rawPath, null));
            }
            catch (Exception ex)
            {
                warnings.Add($"{dbTab} failed: {ex.Message}");
                tabs.Add(new FmTabOutcome(dbTab, "ERROR", 0, null, ex.Message));
            }
        }
    }

    private static string ToPathAndQuery(string url) =>
        url.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? new Uri(url).PathAndQuery
            : url;

    // Replaces (never appends to) any location param the SPA may have left in
    // the intercepted base URL, so the request carries exactly location=match.
    private static string WithLocationMatch(string pathAndQuery)
    {
        var q = pathAndQuery.IndexOf('?');
        if (q < 0) return pathAndQuery + "?location=match";
        var basePath = pathAndQuery[..q];
        var kept = pathAndQuery[(q + 1)..]
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Where(p => !p.StartsWith("location=", StringComparison.Ordinal));
        return $"{basePath}?{string.Join("&", kept)}&location=match";
    }

    private async Task<(string Status, int Count, string? Warning)> ComputeTrendsTabAsync(
        string fixtureId, string tab, string raw, DateTime sourceTs, CancellationToken ct)
    {
        List<FmSignalDraft> drafts;
        try
        {
            drafts = FmTrendsJsonParser.Parse(raw, TabSubjectType(tab));
        }
        catch (Exception ex)
        {
            return ("NO_DATA", 0, $"unparseable trends payload for {tab}: {ex.Message}");
        }

        if (drafts.Count == 0) return ("EMPTY", 0, null);

        string? warning = null;
        var status = "OK";
        try
        {
            var previous = await _store.GetLastOkSignalCountsAsync(fixtureId, tab, ct);
            if (previous.Count > 0)
            {
                var current = drafts
                    .GroupBy(d => d.Market ?? "(null)", StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);
                if (FmCanary.ExceedsDeviation(current, previous))
                {
                    status = "SUSPECT";
                    warning = $"canary: signals per market deviate >30% from last OK run for {tab}";
                }
            }
        }
        catch (Exception ex)
        {
            warning = $"canary check skipped for {tab}: {ex.Message}";
        }

        return (status, drafts.Count, warning);
    }

    private async Task<string> PageHtmlAsync()
    {
        var page = _browserManager.GetPage();
        if (page == null)
            throw new FmNavigationException(
                FmNavigationErrorCode.BrowserNotReady, "Browser not started.");
        return await page.EvaluateAsync<string>(
            "() => document.documentElement.outerHTML") ?? string.Empty;
    }

    private async Task<bool> PaywallMarkersAsync()
    {
        try
        {
            var page = _browserManager.GetPage();
            if (page == null) return false;
            var text = await page.EvaluateAsync<string>(
                "() => document.body?.innerText || ''") ?? string.Empty;
            return text.Contains("Upgrade") && text.Contains("Sign in");
        }
        catch
        {
            return false;
        }
    }

    private static string TabSubjectType(string tab) =>
        tab == "player-trends" ? "player" : "team";

    public static (string VenueScope, string CompetitionScope) ScopesFromUrl(string? responseUrl)
    {
        if (string.IsNullOrEmpty(responseUrl)) return ("all", "all");
        var queryIndex = responseUrl.IndexOf('?');
        if (queryIndex < 0) return ("all", "all");
        var query = responseUrl[(queryIndex + 1)..];
        var venue = "all";
        var comp = "all";
        foreach (var pair in query.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = pair.Split('=', 2);
            if (kv.Length != 2) continue;
            if (kv[0] == "location") venue = Uri.UnescapeDataString(kv[1]);
            else if (kv[0] == "league_only") comp = kv[1] == "true" ? "same_league" : "all";
        }
        return (venue, comp);
    }

    public static string ParamsJsonFromUrl(string? responseUrl)
    {
        var location = "all";
        var leagueOnly = false;
        if (!string.IsNullOrEmpty(responseUrl))
        {
            var queryIndex = responseUrl.IndexOf('?');
            if (queryIndex >= 0)
            {
                foreach (var pair in responseUrl[(queryIndex + 1)..]
                    .Split('&', StringSplitOptions.RemoveEmptyEntries))
                {
                    var kv = pair.Split('=', 2);
                    if (kv.Length != 2) continue;
                    if (kv[0] == "location") location = Uri.UnescapeDataString(kv[1]);
                    else if (kv[0] == "league_only") leagueOnly = kv[1] == "true";
                }
            }
        }

        return System.Text.Json.JsonSerializer.Serialize(
            new { location, league_only = leagueOnly });
    }

    private static string MapCode(FmNavigationErrorCode code) => code switch
    {
        FmNavigationErrorCode.SessionExpired => "SESSION_EXPIRED",
        FmNavigationErrorCode.Timeout => "TIMEOUT",
        FmNavigationErrorCode.PageNotReady => "PAGE_NOT_READY",
        FmNavigationErrorCode.NoData => "NO_DATA",
        FmNavigationErrorCode.BrowserNotReady => "BROWSER_NOT_READY",
        FmNavigationErrorCode.NavigationBudgetExceeded => "BUDGET_EXCEEDED",
        _ => "ERROR"
    };

    private static async Task<string> WriteRawAsync(
        string fixtureId, string tab, string ext, string content, CancellationToken ct)
    {
        var dir = Path.Combine(
            AppContext.BaseDirectory, "tmp", "fm", "snapshots", fixtureId, tab);
        Directory.CreateDirectory(dir);
        var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}.{ext}";
        var path = Path.Combine(dir, fileName);
        await File.WriteAllTextAsync(path, content, new UTF8Encoding(false), ct);
        return path;
    }

    private static string Sha256Hex(string content) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content))).ToLowerInvariant();
}
