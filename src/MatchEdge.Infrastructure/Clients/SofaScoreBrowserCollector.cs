using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using MatchEdge.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace MatchEdge.Infrastructure.Clients;

public class SofaScoreBrowserCollector : ISofaScoreBrowserCollector
{
    private readonly PlaywrightBrowserManager _browserManager;
    private readonly SofaScoreOptions _options;
    private readonly ILogger<SofaScoreBrowserCollector> _logger;

    public SofaScoreBrowserCollector(
        PlaywrightBrowserManager browserManager,
        IOptions<SofaScoreOptions> options,
        ILogger<SofaScoreBrowserCollector> logger)
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
        return await _browserManager.WaitForSofaScoreReadyAsync(timeout, ct);
    }

    public async Task<string?> FetchJsonAsync(string apiPath, CancellationToken ct = default)
    {
        var page = _browserManager.GetPage();
        if (page == null)
        {
            _logger.LogWarning("Browser not started.");
            return null;
        }

        var url = $"{_options.BaseUrl.TrimEnd('/')}/{apiPath.TrimStart('/')}";
        _logger.LogInformation("Fetching: {Url}", url);

        try
        {
            var result = await page.EvaluateAsync<string>(@"
                async (url) => {
                    const resp = await fetch(url, {
                        credentials: 'include',
                        headers: { 'accept': 'application/json' }
                    });
                    if (!resp.ok) return JSON.stringify({ error: resp.status, statusText: resp.statusText });
                    return await resp.text();
                }", url);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch {Url}", url);
            return null;
        }
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
