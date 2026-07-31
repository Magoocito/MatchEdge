namespace MatchEdge.Application.UseCases.ValueBetting;

public class ValueBetCalculator : IValueBetCalculator
{
    public ValueBetAnalysis Analyze(string market, double estimatedProbability, double odds)
    {
        if (odds <= 1.0)
            throw new ArgumentException("Odds must be greater than 1.0", nameof(odds));

        double impliedProbability = 1.0 / odds;
        double edge = estimatedProbability - impliedProbability;
        double expectedValue = (estimatedProbability * odds) - 1.0;

        string classification = ClassifyExpectedValue(expectedValue);

        return new ValueBetAnalysis(
            market,
            odds,
            Math.Round(impliedProbability, 4),
            Math.Round(estimatedProbability, 4),
            Math.Round(edge, 4),
            Math.Round(expectedValue, 4),
            classification);
    }

    private static string ClassifyExpectedValue(double expectedValue)
    {
        if (expectedValue > 0.15) return "Excelente";
        if (expectedValue > 0.08) return "Muy bueno";
        if (expectedValue > 0.03) return "Aceptable";
        if (expectedValue >= 0.0) return "Neutral";
        return "Descartar";
    }
}