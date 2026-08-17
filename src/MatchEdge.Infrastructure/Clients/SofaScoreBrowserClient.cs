using System.Text.Json;
using MatchEdge.Application.Clients;
using MatchEdge.Domain.Matches;
using MatchEdge.Domain.Models;
using MatchEdge.Domain.Teams;
using Microsoft.Extensions.Logging;

namespace MatchEdge.Infrastructure.Clients;

public class SofaScoreBrowserClient : ISofaScoreClient
{
    private readonly PlaywrightBrowserManager _browserManager;
    private readonly ILogger<SofaScoreBrowserClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public SofaScoreBrowserClient(
        PlaywrightBrowserManager browserManager,
        ILogger<SofaScoreBrowserClient> logger)
    {
        _browserManager = browserManager;
        _logger = logger;
    }

    public async Task<SofaScoreStatisticsResponse?> GetTeamStatisticsAsync(
        int teamId, int tournamentId, int seasonId)
    {
        var path = $"team/{teamId}/unique-tournament/{tournamentId}/season/{seasonId}/statistics/overall";
        var json = await FetchFromBrowserAsync(path);
        if (json == null) return null;
        return JsonSerializer.Deserialize<SofaScoreStatisticsResponse>(json, _jsonOptions);
    }

    public async Task<List<Team>?> GetTeamsAsync(int tournamentId, int seasonId)
    {
        var path = $"unique-tournament/{tournamentId}/season/{seasonId}/teams";
        var json = await FetchFromBrowserAsync(path);
        if (json == null) return null;
        var response = JsonSerializer.Deserialize<TeamsResponse>(json, _jsonOptions);
        return response?.Teams;
    }

    public async Task<MatchEventsResponse?> GetMatchEventsByRoundAsync(
        int tournamentId, int seasonId, int round, string prefix)
    {
        var path = $"unique-tournament/{tournamentId}/season/{seasonId}/events/round/{round}/prefix/{prefix}";
        var json = await FetchFromBrowserAsync(path);
        if (json == null) return null;
        return JsonSerializer.Deserialize<MatchEventsResponse>(json, _jsonOptions);
    }

    private async Task<string?> FetchFromBrowserAsync(string apiPath)
    {
        var page = _browserManager.GetPage();
        if (page == null)
        {
            _logger.LogWarning("Browser not started.");
            return null;
        }

        var baseUrl = "https://www.sofascore.com/api/v1/";
        var url = $"{baseUrl}{apiPath.TrimStart('/')}";
        _logger.LogInformation("Browser fetch: {Url}", url);

        try
        {
            var result = await page.EvaluateAsync<string>(@"
                async (url) => {
                    const resp = await fetch(url, {
                        credentials: 'include',
                        headers: { 'accept': 'application/json' }
                    });
                    if (!resp.ok) return JSON.stringify({ error: resp.status, statusText: resp.statusText });
                    return await resp.text();
                }", url);

            if (result != null && result.Contains("\"error\""))
            {
                _logger.LogWarning("API returned error: {Result}", result);
                return null;
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch {Path}", apiPath);
            return null;
        }
    }
}
