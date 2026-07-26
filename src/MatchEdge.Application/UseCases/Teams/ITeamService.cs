using MatchEdge.Domain.Teams;

namespace MatchEdge.Application.UseCases.Teams;

public interface ITeamService
{
    Task<List<Team>?> GetTeamsAsync(int tournamentId);
}
