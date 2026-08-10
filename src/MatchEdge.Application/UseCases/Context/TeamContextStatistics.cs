namespace MatchEdge.Application.UseCases.Context;

public record TeamContextStatistics(
    double AttackHome,
    double DefenseHome,
    double AttackAway,
    double DefenseAway,
    int HomeMatchesCount,
    int AwayMatchesCount,
    int SkippedMatchesCount);
