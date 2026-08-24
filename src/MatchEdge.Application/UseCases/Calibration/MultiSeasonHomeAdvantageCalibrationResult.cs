namespace MatchEdge.Application.UseCases.Calibration;

public record MultiSeasonHomeAdvantageCalibrationResult(
    int TournamentId,
    int SeasonCount,
    int FromRound,
    int ToRound,
    int TotalMatches,
    int TotalHomeGoals,
    int TotalAwayGoals,
    double OverallAverageHomeGoals,
    double OverallAverageAwayGoals,
    double OverallHomeAdvantageFactor,
    IReadOnlyList<SeasonCalibrationDetail> SeasonDetails);
