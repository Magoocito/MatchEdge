namespace MatchEdge.Domain.Matches;

public class FootballMatchEvent
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public MatchTeam HomeTeam { get; set; } = new();
    public MatchTeam AwayTeam { get; set; } = new();
    public MatchScore HomeScore { get; set; } = new();
    public MatchScore AwayScore { get; set; } = new();
    public MatchStatus Status { get; set; } = new();
    public RoundInfo RoundInfo { get; set; } = new();
    public int StartTimestamp { get; set; }
}
