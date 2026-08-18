using MatchEdge.Application.UseCases.Backtesting;

namespace MatchEdge.UnitTests;

public class GammaOptimizerTests
{
    [Fact]
    public async Task FindOptimalGammaAsync_ReturnsOptimalGamma()
    {
        // Arrange: mock BacktestingService to return different Brier scores per gamma
        var mockService = new FakeBacktestingServiceForGamma(
            b1SplitOnlyBrier: 0.5,
            b1SplitOnlyLogLoss: 0.9,
            brierByGamma: new Dictionary<double, (double Brier, double LogLoss)>
            {
                [1.0] = (0.60, 1.10),
                [1.05] = (0.58, 1.08),
                [1.10] = (0.55, 1.05),
                [1.15] = (0.52, 1.02),
                [1.20] = (0.50, 1.00),  // best
                [1.25] = (0.53, 1.03),
                [1.30] = (0.56, 1.06),
            });

        var sut = new GammaOptimizer(mockService);

        // Act
        var result = await sut.FindOptimalGammaAsync(
            tournamentId: 406,
            fromDate: new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            toDate: new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc),
            gammaMin: 1.0,
            gammaMax: 1.30,
            step: 0.05);

        // Assert
        Assert.Equal(1.20, result.OptimalGamma);
        Assert.Equal(0.50, result.BestBrierScore);
        Assert.Equal(1.00, result.BestLogLoss);
        Assert.Equal(0.5, result.B1SplitOnlyReferenceBrier);
        Assert.Equal(0.9, result.B1SplitOnlyReferenceLogLoss);
        Assert.Equal(7, result.GridResults.Count);
    }

    [Fact]
    public async Task FindOptimalGammaAsync_B1RunOnce_WithGamma1()
    {
        // Arrange
        var mockService = new FakeBacktestingServiceForGamma(
            b1SplitOnlyBrier: 0.5,
            b1SplitOnlyLogLoss: 0.9,
            brierByGamma: new Dictionary<double, (double Brier, double LogLoss)>
            {
                [1.0] = (0.60, 1.10),
                [1.05] = (0.58, 1.08),
            });

        var sut = new GammaOptimizer(mockService);

        // Act
        await sut.FindOptimalGammaAsync(
            tournamentId: 406,
            fromDate: DateTime.UtcNow.AddDays(-90),
            toDate: DateTime.UtcNow,
            gammaMin: 1.0,
            gammaMax: 1.05,
            step: 0.05);

        // Assert: B1 called once with gamma=1.0, then Model A called for each grid point
        Assert.Equal(3, mockService.RunCallCount);
        Assert.Equal(1.0, mockService.B1RunGamma);
    }

    [Fact]
    public async Task FindOptimalGammaAsync_GridResultsHaveCorrectGammaValues()
    {
        // Arrange
        var mockService = new FakeBacktestingServiceForGamma(
            b1SplitOnlyBrier: 0.5,
            b1SplitOnlyLogLoss: 0.9,
            brierByGamma: new Dictionary<double, (double Brier, double LogLoss)>
            {
                [1.0] = (0.60, 1.10),
                [1.05] = (0.58, 1.08),
                [1.10] = (0.55, 1.05),
            });

        var sut = new GammaOptimizer(mockService);

        // Act
        var result = await sut.FindOptimalGammaAsync(
            tournamentId: 406,
            fromDate: DateTime.UtcNow.AddDays(-90),
            toDate: DateTime.UtcNow,
            gammaMin: 1.0,
            gammaMax: 1.10,
            step: 0.05);

        // Assert
        Assert.Equal(3, result.GridResults.Count);
        Assert.Equal(1.0, result.GridResults[0].Gamma);
        Assert.Equal(1.05, result.GridResults[1].Gamma);
        Assert.Equal(1.1, result.GridResults[2].Gamma);
    }

    [Fact]
    public async Task FindOptimalGammaAsync_ReportsProgress()
    {
        // Arrange
        var mockService = new FakeBacktestingServiceForGamma(
            b1SplitOnlyBrier: 0.5,
            b1SplitOnlyLogLoss: 0.9,
            brierByGamma: new Dictionary<double, (double Brier, double LogLoss)>
            {
                [1.0] = (0.60, 1.10),
                [1.05] = (0.58, 1.08),
            });

        var progressReports = new List<BacktestProgress>();
        var progress = new Progress<BacktestProgress>(p => progressReports.Add(p));

        var sut = new GammaOptimizer(mockService);

        // Act
        await sut.FindOptimalGammaAsync(
            tournamentId: 406,
            fromDate: DateTime.UtcNow.AddDays(-90),
            toDate: DateTime.UtcNow,
            gammaMin: 1.0,
            gammaMax: 1.05,
            step: 0.05,
            progress: progress);

        // Assert
        Assert.Equal(2, progressReports.Count);
        Assert.Contains("gamma=1.0", progressReports[0].CurrentMatch);
        Assert.Contains("gamma=1.05", progressReports[1].CurrentMatch);
    }

    [Fact]
    public async Task FindOptimalGammaAsync_ChoosesBestBrierScore()
    {
        // Arrange: gamma=1.25 has best Brier
        var mockService = new FakeBacktestingServiceForGamma(
            b1SplitOnlyBrier: 0.5,
            b1SplitOnlyLogLoss: 0.9,
            brierByGamma: new Dictionary<double, (double Brier, double LogLoss)>
            {
                [1.0] = (0.60, 1.10),
                [1.05] = (0.58, 1.08),
                [1.10] = (0.55, 1.05),
                [1.15] = (0.53, 1.03),
                [1.20] = (0.52, 1.00),
                [1.25] = (0.51, 0.99),  // best Brier
                [1.30] = (0.53, 1.01),
            });

        var sut = new GammaOptimizer(mockService);

        // Act
        var result = await sut.FindOptimalGammaAsync(
            tournamentId: 406,
            fromDate: DateTime.UtcNow.AddDays(-90),
            toDate: DateTime.UtcNow,
            gammaMin: 1.0,
            gammaMax: 1.30,
            step: 0.05);

        // Assert
        Assert.Equal(1.25, result.OptimalGamma);
        Assert.Equal(0.51, result.BestBrierScore);
        Assert.Equal(0.99, result.BestLogLoss);
    }
}

