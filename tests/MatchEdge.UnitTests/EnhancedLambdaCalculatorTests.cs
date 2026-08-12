using MatchEdge.Application.Configuration;
using MatchEdge.Application.UseCases.Context;
using MatchEdge.Application.UseCases.Lambda;
using MatchEdge.Application.UseCases.Statistics;
using MatchEdge.Domain.Models;
using Microsoft.Extensions.Options;

namespace MatchEdge.UnitTests;

public class EnhancedLambdaCalculatorTests
{
    private readonly IMatchLambdaCalculator _baselineCalculator;
    private readonly MatchModelOptions _options;

    public EnhancedLambdaCalculatorTests()
    {
        _options = new MatchModelOptions { HomeAdvantageFactor = 1.58 };
        _baselineCalculator = new MatchLambdaCalculator(Options.Create(_options));
    }

    private EnhancedLambdaCalculator CreateCalculator(IStatisticsService? statsService = null)
    {
        var mockStats = statsService ?? new FakeStatisticsService(null);
        return new EnhancedLambdaCalculator(_baselineCalculator, mockStats, Options.Create(_options));
    }

    [Fact]
    public async Task CalculateAsync_SufficientData_UsesHomeAwaySplitWithoutGamma()
    {
        var calculator = CreateCalculator();
        var homeContext = new TeamContextStatistics(
            AttackHome: 1.8, DefenseHome: 1.0, AttackAway: 1.5, DefenseAway: 1.2,
            HomeMatchesCount: 10, AwayMatchesCount: 10, SkippedMatchesCount: 0);
        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4, DefenseHome: 1.3, AttackAway: 1.2, DefenseAway: 1.1,
            HomeMatchesCount: 10, AwayMatchesCount: 10, SkippedMatchesCount: 0);

        var result = await calculator.CalculateAsync(homeContext, awayContext, 1, 2, 406);

