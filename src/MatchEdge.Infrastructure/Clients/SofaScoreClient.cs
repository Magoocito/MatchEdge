using MatchEdge.Application.Clients;
using MatchEdge.Domain.Models;
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
    }
}
