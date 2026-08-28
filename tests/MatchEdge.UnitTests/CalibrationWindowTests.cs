using Xunit;
using MatchEdge.Application.UseCases.Backtesting;

namespace MatchEdge.UnitTests;

public class CalibrationWindowTests
{
    [Fact]
    public void EnsureNoOverlap_Throws_WhenCalibrationAfterEvaluation()
    {
        // Test case: Calibration window [2023-01-01..2023-12-31], Evaluation window [2023-01-01..2024-12-31]
        // (calibration TO date = evaluation FROM date)
        var calibration = new CalibrationWindow(new DateTime(2023, 1, 1), new DateTime(2023, 12, 31));
        var evaluation = new EvaluationWindow(new DateTime(2023, 1, 1), new DateTime(2024, 12, 31));

        var exception = Assert.Throws<InvalidOperationException>(
            () => calibration.EnsureNoOverlap(evaluation)
        );

        Assert.Contains("Calibration window", exception.Message);
        Assert.Contains("must end before evaluation window", exception.Message);
    }

    [Fact]
    public void EnsureNoOverlap_Passes_WhenCalibrationBeforeEvaluation()
    {
        // Test case: Calibration window [2022-01-01..2023-12-31], Evaluation window [2024-01-01..2025-12-31]
        // (calibration TO date < evaluation FROM date)
        var calibration = new CalibrationWindow(new DateTime(2022, 1, 1), new DateTime(2023, 12, 31));
        var evaluation = new EvaluationWindow(new DateTime(2024, 1, 1), new DateTime(2025, 12, 31));

        // This should NOT throw
        calibration.EnsureNoOverlap(evaluation);
    }

    [Fact]
    public void EvaluationWindow_EnsureNoOverlap_Throws_WhenEvaluationBeforeCalibration()
    {
        // Test case: Calibration window [2023-01-01..2024-12-31], Evaluation window [2022-01-01..2023-12-31]
        // (evaluation FROM date <= calibration TO date)
        var calibration = new CalibrationWindow(new DateTime(2023, 1, 1), new DateTime(2024, 12, 31));
        var evaluation = new EvaluationWindow(new DateTime(2022, 1, 1), new DateTime(2023, 12, 31));

        var exception = Assert.Throws<InvalidOperationException>(
            () => evaluation.EnsureNoOverlap(calibration)
        );

        Assert.Contains("Evaluation window", exception.Message);
        Assert.Contains("must start after calibration window", exception.Message);
    }

    [Fact]
    public void ForYears_CreatesCorrectWindow()
    {
        // Test that the helper method creates windows with correct boundaries
        var calWindow = CalibrationWindow.ForYears(2023, 2024);
        var evalWindow = EvaluationWindow.ForYears(2025, 2026);

        Assert.Equal(new DateTime(2023, 1, 1), calWindow.FromDate);
        Assert.Equal(new DateTime(2024, 12, 31), calWindow.ToDate);
        Assert.Equal(new DateTime(2025, 1, 1), evalWindow.FromDate);
        Assert.Equal(new DateTime(2026, 12, 31), evalWindow.ToDate);
    }

    [Fact]
    public void Pilot_Analyzer_Compatible_With_Existing_Backtest_Naming_Convention()
    {
        // The existing BACKTEST_RESULTS.md documents calibration using 2023+2024 to predict 2025.
        // This test ensures that the new window types support this pattern.

        var calibration = CalibrationWindow.ForYears(2023, 2024);  // Training window
        var evaluation = EvaluationWindow.ForYears(2025, 2025);    // Test window

        // Should not throw
        calibration.EnsureNoOverlap(evaluation);
        evaluation.EnsureNoOverlap(calibration);

        Assert.True(calibration.ToDate < evaluation.FromDate,
            "Calibration window must end before evaluation window for pilot analysis");
    }
}