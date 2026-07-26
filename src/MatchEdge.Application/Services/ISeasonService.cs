namespace MatchEdge.Application.Services;

public interface ISeasonService
{
    Task<int> GetCurrentSeasonAsync(int tournamentId);
}
