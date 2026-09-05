namespace MatchEdge.Infrastructure.Data.Entities;

public class MatchMappingEntity
{
    public int Id { get; set; }
    public string Source { get; set; } = string.Empty;
    public int SourceMatchId { get; set; }
    public int SofaScoreEventId { get; set; }
    public DateTime MatchDate { get; set; }
    public double MatchConfidence { get; set; }
    public DateTime CreatedAt { get; set; }
}
