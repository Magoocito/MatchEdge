namespace MatchEdge.Application.UseCases.Backtesting;

public record GammaValidationResult
{
    public double OptimalGamma { get; init; }
    public double BrierScore { get; init; }
    public double LogLoss { get; init; }
    public double B1SplitOnlyBrier { get; init; }
    public double B1SplitOnlyLogLoss { get; init; }
    public bool ImprovedVsTrain { get; init; }
    public bool OverfittingDetected { get; init; }
}
