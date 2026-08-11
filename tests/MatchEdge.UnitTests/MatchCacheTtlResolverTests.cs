using MatchEdge.Domain.Matches;
using MatchEdge.Infrastructure.Clients;
using MatchEdge.Infrastructure.Configuration;

namespace MatchEdge.UnitTests;

public class MatchCacheTtlResolverTests
{
    private readonly MatchCacheTtlResolver _resolver = new();
    private readonly SofaScoreCacheOptions _options = new();

    [Fact]
    public void Resolve_NullResponse_ReturnsUpcomingTtl()
    {
        var ttl = _resolver.Resolve(null, _options);

        Assert.Equal(TimeSpan.FromHours(1), ttl);
    }

    [Fact]
    public void Resolve_EmptyEvents_ReturnsUpcomingTtl()
    {
        var response = new MatchEventsResponse { Events = [] };

        var ttl = _resolver.Resolve(response, _options);

        Assert.Equal(TimeSpan.FromHours(1), ttl);
    }

    [Fact]
    public void Resolve_AllFinished_ReturnsFinishedTtl()
    {
        var response = new MatchEventsResponse
        {
            Events = new List<FootballMatchEvent>
            {
                new() { Status = new MatchStatus { Type = "finished" } },
                new() { Status = new MatchStatus { Type = "finished" } }
            }
        };

        var ttl = _resolver.Resolve(response, _options);

        Assert.Equal(TimeSpan.FromDays(30), ttl);
    }

    [Fact]
    public void Resolve_AllNotStarted_ReturnsUpcomingTtl()
    {
        var response = new MatchEventsResponse
        {
            Events = new List<FootballMatchEvent>
            {
                new() { Status = new MatchStatus { Type = "notstarted" } },
                new() { Status = new MatchStatus { Type = "notstarted" } }
            }
        };

        var ttl = _resolver.Resolve(response, _options);

        Assert.Equal(TimeSpan.FromHours(1), ttl);
    }

    [Fact]
    public void Resolve_MixedFinishedAndNotStarted_ReturnsUpcomingTtl()
    {
        var response = new MatchEventsResponse
        {
            Events = new List<FootballMatchEvent>
            {
                new() { Status = new MatchStatus { Type = "finished" } },
                new() { Status = new MatchStatus { Type = "finished" } },
                new() { Status = new MatchStatus { Type = "notstarted" } }
            }
        };

        var ttl = _resolver.Resolve(response, _options);

        Assert.Equal(TimeSpan.FromHours(1), ttl);
    }

    [Fact]
    public void Resolve_LiveMatches_ReturnsLiveTtl()
    {
        var response = new MatchEventsResponse
        {
            Events = new List<FootballMatchEvent>
            {
                new() { Status = new MatchStatus { Type = "inprogress" } },
                new() { Status = new MatchStatus { Type = "inprogress" } }
            }
        };

        var ttl = _resolver.Resolve(response, _options);

        Assert.Equal(TimeSpan.FromMinutes(2), ttl);
    }
}
