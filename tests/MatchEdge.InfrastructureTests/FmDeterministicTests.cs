using MatchEdge.Infrastructure.Clients;
using MatchEdge.Infrastructure.Services;
using Xunit;

namespace MatchEdge.InfrastructureTests;

public class FmDeterministicTests
{
    private const string PlayerJson = """
    {
      "data": [
        {
          "shortName": "S. Lukic",
          "market": "fouls_committed",
          "line": "0.5",
          "direction": "over",
          "bestCount": 10,
          "bestTotal": 10,
          "percentage": 100,
          "score": "0.5757",
          "history": [ { "h": true, "t": "2026-03-31T16:00:00.000Z", "v": 1 } ]
        }
      ],
      "pagination": { "count": 1 }
    }
    """;

    private const string TeamJson = """
    {
      "data": [
        {
          "name": "Serbia",
          "market": "total_corners",
          "line": "5.5",
          "direction": "over",
          "bestCount": 10,
          "bestTotal": 10,
          "oppCount": 8,
          "oppTotal": 10,
          "oppHitRate": "80.0",
          "history": [ { "h": true, "t": "2026-01-01T00:00:00.000Z", "va": 9.3 } ]
        }
      ],
      "pagination": { "count": 1 }
    }
    """;

    [Fact]
    public void Parser_Player_ExtractsHitsSampleAndDerivedRate()
    {
        var signals = FmTrendsJsonParser.Parse(PlayerJson, "player");

        var s = Assert.Single(signals);
        Assert.Equal("player", s.SubjectType);
        Assert.Equal("S. Lukic", s.SubjectName);
        Assert.Equal("fouls_committed", s.Market);
        Assert.Equal(10, s.Hits);
        Assert.Equal(10, s.SampleSize);
        Assert.Equal(1.0, s.ObservedHitRate);
        Assert.Equal(0.5757, s.ConfidenceScore);
        Assert.NotNull(s.RecentValuesJson);
        Assert.Null(s.OppHits);
    }

    [Fact]
    public void Parser_Team_ExtractsOpponentCounts()
    {
        var signals = FmTrendsJsonParser.Parse(TeamJson, "team");

        var s = Assert.Single(signals);
        Assert.Equal("team", s.SubjectType);
        Assert.Equal("Serbia", s.SubjectName);
        Assert.Equal(8, s.OppHits);
        Assert.Equal(10, s.OppSampleSize);
        Assert.Equal(1.0, s.ObservedHitRate);
    }

    [Fact]
    public void Parser_EmptyData_ReturnsNoSignals()
    {
        var signals = FmTrendsJsonParser.Parse("""{"data":[],"pagination":{"count":0}}""", "team");
        Assert.Empty(signals);
    }

    [Fact]
    public void Parser_MissingSample_DoesNotInventRate()
    {
        var signals = FmTrendsJsonParser.Parse(
            """{"data":[{"shortName":"X","market":"m","bestCount":3,"bestTotal":0}]}""", "player");
        var s = Assert.Single(signals);
        Assert.Equal(3, s.Hits);
        Assert.Null(s.ObservedHitRate);
    }

    [Fact]
    public void Canary_DeviationAbove30Percent_IsSuspect()
    {
        var previous = new Dictionary<string, int> { ["corners"] = 10 };
        var current = new Dictionary<string, int> { ["corners"] = 6 };
        Assert.True(FmCanary.ExceedsDeviation(current, previous));
    }

    [Fact]
    public void Canary_DeviationWithin30Percent_IsOk()
    {
        var previous = new Dictionary<string, int> { ["corners"] = 10 };
        var current = new Dictionary<string, int> { ["corners"] = 8 };
        Assert.False(FmCanary.ExceedsDeviation(current, previous));
    }

    [Fact]
    public void Canary_MarketDisappeared_IsSuspect()
    {
        var previous = new Dictionary<string, int> { ["corners"] = 10 };
        var current = new Dictionary<string, int>();
        Assert.True(FmCanary.ExceedsDeviation(current, previous));
    }

    [Fact]
    public void SessionDetection_LoginUrl_IsDetected()
    {
        Assert.True(FmNavigator.LooksLikeLoginUrl("https://www.footymetrics.com/login"));
        Assert.False(FmNavigator.LooksLikeLoginUrl("https://www.footymetrics.com/fixtures/1-x"));
    }

    [Fact]
    public void SessionDetection_SignInMarkers_IsDetected()
    {
        Assert.True(FmNavigator.LooksLikeSignedOutText("Welcome back. Sign in to continue."));
        Assert.False(FmNavigator.LooksLikeSignedOutText("Serbia vs Greece lineups"));
    }

    [Fact]
    public void NavigationBudget_IsFortyPerRun()
    {
        Assert.Equal(40, FmNavigator.MaxNavigationsPerRun);
        Assert.True(FmNavigator.MaxNavigationsPerRun / 3 >= 13,
            "run topN cap must allow at least the 4 known fixtures (3 tabs each)");
    }

    [Fact]
    public void ScopesFromUrl_DerivesVenueAndCompetitionScope()
    {
        var (venue, comp) = FmFixtureSnapshotService.ScopesFromUrl(
            "https://www.footymetrics.com/api/front/trends/fixtures/1/teams?x=1&location=match&league_only=true");
        Assert.Equal("match", venue);
        Assert.Equal("same_league", comp);
    }

    [Fact]
    public void ScopesFromUrl_DefaultsToAll()
    {
        var (venue, comp) = FmFixtureSnapshotService.ScopesFromUrl(null);
        Assert.Equal("all", venue);
        Assert.Equal("all", comp);
    }
}
