using MatchEdge.Application.Configuration;
using MatchEdge.Application.UseCases.Context;
using MatchEdge.Application.UseCases.Lambda;
using MatchEdge.Application.UseCases.Statistics;
using MatchEdge.Domain.Models;
using Microsoft.Extensions.Options;

namespace MatchEdge.UnitTests;

public class EnhancedLambdaCalculatorTests
{
    private static readonly DateTime AsOfDateTime = new(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc);

    private readonly IMatchLambdaCalculator _baselineCalculator;
    private readonly MatchModelOptions _options;

    public EnhancedLambdaCalculatorTests()
    {
        _options = new MatchModelOptions { HomeAdvantageFactor = 1.58 };
        _baselineCalculator = new MatchLambdaCalculator(Options.Create(_options));
    }

    private EnhancedLambdaCalculator CreateCalculator(IHistoricalTeamStatisticsProvider? historicalProvider = null)
    {
        var provider = historicalProvider ?? FakeHistoricalTeamStatisticsProvider.Returning(null);
        return new EnhancedLambdaCalculator(_baselineCalculator, provider, Options.Create(_options));
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

        var result = await calculator.CalculateAsync(homeContext, awayContext, 1, 2, 406, AsOfDateTime);

        // λ_home = (home.AttackHome + away.DefenseAway) / 2 = (1.8 + 1.1) / 2 = 1.45
        // λ_away = (away.AttackAway + home.DefenseHome) / 2 = (1.2 + 1.0) / 2 = 1.1
        Assert.Equal("HomeAwaySplit", result.CalculationMethod);
        Assert.Equal(1.45, result.LambdaHome, 4);
        Assert.Equal(1.1, result.LambdaAway, 4);
    }

    [Fact]
    public async Task CalculateAsync_InsufficientHomeData_FallsBackToBaseline()
    {
        var fakeProvider = FakeHistoricalTeamStatisticsProvider.Returning(new TeamStatistics
        {
            GoalsScored = 30, GoalsConceded = 15, Matches = 20
        });
        var calculator = CreateCalculator(fakeProvider);

        var homeContext = new TeamContextStatistics(
            AttackHome: 1.8, DefenseHome: 1.0, AttackAway: 1.5, DefenseAway: 1.2,
            HomeMatchesCount: 5, AwayMatchesCount: 10, SkippedMatchesCount: 0);
        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4, DefenseHome: 1.3, AttackAway: 1.2, DefenseAway: 1.1,
            HomeMatchesCount: 10, AwayMatchesCount: 10, SkippedMatchesCount: 0);

        var result = await calculator.CalculateAsync(homeContext, awayContext, 1, 2, 406, AsOfDateTime);

