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
        // Derive first-half and second-half windows from the provided range
        var midpoint = new DateTime(
            fromDate.Year, 7, 1, 0, 0, 0, DateTimeKind.Utc);

        var trainFrom = fromDate;
        var trainTo = midpoint.AddTicks(-1); // 2025-06-30 23:59:59
        var valFrom = midpoint;               // 2025-07-01 00:00:00
        var valTo = toDate;

        // Phase 1: Pilot validation (3 points)
        var pilotGammas = new[] { 1.50, 1.55, 1.60 };
        var pilotPoints = new List<GammaGridPoint>();
        IReadOnlyList<int>? referenceMatchIds = null;
        IReadOnlyList<DateTime>? referenceAsOfs = null;
        var pilotConsistent = true;
        string? pilotInconsistencyReason = null;

        foreach (var g in pilotGammas)
        {
            var point = await RunSingleGamma(tournamentId, trainFrom, trainTo, g, seasonLookback);
            pilotPoints.Add(point);

            if (referenceMatchIds == null)
            {
                referenceMatchIds = point.MatchIds;
                referenceAsOfs = point.AsOfDateTimes;
            }
            else
            {
                if (!MatchSequenceEqual(referenceMatchIds, point.MatchIds))
                {
                    pilotConsistent = false;
                    pilotInconsistencyReason =
                        $"MatchIds differ at gamma={g}: expected {referenceMatchIds.Count} matches, got {point.MatchIds.Count}";
                }
                else if (!AsOfDateTimeSequenceEqual(referenceAsOfs!, point.AsOfDateTimes))
                {
                    pilotConsistent = false;
                    pilotInconsistencyReason =
                        $"asOfDateTime values differ at gamma={g} for same match sequence";
                }
            }
        }

        progress?.Report(new BacktestProgress(3, 3,
            pilotConsistent ? "Pilot OK: matches identical across gamma values" :
            $"PILOT FAILED: {pilotInconsistencyReason}"));

        if (!pilotConsistent)
        {
            return new GammaOptimizationResult
            {
                PilotValidation = new GammaPilotValidation
                {
                    IsConsistent = false,
                    InconsistencyReason = pilotInconsistencyReason,
                    PilotPoints = pilotPoints
                }
            };
        }

        // Phase 2: Grid search on first half (training)
        var b1Train = await RunB1Once(tournamentId, trainFrom, trainTo, seasonLookback);

        var gridResults = new List<GammaGridPoint>();
        var bestBrier = double.MaxValue;
        var bestGamma = gammaMin;

        var totalSteps = (int)((gammaMax - gammaMin) / step) + 1;
        var stepIndex = 0;

        for (var gamma = gammaMin; gamma <= gammaMax + step * 0.001; gamma += step)
        {
            var roundedGamma = Math.Round(gamma, 4);
            var point = await RunSingleGamma(tournamentId, trainFrom, trainTo, roundedGamma, seasonLookback);
            gridResults.Add(point);

            if (point.BrierScore < bestBrier)
            {
                bestBrier = point.BrierScore;
                bestGamma = roundedGamma;
            }

            stepIndex++;
            progress?.Report(new BacktestProgress(stepIndex, totalSteps,
                $"[train] gamma={roundedGamma:F4} Brier={point.BrierScore:F6}"));
        }

        var bestPoint = gridResults.First(p => p.Gamma == bestGamma);

        // Phase 3: Out-of-sample validation on second half
        var b1Val = await RunB1Once(tournamentId, valFrom, valTo, seasonLookback);
        var valPoint = await RunSingleGamma(tournamentId, valFrom, valTo, bestGamma, seasonLookback);

        var improved = valPoint.BrierScore < bestPoint.BrierScore;
        var overfitting = valPoint.BrierScore > bestPoint.BrierScore;

        progress?.Report(new BacktestProgress(totalSteps, totalSteps,
            overfitting ?
                $"OVERFITTING: train Brier={bestPoint.BrierScore:F6} vs val Brier={valPoint.BrierScore:F6}" :
                $"OK: train Brier={bestPoint.BrierScore:F6} vs val Brier={valPoint.BrierScore:F6}"));

        return new GammaOptimizationResult
        {
            PilotValidation = new GammaPilotValidation
            {
                IsConsistent = true,
                PilotPoints = pilotPoints
            },
            Training = new GammaTrainResult
            {
                OptimalGamma = bestGamma,
                BestBrierScore = bestPoint.BrierScore,
                BestLogLoss = bestPoint.LogLoss,
                B1SplitOnlyBrier = b1Train.Brier,
                B1SplitOnlyLogLoss = b1Train.LogLoss,
                GridResults = gridResults
            },
            Validation = new GammaValidationResult
            {
                OptimalGamma = bestGamma,
                BrierScore = valPoint.BrierScore,
                LogLoss = valPoint.LogLoss,
                B1SplitOnlyBrier = b1Val.Brier,
                B1SplitOnlyLogLoss = b1Val.LogLoss,
                ImprovedVsTrain = improved,
                OverfittingDetected = overfitting
            }
        };
    }

    private async Task<GammaGridPoint> RunSingleGamma(
        int tournamentId, DateTime fromDate, DateTime toDate,
        double gamma, int seasonLookback)
    {
        var (summary, details) = await _backtestingService.RunAsync(
            tournamentId, fromDate, toDate,
            experimentalGamma: gamma,
            includeB2: false,
            seasonLookback: seasonLookback);

        return new GammaGridPoint
        {
            Gamma = gamma,
            BrierScore = summary.ModelA.Overall.BrierScore,
            LogLoss = summary.ModelA.Overall.LogLoss,
            MatchCount = summary.ModelA.Overall.MatchCount,
            MatchIds = details.Select(d => d.MatchId).ToList(),
            AsOfDateTimes = details.Select(d => d.MatchDate).ToList()
        };
    }

    private async Task<(double Brier, double LogLoss)> RunB1Once(
        int tournamentId, DateTime fromDate, DateTime toDate, int seasonLookback)
    {
        var (summary, _) = await _backtestingService.RunAsync(
            tournamentId, fromDate, toDate,
            experimentalGamma: 1.0,
            includeB2: true,
            seasonLookback: seasonLookback);

        return (summary.ModelB1.SplitOnly.BrierScore, summary.ModelB1.SplitOnly.LogLoss);
    }

    private static bool MatchSequenceEqual(IReadOnlyList<int> a, IReadOnlyList<int> b)
    {
        if (a.Count != b.Count) return false;
        for (var i = 0; i < a.Count; i++)
            if (a[i] != b[i]) return false;
        return true;
    }

    private static bool AsOfDateTimeSequenceEqual(IReadOnlyList<DateTime> a, IReadOnlyList<DateTime> b)
    {
        if (a.Count != b.Count) return false;
        for (var i = 0; i < a.Count; i++)
            if (a[i] != b[i]) return false;
        return true;
    }
}
