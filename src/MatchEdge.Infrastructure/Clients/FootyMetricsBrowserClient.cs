using System.Text.Json;
using MatchEdge.Application.Clients.FootyMetrics;
using MatchEdge.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MatchEdge.Infrastructure.Clients;

public class FootyMetricsBrowserClient : IFootyMetricsClient
{
    private readonly FootyMetricsDomScraper _scraper;
    private readonly FootyMetricsOptions _options;
    private readonly ILogger<FootyMetricsBrowserClient> _logger;

    public FootyMetricsBrowserClient(
        FootyMetricsDomScraper scraper,
        IOptions<FootyMetricsOptions> options,
        ILogger<FootyMetricsBrowserClient> logger)
    {
        _scraper = scraper;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<FootyMetricsTeamTrendResponse?> GetTeamTrendsAsync(
        int fixtureId, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Fetching team trends via DOM scraping for fixture {FixtureId}", fixtureId);

        var trends = await _scraper.ScrapeFixtureTrendsAsync(
            fixtureId.ToString(), ct);

        if (trends.Count == 0)
        {
            _logger.LogWarning("No trends scraped for fixture {FixtureId}", fixtureId);
            return null;
        }

        var data = trends.Select(t => new FootyMetricsTeamTrendData(
            TeamName: t.Team,
            TeamSlug: t.TeamSlug,
            HitCount: t.HitCount,
            Market: t.Market,
            Odds: t.Odds,
            Description: t.Description,
            Venue: t.Venue,
            OppHitRate: t.OppHitRate.ToString() + "%",
            AvgValue: t.AvgValue.ToString("F2"),
            RatePercentage: t.RatePercentage.ToString("F0") + "%",
            Fixture: t.Fixture
        )).ToList();

        return new FootyMetricsTeamTrendResponse(
            FixtureSlug: fixtureId.ToString(),
            HomeTeam: "",
            AwayTeam: "",
            Data: data
        );
    }

    public async Task<List<FootyMetricsScrapedTrend>> GetScrapedTrendsAsync(
        string fixtureSlug, CancellationToken ct = default)
    {
        return await _scraper.ScrapeFixtureTrendsAsync(fixtureSlug, ct);
    }

    public async Task<List<FootyMetricsScrapedTrend>> GetMarketTrendsAsync(
        string market, int maxPages = 1, CancellationToken ct = default)
    {
        return await _scraper.ScrapeMarketTrendsAsync(market, maxPages, ct);
    }
}
