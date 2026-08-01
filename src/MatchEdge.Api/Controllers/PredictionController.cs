using MatchEdge.Application.UseCases.Predictions;
using MatchEdge.Application.UseCases.ValueBetting;
using Microsoft.AspNetCore.Mvc;

namespace MatchEdge.Api.Controllers;

[Route("api/predictions")]
[ApiController]
public class PredictionController : ControllerBase
{
    private readonly IMatchPredictionService _predictionService;
    private readonly IValueBetCalculator _valueBetCalculator;

    public PredictionController(IMatchPredictionService predictionService,
        IValueBetCalculator valueBetCalculator)
    {
        _predictionService = predictionService;
        _valueBetCalculator = valueBetCalculator;
    }

    [HttpGet("match")]
    public async Task<IActionResult> PredictMatch(
        [FromQuery] int homeTeamId,
        [FromQuery] int awayTeamId,
        [FromQuery] int tournamentId = 406,
        [FromQuery] double? oddsHomeWin = null,
        [FromQuery] double? oddsDraw = null,
        [FromQuery] double? oddsAwayWin = null,
        [FromQuery] double? oddsOver2_5 = null,
        [FromQuery] double? oddsUnder2_5 = null,
        [FromQuery] double? oddsBttsYes = null,
        [FromQuery] double? oddsBttsNo = null)
    {
        var prediction = await _predictionService.PredictMatchAsync(homeTeamId, awayTeamId, tournamentId);

        if (prediction == null)
        {
            return NotFound(new { message = $"Could not generate prediction. Ensure team IDs {homeTeamId} and {awayTeamId} exist for tournament {tournamentId}." });
        }

        var valueBets = BuildValueBets(prediction, oddsHomeWin, oddsDraw, oddsAwayWin, oddsOver2_5, oddsUnder2_5, oddsBttsYes, oddsBttsNo);

        if (valueBets != null)
        {
            prediction = prediction with { ValueBets = valueBets };
        }

        return Ok(prediction);
    }

    private List<ValueBetAnalysis>? BuildValueBets(
    MatchPredictionResult prediction,
    double? oddsHomeWin,
    double? oddsDraw,
    double? oddsAwayWin,
    double? oddsOver2_5,
    double? oddsUnder2_5,
    double? oddsBttsYes,
    double? oddsBttsNo)
    {
        if (oddsHomeWin == null && oddsDraw == null && oddsAwayWin == null &&
            oddsOver2_5 == null && oddsUnder2_5 == null &&
            oddsBttsYes == null && oddsBttsNo == null)
        {
            return null;
        }

        var result = new List<ValueBetAnalysis>();

        if (oddsHomeWin.HasValue)
            result.Add(_valueBetCalculator.Analyze("HomeWin", prediction.MatchResultProbabilities.HomeWin, oddsHomeWin.Value));
        if (oddsDraw.HasValue)
            result.Add(_valueBetCalculator.Analyze("Draw", prediction.MatchResultProbabilities.Draw, oddsDraw.Value));
        if (oddsAwayWin.HasValue)
            result.Add(_valueBetCalculator.Analyze("AwayWin", prediction.MatchResultProbabilities.AwayWin, oddsAwayWin.Value));
        if (oddsOver2_5.HasValue)
            result.Add(_valueBetCalculator.Analyze("Over2.5", prediction.OverUnderProbabilities.Over2_5, oddsOver2_5.Value));
        if (oddsUnder2_5.HasValue)
            result.Add(_valueBetCalculator.Analyze("Under2.5", prediction.OverUnderProbabilities.Under2_5, oddsUnder2_5.Value));
        if (oddsBttsYes.HasValue)
            result.Add(_valueBetCalculator.Analyze("BttsYes", prediction.BttsProbabilities.Yes, oddsBttsYes.Value));
        if (oddsBttsNo.HasValue)
            result.Add(_valueBetCalculator.Analyze("BttsNo", prediction.BttsProbabilities.No, oddsBttsNo.Value));

        return result;
    }
}