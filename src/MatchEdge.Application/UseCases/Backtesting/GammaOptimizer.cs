namespace MatchEdge.Application.UseCases.Backtesting;

public class GammaOptimizer : IGammaOptimizer
{
    private readonly IBacktestingService _backtestingService;

    public GammaOptimizer(IBacktestingService backtestingService)
    {
        _backtestingService = backtestingService;
    }

    public async Task<GammaOptimizationResult> FindOptimalGammaAsync(
        int tournamentId,
        DateTime fromDate,
        DateTime toDate,
        double gammaMin = 1.0,
        double gammaMax = 2.5,
        double step = 0.05,
        int seasonLookback = 2,
        IProgress<BacktestProgress>? progress = null)
    {
        // Step 1: Run B1 once to get the splitOnly reference (invariant to gamma)
        var (b1Summary, _) = await _backtestingService.RunAsync(
            tournamentId, fromDate, toDate,
            experimentalGamma: 1.0,
            includeB2: true,
            seasonLookback: seasonLookback);

        var b1SplitOnlyBrier = b1Summary.ModelB1.SplitOnly.BrierScore;
        var b1SplitOnlyLogLoss = b1Summary.ModelB1.SplitOnly.LogLoss;

        // Step 2: Grid search over gamma for Model A
        var gridResults = new List<GammaGridPoint>();
        var bestBrier = double.MaxValue;
        var bestGamma = gammaMin;

        for (var gamma = gammaMin; gamma <= gammaMax + step * 0.001; gamma += step)
        {
            var roundedGamma = Math.Round(gamma, 4);
            var (summary, _) = await _backtestingService.RunAsync(
                tournamentId, fromDate, toDate,
                experimentalGamma: roundedGamma,
                includeB2: false,
                seasonLookback: seasonLookback);

            var point = new GammaGridPoint
            {
                Gamma = roundedGamma,
                BrierScore = summary.ModelA.Overall.BrierScore,
                LogLoss = summary.ModelA.Overall.LogLoss,
                MatchCount = summary.ModelA.Overall.MatchCount
            };
            gridResults.Add(point);

            if (point.BrierScore < bestBrier)
            {
                bestBrier = point.BrierScore;
                bestGamma = point.Gamma;
            }

            progress?.Report(new BacktestProgress(
                gridResults.Count,
                (int)((gammaMax - gammaMin) / step) + 1,
                $"gamma={roundedGamma:F4} Brier={point.BrierScore:F6}"));
        }

        var bestPoint = gridResults.First(p => p.Gamma == bestGamma);

        return new GammaOptimizationResult
        {
            OptimalGamma = bestGamma,
            BestBrierScore = bestPoint.BrierScore,
            BestLogLoss = bestPoint.LogLoss,
            B1SplitOnlyReferenceBrier = b1SplitOnlyBrier,
            B1SplitOnlyReferenceLogLoss = b1SplitOnlyLogLoss,
            GridResults = gridResults
        };
    }
}
