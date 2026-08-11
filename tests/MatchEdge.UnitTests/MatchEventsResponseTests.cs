using System.Text.Json;
using MatchEdge.Domain.Matches;

namespace MatchEdge.UnitTests;

public class MatchEventsResponseTests
{
    [Fact]
    public void Deserialize_WithReadOnlyList_PopulatesEventsCorrectly()
    {
        var json = """
        {
            "hasNextPage": false,
            "events": [
                {
                    "id": 123456,
                    "slug": "team-a-vs-team-b",
                    "homeTeam": { "id": 1, "name": "Alianza Lima", "shortName": "ALI" },
                    "awayTeam": { "id": 2, "name": "Sporting Cristal", "shortName": "SCR" },
                    "homeScore": { "current": 2, "display": 2, "period1": 1, "period2": 1 },
                    "awayScore": { "current": 1, "display": 1, "period1": 0, "period2": 1 },
                    "status": { "code": 100, "description": "Finished", "type": "finished" },
                    "roundInfo": { "round": 5 },
                    "startTimestamp": 1700000000
                },
                {
                    "id": 123457,
                    "slug": "team-c-vs-team-d",
                    "homeTeam": { "id": 3, "name": "Universitario", "shortName": "UNI" },
                    "awayTeam": { "id": 4, "name": "Melgar", "shortName": "MEL" },
                    "homeScore": { "current": 0, "display": 0, "period1": 0, "period2": 0 },
                    "awayScore": { "current": 0, "display": 0, "period1": 0, "period2": 0 },
                    "status": { "code": 0, "description": "Not started", "type": "notstarted" },
                    "roundInfo": { "round": 5 },
                    "startTimestamp": 1700100000
                }
            ]
        }
        """;

        var response = JsonSerializer.Deserialize<MatchEventsResponse>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(response);
        Assert.False(response!.HasNextPage);
        Assert.Equal(2, response.Events.Count);

        var first = response.Events[0];
        Assert.Equal(123456, first.Id);
        Assert.Equal("Alianza Lima", first.HomeTeam.Name);
        Assert.Equal("Sporting Cristal", first.AwayTeam.Name);
        Assert.Equal(2, first.HomeScore.Current);
        Assert.Equal(1, first.AwayScore.Current);
        Assert.Equal("finished", first.Status.Type);

        var second = response.Events[1];
        Assert.Equal(123457, second.Id);
        Assert.Equal("notstarted", second.Status.Type);
        Assert.Equal(0, second.HomeScore.Current);
    }

    [Fact]
    public void Deserialize_EmptyEvents_ReturnsEmptyReadOnlyList()
    {
        var json = """
        {
            "hasNextPage": false,
            "events": []
        }
        """;

        var response = JsonSerializer.Deserialize<MatchEventsResponse>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(response);
        Assert.NotNull(response!.Events);
        Assert.Empty(response.Events);
        Assert.IsAssignableFrom<IReadOnlyList<FootballMatchEvent>>(response.Events);
    }
}
