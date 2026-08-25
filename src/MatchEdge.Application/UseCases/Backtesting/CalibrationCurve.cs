namespace MatchEdge.Application.UseCases.Backtesting;

/// <summary>
/// Calibration curve for a single outcome (H, D, or A).
/// Shows how predicted probabilities relate to observed frequencies.
/// </summary>
public record CalibrationCurve
{
    /// <summary>Outcome label ("H", "D", or "A").</summary>
    public string Outcome { get; init; } = string.Empty;

    /// <summary>Bins for this calibration curve.</summary>
    public IReadOnlyList<CalibrationBin> Bins { get; init; } = [];

    /// <summary>Expected Calibration Error (ECE) for this outcome.</summary>
    public double ExpectedCalibrationError { get; init; }

    /// <summary>Brier Score for this outcome.</summary>
    public double BrierScore { get; init; }
}
