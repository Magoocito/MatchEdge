using MatchEdge.Application.Clients;
using MatchEdge.Application.UseCases.Historical;
using MatchEdge.Domain.Matches;
using MatchEdge.Domain.Models;
using MatchEdge.Domain.Teams;

namespace MatchEdge.UnitTests;

public class HistoricalMatchEnumeratorTests
{
    private const int TournamentId = 406;

    [Fact]
    public async Task GetFinishedMatchesAsync_ReturnsOnlyFinishedMatches()
    {
        var finishedMatch = CreateMatch(1, "finished", homeScore: 2, awayScore: 1);
        var inProgressMatch = CreateMatch(2, "inprogress", homeScore: 1, awayScore: 0);
        var notStartedMatch = CreateMatch(3, "notstarted");

        var client = FakeSofaScoreClient.ForRound(TournamentId, 2024, 1, "Apertura",
            [finishedMatch, inProgressMatch, notStartedMatch]);

        var sut = new HistoricalMatchEnumerator(client);
        var result = await sut.GetFinishedMatchesAsync(
            TournamentId, [2024], 1, 1, ["Apertura"]);

        Assert.Single(result);
        Assert.Equal(1, result[0].Event.Id);
    }

    [Fact]
    public async Task GetFinishedMatchesAsync_ExcludesMatchesWithoutValidScore()
    {
        var validMatch = CreateMatch(1, "finished", homeScore: 2, awayScore: 1);
        var noHomeScore = CreateMatch(2, "finished", homeScore: null, awayScore: 1);
        var noAwayScore = CreateMatch(3, "finished", homeScore: 2, awayScore: null);

        var client = FakeSofaScoreClient.ForRound(TournamentId, 2024, 1, "Apertura",
            [validMatch, noHomeScore, noAwayScore]);

        var sut = new HistoricalMatchEnumerator(client);
        var result = await sut.GetFinishedMatchesAsync(
            TournamentId, [2024], 1, 1, ["Apertura"]);

        Assert.Single(result);
        Assert.Equal(1, result[0].Event.Id);
    }

    [Fact]
    public async Task GetFinishedMatchesAsync_ExcludesDuplicateMatchIds()
    {
        var match = CreateMatch(1, "finished", homeScore: 2, awayScore: 1);

        var client = FakeSofaScoreClient.ForRound(TournamentId, 2024, 1, "Apertura", [match]);
        // Same match returned in round 2 as well
        var client2 = FakeSofaScoreClient.ForMultipleRounds(TournamentId, 2024,
            [(1, "Apertura", [match]), (2, "Apertura", [match])]);

        var sut = new HistoricalMatchEnumerator(client2);
        var result = await sut.GetFinishedMatchesAsync(
            TournamentId, [2024], 1, 2, ["Apertura"]);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetFinishedMatchesAsync_IncludesSeasonAndPrefixInfo()
    {
        var match = CreateMatch(1, "finished", homeScore: 2, awayScore: 1);

        var client = FakeSofaScoreClient.ForRound(TournamentId, 2024, 1, "Clausura", [match]);
        var sut = new HistoricalMatchEnumerator(client);

        var result = await sut.GetFinishedMatchesAsync(
            TournamentId, [2024], 1, 1, ["Clausura"]);

        Assert.Single(result);
        Assert.Equal(2024, result[0].SeasonId);
        Assert.Equal("Clausura", result[0].Prefix);
    }

    [Fact]
    public async Task GetFinishedMatchesAsync_MultipleSeasonsAndPrefixes()
    {
        var match2024A = CreateMatch(1, "finished", homeScore: 2, awayScore: 1);
        var match2024C = CreateMatch(2, "finished", homeScore: 3, awayScore: 0);
        var match2023A = CreateMatch(3, "finished", homeScore: 1, awayScore: 1);

        var client = FakeSofaScoreClient.ForMultipleRounds(TournamentId, 2024,
            [(1, "Apertura", [match2024A]), (1, "Clausura", [match2024C])]);
        var client2023 = FakeSofaScoreClient.ForMultipleRounds(TournamentId, 2023,
            [(1, "Apertura", [match2023A])]);

        var combinedClient = new CombinedSofaScoreClient([client, client2023]);
        var sut = new HistoricalMatchEnumerator(combinedClient);

        var result = await sut.GetFinishedMatchesAsync(
            TournamentId, [2024, 2023], 1, 1, ["Apertura", "Clausura"]);

        Assert.Equal(3, result.Count);
    }

    private static FootballMatchEvent CreateMatch(
        int id, string statusType, int? homeScore = null, int? awayScore = null)
    {
        return new FootballMatchEvent
        {
            Id = id,
            Status = new MatchStatus { Type = statusType },
            HomeScore = new MatchScore { Current = homeScore },
            AwayScore = new MatchScore { Current = awayScore },
            RoundInfo = new RoundInfo { Round = 1 }
        };
    }
}

internal class FakeSofaScoreClient : ISofaScoreClient
{
    private readonly Dictionary<(int SeasonId, int Round, string Prefix), List<FootballMatchEvent>> _events;

