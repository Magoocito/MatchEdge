using MatchEdge.Application.UseCases.OddsImport;
using MatchEdge.Domain.Odds;
using Xunit;

namespace MatchEdge.UnitTests;

public class HistoricalOddsServiceTests
{
    private readonly HistoricalOddsService _service = new();

    [Fact]
    public void Load_And_GetAll_ReturnsLoadedOdds()
    {
        var odds = new List<HistoricalOdds>
        {
            new() { MatchId = 1, MatchDate = new DateTime(2025, 1, 15), TournamentId = 406, HomeWinOdds = 1.80, DrawOdds = 3.40, AwayWinOdds = 4.20 },
            new() { MatchId = 2, MatchDate = new DateTime(2025, 2, 20), TournamentId = 406, HomeWinOdds = 2.10, DrawOdds = 3.20, AwayWinOdds = 3.50 }
        };

        _service.Load(odds);

        var result = _service.GetAll();
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetByDateRange_ReturnsOnlyMatchesInRange()
    {
        var odds = new List<HistoricalOdds>
        {
            new() { MatchId = 1, MatchDate = new DateTime(2025, 1, 15), TournamentId = 406 },
            new() { MatchId = 2, MatchDate = new DateTime(2025, 3, 20), TournamentId = 406 },
            new() { MatchId = 3, MatchDate = new DateTime(2025, 6, 10), TournamentId = 406 }
        };

        _service.Load(odds);

        var result = _service.GetByDateRange(new DateTime(2025, 2, 1), new DateTime(2025, 5, 1));
        Assert.Single(result);
        Assert.Equal(2, result[0].MatchId);
    }

    [Fact]
    public void GetByTournament_ReturnsOnlyMatchesInTournament()
    {
        var odds = new List<HistoricalOdds>
        {
            new() { MatchId = 1, TournamentId = 406 },
            new() { MatchId = 2, TournamentId = 17 },
            new() { MatchId = 3, TournamentId = 406 }
        };

        _service.Load(odds);

        var result = _service.GetByTournament(406);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Load_ReplacesPreviousData()
    {
        _service.Load(new List<HistoricalOdds>
        {
            new() { MatchId = 1, TournamentId = 406 }
        });

        _service.Load(new List<HistoricalOdds>
        {
            new() { MatchId = 2, TournamentId = 17 },
            new() { MatchId = 3, TournamentId = 17 }
        });

        var result = _service.GetAll();
        Assert.Equal(2, result.Count);
        Assert.All(result, o => Assert.Equal(17, o.TournamentId));
    }

    [Fact]
    public void GetAll_WhenEmpty_ReturnsEmptyList()
    {
        var result = _service.GetAll();
        Assert.Empty(result);
    }
}
