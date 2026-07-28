using MatchEdge.Application.UseCases.Calibration;
using Microsoft.AspNetCore.Mvc;

namespace MatchEdge.Api.Controllers;

[Route("api/calibration")]
[ApiController]
public class CalibrationController : ControllerBase
{
    private readonly IHomeAdvantageCalibrationService _homeAdvantageCalibrationService;

    public CalibrationController(IHomeAdvantageCalibrationService homeAdvantageCalibrationService)
    {
        _homeAdvantageCalibrationService = homeAdvantageCalibrationService;
    }

    [HttpGet("home-advantage")]
    public async Task<IActionResult> GetHomeAdvantage(
        [FromQuery] int tournamentId,
        [FromQuery] string prefix = "Apertura",
        [FromQuery] int fromRound = 1,
        [FromQuery] int toRound = 17)
    {
        var result = await _homeAdvantageCalibrationService.CalculateAsync(
            tournamentId,
            prefix,
            fromRound,
            toRound);

        return Ok(result);
    }
}
