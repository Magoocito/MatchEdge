using MatchEdge.Application.UseCases.Probability;

namespace MatchEdge.UnitTests;

public class PoissonProbabilityEngineTests
{
    private readonly PoissonProbabilityEngine _engine = new();

    [Fact]
    public void PoissonProbability_WithLambda174_AndX0_ShouldReturnCorrectValue()
    {
        // P(0) = e^(-1.74) * 1.74^0 / 0! = e^(-1.74) * 1 / 1 = 0.1755
        var result = _engine.PoissonProbability(1.74, 0);
        Assert.InRange(result, 0.174, 0.177);
    }

    [Fact]
    public void PoissonProbability_WithLambda174_AndX1_ShouldReturnCorrectValue()
    {
        // P(1) = e^(-1.74) * 1.74^1 / 1! = 0.1755 * 1.74 = 0.3054
        var result = _engine.PoissonProbability(1.74, 1);
        Assert.InRange(result, 0.304, 0.307);
    }

    [Fact]
    public void PoissonProbability_WithLambda174_AndX2_ShouldReturnCorrectValue()
    {
        // P(2) = e^(-1.74) * 1.74^2 / 2! = 0.1755 * 3.0276 / 2 = 0.2657
        var result = _engine.PoissonProbability(1.74, 2);
        Assert.InRange(result, 0.264, 0.267);
    }

    [Fact]
    public void GetOverUnderProbability_WithLambda174_Line25_Over_ShouldBeApprox052()
    {
        // P(Under 2.5) = P(0) + P(1) + P(2) = 0.1755 + 0.3054 + 0.2657 = 0.7466
        // P(Over 2.5) = 1 - 0.7466 = 0.2534
        var result = _engine.GetOverUnderProbability(1.74, 2.5, over: true);
        Assert.InRange(result, 0.242, 0.264);
    }

    [Fact]
    public void GetOverUnderProbability_WithLambda174_Line25_Under_ShouldBeApprox075()
    {
        // P(Under 2.5) = P(0) + P(1) + P(2) = 0.7466
        var result = _engine.GetOverUnderProbability(1.74, 2.5, over: false);
        Assert.InRange(result, 0.736, 0.758);
    }

    [Fact]
    public void GetOverUnderProbability_Over_Plus_Under_ShouldEqual1()
    {
        var over = _engine.GetOverUnderProbability(1.74, 2.5, over: true);
        var under = _engine.GetOverUnderProbability(1.74, 2.5, over: false);
        Assert.Equal(1.0, over + under, 2);
    }

    [Fact]
    public void PoissonProbability_WithLambda0_ShouldReturn1ForX0()
    {
        // P(0) = e^0 * 0^0 / 0! = 1 * 1 / 1 = 1
        var result = _engine.PoissonProbability(0, 0);
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void PoissonProbability_WithLambda0_ShouldReturn0ForXGreaterThan0()
    {
        // P(1) = e^0 * 0^1 / 1! = 1 * 0 / 1 = 0
        var result = _engine.PoissonProbability(0, 1);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void GetOverUnderProbability_WithLine15_ShouldWorkCorrectly()
    {
        // P(Under 1.5) = P(0) + P(1)
        var under = _engine.GetOverUnderProbability(1.74, 1.5, over: false);
        var expected = _engine.PoissonProbability(1.74, 0) + _engine.PoissonProbability(1.74, 1);
        Assert.Equal(expected, under, 4);
    }

    [Fact]
    public void GetOverUnderProbability_WithHighLambda_ShouldReturnValidProbability()
    {
        var result = _engine.GetOverUnderProbability(5.0, 2.5, over: true);
        Assert.InRange(result, 0.0, 1.0);
    }

    [Fact]
    public void PoissonProbability_WithNegativeLambda_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _engine.PoissonProbability(-1, 0));
    }

    [Fact]
    public void PoissonProbability_WithNegativeX_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _engine.PoissonProbability(1.74, -1));
    }
}
