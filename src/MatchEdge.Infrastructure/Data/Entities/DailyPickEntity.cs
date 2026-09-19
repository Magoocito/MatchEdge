namespace MatchEdge.Infrastructure.Data.Entities;

public class DailyPickEntity
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TrendResultId { get; set; }
    public string Team { get; set; } = string.Empty;
    public string Market { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Score { get; set; }
    public string Classification { get; set; } = string.Empty;
    public double Edge { get; set; }
    public double Odds { get; set; }
    public double HitRate { get; set; }
    public int SampleSize { get; set; }
    public double StakeUnits { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public int? PickOutcomeId { get; set; }
}
