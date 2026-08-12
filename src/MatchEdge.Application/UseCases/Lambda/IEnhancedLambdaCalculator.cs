using MatchEdge.Application.UseCases.Context;

namespace MatchEdge.Application.UseCases.Lambda;

public interface IEnhancedLambdaCalculator
{
    Task<EnhancedLambdaResult> CalculateAsync(
        TeamContextStatistics homeContext,
        TeamContextStatistics awayContext,
        int homeTeamId,
        int awayTeamId,
        int tournamentId);
}

public record EnhancedLambdaResult(
    double LambdaHome,
    double LambdaAway,
    string CalculationMethod);
