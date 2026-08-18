# Backtesting Results — Liga 1 Perú 2025

## Configuration

- **Tournament:** Liga 1 Perú (ID 406)
- **Evaluation window:** 2025-01-01 to 2025-12-31
- **Calibration seasons:** 2023 + 2024 (seasonLookback=2, via toDate)
- **Gamma:** 1.6387 (calibrated on 2023+2024 goals ratio, calibrationAsOf=2024-12-31)
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

## Bug Fix: Season Selection (This PR)

The backtesting service previously used `calibrationAsOf` to select which seasons to enumerate for evaluation matches. This caused 0 matches when `calibrationAsOf` preceded the evaluation window. Fixed by using `toDate` for season selection. See commit history for details.
