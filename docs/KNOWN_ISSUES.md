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
| Puppeteer/Playwright | Not attempted — would require separate process, fragile |

### Decision: Do NOT Patch Further

**Do not** attempt to bypass SofaScore's Cloudflare protection. The effort is not sustainable and the approach is fundamentally broken (IP binding + JS challenge).

### Long-Term Solution

Migrate to an alternative data source for Liga 1 Perú statistics. Possible alternatives:

- **API-Football** (api-football.com) — paid, includes Liga 1, good API
- **Football-Data.org** — free tier available, limited Liga 1 coverage
- **Opta / StatsBomb** — enterprise-grade, expensive
- **Manual data collection** — local CSV/JSON database

### Related Files

- ADR-002 — previous HttpClient attempt (documented in project)

## Backtesting Infrastructure — Ready but Blocked

The backtesting infrastructure (Branch `feature/backtesting-service`, Commit `c4a82e7`) is **complete and tested** (102 tests passing). It is ready to execute but requires a working data source to run.

Once a new data source is implemented, the backtesting will work without changes.
