using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using MatchEdge.Infrastructure.Data;
using MatchEdge.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchEdge.Infrastructure.Services;

public class OddsMatchingService : IOddsMatchingService
{
    private readonly MatchEdgeDbContext _db;

    public OddsMatchingService(MatchEdgeDbContext db)
    {
        _db = db;
    }

    public string NormalizeTeamName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        var normalized = name.ToLowerInvariant().Trim();

        normalized = normalized.Replace("á", "a").Replace("é", "e").Replace("í", "i")
            .Replace("ó", "o").Replace("ú", "u").Replace("ñ", "n");

        normalized = Regex.Replace(normalized, @"\s+", " ");

        return normalized;
    }

    public IReadOnlyList<TeamMappingEntity> GetAllTeamMappings()
    {
        return _db.TeamMappings
            .AsNoTracking()
            .ToList()
            .AsReadOnly();
    }

    public TeamMappingEntity? FindTeamMapping(string source, string teamName)
    {
        var normalized = NormalizeTeamName(teamName);
        return _db.TeamMappings
            .AsNoTracking()
            .FirstOrDefault(m => m.Source == source &&
                (NormalizeTeamName(m.SourceTeamName) == normalized ||
                 NormalizeTeamName(m.SofaScoreTeamName) == normalized));
    }

    public void SaveTeamMapping(string source, int sourceTeamId, string sourceName,
        int sofaScoreTeamId, string sofaScoreName, double confidence = 1.0)
    {
        var existing = _db.TeamMappings
            .FirstOrDefault(m => m.Source == source && m.SourceTeamId == sourceTeamId);

        if (existing != null)
        {
            existing.SofaScoreTeamId = sofaScoreTeamId;
            existing.SofaScoreTeamName = sofaScoreName;
            existing.Confidence = confidence;
        }
        else
        {
            _db.TeamMappings.Add(new TeamMappingEntity
            {
                Source = source,
                SourceTeamId = sourceTeamId,
                SourceTeamName = sourceName,
                SofaScoreTeamId = sofaScoreTeamId,
                SofaScoreTeamName = sofaScoreName,
                Confidence = confidence,
                CreatedAt = DateTime.UtcNow
            });
        }

        _db.SaveChanges();
    }

    public void DeleteTeamMapping(int id)
    {
        var entity = _db.TeamMappings.Find(id);
        if (entity != null)
        {
            _db.TeamMappings.Remove(entity);
            _db.SaveChanges();
        }
    }

    public IReadOnlyList<MatchMappingEntity> GetAllMatchMappings()
    {
        return _db.MatchMappings
            .AsNoTracking()
            .ToList()
            .AsReadOnly();
    }

    public MatchMappingEntity? FindMatchMapping(string source, int sourceMatchId)
    {
        return _db.MatchMappings
            .AsNoTracking()
            .FirstOrDefault(m => m.Source == source && m.SourceMatchId == sourceMatchId);
    }

    public void SaveMatchMapping(string source, int sourceMatchId, int sofaScoreEventId,
        DateTime matchDate, double confidence = 1.0)
    {
        var existing = _db.MatchMappings
            .FirstOrDefault(m => m.Source == source && m.SourceMatchId == sourceMatchId);

        if (existing != null)
        {
            existing.SofaScoreEventId = sofaScoreEventId;
            existing.MatchConfidence = confidence;
        }
        else
        {
            _db.MatchMappings.Add(new MatchMappingEntity
            {
                Source = source,
                SourceMatchId = sourceMatchId,
                SofaScoreEventId = sofaScoreEventId,
                MatchDate = matchDate,
                MatchConfidence = confidence,
                CreatedAt = DateTime.UtcNow
            });
        }

        _db.SaveChanges();
    }

    public void DeleteMatchMapping(int id)
    {
        var entity = _db.MatchMappings.Find(id);
        if (entity != null)
        {
            _db.MatchMappings.Remove(entity);
            _db.SaveChanges();
        }
    }
}

public interface IOddsMatchingService
{
    string NormalizeTeamName(string name);
    IReadOnlyList<TeamMappingEntity> GetAllTeamMappings();
    TeamMappingEntity? FindTeamMapping(string source, string teamName);
    void SaveTeamMapping(string source, int sourceTeamId, string sourceName,
        int sofaScoreTeamId, string sofaScoreName, double confidence = 1.0);
    void DeleteTeamMapping(int id);
    IReadOnlyList<MatchMappingEntity> GetAllMatchMappings();
    MatchMappingEntity? FindMatchMapping(string source, int sourceMatchId);
    void SaveMatchMapping(string source, int sourceMatchId, int sofaScoreEventId,
        DateTime matchDate, double confidence = 1.0);
    void DeleteMatchMapping(int id);
}
