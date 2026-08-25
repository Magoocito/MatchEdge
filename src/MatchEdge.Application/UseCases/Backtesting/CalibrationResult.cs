namespace MatchEdge.Application.UseCases.Backtesting;

/// <summary>
/// Complete calibration analysis result for a model.
/// Contains calibration curves for all three outcomes (H, D, A).
/// </summary>
public record CalibrationResult
{
    /// <summary>Calibration curve for Home Win predictions.</summary>
    public CalibrationCurve HomeWin { get; init; } = new();

    /// <summary>Calibration curve for Draw predictions.</summary>
    public CalibrationCurve Draw { get; init; } = new();

    /// <summary>Calibration curve for Away Win predictions.</summary>
    public CalibrationCurve AwayWin { get; init; } = new();

    /// <summary>Overall Expected Calibration Error (macro-average of the three outcomes).</summary>
    public double OverallECE { get; init; }

    /// <summary>Total number of matches analyzed.</summary>
    public int TotalMatches { get; init; }
}
