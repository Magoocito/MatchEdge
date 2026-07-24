using MatchEdge.Application.Clients;
using MatchEdge.Domain.Models;

namespace MatchEdge.Application.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ISofaScoreClient _sofaScoreClient;
        public StatisticsService(ISofaScoreClient sofaScoreClient)
        {
            _sofaScoreClient = sofaScoreClient;
        }
        public async Task<TeamStatistics?> GetTeamStatisticsAsync(int teamId, int tournamentId, int seasonId)
        {
            var response = await _sofaScoreClient.GetTeamStatisticsAsync(teamId, tournamentId, seasonId);

            return response?.Statistics;
        }
    }
}
