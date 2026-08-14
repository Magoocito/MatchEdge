using MatchEdge.Application.Clients;
using MatchEdge.Application.Configuration;
using MatchEdge.Application.Services;
using MatchEdge.Application.UseCases.Backtesting;
using MatchEdge.Application.UseCases.Context;
using MatchEdge.Application.UseCases.Historical;
using MatchEdge.Application.UseCases.Probability;
using MatchEdge.Application.UseCases.Statistics;
using MatchEdge.Domain.Matches;
using MatchEdge.Domain.Models;
using MatchEdge.Domain.Teams;
using Microsoft.Extensions.Options;

namespace MatchEdge.UnitTests;

public class BacktestingServiceTests
{
    private const int TournamentId = 406;

    [Fact]
    public async Task RunAsync_UsesSameAsOfDateTimeForAllModels()
    {
        var asOfDates = new List<DateTime>();
        var fakeContextStats = new FakeBacktestingTeamContextStatisticsService(asOfDates: asOfDates);
        var fakeHistoricalStats = new FakeHistoricalTeamStatisticsProviderForBacktest(asOfDates);
        var fakeSeasonService = new FakeSeasonServiceForBacktest();
        var fakeEnumerator = new FakeHistoricalMatchEnumeratorForBacktest(
            CreateMatch(1, 100, 200, 1748736000, "H"));
        var fakeProbEngine = new FakeProbabilityEngine();

        var sut = new BacktestingService(
            fakeSeasonService, fakeEnumerator, fakeContextStats,
            fakeHistoricalStats, fakeProbEngine);

        await sut.RunAsync(
            TournamentId,
            new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc),
            1.58,
            new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc));

        // All asOfDateTime values should be identical (the match's start time)
        Assert.All(asOfDates, d => Assert.Equal(asOfDates[0], d));
    }

    [Fact]
    public async Task RunAsync_GammaIsIsolated_DoesNotReadAppSettings()
    {
        var usedGamma = 0.0;
        var fakeProbEngine = new FakeProbabilityEngine(probabilities: (0.5, 0.25, 0.25),
            onCalculate: (h, a) => { usedGamma = h; });

        var fakeContextStats = new FakeBacktestingTeamContextStatisticsService();
        var fakeHistoricalStats = new FakeHistoricalTeamStatisticsProviderForBacktest();
        var fakeSeasonService = new FakeSeasonServiceForBacktest();
        var fakeEnumerator = new FakeHistoricalMatchEnumeratorForBacktest(
            CreateMatch(1, 100, 200, 1748736000, "H"));

        var sut = new BacktestingService(
            fakeSeasonService, fakeEnumerator, fakeContextStats,
            fakeHistoricalStats, fakeProbEngine);

        var customGamma = 2.5;
        await sut.RunAsync(
            TournamentId,
            new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc),
            customGamma,
            new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc));

        // The custom gamma should be used, not 1.58 from appsettings
        // (the exact value depends on the calculation, but it should NOT be 1.58)
        Assert.NotEqual(1.58, usedGamma, 4);
    }

    [Fact]
    public async Task RunAsync_IncludeB2False_B2ProbabilitiesAreZero()
    {
        var fakeContextStats = new FakeBacktestingTeamContextStatisticsService();
        var fakeHistoricalStats = new FakeHistoricalTeamStatisticsProviderForBacktest();
        var fakeSeasonService = new FakeSeasonServiceForBacktest();
        var fakeEnumerator = new FakeHistoricalMatchEnumeratorForBacktest(
            CreateMatch(1, 100, 200, 1748736000, "H"));
        var fakeProbEngine = new FakeProbabilityEngine();

        var sut = new BacktestingService(
            fakeSeasonService, fakeEnumerator, fakeContextStats,
            fakeHistoricalStats, fakeProbEngine);

        var (summary, details) = await sut.RunAsync(
            TournamentId,
            new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc),
            1.58,
            new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc),
            includeB2: false);

        Assert.All(details, d =>
        {
            Assert.Equal(0, d.ModelB2_HomeWinProb);
            Assert.Equal(0, d.ModelB2_DrawProb);
            Assert.Equal(0, d.ModelB2_AwayWinProb);
        });
    }

    [Fact]
    public async Task RunAsync_SplitOnlyAndFallbackOnly_AreDeterminedByModelBCalculationMethod()
    {
        var matchSplit = CreateMatch(1, 100, 200, 1748736000, "H"); // 2025-06-01
        var matchFallback = CreateMatch(2, 300, 400, 1751328000, "D"); // 2025-07-01

        // First call returns split data (enough matches), subsequent calls return insufficient data
        var fakeContextStats = new FakeBacktestingTeamContextStatisticsService(
            sequentialResults: [
                new TeamContextStatistics(1.5, 1.0, 1.2, 0.9, 10, 10, 0), // match 1 home
                new TeamContextStatistics(1.3, 1.1, 1.0, 0.8, 10, 10, 0), // match 1 away
                new TeamContextStatistics(1.5, 1.0, 1.2, 0.9, 3, 3, 0),  // match 2 home (insufficient)
                new TeamContextStatistics(1.3, 1.1, 1.0, 0.8, 3, 3, 0),  // match 2 away (insufficient)
            ]);

        var fakeHistoricalStats = new FakeHistoricalTeamStatisticsProviderForBacktest();
        var fakeSeasonService = new FakeSeasonServiceForBacktest();
        var fakeEnumerator = new FakeHistoricalMatchEnumeratorForBacktest(matchSplit, matchFallback);
        var fakeProbEngine = new FakeProbabilityEngine();

        var sut = new BacktestingService(
            fakeSeasonService, fakeEnumerator, fakeContextStats,
            fakeHistoricalStats, fakeProbEngine);

        var (summary, details) = await sut.RunAsync(
            TournamentId,
            new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc),
            1.58,
            new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc));

        Assert.Equal(2, summary.TotalMatches);
        Assert.Equal("HomeAwaySplit", details[0].CalculationMethod);
        Assert.Equal("SeasonAverageWithGamma", details[1].CalculationMethod);
        Assert.Equal(1, summary.ModelA.SplitOnly.MatchCount);
        Assert.Equal(1, summary.ModelA.FallbackOnly.MatchCount);
        Assert.Equal(1, summary.ModelB1.SplitOnly.MatchCount);
        Assert.Equal(1, summary.ModelB1.FallbackOnly.MatchCount);
    }

    [Fact]
    public async Task RunAsync_NoRealSofaScoreCalls()
    {
        var fakeSofaScore = new FakeSofaScoreClientForBacktest();
        var fakeContextStats = new FakeBacktestingTeamContextStatisticsService();
        var fakeHistoricalStats = new FakeHistoricalTeamStatisticsProviderForBacktest();
        var fakeSeasonService = new FakeSeasonServiceForBacktest();
        var fakeEnumerator = new FakeHistoricalMatchEnumeratorForBacktest(
            CreateMatch(1, 100, 200, 1748736000, "H"));
        var fakeProbEngine = new FakeProbabilityEngine();

        // If any real SofaScore call is made, this fake will throw
        var sut = new BacktestingService(
            fakeSeasonService, fakeEnumerator, fakeContextStats,
            fakeHistoricalStats, fakeProbEngine);

        var (summary, details) = await sut.RunAsync(
            TournamentId,
            new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc),
            1.58,
            new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc));

        Assert.Single(details);
        Assert.Equal(1, summary.TotalMatches);
    }

    [Fact]
    public async Task RunAsync_FiltersMatchesByDateRange()
    {
        var matchInRange = CreateMatch(1, 100, 200, 1748736000, "H"); // 2025-06-01
        var matchOutOfRange = CreateMatch(2, 300, 400, 1800000000, "D"); // 2027-01-15

        var fakeContextStats = new FakeBacktestingTeamContextStatisticsService();
        var fakeHistoricalStats = new FakeHistoricalTeamStatisticsProviderForBacktest();
        var fakeSeasonService = new FakeSeasonServiceForBacktest();
        var fakeEnumerator = new FakeHistoricalMatchEnumeratorForBacktest(matchInRange, matchOutOfRange);
        var fakeProbEngine = new FakeProbabilityEngine();

        var sut = new BacktestingService(
            fakeSeasonService, fakeEnumerator, fakeContextStats,
            fakeHistoricalStats, fakeProbEngine);

        // Only include matches between 2025-01-01 and 2025-12-31
        var (summary, details) = await sut.RunAsync(
            TournamentId,
            new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc),
            1.58,
            new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc));

        Assert.Single(details);
        Assert.Equal(1, details[0].MatchId);
    }

    private static HistoricalMatch CreateMatch(
        int id, int homeTeamId, int awayTeamId, long startTimestamp, string result)
    {
        var (homeGoals, awayGoals) = result switch
        {
            "H" => (2, 1),
            "A" => (1, 2),
            _ => (1, 1)
        };

        return new HistoricalMatch(
            new FootballMatchEvent
            {
                Id = id,
                HomeTeam = new MatchTeam { Id = homeTeamId, Name = $"Team {homeTeamId}" },
                AwayTeam = new MatchTeam { Id = awayTeamId, Name = $"Team {awayTeamId}" },
                HomeScore = new MatchScore { Current = homeGoals },
                AwayScore = new MatchScore { Current = awayGoals },
                Status = new MatchStatus { Type = "finished" },
                StartTimestamp = (int)startTimestamp
            }, 2024, "Apertura");
    }
}

