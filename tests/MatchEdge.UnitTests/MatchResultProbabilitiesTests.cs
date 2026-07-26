using MatchEdge.Application.UseCases.Probability;

namespace MatchEdge.UnitTests;

public class MatchResultProbabilitiesTests
{
    private readonly PoissonProbabilityEngine _engine = new();

    [Fact]
    public void GetMatchResultProbabilities_ShouldSumTo1()
    {
        var result = _engine.GetMatchResultProbabilities(1.5, 1.0);
        var sum = result.HomeWin + result.Draw + result.AwayWin;
        Assert.InRange(sum, 0.999, 1.001);
    }

    [Fact]
    public void GetMatchResultProbabilities_WithEqualLambdas_ShouldBeSymmetric()
    {
        var result = _engine.GetMatchResultProbabilities(1.2, 1.2);
        Assert.InRange(result.HomeWin, result.AwayWin - 0.01, result.AwayWin + 0.01);
    }

    [Fact]
    public void GetMatchResultProbabilities_WithHigherHomeLambda_ShouldFavorHome()
    {
        var result = _engine.GetMatchResultProbabilities(3.0, 0.3);
        Assert.True(result.HomeWin > result.Draw);
        Assert.True(result.HomeWin > result.AwayWin);
    }

    [Fact]
    public void GetMatchResultProbabilities_WithBothZero_ShouldReturnDraw1()
    {
        var result = _engine.GetMatchResultProbabilities(0, 0);
        Assert.Equal(1.0, result.Draw);
        Assert.Equal(0.0, result.HomeWin);
        Assert.Equal(0.0, result.AwayWin);
    }
}
