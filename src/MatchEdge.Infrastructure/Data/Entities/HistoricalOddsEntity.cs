namespace MatchEdge.Infrastructure.Data.Entities;

public class HistoricalOddsEntity
{
    public int Id { get; set; }
    public int SourceMatchId { get; set; }
    public string Source { get; set; } = string.Empty;
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
    public int? SofaScoreEventId { get; set; }
    public int? SofaScoreHomeTeamId { get; set; }
    public int? SofaScoreAwayTeamId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? MappedAt { get; set; }
}
