namespace MatchEdge.Api.Controllers;

public class BacktestRequest
{
    public int TournamentId { get; set; } = 406;
    public DateTime FromDate { get; set; } = DateTime.UtcNow.AddMonths(-3);
    public DateTime ToDate { get; set; } = DateTime.UtcNow;
    public double ExperimentalGamma { get; set; } = 1.58;
    public bool IncludeB2 { get; set; } = true;
    public int SeasonLookback { get; set; } = 2;
}
