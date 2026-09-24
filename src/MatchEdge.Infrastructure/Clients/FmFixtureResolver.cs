using System.Text.Json;
using System.Text.RegularExpressions;
using MatchEdge.Application.Clients.FootyMetrics;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace MatchEdge.Infrastructure.Clients;

public sealed class FmFixtureResolver : IFmFixtureResolver
{
    private static readonly Regex LeagueApidFromLogo = new(
        @"leagues/(\d+)\.webp", RegexOptions.Compiled);
    private static readonly Regex RscHomeName = new(
        @"""Home"":\{[^{}]*?""name"":""([^""]+)""", RegexOptions.Compiled);
    private static readonly Regex RscAwayName = new(
        @"""Away"":\{[^{}]*?""name"":""([^""]+)""", RegexOptions.Compiled);
    private static readonly Regex FixtureIdFromPath = new(
        @"/fixtures/(\d+)-", RegexOptions.Compiled);

    private const string BaseUrl = "https://www.footymetrics.com";

    private readonly FootyMetricsBrowserManager _browserManager;
    private readonly FmNavigator _navigator;
    private readonly ILogger<FmFixtureResolver> _logger;

    public FmFixtureResolver(
        FootyMetricsBrowserManager browserManager,
        FmNavigator navigator,
        ILogger<FmFixtureResolver> logger)
    {
        _browserManager = browserManager;
        _navigator = navigator;
        _logger = logger;
    }

    public async Task<IReadOnlyList<FmFixtureRef>> ResolveAsync(
        string league,
        DateOnly date,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(league))
            throw new FmResolverException("league is required (no default guessing).");

        await EnsureOnSiteAsync(ct);

        var leaguesJson = await FetchTextAsync("/api/front/leagues", ct);
        var apid = FindLeagueApid(leaguesJson, league.Trim());
        var leagueName = FindLeagueName(leaguesJson, league.Trim());

        var fixturesUrl = $"/api/front/fixtures/league?id={apid}";
        var fixturesJson = await FetchTextAsync(fixturesUrl, ct);
        var dateKey = date.ToString("yyyy-MM-dd");

        using var doc = JsonDocument.Parse(fixturesJson);
        if (doc.RootElement.ValueKind != JsonValueKind.Array)
            throw new FmResolverException($"Unexpected fixtures payload for league '{league}'.");

        var entries = new List<(string Id, string Slug, string? Timestamp, string? Stage, bool HasTrends)>();
        foreach (var group in doc.RootElement.EnumerateArray())
        {
            if (!group.TryGetProperty("date", out var dateProp)) continue;
            if (dateProp.GetString() != dateKey) continue;
            if (!group.TryGetProperty("fixtures", out var list) || list.ValueKind != JsonValueKind.Array) continue;

            foreach (var fx in list.EnumerateArray())
            {
                var id = fx.GetProperty("id").GetString();
                var slug = fx.GetProperty("slug").GetString();
                if (id == null || slug == null) continue;
                entries.Add((
                    id,
                    slug,
                    fx.TryGetProperty("timestamp", out var ts) ? ts.GetString() : null,
                    fx.TryGetProperty("stageName", out var st) ? st.GetString() : null,
                    fx.TryGetProperty("hasTrends", out var ht) && ht.ValueKind == JsonValueKind.True));
            }
        }

        if (entries.Count == 0)
        {
            _logger.LogInformation("No fixtures for league '{League}' on {Date}.", league, dateKey);
            return Array.Empty<FmFixtureRef>();
        }

        var seen = new HashSet<string>(StringComparer.Ordinal);
        var result = new List<FmFixtureRef>();
        foreach (var e in entries)
        {
            if (!seen.Add(e.Id)) continue;

            var (home, away) = await FetchTeamNamesAsync(e.Slug, ct);
            var kickoff = DateTime.TryParse(e.Timestamp, null,
                System.Globalization.DateTimeStyles.AdjustToUniversal | System.Globalization.DateTimeStyles.AssumeUniversal,
                out var kt) ? DateTime.SpecifyKind(kt, DateTimeKind.Utc) : (DateTime?)null;

            result.Add(new FmFixtureRef(
                FixtureId: e.Id,
                Url: $"/fixtures/{e.Slug}",
                Home: home,
                Away: away,
                KickoffUtc: kickoff,
                Competition: leagueName));
        }

