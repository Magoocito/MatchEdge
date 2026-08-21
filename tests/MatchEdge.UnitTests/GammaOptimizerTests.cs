using MatchEdge.Application.UseCases.Backtesting;

namespace MatchEdge.UnitTests;

public class GammaOptimizerTests
{
    private static readonly DateTime TrainFrom = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime TrainTo = new(2025, 6, 30, 23, 59, 59, DateTimeKind.Utc);
    private static readonly DateTime ValFrom = new(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime ValTo = new(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc);

    [Fact]
    public async Task FindOptimalGammaAsync_PilotConsistent_OptimalGammaFromTraining()
    {
        var mockService = new FakeBacktestingServiceForGamma(
            b1SplitOnlyBrier: 0.50,
            b1SplitOnlyLogLoss: 0.90,
            brierByGamma: new Dictionary<double, (double Brier, double LogLoss)>
            {
                [1.0] = (0.60, 1.10),
                [1.05] = (0.58, 1.08),
                [1.10] = (0.55, 1.05),
                [1.15] = (0.52, 1.02),
                [1.20] = (0.50, 1.00), // best train
                [1.25] = (0.53, 1.03),
                [1.30] = (0.56, 1.06),
            },
            brierValByGamma: new Dictionary<double, (double Brier, double LogLoss)>
            {
                [1.20] = (0.51, 1.01),
            });

        var sut = new GammaOptimizer(mockService);

        var result = await sut.FindOptimalGammaAsync(
            406, TrainFrom, ValTo, 1.0, 1.30, 0.05);

        Assert.True(result.PilotValidation.IsConsistent);
        Assert.Null(result.PilotValidation.InconsistencyReason);
        Assert.Equal(1.20, result.Training.OptimalGamma);
        Assert.Equal(0.50, result.Training.BestBrierScore);
        Assert.Equal(0.50, result.Training.B1SplitOnlyBrier);
        Assert.Equal(7, result.Training.GridResults.Count);
    }

    [Fact]
    public async Task FindOptimalGammaAsync_ValidationOverfitting_Detected()
    {
        var mockService = new FakeBacktestingServiceForGamma(
            b1SplitOnlyBrier: 0.50,
            b1SplitOnlyLogLoss: 0.90,
            brierByGamma: new Dictionary<double, (double Brier, double LogLoss)>
            {
                [1.0] = (0.55, 1.05),
                [1.05] = (0.52, 1.02),
                [1.10] = (0.50, 1.00), // best train
                [1.15] = (0.53, 1.03),
                [1.20] = (0.56, 1.06),
            },
            brierValByGamma: new Dictionary<double, (double Brier, double LogLoss)>
            {
                [1.10] = (0.58, 1.08), // WORSE in validation → overfitting
            });

        var sut = new GammaOptimizer(mockService);

        var result = await sut.FindOptimalGammaAsync(
            406, TrainFrom, ValTo, 1.0, 1.20, 0.05);

        Assert.True(result.PilotValidation.IsConsistent);
        Assert.Equal(1.10, result.Training.OptimalGamma);
        Assert.True(result.Validation.OverfittingDetected);
        Assert.False(result.Validation.ImprovedVsTrain);
        Assert.Equal(0.58, result.Validation.BrierScore);
    }

    [Fact]
    public async Task FindOptimalGammaAsync_ValidationImproved_NotOverfitting()
    {
        var mockService = new FakeBacktestingServiceForGamma(
            b1SplitOnlyBrier: 0.50,
            b1SplitOnlyLogLoss: 0.90,
            brierByGamma: new Dictionary<double, (double Brier, double LogLoss)>
            {
                [1.0] = (0.55, 1.05),
                [1.05] = (0.52, 1.02),
                [1.10] = (0.50, 1.00), // best train
                [1.15] = (0.53, 1.03),
                [1.20] = (0.56, 1.06),
            },
            brierValByGamma: new Dictionary<double, (double Brier, double LogLoss)>
            {
                [1.10] = (0.48, 0.98), // BETTER in validation
            });

        var sut = new GammaOptimizer(mockService);

        var result = await sut.FindOptimalGammaAsync(
            406, TrainFrom, ValTo, 1.0, 1.20, 0.05);

        Assert.True(result.Validation.ImprovedVsTrain);
        Assert.False(result.Validation.OverfittingDetected);
    }

    [Fact]
    public async Task FindOptimalGammaAsync_B1SplitOnlyReported_BothHalves()
    {
        var mockService = new FakeBacktestingServiceForGamma(
            b1SplitOnlyBrier: 0.50,
            b1SplitOnlyLogLoss: 0.90,
            brierByGamma: new Dictionary<double, (double Brier, double LogLoss)>
            {
                [1.0] = (0.55, 1.05),
                [1.05] = (0.52, 1.02),
            },
            brierValByGamma: new Dictionary<double, (double Brier, double LogLoss)>
            {
                [1.05] = (0.53, 1.03),
            },
            b1SplitOnlyBrierVal: 0.48,
            b1SplitOnlyLogLossVal: 0.88);

        var sut = new GammaOptimizer(mockService);

        var result = await sut.FindOptimalGammaAsync(
            406, TrainFrom, ValTo, 1.0, 1.05, 0.05);

        Assert.Equal(0.50, result.Training.B1SplitOnlyBrier);
        Assert.Equal(0.90, result.Training.B1SplitOnlyLogLoss);
        Assert.Equal(0.48, result.Validation.B1SplitOnlyBrier);
        Assert.Equal(0.88, result.Validation.B1SplitOnlyLogLoss);
    }

    [Fact]
    public async Task FindOptimalGammaAsync_PilotInconsistent_ReturnsEarlyWithReason()
    {
        var mockService = new FakeBacktestingServiceForGamma(
            b1SplitOnlyBrier: 0.50,
            b1SplitOnlyLogLoss: 0.90,
            brierByGamma: new Dictionary<double, (double Brier, double LogLoss)>(),
            brierValByGamma: new Dictionary<double, (double Brier, double LogLoss)>(),
            pilotInconsistent: true);

        var sut = new GammaOptimizer(mockService);

        var result = await sut.FindOptimalGammaAsync(
            406, TrainFrom, ValTo, 1.0, 1.20, 0.05);

        Assert.False(result.PilotValidation.IsConsistent);
        Assert.NotNull(result.PilotValidation.InconsistencyReason);
        Assert.Empty(result.Training.GridResults);
    }

    [Fact]
    public async Task FindOptimalGammaAsync_PilotPointsContainMatchIdsAndAsOfDateTimes()
    {
        var mockService = new FakeBacktestingServiceForGamma(
            b1SplitOnlyBrier: 0.50,
            b1SplitOnlyLogLoss: 0.90,
            brierByGamma: new Dictionary<double, (double Brier, double LogLoss)>
            {
                [1.0] = (0.55, 1.05),
                [1.05] = (0.52, 1.02),
            },
            brierValByGamma: new Dictionary<double, (double Brier, double LogLoss)>
            {
                [1.05] = (0.53, 1.03),
            });

        var sut = new GammaOptimizer(mockService);

        var result = await sut.FindOptimalGammaAsync(
            406, TrainFrom, ValTo, 1.0, 1.05, 0.05);

        Assert.Equal(3, result.PilotValidation.PilotPoints.Count);
        foreach (var pilot in result.PilotValidation.PilotPoints)
        {
            Assert.NotEmpty(pilot.MatchIds);
            Assert.NotEmpty(pilot.AsOfDateTimes);
            Assert.Equal(pilot.MatchIds.Count, pilot.AsOfDateTimes.Count);
        }
    }

    [Fact]
    public async Task FindOptimalGammaAsync_TrainingGridPointsContainMatchIds()
    {
        var mockService = new FakeBacktestingServiceForGamma(
            b1SplitOnlyBrier: 0.50,
            b1SplitOnlyLogLoss: 0.90,
            brierByGamma: new Dictionary<double, (double Brier, double LogLoss)>
            {
                [1.0] = (0.55, 1.05),
                [1.05] = (0.52, 1.02),
            },
            brierValByGamma: new Dictionary<double, (double Brier, double LogLoss)>
            {
                [1.05] = (0.53, 1.03),
            });

        var sut = new GammaOptimizer(mockService);

        var result = await sut.FindOptimalGammaAsync(
            406, TrainFrom, ValTo, 1.0, 1.05, 0.05);

        foreach (var point in result.Training.GridResults)
        {
            Assert.NotEmpty(point.MatchIds);
            Assert.Equal(point.MatchIds.Count, point.AsOfDateTimes.Count);
        }
    }
}

internal class FakeBacktestingServiceForGamma : IBacktestingService
{
    private readonly Dictionary<double, (double Brier, double LogLoss)> _brierByGamma;
    private readonly Dictionary<double, (double Brier, double LogLoss)> _brierValByGamma;
    private readonly double _b1SplitOnlyBrier;
    private readonly double _b1SplitOnlyLogLoss;
    private readonly double? _b1SplitOnlyBrierVal;
    private readonly double? _b1SplitOnlyLogLossVal;
    private readonly bool _pilotInconsistent;
    private int _pilotCallCount;

