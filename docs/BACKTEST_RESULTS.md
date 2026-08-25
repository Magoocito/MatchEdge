# Backtesting Results — Liga 1 Perú (2023-2025)

## Configuration

- **Tournament:** Liga 1 Perú (ID 406)
- **Evaluation window:** 2023-01-01 to 2025-12-31 (3 separate yearly backtests)
- **Calibration seasons:** seasonLookback=2 per year (e.g., 2025 uses 2023+2024)
- **Gamma:** 1.6387 (calibrated on 2023+2024 goals ratio)
- **Models:** A (baseline), B1 (home/away split, no gamma reapply), B2 (split + gamma reapply)

## Consolidated Results — 890 Matches

### Per-Year Summary

| Year | Matches | A Brier ↓ | B1 Brier ↓ | Diff (A-B1) |
|------|---------|-----------|------------|-------------|
| 2023 | 295 | 0.5890 | 0.5825 | +0.0065 |
| 2024 | 304 | 0.5569 | 0.5612 | -0.0043 |
| 2025 | 291 | 0.6076 | 0.6038 | +0.0038 |
| **Total** | **890** | **0.5847** | **0.5825** | **+0.0021** |

### Statistical Significance — Paired Bootstrap (N=1000, 890 matches)

**Multiclass Brier (Home+Draw+Away)** — the definitive metric:

| Metric | Value |
|--------|-------|
| Model A Brier | 0.584134 |
| Model B1 Brier | 0.582210 |
| Observed diff (A - B1) | +0.001924 |
| Bootstrap mean | 0.001719 |
| Bootstrap median | 0.001746 |
| **95% CI** | **[-0.003262, +0.006446]** |
| Observed inside CI | **YES** |

**Conclusion:** B1 is better by 0.0019 Brier points, but the difference is **not statistically significant** at 95% confidence. The CI includes 0. Model A remains the production model.

### HomeWin Calibration Detail (Model A, 890 matches)

| Bin | Pred Avg | Observed | Diff (Obs-Pred) | N | Direction |
|-----|----------|----------|-----------------|---|-----------|
| 0.2-0.3 | 27.0% | 20.0% | -7.0% | 15 | OVER-estimates |
| 0.3-0.4 | 36.0% | 26.3% | -9.8% | 80 | OVER-estimates |
| 0.4-0.5 | 45.7% | 35.6% | -10.1% | 194 | OVER-estimates |
| 0.5-0.6 | 55.6% | 45.8% | -9.8% | 262 | OVER-estimates |
| 0.6-0.7 | 64.5% | 63.4% | -1.1% | 221 | OK |
| 0.7-0.8 | 73.6% | 79.6% | +6.0% | 103 | UNDER-estimates |
| 0.8-0.9 | 81.3% | 92.9% | +11.6% | 14 | UNDER-estimates |

**Aggregate bias:** -5.6% (Model A over-estimates home wins overall)

**Pattern:** Over-estimates at 20-60% (median predictions), under-estimates at 70-90% (strong favorites). The model is too optimistic for medium home advantage and too pessimistic for strong home advantage.

### 2025 Detailed Results

| Model | Block | Brier ↓ | LogLoss | MatchCount |
|-------|-------|---------|---------|------------|
| A | Overall | 0.6076 | 1.0203 | 291 |
| A | SplitOnly | 0.6025 | 1.0084 | 236 |
| A | FallbackOnly | 0.6297 | 1.0713 | 55 |
| **B1** | **Overall** | **0.6038** | **1.0145** | **291** |
| **B1** | **SplitOnly** | **0.5978** | **1.0012** | **236** |
| B1 | FallbackOnly | 0.6297 | 1.0713 | 55 |
| B2 | Overall | 0.6705 | 1.1166 | 291 |
| B2 | SplitOnly | 0.6800 | 1.1271 | 236 |
| B2 | FallbackOnly | 0.6297 | 1.0713 | 55 |

## Decision

- **Model A** remains the production model (confirmed with 890 matches, multiclass Brier bootstrap)
- **Model B1** does NOT provide statistically significant improvement over A
- **Model B2** (split + gamma) — double-counts home advantage, not recommended

### Next Steps

1. **Probability calibration** — Platt scaling or isotonic regression to correct the 20-60% over-estimation and 70-90% under-estimation bias in HomeWin predictions
2. **Market comparison** — obtain historical closing odds for Liga 1 Perú to compute CLV (Closing Line Value)

## Calibration Curves

Calibration curves (reliability diagrams) are computed automatically with each backtest. They measure whether predicted probabilities match observed frequencies.

### How to Read Calibration Curves

The `BacktestSummary` includes `CalibrationA`, `CalibrationB1`, and `CalibrationB2` fields:

- **HomeWin / Draw / AwayWin**: Calibration curves per outcome
  - `Bins[]`: Each bin has `PredictedProbability`, `ObservedFrequency`, and `Count`
  - `ExpectedCalibrationError (ECE)`: Weighted average of |predicted - observed| across bins
  - `BrierScore`: Per-outcome Brier score
- **OverallECE**: Macro-average of the three outcomes' ECE

### Interpretation

ECE is a heuristic, not a strict threshold. ECE ≈ 0.10 indicates imperfect calibration without specifying exact quality bands. The bin-by-bin analysis above provides more actionable detail than the aggregate ECE.

### Accessing Calibration Data

Calibration data is included in `GET /api/backtesting/result/{jobId}` response under `summary.calibrationA`, `summary.calibrationB1`, and `summary.calibrationB2`.

## Skipped Matches (2025 only, 4)

| MatchId | Home | Away | Date | Error |
|---------|------|------|------|-------|
| 13352937 | 282538 | 33894 | 2025-02-08 | No historical data for team 33894 |
| 13352938 | 306660 | 2302 | 2025-02-09 | No historical data for team 306660 |
| 13352936 | 2312 | 511206 | 2025-02-09 | No historical data for team 511206 |
| 13387752 | 87854 | 275839 | 2025-02-15 | No historical data for team 275839 |
