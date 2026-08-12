using MatchEdge.Application.Configuration;
using MatchEdge.Application.UseCases.Context;
using MatchEdge.Application.UseCases.Statistics;
using MatchEdge.Domain.Models;
using Microsoft.Extensions.Options;

namespace MatchEdge.Application.UseCases.Lambda;

/// <summary>
/// Enhanced lambda calculator using home/away contextual statistics.
/// 
/// GAMMA (HomeAdvantageFactor) DESIGN DECISION:
/// When using the home/away split, gamma is NOT applied because the home advantage
/// is already captured in the contextual average. For example, if a team scores
/// 1.8 goals per game at home on average, that number already includes all factors
/// that make them score more at home (crowd, familiarity, opponent travel fatigue, etc.).
/// Applying gamma on top would double-count the home advantage.
/// 
/// Gamma IS applied in the fallback path (SeasonAverageWithGamma) where the baseline
/// MatchLambdaCalculator is used, because that calculator works with league-wide averages
/// that don't distinguish localía.
/// 
/// This is a HYPOTHESIS that must be validated in backtesting (step 2 of the plan).
/// If time permits, also run the variant WITH gamma applied over the split for comparison.
/// If not compared, document explicitly why.
/// 
/// FALLBACK DATA SOURCE:
/// When the contextual sample is insufficient (< 8 matches per role), the fallback
/// uses IStatisticsService.GetTeamStatisticsAsync to get REAL full-season statistics
/// (home + away combined) instead of reconstructing partial data from TeamContextStatistics.
/// This ensures the fallback uses the most reliable data available.
/// </summary>
public class EnhancedLambdaCalculator : IEnhancedLambdaCalculator
{
    private const int MinMatchesThreshold = 8;
    private readonly IMatchLambdaCalculator _fallbackCalculator;
    private readonly IStatisticsService _statisticsService;
    private readonly MatchModelOptions _options;

    public EnhancedLambdaCalculator(
        IMatchLambdaCalculator fallbackCalculator,
        IStatisticsService statisticsService,
        IOptions<MatchModelOptions> options)
    {
        _fallbackCalculator = fallbackCalculator;
        _statisticsService = statisticsService;
        _options = options.Value;
    }

    public async Task<EnhancedLambdaResult> CalculateAsync(
        TeamContextStatistics homeContext,
        TeamContextStatistics awayContext,
        int homeTeamId,
        int awayTeamId,
        int tournamentId)
    {
        if (homeContext == null)
            throw new ArgumentNullException(nameof(homeContext));

        if (awayContext == null)
            throw new ArgumentNullException(nameof(awayContext));

        bool hasSufficientHomeData = homeContext.HomeMatchesCount >= MinMatchesThreshold;
        bool hasSufficientAwayData = awayContext.AwayMatchesCount >= MinMatchesThreshold;

        if (hasSufficientHomeData && hasSufficientAwayData)
        {
            double lambdaHome = (homeContext.AttackHome + awayContext.DefenseAway) / 2;
            double lambdaAway = (awayContext.AttackAway + homeContext.DefenseHome) / 2;

            return new EnhancedLambdaResult(lambdaHome, lambdaAway, "HomeAwaySplit");
        }

        var homeStats = await _statisticsService.GetTeamStatisticsAsync(homeTeamId, tournamentId)
            ?? throw new InvalidOperationException(
                $"No statistics available for home team {homeTeamId} in tournament {tournamentId}");

        var awayStats = await _statisticsService.GetTeamStatisticsAsync(awayTeamId, tournamentId)
            ?? throw new InvalidOperationException(
                $"No statistics available for away team {awayTeamId} in tournament {tournamentId}");

        var fallback = _fallbackCalculator.CalculateGoalLambdas(homeStats, awayStats);

        return new EnhancedLambdaResult(fallback.lambdaHome, fallback.lambdaAway, "SeasonAverageWithGamma");
    }
}
