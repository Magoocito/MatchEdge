namespace MatchEdge.Application.Clients.FootyMetrics;

public interface IFootyMetricsCollector : IAsyncDisposable
{
    Task StartAsync(CancellationToken ct = default);
    Task<bool> WaitForReadyAsync(TimeSpan? timeout = null, CancellationToken ct = default);
    Task<string?> FetchJsonAsync(string apiPath, CancellationToken ct = default);
}
