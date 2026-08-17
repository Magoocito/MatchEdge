using MatchEdge.Application.Clients;
using MatchEdge.Domain.Matches;
using MatchEdge.Domain.Models;
using MatchEdge.Domain.Teams;
using MatchEdge.Infrastructure.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MatchEdge.Infrastructure.Clients;

public class CachedSofaScoreClient : ISofaScoreClient
{
    private readonly ISofaScoreClient _inner;
    private readonly IMemoryCache _cache;
    private readonly MatchCacheTtlResolver _ttlResolver;
    private readonly SofaScoreCacheOptions _cacheOptions;
    private readonly ILogger<CachedSofaScoreClient> _logger;

    public CachedSofaScoreClient(
        ISofaScoreClient inner,
        IMemoryCache cache,
        MatchCacheTtlResolver ttlResolver,
        IOptions<SofaScoreCacheOptions> cacheOptions,
        ILogger<CachedSofaScoreClient> logger)
    {
        _inner = inner;
        _cache = cache;
        _ttlResolver = ttlResolver;
        _cacheOptions = cacheOptions.Value;
        _logger = logger;
    }

    public async Task<SofaScoreStatisticsResponse?> GetTeamStatisticsAsync(
        int teamId, int tournamentId, int seasonId)
    {
        var cacheKey = $"Stats:{teamId}:{tournamentId}:{seasonId}";

        if (_cache.TryGetValue(cacheKey, out SofaScoreStatisticsResponse? cached))
        {
            _logger.LogDebug("Cache HIT for {CacheKey}", cacheKey);
            return cached;
        }

        _logger.LogDebug("Cache MISS for {CacheKey}", cacheKey);
        var result = await _inner.GetTeamStatisticsAsync(teamId, tournamentId, seasonId);

        if (result != null)
            _cache.Set(cacheKey, result, TimeSpan.FromDays(_cacheOptions.FinishedMatchesTtlDays));

        return result;
    }

    public async Task<List<Team>?> GetTeamsAsync(int tournamentId, int seasonId)
    {
        var cacheKey = $"Teams:{tournamentId}:{seasonId}";

        if (_cache.TryGetValue(cacheKey, out List<Team>? cached))
        {
            _logger.LogDebug("Cache HIT for {CacheKey}", cacheKey);
            return cached;
        }

        _logger.LogDebug("Cache MISS for {CacheKey}", cacheKey);
        var result = await _inner.GetTeamsAsync(tournamentId, seasonId);

        if (result != null)
            _cache.Set(cacheKey, result, TimeSpan.FromDays(_cacheOptions.FinishedMatchesTtlDays));

        return result;
    }

    public async Task<MatchEventsResponse?> GetMatchEventsByRoundAsync(
        int tournamentId, int seasonId, int round, string prefix)
    {
        var cacheKey = $"Events:{tournamentId}:{seasonId}:{round}:{prefix}";

        if (_cache.TryGetValue(cacheKey, out MatchEventsResponse? cached))
        {
            _logger.LogDebug("Cache HIT for {CacheKey}", cacheKey);
            return cached;
        }

        _logger.LogDebug("Cache MISS for {CacheKey}", cacheKey);
        var result = await _inner.GetMatchEventsByRoundAsync(tournamentId, seasonId, round, prefix);

        if (result != null)
        {
            var ttl = _ttlResolver.Resolve(result, _cacheOptions);
            _cache.Set(cacheKey, result, ttl);
        }

        return result;
    }
}
