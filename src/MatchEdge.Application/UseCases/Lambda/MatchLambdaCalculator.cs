using MatchEdge.Application.Configuration;
using MatchEdge.Domain.Models;
using Microsoft.Extensions.Options;

namespace MatchEdge.Application.UseCases.Lambda;

public class MatchLambdaCalculator : IMatchLambdaCalculator
{
    private readonly MatchModelOptions _options;

    public MatchLambdaCalculator(IOptions<MatchModelOptions> options)
    {
        _options = options.Value;
    }

    public (double lambdaHome, double lambdaAway) CalculateGoalLambdas(
        TeamStatistics home,
        TeamStatistics away)
    {
        if (home.Matches == 0)
            throw new ArgumentException("Home team has no matches played", nameof(home));

        if (away.Matches == 0)
            throw new ArgumentException("Away team has no matches played", nameof(away));

        double ataqueHome = (double)home.GoalsScored / home.Matches;
        double defensaHome = (double)home.GoalsConceded / home.Matches;
        double ataqueAway = (double)away.GoalsScored / away.Matches;
        double defensaAway = (double)away.GoalsConceded / away.Matches;

        double lambdaHome = ((ataqueHome + defensaAway) / 2) * _options.HomeAdvantageFactor;
        double lambdaAway = (ataqueAway + defensaHome) / 2;

        return (lambdaHome, lambdaAway);
    }
}
