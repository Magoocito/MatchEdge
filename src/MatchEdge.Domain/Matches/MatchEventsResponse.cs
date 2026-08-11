namespace MatchEdge.Domain.Matches;

public class MatchEventsResponse
{
    public IReadOnlyList<FootballMatchEvent> Events { get; set; } = [];
    public bool HasNextPage { get; set; }
}
