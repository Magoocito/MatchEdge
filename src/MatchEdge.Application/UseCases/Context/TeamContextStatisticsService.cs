using MatchEdge.Application.Clients;
using MatchEdge.Application.Services;
using MatchEdge.Domain.Matches;

namespace MatchEdge.Application.UseCases.Context;

public class TeamContextStatisticsService : ITeamContextStatisticsService
{
    private static readonly string[] Prefixes = ["Apertura", "Clausura"];
    private const int FromRound = 1;
    private const int ToRound = 17;

    private readonly ISofaScoreClient _sofaScoreClient;
    private readonly ISeasonService _seasonService;
    private readonly TeamContextStatisticsCalculator _calculator;

    public TeamContextStatisticsService(
        ISofaScoreClient sofaScoreClient,
        ISeasonService seasonService,
        TeamContextStatisticsCalculator calculator)
    {
        _sofaScoreClient = sofaScoreClient;
        _seasonService = seasonService;
        _calculator = calculator;
    }

    public async Task<TeamContextStatistics> CalculateAsync(
        int teamId,
        int tournamentId,
        DateTime asOfDateTime,
        int seasonLookback = 2)
    {
        if (seasonLookback <= 0)
            throw new ArgumentException("Season lookback must be greater than zero", nameof(seasonLookback));

        var seasonIds = await _seasonService.GetRecentSeasonIdsAsOfAsync(
            tournamentId,
            seasonLookback,
            asOfDateTime);

        var matches = new List<FootballMatchEvent>();

        foreach (var seasonId in seasonIds)
        {
            foreach (var prefix in Prefixes)
            {
                for (var round = FromRound; round <= ToRound; round++)
                {
                    var response = await _sofaScoreClient.GetMatchEventsByRoundAsync(
                        tournamentId,
                        seasonId,
                        round,
                        prefix);

                    if (response?.Events is { Count: > 0 })
                    {
                        matches.AddRange(response.Events);
                    }
                }
            }
        }

        return _calculator.Calculate(teamId, matches, asOfDateTime);
    }
}
