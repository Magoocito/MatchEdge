namespace MatchEdge.Application.UseCases.Context;

public interface ITeamContextStatisticsService
{
    Task<TeamContextStatistics> CalculateAsync(
        int teamId,
        int tournamentId,
        DateTime asOfDateTime,
        int seasonLookback = 2);
}
