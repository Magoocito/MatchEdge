namespace MatchEdge.Application.UseCases.Backtesting;

public record BacktestMatchResult
{
    public int MatchId { get; init; }
    public int HomeTeamId { get; init; }
    public int AwayTeamId { get; init; }
    public DateTime MatchDate { get; init; }
    public string ActualResult { get; init; } = string.Empty;

    public double ModelA_HomeWinProb { get; init; }
    public double ModelA_DrawProb { get; init; }
    public double ModelA_AwayWinProb { get; init; }

    public double ModelB1_HomeWinProb { get; init; }
    public double ModelB1_DrawProb { get; init; }
    public double ModelB1_AwayWinProb { get; init; }

    public double ModelB2_HomeWinProb { get; init; }
    public double ModelB2_DrawProb { get; init; }
    public double ModelB2_AwayWinProb { get; init; }

    public string CalculationMethod { get; init; } = string.Empty;
}
