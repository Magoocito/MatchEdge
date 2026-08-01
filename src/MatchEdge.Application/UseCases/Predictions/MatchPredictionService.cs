using MatchEdge.Application.UseCases.Lambda;
using MatchEdge.Application.UseCases.Probability;
using MatchEdge.Application.UseCases.Statistics;
using MatchEdge.Application.UseCases.Teams;

namespace MatchEdge.Application.UseCases.Predictions;

public class MatchPredictionService : IMatchPredictionService
{
    private readonly IStatisticsService _statisticsService;
    private readonly ITeamService _teamService;
    private readonly IMatchLambdaCalculator _lambdaCalculator;
    private readonly IProbabilityEngine _probabilityEngine;

    public MatchPredictionService(
        IStatisticsService statisticsService,
        ITeamService teamService,
        IMatchLambdaCalculator lambdaCalculator,
        IProbabilityEngine probabilityEngine)
    {
        _statisticsService = statisticsService;
        _teamService = teamService;
        _lambdaCalculator = lambdaCalculator;
        _probabilityEngine = probabilityEngine;
    }

    public async Task<MatchPredictionResult?> PredictMatchAsync(
        int homeTeamId,
        int awayTeamId,
        int tournamentId)
    {
        if (homeTeamId <= 0)
            throw new ArgumentException("Home team ID must be greater than zero", nameof(homeTeamId));

        if (awayTeamId <= 0)
            throw new ArgumentException("Away team ID must be greater than zero", nameof(awayTeamId));

        if (homeTeamId == awayTeamId)
            throw new ArgumentException("Home and away teams must be different");

        // 1. Obtener estadísticas de ambos equipos
        var homeStatsTask = _statisticsService.GetTeamStatisticsAsync(homeTeamId, tournamentId);
        var awayStatsTask = _statisticsService.GetTeamStatisticsAsync(awayTeamId, tournamentId);
        var teamsTask = _teamService.GetTeamsAsync(tournamentId);

        await Task.WhenAll(homeStatsTask, awayStatsTask, teamsTask);

        var homeStats = homeStatsTask.Result;
        var awayStats = awayStatsTask.Result;
        var teams = teamsTask.Result;

        if (homeStats == null || awayStats == null)
        {
            return null;
        }

        // 2. Resolver nombres de equipos
        var homeTeamName = teams?.FirstOrDefault(t => t.Id == homeTeamId)?.Name ?? $"Team {homeTeamId}";
        var awayTeamName = teams?.FirstOrDefault(t => t.Id == awayTeamId)?.Name ?? $"Team {awayTeamId}";

        var homeTeamSummary = new TeamSummary(homeTeamId, homeTeamName);
        var awayTeamSummary = new TeamSummary(awayTeamId, awayTeamName);

        // 3. Calcular lambdas de goles esperados
        var (lambdaHome, lambdaAway) = _lambdaCalculator.CalculateGoalLambdas(homeStats, awayStats);
        var lambdaTotal = lambdaHome + lambdaAway;

        var expectedGoals = new ExpectedGoalsResult(
            Math.Round(lambdaHome, 2),
            Math.Round(lambdaAway, 2),
            Math.Round(lambdaTotal, 2));

        // 4. Calcular probabilidades 1X2
        var rawResultProbs = _probabilityEngine.GetMatchResultProbabilities(lambdaHome, lambdaAway);
        var matchResultProbabilities = new MatchResultProbabilities
        {
            HomeWin = Math.Round(rawResultProbs.HomeWin, 4),
            Draw = Math.Round(rawResultProbs.Draw, 4),
            AwayWin = Math.Round(rawResultProbs.AwayWin, 4)
        };

        // 5. Calcular probabilidades Over/Under
        var overUnder = new OverUnderProbabilities(
            Over1_5: Math.Round(_probabilityEngine.GetOverUnderProbability(lambdaTotal, 1.5, true), 4),
            Under1_5: Math.Round(_probabilityEngine.GetOverUnderProbability(lambdaTotal, 1.5, false), 4),
            Over2_5: Math.Round(_probabilityEngine.GetOverUnderProbability(lambdaTotal, 2.5, true), 4),
            Under2_5: Math.Round(_probabilityEngine.GetOverUnderProbability(lambdaTotal, 2.5, false), 4),
            Over3_5: Math.Round(_probabilityEngine.GetOverUnderProbability(lambdaTotal, 3.5, true), 4),
            Under3_5: Math.Round(_probabilityEngine.GetOverUnderProbability(lambdaTotal, 3.5, false), 4));

        // 6. Calcular BTTS
        double bttsYes = Math.Round(_probabilityEngine.GetBttsYesProbability(lambdaHome, lambdaAway), 4);
        double bttsNo = Math.Round(1.0 - bttsYes, 4);
        var btts = new BttsProbabilities(bttsYes, bttsNo);

        // 7. Calcular marcadores exactos más probables (Top 5)
        var scoresList = new List<ScoreProbability>();
        for (int x = 0; x <= 6; x++)
        {
            for (int y = 0; y <= 6; y++)
            {
                var prob = _probabilityEngine.PoissonProbability(lambdaHome, x) *
                           _probabilityEngine.PoissonProbability(lambdaAway, y);

                scoresList.Add(new ScoreProbability($"{x} - {y}", Math.Round(prob, 4)));
            }
        }

        var mostLikelyScores = scoresList
            .OrderByDescending(s => s.Probability)
            .Take(5)
            .ToList();

        // 8. Retornar DTO consolidado
        return new MatchPredictionResult(
            tournamentId,
            homeTeamSummary,
            awayTeamSummary,
            expectedGoals,
            matchResultProbabilities,
            overUnder,
            btts,
            mostLikelyScores);
    }
}