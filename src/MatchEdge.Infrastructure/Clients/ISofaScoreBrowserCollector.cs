namespace MatchEdge.Infrastructure.Clients;

public interface ISofaScoreBrowserCollector : IAsyncDisposable
{
    Task StartAsync(CancellationToken ct = default);
    Task<bool> WaitForReadyAsync(TimeSpan? timeout = null, CancellationToken ct = default);
    Task<string?> FetchJsonAsync(string apiPath, CancellationToken ct = default);
}
