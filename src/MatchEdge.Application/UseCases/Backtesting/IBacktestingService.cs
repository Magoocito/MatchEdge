namespace MatchEdge.Application.UseCases.Backtesting;

public interface IBacktestingService
{
    Task<(BacktestSummary Summary, IReadOnlyList<BacktestMatchResult> Details)> RunAsync(
        int tournamentId,
        DateTime fromDate,
        DateTime toDate,
        double experimentalGamma,
        bool includeB2 = true,
        int seasonLookback = 2,
        IProgress<BacktestProgress>? progress = null,
        CalibrationWindow? calibrationWindow = null);
}
