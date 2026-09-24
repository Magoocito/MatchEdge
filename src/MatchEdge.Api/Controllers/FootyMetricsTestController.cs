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
    private readonly IWebHostEnvironment _env;
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
        IWebHostEnvironment env,
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
        _env = env;
        _logger = logger;
    }

    private bool EvalAllowed => _env.IsDevelopment();

    private IActionResult EvalForbidden() =>
        NotFound(new { error = "/eval is restricted to Development environment." });

    private IActionResult DevOnlyForbidden(string endpoint) =>
        NotFound(new { error = $"/{endpoint} is restricted to Development environment." });

    [HttpPost("start")]
    public async Task<IActionResult> StartBrowser()
    {
        await _scraper.StartAsync();
        return Ok(new { status = "Chrome started for FootyMetrics", profile = "persistent-footymetrics" });
    }

    [HttpPost("start-visible")]
    public async Task<IActionResult> StartBrowserVisible()
    {
        await _scraper.StartAsync(headless: false);
        return Ok(new { status = "Chrome started VISIBLE for FootyMetrics - Login with Google manually", profile = "persistent-footymetrics" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var page = _browserManager.GetPage();
        if (page == null)
            return BadRequest(new { error = "Browser not started. Call /start first." });

        try
        {
            // Navegar a la página de login
            await page.GotoAsync("https://www.footymetrics.com/login", new Microsoft.Playwright.PageGotoOptions
            {
                Timeout = 30000,
                WaitUntil = Microsoft.Playwright.WaitUntilState.Load
            });

            await Task.Delay(5000);

            // Aceptar cookies si aparecen
            try
            {
                var acceptBtn = page.Locator("button:has-text('Accept all')");
                if (await acceptBtn.CountAsync() > 0)
                {
                    await acceptBtn.First.ClickAsync();
                    await Task.Delay(1000);
                }
            }
            catch { }

            // Llenar email - selectores conocidos
            var emailInput = page.Locator("input[name='email']");
            if (await emailInput.CountAsync() == 0)
                emailInput = page.Locator("input[type='email']");

            if (await emailInput.CountAsync() > 0)
            {
                await emailInput.First.ClearAsync();
                await emailInput.First.FillAsync(request.Email);
            }
            else
            {
                return BadRequest(new { error = "No se encontró campo de email" });
            }

            await Task.Delay(500);

            // Llenar password - selectores conocidos
            var passwordInput = page.Locator("input[name='password']");
            if (await passwordInput.CountAsync() == 0)
                passwordInput = page.Locator("input[type='password']");

            if (await passwordInput.CountAsync() > 0)
            {
                await passwordInput.First.ClearAsync();
                await passwordInput.First.FillAsync(request.Password);
            }
            else
            {
                return BadRequest(new { error = "No se encontró campo de password" });
            }

            await Task.Delay(500);

            // Click en "Sign in" button
            var signInBtn = page.Locator("button[type='submit']:has-text('Sign in')");
            if (await signInBtn.CountAsync() > 0)
            {
                await signInBtn.First.ClickAsync();
            }
            else
            {
                // Fallback: buscar cualquier submit button
                var submitBtn = page.Locator("button[type='submit']");
                if (await submitBtn.CountAsync() > 0)
                {
                    await submitBtn.First.ClickAsync();
                }
            }

            // Esperar a que redirija
            await Task.Delay(10000);

            // Verificar login exitoso
            var currentUrl = page.Url;
            var title = await page.TitleAsync();
            var finalText = await page.EvaluateAsync<string>("() => document.body?.innerText?.substring(0, 500) || ''");

            bool isLoggedIn = !currentUrl.Contains("login") && 
                            !finalText.ToLower().Contains("sign in") &&
                            !finalText.ToLower().Contains("log in");

            return Ok(new
            {
                success = isLoggedIn,
                url = currentUrl,
                title,
                message = isLoggedIn ? "Login exitoso - Sesión guardada en perfil persistente" : "Login puede haber fallado",
                contentPreview = finalText
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("login-status")]
    public async Task<IActionResult> LoginStatus()
    {
        var page = _browserManager.GetPage();
        if (page == null)
            return BadRequest(new { error = "Browser not started" });

        try
        {
            var url = page.Url;
            var title = await page.TitleAsync();
            var text = await page.EvaluateAsync<string>("() => document.body?.innerText?.substring(0, 500) || ''");

            bool isLoggedIn = !url.Contains("login") && 
                            !text.ToLower().Contains("sign in") &&
                            !text.ToLower().Contains("log in");

            return Ok(new
            {
                loggedIn = isLoggedIn,
                url,
                title,
                contentPreview = text
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("eval")]
    public async Task<IActionResult> EvalJs([FromBody] EvalRequest request)
    {
        if (!EvalAllowed) return EvalForbidden();
        var page = _browserManager.GetPage();
        if (page == null)
            return BadRequest(new { error = "Browser not started" });

        try
        {
            var result = await page.EvaluateAsync<string>(request.Script);
            return Ok(new { result, url = page.Url });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("navigate")]
    public async Task<IActionResult> Navigate([FromBody] NavigateRequest request)
    {
        if (!_env.IsDevelopment()) return DevOnlyForbidden("navigate");
        var page = _browserManager.GetPage();
        if (page == null)
            return BadRequest(new { error = "Browser not started" });

        try
        {
            await page.GotoAsync(request.Url, new Microsoft.Playwright.PageGotoOptions
            {
                Timeout = 30000,
                WaitUntil = Microsoft.Playwright.WaitUntilState.Load
            });

            await Task.Delay(5000);

            var title = await page.TitleAsync();
            var text = await page.EvaluateAsync<string>("() => document.body?.innerText?.substring(0, 500) || ''");

            return Ok(new { url = page.Url, title, contentPreview = text });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>V6: opens one fixture tab with a fresh EMPTY Chrome profile (no login).</summary>
    [HttpGet("probe-nonpremium")]
    public async Task<IActionResult> ProbeNonPremium([FromQuery] string slug)
    {
        if (!_env.IsDevelopment()) return DevOnlyForbidden("probe-nonpremium");
        if (string.IsNullOrWhiteSpace(slug))
            return BadRequest(new { error = "slug is required (fixture slug like '33441813-...')." });

        var profileDir = Path.Combine(
            AppContext.BaseDirectory, "chrome-profile-fm-nonpremium-" + Guid.NewGuid().ToString("N")[..8]);
        Microsoft.Playwright.IPlaywright? playwright = null;
        Microsoft.Playwright.IBrowserContext? ctx = null;
        try
        {
            playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            ctx = await playwright.Chromium.LaunchPersistentContextAsync(
                profileDir,
                new Microsoft.Playwright.BrowserTypeLaunchPersistentContextOptions
                {
                    ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe",
                    Headless = true,
                    ViewportSize = new Microsoft.Playwright.ViewportSize { Width = 1280, Height = 800 },
                    Args = [
                        "--disable-blink-features=AutomationControlled",
                        "--no-first-run",
                        "--no-default-browser-check",
                        "--disable-gpu",
                        "--disable-dev-shm-usage",
                        "--no-sandbox"
                    ]
                });

            var page = ctx.Pages.Count > 0 ? ctx.Pages[0] : await ctx.NewPageAsync();

            var apiTcs = new TaskCompletionSource<(int Status, string Url, string Body)>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            var apiRx = new System.Text.RegularExpressions.Regex(
                @"/api/front/trends/fixtures/\d+/(players|teams)");
            EventHandler<Microsoft.Playwright.IResponse> handler = async (_, resp) =>
            {
                if (!apiRx.IsMatch(resp.Url)) return;
                string body = "";
                try { body = await resp.TextAsync(); } catch { }
                apiTcs.TrySetResult((resp.Status, resp.Url, body));
            };
            page.Response += handler;

            var url = $"https://www.footymetrics.com/fixtures/{slug}?tab=team-trends";
            string finalUrl;
            try
            {
                await page.GotoAsync(url, new Microsoft.Playwright.PageGotoOptions
                {
                    Timeout = 45000,
                    WaitUntil = Microsoft.Playwright.WaitUntilState.DOMContentLoaded
                });
            }
            finally
            {
                page.Response -= handler;
            }

            (int Status, string Url, string Body) api;
            string apiState;
            try
            {
                api = await apiTcs.Task.WaitAsync(TimeSpan.FromSeconds(15));
                apiState = "RESPONDED";
            }
            catch (TimeoutException)
            {
                api = (0, "", "");
                apiState = "NO_RESPONSE_15S";
            }

            finalUrl = page.Url;
            var title = await page.TitleAsync();
            var text = await page.EvaluateAsync<string>(
                "() => document.body?.innerText?.substring(0, 400) || ''") ?? "";

            int? dataCount = null;
            int? totalCount = null;
            if (api.Body.Length > 0)
            {
                try
                {
                    using var doc = System.Text.Json.JsonDocument.Parse(api.Body);
                    if (doc.RootElement.TryGetProperty("data", out var data) &&
                        data.ValueKind == System.Text.Json.JsonValueKind.Array)
                        dataCount = data.GetArrayLength();
                    if (doc.RootElement.TryGetProperty("pagination", out var pg) &&
                        pg.TryGetProperty("count", out var c))
                        totalCount = c.GetInt32();
                }
                catch { /* non-json body */ }
            }

            var evidence = new
            {
                slug,
                finalUrl,
                redirectedToLogin = finalUrl.Contains("/login"),
                apiState,
                apiStatus = api.Status,
                apiUrl = api.Url,
                dataCount,
                totalCount,
                title,
                bodyPreview = text.Replace("\n", " ").Substring(0, Math.Min(300, text.Length)),
                capturedAtUtc = DateTime.UtcNow
            };

            var dir = Path.Combine(AppContext.BaseDirectory, "tmp", "fm", "nonpremium");
            Directory.CreateDirectory(dir);
            var rawPath = Path.Combine(dir, $"{DateTime.UtcNow:yyyyMMddHHmmssfff}.json");
            await System.IO.File.WriteAllTextAsync(
                rawPath,
                System.Text.Json.JsonSerializer.Serialize(evidence, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));

            return Ok(new { evidence, rawPath });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        finally
        {
            if (ctx != null)
            {
                try { await ctx.CloseAsync(); } catch { }
                try { await ctx.DisposeAsync(); } catch { }
            }
            playwright?.Dispose();
            try
            {
                for (var i = 0; i < 5 && Directory.Exists(profileDir); i++)
                {
                    try { Directory.Delete(profileDir, true); break; }
                    catch { await Task.Delay(500); }
                }
            }
            catch { /* leftover temp profile is harmless */ }
        }
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
        if (!EvalAllowed) return EvalForbidden();
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

    [HttpGet("page-content-full")]
    public async Task<IActionResult> GetPageContentFull([FromQuery] string url)
    {
        var page = _browserManager.GetPage();
        if (page == null)
            return BadRequest(new { error = "Browser not started" });

        try
        {
            await page.GotoAsync(url, new Microsoft.Playwright.PageGotoOptions
            {
                Timeout = 45000,
                WaitUntil = Microsoft.Playwright.WaitUntilState.Load
            });

            await Task.Delay(15000);

            var title = await page.TitleAsync();
            var text = await page.EvaluateAsync<string>("() => document.body?.innerText || ''");

            return Ok(new
            {
                url = page.Url,
                title,
                contentPreview = text.Length > 20000 ? text.Substring(0, 20000) : text,
                contentLength = text.Length
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Obsolete("Fragile innerText/scroll parser. Use POST /api/fm/snapshot (JSON-based) instead.")]
    [HttpGet("fixture-trends-v2")]
    public async Task<IActionResult> GetFixtureTrendsV2(
        [FromQuery] string url,
        [FromQuery] int scrollCount = 3,
        [FromQuery] int waitSeconds = 20)
    {
        var page = _browserManager.GetPage();
        if (page == null)
            return BadRequest(new { error = "Browser not started" });

        try
        {
            // Navegar directamente al tab de team trends
            var trendsUrl = url.Contains("?") ? 
                url.Split('?')[0] + "?tab=team-trends" : 
                url + "?tab=team-trends";

            await page.GotoAsync(trendsUrl, new Microsoft.Playwright.PageGotoOptions
            {
                Timeout = 45000,
                WaitUntil = Microsoft.Playwright.WaitUntilState.Load
            });

            // Esperar JS dinámico
            await Task.Delay(waitSeconds * 1000);

            // Scroll
            for (int i = 0; i < scrollCount; i++)
            {
                await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");
                await Task.Delay(2000);
            }

            await page.EvaluateAsync("window.scrollTo(0, 0)");
            await Task.Delay(1000);

            // Obtener contenido COMPLETO
            var title = await page.TitleAsync();
            var text = await page.EvaluateAsync<string>("() => document.body?.innerText || ''");

            return Ok(new
            {
                url = page.Url,
                title,
                contentPreview = text.Length > 15000 ? text.Substring(0, 15000) : text,
                contentLength = text.Length
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("fixture-trends-raw")]
    public async Task<IActionResult> GetFixtureTrendsRaw(
        [FromQuery] string url,
        [FromQuery] int waitSeconds = 20)
    {
        var page = _browserManager.GetPage();
        if (page == null)
            return BadRequest(new { error = "Browser not started" });

        try
        {
            // Navegar directamente al tab de team trends
            var trendsUrl = url.Contains("?") ? 
                url.Split('?')[0] + "?tab=team-trends" : 
                url + "?tab=team-trends";

            await page.GotoAsync(trendsUrl, new Microsoft.Playwright.PageGotoOptions
            {
                Timeout = 45000,
                WaitUntil = Microsoft.Playwright.WaitUntilState.Load
            });

            await Task.Delay(waitSeconds * 1000);

            // Scroll
            for (int i = 0; i < 3; i++)
            {
                await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");
                await Task.Delay(2000);
            }

            await page.EvaluateAsync("window.scrollTo(0, 0)");
            await Task.Delay(1000);

            // Obtener contenido COMPLETO
            var text = await page.EvaluateAsync<string>("() => document.body?.innerText || ''");

            // Obtener HTML de trends
            var html = await page.EvaluateAsync<string>(@"() => {
                const trendCards = document.querySelectorAll('[class*=""trend-card""], [class*=""TrendCard""], [class*=""trend-item""]');
                if (trendCards.length > 0) {
                    return Array.from(trendCards).map(c => c.innerText).join('\\n---\\n');
                }
                return '';
            }");

            return Ok(new
            {
                url = page.Url,
                textLength = text.Length,
                htmlLength = html.Length,
                fullText = text,
                trendCardsHtml = html
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
        if (!_env.IsDevelopment()) return DevOnlyForbidden("intercept");
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

    [HttpPost("bets/register")]
    public async Task<IActionResult> RegisterBet([FromBody] RegisterBetRequest request)
    {
        using var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=C:\\Services\\MatchEdge\\matchedge.db");
        await connection.OpenAsync();

        var insertCmd = connection.CreateCommand();
        insertCmd.CommandText = @"
            INSERT INTO Bets (BetDate, MatchDate, HomeTeam, AwayTeam, League, Market, Selection, Odds, Stake, PotentialReturn, Status, Notes, Source, ContextJson)
            VALUES ($betDate, $matchDate, $homeTeam, $awayTeam, $league, $market, $selection, $odds, $stake, $potentialReturn, 'Pending', $notes, $source, $contextJson);
            SELECT last_insert_rowid();";

        insertCmd.Parameters.AddWithValue("$betDate", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm"));
        insertCmd.Parameters.AddWithValue("$matchDate", request.MatchDate);
        insertCmd.Parameters.AddWithValue("$homeTeam", request.HomeTeam);
        insertCmd.Parameters.AddWithValue("$awayTeam", request.AwayTeam);
        insertCmd.Parameters.AddWithValue("$league", request.League ?? "");
        insertCmd.Parameters.AddWithValue("$market", request.Market);
        insertCmd.Parameters.AddWithValue("$selection", request.Selection);
        insertCmd.Parameters.AddWithValue("$odds", request.Odds);
        insertCmd.Parameters.AddWithValue("$stake", request.Stake);
        insertCmd.Parameters.AddWithValue("$potentialReturn", request.Stake * request.Odds);
        insertCmd.Parameters.AddWithValue("$notes", request.Notes ?? "");
        insertCmd.Parameters.AddWithValue("$source", request.Source ?? "FootyMetrics");
        insertCmd.Parameters.AddWithValue("$contextJson", request.ContextJson ?? "{}");

        var id = await insertCmd.ExecuteScalarAsync();

        return Ok(new { betId = id, message = "Apuesta registrada" });
    }

    [HttpPost("bets/evaluate")]
    public async Task<IActionResult> EvaluateBet([FromBody] EvaluateBetRequest request)
    {
        using var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=C:\\Services\\MatchEdge\\matchedge.db");
        await connection.OpenAsync();

        var updateCmd = connection.CreateCommand();
        var profit = request.Won ? (request.Odds - 1) * request.Stake : -request.Stake;

        updateCmd.CommandText = @"
            UPDATE Bets SET 
                Status = CASE WHEN $won = 1 THEN 'Won' ELSE 'Lost' END,
                Result = $result,
                Profit = $profit,
                EvaluatedAt = datetime('now')
            WHERE Id = $betId;";
        updateCmd.Parameters.AddWithValue("$betId", request.BetId);
        updateCmd.Parameters.AddWithValue("$won", request.Won ? 1 : 0);
        updateCmd.Parameters.AddWithValue("$result", request.Result ?? "");
        updateCmd.Parameters.AddWithValue("$profit", profit);

        await updateCmd.ExecuteNonQueryAsync();

        // Update bankroll
        var updateBankroll = connection.CreateCommand();
        updateBankroll.CommandText = @"
            UPDATE BankrollStates SET 
                CurrentBalance = CurrentBalance + $profit,
                TotalProfit = TotalProfit + $profit,
                TotalPicks = TotalPicks + 1,
                Won = Won + CASE WHEN $won = 1 THEN 1 ELSE 0 END,
                Lost = Lost + CASE WHEN $won = 0 THEN 1 ELSE 0 END,
                LastUpdated = datetime('now')
            WHERE Id = 1;";
        updateBankroll.Parameters.AddWithValue("$profit", profit);
        updateBankroll.Parameters.AddWithValue("$won", request.Won ? 1 : 0);
        await updateBankroll.ExecuteNonQueryAsync();

        return Ok(new { betId = request.BetId, profit, message = request.Won ? "Ganada" : "Perdida" });
    }

    [HttpGet("bets/today")]
    public async Task<IActionResult> GetTodayBets()
    {
        using var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=C:\\Services\\MatchEdge\\matchedge.db");
        await connection.OpenAsync();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM Bets WHERE date(BetDate) = date('now') ORDER BY MatchDate";
        var reader = await cmd.ExecuteReaderAsync();

        var bets = new List<object>();
        while (await reader.ReadAsync())
        {
            bets.Add(new
            {
                id = reader.GetInt32(0),
                betDate = reader.GetString(1),
                matchDate = reader.GetString(2),
                homeTeam = reader.GetString(3),
                awayTeam = reader.GetString(4),
                league = reader.GetString(5),
                market = reader.GetString(6),
                selection = reader.GetString(7),
                odds = reader.GetDouble(8),
                stake = reader.GetDouble(9),
                potentialReturn = reader.GetDouble(10),
                status = reader.GetString(11),
                result = reader.IsDBNull(12) ? (string?)null : reader.GetString(12),
                profit = reader.IsDBNull(13) ? (double?)null : reader.GetDouble(13),
                notes = reader.IsDBNull(14) ? (string?)null : reader.GetString(14)
            });
        }

        return Ok(new { date = DateTime.UtcNow.Date, count = bets.Count, bets });
    }

    [HttpGet("bets/history")]
    public async Task<IActionResult> GetBetHistory([FromQuery] int days = 30)
    {
        using var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=C:\\Services\\MatchEdge\\matchedge.db");
        await connection.OpenAsync();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT * FROM Bets 
            WHERE BetDate >= datetime('now', '-' || $days || ' days')
            ORDER BY BetDate DESC";
        cmd.Parameters.AddWithValue("$days", days);
        var reader = await cmd.ExecuteReaderAsync();

        var bets = new List<object>();
        while (await reader.ReadAsync())
        {
            bets.Add(new
            {
                id = reader.GetInt32(0),
                betDate = reader.GetString(1),
                matchDate = reader.GetString(2),
                homeTeam = reader.GetString(3),
                awayTeam = reader.GetString(4),
                market = reader.GetString(6),
                selection = reader.GetString(7),
                odds = reader.GetDouble(8),
                stake = reader.GetDouble(9),
                status = reader.GetString(11),
                profit = reader.IsDBNull(13) ? (double?)null : reader.GetDouble(13)
            });
        }

        var won = bets.Count(b => ((dynamic)b).status == "Won");
        var lost = bets.Count(b => ((dynamic)b).status == "Lost");
        var pending = bets.Count(b => ((dynamic)b).status == "Pending");
        var totalProfit = bets.Where(b => ((dynamic)b).profit != null).Sum(b => (double)((dynamic)b).profit);

        return Ok(new
        {
            totalBets = bets.Count,
            won,
            lost,
            pending,
            winRate = won + lost > 0 ? (double)won / (won + lost) * 100 : 0,
            totalProfit,
            bets
        });
    }

    [HttpGet("bets/bankroll")]
    public async Task<IActionResult> GetBankroll()
    {
        using var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=C:\\Services\\MatchEdge\\matchedge.db");
        await connection.OpenAsync();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM BankrollStates WHERE Id = 1";
        var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return Ok(new
            {
                initialBankroll = reader.GetDouble(1),
                currentBalance = reader.GetDouble(2),
                peakBalance = reader.GetDouble(3),
                maxDrawdown = reader.GetDouble(4),
                totalStaked = reader.GetDouble(6),
                totalProfit = reader.GetDouble(7),
                totalPicks = reader.GetInt32(8),
                won = reader.GetInt32(9),
                lost = reader.GetInt32(10),
                pending = reader.GetInt32(11),
                roi = reader.GetDouble(6) > 0 ? reader.GetDouble(7) / reader.GetDouble(6) * 100 : 0
            });
        }

        return Ok(new { error = "No bankroll found" });
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

public class RegisterBetRequest
{
    public string MatchDate { get; set; } = "";
    public string HomeTeam { get; set; } = "";
    public string AwayTeam { get; set; } = "";
    public string? League { get; set; }
    public string Market { get; set; } = "";
    public string Selection { get; set; } = "";
    public double Odds { get; set; }
    public double Stake { get; set; }
    public string? Notes { get; set; }
    public string? Source { get; set; }
    public string? ContextJson { get; set; }
}

public class EvaluateBetRequest
{
    public int BetId { get; set; }
    public bool Won { get; set; }
    public string? Result { get; set; }
    public double Odds { get; set; }
    public double Stake { get; set; }
}

// === MODELOS PARA FIXTURE TRENDS ===

public class FixtureTrendsRequest
{
    public string Url { get; set; } = "";
    public int ScrollCount { get; set; } = 3;
    public int WaitSeconds { get; set; } = 15;
}

public class TrendData
{
    public string Type { get; set; } = "";
    public string Team { get; set; } = "";
    public string HitRate { get; set; } = "";
    public string Avg { get; set; } = "";
    public string Odds { get; set; } = "";
    public string OppHits { get; set; } = "";
    public string H2H { get; set; } = "";
    public string Context { get; set; } = "";
    public string Total { get; set; } = "";
}

public class FixtureTrendsResponse
{
    public string Url { get; set; } = "";
    public string Title { get; set; } = "";
    public int TotalTrends { get; set; }
    public List<TrendData> Trends { get; set; } = new();
    public string RawContent { get; set; } = "";
    public int ContentLength { get; set; }
}

public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class EvalRequest
{
    public string Script { get; set; } = "";
}

public class NavigateRequest
{
    public string Url { get; set; } = "";
}
