using MatchEdge.Application.Services;
using MatchEdge.Infrastructure.Clients;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace MatchEdge.Infrastructure.Services;

public class SofaScoreSeasonService : ISeasonService
{
    private readonly IHttpRequestExecutor _http;
    private readonly IMemoryCache _cache;
    private readonly string _baseUrl;

    public SofaScoreSeasonService(
        IHttpRequestExecutor http,
        IMemoryCache cache,
        IConfiguration configuration)
    {
        _http = http;
        _cache = cache;
        _baseUrl = configuration["SofaScore:BaseUrl"]!;
    }

    public async Task<int> GetCurrentSeasonAsync(int tournamentId)
    {
        var cacheKey = $"Season:{tournamentId}";

        if (_cache.TryGetValue(cacheKey, out int cachedSeasonId))
        {
            return cachedSeasonId;
        }

        var url = $"{_baseUrl}unique-tournament/{tournamentId}/seasons";
        var json = await _http.ExecuteCurlAsync(url);

        var response = JsonSerializer.Deserialize<SeasonResponse>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var currentSeason = response?.Seasons?.FirstOrDefault()
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

        var asOfUtc = ToUtc(asOfDateTime);
        var cacheKey = $"Seasons:{tournamentId}:{count}:{asOfUtc:yyyy-MM-dd}";

        if (_cache.TryGetValue(cacheKey, out List<int> cachedSeasonIds))
        {
            return cachedSeasonIds;
        }

        var url = $"{_baseUrl}unique-tournament/{tournamentId}/seasons";
        var json = await _http.ExecuteCurlAsync(url);

        var response = JsonSerializer.Deserialize<SeasonResponse>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var recentSeasonIds = response?.Seasons?
            .Where(season => TryGetSeasonStartUtc(season, out var seasonStart) && seasonStart < asOfUtc)
            .OrderByDescending(season => season.Year)
            .ThenByDescending(season => season.Id)
            .Take(count)
            .Select(season => season.Id)
            .ToList() ?? [];

        if (recentSeasonIds.Count == 0)
            throw new Exception($"No seasons found for tournament {tournamentId} as of {asOfUtc:O}");

        _cache.Set(cacheKey, recentSeasonIds, TimeSpan.FromHours(12));

        return recentSeasonIds;
    }

    private static bool TryGetSeasonStartUtc(SeasonInfo season, out DateTime seasonStartUtc)
    {
        if (season.StartTimestamp > 0)
        {
            seasonStartUtc = DateTimeOffset.FromUnixTimeSeconds(season.StartTimestamp).UtcDateTime;
            return true;
        }

        if (int.TryParse(season.Year, out var year))
        {
            seasonStartUtc = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return true;
        }

        seasonStartUtc = default;
        return false;
    }

    private static DateTime ToUtc(DateTime value) =>
        value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

    public async Task<string> GetSeasonNameAsync(int tournamentId, int seasonId)
    {
        var cacheKey = $"SeasonName:{tournamentId}:{seasonId}";

        if (_cache.TryGetValue(cacheKey, out string cachedName))
        {
            return cachedName;
        }

        var url = $"{_baseUrl}unique-tournament/{tournamentId}/seasons";
        var json = await _http.ExecuteCurlAsync(url);

        var response = JsonSerializer.Deserialize<SeasonResponse>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var season = response?.Seasons?.FirstOrDefault(s => s.Id == seasonId);
        var name = season?.Name ?? seasonId.ToString();

        _cache.Set(cacheKey, name, TimeSpan.FromHours(12));

        return name;
    }
}

public class SeasonResponse
{
    public List<SeasonInfo>? Seasons { get; set; }
}

public class SeasonInfo
{
    public string Name { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public int Id { get; set; }
    public int StartTimestamp { get; set; }
}
