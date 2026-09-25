# STATE — MatchEdge / FootyMetrics (al cierre de P3)
Rama `feature/fm-deterministic-navigation` (15 commits locales, sin push). Briefs: `docs/new 6.txt` (P1), P2 inline, `docs/FM_AGENT_BRIEF_P3.md` (P3). Handoff P2: `docs/HANDOFF_REVIEW.md`.

## Qué existe (endpoints)
- `POST /api/fm/fixtures|snapshot|run` — catálogo/snapshots (navegación gate+presupuesto 40/run).
- `POST /api/fm/outcomes/resolve {date?, fixtureIds?}` — D0: history (kickoff±1d) → stats-panel/fixtures-api; 0 navs; idempotente por señal salvo UNAVAILABLE (reintentable).
- `GET /api/fm/fixtures/{id}/confluence` — B1+B3+B4, solo lectura, 0 navs (nuevo en P3).
- Dev-only (Production devuelve 404): `/api/FootyMetricsTest/navigate|intercept|probe-nonpremium`, `/api/FootyMetricsTest/eval`.

## Esquema (SQLite matchedge.db)
- `fm_snapshot(+leakage_flag)`, `fm_signal(hits,sample_size,observed_hit_rate,opp_hits,opp_sample_size,venue_scope,competition_scope,recent_values_json=history[] crudo,params_json,status,motivo)` — máx. history observado: 10 (250/283 señales con 10).
- `fm_outcome(signal_id UNIQUE, status[RESOLVED|NOT_PLAYED|UNAVAILABLE|AMBIGUOUS], unavailable_reason[NO_HISTORY_ELEMENT|NO_DATE_MATCH|OTHER], source_conflict, source, motivo)` — P3 añadió las 2 últimas columnas.
- Estados actuales: 23 fixtures 2026-09-24 → RESOLVED=70, UNAVAILABLE=108 (100% NO_DATE_MATCH, lag FM), AMBIGUOUS=1, source_conflict=4 (fixture 33441811).

## Decisiones clave
- bestCount/bestTotal de FM = ventana optimizada por FM → solo referencia/auditoría; ventanas propias = 5/10/all fijas desde history crudo (`FmWindowCalculator`, on-demand, no persistidas; B2: nunca probabilidad).
- Confluencia = piezas separadas (team_attack/opponent/player), sin score; `overlap_flag` si comparten >50% de fechas o misma fila (`FmConfluenceBuilder`).
- `source_conflict` documenta contradicciones FM (saves+goles≠SOT, SOT>shots, saves=0 con goles), no las corrige ni excluye.
- Histórico de equipos usa `vt`/`vf` (jugadores `v`) como valor de mercado.
- Entorno: desplegado en **Production** (`C:\Services\MatchEdge`, puerto 5272), worker default **true** (ventana 10-22 UTC); toggle dev = env vars `ASPNETCORE_ENVIRONMENT=Development` + `Pipeline__LegacyWorker__Enabled=false` (solo sesión, no persistidas). Navs P3: ~1/10 (browser /start); resolve/confluence = APIRequest 0 navs.

## Pendientes reales
- Parte C del brief P3 (gate) — no ejecutar sin nueva instrucción.
- `POST /api/fm/collect` (§5 de P2) sin implementar.
- 108 outcomes player pendientes de re-resolve cuando FM ingiera history del 2026-09-24.
- Reporte con comparación de mercado: bloqueado hasta confirmar fuente de cuotas manual (no FM).
