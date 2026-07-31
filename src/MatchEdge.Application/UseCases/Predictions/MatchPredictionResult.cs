using MatchEdge.Application.UseCases.Probability;
using MatchEdge.Application.UseCases.ValueBetting;

namespace MatchEdge.Application.UseCases.Predictions;

public record MatchPredictionResult(
    int TournamentId,
    TeamSummary HomeTeam,
    TeamSummary AwayTeam,
    ExpectedGoalsResult ExpectedGoals,
    MatchResultProbabilities MatchResultProbabilities,
    OverUnderProbabilities OverUnderProbabilities,
    IReadOnlyList<ScoreProbability> MostLikelyScores,
    IReadOnlyList<ValueBetAnalysis>? ValueBets = null);