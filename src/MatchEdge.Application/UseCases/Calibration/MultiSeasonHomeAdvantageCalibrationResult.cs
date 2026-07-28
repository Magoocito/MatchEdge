namespace MatchEdge.Application.UseCases.Calibration;

public record SeasonCalibrationDetail(
    int SeasonId,
    string SeasonName,
    string Prefix,
    int Matches,
    int HomeGoals,
    int AwayGoals,
    double AverageHomeGoals,
    double AverageAwayGoals,
    double HomeAdvantageFactor);

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