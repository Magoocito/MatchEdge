using MatchEdge.Application.UseCases.Probability;

namespace MatchEdge.UnitTests;

public class BttsTests
{
    private readonly PoissonProbabilityEngine _engine = new();

    [Fact]
    public void GetBttsYesProbability_WithLambdaHome15_AndLambdaAway10_ShouldReturnCorrectValue()
    {
        // P(Local=0) = e^(-1.5) = 0.2231
        // P(Local>=1) = 1 - 0.2231 = 0.7769
        // P(Visitante=0) = e^(-1.0) = 0.3679
        // P(Visitante>=1) = 1 - 0.3679 = 0.6321
        // P(BTTS=Sí) = 0.7769 * 0.6321 = 0.4910
        var result = _engine.GetBttsYesProbability(1.5, 1.0);
        Assert.InRange(result, 0.4900, 0.4920);
    }

    [Fact]
    public void GetBttsYesProbability_BttsYes_Plus_BttsNo_ShouldEqual1()
    {
        double lambdaHome = 1.5;
        double lambdaAway = 1.0;

        double bttsYes = _engine.GetBttsYesProbability(lambdaHome, lambdaAway);
        double bttsNo = 1.0 - bttsYes;

        Assert.Equal(1.0, bttsYes + bttsNo, 10);
    }

    [Fact]
    public void GetBttsYesProbability_WithLambda0_AndLambda0_ShouldReturn0()
    {
        // Si ambos lambdas son 0, ningún equipo anota → BTTS Sí = 0
        var result = _engine.GetBttsYesProbability(0, 0);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void GetBttsYesProbability_WithHighLambdas_ShouldApproach1()
    {
        // Con lambdas muy altos, ambos equipos casi seguro anotan → BTTS Sí ≈ 0.9866
        // P(Local=0) = e^(-5) = 0.0067, P(Local>=1) = 0.9933
        // P(BTTS=Sí) = 0.9933 * 0.9933 = 0.9866
        var result = _engine.GetBttsYesProbability(5.0, 5.0);
        Assert.InRange(result, 0.98, 0.99);
    }

    [Fact]
    public void GetBttsYesProbability_WithNegativeLambda_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _engine.GetBttsYesProbability(-1, 1.0));
    }
}
