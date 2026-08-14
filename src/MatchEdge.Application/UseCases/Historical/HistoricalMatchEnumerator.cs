using MatchEdge.Application.Clients;
using MatchEdge.Domain.Matches;

namespace MatchEdge.Application.UseCases.Historical;

public class HistoricalMatchEnumerator : IHistoricalMatchEnumerator
{
    private readonly ISofaScoreClient _sofaScoreClient;

    public HistoricalMatchEnumerator(ISofaScoreClient sofaScoreClient)
    {
        _sofaScoreClient = sofaScoreClient;
    }

    public async Task<IReadOnlyList<HistoricalMatch>> GetFinishedMatchesAsync(
        int tournamentId,
        IReadOnlyList<int> seasonIds,
        int fromRound,
        int toRound,
        IReadOnlyList<string> prefixes)
    {
        var allMatches = new List<HistoricalMatch>();
        var seenIds = new HashSet<int>();

        foreach (var seasonId in seasonIds)
        {
            foreach (var prefix in prefixes)
            {
                for (var round = fromRound; round <= toRound; round++)
                {
                    var response = await _sofaScoreClient.GetMatchEventsByRoundAsync(
                        tournamentId,
                        seasonId,
                        round,
                        prefix);

                    if (response?.Events is not { Count: > 0 })
                        continue;

                    foreach (var match in response.Events)
                    {
                        if (match.Status.Type != "finished")
                            continue;

                        if (!match.HomeScore.Current.HasValue || !match.AwayScore.Current.HasValue)
                            continue;

                        if (!seenIds.Add(match.Id))
                            continue;

                        allMatches.Add(new HistoricalMatch(match, seasonId, prefix));
                    }
                }
            }
        }

        return allMatches;
    }
}
