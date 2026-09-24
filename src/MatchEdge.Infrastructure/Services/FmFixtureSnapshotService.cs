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
    private readonly FmSnapshotStore _store;
    private readonly ILogger<FmFixtureSnapshotService> _logger;

    public FmFixtureSnapshotService(
        FmNavigator navigator,
        FootyMetricsBrowserManager browserManager,
        FmSnapshotStore store,
        ILogger<FmFixtureSnapshotService> logger)
    {
        _navigator = navigator;
        _browserManager = browserManager;
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

        foreach (var (tab, readyPattern, kind, ext) in Tabs)
        {
            ct.ThrowIfCancellationRequested();
            var tabUrl = tab == "overview" ? baseUrl : $"{baseUrl}?tab={tab}";
            var sourceTs = DateTime.UtcNow;

            try
            {
                var outcome = await _navigator.GoToAsync(
                    tabUrl,
                    new FmReadySignal(kind, readyPattern),
                    NavTimeout,
                    ct);

                string raw;
                string status;
                int signalCount = 0;

                if (kind == FmReadySignalKind.DomSelector)
                {
                    raw = await PageHtmlAsync();
                    status = raw.Length >= 1000 ? "OK" : "NO_DATA";
                }
                else
                {
                    raw = outcome.ResponseBody ?? string.Empty;
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
                    var snapshotId = await _store.InsertSnapshotAsync(
                        fixture.FixtureId, tab, tabUrl, sourceTs,
                        rawPath ?? string.Empty, sha,
                        kind == FmReadySignalKind.ResponsePattern
                            ? FmTrendsJsonParser.ParserVersion
                            : HtmlParserVersion,
                        status, ct);

                    if (kind == FmReadySignalKind.ResponsePattern && signalCount > 0)
                    {
                        var drafts = FmTrendsJsonParser.Parse(raw, TabSubjectType(tab));
                        var (venueScope, compScope) = ScopesFromUrl(outcome.ResponseUrl);
                        await _store.InsertSignalsAsync(
                            snapshotId, fixture.FixtureId, drafts,
                            venueScope, compScope, sourceTs, ct);
                    }                }
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
                tabs.Add(new FmTabOutcome(tab, MapCode(ex.Code), 0, null, ex.Message));
            }
        }

        var partial = tabs.Any(t => t.Status is not ("OK" or "SUSPECT" or "EMPTY"));
        return new FmSnapshotOutcome(fixture.FixtureId, tabs, rawPaths, warnings, partial);
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
