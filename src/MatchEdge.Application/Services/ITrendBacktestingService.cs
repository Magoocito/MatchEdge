namespace MatchEdge.Application.Services;

public interface ITrendBacktestingService
{
    Task<BacktestResult> RunBacktestAsync(
        DateTime fromDate,
        DateTime toDate,
        double stakePerUnit = 1.0,
        CancellationToken ct = default);

    Task<BacktestSummary> GetSummaryAsync(
        CancellationToken ct = default);

    Task<List<DailyPickEntityDto>> GetEvaluatablePicksAsync(
        DateTime date,
        CancellationToken ct = default);
}

public record BacktestResult(
    DateTime FromDate,
    DateTime ToDate,
    int TotalPicks,
    int Won,
    int Lost,
    int Pending,
    int Voided,
    double WinRate,
    double TotalStaked,
    double TotalProfit,
    double ROI,
    double Yield,
    double MaxDrawdown,
    double PeakBalance,
    List<DailyPickEntityDto> Picks,
    List<DayBacktestResult> DailyResults
);

public record DayBacktestResult(
    DateTime Date,
    int Picks,
    int Won,
    int Lost,
    double DailyProfit,
    double BalanceAfter
);

public record BacktestSummary(
    int TotalPicks,
    int Won,
    int Lost,
    int Pending,
    double WinRate,
    double TotalStaked,
    double TotalProfit,
    double ROI,
    double Yield,
    double CurrentBalance,
    DateTime LastUpdated
);
