namespace MatchEdge.Application.UseCases.Calibration;

public interface IMultiSeasonHomeAdvantageCalibrationService
{
    Task<MultiSeasonHomeAdvantageCalibrationResult> CalculateAsync(
        int tournamentId,
        int seasonCount = 3,
        int fromRound = 1,
        int toRound = 17);
}