#region Fakes for BacktestingService Tests

internal class FakeSeasonServiceForBacktest : ISeasonService
{
    public Task<int> GetCurrentSeasonAsync(int tournamentId) => Task.FromResult(2024);
    public Task<List<int>> GetRecentSeasonIdsAsync(int tournamentId, int count) =>
        Task.FromResult(new List<int> { 2024 });
    public Task<List<int>> GetRecentSeasonIdsAsOfAsync(int tournamentId, int count, DateTime asOfDateTime) =>
        Task.FromResult(new List<int> { 2024 });
    public Task<string> GetSeasonNameAsync(int tournamentId, int seasonId) =>
        Task.FromResult($"Season {seasonId}");
}

internal class FakeHistoricalMatchEnumeratorForBacktest : IHistoricalMatchEnumerator
{
    private readonly IReadOnlyList<HistoricalMatch> _matches;

    public FakeHistoricalMatchEnumeratorForBacktest(params HistoricalMatch[] matches)
    {
        _matches = matches;
    }

    public Task<IReadOnlyList<HistoricalMatch>> GetFinishedMatchesAsync(
        int tournamentId, IReadOnlyList<int> seasonIds,
        int fromRound, int toRound, IReadOnlyList<string> prefixes) =>
        Task.FromResult<IReadOnlyList<HistoricalMatch>>(_matches);
}

