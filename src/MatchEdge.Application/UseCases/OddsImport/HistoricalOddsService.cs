using MatchEdge.Domain.Odds;

namespace MatchEdge.Application.UseCases.OddsImport;

public class HistoricalOddsService : IHistoricalOddsService
{
    private List<HistoricalOdds> _odds = new();

    public IReadOnlyList<HistoricalOdds> GetAll() => _odds.AsReadOnly();

    public IReadOnlyList<HistoricalOdds> GetByDateRange(DateTime fromDate, DateTime toDate)
    {
        return _odds
            .Where(o => o.MatchDate >= fromDate && o.MatchDate <= toDate)
            .ToList()
            .AsReadOnly();
    }

    public IReadOnlyList<HistoricalOdds> GetByTournament(int tournamentId)
    {
        return _odds
            .Where(o => o.TournamentId == tournamentId)
            .ToList()
            .AsReadOnly();
    }

    public void Load(IReadOnlyList<HistoricalOdds> odds)
    {
        _odds = odds.ToList();
    }
}
