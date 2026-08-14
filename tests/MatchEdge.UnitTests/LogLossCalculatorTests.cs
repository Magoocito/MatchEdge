using MatchEdge.Application.UseCases.Backtesting;

namespace MatchEdge.UnitTests;

public class LogLossCalculatorTests
{
    [Fact]
    public void Calculate_PerfectPrediction_ReturnsZero()
    {
        var predictions = new List<(double HomeWin, double Draw, double AwayWin, string Actual)>
        {
            (1.0, 0.0, 0.0, "H"),
            (0.0, 1.0, 0.0, "D"),
            (0.0, 0.0, 1.0, "A")
        };

        var result = LogLossCalculator.Calculate(predictions);

        Assert.Equal(0.0, result, 10);
    }

    [Fact]
    public void Calculate_SingleMatch_ClampsNearZeroProbability()
    {
        // Predicting near-zero probability for actual outcome
        // LogLoss = -ln(1e-15) ≈ 34.54
        var predictions = new List<(double HomeWin, double Draw, double AwayWin, string Actual)>
        {
            (1e-16, 0.5, 0.5, "H")
        };

        var result = LogLossCalculator.Calculate(predictions);

        Assert.True(result > 30.0);
    }

    [Fact]
    public void Calculate_SingleMatch_GoodPrediction()
    {
        // Predicting 0.8 for actual home win
        // LogLoss = -ln(0.8) ≈ 0.2231
        var predictions = new List<(double HomeWin, double Draw, double AwayWin, string Actual)>
        {
            (0.8, 0.1, 0.1, "H")
        };

        var result = LogLossCalculator.Calculate(predictions);

        Assert.Equal(-Math.Log(0.8), result, 4);
    }

    [Fact]
    public void Calculate_MultipleMatches_Averaged()
    {
        // Match 1: -ln(0.7) ≈ 0.3567
        // Match 2: -ln(0.5) ≈ 0.6931
        // Average: (0.3567 + 0.6931) / 2 ≈ 0.5249
        var predictions = new List<(double HomeWin, double Draw, double AwayWin, string Actual)>
        {
            (0.7, 0.2, 0.1, "H"),
            (0.2, 0.5, 0.3, "D")
        };

        var result = LogLossCalculator.Calculate(predictions);

        var expected = (-Math.Log(0.7) + -Math.Log(0.5)) / 2;
        Assert.Equal(expected, result, 4);
    }

    [Fact]
    public void Calculate_EmptyList_ReturnsZero()
    {
        var result = LogLossCalculator.Calculate([]);

        Assert.Equal(0.0, result, 10);
    }
}
