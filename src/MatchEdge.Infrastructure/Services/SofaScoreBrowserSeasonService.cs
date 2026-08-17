using MatchEdge.Application.Clients;
using MatchEdge.Application.Services;
using MatchEdge.Infrastructure.Clients;
using MatchEdge.Infrastructure.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace MatchEdge.Infrastructure.Services;

public class SofaScoreBrowserSeasonService : ISeasonService
{
    private readonly ISofaScoreBrowserCollector _collector;
    private readonly IMemoryCache _cache;
    private readonly ILogger<SofaScoreBrowserSeasonService> _logger;

    public SofaScoreBrowserSeasonService(
        ISofaScoreBrowserCollector collector,
        IMemoryCache cache,
        ILogger<SofaScoreBrowserSeasonService> logger)
    {
        _collector = collector;
        _cache = cache;
        _logger = logger;
    }

    public async Task<int> GetCurrentSeasonAsync(int tournamentId)
    {
        var cacheKey = $"Season:{tournamentId}";

        if (_cache.TryGetValue(cacheKey, out int cachedSeasonId))
            return cachedSeasonId;

        var seasons = await FetchSeasonsAsync(tournamentId);
        var currentSeason = seasons.FirstOrDefault()
            ?? throw new Exception($"No seasons found for tournament {tournamentId}");

        _cache.Set(cacheKey, currentSeason.Id, TimeSpan.FromHours(12));
        return currentSeason.Id;
    }

    public async Task<List<int>> GetRecentSeasonIdsAsync(int tournamentId, int count)
        => await GetRecentSeasonIdsAsOfAsync(tournamentId, count, DateTime.UtcNow);

    public async Task<List<int>> GetRecentSeasonIdsAsOfAsync(int tournamentId, int count, DateTime asOfDateTime)
    {
        if (count <= 0)
            throw new ArgumentException("Count must be greater than zero", nameof(count));

        var asOfUtc = asOfDateTime.Kind == DateTimeKind.Utc
            ? asOfDateTime
            : asOfDateTime.ToUniversalTime();

        var cacheKey = $"Seasons:{tournamentId}:{count}:{asOfUtc:yyyy-MM-dd}";

        if (_cache.TryGetValue(cacheKey, out List<int> cachedSeasonIds))
            return cachedSeasonIds;

        var seasons = await FetchSeasonsAsync(tournamentId);

        var recentSeasonIds = seasons
            .Where(s =>
            {
                if (s.StartTimestamp > 0)
                    return DateTimeOffset.FromUnixTimeSeconds(s.StartTimestamp).UtcDateTime < asOfUtc;

                if (int.TryParse(s.Year, out var year))
                    return new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc) < asOfUtc;

                return false;
            })
            .OrderByDescending(s => s.Year)
            .ThenByDescending(s => s.Id)
            .Take(count)
            .Select(s => s.Id)
            .ToList();

        if (recentSeasonIds.Count == 0)
            throw new Exception($"No seasons found for tournament {tournamentId} as of {asOfUtc:O}");

        _cache.Set(cacheKey, recentSeasonIds, TimeSpan.FromHours(12));
        return recentSeasonIds;
    }

    public async Task<string> GetSeasonNameAsync(int tournamentId, int seasonId)
    {
        var cacheKey = $"SeasonName:{tournamentId}:{seasonId}";

        if (_cache.TryGetValue(cacheKey, out string cachedName))
            return cachedName;

        var seasons = await FetchSeasonsAsync(tournamentId);
        var season = seasons.FirstOrDefault(s => s.Id == seasonId);
        var name = season?.Name ?? seasonId.ToString();

        _cache.Set(cacheKey, name, TimeSpan.FromHours(12));
        return name;
    }

    private async Task<List<SeasonInfo>> FetchSeasonsAsync(int tournamentId)
    {
        var apiPath = $"unique-tournament/{tournamentId}/seasons";
        var json = await _collector.FetchJsonAsync(apiPath);

        if (string.IsNullOrEmpty(json))
            throw new Exception($"Failed to fetch seasons for tournament {tournamentId}");

        var response = JsonSerializer.Deserialize<SeasonResponse>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return response?.Seasons ?? throw new Exception($"Invalid seasons response for tournament {tournamentId}");
    }
}
