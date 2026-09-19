using System.Text.Json;
using MatchEdge.Application.Clients.FootyMetrics;
using MatchEdge.Application.Services;
using MatchEdge.Infrastructure.Clients;
using MatchEdge.Infrastructure.Configuration;
using MatchEdge.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MatchEdge.Infrastructure.Services;

public class OrchestratorService : IOrchestratorService
{
    private readonly FootyMetricsBrowserManager _browserManager;
    private readonly FootyMetricsBrowserCollector _collector;
    private readonly FootyMetricsDomScraper _scraper;
    private readonly ITrendPersistenceService _persistence;
    private readonly ITrendBacktestingService _backtesting;
    private readonly IBankrollManager _bankroll;
    private readonly FootyMetricsOptions _options;
    private readonly ILogger<OrchestratorService> _logger;

    private static PipelineStatus _status = new(
        false, null, null, 0, "", null);

    public OrchestratorService(
        FootyMetricsBrowserManager browserManager,
        FootyMetricsBrowserCollector collector,
        FootyMetricsDomScraper scraper,
        ITrendPersistenceService persistence,
        ITrendBacktestingService backtesting,
        IBankrollManager bankroll,
        IOptions<FootyMetricsOptions> options,
        ILogger<OrchestratorService> logger)
    {
        _browserManager = browserManager;
        _collector = collector;
        _scraper = scraper;
        _persistence = persistence;
        _backtesting = backtesting;
        _bankroll = bankroll;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<PipelineResult> RunDailyPipelineAsync(
        DateTime? date = null,
        int maxFixtures = 50,
        int topPicksPerFixture = 10,
        int topPicksOverall = 30,
        CancellationToken ct = default)
    {
        var startTime = DateTime.UtcNow;
        var targetDate = date ?? DateTime.UtcNow;

        if (_status.IsRunning)
        {
            return new PipelineResult(
                targetDate, false, "Pipeline already running",
                0, 0, 0, 0, 0, 0, 0, [], [], TimeSpan.Zero);
        }

        _status = _status with { IsRunning = true };
        _logger.LogInformation("Starting daily pipeline for {Date}", targetDate.Date);

        try
        {
            if (!_browserManager.IsReady)
            {
                _logger.LogInformation("Browser not ready, starting...");
                await _scraper.StartAsync();
                await Task.Delay(10000, ct);
            }

            var fixtures = await FetchFixturesAsync(ct);
            if (fixtures.Count == 0)
            {
                _logger.LogWarning("No fixtures found");
                _status = _status with
                {
                    IsRunning = false,
                    LastRun = DateTime.UtcNow,
                    LastResult = "No fixtures found"
                };
                return new PipelineResult(
                    targetDate, false, "No fixtures found",
                    0, 0, 0, 0, 0, 0, 0, [], [], DateTime.UtcNow - startTime);
            }

            var fixturesWithTrends = fixtures
                .Where(f => f.HasTrends)
                .Take(maxFixtures)
                .ToList();

            _logger.LogInformation(
                "Found {Total} fixtures, {WithTrends} with trends",
                fixtures.Count, fixturesWithTrends.Count);

            var allPicks = new List<FixtureSummary>();
            var totalTrends = 0;
            var totalPicksScored = 0;
            var allScoredPicks = new List<TrendScoreResult>();

            foreach (var fixture in fixturesWithTrends)
            {
                if (ct.IsCancellationRequested) break;

                try
                {
                    _logger.LogInformation(
                        "Scraping {Home} vs {Away} ({League})",
                        fixture.HomeTeam, fixture.AwayTeam, fixture.League);

                    var trends = await _scraper.ScrapeFixtureTrendsAsync(
                        fixture.Slug, ct);

                    if (trends.Count == 0)
                    {
                        allPicks.Add(new FixtureSummary(
                            fixture.Slug, fixture.HomeTeam, fixture.AwayTeam,
                            fixture.League, true, 0, 0, "No trends"));
                        continue;
                    }

                    await _persistence.SaveTrendsAsync(
                        trends, fixture.Slug, fixture.HomeTeam,
                        fixture.AwayTeam, fixture.League, ct);

                    var scored = FootyMetricsScoringEngine.RankScrapedPicks(
                        trends, topPicksPerFixture);

                    await _persistence.SaveScoredPicksAsync(
                        scored, targetDate, "FootyMetrics", ct);

                    allScoredPicks.AddRange(scored);
                    totalTrends += trends.Count;
                    totalPicksScored += scored.Count;

                    allPicks.Add(new FixtureSummary(
                        fixture.Slug, fixture.HomeTeam, fixture.AwayTeam,
                        fixture.League, true, trends.Count, scored.Count, "Scraped"));

                    _logger.LogInformation(
                        "Scraped {Trends} trends, scored {Picks} picks",
                        trends.Count, scored.Count);

                    await Task.Delay(2000, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to scrape {Slug}", fixture.Slug);
                    allPicks.Add(new FixtureSummary(
                        fixture.Slug, fixture.HomeTeam, fixture.AwayTeam,
                        fixture.League, true, 0, 0, $"Error: {ex.Message}"));
                }
            }

            var topPicks = allScoredPicks
                .OrderByDescending(p => p.CompositeScore)
                .Take(topPicksOverall)
                .ToList();

            var recommendations = await _bankroll.CalculateStakesAsync(
                topPicks.Select(p => new DailyPickEntityDto(
                    0, targetDate, DateTime.UtcNow,
                    p.TeamName, p.Market, $"{p.Direction} {p.Line}",
                    p.CompositeScore, p.Classification, p.Edge,
                    (double)(p.OddsValue ?? 0), p.HitRate, p.SampleSize,
                    0, "FootyMetrics", "Pending")).ToList(), ct);

            var duration = DateTime.UtcNow - startTime;

            _status = _status with
            {
                IsRunning = false,
                LastRun = DateTime.UtcNow,
                LastDuration = duration,
                TotalRunsToday = _status.TotalRunsToday + 1,
                LastResult = $"OK: {totalPicksScored} picks from {fixturesWithTrends.Count} fixtures"
            };

            _logger.LogInformation(
                "Pipeline complete: {Fixtures} fixtures, {Trends} trends, " +
                "{Picks} picks, {Duration:F1}s",
                fixturesWithTrends.Count, totalTrends, totalPicksScored,
                duration.TotalSeconds);

            return new PipelineResult(
                targetDate,
                true,
                "Pipeline completed successfully",
                fixtures.Count,
                fixturesWithTrends.Count,
                totalTrends,
                totalPicksScored,
                topPicks.Count,
                recommendations.Count(r => r.RecommendedStake > 0),
                recommendations.Sum(r => r.RecommendedStake),
                allPicks,
                recommendations.Where(r => r.RecommendedStake > 0).ToList(),
                duration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Pipeline failed");
            _status = _status with
            {
                IsRunning = false,
                LastRun = DateTime.UtcNow,
                LastResult = $"Failed: {ex.Message}"
            };

            return new PipelineResult(
                targetDate, false, ex.Message,
                0, 0, 0, 0, 0, 0, 0, [], [], DateTime.UtcNow - startTime);
        }
    }

    public async Task<PipelineStatus> GetStatusAsync(CancellationToken ct = default)
    {
        return _status;
    }

    public async Task<List<FixtureSummary>> GetTodayFixturesAsync(
        CancellationToken ct = default)
    {
        var fixtures = await FetchFixturesAsync(ct);
        return fixtures.Select(f => new FixtureSummary(
            f.Slug, f.HomeTeam, f.AwayTeam, f.League,
            f.HasTrends, 0, 0, "Pending")).ToList();
    }

    private async Task<List<FixtureInfo>> FetchFixturesAsync(CancellationToken ct)
    {
        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var apiPath = $"/api/front/fixtures?date={today}";

        _logger.LogInformation("Fetching fixtures from {Path}", apiPath);
        var json = await _collector.FetchJsonAsync(apiPath, ct);

        if (string.IsNullOrEmpty(json))
        {
            _logger.LogWarning("Failed to fetch fixtures from FootyMetrics");
            return [];
        }

        try
        {
            var leagues = JsonSerializer.Deserialize<List<LeagueData>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (leagues == null) return [];

            var fixtures = new List<FixtureInfo>();
            foreach (var league in leagues)
            {
                if (league.Fixtures == null) continue;

                foreach (var fixture in league.Fixtures)
                {
                    fixtures.Add(new FixtureInfo(
                        fixture.Slug ?? "",
                        fixture.Home?.Name ?? "",
                        fixture.Away?.Name ?? "",
                        league.Name ?? "",
                        fixture.HasTrends,
                        fixture.Timestamp));
                }
            }

            return fixtures
                .OrderBy(f => f.Timestamp)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse fixtures JSON");
            return [];
        }
    }

    private record FixtureInfo(
        string Slug,
        string HomeTeam,
        string AwayTeam,
        string League,
        bool HasTrends,
        string Timestamp);

    private class LeagueData
    {
        public string? Name { get; set; }
        public List<FixtureData>? Fixtures { get; set; }
    }

    private class FixtureData
    {
        public string? Slug { get; set; }
        public bool HasTrends { get; set; }
        public string? Timestamp { get; set; }
        public TeamData? Home { get; set; }
        public TeamData? Away { get; set; }
    }

    private class TeamData
    {
        public string? Name { get; set; }
    }
}
