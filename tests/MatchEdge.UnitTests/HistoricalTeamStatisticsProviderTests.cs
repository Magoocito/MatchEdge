using MatchEdge.Application.UseCases.Context;
using MatchEdge.Application.UseCases.Statistics;

namespace MatchEdge.UnitTests;

public class HistoricalTeamStatisticsProviderTests
{
    [Fact]
    public async Task GetAsOfAsync_KnownContext_DerivesCorrectTotals()
    {
        var context = new TeamContextStatistics(
            AttackHome: 1.8, DefenseHome: 1.0, AttackAway: 1.5, DefenseAway: 1.2,
            HomeMatchesCount: 10, AwayMatchesCount: 8, SkippedMatchesCount: 1);
        var fakeContextService = new FakeTeamContextStatisticsService(context);
        var provider = new HistoricalTeamStatisticsProvider(fakeContextService);

        var result = await provider.GetAsOfAsync(1, 406, new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc));

        // GoalsScored = 1.8*10 + 1.5*8 = 18 + 12 = 30
        // GoalsConceded = 1.0*10 + 1.2*8 = 10 + 9.6 = 19.6 -> round 20
        // Matches = 10 + 8 = 18
        Assert.Equal(30, result.GoalsScored);
        Assert.Equal(20, result.GoalsConceded);
        Assert.Equal(18, result.Matches);
    }

    [Fact]
    public async Task GetAsOfAsync_NoMatchesBeforeAsOfDate_ThrowsInvalidOperationException()
    {
        var context = new TeamContextStatistics(
            AttackHome: 0, DefenseHome: 0, AttackAway: 0, DefenseAway: 0,
            HomeMatchesCount: 0, AwayMatchesCount: 0, SkippedMatchesCount: 0);
        var fakeContextService = new FakeTeamContextStatisticsService(context);
        var provider = new HistoricalTeamStatisticsProvider(fakeContextService);
        var asOfDateTime = new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => provider.GetAsOfAsync(7, 406, asOfDateTime));

        // El mensaje debe incluir teamId, tournamentId y asOfDateTime para poder
        // identificar rápidamente qué combinación equipo/fecha falló durante un backtest.
        Assert.Contains("7", exception.Message);
        Assert.Contains("406", exception.Message);
        Assert.Contains(asOfDateTime.ToString("O"), exception.Message);
    }

    [Fact]
    public async Task GetAsOfAsync_MidpointGoalsConceded_RoundsAwayFromZero()
    {
        var context = new TeamContextStatistics(
            AttackHome: 1.0, DefenseHome: 1.05, AttackAway: 0, DefenseAway: 0,
            HomeMatchesCount: 10, AwayMatchesCount: 0, SkippedMatchesCount: 0);
        var fakeContextService = new FakeTeamContextStatisticsService(context);
        var provider = new HistoricalTeamStatisticsProvider(fakeContextService);

        var result = await provider.GetAsOfAsync(1, 406, new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc));

        // GoalsConceded = 1.05*10 = 10.5 -> AwayFromZero rounds up to 11 (ToEven would give 10)
        Assert.Equal(11, result.GoalsConceded);
    }

    [Fact]
    public async Task GetAsOfAsync_PassesSeasonLookbackThrough()
    {
        var context = new TeamContextStatistics(
            AttackHome: 1.0, DefenseHome: 1.0, AttackAway: 1.0, DefenseAway: 1.0,
            HomeMatchesCount: 5, AwayMatchesCount: 5, SkippedMatchesCount: 0);
        var fakeContextService = new FakeTeamContextStatisticsService(context);
        var provider = new HistoricalTeamStatisticsProvider(fakeContextService);

        await provider.GetAsOfAsync(1, 406, new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc), seasonLookback: 3);

        Assert.Equal(3, fakeContextService.SeasonLookbackReceived);
    }
}

internal class FakeTeamContextStatisticsService : ITeamContextStatisticsService
{
    private readonly TeamContextStatistics _stats;

    public int SeasonLookbackReceived { get; private set; }

    public FakeTeamContextStatisticsService(TeamContextStatistics stats)
    {
        _stats = stats;
    }

    public Task<TeamContextStatistics> CalculateAsync(
        int teamId, int tournamentId, DateTime asOfDateTime, int seasonLookback = 2)
    {
        SeasonLookbackReceived = seasonLookback;
        return Task.FromResult(_stats);
    }
}
