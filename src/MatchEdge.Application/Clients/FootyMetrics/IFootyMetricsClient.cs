namespace MatchEdge.Application.Clients.FootyMetrics;

public interface IFootyMetricsClient
{
    Task<FootyMetricsTeamTrendResponse?> GetTeamTrendsAsync(
        int fixtureId,
        CancellationToken ct = default);
}
