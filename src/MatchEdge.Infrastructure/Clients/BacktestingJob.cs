using MatchEdge.Application.UseCases.Backtesting;

namespace MatchEdge.Infrastructure.Clients;

public class BacktestingJob
{
    public string JobId { get; init; } = Guid.NewGuid().ToString("N")[..8];
    public string Status { get; set; } = "Pending";
    public int TotalMatches { get; set; }
    public int ProcessedMatches { get; set; }
    public string CurrentMatch { get; set; } = string.Empty;
    public DateTime StartedAt { get; init; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }

    public BacktestSummary? Summary { get; set; }
    public IReadOnlyList<BacktestMatchResult>? Details { get; set; }
    public GammaOptimizationResult? GammaResult { get; set; }
}
