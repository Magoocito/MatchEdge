using MatchEdge.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MatchEdge.Api.Controllers
{
    [Route("api/TeamStatistics")]
    [ApiController]
    public class TeamStatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;
        public TeamStatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTeamStatistics(
            [FromQuery] int teamId,
            [FromQuery] int tournamentId,
            [FromQuery] int seasonId)
        {
            var result = await _statisticsService.GetTeamStatisticsAsync(teamId, tournamentId, seasonId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
