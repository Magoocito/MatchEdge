using MatchEdge.Application.Clients.FootyMetrics;
using MatchEdge.Application.Services;
using MatchEdge.Infrastructure.Clients;
using MatchEdge.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace MatchEdge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FootyMetricsTestController : ControllerBase
{
    private readonly FootyMetricsBrowserManager _browserManager;
    private readonly FootyMetricsBrowserCollector _collector;
    private readonly FootyMetricsDomScraper _scraper;
    private readonly FootyMetricsBrowserClient _client;
    private readonly ITrendPersistenceService _persistence;
    private readonly ITrendBacktestingService _backtesting;
    private readonly IBankrollManager _bankroll;
    private readonly IOrchestratorService _orchestrator;
    private readonly ILogger<FootyMetricsTestController> _logger;

    public FootyMetricsTestController(
        FootyMetricsBrowserManager browserManager,
        FootyMetricsBrowserCollector collector,
        FootyMetricsDomScraper scraper,
        FootyMetricsBrowserClient client,
        ITrendPersistenceService persistence,
        ITrendBacktestingService backtesting,
        IBankrollManager bankroll,
        IOrchestratorService orchestrator,
        ILogger<FootyMetricsTestController> logger)
    {
        _browserManager = browserManager;
        _collector = collector;
        _scraper = scraper;
        _client = client;
        _persistence = persistence;
        _backtesting = backtesting;
        _bankroll = bankroll;
        _orchestrator = orchestrator;
        _logger = logger;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartBrowser()
    {
        await _scraper.StartAsync();
        return Ok(new { status = "Chrome started for FootyMetrics", profile = "persistent-footymetrics" });
    }

    [HttpGet("status")]
    public IActionResult Status()
    {
        return Ok(new
        {
            ready = _browserManager.IsReady,
            message = _browserManager.IsReady
                ? "FootyMetrics detected. Ready to fetch."
                : "Navigate to FootyMetrics manually in the Chrome window."
        });
    }

    [HttpGet("wait")]
    public async Task<IActionResult> WaitForReady([FromQuery] int timeoutSeconds = 300)
    {
        var timeout = TimeSpan.FromSeconds(timeoutSeconds);
        var ready = await _scraper.WaitForReadyAsync(timeout);
        return Ok(new { ready, timeoutSeconds });
    }

    [HttpGet("fetch/raw")]
    public async Task<IActionResult> FetchRaw([FromQuery] string apiPath)
    {
        if (string.IsNullOrEmpty(apiPath))
            return BadRequest(new { error = "apiPath is required" });

        var json = await _collector.FetchJsonAsync(apiPath);
        if (json == null)
            return StatusCode(502, new { error = "Failed to fetch from FootyMetrics" });

        return Content(json, "application/json");
    }

    [HttpGet("scrape/fixture-trends/{fixtureSlug}")]
    public async Task<IActionResult> ScrapeFixtureTrends(string fixtureSlug)
    {
        var trends = await _scraper.ScrapeFixtureTrendsAsync(fixtureSlug);
        return Ok(new
        {
            fixtureSlug,
            totalTrends = trends.Count,
            trends
        });
    }

    [HttpGet("scrape/market-trends/{market}")]
    public async Task<IActionResult> ScrapeMarketTrends(
        string market,
        [FromQuery] int maxPages = 1)
    {
        var trends = await _scraper.ScrapeMarketTrendsAsync(market, maxPages);
        return Ok(new
        {
            market,
            maxPages,
            totalTrends = trends.Count,
            trends
        });
    }

    [HttpGet("scoring/fixture-trends/{fixtureSlug}")]
    public async Task<IActionResult> GetScoredFixtureTrends(
        string fixtureSlug,
        [FromQuery] int topN = 10)
    {
        var trends = await _scraper.ScrapeFixtureTrendsAsync(fixtureSlug);
        var scoredPicks = FootyMetricsScoringEngine.RankScrapedPicks(trends, topN);

        return Ok(new
        {
            fixtureSlug,
            totalTrends = trends.Count,
            scoredPicks = scoredPicks.Count,
            picks = scoredPicks
        });
    }

    [HttpGet("scoring/market-trends/{market}")]
    public async Task<IActionResult> GetScoredMarketTrends(
        string market,
        [FromQuery] int maxPages = 1,
        [FromQuery] int topN = 10)
    {
        var trends = await _scraper.ScrapeMarketTrendsAsync(market, maxPages);
        var scoredPicks = FootyMetricsScoringEngine.RankScrapedPicks(trends, topN);

        return Ok(new
        {
            market,
            maxPages,
            totalTrends = trends.Count,
            scoredPicks = scoredPicks.Count,
            picks = scoredPicks
        });
    }

    [HttpGet("fetch/team-trends/{fixtureId}")]
    public async Task<IActionResult> FetchTeamTrends(int fixtureId)
    {
        var response = await _client.GetTeamTrendsAsync(fixtureId);
        if (response == null)
            return StatusCode(502, new { error = "Failed to fetch team trends" });

        return Ok(response);
    }

    [HttpGet("scoring/team-trends/{fixtureId}")]
    public async Task<IActionResult> GetScoredTeamTrends(
        int fixtureId,
        [FromQuery] int topN = 10)
    {
        var response = await _client.GetTeamTrendsAsync(fixtureId);
        if (response == null)
            return StatusCode(502, new { error = "Failed to fetch team trends" });

        var scoredPicks = FootyMetricsScoringEngine.RankPicks(response, topN);

        return Ok(new
        {
            fixtureId,
            totalTrends = response.Data.Count,
            scoredPicks = scoredPicks.Count,
            picks = scoredPicks
        });
    }

    [HttpPost("persist/scrape-and-save/{fixtureSlug}")]
    public async Task<IActionResult> ScrapeAndSave(
        string fixtureSlug,
        [FromQuery] string homeTeam = "",
        [FromQuery] string awayTeam = "",
        [FromQuery] string league = "")
    {
        var trends = await _scraper.ScrapeFixtureTrendsAsync(fixtureSlug);
        if (trends.Count == 0)
            return BadRequest(new { error = "No trends scraped" });

        var saved = await _persistence.SaveTrendsAsync(
            trends, fixtureSlug, homeTeam, awayTeam, league);

        var scored = FootyMetricsScoringEngine.RankScrapedPicks(trends, 50);
        var savedPicks = await _persistence.SaveScoredPicksAsync(scored, DateTime.UtcNow);

        return Ok(new
        {
            fixtureSlug,
            trendsScraped = trends.Count,
            trendsSaved = saved,
            picksScored = scored.Count,
            picksSaved = savedPicks
        });
    }

    [HttpPost("persist/save-scored-picks")]
    public async Task<IActionResult> SaveScoredPicks(
        [FromQuery] int topN = 30,
        [FromQuery] string source = "FootyMetrics")
    {
        var trends = await _scraper.ScrapeMarketTrendsAsync("corners", 1);
        var scored = FootyMetricsScoringEngine.RankScrapedPicks(trends, topN);
        var saved = await _persistence.SaveScoredPicksAsync(scored, DateTime.UtcNow, source);

        return Ok(new
        {
            source,
            totalTrends = trends.Count,
            scoredPicks = scored.Count,
            savedPicks = saved
        });
    }

    [HttpGet("persist/daily-picks")]
    public async Task<IActionResult> GetDailyPicks(
        [FromQuery] DateTime? date = null)
    {
        var targetDate = date ?? DateTime.UtcNow;
        var picks = await _persistence.GetDailyPicksAsync(targetDate);
        return Ok(new { date = targetDate.Date, count = picks.Count, picks });
    }

    [HttpGet("persist/trends/{fixtureSlug}")]
    public async Task<IActionResult> GetPersistedTrends(string fixtureSlug)
    {
        var trends = await _persistence.GetTrendsByFixtureAsync(fixtureSlug);
        return Ok(new { fixtureSlug, count = trends.Count, trends });
    }

    [HttpGet("eval")]
    public async Task<IActionResult> EvalJs([FromQuery] string? url, [FromQuery] string js)
    {
        var page = _browserManager.GetPage();
        if (page == null)
            return BadRequest(new { error = "Browser not started" });

        try
        {
            if (!string.IsNullOrEmpty(url))
            {
                await page.GotoAsync(url, new Microsoft.Playwright.PageGotoOptions
                {
                    Timeout = 45000,
                    WaitUntil = Microsoft.Playwright.WaitUntilState.DOMContentLoaded
                });
                await Task.Delay(5000);
            }

            var result = await page.EvaluateAsync<string>(js);
            return Content(result ?? "null", "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EvalJs failed for {Url}", url);
            return StatusCode(500, new { error = ex.Message, url });
        }
    }

    [HttpGet("page-content")]
    public async Task<IActionResult> GetPageContent([FromQuery] string url)
    {
        var page = _browserManager.GetPage();
        if (page == null)
            return BadRequest(new { error = "Browser not started" });

        try
        {
            await page.GotoAsync(url, new Microsoft.Playwright.PageGotoOptions
            {
                Timeout = 30000,
                WaitUntil = Microsoft.Playwright.WaitUntilState.DOMContentLoaded
            });

            await Task.Delay(8000);

            var title = await page.TitleAsync();
            var text = await page.EvaluateAsync<string>("() => document.body?.innerText?.substring(0, 3000) || ''");

            return Ok(new
            {
                url = page.Url,
                title,
                contentPreview = text
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("intercept")]
    public async Task<IActionResult> InterceptRequests(
        [FromQuery] string url,
        [FromQuery] int waitSeconds = 15)
    {
        var page = _browserManager.GetPage();
        if (page == null)
            return BadRequest(new { error = "Browser not started" });

        var captured = new List<object>();

        EventHandler<Microsoft.Playwright.IResponse> handler = async (sender, response) =>
        {
            try
            {
                var reqUrl = response.Url;
                if (reqUrl.Contains("/api/") || reqUrl.Contains("trend"))
                {
                    string body = "";
                    try { body = await response.TextAsync(); } catch { }
                    captured.Add(new
                    {
                        method = response.Request.Method,
                        url = reqUrl,
                        status = response.Status,
                        bodyPreview = body.Length > 500 ? body.Substring(0, 500) : body
                    });
                }
            }
            catch { }
        };

        page.Response += handler;

        try
        {
            await page.GotoAsync(url, new Microsoft.Playwright.PageGotoOptions
            {
                Timeout = 45000,
                WaitUntil = Microsoft.Playwright.WaitUntilState.DOMContentLoaded
            });
            await Task.Delay(TimeSpan.FromSeconds(waitSeconds));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Intercept navigation issue");
        }
        finally
        {
            page.Response -= handler;
        }

        return Ok(new
        {
            navigatedTo = url,
            currentUrl = page.Url,
            totalCaptured = captured.Count,
            requests = captured
        });
    }

    [HttpPost("close")]
    public async Task<IActionResult> CloseBrowser()
    {
        await _browserManager.DisposeAsync();
        return Ok(new { status = "FootyMetrics browser closed" });
    }

    [HttpGet("backtest/run")]
    public async Task<IActionResult> RunBacktest(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] double stakePerUnit = 1.0)
    {
        var from = fromDate ?? DateTime.UtcNow.AddDays(-7);
        var to = toDate ?? DateTime.UtcNow;

        var result = await _backtesting.RunBacktestAsync(from, to, stakePerUnit);
        return Ok(result);
    }

    [HttpGet("backtest/summary")]
    public async Task<IActionResult> GetBacktestSummary()
    {
        var summary = await _backtesting.GetSummaryAsync();
        return Ok(summary);
    }

    [HttpGet("backtest/evaluatable/{date}")]
    public async Task<IActionResult> GetEvaluatablePicks(DateTime date)
    {
        var picks = await _backtesting.GetEvaluatablePicksAsync(date);
        return Ok(new { date = date.Date, count = picks.Count, picks });
    }

    [HttpPost("backtest/evaluate")]
    public async Task<IActionResult> EvaluatePicks(
        [FromQuery] DateTime date,
        [FromBody] List<PickEvaluationRequest> evaluations)
    {
        var results = new List<object>();

        foreach (var eval in evaluations)
        {
            var pick = await _persistence.GetDailyPicksAsync(date);
            var target = pick.FirstOrDefault(p => p.Id == eval.PickId);

            if (target == null)
                continue;

            var won = eval.Result == "Won";
            var profit = won ? (target.Odds - 1) * eval.StakeUnits : -eval.StakeUnits;

            await _persistence.UpdatePickOutcomeAsync(
                eval.PickId,
                won,
                profit,
                eval.StakeUnits,
                0,
                eval.ResultDetail ?? $"{eval.Result} at {DateTime.UtcNow:yyyy-MM-dd HH:mm}");

            results.Add(new
            {
                pickId = eval.PickId,
                team = target.Team,
                market = target.Market,
                result = eval.Result,
                profit,
                odds = target.Odds
            });
        }

        return Ok(new
        {
            date = date.Date,
            evaluated = results.Count,
            results
        });
    }

    [HttpPost("backtest/bulk-evaluate")]
    public async Task<IActionResult> BulkEvaluate(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var from = fromDate ?? DateTime.UtcNow.AddDays(-7);
        var to = toDate ?? DateTime.UtcNow.AddDays(-1);

        var allPicks = new List<DailyPickEntityDto>();
        var current = from.Date;
        while (current <= to.Date)
        {
            var dayPicks = await _persistence.GetDailyPicksAsync(current);
            allPicks.AddRange(dayPicks);
            current = current.AddDays(1);
        }

        var pending = allPicks.Where(p => p.Status == "Pending").ToList();

        return Ok(new
        {
            fromDate = from.Date,
            toDate = to.Date,
            totalPicks = allPicks.Count,
            pendingEvaluation = pending.Count,
            won = allPicks.Count(p => p.Status == "Won"),
            lost = allPicks.Count(p => p.Status == "Lost"),
            picks = pending
        });
    }

    [HttpGet("bankroll/state")]
    public async Task<IActionResult> GetBankrollState()
    {
        var state = await _bankroll.GetStateAsync();
        return Ok(state);
    }

    [HttpPost("bankroll/initialize")]
    public async Task<IActionResult> InitializeBankroll(
        [FromQuery] double initialBankroll = 100.0,
        [FromBody] BankrollConfigRequest? config = null)
    {
        var bankrollConfig = config != null
            ? new BankrollConfig(
                config.InitialBankroll,
                config.KellyFraction,
                config.MaxStakePerPick,
                config.MaxDailyExposure,
                config.MaxDrawdownPercent,
                config.MinEdge,
                config.MinSampleSize,
                config.StakeMethod,
                config.AutoUpdateStakes)
            : null;

        var state = await _bankroll.InitializeAsync(initialBankroll, bankrollConfig);
        return Ok(state);
    }

    [HttpPost("bankroll/config")]
    public async Task<IActionResult> UpdateBankrollConfig(
        [FromBody] BankrollConfigRequest config)
    {
        var bankrollConfig = new BankrollConfig(
            config.InitialBankroll,
            config.KellyFraction,
            config.MaxStakePerPick,
            config.MaxDailyExposure,
            config.MaxDrawdownPercent,
            config.MinEdge,
            config.MinSampleSize,
            config.StakeMethod,
            config.AutoUpdateStakes);

        await _bankroll.UpdateConfigAsync(bankrollConfig);
        return Ok(new { status = "Config updated" });
    }

    [HttpGet("bankroll/config")]
    public async Task<IActionResult> GetBankrollConfig()
    {
        var config = await _bankroll.GetConfigAsync();
        return Ok(config);
    }

    [HttpPost("bankroll/calculate-stakes")]
    public async Task<IActionResult> CalculateStakes(
        [FromQuery] DateTime? date = null,
        [FromQuery] int topN = 10)
    {
        var targetDate = date ?? DateTime.UtcNow;
        var picks = await _persistence.GetDailyPicksAsync(targetDate);
        var pending = picks.Where(p => p.Status == "Pending").Take(topN).ToList();

        var recommendations = await _bankroll.CalculateStakesAsync(pending);

        return Ok(new
        {
            date = targetDate.Date,
            totalPicks = picks.Count,
            pendingPicks = pending.Count,
            recommendations
        });
    }

    [HttpPost("bankroll/apply-result")]
    public async Task<IActionResult> ApplyBankrollResult(
        [FromBody] BankrollResultRequest request)
    {
        var state = await _bankroll.ApplyResultAsync(
            request.PickId,
            request.Won,
            request.Odds,
            request.StakeUnits);

        return Ok(state);
    }

    [HttpPost("bankroll/evaluate-and-apply")]
    public async Task<IActionResult> EvaluateAndApply(
        [FromQuery] DateTime date,
        [FromBody] List<PickEvaluationRequest> evaluations)
    {
        var results = new List<object>();

        foreach (var eval in evaluations)
        {
            var picks = await _persistence.GetDailyPicksAsync(date);
            var target = picks.FirstOrDefault(p => p.Id == eval.PickId);

            if (target == null)
                continue;

            var won = eval.Result == "Won";
            var profit = won ? (target.Odds - 1) * eval.StakeUnits : -eval.StakeUnits;

            await _persistence.UpdatePickOutcomeAsync(
                eval.PickId,
                won,
                profit,
                eval.StakeUnits,
                0,
                eval.ResultDetail ?? $"{eval.Result} at {DateTime.UtcNow:yyyy-MM-dd HH:mm}");

            var state = await _bankroll.ApplyResultAsync(
                eval.PickId,
                won,
                target.Odds,
                eval.StakeUnits);

            results.Add(new
            {
                pickId = eval.PickId,
                team = target.Team,
                market = target.Market,
                result = eval.Result,
                profit,
                odds = target.Odds,
                stakeUnits = eval.StakeUnits,
                newBalance = state.CurrentBalance
            });
        }

        return Ok(new
        {
            date = date.Date,
            evaluated = results.Count,
            results
        });
    }

    [HttpGet("pipeline/status")]
    public async Task<IActionResult> GetPipelineStatus()
    {
        var status = await _orchestrator.GetStatusAsync();
        return Ok(status);
    }

    [HttpGet("pipeline/fixtures")]
    public async Task<IActionResult> GetTodayFixtures()
    {
        var fixtures = await _orchestrator.GetTodayFixturesAsync();
        return Ok(new
        {
            count = fixtures.Count,
            fixtures
        });
    }

    [HttpPost("pipeline/run")]
    public async Task<IActionResult> RunPipeline(
        [FromQuery] int maxFixtures = 50,
        [FromQuery] int topPicksPerFixture = 10,
        [FromQuery] int topPicksOverall = 30)
    {
        var result = await _orchestrator.RunDailyPipelineAsync(
            maxFixtures: maxFixtures,
            topPicksPerFixture: topPicksPerFixture,
            topPicksOverall: topPicksOverall);

        return Ok(result);
    }
}

public class PickEvaluationRequest
{
    public int PickId { get; set; }
    public string Result { get; set; } = "Pending";
    public double StakeUnits { get; set; } = 1.0;
    public string? ResultDetail { get; set; }
}

public class BankrollConfigRequest
{
    public double InitialBankroll { get; set; } = 100.0;
    public double KellyFraction { get; set; } = 0.25;
    public double MaxStakePerPick { get; set; } = 5.0;
    public double MaxDailyExposure { get; set; } = 20.0;
    public double MaxDrawdownPercent { get; set; } = 0.30;
    public double MinEdge { get; set; } = 0.02;
    public double MinSampleSize { get; set; } = 5;
    public string StakeMethod { get; set; } = "Kelly";
    public bool AutoUpdateStakes { get; set; } = true;
}

public class BankrollResultRequest
{
    public int PickId { get; set; }
    public bool Won { get; set; }
    public double Odds { get; set; }
    public double StakeUnits { get; set; } = 1.0;
}
