using MatchEdge.Application.UseCases.Predictions;
using Microsoft.AspNetCore.Mvc;

namespace MatchEdge.Api.Controllers;

[Route("api/predictions")]
[ApiController]
public class PredictionController : ControllerBase
{
    private readonly IMatchPredictionService _predictionService;

    public PredictionController(IMatchPredictionService predictionService)
    {
        _predictionService = predictionService;
    }

    [HttpGet("match")]
    public async Task<IActionResult> PredictMatch(
        [FromQuery] int homeTeamId,
        [FromQuery] int awayTeamId,
        [FromQuery] int tournamentId = 406)
    {
        var result = await _predictionService.PredictMatchAsync(homeTeamId, awayTeamId, tournamentId);

        if (result == null)
        {
            return NotFound(new { message = $"Could not generate prediction. Ensure team IDs {homeTeamId} and {awayTeamId} exist for tournament {tournamentId}." });
        }

        return Ok(result);
    }
}