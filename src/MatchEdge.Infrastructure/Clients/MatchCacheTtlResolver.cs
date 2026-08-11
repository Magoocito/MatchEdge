using MatchEdge.Infrastructure.Configuration;
using MatchEdge.Domain.Matches;

namespace MatchEdge.Infrastructure.Clients;

public class MatchCacheTtlResolver
{
    public TimeSpan Resolve(MatchEventsResponse? response, SofaScoreCacheOptions options)
    {
        if (response?.Events == null || response.Events.Count == 0)
            return TimeSpan.FromHours(options.UpcomingMatchesTtlHours);

        if (response.Events.All(e =>
            string.Equals(e.Status.Type, "finished", StringComparison.OrdinalIgnoreCase)))
            return TimeSpan.FromDays(options.FinishedMatchesTtlDays);

        if (response.Events.Any(e =>
            string.Equals(e.Status.Type, "notstarted", StringComparison.OrdinalIgnoreCase)))
            return TimeSpan.FromHours(options.UpcomingMatchesTtlHours);

        return TimeSpan.FromMinutes(options.LiveMatchesTtlMinutes);
    }
}