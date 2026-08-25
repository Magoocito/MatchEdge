using MatchEdge.Domain.Matches;

namespace MatchEdge.Application.UseCases.Historical;

public record HistoricalMatch(FootballMatchEvent Event, int SeasonId, string Prefix);
