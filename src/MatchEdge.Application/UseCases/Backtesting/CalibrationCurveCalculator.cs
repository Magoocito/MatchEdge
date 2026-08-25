namespace MatchEdge.Application.UseCases.Backtesting;

/// <summary>
/// Calculates calibration curves (reliability diagrams) for 1X2 match predictions.
/// 
/// Calibration measures whether predicted probabilities match observed frequencies.
/// A perfectly calibrated model would have predicted probability = observed frequency
/// for all bins (diagonal on the reliability diagram).
/// 
/// Expected Calibration Error (ECE) = SUM[|bin_count/N| * |observed - predicted|]
/// </summary>
public class CalibrationCurveCalculator : ICalibrationCurveCalculator
{
    public CalibrationResult Calculate(
        IReadOnlyList<(double HomeWin, double Draw, double AwayWin, string Actual)> predictions,
        int binCount = 10)
    {
        if (predictions.Count == 0)
            return new CalibrationResult { TotalMatches = 0 };

        var homeCurve = CalculateCurve(predictions, "H", p => p.HomeWin, binCount);
        var drawCurve = CalculateCurve(predictions, "D", p => p.Draw, binCount);
        var awayCurve = CalculateCurve(predictions, "A", p => p.AwayWin, binCount);

        var overallECE = (homeCurve.ExpectedCalibrationError +
                          drawCurve.ExpectedCalibrationError +
                          awayCurve.ExpectedCalibrationError) / 3.0;

        return new CalibrationResult
        {
            HomeWin = homeCurve,
            Draw = drawCurve,
            AwayWin = awayCurve,
            OverallECE = overallECE,
            TotalMatches = predictions.Count
        };
    }

    private static CalibrationCurve CalculateCurve(
        IReadOnlyList<(double HomeWin, double Draw, double AwayWin, string Actual)> predictions,
        string outcome,
        Func<(double HomeWin, double Draw, double AwayWin, string Actual), double> probSelector,
        int binCount)
    {
        var bins = new List<CalibrationBin>();
        var binWidth = 1.0 / binCount;

        for (var i = 0; i < binCount; i++)
        {
            var binLower = i * binWidth;
            var binUpper = (i + 1) * binWidth;
            var binMidpoint = (binLower + binUpper) / 2.0;

            var binPredictions = predictions
                .Where(p =>
                {
                    var prob = probSelector(p);
                    if (i == binCount - 1)
                        return prob >= binLower && prob <= binUpper;
                    return prob >= binLower && prob < binUpper;
                })
                .ToList();

            if (binPredictions.Count == 0)
            {
                bins.Add(new CalibrationBin
                {
                    PredictedProbability = binMidpoint,
                    ObservedFrequency = 0,
                    Count = 0
                });
                continue;
            }

            var predictedAvg = binPredictions.Average(probSelector);
            var observedFreq = binPredictions.Average(p => p.Actual == outcome ? 1.0 : 0.0);

            bins.Add(new CalibrationBin
            {
                PredictedProbability = predictedAvg,
                ObservedFrequency = observedFreq,
                Count = binPredictions.Count
            });
        }

        var n = predictions.Count;
        var ece = bins.Sum(b => (double)b.Count / n * Math.Abs(b.ObservedFrequency - b.PredictedProbability));

        var brierPreds = predictions.Select(p => (probSelector(p), p.Actual == outcome ? 1.0 : 0.0)).ToList();
        var brierScore = brierPreds.Average(x => Math.Pow(x.Item1 - x.Item2, 2));

        return new CalibrationCurve
        {
            Outcome = outcome,
            Bins = bins,
            ExpectedCalibrationError = ece,
            BrierScore = brierScore
        };
    }
}