internal class FakeBacktestingTeamContextStatisticsService : ITeamContextStatisticsService
{
    private readonly List<DateTime> _asOfDates;
    private readonly IReadOnlyList<TeamContextStatistics>? _sequentialResults;
    private int _callIndex;

    public FakeBacktestingTeamContextStatisticsService(
        IReadOnlyList<TeamContextStatistics>? sequentialResults = null,
        List<DateTime>? asOfDates = null)
    {
        _sequentialResults = sequentialResults;
        _asOfDates = asOfDates ?? new List<DateTime>();
    }

    public Task<TeamContextStatistics> CalculateAsync(
        int teamId, int tournamentId, DateTime asOfDateTime, int seasonLookback = 2)
    {
        _asOfDates.Add(asOfDateTime);

        if (_sequentialResults != null && _callIndex < _sequentialResults.Count)
        {
            return Task.FromResult(_sequentialResults[_callIndex++]);
        }

        return Task.FromResult(new TeamContextStatistics(
            AttackHome: 1.5, DefenseHome: 1.0,
            AttackAway: 1.2, DefenseAway: 0.9,
            HomeMatchesCount: 10, AwayMatchesCount: 10,
            SkippedMatchesCount: 0));
    }
}

internal class FakeHistoricalTeamStatisticsProviderForBacktest : IHistoricalTeamStatisticsProvider
{
    private readonly List<DateTime> _asOfDates;

    public FakeHistoricalTeamStatisticsProviderForBacktest(List<DateTime>? asOfDates = null)
    {
        _asOfDates = asOfDates ?? new List<DateTime>();
    }

    public Task<TeamStatistics> GetAsOfAsync(
        int teamId, int tournamentId, DateTime asOfDateTime, int seasonLookback = 2)
    {
        _asOfDates.Add(asOfDateTime);

        return Task.FromResult(new TeamStatistics
        {
            GoalsScored = 30,
            GoalsConceded = 20,
            Matches = 20
        });
    }
}

internal class FakeProbabilityEngine : IProbabilityEngine
{
    private readonly (double HomeWin, double Draw, double AwayWin) _probabilities;
    private readonly Action<double, double>? _onCalculate;

    public FakeProbabilityEngine(
        (double HomeWin, double Draw, double AwayWin)? probabilities = null,
        Action<double, double>? onCalculate = null)
    {
        _probabilities = probabilities ?? (0.4, 0.3, 0.3);
        _onCalculate = onCalculate;
    }

    public double PoissonProbability(double lambda, int x) =>
        Math.Exp(-lambda) * Math.Pow(lambda, x) / Factorial(x);

    public double GetOverUnderProbability(double lambdaTotal, double line, bool over) => 0.5;

    public MatchResultProbabilities GetMatchResultProbabilities(double lambdaHome, double lambdaAway, int maxGoals = 10)
    {
        _onCalculate?.Invoke(lambdaHome, lambdaAway);
        return new MatchResultProbabilities
        {
            HomeWin = _probabilities.HomeWin,
            Draw = _probabilities.Draw,
            AwayWin = _probabilities.AwayWin
        };
    }

    public double GetBttsYesProbability(double lambdaHome, double lambdaAway) => 0.5;

    private static double Factorial(int n) =>
        n <= 1 ? 1 : n * Factorial(n - 1);
}

internal class FakeSofaScoreClientForBacktest : ISofaScoreClient
{
    public Task<SofaScoreStatisticsResponse?> GetTeamStatisticsAsync(int teamId, int tournamentId, int seasonId) =>
        throw new InvalidOperationException("Real SofaScore call detected");

    public Task<List<Team>?> GetTeamsAsync(int tournamentId, int seasonId) =>
        throw new InvalidOperationException("Real SofaScore call detected");

    public Task<MatchEventsResponse?> GetMatchEventsByRoundAsync(int tournamentId, int seasonId, int round, string prefix) =>
        throw new InvalidOperationException("Real SofaScore call detected");
}

#endregion
