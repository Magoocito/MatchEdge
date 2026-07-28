using MatchEdge.Domain.Models;
using MatchEdge.Domain.Matches;
using MatchEdge.Domain.Teams;

namespace MatchEdge.Application.Clients
{
    public interface ISofaScoreClient
    {
        Task<SofaScoreStatisticsResponse?> GetTeamStatisticsAsync(
        int teamId, int tournamentId, int seasonId);

        Task<List<Team>?> GetTeamsAsync(int tournamentId, int seasonId);

        Task<MatchEventsResponse?> GetMatchEventsByRoundAsync(
            int tournamentId,
            int seasonId,
            int round,
            string prefix);
    }
}
