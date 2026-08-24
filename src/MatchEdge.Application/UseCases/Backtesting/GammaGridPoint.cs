namespace MatchEdge.Application.UseCases.Backtesting;

public record GammaGridPoint
{
    public double Gamma { get; init; }
    public double BrierScore { get; init; }
    public double LogLoss { get; init; }
    public int MatchCount { get; init; }
    public IReadOnlyList<int> MatchIds { get; init; } = [];
    public IReadOnlyList<DateTime> AsOfDateTimes { get; init; } = [];
}
