namespace MatchEdge.Domain.Matches;

public class MatchEventsResponse
{
    public List<FootballMatchEvent> Events { get; set; } = [];
    public bool HasNextPage { get; set; }
}
