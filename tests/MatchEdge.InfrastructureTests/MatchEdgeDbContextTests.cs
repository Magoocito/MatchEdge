using MatchEdge.Infrastructure.Data;
using MatchEdge.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MatchEdge.InfrastructureTests;

public class MatchEdgeDbContextTests
{
    private MatchEdgeDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<MatchEdgeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new MatchEdgeDbContext(options);
    }

    [Fact]
    public async Task HistoricalOdds_InsertAndRetrieve()
    {
        using var context = CreateInMemoryContext();
        var odds = new HistoricalOddsEntity
        {
            Source = "FootyStats",
            SourceMatchId = 12345,
            MatchDate = new DateTime(2025, 6, 15),
            TournamentId = 1,
            Round = 10,
            HomeTeamId = 100,
            HomeTeamName = "Alianza Lima",
            AwayTeamId = 200,
            AwayTeamName = "Sporting Cristal",
            HomeWinOdds = 2.10,
            DrawOdds = 3.25,
            AwayWinOdds = 3.40,
            CreatedAt = DateTime.UtcNow
        };
        context.HistoricalOdds.Add(odds);
        await context.SaveChangesAsync();

        var result = await context.HistoricalOdds.FirstOrDefaultAsync(x => x.SourceMatchId == 12345);
        Assert.NotNull(result);
        Assert.Equal("Alianza Lima", result.HomeTeamName);
        Assert.Equal(2.10, result.HomeWinOdds);
    }

    [Fact]
    public void HistoricalOdds_UniqueConstraint_DefinedInModel()
    {
        using var context = CreateInMemoryContext();
        var entityType = context.Model.FindEntityType(typeof(HistoricalOddsEntity));
        var index = entityType!.GetIndexes().First(i => i.Properties.Any(p => p.Name == "SourceMatchId"));
        Assert.True(index.IsUnique);
    }

    [Fact]
    public async Task TeamMapping_InsertAndRetrieve()
    {
        using var context = CreateInMemoryContext();
        context.TeamMappings.Add(new TeamMappingEntity
        {
            Source = "FootyStats",
            SourceTeamId = 100,
            SourceTeamName = "Alianza Lima",
            SofaScoreTeamId = 5001,
            SofaScoreTeamName = "Alianza Lima",
            Confidence = 0.95,
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var result = await context.TeamMappings.FirstOrDefaultAsync(x => x.SourceTeamId == 100);
        Assert.NotNull(result);
        Assert.Equal(5001, result.SofaScoreTeamId);
    }

    [Fact]
    public async Task MatchMapping_InsertAndRetrieve()
    {
        using var context = CreateInMemoryContext();
        context.MatchMappings.Add(new MatchMappingEntity
        {
            Source = "FootyStats",
            SourceMatchId = 200,
            SofaScoreEventId = 9001,
            MatchDate = new DateTime(2025, 7, 1),
            MatchConfidence = 0.90,
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var result = await context.MatchMappings.FirstOrDefaultAsync(x => x.SourceMatchId == 200);
        Assert.NotNull(result);
        Assert.Equal(9001, result.SofaScoreEventId);
    }

    [Fact]
    public async Task HistoricalOdds_BulkInsert()
    {
        using var context = CreateInMemoryContext();
        var oddsList = Enumerable.Range(1, 50).Select(i => new HistoricalOddsEntity
        {
            Source = "FootyStats",
            SourceMatchId = i,
            MatchDate = new DateTime(2025, 1, 1).AddDays(i),
            TournamentId = 1, Round = i % 30 + 1,
            HomeTeamId = 100 + i, HomeTeamName = $"Team {100 + i}",
            AwayTeamId = 200 + i, AwayTeamName = $"Team {200 + i}",
            HomeWinOdds = 2.0, DrawOdds = 3.0, AwayWinOdds = 4.0,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        context.HistoricalOdds.AddRange(oddsList);
        await context.SaveChangesAsync();

        Assert.Equal(50, await context.HistoricalOdds.CountAsync());
    }
}
