using System;

namespace MatchEdge.Application.UseCases.Backtesting;

/// <summary>
/// Defines the window for calibration data — used to prevent leakage.
/// Calibrations must be computed strictly from historical data, not including
/// the period being evaluated for model performance.
/// </summary>
public record CalibrationWindow(DateTime FromDate, DateTime ToDate)
{
    /// <summary>
    /// Validates that the calibration window is earlier than the evaluation period.
    /// Prevents calibration using data from the same matches being evaluated.
    /// </summary>
    public void EnsureNoOverlap(EvaluationWindow evaluation)
    {
        if (ToDate > evaluation.FromDate)
            throw new InvalidOperationException(
                $"Calibration window [{FromDate:yyyy-MM-dd}..{ToDate:yyyy-MM-dd}] " +
                $"must end before evaluation window [{evaluation.FromDate:yyyy-MM-dd}..{evaluation.ToDate:yyyy-MM-dd}]. " +
                $"Calibration using evaluated data is not allowed."
            );
    }

    /// <summary>
    /// Helper to create calibration window for a given year range.
    /// </summary>
    public static CalibrationWindow ForYears(int startYear, int endYear)
    {
        var fromDate = new DateTime(startYear, 1, 1);
        var toDate = new DateTime(endYear, 12, 31);
        return new CalibrationWindow(fromDate, toDate);
    }
}

/// <summary>
/// Defines the window for evaluating model performance.
/// Contains the matches against which calibrated models are tested.
/// </summary>
public record EvaluationWindow(DateTime FromDate, DateTime ToDate)
{
    /// <summary>
    /// Validates that the evaluation window starts after the calibration period ends.
    /// Ensures strict separation between calibration and evaluation data.
    /// </summary>
    public void EnsureNoOverlap(CalibrationWindow calibration)
    {
        if (FromDate <= calibration.ToDate)
            throw new InvalidOperationException(
                $"Evaluation window [{FromDate:yyyy-MM-dd}..{ToDate:yyyy-MM-dd}] " +
                $"must start after calibration window [{calibration.FromDate:yyyy-MM-dd}..{calibration.ToDate:yyyy-MM-dd}]. " +
                $"Evaluation using calibration data is not allowed."
            );
    }

    /// <summary>
    /// Helper to create evaluation window for a given year range.
    /// </summary>
    public static EvaluationWindow ForYears(int startYear, int endYear)
    {
        var fromDate = new DateTime(startYear, 1, 1);
        var toDate = new DateTime(endYear, 12, 31);
        return new EvaluationWindow(fromDate, toDate);
    }
}