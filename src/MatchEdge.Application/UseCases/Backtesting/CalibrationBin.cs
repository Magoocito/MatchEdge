namespace MatchEdge.Application.UseCases.Backtesting;

/// <summary>
/// A single bin in the calibration curve.
/// </summary>
public record CalibrationBin
{
    /// <summary>Midpoint of the predicted probability bin (e.g., 0.05 for bin [0.0, 0.1]).</summary>
    public double PredictedProbability { get; init; }

    /// <summary>Observed frequency of the outcome in this bin.</summary>
    public double ObservedFrequency { get; init; }

    /// <summary>Number of predictions in this bin.</summary>
    public int Count { get; init; }
}
