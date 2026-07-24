using MatchEdge.Domain.Models;

namespace MatchEdge.Application.Clients
{
    public interface ISofaScoreClient
    {
        Task<SofaScoreStatisticsResponse?> GetTeamStatisticsAsync(
        int teamId, int tournamentId, int seasonId);
    }
}
