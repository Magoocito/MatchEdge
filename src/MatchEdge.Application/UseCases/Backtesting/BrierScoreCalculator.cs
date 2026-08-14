namespace MatchEdge.Application.UseCases.Backtesting;

/// <summary>
/// Calculates multi-class Brier Score for 1X2 match predictions.
/// 
/// Formula: BS = (1/N) * SUM[SUM[(p_i - o_i)^2]]
/// 
/// For each match, we compute the squared difference between predicted probability
/// and actual outcome (1.0 for the realized result, 0.0 for others) across all
/// three outcomes (Home, Draw, Away), then average across all matches.
/// 
/// Result range: 0.0 to 2.0 (NOT divided by 2).
/// Lower is better.
/// Example: BS 0.18 is better than 0.22.
/// </summary>
public static class BrierScoreCalculator
{
    public static double Calculate(IReadOnlyList<(double HomeWin, double Draw, double AwayWin, string Actual)> predictions)
    {
        if (predictions.Count == 0)
            return 0.0;

        double totalScore = 0.0;

        foreach (var (homeWin, draw, awayWin, actual) in predictions)
        {
            double oHome = actual == "H" ? 1.0 : 0.0;
            double oDraw = actual == "D" ? 1.0 : 0.0;
            double oAway = actual == "A" ? 1.0 : 0.0;

            totalScore += Math.Pow(homeWin - oHome, 2);
            totalScore += Math.Pow(draw - oDraw, 2);
            totalScore += Math.Pow(awayWin - oAway, 2);
        }

        return totalScore / predictions.Count;
    }
}
