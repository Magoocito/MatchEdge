namespace MatchEdge.Application.UseCases.ValueBetting;

public record ValueBetAnalysis(
    string Market,
    double Odds,
    double ImpliedProbability,
    double EstimatedProbability,
    double Edge,
    double ExpectedValue,
    string Classification);