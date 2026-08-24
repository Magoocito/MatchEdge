namespace MatchEdge.Application.UseCases.Backtesting;

public record GammaOptimizationResult
{
    public GammaPilotValidation PilotValidation { get; init; } = new();
    public GammaTrainResult Training { get; init; } = new();
    public GammaValidationResult Validation { get; init; } = new();
}
