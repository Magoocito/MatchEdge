namespace MatchEdge.Infrastructure.Data.Entities;

public class TrendResultEntity
{
    public int Id { get; set; }
    public DateTime ScrapedAt { get; set; }
    public string FixtureSlug { get; set; } = string.Empty;
    public long FixtureApid { get; set; }
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public string League { get; set; } = string.Empty;
    public string Team { get; set; } = string.Empty;
    public string TeamSlug { get; set; } = string.Empty;
    public string Market { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Venue { get; set; } = string.Empty;
    public string HitCount { get; set; } = string.Empty;
    public int HitNumerator { get; set; }
    public int HitDenominator { get; set; }
    public double HitRate { get; set; }
    public int OppHitRate { get; set; }
    public double AvgValue { get; set; }
    public double RatePercentage { get; set; }
    public double Odds { get; set; }
    public string Fixture { get; set; } = string.Empty;
    public double Score { get; set; }
    public string Classification { get; set; } = string.Empty;
    public double Edge { get; set; }
    public bool IsSelected { get; set; }
    public int? DailyPickId { get; set; }
}
