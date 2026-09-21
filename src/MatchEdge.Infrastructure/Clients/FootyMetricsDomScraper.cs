using System.Text.RegularExpressions;
using MatchEdge.Application.Clients.FootyMetrics;
using MatchEdge.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;

namespace MatchEdge.Infrastructure.Clients;

public class FootyMetricsDomScraper : IFootyMetricsScraper
{
    private readonly FootyMetricsBrowserManager _browserManager;
    private readonly FootyMetricsOptions _options;
    private readonly ILogger<FootyMetricsDomScraper> _logger;

    private const string ExtractTrendCardsJs = @"
(() => {
    const trends = [];
    const bodyText = document.body.innerText || '';
    const blocks = bodyText.split(/\n(?=[A-Z][^\n]{1,40}\nOpp\. hits)/);

    for (const block of blocks) {
        const lines = block.split('\n').map(l => l.trim()).filter(l => l);
        if (lines.length < 5) continue;

        const teamIdx = lines.findIndex(l => l === 'Opp. hits');
        if (teamIdx < 0) continue;

        const team = lines[teamIdx - 1] || '';
        if (!team || team.length > 50) continue;

        const oppHits = (teamIdx + 1 < lines.length) ? lines[teamIdx + 1] : '';

        let h2hHits = '';
        const h2hIdx = lines.indexOf('H2H');
        if (h2hIdx > 0 && h2hIdx + 1 < lines.length) {
            h2hHits = lines[h2hIdx + 1];
        }

        let direction = 'yes';
        let odds = '';
        let hitCount = '';
        let percentage = '';
        let fixture = '';
        let league = '';

        const hitIdx = lines.indexOf('Hit');
        const rateIdx = lines.indexOf('Rate');

        if (hitIdx > 0 && hitIdx - 1 >= 0) {
            hitCount = lines[hitIdx - 1];
        }

        if (rateIdx > 0 && rateIdx - 1 >= 0) {
            percentage = lines[rateIdx - 1];
        }

        const yesIdx = lines.findIndex(l => l === 'Yes' || l === 'No');
        if (yesIdx > 0) {
            direction = lines[yesIdx].toLowerCase();
            for (let j = yesIdx + 1; j < Math.min(yesIdx + 4, lines.length); j++) {
                const val = parseFloat(lines[j]);
                if (!isNaN(val) && val > 1 && val < 50) {
                    odds = lines[j];
                    break;
                }
            }
        }

        const vsIdx = lines.indexOf('vs');
        if (vsIdx > 0) {
            const homeCode = lines[vsIdx - 1] || '';
            const awayCode = lines[vsIdx + 1] || '';
            fixture = homeCode + ' vs ' + awayCode;
        }

        const leaguePatterns = [
            'Serie A', 'Serie B', 'Premier League', 'La Liga', 'Bundesliga',
            'Ligue 1', 'Eredivisie', 'Primeira Liga', 'Super League',
            'Major League Soccer', 'Champions League', 'Europa League',
            'Conference League', 'UEFA Nations League', 'Friendly International',
            'Liga MX', 'Brasileir', 'Argentina', 'A-League',
            'J1 League', 'K League', 'Saudi Pro', 'Turkish Super',
            'Scottish Premiership', 'Championship', 'League One', 'League Two',
            'MLS', 'USL', 'Indian Super', 'Chinese Super'
        ];
        for (const l of leaguePatterns) {
            if (block.includes(l)) { league = l; break; }
        }

        const lastIdx = lines.indexOf('Last');
        let recentForm = [];
        if (lastIdx > 0) {
            for (let j = lastIdx + 1; j < lines.length && recentForm.length < 5; j++) {
                if (/^\d+-\d+$/.test(lines[j])) {
                    recentForm.push(lines[j]);
                }
            }
        }

        if (team && hitCount && percentage) {
            trends.push({
                team: team,
                slug: team.toLowerCase().replace(/[^a-z0-9]+/g, '-'),
                venue: 'both',
                hitCount: hitCount,
                market: '',
                odds: odds,
                desc: direction === 'yes' ? 'BTTS Yes' : 'BTTS No',
                opp: oppHits,
                avg: '',
                rate: percentage,
                fixture: fixture,
                league: league,
                direction: direction,
                h2h: h2hHits,
                recentForm: recentForm.join(',')
            });
        }
    }

    return JSON.stringify(trends);
})()";

    public FootyMetricsDomScraper(
        FootyMetricsBrowserManager browserManager,
        IOptions<FootyMetricsOptions> options,
        ILogger<FootyMetricsDomScraper> logger)
    {
        _browserManager = browserManager;
        _options = options.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        await _browserManager.StartAsync(ct);
    }

    public async Task<bool> WaitForReadyAsync(TimeSpan? timeout = null, CancellationToken ct = default)
    {
        return await _browserManager.WaitForReadyAsync(timeout, ct);
    }

