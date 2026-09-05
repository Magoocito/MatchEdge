namespace MatchEdge.Infrastructure.Data.Entities;

public class TeamMappingEntity
{
    public int Id { get; set; }
    public string Source { get; set; } = string.Empty;
    public int SourceTeamId { get; set; }
    public string SourceTeamName { get; set; } = string.Empty;
    public int SofaScoreTeamId { get; set; }
    public string SofaScoreTeamName { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public DateTime CreatedAt { get; set; }
}
