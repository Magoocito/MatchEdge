namespace MatchEdge.Application.UseCases.Backtesting;

public record GammaPilotValidation
{
    public bool IsConsistent { get; init; }
    public string? InconsistencyReason { get; init; }
    public IReadOnlyList<GammaGridPoint> PilotPoints { get; init; } = [];
}
