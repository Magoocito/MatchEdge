using MatchEdge.Application.UseCases.Context;
using MatchEdge.Domain.Matches;

namespace MatchEdge.UnitTests;

public class TeamContextStatisticsCalculatorTests
{
    private const int TeamId = 100;
    private static readonly DateTime AsOfDateTime = new(2024, 6, 15, 18, 0, 0, DateTimeKind.Utc);

    private readonly TeamContextStatisticsCalculator _calculator = new();

    [Fact]
    public void Calculate_WithHomeAndAwayMatches_ShouldSeparateByRoleAndComputeAverages()
    {
        var matches = new[]
        {
            CreateMatch(
                homeTeamId: TeamId,
                awayTeamId: 200,
                homeScore: 2,
                awayScore: 1,
                kickoffUtc: new DateTime(2024, 6, 10, 20, 0, 0, DateTimeKind.Utc),
                status: "finished"),
            CreateMatch(
                homeTeamId: 300,
                awayTeamId: TeamId,
                homeScore: 0,
                awayScore: 3,
                kickoffUtc: new DateTime(2024, 6, 12, 20, 0, 0, DateTimeKind.Utc),
                status: "finished")
        };

        var result = _calculator.Calculate(TeamId, matches, AsOfDateTime);

        Assert.Equal(1, result.HomeMatchesCount);
        Assert.Equal(1, result.AwayMatchesCount);
        Assert.Equal(0, result.SkippedMatchesCount);
        Assert.Equal(2.0, result.AttackHome);
        Assert.Equal(1.0, result.DefenseHome);
        Assert.Equal(3.0, result.AttackAway);
        Assert.Equal(0.0, result.DefenseAway);
    }

    [Fact]
    public void Calculate_WithMatchOnOrAfterAsOfDateTime_ShouldExcludeMatch()
    {
        var matches = new[]
        {
            CreateMatch(
                homeTeamId: TeamId,
                awayTeamId: 200,
                homeScore: 5,
                awayScore: 0,
                kickoffUtc: AsOfDateTime,
                status: "finished"),
            CreateMatch(
                homeTeamId: TeamId,
                awayTeamId: 201,
                homeScore: 1,
                awayScore: 0,
                kickoffUtc: AsOfDateTime.AddHours(1),
                status: "finished")
        };

        var result = _calculator.Calculate(TeamId, matches, AsOfDateTime);

        Assert.Equal(0, result.HomeMatchesCount);
        Assert.Equal(0, result.AwayMatchesCount);
        Assert.Equal(0.0, result.AttackHome);
    }

    [Fact]
    public void Calculate_WithNotStartedMatch_ShouldExcludeMatch()
    {
        var matches = new[]
        {
            CreateMatch(
                homeTeamId: TeamId,
                awayTeamId: 200,
                homeScore: 2,
                awayScore: 1,
                kickoffUtc: new DateTime(2024, 6, 10, 20, 0, 0, DateTimeKind.Utc),
                status: "notstarted")
        };

        var result = _calculator.Calculate(TeamId, matches, AsOfDateTime);

        Assert.Equal(0, result.HomeMatchesCount);
        Assert.Equal(0, result.AwayMatchesCount);
    }

    [Fact]
    public void Calculate_WithZeroStartTimestamp_ShouldSkipAndIncrementSkippedCount()
    {
        var matches = new[]
        {
            CreateMatch(
                homeTeamId: TeamId,
                awayTeamId: 200,
                homeScore: 2,
                awayScore: 1,
                kickoffUtc: DateTime.MinValue,
                status: "finished",
                startTimestamp: 0)
        };

        var result = _calculator.Calculate(TeamId, matches, AsOfDateTime);

        Assert.Equal(0, result.HomeMatchesCount);
        Assert.Equal(1, result.SkippedMatchesCount);
    }

    [Fact]
    public void Calculate_WithSameDayMatchesBeforeAndAfterAsOfTime_ShouldIncludeOnlyEarlierMatch()
    {
        var matches = new[]
        {
            CreateMatch(
                homeTeamId: TeamId,
                awayTeamId: 200,
                homeScore: 1,
                awayScore: 0,
                kickoffUtc: new DateTime(2024, 6, 15, 15, 0, 0, DateTimeKind.Utc),
                status: "finished"),
            CreateMatch(
                homeTeamId: TeamId,
                awayTeamId: 201,
                homeScore: 4,
                awayScore: 4,
                kickoffUtc: new DateTime(2024, 6, 15, 20, 0, 0, DateTimeKind.Utc),
                status: "finished")
        };

        var result = _calculator.Calculate(TeamId, matches, AsOfDateTime);

        Assert.Equal(1, result.HomeMatchesCount);
        Assert.Equal(1.0, result.AttackHome);
        Assert.Equal(0.0, result.DefenseHome);
    }

    [Fact]
    public void Calculate_WithNoMatches_ShouldReturnZeroCountsWithoutException()
    {
        var result = _calculator.Calculate(TeamId, [], AsOfDateTime);

        Assert.Equal(0, result.HomeMatchesCount);
        Assert.Equal(0, result.AwayMatchesCount);
        Assert.Equal(0, result.SkippedMatchesCount);
        Assert.Equal(0.0, result.AttackHome);
        Assert.Equal(0.0, result.DefenseHome);
        Assert.Equal(0.0, result.AttackAway);
        Assert.Equal(0.0, result.DefenseAway);
    }

    private static FootballMatchEvent CreateMatch(
        int homeTeamId,
        int awayTeamId,
        int homeScore,
        int awayScore,
        DateTime kickoffUtc,
        string status,
        int? startTimestamp = null)
    {
        return new FootballMatchEvent
        {
            HomeTeam = new MatchTeam { Id = homeTeamId },
            AwayTeam = new MatchTeam { Id = awayTeamId },
            HomeScore = new MatchScore { Current = homeScore },
            AwayScore = new MatchScore { Current = awayScore },
            Status = new MatchStatus { Type = status },
            StartTimestamp = startTimestamp ?? (int)new DateTimeOffset(kickoffUtc).ToUnixTimeSeconds()
        };
    }
}
