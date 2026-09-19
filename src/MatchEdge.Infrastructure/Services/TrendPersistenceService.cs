using MatchEdge.Application.Clients.FootyMetrics;
using MatchEdge.Application.Services;
using MatchEdge.Infrastructure.Data;
using MatchEdge.Infrastructure.Data.Entities;
using MatchEdge.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MatchEdge.Infrastructure.Services;

public class TrendPersistenceService : ITrendPersistenceService
{
    private readonly MatchEdgeDbContext _db;
    private readonly ILogger<TrendPersistenceService> _logger;

    public TrendPersistenceService(
        MatchEdgeDbContext db,
        ILogger<TrendPersistenceService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<int> SaveTrendsAsync(
        List<FootyMetricsScrapedTrend> trends,
        string fixtureSlug,
        string homeTeam,
        string awayTeam,
        string league,
        CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var saved = 0;

        foreach (var trend in trends)
        {
            var existing = await _db.TrendResults
                .FirstOrDefaultAsync(t =>
                    t.FixtureSlug == fixtureSlug &&
                    t.Team == trend.Team &&
                    t.Market == trend.Market &&
                    t.Venue == trend.Venue,
                    ct);

            if (existing != null)
            {
                existing.HitCount = trend.HitCount;
                existing.HitNumerator = trend.HitNumerator;
                existing.HitDenominator = trend.HitDenominator;
                existing.HitRate = trend.HitRate;
                existing.OppHitRate = trend.OppHitRate;
                existing.AvgValue = trend.AvgValue;
                existing.RatePercentage = trend.RatePercentage;
                existing.Odds = ParseOdds(trend.Odds);
                existing.ScrapedAt = now;
            }
            else
            {
                var entity = new TrendResultEntity
                {
                    ScrapedAt = now,
                    FixtureSlug = fixtureSlug,
                    FixtureApid = ParseFixtureApid(fixtureSlug),
                    HomeTeam = homeTeam,
                    AwayTeam = awayTeam,
                    League = league,
                    Team = trend.Team,
                    TeamSlug = trend.TeamSlug,
                    Market = trend.Market,
                    Description = trend.Description,
                    Venue = trend.Venue,
                    HitCount = trend.HitCount,
                    HitNumerator = trend.HitNumerator,
                    HitDenominator = trend.HitDenominator,
                    HitRate = trend.HitRate,
                    OppHitRate = trend.OppHitRate,
                    AvgValue = trend.AvgValue,
                    RatePercentage = trend.RatePercentage,
                    Odds = ParseOdds(trend.Odds),
                    Fixture = trend.Fixture,
                    Score = 0,
                    Classification = "",
                    Edge = 0,
                    IsSelected = false
                };

                _db.TrendResults.Add(entity);
            }

            saved++;
        }

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Saved {Count} trends for fixture {Slug}", saved, fixtureSlug);
        return saved;
    }

    public async Task<int> SaveScoredPicksAsync(
        List<TrendScoreResult> picks,
        DateTime date,
        string source = "FootyMetrics",
        CancellationToken ct = default)
    {
        var saved = 0;
        var dateOnly = date.Date;
        var nextDay = dateOnly.AddDays(1);
        var seenKeys = new HashSet<string>();

        foreach (var pick in picks)
        {
            var key = $"{pick.TeamName}|{pick.Market}";
            if (!seenKeys.Add(key))
                continue;

            var existing = await _db.DailyPicks
                .FirstOrDefaultAsync(p =>
                    p.Date >= dateOnly &&
                    p.Date < nextDay &&
                    p.Team == pick.TeamName &&
                    p.Market == pick.Market,
                    ct);

            if (existing != null)
            {
                existing.Score = pick.CompositeScore;
                existing.Classification = pick.Classification;
                existing.Edge = pick.Edge;
                existing.Odds = (double)(pick.OddsValue ?? 0);
                existing.HitRate = pick.HitRate;
                existing.SampleSize = pick.SampleSize;
            }
            else
            {
                var entity = new DailyPickEntity
                {
                    Date = date,
                    CreatedAt = DateTime.UtcNow,
                    TrendResultId = 0,
                    Team = pick.TeamName,
                    Market = pick.Market,
                    Description = $"{pick.Direction} {pick.Line}",
                    Score = pick.CompositeScore,
                    Classification = pick.Classification,
                    Edge = pick.Edge,
                    Odds = (double)(pick.OddsValue ?? 0),
                    HitRate = pick.HitRate,
                    SampleSize = pick.SampleSize,
                    StakeUnits = 0,
                    Source = source,
                    Status = "Pending"
                };

                _db.DailyPicks.Add(entity);
            }

            saved++;
        }

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Saved {Count} scored picks for {Date}", saved, date.Date);
        return saved;
    }

    public async Task<List<TrendResultEntityDto>> GetTrendsByDateAsync(
        DateTime date,
        CancellationToken ct = default)
    {
        return await _db.TrendResults
            .Where(t => t.ScrapedAt.Date == date.Date)
            .OrderByDescending(t => t.Score)
            .Select(t => MapToDto(t))
            .ToListAsync(ct);
    }

    public async Task<List<TrendResultEntityDto>> GetTrendsByFixtureAsync(
        string fixtureSlug,
        CancellationToken ct = default)
    {
        return await _db.TrendResults
            .Where(t => t.FixtureSlug == fixtureSlug)
            .OrderByDescending(t => t.Score)
            .Select(t => MapToDto(t))
            .ToListAsync(ct);
    }

    public async Task<List<DailyPickEntityDto>> GetDailyPicksAsync(
        DateTime date,
        CancellationToken ct = default)
    {
        return await _db.DailyPicks
            .Where(p => p.Date.Date == date.Date)
            .OrderByDescending(p => p.Score)
            .Select(p => MapToDto(p))
            .ToListAsync(ct);
    }

    public async Task<List<DailyPickEntityDto>> GetPendingPicksAsync(
        CancellationToken ct = default)
    {
        return await _db.DailyPicks
            .Where(p => p.Status == "Pending")
            .OrderByDescending(p => p.Score)
            .Select(p => MapToDto(p))
            .ToListAsync(ct);
    }

    public async Task UpdatePickOutcomeAsync(
        int dailyPickId,
        bool won,
        double profit,
        double profitUnits,
        double balanceAfter,
        string resultDetail,
        CancellationToken ct = default)
    {
        var pick = await _db.DailyPicks.FindAsync(new object[] { dailyPickId }, ct);
        if (pick == null) return;

        pick.Status = won ? "Won" : "Lost";

        var outcome = new PickOutcomeEntity
        {
            DailyPickId = dailyPickId,
            MatchDate = DateTime.UtcNow,
            Team = pick.Team,
            Market = pick.Market,
            Odds = pick.Odds,
            Won = won,
            Profit = profit,
            ProfitUnits = profitUnits,
            BalanceAfter = balanceAfter,
            ResultDetail = resultDetail,
            EvaluatedAt = DateTime.UtcNow
        };

        _db.PickOutcomes.Add(outcome);
        await _db.SaveChangesAsync(ct);
    }

    private static TrendResultEntityDto MapToDto(TrendResultEntity t)
    {
        return new TrendResultEntityDto(
            t.Id,
            t.ScrapedAt,
            t.FixtureSlug,
            t.HomeTeam,
            t.AwayTeam,
            t.League,
            t.Team,
            t.Market,
            t.Description,
            t.Venue,
            t.HitCount,
            t.HitNumerator,
            t.HitDenominator,
            t.HitRate,
            t.OppHitRate,
            t.AvgValue,
            t.RatePercentage,
            t.Odds,
            t.Score,
            t.Classification,
            t.Edge,
            t.IsSelected
        );
    }

    private static DailyPickEntityDto MapToDto(DailyPickEntity p)
    {
        return new DailyPickEntityDto(
            p.Id,
            p.Date,
            p.CreatedAt,
            p.Team,
            p.Market,
            p.Description,
            p.Score,
            p.Classification,
            p.Edge,
            p.Odds,
            p.HitRate,
            p.SampleSize,
            p.StakeUnits,
            p.Source,
            p.Status
        );
    }

    private static double ParseOdds(string oddsStr)
    {
        if (string.IsNullOrWhiteSpace(oddsStr))
            return 0;

        if (double.TryParse(oddsStr, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var result))
            return result;

        return 0;
    }

    private static long ParseFixtureApid(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return 0;

        var parts = slug.Split('-');
        if (parts.Length > 0 && long.TryParse(parts[0], out var apid))
            return apid;

        return 0;
    }
}