    private FakeSofaScoreClient(
        Dictionary<(int SeasonId, int Round, string Prefix), List<FootballMatchEvent>> events)
    {
        _events = events;
    }

    public static FakeSofaScoreClient ForRound(
        int tournamentId, int seasonId, int round, string prefix,
        List<FootballMatchEvent> events)
    {
        return new FakeSofaScoreClient(
            new Dictionary<(int, int, string), List<FootballMatchEvent>>
            {
                [(seasonId, round, prefix)] = events
            });
    }

    public static FakeSofaScoreClient ForMultipleRounds(
        int tournamentId, int seasonId,
        List<(int Round, string Prefix, List<FootballMatchEvent> Events)> rounds)
    {
        var dict = new Dictionary<(int, int, string), List<FootballMatchEvent>>();
        foreach (var (round, prefix, events) in rounds)
        {
            dict[(seasonId, round, prefix)] = events;
        }
        return new FakeSofaScoreClient(dict);
    }

    public Task<SofaScoreStatisticsResponse?> GetTeamStatisticsAsync(
        int teamId, int tournamentId, int seasonId) =>
        Task.FromResult<SofaScoreStatisticsResponse?>(null);

    public Task<List<Team>?> GetTeamsAsync(int tournamentId, int seasonId) =>
        Task.FromResult<List<Team>?>(null);

    public Task<MatchEventsResponse?> GetMatchEventsByRoundAsync(
        int tournamentId, int seasonId, int round, string prefix)
    {
        if (_events.TryGetValue((seasonId, round, prefix), out var events))
        {
            return Task.FromResult<MatchEventsResponse?>(
                new MatchEventsResponse { Events = events });
        }
        return Task.FromResult<MatchEventsResponse?>(
            new MatchEventsResponse { Events = [] });
    }
}

internal class CombinedSofaScoreClient : ISofaScoreClient
{
    private readonly IReadOnlyList<ISofaScoreClient> _clients;

    public CombinedSofaScoreClient(IReadOnlyList<ISofaScoreClient> clients)
    {
        _clients = clients;
    }

    public async Task<SofaScoreStatisticsResponse?> GetTeamStatisticsAsync(
        int teamId, int tournamentId, int seasonId) => null;

    public async Task<List<Team>?> GetTeamsAsync(int tournamentId, int seasonId) => null;

    public async Task<MatchEventsResponse?> GetMatchEventsByRoundAsync(
        int tournamentId, int seasonId, int round, string prefix)
    {
        foreach (var client in _clients)
        {
            var result = await client.GetMatchEventsByRoundAsync(
                tournamentId, seasonId, round, prefix);
            if (result?.Events is { Count: > 0 })
                return result;
        }
        return new MatchEventsResponse { Events = [] };
    }
}
