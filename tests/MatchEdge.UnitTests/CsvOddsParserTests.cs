using MatchEdge.Application.UseCases.OddsImport;
using Xunit;

namespace MatchEdge.UnitTests;

public class CsvOddsParserTests
{
    private readonly CsvOddsParser _parser = new();

    [Fact]
    public void Parse_ValidCsv_ReturnsOdds()
    {
        var csv = @"MatchDate,TournamentId,Round,MatchId,HomeTeamId,HomeTeamName,AwayTeamId,AwayTeamName,HomeWinOdds,DrawOdds,AwayWinOdds
2025-01-15,406,1,12345,1001,Aliquanta,1002,Sport Boys,1.80,3.40,4.20
2025-01-16,406,1,12346,1003,Cienciano,1004,Deportivo Binacional,2.10,3.20,3.50";

        var result = _parser.Parse(csv);

        Assert.Equal(2, result.Count);
        Assert.Equal(12345, result[0].MatchId);
        Assert.Equal(new DateTime(2025, 1, 15), result[0].MatchDate);
        Assert.Equal(1001, result[0].HomeTeamId);
        Assert.Equal("Aliquanta", result[0].HomeTeamName);
        Assert.Equal(1002, result[0].AwayTeamId);
        Assert.Equal("Sport Boys", result[0].AwayTeamName);
        Assert.Equal(1.80, result[0].HomeWinOdds);
        Assert.Equal(3.40, result[0].DrawOdds);
        Assert.Equal(4.20, result[0].AwayWinOdds);
        Assert.Equal(406, result[0].TournamentId);
        Assert.Equal(1, result[0].Round);
    }

    [Fact]
    public void Parse_AliasHeaders_Works()
    {
        var csv = @"Date,Tournament,Round,MatchId,HomeTeamId,Home,AwayTeamId,Away,1,X,2
2025-03-01,406,1,500,1001,Aliquanta,1002,Sport Boys,1.80,3.40,4.20";

        var result = _parser.Parse(csv);

        Assert.Single(result);
        Assert.Equal(new DateTime(2025, 3, 1), result[0].MatchDate);
        Assert.Equal(1.80, result[0].HomeWinOdds);
        Assert.Equal(3.40, result[0].DrawOdds);
        Assert.Equal(4.20, result[0].AwayWinOdds);
    }

    [Fact]
    public void Parse_InvalidOddsLine_SkipsLine()
    {
        var csv = @"MatchDate,TournamentId,HomeWinOdds,DrawOdds,AwayWinOdds
2025-01-15,406,1.80,3.40,4.20
2025-01-16,406,0,0,0
2025-01-17,406,2.10,3.20,3.50";

        var result = _parser.Parse(csv);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Parse_EmptyCsv_ReturnsEmpty()
    {
        var result = _parser.Parse("");
        Assert.Empty(result);
    }

    [Fact]
    public void Parse_HeaderOnly_ReturnsEmpty()
    {
        var csv = @"MatchDate,TournamentId,HomeWinOdds,DrawOdds,AwayWinOdds";
        var result = _parser.Parse(csv);
        Assert.Empty(result);
    }

    [Fact]
    public void Parse_CommaInQuotes_DoesNotSplit()
    {
        var csv = @"MatchDate,HomeTeamName,HomeWinOdds,DrawOdds,AwayWinOdds
2025-01-15,""Sport Boys, SAC"",1.80,3.40,4.20";

        var result = _parser.Parse(csv);

        Assert.Single(result);
        Assert.Equal("Sport Boys, SAC", result[0].HomeTeamName);
    }

    [Fact]
    public void Parse_ImpliedProbabilities_AreCalculated()
    {
        var csv = @"MatchDate,HomeWinOdds,DrawOdds,AwayWinOdds
2025-01-15,2.00,3.00,6.00";

        var result = _parser.Parse(csv);

        Assert.Single(result);
        Assert.Equal(0.50, result[0].ImpliedHomeWinProbability);
        Assert.Equal(1.0 / 3.0, result[0].ImpliedDrawProbability, 10);
        Assert.Equal(1.0 / 6.0, result[0].ImpliedAwayWinProbability, 10);
    }

    [Fact]
    public void Parse_WithTrailingComma_Parses()
    {
        var csv = @"MatchDate,TournamentId,Round,MatchId,HomeTeamId,HomeTeamName,AwayTeamId,AwayTeamName,HomeWinOdds,DrawOdds,AwayWinOdds,";
        csv += "\n2025-01-15,406,1,12345,1001,Aliquanta,1002,Sport Boys,1.80,3.40,4.20,";

        var result = _parser.Parse(csv);

        Assert.Single(result);
        Assert.Equal(1.80, result[0].HomeWinOdds);
    }
}
