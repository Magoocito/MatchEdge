using MatchEdge.Application.Clients;
using MatchEdge.Application.Services;
using MatchEdge.Domain.Models;

namespace MatchEdge.Application.UseCases.Statistics;

public class StatisticsService : IStatisticsService
{
    private readonly ISofaScoreClient _sofaScoreClient;
    private readonly ISeasonService _seasonService;

    public StatisticsService(
        ISofaScoreClient sofaScoreClient,
        ISeasonService seasonService)
    {
        _sofaScoreClient = sofaScoreClient;
        _seasonService = seasonService;
    }

    public async Task<TeamStatistics?> GetTeamStatisticsAsync(int teamId, int tournamentId)
    {
        var seasonId = await _seasonService.GetCurrentSeasonAsync(tournamentId);
        var response = await _sofaScoreClient.GetTeamStatisticsAsync(teamId, tournamentId, seasonId);

        return response?.Statistics;
    }
}
