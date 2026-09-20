namespace MatchEdge.Infrastructure.Data.Entities;

public class PickOutcomeEntity
{
    public int Id { get; set; }
    public int DailyPickId { get; set; }
    public DateTime MatchDate { get; set; }
    public string Team { get; set; } = string.Empty;
    public string Market { get; set; } = string.Empty;
    public double Odds { get; set; }
    public bool Won { get; set; }
    public double Profit { get; set; }
    public double ProfitUnits { get; set; }
    public double BalanceAfter { get; set; }
    public string ResultDetail { get; set; } = string.Empty;
    public DateTime EvaluatedAt { get; set; }
}
