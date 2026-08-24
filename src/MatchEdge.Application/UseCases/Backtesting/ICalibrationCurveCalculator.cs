namespace MatchEdge.Application.UseCases.Backtesting;

/// <summary>
/// Calculates calibration curves (reliability diagrams) for 1X2 match predictions.
/// Groups predictions into probability bins and compares predicted vs observed frequencies.
/// </summary>
public interface ICalibrationCurveCalculator
{
    /// <summary>
    /// Calculates calibration curves for the given predictions.
    /// </summary>
    /// <param name="predictions">List of (HomeWin, Draw, AwayWin, Actual) tuples.</param>
    /// <param name="binCount">Number of bins (default 10 for decile calibration).</param>
    /// <returns>Calibration curves for H, D, and A outcomes.</returns>
    CalibrationResult Calculate(
        IReadOnlyList<(double HomeWin, double Draw, double AwayWin, string Actual)> predictions,
        int binCount = 10);
}
