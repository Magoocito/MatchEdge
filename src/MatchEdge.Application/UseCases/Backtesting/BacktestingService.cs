using MatchEdge.Application.Configuration;
using MatchEdge.Application.Services;
using MatchEdge.Application.UseCases.Context;
using MatchEdge.Application.UseCases.Historical;
using MatchEdge.Application.UseCases.Lambda;
using MatchEdge.Application.UseCases.Probability;
using MatchEdge.Application.UseCases.Statistics;
using Microsoft.Extensions.Options;

namespace MatchEdge.Application.UseCases.Backtesting;

public class BacktestingService : IBacktestingService
{
    private readonly ISeasonService _seasonService;
    private readonly IHistoricalMatchEnumerator _matchEnumerator;
    private readonly ITeamContextStatisticsService _contextStatisticsService;
    private readonly IHistoricalTeamStatisticsProvider _historicalStatisticsProvider;
    private readonly IProbabilityEngine _probabilityEngine;

    private static readonly string[] Prefixes = ["Apertura", "Clausura"];
    private const int FromRound = 1;
    private const int ToRound = 17;

    public BacktestingService(
        ISeasonService seasonService,
        IHistoricalMatchEnumerator matchEnumerator,
        ITeamContextStatisticsService contextStatisticsService,
        IHistoricalTeamStatisticsProvider historicalStatisticsProvider,
        IProbabilityEngine probabilityEngine)
    {
        _seasonService = seasonService;
        _matchEnumerator = matchEnumerator;
        _contextStatisticsService = contextStatisticsService;
        _historicalStatisticsProvider = historicalStatisticsProvider;
        _probabilityEngine = probabilityEngine;
    }

    public async Task<(BacktestSummary Summary, IReadOnlyList<BacktestMatchResult> Details)> RunAsync(
        int tournamentId,
        DateTime fromDate,
        DateTime toDate,
        double experimentalGamma,
        bool includeB2 = true,
        int seasonLookback = 2,
        IProgress<BacktestProgress>? progress = null)
    {
        var seasonIds = await _seasonService.GetRecentSeasonIdsAsOfAsync(
            tournamentId, seasonLookback, toDate);

        var allMatches = await _matchEnumerator.GetFinishedMatchesAsync(
            tournamentId, seasonIds, FromRound, ToRound, Prefixes);

        var filteredMatches = allMatches
            .Where(m =>
            {
                var matchDate = DateTimeOffset.FromUnixTimeSeconds(m.Event.StartTimestamp).UtcDateTime;
                return matchDate >= fromDate && matchDate <= toDate;
            })
            .OrderBy(m => m.Event.StartTimestamp)
            .ToList();

        var baselineOptions = Options.Create(new MatchModelOptions { HomeAdvantageFactor = experimentalGamma });
        var baselineCalculator = new MatchLambdaCalculator(baselineOptions);

        var b1Calculator = new EnhancedLambdaCalculator(
            baselineCalculator,
            _historicalStatisticsProvider,
            baselineOptions,
            applyGammaToSplit: false);

        EnhancedLambdaCalculator? b2Calculator = null;
        if (includeB2)
        {
            b2Calculator = new EnhancedLambdaCalculator(
                baselineCalculator,
                _historicalStatisticsProvider,
                baselineOptions,
                applyGammaToSplit: true);
        }

        var details = new List<BacktestMatchResult>();
        var totalMatches = filteredMatches.Count;
        var skippedMatches = 0;

        for (var i = 0; i < filteredMatches.Count; i++)
        {
            var match = filteredMatches[i];
            var matchDate = DateTimeOffset.FromUnixTimeSeconds(match.Event.StartTimestamp).UtcDateTime;
            var homeTeamId = match.Event.HomeTeam.Id;
            var awayTeamId = match.Event.AwayTeam.Id;

            BacktestMatchResult? result;
            try
            {
                var actualResult = GetActualResult(match.Event);

                var homeContext = await _contextStatisticsService.CalculateAsync(
                    homeTeamId, tournamentId, matchDate, seasonLookback);
                var awayContext = await _contextStatisticsService.CalculateAsync(
                    awayTeamId, tournamentId, matchDate, seasonLookback);

                var modelAResult = await CalculateModelA(
                    baselineCalculator, homeTeamId, awayTeamId, tournamentId, matchDate, seasonLookback);

                var modelB1Result = await b1Calculator.CalculateAsync(
                    homeContext, awayContext, homeTeamId, awayTeamId, tournamentId, matchDate, seasonLookback);

                var modelB1Probs = _probabilityEngine.GetMatchResultProbabilities(
                    modelB1Result.LambdaHome, modelB1Result.LambdaAway);

                double modelB2HomeWin = 0, modelB2Draw = 0, modelB2AwayWin = 0;
                if (includeB2 && b2Calculator != null)
                {
                    var modelB2Result = await b2Calculator.CalculateAsync(
                        homeContext, awayContext, homeTeamId, awayTeamId, tournamentId, matchDate, seasonLookback);

                    var modelB2Probs = _probabilityEngine.GetMatchResultProbabilities(
                        modelB2Result.LambdaHome, modelB2Result.LambdaAway);

                    modelB2HomeWin = modelB2Probs.HomeWin;
                    modelB2Draw = modelB2Probs.Draw;
                    modelB2AwayWin = modelB2Probs.AwayWin;
                }

                result = new BacktestMatchResult
                {
                    MatchId = match.Event.Id,
                    HomeTeamId = homeTeamId,
                    AwayTeamId = awayTeamId,
                    MatchDate = matchDate,
                    ActualResult = actualResult,
                    ModelA_HomeWinProb = modelAResult.Probs.HomeWin,
                    ModelA_DrawProb = modelAResult.Probs.Draw,
                    ModelA_AwayWinProb = modelAResult.Probs.AwayWin,
                    ModelB1_HomeWinProb = modelB1Probs.HomeWin,
                    ModelB1_DrawProb = modelB1Probs.Draw,
                    ModelB1_AwayWinProb = modelB1Probs.AwayWin,
                    ModelB2_HomeWinProb = modelB2HomeWin,
                    ModelB2_DrawProb = modelB2Draw,
                    ModelB2_AwayWinProb = modelB2AwayWin,
                    CalculationMethod = modelB1Result.CalculationMethod
                };
            }
            catch (Exception ex)
            {
                skippedMatches++;
                continue;
            }

            details.Add(result);

            progress?.Report(new BacktestProgress(i + 1, totalMatches,
                $"{match.Event.HomeTeam.ShortName} vs {match.Event.AwayTeam.ShortName}"));
        }

        var summary = ComputeSummary(details, includeB2) with { SkippedMatches = skippedMatches };
        return (summary, details);
    }

