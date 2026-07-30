namespace MatchEdge.Application.UseCases.Predictions;

public interface IMatchPredictionService
{
    Task<MatchPredictionResult?> PredictMatchAsync(
        int homeTeamId,
        int awayTeamId,
        int tournamentId);
}