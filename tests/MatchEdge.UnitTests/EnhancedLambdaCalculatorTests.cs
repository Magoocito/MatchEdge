using MatchEdge.Application.Configuration;
using MatchEdge.Application.UseCases.Context;
using MatchEdge.Application.UseCases.Lambda;
using Microsoft.Extensions.Options;

namespace MatchEdge.UnitTests;

public class EnhancedLambdaCalculatorTests
{
    private readonly IMatchLambdaCalculator _baselineCalculator;
    private readonly EnhancedLambdaCalculator _calculator;

    public EnhancedLambdaCalculatorTests()
    {
        var options = Options.Create(new MatchModelOptions { HomeAdvantageFactor = 1.58 });
        _baselineCalculator = new MatchLambdaCalculator(options);
        _calculator = new EnhancedLambdaCalculator(_baselineCalculator, options);
    }

    [Fact]
    public void Calculate_SufficientData_UsesHomeAwaySplitWithoutGamma()
    {
        var homeContext = new TeamContextStatistics(
            AttackHome: 1.8,
            DefenseHome: 1.0,
            AttackAway: 1.5,
            DefenseAway: 1.2,
            HomeMatchesCount: 10,
            AwayMatchesCount: 10,
            SkippedMatchesCount: 0);

        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4,
            DefenseHome: 1.3,
            AttackAway: 1.2,
            DefenseAway: 1.1,
            HomeMatchesCount: 10,
            AwayMatchesCount: 10,
            SkippedMatchesCount: 0);

        var result = _calculator.Calculate(homeContext, awayContext);

        // λ_home = (home.AttackHome + away.DefenseAway) / 2 = (1.8 + 1.1) / 2 = 1.45
        // λ_away = (away.AttackAway + home.DefenseHome) / 2 = (1.2 + 1.0) / 2 = 1.1
        Assert.Equal("HomeAwaySplit", result.CalculationMethod);
        Assert.Equal(1.45, result.LambdaHome, 4);
        Assert.Equal(1.1, result.LambdaAway, 4);
    }

    [Fact]
    public void Calculate_InsufficientHomeData_FallsBackToBaseline()
    {
        var homeContext = new TeamContextStatistics(
            AttackHome: 1.8,
            DefenseHome: 1.0,
            AttackAway: 1.5,
            DefenseAway: 1.2,
            HomeMatchesCount: 5, // < 8
            AwayMatchesCount: 10,
            SkippedMatchesCount: 0);

        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4,
            DefenseHome: 1.3,
            AttackAway: 1.2,
            DefenseAway: 1.1,
            HomeMatchesCount: 10,
            AwayMatchesCount: 10,
            SkippedMatchesCount: 0);

        var result = _calculator.Calculate(homeContext, awayContext);

        Assert.Equal("SeasonAverageWithGamma", result.CalculationMethod);
    }

    [Fact]
    public void Calculate_InsufficientAwayData_FallsBackToBaseline()
    {
        var homeContext = new TeamContextStatistics(
            AttackHome: 1.8,
            DefenseHome: 1.0,
            AttackAway: 1.5,
            DefenseAway: 1.2,
            HomeMatchesCount: 10,
            AwayMatchesCount: 10,
            SkippedMatchesCount: 0);

        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4,
            DefenseHome: 1.3,
            AttackAway: 1.2,
            DefenseAway: 1.1,
            HomeMatchesCount: 10,
            AwayMatchesCount: 5, // < 8
            SkippedMatchesCount: 0);

        var result = _calculator.Calculate(homeContext, awayContext);

        Assert.Equal("SeasonAverageWithGamma", result.CalculationMethod);
    }

    [Fact]
    public void Calculate_ExactlyEightMatches_UsesHomeAwaySplit()
    {
        var homeContext = new TeamContextStatistics(
            AttackHome: 1.8,
            DefenseHome: 1.0,
            AttackAway: 1.5,
            DefenseAway: 1.2,
            HomeMatchesCount: 8, // exactly 8
            AwayMatchesCount: 8, // exactly 8
            SkippedMatchesCount: 0);

        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4,
            DefenseHome: 1.3,
            AttackAway: 1.2,
            DefenseAway: 1.1,
            HomeMatchesCount: 8,
            AwayMatchesCount: 8,
            SkippedMatchesCount: 0);

        var result = _calculator.Calculate(homeContext, awayContext);

        Assert.Equal("HomeAwaySplit", result.CalculationMethod);
    }

    [Fact]
    public void Calculate_HomeAwaySplit_DoesNotApplyGamma()
    {
        var homeContext = new TeamContextStatistics(
            AttackHome: 2.0,
            DefenseHome: 1.0,
            AttackAway: 1.5,
            DefenseAway: 1.2,
            HomeMatchesCount: 10,
            AwayMatchesCount: 10,
            SkippedMatchesCount: 0);

        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4,
            DefenseHome: 1.3,
            AttackAway: 1.0,
            DefenseAway: 1.0,
            HomeMatchesCount: 10,
            AwayMatchesCount: 10,
            SkippedMatchesCount: 0);

        var result = _calculator.Calculate(homeContext, awayContext);

        // λ_home = (2.0 + 1.0) / 2 = 1.5
        // If gamma were applied: 1.5 * 1.58 = 2.37
        Assert.Equal(1.5, result.LambdaHome, 4);
        Assert.DoesNotContain("Gamma", result.CalculationMethod);
    }

    [Fact]
    public void Calculate_NullHomeContext_ThrowsArgumentNullException()
    {
        var awayContext = new TeamContextStatistics(
            AttackHome: 1.4, DefenseHome: 1.3, AttackAway: 1.2, DefenseAway: 1.1,
            HomeMatchesCount: 10, AwayMatchesCount: 10, SkippedMatchesCount: 0);

        Assert.Throws<ArgumentNullException>(() => _calculator.Calculate(null!, awayContext));
    }

    [Fact]
    public void Calculate_NullAwayContext_ThrowsArgumentNullException()
    {
        var homeContext = new TeamContextStatistics(
            AttackHome: 1.8, DefenseHome: 1.0, AttackAway: 1.5, DefenseAway: 1.2,
            HomeMatchesCount: 10, AwayMatchesCount: 10, SkippedMatchesCount: 0);

        Assert.Throws<ArgumentNullException>(() => _calculator.Calculate(homeContext, null!));
    }
}
