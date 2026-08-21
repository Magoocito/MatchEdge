namespace MatchEdge.Application.UseCases.Backtesting;

public record GammaOptimizationResult
{
    public GammaPilotValidation PilotValidation { get; init; } = new();
    public GammaTrainResult Training { get; init; } = new();
    public GammaValidationResult Validation { get; init; } = new();
}

public record GammaPilotValidation
{
    public bool IsConsistent { get; init; }
    public string? InconsistencyReason { get; init; }
    public IReadOnlyList<GammaGridPoint> PilotPoints { get; init; } = [];
}

public record GammaTrainResult
{
    public double OptimalGamma { get; init; }
    public double BestBrierScore { get; init; }
    public double BestLogLoss { get; init; }
    public double B1SplitOnlyBrier { get; init; }
    public double B1SplitOnlyLogLoss { get; init; }
    public IReadOnlyList<GammaGridPoint> GridResults { get; init; } = [];
}

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

public record GammaGridPoint
{
    public double Gamma { get; init; }
    public double BrierScore { get; init; }
    public double LogLoss { get; init; }
    public int MatchCount { get; init; }
    public IReadOnlyList<int> MatchIds { get; init; } = [];
    public IReadOnlyList<DateTime> AsOfDateTimes { get; init; } = [];
}
