using MatchEdge.Application.Clients.FootyMetrics;

namespace MatchEdge.Application.Services;

public interface ITrendPersistenceService
{
    Task<int> SaveTrendsAsync(
        List<FootyMetricsScrapedTrend> trends,
        string fixtureSlug,
        string homeTeam,
        string awayTeam,
        string league,
        CancellationToken ct = default);

    Task<int> SaveScoredPicksAsync(
        List<TrendScoreResult> picks,
        DateTime date,
        string source = "FootyMetrics",
        CancellationToken ct = default);

    Task<List<TrendResultEntityDto>> GetTrendsByDateAsync(
        DateTime date,
        CancellationToken ct = default);

    Task<List<TrendResultEntityDto>> GetTrendsByFixtureAsync(
        string fixtureSlug,
        CancellationToken ct = default);

    Task<List<DailyPickEntityDto>> GetDailyPicksAsync(
        DateTime date,
        CancellationToken ct = default);

    Task<List<DailyPickEntityDto>> GetPendingPicksAsync(
        CancellationToken ct = default);

    Task UpdatePickOutcomeAsync(
        int dailyPickId,
        bool won,
        double profit,
        double profitUnits,
        double balanceAfter,
        string resultDetail,
        CancellationToken ct = default);
}

public record TrendResultEntityDto(
    int Id,
    DateTime ScrapedAt,
    string FixtureSlug,
    string HomeTeam,
    string AwayTeam,
    string League,
    string Team,
    string Market,
    string Description,
    string Venue,
    string HitCount,
    int HitNumerator,
    int HitDenominator,
    double HitRate,
    int OppHitRate,
    double AvgValue,
    double RatePercentage,
    double Odds,
    double Score,
    string Classification,
    double Edge,
    bool IsSelected
);

public record DailyPickEntityDto(
    int Id,
    DateTime Date,
    DateTime CreatedAt,
    string Team,
    string Market,
    string Description,
    double Score,
    string Classification,
    double Edge,
    double Odds,
    double HitRate,
    int SampleSize,
    double StakeUnits,
    string Source,
    string Status
);
