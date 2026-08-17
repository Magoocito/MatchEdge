namespace MatchEdge.Infrastructure.Clients;

public interface IPlaywrightBrowserManager : IAsyncDisposable
{
    Task StartAsync(CancellationToken ct = default);
    Task<bool> WaitForSofaScoreReadyAsync(TimeSpan? timeout = null, CancellationToken ct = default);
    bool IsReady { get; }
}
