namespace MatchEdge.Application.Clients.FootyMetrics;

public record TrendScoreResult(
    string TeamName,
    string Market,
    string Direction,
    string Line,
    double HitRate,
    double OpponentHitRate,
    int SampleSize,
    double Edge,
    decimal? OddsValue,
    double CompositeScore,
    string Classification
);
