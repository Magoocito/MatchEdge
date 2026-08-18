namespace MatchEdge.Application.UseCases.Backtesting;

public record BacktestSummary
{
    public int TotalMatches { get; init; }
    public int SkippedMatches { get; init; }
    public ModelVariantMetrics ModelA { get; init; } = new();
    public ModelVariantMetrics ModelB1 { get; init; } = new();
    public ModelVariantMetrics ModelB2 { get; init; } = new();
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
