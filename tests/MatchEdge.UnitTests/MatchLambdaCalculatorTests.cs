using MatchEdge.Application.Configuration;
using MatchEdge.Application.UseCases.Lambda;
using MatchEdge.Domain.Models;
using Microsoft.Extensions.Options;

namespace MatchEdge.UnitTests;

public class MatchLambdaCalculatorTests
{
    private readonly TeamStatistics _homeTeam = new()
    {
        GoalsScored = 3,
        GoalsConceded = 7,
        Matches = 6
    };

    private readonly TeamStatistics _awayTeam = new()
    {
        GoalsScored = 10,
        GoalsConceded = 8,
        Matches = 10
    };

    [Fact]
    public void CalculateGoalLambdas_WithKnownValues_ShouldReturnExpectedResult()
    {
        // Arrange
        var options = Options.Create(new MatchModelOptions { HomeAdvantageFactor = 1.35 });
        var calculator = new MatchLambdaCalculator(options);

        // Act
        var (lambdaHome, lambdaAway) = calculator.CalculateGoalLambdas(_homeTeam, _awayTeam);

        // Assert
        // ataqueHome = 3/6 = 0.5
        // defensaHome = 7/6 = 1.1667
        // ataqueAway = 10/10 = 1.0
        // defensaAway = 8/10 = 0.8
        // lambdaHome = ((0.5 + 0.8) / 2) * 1.35 = 0.65 * 1.35 = 0.8775
        // lambdaAway = (1.0 + 1.1667) / 2 = 1.0833
        Assert.InRange(lambdaHome, 0.87, 0.88);
        Assert.InRange(lambdaAway, 1.08, 1.09);
    }

    [Fact]
    public void CalculateGoalLambdas_WithHomeAdvantage1_ShouldNotApplyAdvantage()
    {
        // Arrange
        var options = Options.Create(new MatchModelOptions { HomeAdvantageFactor = 1.0 });
        var calculator = new MatchLambdaCalculator(options);

        // Act
        var (lambdaHome, lambdaAway) = calculator.CalculateGoalLambdas(_homeTeam, _awayTeam);

        // Assert
        // lambdaHome = ((0.5 + 0.8) / 2) * 1.0 = 0.65
        // lambdaAway = (1.0 + 1.1667) / 2 = 1.0833
        Assert.InRange(lambdaHome, 0.64, 0.66);
        Assert.InRange(lambdaAway, 1.08, 1.09);
    }

    [Fact]
    public void CalculateGoalLambdas_WithZeroMatches_ShouldThrowArgumentException()
    {
        // Arrange
        var options = Options.Create(new MatchModelOptions { HomeAdvantageFactor = 1.35 });
        var calculator = new MatchLambdaCalculator(options);

        var homeWithZeroMatches = new TeamStatistics
        {
            GoalsScored = 0,
            GoalsConceded = 0,
            Matches = 0
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            calculator.CalculateGoalLambdas(homeWithZeroMatches, _awayTeam));
    }

    [Fact]
    public void CalculateGoalLambdas_WithAwayZeroMatches_ShouldThrowArgumentException()
    {
        // Arrange
        var options = Options.Create(new MatchModelOptions { HomeAdvantageFactor = 1.35 });
        var calculator = new MatchLambdaCalculator(options);

        var awayWithZeroMatches = new TeamStatistics
        {
            GoalsScored = 0,
            GoalsConceded = 0,
            Matches = 0
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            calculator.CalculateGoalLambdas(_homeTeam, awayWithZeroMatches));
    }
}
