using MatchEdge.Application.Clients;
using MatchEdge.Domain.Matches;
using MatchEdge.Domain.Models;
using MatchEdge.Domain.Teams;
using MatchEdge.Infrastructure.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace MatchEdge.Infrastructure.Clients
{
    public class SofaScoreClient : ISofaScoreClient
    {
        private readonly string _baseUrl;
        private readonly IHttpRequestExecutor _http;
        private readonly IMemoryCache _cache;
        private readonly MatchCacheTtlResolver _ttlResolver;
        private readonly SofaScoreCacheOptions _cacheOptions;

        public SofaScoreClient(
            IOptions<SofaScoreOptions> options,
            IHttpRequestExecutor http,
            IMemoryCache cache,
            MatchCacheTtlResolver ttlResolver,
            IOptions<SofaScoreCacheOptions> cacheOptions)
        {
            _baseUrl = options.Value.BaseUrl;
            _http = http;
            _cache = cache;
            _ttlResolver = ttlResolver;
            _cacheOptions = cacheOptions.Value;
        }

        public async Task<SofaScoreStatisticsResponse?> GetTeamStatisticsAsync(int teamId, int tournamentId, int seasonId)
        {
            var url = $"{_baseUrl}team/{teamId}/unique-tournament/{tournamentId}/season/{seasonId}/statistics/overall";

            var body = await _http.ExecuteCurlAsync(url);

            return JsonSerializer.Deserialize<SofaScoreStatisticsResponse>(
                body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        public async Task<List<Team>?> GetTeamsAsync(int tournamentId, int seasonId)
        {
            var url = $"{_baseUrl}unique-tournament/{tournamentId}/season/{seasonId}/teams";

            var body = await _http.ExecuteCurlAsync(url);

            var response = JsonSerializer.Deserialize<TeamsResponse>(
                body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return response?.Teams;
        }

        public async Task<MatchEventsResponse?> GetMatchEventsByRoundAsync(
            int tournamentId,
            int seasonId,
            int round,
            string prefix)
        {
            var cacheKey = $"Events:{tournamentId}:{seasonId}:{round}:{prefix}";

            if (_cache.TryGetValue(cacheKey, out MatchEventsResponse? cached))
                return cached;

            var url = $"{_baseUrl}unique-tournament/{tournamentId}/season/{seasonId}/events/round/{round}/prefix/{prefix}";

            var body = await _http.ExecuteCurlAsync(url);

            var response = JsonSerializer.Deserialize<MatchEventsResponse>(
                body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            var ttl = _ttlResolver.Resolve(response, _cacheOptions);
            _cache.Set(cacheKey, response, ttl);

            return response;
        }
    }
}
