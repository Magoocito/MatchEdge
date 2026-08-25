namespace MatchEdge.Application.UseCases.Historical;

public interface IHistoricalMatchEnumerator
{
    Task<IReadOnlyList<HistoricalMatch>> GetFinishedMatchesAsync(
        int tournamentId,
        IReadOnlyList<int> seasonIds,
        int fromRound,
        int toRound,
        IReadOnlyList<string> prefixes);
}
