using MatchEdge.Application.UseCases.Backtesting;

namespace MatchEdge.UnitTests;

public class BrierScoreCalculatorTests
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

        var result = BrierScoreCalculator.Calculate(predictions);

        Assert.Equal(0.0, result, 10);
    }

    [Fact]
    public void Calculate_WorstPrediction_ReturnsTwo()
    {
        var predictions = new List<(double HomeWin, double Draw, double AwayWin, string Actual)>
        {
            (0.0, 0.0, 1.0, "H"),
            (0.0, 0.0, 1.0, "D"),
            (1.0, 0.0, 0.0, "A")
        };

        var result = BrierScoreCalculator.Calculate(predictions);

        Assert.Equal(2.0, result, 10);
    }

    [Fact]
    public void Calculate_SingleMatch_HomeWin()
    {
        // Predicting 0.6 home win, actual is home win
        // BS = (0.6-1)^2 + (0.2-0)^2 + (0.2-0)^2 = 0.16 + 0.04 + 0.04 = 0.24
        var predictions = new List<(double HomeWin, double Draw, double AwayWin, string Actual)>
        {
            (0.6, 0.2, 0.2, "H")
        };

        var result = BrierScoreCalculator.Calculate(predictions);

        Assert.Equal(0.24, result, 10);
    }

    [Fact]
    public void Calculate_SingleMatch_Draw()
    {
        // Predicting 0.3 draw, actual is draw
        // BS = (0.3-0)^2 + (0.4-1)^2 + (0.3-0)^2 = 0.09 + 0.36 + 0.09 = 0.54
        var predictions = new List<(double HomeWin, double Draw, double AwayWin, string Actual)>
        {
            (0.3, 0.4, 0.3, "D")
        };

        var result = BrierScoreCalculator.Calculate(predictions);

        Assert.Equal(0.54, result, 10);
    }

    [Fact]
    public void Calculate_EmptyList_ReturnsZero()
    {
        var result = BrierScoreCalculator.Calculate([]);

        Assert.Equal(0.0, result, 10);
    }
}
