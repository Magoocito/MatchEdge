using MatchEdge.Application.Clients;
using MatchEdge.Domain.Matches;
using MatchEdge.Domain.Models;
using MatchEdge.Domain.Teams;
using MatchEdge.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;

namespace MatchEdge.Infrastructure.Clients
{
    public class SofaScoreClient : ISofaScoreClient
    {
        private readonly string _baseUrl;
        private readonly IHttpRequestExecutor _http;

        public SofaScoreClient(IOptions<SofaScoreOptions> options, IHttpRequestExecutor http)
        {
            _baseUrl = options.Value.BaseUrl;
            _http = http;
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
            var url = $"{_baseUrl}unique-tournament/{tournamentId}/season/{seasonId}/events/round/{round}/prefix/{prefix}";

            var body = await _http.ExecuteCurlAsync(url);

            return JsonSerializer.Deserialize<MatchEventsResponse>(
                body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
    }

    internal class TeamsResponse
    {
        public List<Team>? Teams { get; set; }
    }
}
