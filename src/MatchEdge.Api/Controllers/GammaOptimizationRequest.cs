namespace MatchEdge.Api.Controllers;

public class GammaOptimizationRequest
{
    public int TournamentId { get; set; } = 406;
    public DateTime FromDate { get; set; } = DateTime.UtcNow.AddMonths(-3);
    public DateTime ToDate { get; set; } = DateTime.UtcNow;
    public double GammaMin { get; set; } = 1.0;
    public double GammaMax { get; set; } = 2.5;
    public double Step { get; set; } = 0.05;
    public int SeasonLookback { get; set; } = 2;
}
