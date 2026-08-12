using MatchEdge.Application.UseCases.Context;

namespace MatchEdge.Application.UseCases.Lambda;

public interface IEnhancedLambdaCalculator
{
    EnhancedLambdaResult Calculate(
        TeamContextStatistics homeContext,
        TeamContextStatistics awayContext);
}

public record EnhancedLambdaResult(
    double LambdaHome,
    double LambdaAway,
    string CalculationMethod);
