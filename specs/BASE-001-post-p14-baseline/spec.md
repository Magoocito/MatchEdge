# BASE-001 — Post-P14 Baseline Reconciliation

**Feature Branch**: `cursor/docs-base-001-baseline-1caf`

**Created**: 2026-11-03

**Status**: Draft

**Input**: Baseline analysis reconciliation per issue #49

---

## 1. Purpose

This document records the verified baseline state of `main` at commit `8858588` (post-P14), reconciling the provided analysis against actual code. It follows the Constitution principles—especially #14 (Documentation as Current State)—and serves as the evidence base before any P15 planning.

---

## 2. Verified Current Capabilities (CA1)

### 2.1 FootyMetrics Ingestion & Persistence

| Component | Location | Lines | Verified |
|-----------|----------|-------|----------|
| `FmNavigator` | `src/MatchEdge.Infrastructure/Clients/FmNavigator.cs` | 127 | ✓ MaxNavigationsPerRun=40 confirmed (line 11) |
| `FootyMetricsBrowserManager` | `src/MatchEdge.Infrastructure/Clients/FootyMetricsBrowserManager.cs` | — | ✓ Exists |
| `FootyMetricsBrowserCollector` | `src/MatchEdge.Infrastructure/Clients/FootyMetricsBrowserCollector.cs` | — | ✓ Exists |
| `FmSnapshotController` | `src/MatchEdge.Api/Controllers/FmSnapshotController.cs` | — | ✓ POST /api/fm/fixtures, snapshot, run |
| `FmTeamController` | `src/MatchEdge.Api/Controllers/FmTeamController.cs` | 698 | ✓ POST collect, GET matches |
| `FmGlobalCollectOrchestrator` | Not found as separate file | — | ✓ Logic embedded in FmTeamController (P10) |

### 2.2 Persistence & Outcomes

| Component | Location | Lines | Verified |
|-----------|----------|-------|----------|
| `FmSnapshotStore` | `src/MatchEdge.Infrastructure/Services/FmSnapshotStore.cs` | 1278 | ✓ |
| `FmOutcomeStore` | `src/MatchEdge.Infrastructure/Services/FmOutcomeStore.cs` | — | ✓ |
| `FmOutcomeResolver` | `src/MatchEdge.Infrastructure/Services/FmOutcomeResolver.cs` | 755 | ✓ |

### 2.3 Reporting (P11/P12 360° Descriptive)

| Component | Location | Lines | Verified |
|-----------|----------|-------|----------|
| `FmConfluenceReportBuilder` | `src/MatchEdge.Infrastructure/Services/FmConfluenceReportBuilder.cs` | 1136 | ✓ |
| `FmConfluenceReportLoader` | Same file or adjacent | — | ✓ |
| `FmWindowCalculator` | `src/MatchEdge.Infrastructure/Services/FmWindowCalculator.cs` | — | ✓ Windows 5/10/15/20 |

### 2.4 P13 Proxy-EV / Delta%

| Component | Location | Verified Details |
|-----------|----------|------------------|
| `FmP13Controller` | `src/MatchEdge.Api/Controllers/FmP13Controller.cs` | ✓ Endpoints: GET p13/report, p13/report.md, fixtures/{id}/proxy-ev |
| `P13ProxyEvReportBuilder` | `src/MatchEdge.Infrastructure/Services/P13ProxyEvReportBuilder.cs` | ✓ 1037 lines |
| Dixon-Coles rho | P13ProxyEvReportBuilder.cs:77 | ✓ `DixonColesRho = DixonColes.DefaultRho` (0.1) |
| HomeAdvantageFactor | P13ProxyEvReportBuilder.cs:79 | ✓ 1.15 for UEFA |
| Lambda source | P13ProxyEvReportBuilder.cs:130 | ✓ "media FM de la ventana last10" |
| IsPostKickoff | P13ProxyEvReportBuilder.cs:277-280 | ✓ Fail-closed detection |
| StatWindow | P13ProxyEvReportBuilder.cs:90 | ✓ 10 |

### 2.5 P14 Reports

