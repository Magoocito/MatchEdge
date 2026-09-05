namespace MatchEdge.Application.UseCases.OddsImport;

public interface IMatchMappingProvider
{
    IReadOnlyList<MatchMapping> GetAllMatchMappings();
}

public record MatchMapping(
    string Source,
    int SourceMatchId,
    int SofaScoreEventId,
    DateTime MatchDate,
    double MatchConfidence);
