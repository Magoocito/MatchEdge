# Backtesting Results — Liga 1 Perú 2025

## Configuration

- **Tournament:** Liga 1 Perú (ID 406)
- **Evaluation window:** 2025-01-01 to 2025-12-31
- **Calibration seasons:** 2023 + 2024 (seasonLookback=2, via toDate)
- **Gamma:** 1.6387 (calibrated on 2023+2024 goals ratio, via CalibrationWindow 2023-01-01 to 2024-12-31)
- **Models:** A (baseline), B1 (home/away split, no gamma reapply), B2 (split + gamma reapply)

## Results Summary

**291 matches evaluated** (4 skipped due to missing team data — newly promoted teams without historical records).

### Full 9-Block Results

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

### Sanity Checks ✅

1. **Match count ~300:** 291 evaluated + 4 skipped = 295 total ✅
2. **Fallback equality:** A = B1 = B2 on fallbackOnly (0.6297 / 1.0713 / 55) ✅
3. **Split + Fallback = Overall:** 236 + 55 = 291 for all models ✅

## Statistical Significance — Paired Bootstrap (N=1000)

Paired bootstrap on 236 splitOnly matches (Brier A - Brier B1):

| Metric | Value |
|--------|-------|
| Observed diff (A - B1) | 0.004721 |
| Bootstrap mean | 0.004513 |
| Bootstrap median | 0.004734 |
| **95% CI** | **[-0.00734, 0.01633]** |
| Observed inside CI | **YES** |

**Conclusion:** B1 is better by point estimate (0.0047 Brier), but the difference is **not statistically significant** at 95% confidence. The CI includes 0. Model A is maintained in production.

## Skipped Matches (4)

| MatchId | Home | Away | Date | Error |
|---------|------|------|------|-------|
| 13352937 | 282538 | 33894 | 2025-02-08 | No historical data for team 33894 |
| 13352938 | 306660 | 2302 | 2025-02-09 | No historical data for team 306660 |
| 13352936 | 2312 | 511206 | 2025-02-09 | No historical data for team 511206 |
| 13387752 | 87854 | 275839 | 2025-02-15 | No historical data for team 275839 |

All 4 are newly promoted teams without sufficient historical matches. Confirmed: no hidden pattern.

## Decision

- **Model A** remains the production model
- **Model B1** is the leading candidate for future re-evaluation when more data is available (e.g., end of 2026 season)
- **Model B2** (split + gamma) performs worst — overfits local advantage

## Calibration Curves (NEW)

Calibration curves (reliability diagrams) are now computed automatically with each backtest. They measure whether predicted probabilities match observed frequencies.

### How to Read Calibration Curves

The `BacktestSummary` now includes `CalibrationA`, `CalibrationB1`, and `CalibrationB2` fields. Each contains:

- **HomeWin / Draw / AwayWin**: Calibration curves per outcome
  - `Bins[]`: Each bin has `PredictedProbability`, `ObservedFrequency`, and `Count`
  - `ExpectedCalibrationError (ECE)`: Weighted average of |predicted - observed| across bins
  - `BrierScore`: Per-outcome Brier score
- **OverallECE**: Macro-average of the three outcomes' ECE

### Interpretation

| ECE Range | Quality |
|-----------|---------|
| 0.00 - 0.05 | Excellent calibration |
| 0.05 - 0.10 | Good calibration |
| 0.10 - 0.20 | Moderate miscalibration |
| > 0.20 | Poor calibration |

**Ideal scenario:** Predicted probability ≈ Observed frequency for all bins (diagonal on reliability diagram).

### Accessing Calibration Data

Calibration data is included in `GET /api/backtesting/result/{jobId}` response under `summary.calibrationA`, `summary.calibrationB1`, and `summary.calibrationB2`.

## Historical Bug: calibrationAsOf Leakage

### The Bug

The backtesting service previously used a single `calibrationAsOf` parameter to control both:
1. **Season selection** — which seasons to enumerate for evaluation matches.
2. **Gamma calibration** — the date from which to compute the home advantage factor.

This caused a subtle but critical data leakage: if `calibrationAsOf` was set to a date within the evaluation window, the gamma optimization would include matches that should have been held out for testing. The model would "see the future" during calibration, producing unrealistically optimistic performance.

### How It Was Fixed

The fix introduced explicit `CalibrationWindow` and `EvaluationWindow` types with automatic overlap validation:

- **CalibrationWindow**: `CalibrationWindow.ForYears(2023, 2024)` → `2023-01-01` to `2024-12-31`
- **EvaluationWindow**: `EvaluationWindow.ForYears(2025, 2025)` → `2025-01-01` to `2025-12-31`
- **Validation**: `calibrationWindow.EnsureNoOverlap(evaluationWindow)` throws `InvalidOperationException` if the windows overlap.

This ensures gamma is computed **only** on historical data (2023+2024), never on evaluation data (2025).

### Relevant Tests

- `CalibrationWindowTests.cs` — overlap prevention, boundary conditions
- `CalibrationBugTests.cs` — reproduces the historical bug scenario and verifies the fix

## Model A — Production Model

**Model A (baseline) is the production model.** It is maintained in production because:

1. **Statistical equivalence**: Paired bootstrap (N=1000) on 236 split-only matches shows B1's point-estimate advantage is **not statistically significant** (95% CI includes 0).
2. **Occam's Razor**: Model A is the simplest model. Among statistically indistinct models, prefer the simplest.
3. **Robustness**: Model A has no moving parts beyond the gamma factor (1.6387), making it stable and predictable.

**Model B1** (home/away split, no gamma reapply) is the leading candidate for future re-evaluation when more data is available (e.g., end of 2026 season). **Model B2** (split + gamma) performs worst — it double-counts home advantage.

## Reproducibility Checklist

To verify the backtest results, follow these steps:

### Prerequisites

- .NET 8+ SDK installed
- SofaScore data accessible via Playwright bridge (`SofaScoreBrowserClient`)
- All 131 unit tests passing

### Step 1: Run Unit Tests (Fully Reproducible)

```bash
dotnet test tests/MatchEdge.UnitTests/ --verbosity normal
```

Expected: 131 tests pass, including:
- `CalibrationWindowTests` (5 tests) — overlap prevention
- `CalibrationBugTests` (5 tests) — historical bug reproduction
- `BacktestingServiceTests` — core backtesting logic
- `GammaOptimizerTests` — gamma calibration

### Step 2: Run Full Backtest (Requires SofaScore Data)

1. Start the API: `dotnet run --project src/MatchEdge.Api`
2. Ensure Playwright/SofaScore browser is configured and authenticated
3. Trigger backtest via API: `POST /api/backtesting/run` with evaluation window 2025-01-01 to 2025-12-31
4. Poll status: `GET /api/backtesting/status/{jobId}`
5. Retrieve results: `GET /api/backtesting/result/{jobId}`

### Expected Results

| Metric | Model A | Model B1 | Model B2 |
|--------|---------|----------|----------|
| Brier (Overall) | 0.6076 | 0.6038 | 0.6705 |
| LogLoss (Overall) | 1.0203 | 1.0145 | 1.1166 |
| Matches | 291 | 291 | 291 |

**Note**: Results may vary slightly depending on SofaScore data availability at the time of the backtest. The 291-match count assumes all evaluation matches have sufficient historical data for the teams involved.

### Step 3: Verify No Data Leakage

```bash
dotnet test tests/MatchEdge.UnitTests/ --filter "FullyQualifiedName~CalibrationBugTests" --verbosity normal
```

Expected: 5 tests pass, confirming the calibration/evaluation window separation prevents historical data leakage.
