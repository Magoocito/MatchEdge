using MatchEdge.Domain.Odds;
using MatchEdge.Infrastructure.Data;
using MatchEdge.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MatchEdge.InfrastructureTests;

public class SqlHistoricalOddsServiceTests
{
    private MatchEdgeDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<MatchEdgeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new MatchEdgeDbContext(options);
    }

    private static HistoricalOdds CreateTestOdds(int matchId = 1, string homeTeam = "Alianza Lima", string awayTeam = "Sporting Cristal", DateTime? matchDate = null, int? tournamentId = null) => new()
    {
        MatchId = matchId,
        MatchDate = matchDate ?? new DateTime(2025, 6, 15),
        TournamentId = tournamentId ?? 1,
        Round = 10,
        HomeTeamId = 100,
        HomeTeamName = homeTeam,
        AwayTeamId = 200,
        AwayTeamName = awayTeam,
        HomeWinOdds = 2.10,
        DrawOdds = 3.25,
        AwayWinOdds = 3.40
    };

    [Fact]
    public void Load_SingleRecord_PersistsToDatabase()
    {
        using var context = CreateInMemoryContext();
        var service = new SqlHistoricalOddsService(context);

        service.Load(new[] { CreateTestOdds() });

        var result = service.GetAll();
        Assert.Single(result);
        Assert.Equal("Alianza Lima", result[0].HomeTeamName);
        Assert.Equal(2.10, result[0].HomeWinOdds);
    }

    [Fact]
    public void Load_MultipleRecords_PersistsAll()
    {
        using var context = CreateInMemoryContext();
        var service = new SqlHistoricalOddsService(context);

        var odds = Enumerable.Range(1, 10)
            .Select(i => CreateTestOdds(i, $"Team {i}", $"Team {i + 100}"))
            .ToList();

        service.Load(odds);

        Assert.Equal(10, service.GetAll().Count);
    }

    [Fact]
    public void Load_DuplicateRecord_UpdatesInsteadOfInserting()
    {
        using var context = CreateInMemoryContext();
        var service = new SqlHistoricalOddsService(context);

        service.Load(new[] { CreateTestOdds(matchId: 1) });
        Assert.Single(service.GetAll());

        var updated = CreateTestOdds(matchId: 1);
        updated.HomeWinOdds = 1.90;
        service.Load(new[] { updated });

        var result = service.GetAll();
        Assert.Single(result);
        Assert.Equal(1.90, result[0].HomeWinOdds);
    }

    [Fact]
    public void GetByDateRange_ReturnsOnlyMatchesInRange()
    {
        using var context = CreateInMemoryContext();
        var service = new SqlHistoricalOddsService(context);

        var odds = new[]
        {
            CreateTestOdds(1, matchDate: new DateTime(2025, 3, 1)),
            CreateTestOdds(2, matchDate: new DateTime(2025, 6, 15)),
            CreateTestOdds(3, matchDate: new DateTime(2025, 9, 30))
        };
        service.Load(odds);

        var result = service.GetByDateRange(new DateTime(2025, 5, 1), new DateTime(2025, 8, 1));
        Assert.Single(result);
        Assert.Equal(2, result[0].MatchId);
    }

    [Fact]
    public void GetByTournament_ReturnsOnlyMatchesForTournament()
    {
        using var context = CreateInMemoryContext();
        var service = new SqlHistoricalOddsService(context);

        var odds = new[]
        {
            CreateTestOdds(1, tournamentId: 1),
            CreateTestOdds(2, tournamentId: 1),
            CreateTestOdds(3, tournamentId: 2)
        };
        service.Load(odds);

        var result = service.GetByTournament(1);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetAll_EmptyDatabase_ReturnsEmptyList()
    {
        using var context = CreateInMemoryContext();
        var service = new SqlHistoricalOddsService(context);

        var result = service.GetAll();
        Assert.Empty(result);
    }
}
