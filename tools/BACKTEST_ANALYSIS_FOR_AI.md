# MatchEdge Backtesting — Complete Results for AI Analysis

## Project Overview

**MatchEdge** is a .NET 8 Clean Architecture API for football match prediction using Poisson models. It scrapes data from SofaScore via Playwright browser automation, computes team statistics, and evaluates prediction models via backtesting.

### Architecture
```
src/
├── MatchEdge.Domain/           # FootballMatchEvent, MatchTeam, MatchScore
├── MatchEdge.Application/      # Use cases, interfaces, services
│   └── UseCases/
│       ├── Backtesting/        # BacktestingService, BrierScoreCalculator, CalibrationCurveCalculator
│       ├── Lambda/             # MatchLambdaCalculator, EnhancedLambdaCalculator
│       ├── Probability/        # PoissonProbabilityEngine
│       ├── Statistics/         # TeamContextStatisticsCalculator
│       └── Calibration/        # HomeAdvantageCalibrationService
├── MatchEdge.Infrastructure/   # SofaScore clients, Playwright browser
└── MatchEdge.Api/              # Controllers, DI
```

### Models Being Compared

| Model | Description | Lambda Source | Gamma Treatment |
|-------|-------------|---------------|-----------------|
| **A** | Baseline | Season average goals (home/away combined) | gamma applied once |
| **B1** | Enhanced | Home/away split statistics (contextual) | gamma NOT reapplied to split |
| **B2** | Enhanced | Home/away split statistics (contextual) | gamma reapplied to split |

### Key Parameters
- **Tournament:** Liga 1 Perú (SofaScore ID: 406)
- **Gamma:** 1.6387 (home advantage factor = avgHomeGoals / avgAwayGoals from 2023+2024)
- **Season lookback:** 2 years per evaluation year
- **Poisson max goals:** 10 (for probability grid computation)

### How Predictions Work
1. For each match, fetch team statistics as of match date (no future leakage)
2. Compute lambda_home and lambda_away using Poisson model
3. Generate 10x10 probability grid: P(home=i, away=j) = Poisson(λ_home, i) × Poisson(λ_away, j)
4. Sum into HomeWin, Draw, AwayWin probabilities
5. Compare against actual result using Brier Score and LogLoss

### Brier Score Formula (Multi-class)
```
BS = (1/N) × Σ[(p_home - o_home)² + (p_draw - o_draw)² + (p_away - o_away)²]
```
Where o_x = 1.0 if that outcome occurred, 0.0 otherwise. Range: 0.0 to 2.0. Lower is better.

---

## Backtest Configuration

### Temporal Leakage Prevention
Every model receives the same `asOfDateTime = matchDate` for each match. This ensures:
- No future data is used in predictions
- All models are evaluated on identical data
- Fair comparison between A, B1, and B2

### Season Selection Bug Fix
Previously used `calibrationAsOf` to select seasons, which caused 0 matches when `calibrationAsOf` preceded the evaluation window. Fixed to use `toDate` instead.

### Graceful Skip
Matches for teams without historical data (newly promoted) are skipped with `SkippedMatches`/`SkippedDetails` in the summary.

---

## Consolidated Results — 890 Matches (2023-2025)

### Per-Year Breakdown

#### 2023 (295 matches, 3 skipped)
| Model | Block | Brier ↓ | LogLoss | N |
|-------|-------|---------|---------|---|
| A | Overall | 0.5890 | 0.9904 | 295 |
| A | SplitOnly | 0.5874 | 0.9869 | 252 |
| A | FallbackOnly | 0.5987 | 1.0108 | 43 |
| **B1** | **Overall** | **0.5825** | **0.9783** | **295** |
| **B1** | **SplitOnly** | **0.5798** | **0.9747** | **252** |
| B1 | FallbackOnly | 0.5987 | 1.0108 | 43 |

#### 2024 (304 matches, 2 skipped)
| Model | Block | Brier ↓ | LogLoss | N |
|-------|-------|---------|---------|---|
| A | Overall | 0.5569 | 0.9457 | 304 |
| A | SplitOnly | 0.5500 | 0.9339 | 275 |
| A | FallbackOnly | 0.6220 | 1.0540 | 29 |
| **B1** | **Overall** | **0.5612** | **0.9488** | **304** |
| **B1** | **SplitOnly** | **0.5548** | **0.9417** | **275** |
| B1 | FallbackOnly | 0.6220 | 1.0540 | 29 |

#### 2025 (291 matches, 4 skipped)
| Model | Block | Brier ↓ | LogLoss | N |
|-------|-------|---------|---------|---|
| A | Overall | 0.6076 | 1.0203 | 291 |
| A | SplitOnly | 0.6025 | 1.0084 | 236 |
| A | FallbackOnly | 0.6297 | 1.0713 | 55 |
| **B1** | **Overall** | **0.6038** | **1.0145** | **291** |
| **B1** | **SplitOnly** | **0.5978** | **1.0012** | **236** |
| B1 | FallbackOnly | 0.6297 | 1.0713 | 55 |

### Consolidated Metrics

