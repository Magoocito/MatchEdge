using MatchEdge.Infrastructure.Data;
using MatchEdge.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MatchEdge.InfrastructureTests;

public class OddsMatchingServiceTests
{
    private MatchEdgeDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<MatchEdgeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new MatchEdgeDbContext(options);
    }

    [Theory]
    [InlineData("Alianza Atlético", "alianza atletico")]
    [InlineData("ADT", "adt")]
    [InlineData("Deportivo Garcilaso", "deportivo garcilaso")]
    [InlineData("Sporting Cristal", "sporting cristal")]
    [InlineData("  Alianza  Lima  ", "alianza lima")]
    public void NormalizeTeamName_RemovesAccentsAndWhitespace(string input, string expected)
    {
        using var context = CreateInMemoryContext();
        var service = new OddsMatchingService(context);

        var result = service.NormalizeTeamName(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void SaveTeamMapping_InsertsNewMapping()
    {
        using var context = CreateInMemoryContext();
        var service = new OddsMatchingService(context);

        service.SaveTeamMapping("FootyStats", 100, "Alianza Lima", 5001, "Alianza Lima");

        var mappings = service.GetAllTeamMappings();
        Assert.Single(mappings);
        Assert.Equal(5001, mappings[0].SofaScoreTeamId);
    }

    [Fact]
    public void SaveTeamMapping_DuplicateSource_UpdatesExisting()
    {
        using var context = CreateInMemoryContext();
        var service = new OddsMatchingService(context);

        service.SaveTeamMapping("FootyStats", 100, "Alianza Lima", 5001, "Alianza Lima");
        service.SaveTeamMapping("FootyStats", 100, "Alianza Lima", 5002, "Alianza Lima FC");

        var mappings = service.GetAllTeamMappings();
        Assert.Single(mappings);
        Assert.Equal(5002, mappings[0].SofaScoreTeamId);
    }

    [Fact]
    public void FindTeamMapping_BySourceName_ReturnsMatch()
    {
        using var context = CreateInMemoryContext();
        var service = new OddsMatchingService(context);

        service.SaveTeamMapping("FootyStats", 100, "Alianza Lima", 5001, "Alianza Lima");

        var result = service.FindTeamMapping("FootyStats", "Alianza Lima");
        Assert.NotNull(result);
        Assert.Equal(5001, result.SofaScoreTeamId);
    }

    [Fact]
    public void FindTeamMapping_BySofaScoreName_ReturnsMatch()
    {
        using var context = CreateInMemoryContext();
        var service = new OddsMatchingService(context);

        service.SaveTeamMapping("FootyStats", 100, "Alianza", 5001, "Alianza Lima");

        var result = service.FindTeamMapping("FootyStats", "Alianza Lima");
        Assert.NotNull(result);
        Assert.Equal(5001, result.SofaScoreTeamId);
    }

    [Fact]
    public void FindTeamMapping_NoMatch_ReturnsNull()
    {
        using var context = CreateInMemoryContext();
        var service = new OddsMatchingService(context);

        var result = service.FindTeamMapping("FootyStats", "Nonexistent Team");
        Assert.Null(result);
    }

    [Fact]
    public void DeleteTeamMapping_RemovesMapping()
    {
        using var context = CreateInMemoryContext();
        var service = new OddsMatchingService(context);

        service.SaveTeamMapping("FootyStats", 100, "Alianza Lima", 5001, "Alianza Lima");
        var mappings = service.GetAllTeamMappings();
        service.DeleteTeamMapping(mappings[0].Id);

        Assert.Empty(service.GetAllTeamMappings());
    }

    [Fact]
    public void SaveMatchMapping_InsertsNewMapping()
    {
        using var context = CreateInMemoryContext();
        var service = new OddsMatchingService(context);

        service.SaveMatchMapping("FootyStats", 12345, 9001, new DateTime(2025, 6, 15));

        var mappings = service.GetAllMatchMappings();
        Assert.Single(mappings);
        Assert.Equal(9001, mappings[0].SofaScoreEventId);
    }

    [Fact]
    public void SaveMatchMapping_DuplicateSource_UpdatesExisting()
    {
        using var context = CreateInMemoryContext();
        var service = new OddsMatchingService(context);

        service.SaveMatchMapping("FootyStats", 12345, 9001, new DateTime(2025, 6, 15));
        service.SaveMatchMapping("FootyStats", 12345, 9002, new DateTime(2025, 6, 15));

        var mappings = service.GetAllMatchMappings();
        Assert.Single(mappings);
        Assert.Equal(9002, mappings[0].SofaScoreEventId);
    }

    [Fact]
    public void FindMatchMapping_ReturnsMatch()
    {
        using var context = CreateInMemoryContext();
        var service = new OddsMatchingService(context);

        service.SaveMatchMapping("FootyStats", 12345, 9001, new DateTime(2025, 6, 15));

        var result = service.FindMatchMapping("FootyStats", 12345);
        Assert.NotNull(result);
        Assert.Equal(9001, result.SofaScoreEventId);
    }

    [Fact]
    public void DeleteMatchMapping_RemovesMapping()
    {
        using var context = CreateInMemoryContext();
        var service = new OddsMatchingService(context);

        service.SaveMatchMapping("FootyStats", 12345, 9001, new DateTime(2025, 6, 15));
        var mappings = service.GetAllMatchMappings();
        service.DeleteMatchMapping(mappings[0].Id);

        Assert.Empty(service.GetAllMatchMappings());
    }
}
