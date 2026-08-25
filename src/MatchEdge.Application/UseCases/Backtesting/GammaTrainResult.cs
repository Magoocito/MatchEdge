namespace MatchEdge.Application.UseCases.Backtesting;

public record GammaTrainResult
{
    public double OptimalGamma { get; init; }
    public double BestBrierScore { get; init; }
    public double BestLogLoss { get; init; }
    public double B1SplitOnlyBrier { get; init; }
    public double B1SplitOnlyLogLoss { get; init; }
    public IReadOnlyList<GammaGridPoint> GridResults { get; init; } = [];
}