    private async Task<(double LambdaHome, double LambdaAway, MatchResultProbabilities Probs)> CalculateModelA(
        MatchLambdaCalculator calculator,
        int homeTeamId,
        int awayTeamId,
        int tournamentId,
        DateTime asOfDateTime,
        int seasonLookback)
    {
        var homeStats = await _historicalStatisticsProvider.GetAsOfAsync(
            homeTeamId, tournamentId, asOfDateTime, seasonLookback);
        var awayStats = await _historicalStatisticsProvider.GetAsOfAsync(
            awayTeamId, tournamentId, asOfDateTime, seasonLookback);

        var (lambdaHome, lambdaAway) = calculator.CalculateGoalLambdas(homeStats, awayStats);
        var probs = _probabilityEngine.GetMatchResultProbabilities(lambdaHome, lambdaAway);

        return (lambdaHome, lambdaAway, probs);
    }

    private static string GetActualResult(Domain.Matches.FootballMatchEvent match)
    {
        var homeGoals = match.HomeScore.Current!.Value;
        var awayGoals = match.AwayScore.Current!.Value;

        if (homeGoals > awayGoals) return "H";
        if (homeGoals < awayGoals) return "A";
        return "D";
    }

    private static BacktestSummary ComputeSummary(IReadOnlyList<BacktestMatchResult> details, bool includeB2)
    {
        var overallA = details.Select(d => (d.ModelA_HomeWinProb, d.ModelA_DrawProb, d.ModelA_AwayWinProb, d.ActualResult)).ToList();
        var overallB1 = details.Select(d => (d.ModelB1_HomeWinProb, d.ModelB1_DrawProb, d.ModelB1_AwayWinProb, d.ActualResult)).ToList();

        var splitMatches = details.Where(d => d.CalculationMethod == "HomeAwaySplit").ToList();
        var fallbackMatches = details.Where(d => d.CalculationMethod == "SeasonAverageWithGamma").ToList();

        var splitA = splitMatches.Select(d => (d.ModelA_HomeWinProb, d.ModelA_DrawProb, d.ModelA_AwayWinProb, d.ActualResult)).ToList();
        var splitB1 = splitMatches.Select(d => (d.ModelB1_HomeWinProb, d.ModelB1_DrawProb, d.ModelB1_AwayWinProb, d.ActualResult)).ToList();
        var fallbackA = fallbackMatches.Select(d => (d.ModelA_HomeWinProb, d.ModelA_DrawProb, d.ModelA_AwayWinProb, d.ActualResult)).ToList();
        var fallbackB1 = fallbackMatches.Select(d => (d.ModelB1_HomeWinProb, d.ModelB1_DrawProb, d.ModelB1_AwayWinProb, d.ActualResult)).ToList();

        var summary = new BacktestSummary
        {
            TotalMatches = details.Count,
            ModelA = new ModelVariantMetrics
            {
                Overall = ComputeMetrics(overallA),
                SplitOnly = ComputeMetrics(splitA),
                FallbackOnly = ComputeMetrics(fallbackA)
            },
            ModelB1 = new ModelVariantMetrics
            {
                Overall = ComputeMetrics(overallB1),
                SplitOnly = ComputeMetrics(splitB1),
                FallbackOnly = ComputeMetrics(fallbackB1)
            }
        };

        if (includeB2)
        {
            var overallB2 = details.Select(d => (d.ModelB2_HomeWinProb, d.ModelB2_DrawProb, d.ModelB2_AwayWinProb, d.ActualResult)).ToList();
            var splitB2 = splitMatches.Select(d => (d.ModelB2_HomeWinProb, d.ModelB2_DrawProb, d.ModelB2_AwayWinProb, d.ActualResult)).ToList();
            var fallbackB2 = fallbackMatches.Select(d => (d.ModelB2_HomeWinProb, d.ModelB2_DrawProb, d.ModelB2_AwayWinProb, d.ActualResult)).ToList();

            summary = summary with
            {
                ModelB2 = new ModelVariantMetrics
                {
                    Overall = ComputeMetrics(overallB2),
                    SplitOnly = ComputeMetrics(splitB2),
                    FallbackOnly = ComputeMetrics(fallbackB2)
                }
            };
        }

        return summary;
    }

    private static MetricSet ComputeMetrics(IReadOnlyList<(double HomeWin, double Draw, double AwayWin, string Actual)> preds)
    {
        if (preds.Count == 0)
            return new MetricSet { BrierScore = 0, LogLoss = 0, MatchCount = 0 };

        return new MetricSet
        {
            BrierScore = BrierScoreCalculator.Calculate(preds),
            LogLoss = LogLossCalculator.Calculate(preds),
            MatchCount = preds.Count
        };
    }
}
