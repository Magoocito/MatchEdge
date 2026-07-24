using MatchEdge.Domain.Models;

namespace MatchEdge.Application.Services
{
    public interface IStatisticsService
    {
        Task<TeamStatistics?> GetTeamStatisticsAsync(
        int teamId, int tournamentId, int seasonId);
    }
}