        Assert.Equal("SeasonAverageWithGamma", result.CalculationMethod);
    }

    [Fact]
    public async Task CalculateAsync_InsufficientAwayData_FallsBackToBaseline()
    {
        var fakeProvider = FakeHistoricalTeamStatisticsProvider.Returning(new TeamStatistics
        {
            GoalsScored = 25, GoalsConceded = 20, Matches = 18
        });
        var calculator = CreateCalculator(fakeProvider);

        var homeContext = new TeamContextStatistics(
            AttackHome: 1.8, DefenseHome: 1.0, AttackAway: 1.5, DefenseAway: 1.2,
            HomeMatchesCount: 10, AwayMatchesCount: 10, SkippedMatchesCount: 0);
        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4, DefenseHome: 1.3, AttackAway: 1.2, DefenseAway: 1.1,
            HomeMatchesCount: 10, AwayMatchesCount: 5, SkippedMatchesCount: 0);

        var result = await calculator.CalculateAsync(homeContext, awayContext, 1, 2, 406, AsOfDateTime);

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

        var result = await calculator.CalculateAsync(homeContext, awayContext, 1, 2, 406, AsOfDateTime);

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

        var result = await calculator.CalculateAsync(homeContext, awayContext, 1, 2, 406, AsOfDateTime);

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
            () => calculator.CalculateAsync(null!, awayContext, 1, 2, 406, AsOfDateTime));
    }

    [Fact]
    public async Task CalculateAsync_NullAwayContext_ThrowsArgumentNullException()
    {
        var calculator = CreateCalculator();
        var homeContext = new TeamContextStatistics(
            AttackHome: 1.8, DefenseHome: 1.0, AttackAway: 1.5, DefenseAway: 1.2,
            HomeMatchesCount: 10, AwayMatchesCount: 10, SkippedMatchesCount: 0);

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => calculator.CalculateAsync(homeContext, null!, 1, 2, 406, AsOfDateTime));
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

        const int homeTeamId = 1;
        const int awayTeamId = 2;
        var trackingProvider = FakeHistoricalTeamStatisticsProvider.ReturningByTeam(
            teamId => teamId == homeTeamId ? homeFullStats : awayFullStats);
        var calculator = CreateCalculator(trackingProvider);

        var homeContext = new TeamContextStatistics(
            AttackHome: 1.8, DefenseHome: 1.0, AttackAway: 1.5, DefenseAway: 1.2,
            HomeMatchesCount: 5, AwayMatchesCount: 10, SkippedMatchesCount: 0);
        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4, DefenseHome: 1.3, AttackAway: 1.2, DefenseAway: 1.1,
            HomeMatchesCount: 10, AwayMatchesCount: 4, SkippedMatchesCount: 0);

        var result = await calculator.CalculateAsync(homeContext, awayContext, homeTeamId, awayTeamId, 406, AsOfDateTime);

        Assert.Equal("SeasonAverageWithGamma", result.CalculationMethod);

        // Verify the calculator queried the historical provider for each specific team
        // (not swapped) with the right tournament and asOfDateTime — proof it used
        // REAL full-season stats, not reconstructed partial data.
        Assert.Equal(2, trackingProvider.Calls.Count);
        Assert.Equal(homeTeamId, trackingProvider.Calls[0].TeamId);
        Assert.Equal(awayTeamId, trackingProvider.Calls[1].TeamId);
        Assert.All(trackingProvider.Calls, call =>
        {
            Assert.Equal(406, call.TournamentId);
            Assert.Equal(AsOfDateTime, call.AsOfDateTime);
        });
    }

    [Fact]
    public async Task CalculateAsync_NullStatsFromService_ThrowsInvalidOperationException()
    {
        var fakeProvider = FakeHistoricalTeamStatisticsProvider.Returning(null);
        var calculator = CreateCalculator(fakeProvider);

        var homeContext = new TeamContextStatistics(
            AttackHome: 1.8, DefenseHome: 1.0, AttackAway: 1.5, DefenseAway: 1.2,
            HomeMatchesCount: 5, AwayMatchesCount: 10, SkippedMatchesCount: 0);
        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4, DefenseHome: 1.3, AttackAway: 1.2, DefenseAway: 1.1,
            HomeMatchesCount: 10, AwayMatchesCount: 10, SkippedMatchesCount: 0);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => calculator.CalculateAsync(homeContext, awayContext, 1, 2, 406, AsOfDateTime));
    }

    [Fact]
    public async Task CalculateAsync_PassesSeasonLookbackThroughToFallbackProvider()
    {
        var homeFullStats = new TeamStatistics { GoalsScored = 40, GoalsConceded = 20, Matches = 22 };
        var awayFullStats = new TeamStatistics { GoalsScored = 30, GoalsConceded = 25, Matches = 20 };
        var trackingProvider = FakeHistoricalTeamStatisticsProvider.ReturningByTeam(
            teamId => teamId == 1 ? homeFullStats : awayFullStats);
        var calculator = CreateCalculator(trackingProvider);

        var homeContext = new TeamContextStatistics(
            AttackHome: 1.8, DefenseHome: 1.0, AttackAway: 1.5, DefenseAway: 1.2,
            HomeMatchesCount: 5, AwayMatchesCount: 10, SkippedMatchesCount: 0);
        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4, DefenseHome: 1.3, AttackAway: 1.2, DefenseAway: 1.1,
            HomeMatchesCount: 10, AwayMatchesCount: 4, SkippedMatchesCount: 0);

        await calculator.CalculateAsync(homeContext, awayContext, 1, 2, 406, AsOfDateTime, seasonLookback: 3);

        Assert.All(trackingProvider.Calls, call => Assert.Equal(3, call.SeasonLookback));
    }

    [Fact]
    public async Task CalculateAsync_ApplyGammaToSplitTrue_MultipliesLambdaHomeByGamma()
    {
        var options = new MatchModelOptions { HomeAdvantageFactor = 1.58 };
        var calculator = new EnhancedLambdaCalculator(
            _baselineCalculator,
            FakeHistoricalTeamStatisticsProvider.Returning(null),
            Options.Create(options),
            applyGammaToSplit: true);

        var homeContext = new TeamContextStatistics(
            AttackHome: 2.0, DefenseHome: 1.0, AttackAway: 1.5, DefenseAway: 1.2,
            HomeMatchesCount: 10, AwayMatchesCount: 10, SkippedMatchesCount: 0);
        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4, DefenseHome: 1.3, AttackAway: 1.0, DefenseAway: 1.0,
            HomeMatchesCount: 10, AwayMatchesCount: 10, SkippedMatchesCount: 0);

        var result = await calculator.CalculateAsync(homeContext, awayContext, 1, 2, 406, AsOfDateTime);

        // lambdaHome = (2.0 + 1.0) / 2 = 1.5, then * 1.58 = 2.37
        Assert.Equal("HomeAwaySplit", result.CalculationMethod);
        Assert.Equal(2.37, result.LambdaHome, 4);
    }

    [Fact]
    public async Task CalculateAsync_ApplyGammaToSplitFalse_DoesNotApplyGamma()
    {
        var options = new MatchModelOptions { HomeAdvantageFactor = 1.58 };
        var calculator = new EnhancedLambdaCalculator(
            _baselineCalculator,
            FakeHistoricalTeamStatisticsProvider.Returning(null),
            Options.Create(options),
            applyGammaToSplit: false);

        var homeContext = new TeamContextStatistics(
            AttackHome: 2.0, DefenseHome: 1.0, AttackAway: 1.5, DefenseAway: 1.2,
            HomeMatchesCount: 10, AwayMatchesCount: 10, SkippedMatchesCount: 0);
        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4, DefenseHome: 1.3, AttackAway: 1.0, DefenseAway: 1.0,
            HomeMatchesCount: 10, AwayMatchesCount: 10, SkippedMatchesCount: 0);

        var result = await calculator.CalculateAsync(homeContext, awayContext, 1, 2, 406, AsOfDateTime);

        // lambdaHome = (2.0 + 1.0) / 2 = 1.5 (no gamma)
        Assert.Equal("HomeAwaySplit", result.CalculationMethod);
        Assert.Equal(1.5, result.LambdaHome, 4);
    }

    [Fact]
    public async Task CalculateAsync_ApplyGammaToSplit_DoesNotAffectLambdaAway()
    {
        var options = new MatchModelOptions { HomeAdvantageFactor = 1.58 };
        var calculatorWithGamma = new EnhancedLambdaCalculator(
            _baselineCalculator,
            FakeHistoricalTeamStatisticsProvider.Returning(null),
            Options.Create(options),
            applyGammaToSplit: true);

        var homeContext = new TeamContextStatistics(
            AttackHome: 2.0, DefenseHome: 1.0, AttackAway: 1.5, DefenseAway: 1.2,
            HomeMatchesCount: 10, AwayMatchesCount: 10, SkippedMatchesCount: 0);
        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4, DefenseHome: 1.3, AttackAway: 1.0, DefenseAway: 1.0,
            HomeMatchesCount: 10, AwayMatchesCount: 10, SkippedMatchesCount: 0);

        var result = await calculatorWithGamma.CalculateAsync(homeContext, awayContext, 1, 2, 406, AsOfDateTime);

        // lambdaAway = (1.0 + 1.0) / 2 = 1.0 (gamma only affects lambdaHome)
        Assert.Equal(1.0, result.LambdaAway, 4);
    }

    [Fact]
    public async Task CalculateAsync_ApplyGammaToSplit_FallbackStillUsesBaselineWithGamma()
    {
        var options = new MatchModelOptions { HomeAdvantageFactor = 1.58 };
        var fakeProvider = FakeHistoricalTeamStatisticsProvider.ReturningByTeam(
            teamId => teamId == 1
                ? new TeamStatistics { GoalsScored = 40, GoalsConceded = 20, Matches = 22 }
                : new TeamStatistics { GoalsScored = 30, GoalsConceded = 25, Matches = 20 });

        var calculator = new EnhancedLambdaCalculator(
            _baselineCalculator,
            fakeProvider,
            Options.Create(options),
            applyGammaToSplit: true);

        var homeContext = new TeamContextStatistics(
            AttackHome: 1.8, DefenseHome: 1.0, AttackAway: 1.5, DefenseAway: 1.2,
            HomeMatchesCount: 5, AwayMatchesCount: 10, SkippedMatchesCount: 0);
        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4, DefenseHome: 1.3, AttackAway: 1.2, DefenseAway: 1.1,
            HomeMatchesCount: 10, AwayMatchesCount: 4, SkippedMatchesCount: 0);

        var result = await calculator.CalculateAsync(homeContext, awayContext, 1, 2, 406, AsOfDateTime);

        // Fallback path uses MatchLambdaCalculator which always applies gamma
        Assert.Equal("SeasonAverageWithGamma", result.CalculationMethod);
    }
}