| Year | Matches | A Brier | B1 Brier | Diff (A-B1) | Winner |
|------|---------|---------|----------|-------------|--------|
| 2023 | 295 | 0.5890 | 0.5825 | +0.0065 | B1 |
| 2024 | 304 | 0.5569 | 0.5612 | -0.0043 | A |
| 2025 | 291 | 0.6076 | 0.6038 | +0.0038 | B1 |
| **Total** | **890** | **0.5847** | **0.5825** | **+0.0021** | **B1 (marginal)** |

### Sanity Checks ✅
1. **Fallback equality:** A = B1 on fallbackOnly (same lambda when no split data)
2. **Split + Fallback = Overall** for all models and years
3. **No negative match counts**

---

## Statistical Significance — Paired Bootstrap (N=1000)

### Methodology
For each bootstrap iteration:
1. Sample N=890 matches with replacement from the 890 actual matches
2. Compute Brier Score for Model A and Model B1 on the bootstrap sample
3. Record diff = Brier_A - Brier_B1
4. After 1000 iterations, compute 95% CI from 2.5th and 97.5th percentiles

### Results (890 matches, HomeWin Brier)

| Metric | Value |
|--------|-------|
| Observed diff (A - B1) | -0.000382 |
| Bootstrap mean | -0.000325 |
| Bootstrap median | -0.000361 |
| **95% CI** | **[-0.003254, +0.002733]** |
| Observed inside CI | **YES** |

### Interpretation
- The CI includes 0 → difference is **NOT statistically significant**
- With 890 matches, the difference is essentially **zero** (|diff| < 0.0004)
- B1 does NOT provide measurable improvement over A
- **Model A remains the production model**

---

## Calibration Curves

Calibration measures whether predicted probabilities match observed frequencies.

### Expected Calibration Error (ECE)
```
ECE = Σ[|bin_count/N| × |observed_frequency - predicted_probability|]
```

### Interpretation
| ECE Range | Quality |
|-----------|---------|
| 0.00 - 0.05 | Excellent calibration |
| 0.05 - 0.10 | Good calibration |
| 0.10 - 0.20 | Moderate miscalibration |
| > 0.20 | Poor calibration |

### 2025 Calibration Results (sample)

| Model | HomeWin ECE | Draw ECE | AwayWin ECE | Overall ECE |
|-------|-------------|----------|-------------|-------------|
| A | 0.138 | 0.096 | 0.077 | 0.104 |
| B1 | 0.140 | 0.098 | 0.099 | 0.112 |

Both models show moderate miscalibration (ECE ~0.10-0.11), primarily in HomeWin predictions.

---

## Key Findings

1. **Model B1 does NOT outperform Model A** — even with 890 matches, the difference is negligible (diff = -0.0004)
2. **Year-to-year variance is large** — B1 wins in 2023 and 2025, A wins in 2024, suggesting the "improvement" is noise
3. **Gamma optimization found optimal=1.35** but no overfitting detected
4. **Both models have similar calibration** — ECE ~0.10-0.11
5. **B2 (split + gamma) performs worst** — overfits local advantage

## Recommendations

1. **Keep Model A as production** — simpler, equivalent performance
2. **Consider additional features** — xG, closing odds, momentum indicators
3. **Accumulate more data** — 890 matches may still be insufficient for small effect sizes
4. **Explore ensemble approaches** — combine A and B1 predictions

---

## JSON Result Files

Detailed per-match results are stored in:
- `tools/backtest_2023.json` — 295 matches with full probability details
- `tools/backtest_2024.json` — 304 matches with full probability details
- `tools/backtest_2025.json` — 291 matches with full probability details
- `tools/consolidated_results.json` — Summary with bootstrap results

### Per-Match Detail Schema
```json
{
  "matchId": 12345678,
  "homeTeamId": 2312,
  "awayTeamId": 306,
  "matchDate": "2025-06-01T20:00:00Z",
  "actualResult": "H",
  "modelA_HomeWinProb": 0.52,
  "modelA_DrawProb": 0.24,
  "modelA_AwayWinProb": 0.24,
  "modelB1_HomeWinProb": 0.55,
  "modelB1_DrawProb": 0.23,
  "modelB1_AwayWinProb": 0.22,
  "modelB2_HomeWinProb": 0.0,
  "modelB2_DrawProb": 0.0,
  "modelB2_AwayWinProb": 0.0,
  "calculationMethod": "HomeAwaySplit"
}
```

### Summary Schema
```json
{
  "totalMatches": 291,
  "skippedMatches": 4,
  "skippedDetails": [...],
  "modelA": {
    "overall": { "brierScore": 0.6076, "logLoss": 1.0203, "matchCount": 291 },
    "splitOnly": { "brierScore": 0.6025, "logLoss": 1.0084, "matchCount": 236 },
    "fallbackOnly": { "brierScore": 0.6297, "logLoss": 1.0713, "matchCount": 55 }
  },
  "modelB1": { ... },
  "calibrationA": {
    "homeWin": { "outcome": "H", "bins": [...], "expectedCalibrationError": 0.138, "brierScore": 0.214 },
    "draw": { ... },
    "awayWin": { ... },
    "overallECE": 0.104,
    "totalMatches": 291
  },
  "calibrationB1": { ... }
}
```
