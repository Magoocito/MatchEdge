using MatchEdge.Application.Clients;
using MatchEdge.Application.Services;

namespace MatchEdge.Application.UseCases.Calibration;

public class MultiSeasonHomeAdvantageCalibrationService : IMultiSeasonHomeAdvantageCalibrationService
{
    private readonly ISofaScoreClient _sofaScoreClient;
    private readonly ISeasonService _seasonService;

    public MultiSeasonHomeAdvantageCalibrationService(
        ISofaScoreClient sofaScoreClient,
        ISeasonService seasonService)
    {
        _sofaScoreClient = sofaScoreClient;
        _seasonService = seasonService;
    }

    public async Task<MultiSeasonHomeAdvantageCalibrationResult> CalculateAsync(
        int tournamentId,
        int seasonCount = 3,
        int fromRound = 1,
        int toRound = 17)
    {
        if (seasonCount <= 0)
            throw new ArgumentException("Season count must be greater than zero", nameof(seasonCount));

        if (fromRound <= 0)
            throw new ArgumentException("From round must be greater than zero", nameof(fromRound));

        if (toRound < fromRound)
            throw new ArgumentException("To round must be greater than or equal to from round", nameof(toRound));

        var prefixes = new[] { "Apertura", "Clausura" };
        var seasonIds = await _seasonService.GetRecentSeasonIdsAsync(tournamentId, seasonCount);

        var allMatches = 0;
        var allHomeGoals = 0;
        var allAwayGoals = 0;
        var seasonDetails = new List<SeasonCalibrationDetail>();

        foreach (var seasonId in seasonIds)
        {
            foreach (var prefix in prefixes)
            {
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

                if (matches > 0 && awayGoals > 0)
                {
                    var avgHome = (double)homeGoals / matches;
                    var avgAway = (double)awayGoals / matches;
                    var factor = avgHome / avgAway;

                    var seasonName = await _seasonService.GetSeasonNameAsync(tournamentId, seasonId);

                    seasonDetails.Add(new SeasonCalibrationDetail(
                        seasonId,
                        seasonName,
                        prefix,
                        matches,
                        homeGoals,
                        awayGoals,
                        avgHome,
                        avgAway,
                        factor));

                    allMatches += matches;
                    allHomeGoals += homeGoals;
                    allAwayGoals += awayGoals;
                }
            }
        }

        if (allMatches == 0)
            throw new InvalidOperationException("No finished matches found for the selected range across all seasons");

        if (allAwayGoals == 0)
            throw new InvalidOperationException("Cannot calculate home advantage because away goals are zero");

        var overallAvgHome = (double)allHomeGoals / allMatches;
        var overallAvgAway = (double)allAwayGoals / allMatches;
        var overallFactor = overallAvgHome / overallAvgAway;

        return new MultiSeasonHomeAdvantageCalibrationResult(
            tournamentId,
            seasonCount,
            fromRound,
            toRound,
            allMatches,
            allHomeGoals,
            allAwayGoals,
            overallAvgHome,
            overallAvgAway,
            overallFactor,
            seasonDetails);
    }
}