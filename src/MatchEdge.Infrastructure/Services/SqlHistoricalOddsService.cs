using MatchEdge.Application.UseCases.OddsImport;
using MatchEdge.Domain.Odds;
using MatchEdge.Infrastructure.Data;
using MatchEdge.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchEdge.Infrastructure.Services;

public class SqlHistoricalOddsService : IHistoricalOddsService
{
    private readonly MatchEdgeDbContext _db;

    public SqlHistoricalOddsService(MatchEdgeDbContext db)
    {
        _db = db;
    }

    public IReadOnlyList<HistoricalOdds> GetAll()
    {
        return _db.HistoricalOdds
            .AsNoTracking()
            .Select(e => ToDomain(e))
            .ToList()
            .AsReadOnly();
    }

    public IReadOnlyList<HistoricalOdds> GetByDateRange(DateTime fromDate, DateTime toDate)
    {
        return _db.HistoricalOdds
            .AsNoTracking()
            .Where(e => e.MatchDate >= fromDate && e.MatchDate <= toDate)
            .Select(e => ToDomain(e))
            .ToList()
            .AsReadOnly();
    }

    public IReadOnlyList<HistoricalOdds> GetByTournament(int tournamentId)
    {
        return _db.HistoricalOdds
            .AsNoTracking()
            .Where(e => e.TournamentId == tournamentId)
            .Select(e => ToDomain(e))
            .ToList()
            .AsReadOnly();
    }

    public void Load(IReadOnlyList<HistoricalOdds> odds)
    {
        var entities = odds.Select(ToEntity).ToList();

        foreach (var entity in entities)
        {
            var existing = _db.HistoricalOdds
                .FirstOrDefault(e => e.Source == entity.Source && e.SourceMatchId == entity.SourceMatchId);

            if (existing != null)
            {
                existing.MatchDate = entity.MatchDate;
                existing.TournamentId = entity.TournamentId;
                existing.Round = entity.Round;
                existing.HomeTeamId = entity.HomeTeamId;
                existing.HomeTeamName = entity.HomeTeamName;
                existing.AwayTeamId = entity.AwayTeamId;
                existing.AwayTeamName = entity.AwayTeamName;
                existing.HomeWinOdds = entity.HomeWinOdds;
                existing.DrawOdds = entity.DrawOdds;
                existing.AwayWinOdds = entity.AwayWinOdds;
            }
            else
            {
                _db.HistoricalOdds.Add(entity);
            }
        }

        _db.SaveChanges();
    }

    private static HistoricalOddsEntity ToEntity(HistoricalOdds odds) => new()
    {
        Source = "FootyStats",
        SourceMatchId = odds.MatchId,
        MatchDate = odds.MatchDate,
        TournamentId = odds.TournamentId,
        Round = odds.Round,
        HomeTeamId = odds.HomeTeamId,
        HomeTeamName = odds.HomeTeamName,
        AwayTeamId = odds.AwayTeamId,
        AwayTeamName = odds.AwayTeamName,
        HomeWinOdds = odds.HomeWinOdds,
        DrawOdds = odds.DrawOdds,
        AwayWinOdds = odds.AwayWinOdds,
        CreatedAt = DateTime.UtcNow
    };

    private static HistoricalOdds ToDomain(HistoricalOddsEntity entity) => new()
    {
        MatchId = entity.SourceMatchId,
        MatchDate = entity.MatchDate,
        TournamentId = entity.TournamentId,
        Round = entity.Round,
        HomeTeamId = entity.HomeTeamId,
        HomeTeamName = entity.HomeTeamName,
        AwayTeamId = entity.AwayTeamId,
        AwayTeamName = entity.AwayTeamName,
        HomeWinOdds = entity.HomeWinOdds,
        DrawOdds = entity.DrawOdds,
        AwayWinOdds = entity.AwayWinOdds
    };
}