        // λ_home = (home.AttackHome + away.DefenseAway) / 2 = (1.8 + 1.1) / 2 = 1.45
        // λ_away = (away.AttackAway + home.DefenseHome) / 2 = (1.2 + 1.0) / 2 = 1.1
        Assert.Equal("HomeAwaySplit", result.CalculationMethod);
        Assert.Equal(1.45, result.LambdaHome, 4);
        Assert.Equal(1.1, result.LambdaAway, 4);
    }

    [Fact]
    public async Task CalculateAsync_InsufficientHomeData_FallsBackToBaseline()
    {
        var fakeStats = new FakeStatisticsService(new TeamStatistics
        {
            GoalsScored = 30, GoalsConceded = 15, Matches = 20
        });
        var calculator = CreateCalculator(fakeStats);

        var homeContext = new TeamContextStatistics(
            AttackHome: 1.8, DefenseHome: 1.0, AttackAway: 1.5, DefenseAway: 1.2,
            HomeMatchesCount: 5, AwayMatchesCount: 10, SkippedMatchesCount: 0);
        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4, DefenseHome: 1.3, AttackAway: 1.2, DefenseAway: 1.1,
            HomeMatchesCount: 10, AwayMatchesCount: 10, SkippedMatchesCount: 0);

        var result = await calculator.CalculateAsync(homeContext, awayContext, 1, 2, 406);

        Assert.Equal("SeasonAverageWithGamma", result.CalculationMethod);
    }

    [Fact]
    public async Task CalculateAsync_InsufficientAwayData_FallsBackToBaseline()
    {
        var fakeStats = new FakeStatisticsService(new TeamStatistics
        {
            GoalsScored = 25, GoalsConceded = 20, Matches = 18
        });
        var calculator = CreateCalculator(fakeStats);

        var homeContext = new TeamContextStatistics(
            AttackHome: 1.8, DefenseHome: 1.0, AttackAway: 1.5, DefenseAway: 1.2,
            HomeMatchesCount: 10, AwayMatchesCount: 10, SkippedMatchesCount: 0);
        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4, DefenseHome: 1.3, AttackAway: 1.2, DefenseAway: 1.1,
            HomeMatchesCount: 10, AwayMatchesCount: 5, SkippedMatchesCount: 0);

        var result = await calculator.CalculateAsync(homeContext, awayContext, 1, 2, 406);

        Assert.Equal("SeasonAverageWithGamma", result.CalculationMethod);
    }

    [Fact]
    public async Task CalculateAsync_ExactlyEightMatches_UsesHomeAwaySplit()
    {
        var calculator = CreateCalculator();
        var homeContext = new TeamContextStatistics(
            AttackHome: 1.8, DefenseHome: 1.0, AttackAway: 1.5, DefenseAway: 1.2,
            HomeMatchesCount: 8, AwayMatchesCount: 8, SkippedMatchesCount: 0);
        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4, DefenseHome: 1.3, AttackAway: 1.2, DefenseAway: 1.1,
            HomeMatchesCount: 8, AwayMatchesCount: 8, SkippedMatchesCount: 0);

        var result = await calculator.CalculateAsync(homeContext, awayContext, 1, 2, 406);

        Assert.Equal("HomeAwaySplit", result.CalculationMethod);
    }

    [Fact]
    public async Task CalculateAsync_HomeAwaySplit_DoesNotApplyGamma()
    {
        var calculator = CreateCalculator();
        var homeContext = new TeamContextStatistics(
            AttackHome: 2.0, DefenseHome: 1.0, AttackAway: 1.5, DefenseAway: 1.2,
            HomeMatchesCount: 10, AwayMatchesCount: 10, SkippedMatchesCount: 0);
        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4, DefenseHome: 1.3, AttackAway: 1.0, DefenseAway: 1.0,
            HomeMatchesCount: 10, AwayMatchesCount: 10, SkippedMatchesCount: 0);

        var result = await calculator.CalculateAsync(homeContext, awayContext, 1, 2, 406);

        // λ_home = (2.0 + 1.0) / 2 = 1.5
        // If gamma were applied: 1.5 * 1.58 = 2.37
        Assert.Equal(1.5, result.LambdaHome, 4);
        Assert.DoesNotContain("Gamma", result.CalculationMethod);
    }

    [Fact]
    public async Task CalculateAsync_NullHomeContext_ThrowsArgumentNullException()
    {
        var calculator = CreateCalculator();
        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4, DefenseHome: 1.3, AttackAway: 1.2, DefenseAway: 1.1,
            HomeMatchesCount: 10, AwayMatchesCount: 10, SkippedMatchesCount: 0);

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => calculator.CalculateAsync(null!, awayContext, 1, 2, 406));
    }

    [Fact]
    public async Task CalculateAsync_NullAwayContext_ThrowsArgumentNullException()
    {
        var calculator = CreateCalculator();
        var homeContext = new TeamContextStatistics(
            AttackHome: 1.8, DefenseHome: 1.0, AttackAway: 1.5, DefenseAway: 1.2,
            HomeMatchesCount: 10, AwayMatchesCount: 10, SkippedMatchesCount: 0);

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => calculator.CalculateAsync(homeContext, null!, 1, 2, 406));
    }

    [Fact]
    public async Task CalculateAsync_FallbackUsesRealFullSeasonStats()
    {
        var homeFullStats = new TeamStatistics
        {
            GoalsScored = 40,
            GoalsConceded = 20,
            Matches = 22
        };
        var awayFullStats = new TeamStatistics
        {
            GoalsScored = 30,
            GoalsConceded = 25,
            Matches = 20
        };

        var trackingService = new TrackingStatisticsService(homeFullStats, awayFullStats);
        var calculator = CreateCalculator(trackingService);

        var homeContext = new TeamContextStatistics(
            AttackHome: 1.8, DefenseHome: 1.0, AttackAway: 1.5, DefenseAway: 1.2,
            HomeMatchesCount: 5, AwayMatchesCount: 10, SkippedMatchesCount: 0);
        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4, DefenseHome: 1.3, AttackAway: 1.2, DefenseAway: 1.1,
            HomeMatchesCount: 10, AwayMatchesCount: 4, SkippedMatchesCount: 0);

        var result = await calculator.CalculateAsync(homeContext, awayContext, 1, 2, 406);

        Assert.Equal("SeasonAverageWithGamma", result.CalculationMethod);

        // Verify the calculator used the REAL full-season stats, not reconstructed partial data
        Assert.Equal(1, trackingService.HomeTeamIdRequested);
        Assert.Equal(2, trackingService.AwayTeamIdRequested);
        Assert.Equal(406, trackingService.TournamentIdRequested);
        Assert.Equal(22, trackingService.HomeStatsUsed!.Matches);
        Assert.Equal(20, trackingService.AwayStatsUsed!.Matches);
    }

    [Fact]
    public async Task CalculateAsync_NullStatsFromService_ThrowsInvalidOperationException()
    {
        var fakeStats = new FakeStatisticsService(null);
        var calculator = CreateCalculator(fakeStats);

        var homeContext = new TeamContextStatistics(
            AttackHome: 1.8, DefenseHome: 1.0, AttackAway: 1.5, DefenseAway: 1.2,
            HomeMatchesCount: 5, AwayMatchesCount: 10, SkippedMatchesCount: 0);
        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4, DefenseHome: 1.3, AttackAway: 1.2, DefenseAway: 1.1,
            HomeMatchesCount: 10, AwayMatchesCount: 10, SkippedMatchesCount: 0);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => calculator.CalculateAsync(homeContext, awayContext, 1, 2, 406));
    }
}

internal class FakeStatisticsService : IStatisticsService
{
    private readonly TeamStatistics? _stats;

    public FakeStatisticsService(TeamStatistics? stats)
    {
        _stats = stats;
    }

    public Task<TeamStatistics?> GetTeamStatisticsAsync(int teamId, int tournamentId)
        => Task.FromResult(_stats);
}

internal class TrackingStatisticsService : IStatisticsService
{
    private readonly TeamStatistics? _homeStats;
    private readonly TeamStatistics? _awayStats;

    public int HomeTeamIdRequested { get; private set; }
    public int AwayTeamIdRequested { get; private set; }
    public int TournamentIdRequested { get; private set; }
    public TeamStatistics? HomeStatsUsed { get; private set; }
    public TeamStatistics? AwayStatsUsed { get; private set; }

    public TrackingStatisticsService(TeamStatistics? homeStats, TeamStatistics? awayStats)
    {
        _homeStats = homeStats;
        _awayStats = awayStats;
    }

    public Task<TeamStatistics?> GetTeamStatisticsAsync(int teamId, int tournamentId)
    {
        TournamentIdRequested = tournamentId;

        // First call is home team, second is away team
        if (HomeStatsUsed == null)
        {
            HomeTeamIdRequested = teamId;
            HomeStatsUsed = _homeStats;
            return Task.FromResult(_homeStats);
        }
        else
        {
            AwayTeamIdRequested = teamId;
            AwayStatsUsed = _awayStats;
            return Task.FromResult(_awayStats);
        }
    }
}
