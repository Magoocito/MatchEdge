namespace MatchEdge.Application.UseCases.ValueBetting;

public interface IValueBetCalculator
{
    ValueBetAnalysis Analyze(string market, double estimatedProbability, double odds);
}