| Component | Location | Verified Details |
|-----------|----------|------------------|
| `FmP14Controller` | `src/MatchEdge.Api/Controllers/FmP14Controller.cs` | ✓ 183 lines |
| Hardcoded fixtures | FmP14Controller.cs:27-38, 46-51, 69-80 | ✓ P14Fixtures (29/09), P14FixturesOct1 (01/10), P14FixturesOct2 (02/10) |
| `FmRefereeOverview` | `src/MatchEdge.Infrastructure/Services/FmRefereeOverview.cs` | ✓ Used in report-20261002.md |

### 2.6 Manual Odds

| Endpoint | Location | Verified |
|----------|----------|----------|
| POST manual-odds | FmSnapshotController.cs | ✓ |
| PUT manual-odds/{market}/{line} | FmSnapshotController.cs | ✓ |

### 2.7 Legacy SofaScore Stack

| Component | Location | Verified |
|-----------|----------|----------|
| `PredictionController` | `src/MatchEdge.Api/Controllers/PredictionController.cs` | ✓ Exists, active |
| `BacktestingController` | `src/MatchEdge.Api/Controllers/BacktestingController.cs` | ✓ Exists, active |
| `GammaOptimizerController` | `src/MatchEdge.Api/Controllers/GammaOptimizerController.cs` | ✓ Exists, active |
| `CalibrationController` | `src/MatchEdge.Api/Controllers/CalibrationController.cs` | ✓ Exists, active |
| `IValueBetCalculator` | `src/MatchEdge.Application/UseCases/ValueBetting/IValueBetCalculator.cs` | ✓ Registered in Program.cs |
| `BankrollManager` | `src/MatchEdge.Application/Services/IBankrollManager.cs` | ✓ Registered |
| UseCases/Backtesting/* | `src/MatchEdge.Application/UseCases/Backtesting/` | ✓ ~30 files |
| UseCases/Probability/DixonColes.cs | `src/MatchEdge.Application/UseCases/Probability/DixonColes.cs` | ✓ Exists |

### 2.8 Dev-Only Controllers

| Component | Location | Lines | Verified |
|-----------|----------|-------|----------|
| `FootyMetricsTestController` | `src/MatchEdge.Api/Controllers/FootyMetricsTestController.cs` | 1435 | ✓ EvalAllowed => _env.IsDevelopment() |
| `BrowserTestController` | `src/MatchEdge.Api/Controllers/BrowserTestController.cs` | — | ✓ Exists |

---

## 3. STATE.md Inconsistencies (CA2) — Verified

### 3.1 Phase Claim vs Actual Content

| Claim in STATE.md | Actual in main | Status |
|-------------------|----------------|--------|
| "cierre de P12" | main includes P13 (#46), P14 (#47), constitution (#48), Spec Kit (#53), audits (#54, #55) | ✓ CONFIRMED inconsistent |

**Evidence**: `git log --oneline -5` shows merge of PR #55 (betano audit), #54, #53.

### 3.2 Missing Endpoint Documentation

| Missing from STATE.md | Present in Code | Status |
|-----------------------|-----------------|--------|
| P13 endpoints | FmP13Controller.cs | ✓ CONFIRMED |
| P14 endpoints | FmP14Controller.cs | ✓ CONFIRMED |
| FmRefereeOverview | FmRefereeOverview.cs | ✓ CONFIRMED |
| IsPostKickoff detection | P13ProxyEvReportBuilder.cs:277 | ✓ CONFIRMED |

### 3.3 Legacy Controllers Not Documented

| Controller | Present | Documented in STATE.md |
|------------|---------|------------------------|
| PredictionController | ✓ | ✗ |
| BacktestingController | ✓ | ✗ |
| GammaOptimizerController | ✓ | ✗ |
| CalibrationController | ✓ | ✗ |
| TeamController | ✓ | ✗ |
| MatchingController | ✓ | ✗ |

### 3.4 Test Claims

| Claim | Verification | Status |
|-------|--------------|--------|
| "75/75 tests" | **NOT VERIFIED** — dotnet not available in this environment | UNVERIFIED |
| "149/149 MatchEdge.UnitTests" | **NOT VERIFIED** | UNVERIFIED |
| "103/109 InfrastructureTests (6 FAIL preexisting)" | **NOT VERIFIED** | UNVERIFIED |

**Note**: Tests were not executed in this reconciliation because the .NET SDK is not installed in the cloud agent environment. The test claims from STATE.md remain unverified.

### 3.5 Hardcoded Database Path

**Claim**: Hardcoded path `C:\Services\MatchEdge\matchedge.db`

**Verification**:

```csharp
// Program.cs lines 114-117
builder.Services.AddSingleton(_ => new FmSnapshotStore(
    "Data Source=C:\\Services\\MatchEdge\\matchedge.db"));
builder.Services.AddSingleton(_ => new FmOutcomeStore(
    "Data Source=C:\\Services\\MatchEdge\\matchedge.db"));
```

**Status**: ✓ CONFIRMED — hardcoded Windows path in Program.cs

### 3.6 "Solo FM, sin probabilidad/EV" Contradiction

| Claim in STATE.md | Actual Code | Status |
|-------------------|-------------|--------|
| "sin probabilidad/EV" | P13ProxyEvReportBuilder calculates P_modelo, FairOdds, DeltaPct | ✓ CONFIRMED contradictory |
| — | PredictionController, IValueBetCalculator active | ✓ CONFIRMED contradictory |

---

## 4. VISION.md vs Implementation (CA3) — Decision Points

These items are flagged for decision, not for automatic documentation updates per Constitution §14.

### 4.1 §6/§7/§13 (No EV/Edge) vs P13 Proxy-EV

| VISION states | Implementation | Gap |
|---------------|----------------|-----|
| "no convertir frecuencias en probabilidades" | P13 uses Poisson to generate P_modelo | ✓ EXISTS |
| "no calcular edge" | DeltaPct = (P_modelo × Cuota) - 1 | ✓ EXISTS |
| No probability/EV/edge | PredictionController, ValueBetCalculator active | ✓ EXISTS |

**Decision Required**: Is P13 Proxy-EV EXPERIMENTAL (labeled, not production), DEPRECATED, or is VISION out of date?

### 4.2 §3/§11 Own Windows Independent of FM Window

| VISION states | Implementation | Status |
|---------------|----------------|--------|
| Calculate own windows 5/10/15/20 from raw history[] | FmWindowCalculator.cs computes these | ✓ IMPLEMENTED |
| P13 lambda uses last10 | P13ProxyEvReportBuilder.cs:130 confirms | ✓ CONSISTENT with VISION §11 |

### 4.3 §5 Pre-Kickoff Context

| Requirement | P12 Implementation | P13 Implementation |
|-------------|-------------------|-------------------|
| Pre-match only | kickoffUtc: null allowed (fallback) | IsPostKickoff detection, fail-closed |

**Note**: P12 allows null kickoff with fallback; P13 implements fail-closed. This is an improvement, not a violation.

### 4.4 §10 Close Phase 2 Queue First

| VISION states | Actual | Status |
|---------------|--------|--------|
| Close Phase 2 queue before advancing | 108 NO_HISTORY_ELEMENT/UNAVAILABLE remain | ✓ DEVIATED |
| — | P13/P14 implemented before resolution | — |

**Decision Required**: Document as accepted technical debt or create follow-up to close Phase 2 queue.

### 4.5 §8 Auditability

| Artifact Location | Contents | Status |
|-------------------|----------|--------|
| `tmp/` | p13/p14 evidence, betano audit | Mixed |
| `tmp/fm/` | FM-specific artifacts | Mixed |
| Root `.txt` files | 20+ .txt files at repo root | ✓ HYGIENE ISSUE |
| `data/unreliable/` | Unreliable data quarantine | Exists |

---

## 5. Capability Classification (CA4) — Verified

| Capability | Class | Evidence |
|------------|-------|----------|
| Snapshot/collect FM | Descriptive | FmSnapshotController, FmTeamController |
| Outcomes resolution | Descriptive | FmOutcomeResolver, FmOutcomeStore |
| Windows 5/10/15/20 | Descriptive | FmWindowCalculator |
| 360°/Confluence reports | Descriptive | FmConfluenceReportBuilder |
| Manual odds | Descriptive | POST/PUT manual-odds |
| P14 reports | Descriptive | FmP14Controller |
| **P13 Proxy-EV/Delta%** | **Experimental/Predictive** | P13ProxyEvReportBuilder (Poisson, Dixon-Coles) |
| SofaScore A/B/Gamma stack | Experimental/Predictive | PredictionController, BacktestingController, etc. |
| /api/predictions | Experimental/Predictive | PredictionController |
| ValueBet calculation | Experimental/Predictive | IValueBetCalculator |
| FootyMetricsTestController | Dev-only/Internal | 1435 lines, IsDevelopment() guard |
| BrowserTestController | Dev-only/Internal | Development-only |
| data/unreliable/* | Deprecated/Quarantine | Unreliable data |
| **Validated models** | **None** | No model has passed validation per Constitution §13 |

### 5.1 "Home Wins" Hallazgo Analysis

**Source**: `docs/REPORT_FINAL_MODELS_VS_MARKET.md`

| Claim | Location | Analysis |
|-------|----------|----------|
| n=374, ΔBrier −0.063, "+8-12%" | Lines 16, 19, 78, 123-141 | — |
| Segment filtered by outcome | Lines 159-163 | ✓ CONFIRMED |
| "Sesgo de supervivencia" admission | Line 161 | ✓ CONFIRMED |

**Conclusion**: The report itself (lines 159-163) acknowledges this is a posteriori analysis filtered by outcome. The analysis states:

> "Sesgo de supervivencia: El análisis de 'Home wins' solo incluye partidos donde el home team ganó. Esto es a posteriori."

**Status**: ✓ INVALID per Constitution §2 (Statistical Honesty) — segment filtering by result mechanically favors the model with higher p(local). This is correctly documented in the report itself.

---

## 6. Technical Debt (CA5) — Verified

### 6.1 Large Classes / Logic in Controllers

| File | Lines | Constitution Principle |
|------|-------|------------------------|
| FootyMetricsTestController.cs | 1435 | §7, §12 |
| FmTeamController.cs | 698 | §7, §12 |
| FmSnapshotStore.cs | 1278 | §7 |
| FmConfluenceReportBuilder.cs | 1136 | §7 |
| FmOutcomeResolver.cs | 755 | §7 |

### 6.2 P14 Hardcoding

| Issue | Location | Status |
|-------|----------|--------|
| Fixtures hardcoded per date | FmP14Controller.cs:27-80 | ✓ CONFIRMED |
| No parametrizable config | — | ✓ CONFIRMED |

### 6.3 Database Path

| Issue | Location | Status |
|-------|----------|--------|
| Windows path hardcoded | Program.cs:114-117 | ✓ CONFIRMED |

### 6.4 CI / Specs

| Item | Status |
|------|--------|
| `.github/workflows/` | ✗ Does not exist |
| `specs/` directory | Empty before this PR |

### 6.5 Test Failures

| Claim | Status |
|-------|--------|
| 6 failing DateTeamMatchingIntegrationTests | **NOT VERIFIED** — dotnet unavailable |

### 6.6 Hygiene Issues

| Issue | Count | Location |
|-------|-------|----------|
| Root .txt files | 20+ | Repository root |
| Unreliable CSVs | Several | `data/unreliable/` |
| Mixed artifacts | — | `tmp/`, `tmp/fm/` |
| Overlapping docs | — | DIAGNOSTICO_ACTUAL, KNOWN_ISSUES, REPORT_P3-5, "new 6.txt" |

### 6.7 Domain Issues

| Issue | Status |
|-------|--------|
| C3 kickoff null | ✓ Documented in STATE.md, P13 implements fail-closed |
| 108 NO_HISTORY_ELEMENT | ✓ Confirmed in analysis |
| teams/table stat validation | Not verified |
| SUSPECT canary (drift) | Referenced in STATE.md |

### 6.8 Dual Stack Decision

| Stack | Status | Decision Required |
|-------|--------|-------------------|
| SofaScore (Prediction, Backtesting, Gamma, Calibration) | Active in code | Deprecated or conserved? |
| FootyMetrics (Fm*) | Active, primary | — |

---

## 7. Corrections to Provided Analysis

During verification against the codebase, the following corrections were identified:

| Original Claim | Correction | Evidence |
|----------------|------------|----------|
| "FmGlobalCollectOrchestrator.cs (P10)" | Logic is embedded in FmTeamController, not a separate file | `find . -name "FmGlobalCollectOrchestrator.cs"` returns nothing |
| Test counts (75/75, 149/149, 103/109) | **Cannot be verified** — dotnet SDK not installed | Marked as UNVERIFIED |
| "6 FAIL preexistentes" | **Cannot be verified** — no test execution | Marked as UNVERIFIED |

---

## 8. Test Evidence

**Build Status**: NOT EXECUTED

**Test Execution**: NOT EXECUTED

**Reason**: The .NET SDK is not installed in the cloud agent environment. The `dotnet` command is unavailable.

```
$ which dotnet && dotnet --version
dotnet not installed
```

All test-related claims from STATE.md and the original analysis remain **UNVERIFIED** as of this reconciliation.

---

## 9. Reconciliation Recommendations (CA6)

Per the analysis, the following reconciliation actions are recommended before the next development cycle:

1. **STATE.md Update**: Rewrite to reflect actual main state (tests "not verified" until executed).

2. **ADR Required**:
   - (a) P13 Proxy-EV: label as EXPERIMENTAL or retire
   - (b) SofaScore stack: deprecate or conserve
   
3. **REPORT_FINAL_MODELS_VS_MARKET.md**: Mark "Home wins" segment as invalid per selection bias (report already acknowledges this at line 161).

4. **Build + Tests**: Execute `dotnet build` and `dotnet test`; classify failures as INTRODUCED_BY_CHANGE/PREEXISTING/UNKNOWN with evidence; move to CI (#51).

5. **Phase 2 Queue**: Close 108 NO_HISTORY_ELEMENT/UNAVAILABLE items by cause.

6. **Fail-closed Rule**: Implement for kickoff null in pre-match reports (P13 already does this; P12 does not).

---

## 10. Constitution Check

| Principle | Status | Evidence |
|-----------|--------|----------|
| 1. Evidence First | PASS (partial) | Provenance preserved in snapshots; some gaps in audit trail |
| 2. Statistical Honesty | **FAIL** | "Home wins" segment in REPORT_FINAL is filtered by outcome |
| 3. Temporal Integrity | **FAIL** | P12 allows null kickoff with fallback; P13 implements fail-closed |
| 4. Reproducibility | PASS (partial) | Determinism tests exist; some gaps |
| 5. Spec First | **FAIL** | P13/P14 implemented without prior specs |
| 6. Small Changes | PASS (partial) | PR history shows incremental work |
| 7. Explicit Architecture | **FAIL** | Large classes, logic in controllers |
| 8. Opportunistic Refactor | N/A | No refactor in scope |
| 9. Testing as Evidence | **FAIL** | Tests not executed; claims unverified |
| 10. Fail Closed | PASS (partial) | P13 implements; P12 does not |
| 11. Observability | PASS (partial) | Logging exists; some gaps |
| 12. Boundaries | **FAIL** | Controllers contain business logic |
| 13. Model Validation | PASS (with exception) | No models claimed as VALIDATED; "Home wins" report acknowledged as biased |
| 14. Docs = State | **FAIL** | STATE.md claims P12 closure; main has P13/P14 |
| 15. Token Efficiency | N/A | Agent context management |
| 16. Human Control | PASS | Architectural decisions require approval |
| 17. Definition of Ready | **FAIL** | P13/P14 proceeded without full spec |
| 18. Definition of Done | **FAIL** | Code written ≠ Done without test verification |
| 19. SDD Convergence | **FAIL** | No convergence check for P13/P14 |
| 20. Governance | PASS | Constitution exists and is followed for new work |

**Global Baseline Status**: **FAIL** (8 principles failing, 5 partial, 4 passing, 3 N/A)

---

## Assumptions

- This reconciliation is documentation-only per issue #49 instructions
- No application code changes are in scope
- Tests could not be executed due to environment constraints
- STATE.md and VISION.md are not modified per instructions

---

## References

- Issue: https://github.com/Magoocito/MatchEdge/issues/49
- Constitution: `.specify/memory/constitution.md`
- STATE.md: `docs/STATE.md`
- VISION.md: `docs/VISION.md`
- Analysis source: `uploads/BASE-001-analysis.md`