        return result;
    }

    public async Task<FmFixtureRef> DescribeAsync(string url, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new FmResolverException("url is required.");

        var path = url;
        if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            var uri = new Uri(path);
            path = uri.PathAndQuery;
        }
        path = path.TrimEnd('/');
        if (!path.StartsWith("/fixtures/", StringComparison.OrdinalIgnoreCase))
            throw new FmResolverException($"Not a fixture url: {url}");

        var idMatch = FixtureIdFromPath.Match(path);
        if (!idMatch.Success)
            throw new FmResolverException($"Fixture id not found in url: {url}");

        await EnsureOnSiteAsync(ct);
        var rsc = await FetchTextAsync(path, ct, rsc: true);
        var home = RscHomeName.Match(rsc);
        if (!home.Success)
            throw new FmFixtureNotFoundException($"Fixture not found: {path}");
        var away = RscAwayName.Match(rsc);

        DateTime? kickoff = null;
        var tsMatch = Regex.Match(rsc, @"""timestamp"":""([^""]+)""");
        if (tsMatch.Success &&
            DateTime.TryParse(tsMatch.Groups[1].Value, null,
                System.Globalization.DateTimeStyles.AdjustToUniversal |
                System.Globalization.DateTimeStyles.AssumeUniversal, out var kt))
            kickoff = DateTime.SpecifyKind(kt, DateTimeKind.Utc);

        string? competition = null;
        var compMatch = Regex.Match(rsc, @"""competition"":\{[^{}]*?""name"":""([^""]+)""");
        if (compMatch.Success) competition = compMatch.Groups[1].Value;

        return new FmFixtureRef(
            FixtureId: idMatch.Groups[1].Value,
            Url: path.Split('?')[0],
            Home: home.Groups[1].Value,
            Away: away.Success ? away.Groups[1].Value : null,
            KickoffUtc: kickoff,
            Competition: competition);
    }

    private async Task EnsureOnSiteAsync(CancellationToken ct)
    {
        var page = _browserManager.GetPage();
        if (page == null)
            throw new FmNavigationException(
                FmNavigationErrorCode.BrowserNotReady, "Browser not started.");

        if (!page.Url.Contains("footymetrics.com", StringComparison.OrdinalIgnoreCase))
        {
            await _navigator.GoToAsync(
                "https://www.footymetrics.com",
                new FmReadySignal(FmReadySignalKind.DomSelector, "body"),
                TimeSpan.FromSeconds(20),
                ct);
        }
    }

    internal static (string? LeagueApid, string? Name) FindLeagueEntry(string leaguesJson, string requested)
    {
        using var doc = JsonDocument.Parse(leaguesJson);
        if (doc.RootElement.ValueKind != JsonValueKind.Object ||
            !doc.RootElement.TryGetProperty("data", out var data) ||
            data.ValueKind != JsonValueKind.Array)
            throw new FmResolverException("Unexpected /api/front/leagues payload.");

        var candidates = new List<JsonElement>();
        foreach (var item in data.EnumerateArray())
        {
            var name = item.TryGetProperty("name", out var n) ? n.GetString() : null;
            if (name == null) continue;
            if (string.Equals(name, requested, StringComparison.OrdinalIgnoreCase))
                return (ExtractApid(item), name);

            if (name.Contains(requested, StringComparison.OrdinalIgnoreCase) ||
                requested.Contains(name, StringComparison.OrdinalIgnoreCase))
                candidates.Add(item.Clone());
        }

        if (candidates.Count == 1)
            return (ExtractApid(candidates[0]), candidates[0].GetProperty("name").GetString());

        if (candidates.Count > 1)
            throw new FmResolverException(
                $"League '{requested}' is ambiguous: {string.Join(", ", candidates.Select(c => c.GetProperty("name").GetString()))}");

        throw new FmResolverException($"League '{requested}' not found in /api/front/leagues.");
    }

    private static string? ExtractApid(JsonElement item)
    {
        if (item.TryGetProperty("logo", out var logo) && logo.ValueKind == JsonValueKind.String)
        {
            var m = LeagueApidFromLogo.Match(logo.GetString() ?? string.Empty);
            if (m.Success) return m.Groups[1].Value;
        }
        throw new FmResolverException(
            $"leagueApid not resolvable for '{(item.TryGetProperty("name", out var n) ? n.GetString() : "?")}' (logo regex miss).");
    }

    private string FindLeagueApid(string leaguesJson, string requested) =>
        FindLeagueEntry(leaguesJson, requested).LeagueApid!;

    private string FindLeagueName(string leaguesJson, string requested) =>
        FindLeagueEntry(leaguesJson, requested).Name ?? requested;

    private async Task<(string? Home, string? Away)> FetchTeamNamesAsync(string slug, CancellationToken ct)
    {
        var rscJson = await FetchTextAsync(
            $"/fixtures/{slug}?tab=team-trends", ct, rsc: true);
        var home = RscHomeName.Match(rscJson);
        var away = RscAwayName.Match(rscJson);
        return (
            home.Success ? home.Groups[1].Value : null,
            away.Success ? away.Groups[1].Value : null);
    }

    private async Task<string> FetchTextAsync(string pathAndQuery, CancellationToken ct, bool rsc = false)
    {
        // Uses the browser-context APIRequestContext instead of page.evaluate(fetch):
        // the request layer keeps cookies but survives navigations of the shared page
        // (background pipeline can navigate at any time).
        var context = _browserManager.GetContext()
            ?? throw new FmNavigationException(
                FmNavigationErrorCode.BrowserNotReady, "Browser not started.");

        var headers = new Dictionary<string, string>
        {
            ["accept"] = rsc ? "text/x-component, */*" : "application/json, text/plain, */*"
        };
        if (rsc) headers["RSC"] = "1";

        var absoluteUrl = new Uri(new Uri(BaseUrl), pathAndQuery).ToString();

        IAPIResponse? response = null;
        Exception? lastError = null;
        for (var attempt = 1; attempt <= 3; attempt++)
        {
            try
            {
                response = await context.APIRequest.GetAsync(absoluteUrl, new APIRequestContextOptions
                {
                    Headers = headers,
                    Timeout = 20000
                });
                lastError = null;
                break;
            }
            catch (PlaywrightException ex)
            {
                lastError = ex;
                _logger.LogWarning(
                    "Fetch attempt {Attempt}/3 failed for {Url}: {Msg}",
                    attempt, pathAndQuery, ex.Message);
                if (attempt < 3)
                    await Task.Delay(
                        TimeSpan.FromMilliseconds(attempt * 1500 + Random.Shared.Next(0, 1000)),
                        ct);
            }
        }

        if (response == null)
            throw new FmNavigationException(
                FmNavigationErrorCode.NoData,
                $"Request failed for {pathAndQuery}: {lastError?.Message}", lastError);

        if (!response.Ok)
        {
            var status = response.Status;
            if (status == 404)
                throw new FmFixtureNotFoundException($"Resource not found: {pathAndQuery}");
            if (status is 401 or 403 or 407 or 451)
                throw new FmNavigationException(
                    FmNavigationErrorCode.SessionExpired,
                    $"HTTP {status} for {pathAndQuery}. Possible expired session.");
            throw new FmNavigationException(
                FmNavigationErrorCode.NoData, $"HTTP {status} for {pathAndQuery}.");
        }

        var text = await response.TextAsync();
        if (string.IsNullOrEmpty(text))
            throw new FmNavigationException(
                FmNavigationErrorCode.NoData, $"Empty response from {pathAndQuery}");
        return text;
    }
}
