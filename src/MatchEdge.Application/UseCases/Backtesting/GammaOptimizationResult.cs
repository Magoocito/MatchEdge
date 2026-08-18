namespace MatchEdge.Application.UseCases.Backtesting;

public record GammaOptimizationResult
{
    public double OptimalGamma { get; init; }
    public double BestBrierScore { get; init; }
    public double BestLogLoss { get; init; }
    public double B1SplitOnlyReferenceBrier { get; init; }
    public double B1SplitOnlyReferenceLogLoss { get; init; }
    public IReadOnlyList<GammaGridPoint> GridResults { get; init; } = [];
}

public record GammaGridPoint
{
    public double Gamma { get; init; }
    public double BrierScore { get; init; }
    public double LogLoss { get; init; }
    public int MatchCount { get; init; }
}
