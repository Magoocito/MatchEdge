using System.Diagnostics;
using Microsoft.Playwright;
using Microsoft.Extensions.Logging;

namespace MatchEdge.Infrastructure.Clients;

public class FootyMetricsBrowserManager : IAsyncDisposable
{
    private readonly string _chromePath;
    private readonly string _profileDir;
    private readonly ILogger<FootyMetricsBrowserManager> _logger;
    private IPlaywright? _playwright;
    private IBrowserContext? _context;
    private IPage? _page;
    private bool _ready;

    public bool IsReady => _page != null && _page.Url.Contains("footymetrics.com") && !_page.Url.Contains("challenge");

    public FootyMetricsBrowserManager(ILogger<FootyMetricsBrowserManager> logger)
    {
        _logger = logger;
        _chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
        _profileDir = Path.Combine(AppContext.BaseDirectory, "chrome-profile-footymetrics");
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        if (_playwright != null)
            return;

        _logger.LogInformation("Starting Playwright for FootyMetrics...");

        _playwright = await Playwright.CreateAsync();

        _context = await _playwright.Chromium.LaunchPersistentContextAsync(
            _profileDir,
            new BrowserTypeLaunchPersistentContextOptions
            {
                ExecutablePath = _chromePath,
                Headless = false,
                SlowMo = 0,
                ViewportSize = new ViewportSize { Width = 1280, Height = 800 },
                Args = [
                    "--disable-blink-features=AutomationControlled",
                    "--no-first-run",
                    "--no-default-browser-check"
                ]
            });

        _page = _context.Pages.Count > 0
            ? _context.Pages[0]
            : await _context.NewPageAsync();

        _logger.LogInformation("Chrome launched. Navigating to FootyMetrics...");

        await _page.GotoAsync("https://www.footymetrics.com", new PageGotoOptions
        {
            Timeout = 60000
        });
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        _ready = true;
        _logger.LogInformation("FootyMetrics loaded. Ready to fetch.");
    }

    public async Task<bool> WaitForReadyAsync(
        TimeSpan? timeout = null, CancellationToken ct = default)
    {
        if (_page == null)
            return false;

        var elapsed = timeout ?? TimeSpan.FromMinutes(10);
        var sw = Stopwatch.StartNew();

        while (sw.Elapsed < elapsed && !ct.IsCancellationRequested)
        {
            var url = _page.Url;
            if (url.Contains("footymetrics.com") && !url.Contains("challenge"))
            {
                _ready = true;
                _logger.LogInformation("FootyMetrics ready at {Url}", url);
                return true;
            }
            await Task.Delay(1000, ct);
        }

        return false;
    }

    public IPage? GetPage() => _page;

    public async ValueTask DisposeAsync()
    {
        if (_context != null)
        {
            try { await _context.CloseAsync(); } catch { }
            try { await _context.DisposeAsync(); } catch { }
            _context = null;
        }
        _playwright?.Dispose();
        _playwright = null;
        _page = null;
        _ready = false;
        _logger.LogInformation("FootyMetrics browser closed.");
    }
}
