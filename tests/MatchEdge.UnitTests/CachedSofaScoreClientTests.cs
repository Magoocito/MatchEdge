using MatchEdge.Infrastructure.Clients;
using MatchEdge.Infrastructure.Configuration;
using MatchEdge.Domain.Matches;
using MatchEdge.Domain.Models;
using MatchEdge.UnitTests.Fakes;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace MatchEdge.UnitTests;

public class CachedSofaScoreClientTests
{
    private static readonly MatchEventsResponse FinishedEvents = new()
    {
        HasNextPage = false,
        Events = new List<FootballMatchEvent>
        {
            new()
            {
                Id = 123456,
                Status = new MatchStatus { Code = 100, Description = "Finished", Type = "finished" }
            }
        }
    };

    private static readonly SofaScoreStatisticsResponse StatsResponse = new()
    {
        Statistics = new TeamStatistics
        {
            GoalsScored = 38,
            GoalsConceded = 13,
            Matches = 22
        }
    };

    [Fact]
    public async Task GetMatchEventsByRoundAsync_TwoCallsSameParams_OnlyOneInnerInvocation()
    {
        var inner = new StubSofaScoreClient((_, _, _, _) => FinishedEvents);
        var cached = CreateClient(inner);

        await cached.GetMatchEventsByRoundAsync(406, 57741, 5, "Apertura");
        await cached.GetMatchEventsByRoundAsync(406, 57741, 5, "Apertura");

        Assert.Equal(1, inner.CallCount);
    }

    [Fact]
    public async Task GetMatchEventsByRoundAsync_DifferentRound_CachesSeparately()
    {
        var inner = new StubSofaScoreClient((_, _, _, _) => FinishedEvents);
        var cached = CreateClient(inner);

        await cached.GetMatchEventsByRoundAsync(406, 57741, 5, "Apertura");
        await cached.GetMatchEventsByRoundAsync(406, 57741, 6, "Apertura");
        await cached.GetMatchEventsByRoundAsync(406, 57741, 5, "Apertura");

        Assert.Equal(2, inner.CallCount);
    }

    [Fact]
    public async Task GetMatchEventsByRoundAsync_DifferentSeasonId_CachesSeparately()
    {
        var inner = new StubSofaScoreClient((_, _, _, _) => FinishedEvents);
        var cached = CreateClient(inner);

        await cached.GetMatchEventsByRoundAsync(406, 57741, 5, "Apertura");
        await cached.GetMatchEventsByRoundAsync(406, 70962, 5, "Apertura");

        Assert.Equal(2, inner.CallCount);
    }

    [Fact]
    public async Task GetMatchEventsByRoundAsync_NullResult_DoesNotCache()
    {
        var callCount = 0;
        var inner = new StubSofaScoreClient((_, _, _, _) =>
        {
            callCount++;
            return callCount == 1 ? null : FinishedEvents;
        });
        var cached = CreateClient(inner);

        var first = await cached.GetMatchEventsByRoundAsync(406, 57741, 5, "Apertura");
        var second = await cached.GetMatchEventsByRoundAsync(406, 57741, 5, "Apertura");

        Assert.Null(first);
        Assert.NotNull(second);
        Assert.Equal(2, inner.CallCount);
    }

    [Fact]
    public async Task GetTeamStatisticsAsync_TwoCallsSameParams_OnlyOneInnerInvocation()
    {
        var inner = new StubSofaScoreClient(
            (_, _, _, _) => null,
            (_, _, _) => StatsResponse);
        var cached = CreateClient(inner);

        await cached.GetTeamStatisticsAsync(2311, 406, 57741);
        await cached.GetTeamStatisticsAsync(2311, 406, 57741);

        Assert.Equal(1, inner.CallCount);
    }

    [Fact]
    public async Task GetTeamStatisticsAsync_DifferentTeamId_CachesSeparately()
    {
        var inner = new StubSofaScoreClient(
            (_, _, _, _) => null,
            (_, _, _) => StatsResponse);
        var cached = CreateClient(inner);

        await cached.GetTeamStatisticsAsync(2311, 406, 57741);
        await cached.GetTeamStatisticsAsync(2312, 406, 57741);

        Assert.Equal(2, inner.CallCount);
    }

    [Fact]
    public async Task GetTeamStatisticsAsync_NullResult_DoesNotCache()
    {
        var callCount = 0;
        var inner = new StubSofaScoreClient(
            (_, _, _, _) => null,
            (_, _, _) =>
            {
                callCount++;
                return callCount == 1 ? null : StatsResponse;
            });
        var cached = CreateClient(inner);

        var first = await cached.GetTeamStatisticsAsync(2311, 406, 57741);
        var second = await cached.GetTeamStatisticsAsync(2311, 406, 57741);

        Assert.Null(first);
        Assert.NotNull(second);
        Assert.Equal(2, inner.CallCount);
    }

    private static CachedSofaScoreClient CreateClient(StubSofaScoreClient inner)
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var resolver = new MatchCacheTtlResolver();
        var cacheOptions = Options.Create(new SofaScoreCacheOptions());
        var logger = NullLogger<CachedSofaScoreClient>.Instance;

        return new CachedSofaScoreClient(inner, cache, resolver, cacheOptions, logger);
    }
}
