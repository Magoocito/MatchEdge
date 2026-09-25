# STATE — MatchEdge / FootyMetrics (al cierre de P4)
Rama `feature/fm-deterministic-navigation` (commits locales, sin push). Briefs: `docs/new 6.txt` (P1), P2 inline, `docs/FM_AGENT_BRIEF_P3.md`, `docs/FM_AGENT_BRIEF_P4.md` (P4). Handoff P2: `docs/HANDOFF_REVIEW.md`.

## Qué existe (endpoints)
- `POST /api/fm/fixtures|snapshot|run` — catálogo/snapshots. **Snapshot = dual scope** por fixture: 3 navs (overview + player/team trends, `location=all`) + fetch API directo 0 navs para `<tab>+loc=match` (`location=match`). `/run` topN = Max/3 = 13/run.
- `POST /api/fm/outcomes/resolve {date?, fixtureIds?}` — 0 navs, idempotente; UNAVAILABLE reintentable.
- `GET /api/fm/fixtures/{id}/confluence` — solo lectura, 0 navs (P3).
- `POST /api/fm/fixtures/{id}/manual-odds {bookmaker,market,line,odds_value}` — C4: inserta `fm_market_odds` source=manual (json `odds_value` snake_case via JsonPropertyName). Probado: id=1161 fixture 33441811.
- Dev-only (Production 404): `/api/FootyMetricsTest/*`.

## Esquema (SQLite matchedge.db)
- `fm_signal(..., params_json)` — snapshot nuevo escribe `{"location":"all","league_only":false}`; capturas previas quedan NULL (histórico). GetLatestSignals filtra status OK|SUSPECT.
- `fm_outcome(unavailable_reason)` — P4 re-clasificó las 108: **100% NO_HISTORY_ELEMENT** (Δ 80/109/182d, 0 elementos con oponente del fixture) — NO_DATE_MATCH solo con elemento de ESE rival fuera de ±1d. Estados: RESOLVED=70, UNAVAILABLE=108, AMBIGUOUS=1.
- **`fm_market_odds` (nueva P4)**: fixture_id, bookmaker(id), bookmaker_name(NULL salvo manual), subject_type/name, market, line, odds_value, side, source(fm|manual), source_timestamp_utc, snapshot_id FK; índice único por snapshot. **1.247 filas FM** (7 fixtures: 40-392/fx, bk 3-5, 1.01-6.75) + 1 manual.

## Decisiones clave (P4)
- **C1: FM sí expone cuotas** en `data[].odds` de los tabs trends = array `{bk,over,under,yes,no,most}` (siempre 1 entrada por bookmaker id 1-5; yes/no/most siempre null en 1060 arrays → parser over/under completo). Sin timestamp de cuota → capture time. Sin mapeo id→nombre (404 en API, /bookmakers sin ids) → no inventar.
- **B: `location=match`** solo vía fetch API directo — el SPA **borra el parámetro en soft navigation** (nav con ?location=match → API lo manda sin location). Respuesta `data:[]` para los 7 fixtures (FM muestra "Showing 0" también en la UI) pero el request queda evidenciado en tab_url/apiPath/params. `location=home|away` = 400. Venue real estructural (Team vs Fixture.Home/Away + `h` en history) disponible sin navegar.
- Odds capturas desde el pase base (0 navs extra). `WithLocationMatch` reemplaza (no concatena) location previo del SPA.
- Nav budget: 40/run safety valve, documentado, sin límite duro de fase.

## Pendientes reales
- **PARA antes de Parte D** (brief P4 gate): D define contrato de salida (confluencia + contexto + venue + odds) — espera revisión del hallazgo C1.
- `POST /api/fm/collect` (§5 P2) sin implementar.
- Re-resolve de los 108 NO_HISTORY_ELEMENT cuando FM ingiera history del 2026-09-24.
- Riesgo vigente: canary SUSPECT (market-mix drift vs baseline P2) en algunos team/player tabs; fixture 33441811 source_conflict=4.
