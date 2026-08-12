using MatchEdge.Application.UseCases.Context;

namespace MatchEdge.Application.UseCases.Lambda;

public interface IEnhancedLambdaCalculator
{
    Task<EnhancedLambdaResult> CalculateAsync(
        TeamContextStatistics homeContext,
        TeamContextStatistics awayContext,
        int homeTeamId,
        int awayTeamId,
        int tournamentId,
        DateTime asOfDateTime,
        int seasonLookback = 2);
}

public record EnhancedLambdaResult(
    double LambdaHome,
    double LambdaAway,
    string CalculationMethod);
