namespace MatchEdge.Application.UseCases.Backtesting;

/// <summary>
/// Calculates Log-Loss (cross-entropy loss) for 1X2 match predictions.
/// 
/// Formula: LogLoss = -(1/N) * SUM[ln(probability_of_actual_result)]
/// 
/// Uses epsilon = 1e-15 to prevent log(0).
/// 
/// Lower is better.
/// Log-Loss penalizes heavily when the model assigns extremely low probability
/// to the outcome that actually occurs.
/// </summary>
public static class LogLossCalculator
{
    private const double Epsilon = 1e-15;

    public static double Calculate(IReadOnlyList<(double HomeWin, double Draw, double AwayWin, string Actual)> predictions)
    {
        if (predictions.Count == 0)
            return 0.0;

        double totalLogLoss = 0.0;

        foreach (var (homeWin, draw, awayWin, actual) in predictions)
        {
            double probability = actual switch
            {
                "H" => homeWin,
                "D" => draw,
                "A" => awayWin,
                _ => Epsilon
            };

            probability = Math.Max(probability, Epsilon);
            totalLogLoss += Math.Log(probability);
        }

        return -totalLogLoss / predictions.Count;
    }
}
