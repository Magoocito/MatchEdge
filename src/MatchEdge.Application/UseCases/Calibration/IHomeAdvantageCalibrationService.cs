namespace MatchEdge.Application.UseCases.Calibration;

public interface IHomeAdvantageCalibrationService
{
    Task<HomeAdvantageCalibrationResult> CalculateAsync(
        int tournamentId,
        string prefix,
        int fromRound,
        int toRound);
}
