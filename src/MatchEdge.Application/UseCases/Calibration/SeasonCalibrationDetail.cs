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