    public async Task<List<FootyMetricsScrapedTrend>> ScrapeFixtureTrendsAsync(
        string fixtureSlug,
        CancellationToken ct = default)
    {
        var page = _browserManager.GetPage();
        if (page == null)
        {
            _logger.LogWarning("FootyMetrics browser not started.");
            return [];
        }

        var url = $"{_options.BaseUrl.TrimEnd('/')}/fixtures/{fixtureSlug}?tab=team-trends";
        _logger.LogInformation("Scraping fixture trends: {Url}", url);

        try
        {
            await page.GotoAsync(url, new PageGotoOptions
            {
                Timeout = 45000,
                WaitUntil = WaitUntilState.DOMContentLoaded
            });

            await page.WaitForTimeoutAsync(5000);

            var trendsJson = await page.EvaluateAsync<string>(ExtractTrendCardsJs);
            if (string.IsNullOrEmpty(trendsJson))
            {
                _logger.LogWarning("No trends found for fixture {Slug}", fixtureSlug);
                return [];
            }

            return ParseTrends(trendsJson, fixtureSlug);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to scrape fixture trends for {Slug}", fixtureSlug);
            return [];
        }
    }

    public async Task<List<FootyMetricsScrapedTrend>> ScrapeMarketTrendsAsync(
        string market,
        int maxPages = 1,
        CancellationToken ct = default)
    {
        var page = _browserManager.GetPage();
        if (page == null)
        {
            _logger.LogWarning("FootyMetrics browser not started.");
            return [];
        }

        var allTrends = new List<FootyMetricsScrapedTrend>();

        for (int pageNum = 1; pageNum <= maxPages; ct.ThrowIfCancellationRequested())
        {
            var url = $"{_options.BaseUrl.TrimEnd('/')}/trends/{market}";
            if (pageNum > 1)
                url += $"?page={pageNum}";

            _logger.LogInformation("Scraping {Market} trends page {Page}", market, pageNum);

            try
            {
                await page.GotoAsync(url, new PageGotoOptions
                {
                    Timeout = 45000,
                    WaitUntil = WaitUntilState.DOMContentLoaded
                });

                await page.WaitForTimeoutAsync(5000);

                var trendsJson = await page.EvaluateAsync<string>(ExtractTrendCardsJs);
                if (string.IsNullOrEmpty(trendsJson))
                {
                    _logger.LogInformation("No more trends found at page {Page}", pageNum);
                    break;
                }

                var trends = ParseTrends(trendsJson, null);
                if (trends.Count == 0)
                    break;

                allTrends.AddRange(trends);

                var hasNextPage = await page.EvaluateAsync<bool>(@"
                    (() => {
                        const next = document.querySelector('[aria-label=""Next""]') ||
                                     document.querySelector('a[class*=""next""]');
                        return next !== null && !next.disabled;
                    })()
                ");

                if (!hasNextPage)
                    break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to scrape {Market} trends page {Page}", market, pageNum);
                break;
            }
        }

        return allTrends;
    }

    private List<FootyMetricsScrapedTrend> ParseTrends(string json, string? fixtureSlug)
    {
        try
        {
            var rawTrends = System.Text.Json.JsonSerializer.Deserialize<List<RawTrendCard>>(
                json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (rawTrends == null)
                return [];

            return rawTrends.Select(r => MapToScrapedTrend(r, fixtureSlug)).ToList();
        }
        catch (System.Text.Json.JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse trends JSON");
            return [];
        }
    }

    private FootyMetricsScrapedTrend MapToScrapedTrend(RawTrendCard raw, string? fixtureSlug)
    {
        var hitCountParts = (raw.hitCount ?? "").Split('/');
        int numerator = 0, denominator = 0;
        if (hitCountParts.Length == 2)
        {
            int.TryParse(hitCountParts[0], out numerator);
            int.TryParse(hitCountParts[1], out denominator);
        }

        double hitRate = denominator > 0 ? (double)numerator / denominator : 0;

        int oppHitRate = 0;
        if (!string.IsNullOrWhiteSpace(raw.opp))
        {
            var oppClean = Regex.Replace(raw.opp, "[^0-9]", "");
            int.TryParse(oppClean, out oppHitRate);
        }

        double avgValue = 0;
        if (!string.IsNullOrWhiteSpace(raw.avg))
        {
            double.TryParse(raw.avg, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out avgValue);
        }

        double ratePercentage = 0;
        if (!string.IsNullOrWhiteSpace(raw.rate))
        {
            var rateClean = raw.rate.Replace("%", "").Trim();
            double.TryParse(rateClean, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out ratePercentage);
        }

        return new FootyMetricsScrapedTrend(
            Team: raw.team ?? "",
            TeamSlug: raw.slug ?? "",
            Venue: raw.venue ?? "both",
            HitCount: raw.hitCount ?? "",
            Market: raw.market ?? "",
            Odds: raw.odds ?? "",
            Description: raw.desc ?? "",
            HitNumerator: numerator,
            HitDenominator: denominator,
            HitRate: hitRate,
            OppHitRate: oppHitRate,
            AvgValue: avgValue,
            RatePercentage: ratePercentage,
            Fixture: raw.fixture ?? ""
        );
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    private class RawTrendCard
    {
        public string? team { get; set; }
        public string? slug { get; set; }
        public string? venue { get; set; }
        public string? hitCount { get; set; }
        public string? market { get; set; }
        public string? odds { get; set; }
        public string? desc { get; set; }
        public string? opp { get; set; }
        public string? avg { get; set; }
        public string? rate { get; set; }
        public string? fixture { get; set; }
    }
}
