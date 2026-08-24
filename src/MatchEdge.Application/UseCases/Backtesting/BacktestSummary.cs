namespace MatchEdge.Application.UseCases.Backtesting;

public record BacktestSummary
{
    public int TotalMatches { get; init; }
    public int SkippedMatches { get; init; }
    public IReadOnlyList<SkippedMatchInfo> SkippedDetails { get; init; } = [];
    public ModelVariantMetrics ModelA { get; init; } = new();
    public ModelVariantMetrics ModelB1 { get; init; } = new();
    public ModelVariantMetrics ModelB2 { get; init; } = new();
}
