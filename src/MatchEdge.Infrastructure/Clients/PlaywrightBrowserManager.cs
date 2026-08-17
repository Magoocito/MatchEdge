using System.Diagnostics;
using Microsoft.Playwright;
using Microsoft.Extensions.Logging;

namespace MatchEdge.Infrastructure.Clients;

public class PlaywrightBrowserManager : IPlaywrightBrowserManager
{
    private readonly string _chromePath;
    private readonly string _profileDir;
    private readonly ILogger<PlaywrightBrowserManager> _logger;
    private IPlaywright? _playwright;
    private IBrowserContext? _context;
    private IPage? _page;
    private bool _ready;

    public bool IsReady => _page != null && _page.Url.Contains("sofascore.com") && !_page.Url.Contains("challenge");

    public PlaywrightBrowserManager(ILogger<PlaywrightBrowserManager> logger)
    {
        _logger = logger;
        _chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
        _profileDir = Path.Combine(AppContext.BaseDirectory, "chrome-profile");
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        if (_playwright != null)
            return;

        _logger.LogInformation("Starting Playwright...");

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

        _logger.LogInformation("Chrome launched. Navigating to SofaScore...");

        await _page.GotoAsync("https://www.sofascore.com", new PageGotoOptions
        {
            Timeout = 60000
        });
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        _ready = true;
        _logger.LogInformation("SofaScore loaded. Ready to fetch.");
    }

    public async Task<bool> WaitForSofaScoreReadyAsync(
        TimeSpan? timeout = null, CancellationToken ct = default)
    {
        if (_page == null)
            return false;

        var elapsed = timeout ?? TimeSpan.FromMinutes(10);
        var sw = Stopwatch.StartNew();

        while (sw.Elapsed < elapsed && !ct.IsCancellationRequested)
        {
            var url = _page.Url;
            if (url.Contains("sofascore.com") && !url.Contains("challenge"))
            {
                _ready = true;
                _logger.LogInformation("SofaScore ready at {Url}", url);
                return true;
            }
            await Task.Delay(1000, ct);
        }

        return false;
    }

    internal IPage? GetPage() => _page;

    public async ValueTask DisposeAsync()
    {
        if (_context != null)
        {
            await _context.CloseAsync();
            await _context.DisposeAsync();
        }
        _playwright?.Dispose();
        _logger.LogInformation("Browser closed.");
    }
}
