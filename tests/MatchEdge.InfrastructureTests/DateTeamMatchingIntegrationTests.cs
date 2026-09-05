using System.Text.RegularExpressions;
using MatchEdge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MatchEdge.InfrastructureTests;

public class DateTeamMatchingIntegrationTests
{
    private MatchEdgeDbContext CreateRealDatabaseContext()
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "src", "MatchEdge.Api", "matchedge.db");
        if (!File.Exists(dbPath))
            throw new FileNotFoundException($"Database not found at {dbPath}. Run DataImporter first.");

        var options = new DbContextOptionsBuilder<MatchEdgeDbContext>()
            .UseSqlite($"Data Source={dbPath}")
            .Options;
        return new MatchEdgeDbContext(options);
    }

    private static string NormalizeTeamName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return string.Empty;
        var normalized = name.ToLowerInvariant().Trim();
        normalized = normalized
            .Replace("á", "a").Replace("é", "e").Replace("í", "i")
            .Replace("ó", "o").Replace("ú", "u").Replace("ñ", "n");
        normalized = Regex.Replace(normalized, @"\s+", " ");
        return normalized;
    }

    [Fact]
    public void Matching_AlianzaLimaVsRealGarcilaso_FindsOdds()
    {
        using var context = CreateRealDatabaseContext();
        var allOdds = context.HistoricalOdds.AsNoTracking().ToList();

        var matchDate = new DateTime(2025, 2, 8);
        var normalizedHome = NormalizeTeamName("Alianza Lima");
        var normalizedAway = NormalizeTeamName("Real Garcilaso");

        var candidate = allOdds.FirstOrDefault(o =>
            o.MatchDate.Date == matchDate.Date &&
            NormalizeTeamName(o.HomeTeamName) == normalizedHome &&
            NormalizeTeamName(o.AwayTeamName) == normalizedAway);

        Assert.NotNull(candidate);
        Assert.Equal("Alianza Lima", candidate.HomeTeamName);
        Assert.Equal("Real Garcilaso", candidate.AwayTeamName);
        Assert.True(candidate.HomeWinOdds > 0);
        Assert.True(candidate.DrawOdds > 0);
        Assert.True(candidate.AwayWinOdds > 0);
    }

    [Fact]
    public void Matching_AlianzaUniversidadVsSportingCristal_FindsOdds()
    {
        using var context = CreateRealDatabaseContext();
        var allOdds = context.HistoricalOdds.AsNoTracking().ToList();

        var matchDate = new DateTime(2025, 2, 9);
        var normalizedHome = NormalizeTeamName("Alianza Universidad");
        var normalizedAway = NormalizeTeamName("Sporting Cristal");

        var candidate = allOdds.FirstOrDefault(o =>
            o.MatchDate.Date == matchDate.Date &&
            NormalizeTeamName(o.HomeTeamName) == normalizedHome &&
            NormalizeTeamName(o.AwayTeamName) == normalizedAway);

        Assert.NotNull(candidate);
        Assert.Equal("Alianza Universidad", candidate.HomeTeamName);
        Assert.Equal("Sporting Cristal", candidate.AwayTeamName);
    }

    [Fact]
    public void Matching_AccentedNames_FindsOdds()
    {
        using var context = CreateRealDatabaseContext();
        var allOdds = context.HistoricalOdds.AsNoTracking().ToList();

        var matchDate = new DateTime(2023, 10, 8);
        var normalizedHome = NormalizeTeamName("Atlético Grau");
        var normalizedAway = NormalizeTeamName("Alianza Atlético");

        var candidate = allOdds.FirstOrDefault(o =>
            o.MatchDate.Date == matchDate.Date &&
            NormalizeTeamName(o.HomeTeamName) == normalizedHome &&
            NormalizeTeamName(o.AwayTeamName) == normalizedAway);

        Assert.NotNull(candidate);
        Assert.Equal("Atlético Grau", candidate.HomeTeamName);
        Assert.Equal("Alianza Atlético", candidate.AwayTeamName);
    }

    [Fact]
    public void Matching_NonexistentTeam_ReturnsNull()
    {
        using var context = CreateRealDatabaseContext();
        var allOdds = context.HistoricalOdds.AsNoTracking().ToList();

        var matchDate = new DateTime(2025, 6, 15);
        var normalizedHome = NormalizeTeamName("Nonexistent FC");
        var normalizedAway = NormalizeTeamName("Fake United");

        var candidate = allOdds.FirstOrDefault(o =>
            o.MatchDate.Date == matchDate.Date &&
            NormalizeTeamName(o.HomeTeamName) == normalizedHome &&
            NormalizeTeamName(o.AwayTeamName) == normalizedAway);

        Assert.Null(candidate);
    }

    [Fact]
    public void Matching_WrongDate_ReturnsNull()
    {
        using var context = CreateRealDatabaseContext();
        var allOdds = context.HistoricalOdds.AsNoTracking().ToList();

        var matchDate = new DateTime(2025, 2, 8);
        var normalizedHome = NormalizeTeamName("Sporting Cristal");
        var normalizedAway = NormalizeTeamName("Alianza Lima");

        var candidate = allOdds.FirstOrDefault(o =>
            o.MatchDate.Date == matchDate.Date &&
            NormalizeTeamName(o.HomeTeamName) == normalizedHome &&
            NormalizeTeamName(o.AwayTeamName) == normalizedAway);

        Assert.Null(candidate);
    }

    [Fact]
    public void Matching_CalculatesProbabilitiesCorrectly()
    {
        using var context = CreateRealDatabaseContext();
        var allOdds = context.HistoricalOdds.AsNoTracking().ToList();

        var matchDate = new DateTime(2023, 10, 8);
        var normalizedHome = NormalizeTeamName("ADT");
        var normalizedAway = NormalizeTeamName("Unión Comercio");

        var candidate = allOdds.FirstOrDefault(o =>
            o.MatchDate.Date == matchDate.Date &&
            NormalizeTeamName(o.HomeTeamName) == normalizedHome &&
            NormalizeTeamName(o.AwayTeamName) == normalizedAway);

        Assert.NotNull(candidate);

        var implHome = candidate.HomeWinOdds > 0 ? 1.0 / candidate.HomeWinOdds : 0;
        var implDraw = candidate.DrawOdds > 0 ? 1.0 / candidate.DrawOdds : 0;
        var implAway = candidate.AwayWinOdds > 0 ? 1.0 / candidate.AwayWinOdds : 0;
        var totalImplied = implHome + implDraw + implAway;

        Assert.True(totalImplied > 0);

        var homeProb = implHome / totalImplied;
        var drawProb = implDraw / totalImplied;
        var awayProb = implAway / totalImplied;

        Assert.True(homeProb > 0 && homeProb < 1);
        Assert.True(drawProb > 0 && drawProb < 1);
        Assert.True(awayProb > 0 && awayProb < 1);
        Assert.InRange(homeProb + drawProb + awayProb, 0.99, 1.01);
    }

    [Fact]
    public void Matching_MultipleMatchesOnSameDate_FindsAll()
    {
        using var context = CreateRealDatabaseContext();
        var allOdds = context.HistoricalOdds.AsNoTracking().ToList();

        var matchDate = new DateTime(2023, 10, 8);
        var matchesOnDate = allOdds.Where(o => o.MatchDate.Date == matchDate.Date).ToList();

        Assert.True(matchesOnDate.Count >= 2, $"Expected at least 2 matches on {matchDate:yyyy-MM-dd}, found {matchesOnDate.Count}");

        foreach (var match in matchesOnDate)
        {
            var normalizedHome = NormalizeTeamName(match.HomeTeamName);
            var normalizedAway = NormalizeTeamName(match.AwayTeamName);

            var found = allOdds.FirstOrDefault(o =>
                o.MatchDate.Date == matchDate.Date &&
                NormalizeTeamName(o.HomeTeamName) == normalizedHome &&
                NormalizeTeamName(o.AwayTeamName) == normalizedAway);

            Assert.NotNull(found);
            Assert.Equal(match.SourceMatchId, found.SourceMatchId);
        }
    }

    [Fact]
    public void Matching_2024Season_WorksCorrectly()
    {
        using var context = CreateRealDatabaseContext();
        var allOdds = context.HistoricalOdds.AsNoTracking().ToList();

        var matchDate = new DateTime(2024, 1, 28);
        var normalizedHome = NormalizeTeamName("Alianza Lima");
        var normalizedAway = NormalizeTeamName("César Vallejo");

        var candidate = allOdds.FirstOrDefault(o =>
            o.MatchDate.Date == matchDate.Date &&
            NormalizeTeamName(o.HomeTeamName) == normalizedHome &&
            NormalizeTeamName(o.AwayTeamName) == normalizedAway);

        Assert.NotNull(candidate);
        Assert.Equal("Alianza Lima", candidate.HomeTeamName);
        Assert.Equal("César Vallejo", candidate.AwayTeamName);
    }
}
