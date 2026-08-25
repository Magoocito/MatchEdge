namespace MatchEdge.Application.UseCases.Backtesting;

public record MetricSet
{
    public double BrierScore { get; init; }
    public double LogLoss { get; init; }
    public int MatchCount { get; init; }
}
