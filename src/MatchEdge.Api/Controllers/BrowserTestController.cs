using MatchEdge.Infrastructure.Clients;
using Microsoft.AspNetCore.Mvc;

namespace MatchEdge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrowserTestController : ControllerBase
{
    private readonly PlaywrightBrowserManager _browserManager;
    private readonly SofaScoreBrowserCollector _collector;
    private readonly ILogger<BrowserTestController> _logger;

    public BrowserTestController(
        PlaywrightBrowserManager browserManager,
        SofaScoreBrowserCollector collector,
        ILogger<BrowserTestController> logger)
    {
        _browserManager = browserManager;
        _collector = collector;
        _logger = logger;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartBrowser()
    {
        await _browserManager.StartAsync();
        return Ok(new { status = "Chrome started", profile = "persistent" });
    }

    [HttpGet("status")]
    public IActionResult Status()
    {
        return Ok(new
        {
            ready = _browserManager.IsReady,
            message = _browserManager.IsReady
                ? "SofaScore detected. Ready to fetch."
                : "Navigate to SofaScore manually in the Chrome window."
        });
    }

    [HttpGet("wait")]
    public async Task<IActionResult> WaitForReady([FromQuery] int timeoutSeconds = 300)
    {
        var timeout = TimeSpan.FromSeconds(timeoutSeconds);
        var ready = await _browserManager.WaitForSofaScoreReadyAsync(timeout);
        return Ok(new { ready, timeoutSeconds });
    }

    [HttpGet("fetch")]
    public async Task<IActionResult> Fetch([FromQuery] string apiPath)
    {
        if (string.IsNullOrEmpty(apiPath))
            return BadRequest(new { error = "apiPath is required" });

        var json = await _collector.FetchJsonAsync(apiPath);
        if (json == null)
            return StatusCode(502, new { error = "Failed to fetch from SofaScore" });

        return Content(json, "application/json");
    }

    [HttpGet("fetch/statistics")]
    public async Task<IActionResult> FetchStatistics(
        [FromQuery] int teamId = 2311,
        [FromQuery] int tournamentId = 406,
        [FromQuery] int seasonId = 88529)
    {
        var apiPath = $"team/{teamId}/unique-tournament/{tournamentId}/season/{seasonId}/statistics/overall";
        var json = await _collector.FetchJsonAsync(apiPath);
        if (json == null)
            return StatusCode(502, new { error = "Failed to fetch statistics" });

        return Content(json, "application/json");
    }

    [HttpPost("close")]
    public async Task<IActionResult> CloseBrowser()
    {
        await _browserManager.DisposeAsync();
        return Ok(new { status = "Browser closed" });
    }
}
