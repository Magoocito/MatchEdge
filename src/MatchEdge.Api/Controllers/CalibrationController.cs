using MatchEdge.Application.UseCases.Calibration;
using Microsoft.AspNetCore.Mvc;

namespace MatchEdge.Api.Controllers;

[Route("api/calibration")]
[ApiController]
public class CalibrationController : ControllerBase
{
    private readonly IHomeAdvantageCalibrationService _homeAdvantageCalibrationService;
    private readonly IMultiSeasonHomeAdvantageCalibrationService _multiSeasonHomeAdvantageCalibrationService;

    public CalibrationController(
        IHomeAdvantageCalibrationService homeAdvantageCalibrationService,
        IMultiSeasonHomeAdvantageCalibrationService multiSeasonHomeAdvantageCalibrationService)
    {
        _homeAdvantageCalibrationService = homeAdvantageCalibrationService;
        _multiSeasonHomeAdvantageCalibrationService = multiSeasonHomeAdvantageCalibrationService;
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

    [HttpGet("home-advantage/multi-season")]
    public async Task<IActionResult> GetMultiSeasonHomeAdvantage(
        [FromQuery] int tournamentId,
        [FromQuery] int seasonCount = 3,
        [FromQuery] int fromRound = 1,
        [FromQuery] int toRound = 17)
    {
        var result = await _multiSeasonHomeAdvantageCalibrationService.CalculateAsync(
            tournamentId,
            seasonCount,
            fromRound,
            toRound);

        return Ok(result);
    }
}
