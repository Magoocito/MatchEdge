using MatchEdge.Application.UseCases.Context;
using MatchEdge.Domain.Models;

namespace MatchEdge.Application.UseCases.Statistics;

public class HistoricalTeamStatisticsProvider : IHistoricalTeamStatisticsProvider
{
    private readonly ITeamContextStatisticsService _contextStatisticsService;

    public HistoricalTeamStatisticsProvider(ITeamContextStatisticsService contextStatisticsService)
    {
        _contextStatisticsService = contextStatisticsService;
    }

    public async Task<TeamStatistics> GetAsOfAsync(
        int teamId,
        int tournamentId,
        DateTime asOfDateTime,
        int seasonLookback = 2)
    {
        var context = await _contextStatisticsService.CalculateAsync(
            teamId, tournamentId, asOfDateTime, seasonLookback);

        var totalMatches = context.HomeMatchesCount + context.AwayMatchesCount;

        if (totalMatches == 0)
            throw new InvalidOperationException(
                $"No historical matches available for team {teamId} in tournament {tournamentId} as of {asOfDateTime:O}");

        var goalsScored = (context.AttackHome * context.HomeMatchesCount)
            + (context.AttackAway * context.AwayMatchesCount);
        var goalsConceded = (context.DefenseHome * context.HomeMatchesCount)
            + (context.DefenseAway * context.AwayMatchesCount);

        return new TeamStatistics
        {
            GoalsScored = (int)Math.Round(goalsScored, MidpointRounding.AwayFromZero),
            GoalsConceded = (int)Math.Round(goalsConceded, MidpointRounding.AwayFromZero),
            Matches = totalMatches
        };
    }
}
