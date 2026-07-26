using MatchEdge.Application.Clients;
using MatchEdge.Domain.Teams;

namespace MatchEdge.Application.Services;

public class TeamService : ITeamService
{
    private readonly ISofaScoreClient _sofaScoreClient;
    private readonly ISeasonService _seasonService;

    public TeamService(
        ISofaScoreClient sofaScoreClient,
        ISeasonService seasonService)
    {
        _sofaScoreClient = sofaScoreClient;
        _seasonService = seasonService;
    }

    public async Task<List<Team>?> GetTeamsAsync(int tournamentId)
    {
        var seasonId = await _seasonService.GetCurrentSeasonAsync(tournamentId);
        return await _sofaScoreClient.GetTeamsAsync(tournamentId, seasonId);
    }
}
