namespace MatchEdge.Infrastructure.Configuration
{
    public class SofaScoreCacheOptions
    {
        public int FinishedMatchesTtlDays { get; set; } = 30;
        public int UpcomingMatchesTtlHours { get; set; } = 1;
        public int LiveMatchesTtlMinutes { get; set; } = 2;
    }
}
