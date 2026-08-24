namespace MatchEdge.Application.UseCases.Backtesting;

public record ModelVariantMetrics
{
    public MetricSet Overall { get; init; } = new();
    public MetricSet SplitOnly { get; init; } = new();
    public MetricSet FallbackOnly { get; init; } = new();
}
