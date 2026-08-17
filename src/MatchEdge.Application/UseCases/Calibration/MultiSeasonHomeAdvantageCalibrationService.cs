using MatchEdge.Application.Services;
using MatchEdge.Application.UseCases.Historical;

namespace MatchEdge.Application.UseCases.Calibration;

public class MultiSeasonHomeAdvantageCalibrationService : IMultiSeasonHomeAdvantageCalibrationService
{
    private readonly ISeasonService _seasonService;
    private readonly IHistoricalMatchEnumerator _matchEnumerator;

    public MultiSeasonHomeAdvantageCalibrationService(
        IHistoricalMatchEnumerator matchEnumerator,
        ISeasonService seasonService)
    {
        _matchEnumerator = matchEnumerator;
        _seasonService = seasonService;
    }

    public async Task<MultiSeasonHomeAdvantageCalibrationResult> CalculateAsync(
        int tournamentId,
        int seasonCount = 3,
        int fromRound = 1,
        int toRound = 17,
        DateTime? calibrationAsOf = null)
    {
        if (seasonCount <= 0)
            throw new ArgumentException("Season count must be greater than zero", nameof(seasonCount));

        if (fromRound <= 0)
            throw new ArgumentException("From round must be greater than zero", nameof(fromRound));

        if (toRound < fromRound)
            throw new ArgumentException("To round must be greater than or equal to from round", nameof(toRound));

        var prefixes = new[] { "Apertura", "Clausura" };

        var seasonIds = calibrationAsOf.HasValue
            ? await _seasonService.GetRecentSeasonIdsAsOfAsync(tournamentId, seasonCount, calibrationAsOf.Value)
            : await _seasonService.GetRecentSeasonIdsAsync(tournamentId, seasonCount);

        var finishedMatches = await _matchEnumerator.GetFinishedMatchesAsync(
            tournamentId, seasonIds, fromRound, toRound, prefixes);

        var allMatches = 0;
        var allHomeGoals = 0;
        var allAwayGoals = 0;
        var seasonDetails = new List<SeasonCalibrationDetail>();

        foreach (var seasonId in seasonIds)
        {
            foreach (var prefix in prefixes)
            {
                var seasonPrefixMatches = finishedMatches
                    .Where(m => m.SeasonId == seasonId && m.Prefix == prefix)
                    .ToList();

                var matches = seasonPrefixMatches.Count;
                var homeGoals = seasonPrefixMatches.Sum(m => m.Event.HomeScore.Current!.Value);
                var awayGoals = seasonPrefixMatches.Sum(m => m.Event.AwayScore.Current!.Value);

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
