using MatchEdge.Application.Clients;
using MatchEdge.Domain.Matches;
using MatchEdge.Domain.Models;
using MatchEdge.Domain.Teams;

namespace MatchEdge.UnitTests.Fakes;

public class StubSofaScoreClient : ISofaScoreClient
{
    private readonly Func<int, int, int, string, MatchEventsResponse?> _eventsFunc;
    private readonly Func<int, int, int, SofaScoreStatisticsResponse?>? _statsFunc;

    public int CallCount { get; private set; }

    public StubSofaScoreClient(
        Func<int, int, int, string, MatchEventsResponse?> eventsFunc,
        Func<int, int, int, SofaScoreStatisticsResponse?>? statsFunc = null)
    {
        _eventsFunc = eventsFunc;
        _statsFunc = statsFunc;
    }

    public Task<MatchEventsResponse?> GetMatchEventsByRoundAsync(
        int tournamentId, int seasonId, int round, string prefix)
    {
        CallCount++;
        return Task.FromResult(_eventsFunc(tournamentId, seasonId, round, prefix));
    }

    public Task<SofaScoreStatisticsResponse?> GetTeamStatisticsAsync(
        int teamId, int tournamentId, int seasonId)
    {
        CallCount++;
        if (_statsFunc != null)
            return Task.FromResult(_statsFunc(teamId, tournamentId, seasonId));
        return Task.FromResult<SofaScoreStatisticsResponse?>(null);
    }

    public Task<List<Team>?> GetTeamsAsync(int tournamentId, int seasonId)
    {
        CallCount++;
        return Task.FromResult<List<Team>?>(null);
    }
}
