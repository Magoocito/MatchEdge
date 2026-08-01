using MatchEdge.Application.UseCases.ValueBetting;

namespace MatchEdge.UnitTests;

public class ValueBetCalculatorTests
{
    private readonly ValueBetCalculator _calculator = new();

    [Fact]
    public void Analyze_WithPE060_AndOdds200_ShouldReturnExcelente()
    {
        // PI = 1/2.00 = 0.50, Edge = 0.60 - 0.50 = 0.10
        // EV = (0.60 * 2.00) - 1 = 0.20 → > 0.15 → "Excelente"
        var result = _calculator.Analyze("HomeWin", 0.60, 2.00);

        Assert.Equal("HomeWin", result.Market);
        Assert.Equal(2.00, result.Odds);
        Assert.Equal(0.50, result.ImpliedProbability);
        Assert.Equal(0.60, result.EstimatedProbability);
        Assert.Equal(0.10, result.Edge);
        Assert.Equal(0.20, result.ExpectedValue);
        Assert.Equal("Excelente", result.Classification);
    }

    [Fact]
    public void Analyze_WithPE040_AndOdds200_ShouldReturnDescartar()
    {
        // PI = 1/2.00 = 0.50, Edge = 0.40 - 0.50 = -0.10
        // EV = (0.40 * 2.00) - 1 = -0.20 → < 0 → "Descartar"
        var result = _calculator.Analyze("AwayWin", 0.40, 2.00);

        Assert.Equal("AwayWin", result.Market);
        Assert.Equal(2.00, result.Odds);
        Assert.Equal(0.50, result.ImpliedProbability);
        Assert.Equal(0.40, result.EstimatedProbability);
        Assert.Equal(-0.10, result.Edge);
        Assert.Equal(-0.20, result.ExpectedValue);
        Assert.Equal("Descartar", result.Classification);
    }

    [Fact]
    public void Analyze_WithOddsExactly1_ShouldThrowArgumentException()
    {
        // odds = 1.0, no cumple odds > 1.0
        Assert.Throws<ArgumentException>(() => _calculator.Analyze("Draw", 0.40, 1.0));
    }

    [Fact]
    public void Analyze_WithOddsBelow1_ShouldThrowArgumentException()
    {
        // odds = 0.5, menor a 1.0
        Assert.Throws<ArgumentException>(() => _calculator.Analyze("Over2.5", 0.60, 0.5));
    }

    [Fact]
    public void Analyze_WithEVExactly015_ShouldReturnMuyBueno()
    {
        // EV = 0.15 exacto → 0.15 > 0.15 = false → "Muy bueno"
        // odds = (EV + 1) / PE = 1.15 / 0.60 = 1.91666...
        var result = _calculator.Analyze("HomeWin", 0.60, 1.15 / 0.60);

        Assert.Equal(0.15, result.ExpectedValue, 4);
        Assert.Equal("Muy bueno", result.Classification);
    }

    [Fact]
    public void Analyze_WithEVExactly008_ShouldReturnAceptable()
    {
        // EV = 0.08 exacto → 0.08 > 0.08 = false → "Aceptable"
        // odds = (EV + 1) / PE = 1.08 / 0.50 = 2.16
        var result = _calculator.Analyze("Draw", 0.50, 1.08 / 0.50);

        Assert.Equal(0.08, result.ExpectedValue, 4);
        Assert.Equal("Aceptable", result.Classification);
    }

    [Fact]
    public void Analyze_WithEVExactly003_ShouldReturnNeutral()
    {
        // EV = 0.03 exacto → 0.03 > 0.03 = false → "Neutral"
        // odds = (EV + 1) / PE = 1.03 / 0.50 = 2.06
        var result = _calculator.Analyze("AwayWin", 0.50, 1.03 / 0.50);

        Assert.Equal(0.03, result.ExpectedValue, 4);
        Assert.Equal("Neutral", result.Classification);
    }

    [Fact]
    public void Analyze_WithEVExactly000_ShouldReturnNeutral()
    {
        // EV = 0.00 exacto → 0.0 >= 0.0 = true → "Neutral"
        // odds = (EV + 1) / PE = 1.0 / 0.50 = 2.0
        var result = _calculator.Analyze("Over2.5", 0.50, 1.0 / 0.50);

        Assert.Equal(0.0, result.ExpectedValue, 4);
        Assert.Equal("Neutral", result.Classification);
    }
}
