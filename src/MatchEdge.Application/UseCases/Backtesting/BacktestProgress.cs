namespace MatchEdge.Application.UseCases.Backtesting;

public record BacktestProgress(
    int ProcessedMatches,
    int TotalMatches,
    string CurrentMatch);
