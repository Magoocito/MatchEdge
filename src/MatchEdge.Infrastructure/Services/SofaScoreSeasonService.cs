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
}
