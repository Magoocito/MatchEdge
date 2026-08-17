# Known Issues

## SofaScore — Cloudflare Bot Protection (UNSOLVABLE)

### Problem

SofaScore's API is protected by Cloudflare bot detection. When our app tries to call the API, it gets:

```json
{"error": {"code": 403, "reason": "challenge" }}
```

This happens because:
1. **JavaScript challenge** — SofaScore requires a browser to execute JS that generates a `x-captcha` JWT token.
2. **TLS fingerprinting** — .NET HttpClient is identified as non-browser even with correct headers (documented in ADR-002).
3. **IP binding** — The `x-captcha` JWT is bound to the IP address that generated it. Even if curl.exe receives the correct headers from a real browser, the server detects an IP mismatch and returns `challenge`.

### Failed Approaches

| Approach | Result |
|----------|--------|
| HttpClient with headers | 403 — TLS fingerprinting (ADR-002) |
| curl.exe with `-H` flags from browser | 403 — IP mismatch (challenge) |

### Working Approach: Playwright with Real Browser

**Implemented and active in production** (`SofaScoreBrowserClient` + `PlaywrightBrowserManager`,
registered as the live `ISofaScoreClient` in `Program.cs`, wrapped by `CachedSofaScoreClient` for
caching). A real Chrome instance with a persistent profile navigates SofaScore manually once,
inheriting Cloudflare's clearance cookie/JWT; subsequent API calls are made via `fetch()` executed
inside that authenticated browser context (`page.EvaluateAsync`), bypassing both the TLS
fingerprinting and IP-binding issues that blocked HttpClient/curl.exe.

**Known limitations (not solved, accepted trade-offs):**
- Requires a human to navigate to SofaScore manually at least once per session (Cloudflare
  clearance is tied to that browser instance).
- Not headless — cannot run on a server without a display, cannot be scheduled unattended.
- Not viable for a cloud/Docker deployment as currently built — this remains a local-development
  workaround, not a production-ready data pipeline.

### Long-Term Solution (still the target, Playwright is a bridge, not the destination)

Migrate to an alternative data source for Liga 1 Perú statistics once budget allows. Possible
alternatives:

- **API-Football** (api-football.com) — paid, includes Liga 1, good API — confirmed as the
  leading candidate; coverage depth for Liga 1 Perú (historical matches, per-role stats) still
  needs verification before committing to it.
- **Football-Data.org** — free tier available, limited Liga 1 coverage.
- **Opta / StatsBomb** — enterprise-grade, expensive.
- **Manual data collection** — local CSV/JSON database.

### Related Files

- ADR-002 — previous HttpClient attempt (documented in project).
- `SofaScoreBrowserClient.cs`, `PlaywrightBrowserManager.cs`, `CachedSofaScoreClient.cs`,
  `SofaScoreBrowserSeasonService.cs` — current working (bridge) solution.

## Backtesting Infrastructure — Working (Unblocked)

The backtesting infrastructure (`BacktestingService`, `IBacktestingService`) is **complete,
tested, and unblocked** — it now runs successfully against real SofaScore data via the Playwright
bridge described above. 111 tests passing, including `RunAsync_UsesSameAsOfDateTimeForAllModels`
(verifies no temporal leakage — every model uses each match's own date, never a global cutoff).

Known result so far (April–July 2025, Liga 1 Perú, gamma=1.6387 calibrated on 2023+2024 only):
Model B1 (home/away split, no gamma reapplied) outperforms both baseline Model A and Model B2
(split + gamma) on Brier Score and Log-Loss. Preliminary — needs a larger evaluation window before
being treated as conclusive.
