using MatchEdge.Domain.Teams;

namespace MatchEdge.Application.Services;

public interface ITeamService
{
    Task<List<Team>?> GetTeamsAsync(int tournamentId);
}
