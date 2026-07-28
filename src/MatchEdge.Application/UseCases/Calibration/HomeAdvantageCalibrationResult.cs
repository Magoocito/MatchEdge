namespace MatchEdge.Application.UseCases.Calibration;

public record HomeAdvantageCalibrationResult(
    int TournamentId,
    int SeasonId,
    string Prefix,
    int FromRound,
    int ToRound,
    int Matches,
    int HomeGoals,
    int AwayGoals,
    double AverageHomeGoals,
    double AverageAwayGoals,
    double HomeAdvantageFactor);
