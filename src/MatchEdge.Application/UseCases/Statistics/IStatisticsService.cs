using MatchEdge.Domain.Models;

namespace MatchEdge.Application.UseCases.Statistics;

public interface IStatisticsService
{
    Task<TeamStatistics?> GetTeamStatisticsAsync(int teamId, int tournamentId);
}
