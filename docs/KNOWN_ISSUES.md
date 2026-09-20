# Known Issues

## SofaScore — Cloudflare Bot Protection (UNSOLVABLE)

### Problem

SofaScore's API is protected by Cloudflare bot detection. When our app tries to call the API, it gets:

```json
{"error": {"code": 403, "reason": "challenge" }}
```

This happens because:
1. **JavaScript challenge** — SofaScore requires a browser to execute JS that generates a `x-captcha` JWT token.
2. **TLS fingerprinting** — .NET HttpClient is identified as non-browser even with correct headers.
3. **IP binding** — The `x-captcha` JWT is bound to the IP address that generated it.

### Working Approach: Playwright with Real Browser

**Implemented and active in production** (`SofaScoreBrowserClient` + `PlaywrightBrowserManager`). A real Chrome instance with a persistent profile navigates SofaScore manually once, inheriting Cloudflare's clearance cookie/JWT; subsequent API calls are made via `fetch()` executed inside that authenticated browser context.

**Known limitations:**
- Requires a human to navigate to SofaScore manually at least once per session
- Not headless — cannot run on a server without a display
- Not viable for a cloud/Docker deployment

### Long-Term Solution

Migrate to an alternative data source for Liga 1 Perú statistics:
- **API-Football** (api-football.com) — paid, includes Liga 1 (leading candidate)
- **Football-Data.org** — free tier, limited Liga 1 coverage
- **Opta / StatsBomb** — enterprise-grade, expensive

---

## Backtesting Infrastructure — Complete

The backtesting infrastructure (`BacktestingService`, `IBacktestingService`) is **complete and tested**. 111 tests passing, including temporal leakage verification.

### Results Summary (2023-2025, Liga 1 Perú)

| Comparison | Result |
|------------|--------|
| A vs B1 | **Equivalent** (ΔBrier = 0.0016, IC includes 0) |
| A vs Market | **Market significantly better** (ΔBrier = 0.028, IC excludes 0) |
| B1 vs Market | **Market significantly better** (ΔBrier = 0.029, IC excludes 0) |
| B2 vs A/B1 | **B2 worse by ~10%** (permanently discarded) |

**Conclusion:** Market is the strongest baseline. Current models do not add value over it.

---

## Data Quality Issues — Resolved

### Carlos Manucci 2025 (RESOLVED)

- **Problem:** 10 matches in verified CSV 2025 involving Carlos Manucci (relegated to Liga 2)
- **Resolution:** Excluded from dataset. 1191 records in clean SQLite.

### Real Garcilaso Mapping (RESOLVED)

- **Problem:** Real Garcilaso was mapped to Deportivo Garcilaso (ID 458584)
- **Resolution:** Real Garcilaso = Cusco FC (ID 63760). Separate club from Deportivo Garcilaso.
- **Effect:** Market matching improved from 49.7% to 56.8%.

### BacktestingController Scope Bug (RESOLVED)

- **Problem:** Background Task.Run used scoped IBacktestingService after request scope disposed
- **Resolution:** IServiceScopeFactory + new scope per Task.Run + scope.Dispose() in finally

---

## Current State

| Component | Status |
|-----------|--------|
| Data quality | **Audit passed** (100% score match, 96% team mapping) |
| Backtesting | **Complete** (893 matches, bootstrap pareado confirmado) |
| Models | **A = B1 (equivalent), both < Market** |
| Next step | Improve model features OR accept market as baseline |
