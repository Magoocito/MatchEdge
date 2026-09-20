using MatchEdge.Application.Clients.FootyMetrics;
using Microsoft.Extensions.Logging;
using MatchEdge.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace MatchEdge.Infrastructure.Clients;

public class FootyMetricsBrowserCollector : IFootyMetricsCollector
{
    private readonly FootyMetricsBrowserManager _browserManager;
    private readonly FootyMetricsOptions _options;
    private readonly ILogger<FootyMetricsBrowserCollector> _logger;

    public FootyMetricsBrowserCollector(
        FootyMetricsBrowserManager browserManager,
        IOptions<FootyMetricsOptions> options,
        ILogger<FootyMetricsBrowserCollector> logger)
    {
        _browserManager = browserManager;
        _options = options.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        await _browserManager.StartAsync(ct);
    }

    public async Task<bool> WaitForReadyAsync(
        TimeSpan? timeout = null, CancellationToken ct = default)
    {
        return await _browserManager.WaitForReadyAsync(timeout, ct);
    }

    public async Task<string?> FetchJsonAsync(string apiPath, CancellationToken ct = default)
    {
        var page = _browserManager.GetPage();
        if (page == null)
        {
            _logger.LogWarning("FootyMetrics browser not started.");
            return null;
        }

        var path = apiPath.TrimStart('/');
        _logger.LogInformation("FootyMetrics fetching: {Path}", path);

        try
        {
            var result = await page.EvaluateAsync<string>(@"
                async (path) => {
                    const resp = await fetch('/' + path, {
                        credentials: 'include',
                        headers: { 'accept': 'application/json' }
                    });
                    if (!resp.ok) return JSON.stringify({ error: resp.status, statusText: resp.statusText });
                    return await resp.text();
                }", path);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch FootyMetrics {Path}", apiPath);
            return null;
        }
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