    private static readonly IReadOnlyList<int> PilotMatchIds = [101, 102, 103, 104, 105];
    private static readonly IReadOnlyList<DateTime> PilotAsOfs =
    [
        new DateTime(2025, 1, 15, 18, 0, 0, DateTimeKind.Utc),
        new DateTime(2025, 2, 1, 18, 0, 0, DateTimeKind.Utc),
        new DateTime(2025, 3, 10, 18, 0, 0, DateTimeKind.Utc),
        new DateTime(2025, 4, 5, 18, 0, 0, DateTimeKind.Utc),
        new DateTime(2025, 5, 20, 18, 0, 0, DateTimeKind.Utc),
    ];

    public int RunCallCount { get; private set; }

    public FakeBacktestingServiceForGamma(
        double b1SplitOnlyBrier,
        double b1SplitOnlyLogLoss,
        Dictionary<double, (double Brier, double LogLoss)> brierByGamma,
        Dictionary<double, (double Brier, double LogLoss)> brierValByGamma,
        double? b1SplitOnlyBrierVal = null,
        double? b1SplitOnlyLogLossVal = null,
        bool pilotInconsistent = false)
    {
        _b1SplitOnlyBrier = b1SplitOnlyBrier;
        _b1SplitOnlyLogLoss = b1SplitOnlyLogLoss;
        _brierByGamma = brierByGamma;
        _brierValByGamma = brierValByGamma;
        _b1SplitOnlyBrierVal = b1SplitOnlyBrierVal;
        _b1SplitOnlyLogLossVal = b1SplitOnlyLogLossVal;
        _pilotInconsistent = pilotInconsistent;
    }

