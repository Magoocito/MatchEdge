using MatchEdge.Domain.Matches;

namespace MatchEdge.Application.UseCases.Historical;

public record HistoricalMatch(FootballMatchEvent Event, int SeasonId, string Prefix);

public interface IHistoricalMatchEnumerator
{
    Task<IReadOnlyList<HistoricalMatch>> GetFinishedMatchesAsync(
        int tournamentId,
        IReadOnlyList<int> seasonIds,
        int fromRound,
        int toRound,
        IReadOnlyList<string> prefixes);
}
