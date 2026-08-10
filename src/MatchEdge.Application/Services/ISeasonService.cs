namespace MatchEdge.Application.Services;

public interface ISeasonService
{
    Task<int> GetCurrentSeasonAsync(int tournamentId);

    Task<List<int>> GetRecentSeasonIdsAsync(int tournamentId, int count);

    Task<List<int>> GetRecentSeasonIdsAsOfAsync(int tournamentId, int count, DateTime asOfDateTime);

    Task<string> GetSeasonNameAsync(int tournamentId, int seasonId);
}
