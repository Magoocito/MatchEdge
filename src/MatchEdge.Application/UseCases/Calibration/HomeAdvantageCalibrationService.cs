using MatchEdge.Application.Clients;
using MatchEdge.Application.Services;

namespace MatchEdge.Application.UseCases.Calibration;

public class HomeAdvantageCalibrationService : IHomeAdvantageCalibrationService
{
    private readonly ISofaScoreClient _sofaScoreClient;
    private readonly ISeasonService _seasonService;

    public HomeAdvantageCalibrationService(
        ISofaScoreClient sofaScoreClient,
        ISeasonService seasonService)
    {
        _sofaScoreClient = sofaScoreClient;
        _seasonService = seasonService;
    }

    public async Task<HomeAdvantageCalibrationResult> CalculateAsync(
        int tournamentId,
        string prefix,
        int fromRound,
        int toRound)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            throw new ArgumentException("Prefix is required", nameof(prefix));

        if (fromRound <= 0)
            throw new ArgumentException("From round must be greater than zero", nameof(fromRound));

        if (toRound < fromRound)
            throw new ArgumentException("To round must be greater than or equal to from round", nameof(toRound));

        var seasonId = await _seasonService.GetCurrentSeasonAsync(tournamentId);
        var matches = 0;
        var homeGoals = 0;
        var awayGoals = 0;

        for (var round = fromRound; round <= toRound; round++)
        {
            var response = await _sofaScoreClient.GetMatchEventsByRoundAsync(
                tournamentId,
                seasonId,
                round,
                prefix);

            var finishedEvents = response?.Events
                .Where(match => match.Status.Type == "finished")
                .Where(match => match.HomeScore.Current.HasValue && match.AwayScore.Current.HasValue)
                ?? [];

            foreach (var match in finishedEvents)
            {
                matches++;
                homeGoals += match.HomeScore.Current!.Value;
                awayGoals += match.AwayScore.Current!.Value;
            }
        }

        if (matches == 0)
            throw new InvalidOperationException("No finished matches found for the selected range");

        if (awayGoals == 0)
            throw new InvalidOperationException("Cannot calculate home advantage because away goals are zero");

        var averageHomeGoals = (double)homeGoals / matches;
        var averageAwayGoals = (double)awayGoals / matches;
        var homeAdvantageFactor = averageHomeGoals / averageAwayGoals;

        return new HomeAdvantageCalibrationResult(
            tournamentId,
            seasonId,
            prefix,
            fromRound,
            toRound,
            matches,
            homeGoals,
            awayGoals,
            averageHomeGoals,
            averageAwayGoals,
            homeAdvantageFactor);
    }
}