internal class FakeBacktestingServiceForGamma : IBacktestingService
{
    private readonly Dictionary<double, (double Brier, double LogLoss)> _brierByGamma;
    private readonly double _b1SplitOnlyBrier;
    private readonly double _b1SplitOnlyLogLoss;

    public int RunCallCount { get; private set; }
    public double? B1RunGamma { get; private set; }

    public FakeBacktestingServiceForGamma(
        double b1SplitOnlyBrier,
        double b1SplitOnlyLogLoss,
        Dictionary<double, (double Brier, double LogLoss)> brierByGamma)
    {
        _b1SplitOnlyBrier = b1SplitOnlyBrier;
        _b1SplitOnlyLogLoss = b1SplitOnlyLogLoss;
        _brierByGamma = brierByGamma;
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

        if (includeB2)
        {
            B1RunGamma = experimentalGamma;
            var summary = new BacktestSummary
            {
                ModelA = CreateModelVariant(0.7, 1.2, 100),
                ModelB1 = CreateModelVariantWithSplit(0.65, 1.15, _b1SplitOnlyBrier, _b1SplitOnlyLogLoss, 80, 0.7, 1.2, 20),
                ModelB2 = CreateModelVariant(0.7, 1.2, 100),
                TotalMatches = 100,
                SkippedMatches = 0
            };
            return Task.FromResult((summary, (IReadOnlyList<BacktestMatchResult>)[]));
        }

        var (brier, logLoss) = _brierByGamma.ContainsKey(experimentalGamma)
            ? _brierByGamma[experimentalGamma]
            : (0.65, 1.15);

        var aSummary = new BacktestSummary
        {
            ModelA = CreateModelVariant(brier, logLoss, 100),
            ModelB1 = CreateModelVariant(brier, logLoss, 100),
            ModelB2 = CreateModelVariant(brier, logLoss, 100),
            TotalMatches = 100,
            SkippedMatches = 0
        };
        return Task.FromResult((aSummary, (IReadOnlyList<BacktestMatchResult>)[]));
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

    private static ModelVariantMetrics CreateModelVariantWithSplit(
        double overallBrier, double overallLogLoss,
        double splitBrier, double splitLogLoss, int splitCount,
        double fallbackBrier, double fallbackLogLoss, int fallbackCount)
    {
        return new ModelVariantMetrics
        {
            Overall = new MetricSet { BrierScore = overallBrier, LogLoss = overallLogLoss, MatchCount = splitCount + fallbackCount },
            SplitOnly = new MetricSet { BrierScore = splitBrier, LogLoss = splitLogLoss, MatchCount = splitCount },
            FallbackOnly = new MetricSet { BrierScore = fallbackBrier, LogLoss = fallbackLogLoss, MatchCount = fallbackCount }
        };
    }
}
