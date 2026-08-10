using MatchEdge.Domain.Matches;

namespace MatchEdge.Application.UseCases.Context;

public class TeamContextStatisticsCalculator
{
    public TeamContextStatistics Calculate(
        int teamId,
        IEnumerable<FootballMatchEvent> matches,
        DateTime asOfDateTime)
    {
        var asOfUtc = ToUtc(asOfDateTime);

        var homeMatchesCount = 0;
        var awayMatchesCount = 0;
        var skippedMatchesCount = 0;
        var homeGoalsScored = 0;
        var homeGoalsConceded = 0;
        var awayGoalsScored = 0;
        var awayGoalsConceded = 0;

        foreach (var match in matches)
        {
            if (match.StartTimestamp == 0)
            {
                skippedMatchesCount++;
                continue;
            }

            if (!string.Equals(match.Status.Type, "finished", StringComparison.OrdinalIgnoreCase))
                continue;

            var kickoffUtc = DateTimeOffset.FromUnixTimeSeconds(match.StartTimestamp).UtcDateTime;
            if (kickoffUtc >= asOfUtc)
                continue;

            if (!match.HomeScore.Current.HasValue || !match.AwayScore.Current.HasValue)
                continue;

            if (match.HomeTeam.Id == teamId)
            {
                homeMatchesCount++;
                homeGoalsScored += match.HomeScore.Current.Value;
                homeGoalsConceded += match.AwayScore.Current.Value;
            }
            else if (match.AwayTeam.Id == teamId)
            {
                awayMatchesCount++;
                awayGoalsScored += match.AwayScore.Current.Value;
                awayGoalsConceded += match.HomeScore.Current.Value;
            }
        }

        return new TeamContextStatistics(
            AttackHome: homeMatchesCount > 0 ? (double)homeGoalsScored / homeMatchesCount : 0,
            DefenseHome: homeMatchesCount > 0 ? (double)homeGoalsConceded / homeMatchesCount : 0,
            AttackAway: awayMatchesCount > 0 ? (double)awayGoalsScored / awayMatchesCount : 0,
            DefenseAway: awayMatchesCount > 0 ? (double)awayGoalsConceded / awayMatchesCount : 0,
            HomeMatchesCount: homeMatchesCount,
            AwayMatchesCount: awayMatchesCount,
            SkippedMatchesCount: skippedMatchesCount);
    }

    private static DateTime ToUtc(DateTime value) =>
        value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
}
