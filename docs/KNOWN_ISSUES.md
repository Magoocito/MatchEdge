# Known Issues

## SofaScore Anti-Bot Bypass (TEMPORARY PATCH)

### Problem

SofaScore's API is protected by Cloudflare bot detection. When our app tries to call the API, it gets:

```json
{"error": {"code": 403, "reason": "challenge" }}
```

This happens because:
1. **JavaScript challenge** — SofaScore requires a browser to execute JS that generates a `x-captcha` JWT token.
2. **TLS fingerprinting** — .NET HttpClient is identified as non-browser even with correct headers (documented in ADR-002).

Neither `curl.exe` nor .NET `HttpClient` can solve the JavaScript challenge or bypass TLS fingerprinting.

### Current Workaround

We pass the **browser-generated headers** (`x-captcha` and `x-requested-with`) to `curl.exe` via the `-H` flag. These headers are obtained manually from a real browser session.

### How to Configure

1. Open https://www.sofascore.com in your browser.
2. Open DevTools (F12) → **Network** tab.
3. Navigate to any page that triggers API calls (e.g., a team page).
4. Find any request to `sofascore.com/api`, click on it.
5. In **Headers** tab, copy these values:
   - `x-captcha` (the full JWT token)
   - `x-requested-with` (short string like `e24dd0`)
6. Store via User Secrets:

```bash
cd src/MatchEdge.Api
dotnet user-secrets set "SofaScore:Headers:x-captcha" "YOUR_JWT_TOKEN"
dotnet user-secrets set "SofaScore:Headers:x-requested-with" "YOUR_VALUE"
```

Or via environment variables:

```bash
# Windows (PowerShell)
$env:SofaScore__Headers__x-captcha = "YOUR_JWT_TOKEN"
$env:SofaScore__Headers__x-requested-with = "YOUR_VALUE"
```

### Limitations — WHY THIS IS TEMPORAL

| Limitation | Impact |
|------------|--------|
| **Token expires** | The x-captcha JWT expires (typically hours). Requests will fail with 403 again when expired. |
| **IP-bound** | Token is tied to the IP that generated it. VPN/proxy changes break it. |
| **Manual process** | Requires human intervention each time the token expires. |
| **Not production-ready** | This is a development/local workaround only. |

### When the Token Expires

You'll see the same `403 challenge` error. Repeat the setup steps above to get fresh headers.

### Long-Term Solution

If this becomes a frequent problem, the next step should be evaluating **alternative data sources** for Liga 1 Perú statistics, rather than continuing to patch around SofaScore's bot protection. Possible alternatives:

- Official Liga 1 API (if available)
- Other sports data providers with API access
- Manual data collection and local database
- Open-source football data repositories

### Related Files

- `src/MatchEdge.Infrastructure/Clients/HttpRequestExecutor.cs` — implementation
- `src/MatchEdge.Api/appsettings.json` — configuration (empty by default)
- ADR-002 — previous HttpClient attempt (documented in project)
