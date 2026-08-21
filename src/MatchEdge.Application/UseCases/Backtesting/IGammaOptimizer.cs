namespace MatchEdge.Application.UseCases.Backtesting;

public interface IGammaOptimizer
{
    Task<GammaOptimizationResult> FindOptimalGammaAsync(
        int tournamentId,
        DateTime fromDate,
        DateTime toDate,
        double gammaMin = 1.0,
        double gammaMax = 2.5,
        double step = 0.05,
        int seasonLookback = 2,
        IProgress<BacktestProgress>? progress = null);
}
