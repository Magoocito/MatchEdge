namespace MatchEdge.Application.Clients.FootyMetrics;

public interface IFootyMetricsScraper : IAsyncDisposable
{
    Task StartAsync(CancellationToken ct = default);
    Task<bool> WaitForReadyAsync(TimeSpan? timeout = null, CancellationToken ct = default);

    Task<List<FootyMetricsScrapedTrend>> ScrapeFixtureTrendsAsync(
        string fixtureSlug,
        CancellationToken ct = default);

    Task<List<FootyMetricsScrapedTrend>> ScrapeMarketTrendsAsync(
        string market,
        int maxPages = 1,
        CancellationToken ct = default);
}
