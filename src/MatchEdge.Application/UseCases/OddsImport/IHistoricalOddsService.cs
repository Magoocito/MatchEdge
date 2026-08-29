using MatchEdge.Domain.Odds;

namespace MatchEdge.Application.UseCases.OddsImport;

public interface IHistoricalOddsService
{
    IReadOnlyList<HistoricalOdds> GetAll();
    IReadOnlyList<HistoricalOdds> GetByDateRange(DateTime fromDate, DateTime toDate);
    IReadOnlyList<HistoricalOdds> GetByTournament(int tournamentId);
    void Load(IReadOnlyList<HistoricalOdds> odds);
}