    public Task<(BacktestSummary Summary, IReadOnlyList<BacktestMatchResult> Details)> RunAsync(
        int tournamentId,
        DateTime fromDate,
        DateTime toDate,
        double experimentalGamma,
        bool includeB2 = true,
        int seasonLookback = 2,
        IProgress<BacktestProgress>? progress = null)
    {
        RunCallCount++;

        var isVal = fromDate.Month >= 7;
        var matchIds = _pilotInconsistent && !isVal
            ? (_pilotCallCount++ == 0
                ? new List<int>(PilotMatchIds)
                : new List<int> { 101, 102, 103, 999 }) // inconsistent on 2nd+ call
            : new List<int>(PilotMatchIds);
        var asOfs = new List<DateTime>(PilotAsOfs);

        if (includeB2)
        {
            var b1Brier = isVal && _b1SplitOnlyBrierVal.HasValue
                ? _b1SplitOnlyBrierVal.Value : _b1SplitOnlyBrier;
            var b1LogLoss = isVal && _b1SplitOnlyLogLossVal.HasValue
                ? _b1SplitOnlyLogLossVal.Value : _b1SplitOnlyLogLoss;

            var summary = new BacktestSummary
            {
                ModelA = CreateModelVariant(0.7, 1.2, matchIds.Count),
                ModelB1 = new ModelVariantMetrics
                {
                    Overall = new MetricSet { BrierScore = 0.65, LogLoss = 1.15, MatchCount = matchIds.Count },
                    SplitOnly = new MetricSet { BrierScore = b1Brier, LogLoss = b1LogLoss, MatchCount = matchIds.Count },
                    FallbackOnly = new MetricSet { BrierScore = 0.7, LogLoss = 1.2, MatchCount = 0 }
                },
                ModelB2 = CreateModelVariant(0.7, 1.2, matchIds.Count),
                TotalMatches = matchIds.Count,
                SkippedMatches = 0
            };
            var details = matchIds.Select((id, i) => new BacktestMatchResult
            {
                MatchId = id,
                MatchDate = asOfs[i]
            }).ToList();
            return Task.FromResult((summary, (IReadOnlyList<BacktestMatchResult>)details));
        }

        // Model A only
        var dict = isVal ? _brierValByGamma : _brierByGamma;
        var (brier, logLoss) = dict.ContainsKey(experimentalGamma)
            ? dict[experimentalGamma]
            : (0.65, 1.15);

        var aSummary = new BacktestSummary
        {
            ModelA = CreateModelVariant(brier, logLoss, matchIds.Count),
            ModelB1 = CreateModelVariant(brier, logLoss, matchIds.Count),
            ModelB2 = CreateModelVariant(brier, logLoss, matchIds.Count),
            TotalMatches = matchIds.Count,
            SkippedMatches = 0
        };
        var aDetails = matchIds.Select((id, i) => new BacktestMatchResult
        {
            MatchId = id,
            MatchDate = asOfs[i]
        }).ToList();
        return Task.FromResult((aSummary, (IReadOnlyList<BacktestMatchResult>)aDetails));
    }

    private static ModelVariantMetrics CreateModelVariant(double brier, double logLoss, int matchCount)
    {
        return new ModelVariantMetrics
        {
            Overall = new MetricSet { BrierScore = brier, LogLoss = logLoss, MatchCount = matchCount },
            SplitOnly = new MetricSet { BrierScore = brier, LogLoss = logLoss, MatchCount = matchCount },
            FallbackOnly = new MetricSet { BrierScore = brier, LogLoss = logLoss, MatchCount = matchCount }
        };
    }
}
