namespace MatchEdge.Application.Services;

public interface IOrchestratorService
{
    Task<PipelineResult> RunDailyPipelineAsync(
        DateTime? date = null,
        int maxFixtures = 50,
        int topPicksPerFixture = 10,
        int topPicksOverall = 30,
        CancellationToken ct = default);

    Task<PipelineStatus> GetStatusAsync(CancellationToken ct = default);

    Task<List<FixtureSummary>> GetTodayFixturesAsync(
        CancellationToken ct = default);
}

public record PipelineResult(
    DateTime Date,
    bool Success,
    string Message,
    int TotalFixtures,
    int FixturesScraped,
    int TotalTrends,
    int TotalPicksScored,
    int PicksSaved,
    int PicksWithStakes,
    double KellyStakesTotal,
    List<FixtureSummary> Fixtures,
    List<StakeRecommendation> TopPicks,
    TimeSpan Duration
);

public record FixtureSummary(
    string Slug,
    string HomeTeam,
    string AwayTeam,
    string League,
    bool HasTrends,
    int TrendsScraped,
    int PicksScored,
    string Status
);

public record PipelineStatus(
    bool IsRunning,
    DateTime? LastRun,
    DateTime? NextRun,
    int TotalRunsToday,
    string LastResult,
    TimeSpan? LastDuration
);
