using MatchEdge.Infrastructure.Clients;
using MatchEdge.Infrastructure.Configuration;
using MatchEdge.UnitTests.Fakes;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace MatchEdge.UnitTests;

public class SofaScoreClientCacheTests
{
    private const string EventsJson = """
    {
        "hasNextPage": false,
        "events": [
            {
                "id": 123456,
                "slug": "team-a-vs-team-b",
                "homeTeam": { "id": 1, "name": "Alianza Lima", "shortName": "ALI" },
                "awayTeam": { "id": 2, "name": "Sporting Cristal", "shortName": "SCR" },
                "homeScore": { "current": 2, "display": 2 },
                "awayScore": { "current": 1, "display": 1 },
                "status": { "code": 100, "description": "Finished", "type": "finished" },
                "roundInfo": { "round": 5 },
                "startTimestamp": 1700000000
            }
        ]
    }
    """;

    [Fact]
    public async Task GetMatchEventsByRoundAsync_TwoCallsSameParams_OnlyOneHttpInvocation()
    {
        var fake = new FakeHttpRequestExecutor(_ => EventsJson);
        var client = CreateClient(fake);

        await client.GetMatchEventsByRoundAsync(406, 1, 5, "Apertura");
        await client.GetMatchEventsByRoundAsync(406, 1, 5, "Apertura");

        Assert.Equal(1, fake.CallCount);
    }

    [Fact]
    public async Task GetMatchEventsByRoundAsyncHttpException_DoesNotCache()
    {
        var callCount = 0;
        var fake = new FakeHttpRequestExecutor(_ =>
        {
            callCount++;
            if (callCount == 1)
                throw new HttpRequestException("rate limited");
            return EventsJson;
        });

        var client = CreateClient(fake);

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            client.GetMatchEventsByRoundAsync(406, 1, 5, "Apertura"));

        var result = await client.GetMatchEventsByRoundAsync(406, 1, 5, "Apertura");

        Assert.NotNull(result);
        Assert.Equal(2, fake.CallCount);
    }

    [Fact]
    public async Task GetMatchEventsByRoundAsync_DifferentParams_CachesSeparately()
    {
        var fake = new FakeHttpRequestExecutor(_ => EventsJson);
        var client = CreateClient(fake);

        await client.GetMatchEventsByRoundAsync(406, 1, 5, "Apertura");
        await client.GetMatchEventsByRoundAsync(406, 1, 6, "Apertura");
        await client.GetMatchEventsByRoundAsync(406, 1, 5, "Apertura");

        Assert.Equal(2, fake.CallCount);
    }

    private static SofaScoreClient CreateClient(FakeHttpRequestExecutor fake)
    {
        var options = Options.Create(new SofaScoreOptions { BaseUrl = "https://fake/" });
        var cacheOptions = Options.Create(new SofaScoreCacheOptions());
        var cache = new MemoryCache(new MemoryCacheOptions());
        var resolver = new MatchCacheTtlResolver();

        return new SofaScoreClient(options, fake, cache, resolver, cacheOptions);
    }
}
