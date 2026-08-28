namespace MatchEdge.Domain.Odds;

public class HistoricalOdds
{
    public int MatchId { get; set; }
    public DateTime MatchDate { get; set; }
    public int TournamentId { get; set; }
    public int Round { get; set; }
    public int HomeTeamId { get; set; }
    public string HomeTeamName { get; set; } = string.Empty;
    public int AwayTeamId { get; set; }
    public string AwayTeamName { get; set; } = string.Empty;

    public double HomeWinOdds { get; set; }
    public double DrawOdds { get; set; }
    public double AwayWinOdds { get; set; }

    public double ImpliedHomeWinProbability => HomeWinOdds > 0 ? 1.0 / HomeWinOdds : 0;
    public double ImpliedDrawProbability => DrawOdds > 0 ? 1.0 / DrawOdds : 0;
    public double ImpliedAwayWinProbability => AwayWinOdds > 0 ? 1.0 / AwayWinOdds : 0;
}