/// <summary>
/// Fake único para IHistoricalTeamStatisticsProvider, mismo patrón que FakeHttpRequestExecutor:
/// una respuesta configurable via Func (aquí indexada por teamId en vez de por URL) más registro
/// de invocaciones en Calls. Antes existían dos clases separadas (una de "valor fijo" y otra de
/// "rastreo de llamadas por equipo"); se consolidaron porque cubrían la misma necesidad.
/// </summary>
internal class FakeHistoricalTeamStatisticsProvider : IHistoricalTeamStatisticsProvider
{
    private readonly Func<int, TeamStatistics?> _responseFunc;

    public List<HistoricalStatsCall> Calls { get; } = new();

    private FakeHistoricalTeamStatisticsProvider(Func<int, TeamStatistics?> responseFunc)
    {
        _responseFunc = responseFunc;
    }

    public static FakeHistoricalTeamStatisticsProvider Returning(TeamStatistics? stats) =>
        new(_ => stats);

    public static FakeHistoricalTeamStatisticsProvider ReturningByTeam(Func<int, TeamStatistics?> responseFunc) =>
        new(responseFunc);

    public Task<TeamStatistics> GetAsOfAsync(int teamId, int tournamentId, DateTime asOfDateTime, int seasonLookback = 2)
    {
        Calls.Add(new HistoricalStatsCall(teamId, tournamentId, asOfDateTime, seasonLookback));

        var stats = _responseFunc(teamId);
        if (stats == null)
            throw new InvalidOperationException(
                $"No historical matches available for team {teamId} in tournament {tournamentId} as of {asOfDateTime:O}");

        return Task.FromResult(stats);
    }
}

internal record HistoricalStatsCall(int TeamId, int TournamentId, DateTime AsOfDateTime, int SeasonLookback);
