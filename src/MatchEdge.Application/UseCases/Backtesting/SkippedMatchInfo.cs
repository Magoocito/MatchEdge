namespace MatchEdge.Application.UseCases.Backtesting;

public record SkippedMatchInfo
{
    public int MatchId { get; init; }
    public int HomeTeamId { get; init; }
    public int AwayTeamId { get; init; }
    public DateTime MatchDate { get; init; }
    public string Error { get; init; } = "";
}
