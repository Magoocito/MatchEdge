namespace MatchEdge.Application.UseCases.OddsImport;

public interface ITeamMappingProvider
{
    IReadOnlyList<TeamMapping> GetAllTeamMappings();
}

public record TeamMapping(
    string Source,
    int SourceTeamId,
    string SourceTeamName,
    int SofaScoreTeamId,
    string SofaScoreTeamName,
    double Confidence);
