using MatchEdge.Application.UseCases.Backtesting;

namespace MatchEdge.UnitTests;

public class CalibrationCurveCalculatorTests
{
    private readonly CalibrationCurveCalculator _calculator = new();

    [Fact]
    public void Calculate_EmptyPredictions_ReturnsZeroMatches()
    {
        var result = _calculator.Calculate([]);

        Assert.Equal(0, result.TotalMatches);
        Assert.Empty(result.HomeWin.Bins);
        Assert.Empty(result.Draw.Bins);
        Assert.Empty(result.AwayWin.Bins);
    }

    [Fact]
    public void Calculate_PerfectCalibration_HasLowECE()
    {
        var predictions = new List<(double HomeWin, double Draw, double AwayWin, string Actual)>();

        for (var i = 0; i < 100; i++)
        {
            var prob = 0.5;
            predictions.Add((prob, 0.25, 0.25, "H"));
        }

        var result = _calculator.Calculate(predictions, 2);

        Assert.Equal(100, result.TotalMatches);
        var homeBin = result.HomeWin.Bins.First(b => b.Count > 0);
        Assert.Equal(0.5, homeBin.PredictedProbability, 2);
        Assert.Equal(1.0, homeBin.ObservedFrequency, 2);
    }

    [Fact]
    public void Calculate_AllHomeWins_BinCountsCorrect()
    {
        var predictions = new List<(double HomeWin, double Draw, double AwayWin, string Actual)>();

        for (var i = 0; i < 100; i++)
        {
            predictions.Add((0.7, 0.2, 0.1, "H"));
        }

        var result = _calculator.Calculate(predictions, 10);

        var totalBinCount = result.HomeWin.Bins.Sum(b => b.Count);
        Assert.Equal(100, totalBinCount);
    }

    [Fact]
    public void Calculate_MixedOutcomes_CalibrationBinsPopulated()
    {
        var predictions = new List<(double HomeWin, double Draw, double AwayWin, string Actual)>
        {
            (0.6, 0.2, 0.2, "H"),
            (0.4, 0.3, 0.3, "D"),
            (0.2, 0.3, 0.5, "A"),
            (0.7, 0.15, 0.15, "H"),
            (0.3, 0.4, 0.3, "D"),
            (0.1, 0.2, 0.7, "A"),
        };

        var result = _calculator.Calculate(predictions, 10);

        Assert.Equal(6, result.TotalMatches);
        Assert.True(result.HomeWin.Bins.Count > 0);
        Assert.True(result.Draw.Bins.Count > 0);
        Assert.True(result.AwayWin.Bins.Count > 0);
    }

    [Fact]
    public void Calculate_BinMidpointRange_IsBetweenZeroAndOne()
    {
        var predictions = new List<(double HomeWin, double Draw, double AwayWin, string Actual)>();
        var random = new Random(42);

        for (var i = 0; i < 200; i++)
        {
            var homeProb = random.NextDouble();
            var drawProb = random.NextDouble() * (1 - homeProb);
            var awayProb = 1 - homeProb - drawProb;
            var actual = homeProb > drawProb && homeProb > awayProb ? "H"
                       : drawProb > awayProb ? "D" : "A";
            predictions.Add((homeProb, drawProb, awayProb, actual));
        }

        var result = _calculator.Calculate(predictions, 10);

        foreach (var bin in result.HomeWin.Bins.Where(b => b.Count > 0))
        {
            Assert.InRange(bin.PredictedProbability, 0.0, 1.0);
            Assert.InRange(bin.ObservedFrequency, 0.0, 1.0);
        }
    }

    [Fact]
    public void Calculate_ECE_IsWeightedAverage()
    {
        var predictions = new List<(double HomeWin, double Draw, double AwayWin, string Actual)>
        {
            (0.8, 0.1, 0.1, "H"),
            (0.7, 0.2, 0.1, "H"),
            (0.3, 0.4, 0.3, "D"),
            (0.2, 0.3, 0.5, "A"),
        };

        var result = _calculator.Calculate(predictions, 10);

        Assert.True(result.OverallECE >= 0, "ECE should be non-negative");
        Assert.True(result.OverallECE <= 1.0, "ECE should be at most 1.0");
    }

    [Fact]
    public void Calculate_SingleBin_FullRange()
    {
        var predictions = new List<(double HomeWin, double Draw, double AwayWin, string Actual)>
        {
            (0.9, 0.05, 0.05, "H"),
            (0.1, 0.05, 0.85, "A"),
        };

        var result = _calculator.Calculate(predictions, 1);

        Assert.Single(result.HomeWin.Bins);
        Assert.Equal(2, result.HomeWin.Bins[0].Count);
    }
}
