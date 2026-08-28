using MatchEdge.Domain.Odds;

namespace MatchEdge.Application.UseCases.OddsImport;

public interface ICsvOddsParser
{
    IReadOnlyList<HistoricalOdds> Parse(string csvContent);
}
