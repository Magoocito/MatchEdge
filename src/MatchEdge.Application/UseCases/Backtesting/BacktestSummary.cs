namespace MatchEdge.Application.UseCases.Backtesting;

public record BacktestSummary
{
    public int TotalMatches { get; init; }
    public int SkippedMatches { get; init; }
    public IReadOnlyList<SkippedMatchInfo> SkippedDetails { get; init; } = [];
    public ModelVariantMetrics ModelA { get; init; } = new();
    public ModelVariantMetrics ModelB1 { get; init; } = new();
    public ModelVariantMetrics ModelB2 { get; init; } = new();
    public CalibrationResult? CalibrationA { get; init; }
    public CalibrationResult? CalibrationB1 { get; init; }
    public CalibrationResult? CalibrationB2 { get; init; }
}

public record SkippedMatchInfo
{
    public int MatchId { get; init; }
    public int HomeTeamId { get; init; }
    public int AwayTeamId { get; init; }
    public DateTime MatchDate { get; init; }
    public string Error { get; init; } = "";
}

public record ModelVariantMetrics
{
    public MetricSet Overall { get; init; } = new();
    public MetricSet SplitOnly { get; init; } = new();
    public MetricSet FallbackOnly { get; init; } = new();
}

public record MetricSet
{
    public double BrierScore { get; init; }
    public double LogLoss { get; init; }
    public int MatchCount { get; init; }
}
