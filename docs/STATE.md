# STATE — MatchEdge / FootyMetrics (cierre de P12)
Rama `feature/fm-p12-360-ampliation` (desde `main` post-merge PR #44; #37/#40/#42/#43/#44 MERGED). Briefs: `docs/new 6.txt` (P1), P2 inline, `docs/FM_AGENT_BRIEF_P3|P4|P5|P8.md`, P6/P7/P9/P10/P11/**P12** inline. Handoff P2: `docs/HANDOFF_REVIEW.md`. Guía manual: `docs/FM_MANUAL_NAVIGATION.md`.

## Endpoints
- `POST /api/fm/fixtures|snapshot|run` — catálogo + snapshot dual-scope por fixture (3 navs + fetch 0 navs para `<tab>+loc=match`); `/run` topN = Max/3 = 13/run. `POST /api/fm/outcomes/resolve {date?,fixtureIds?}`: 0 navs, idempotente, UNAVAILABLE reintentable.
- `GET /api/fm/fixtures/{id}/confluence|report|report.md|odds`, `POST .../manual-odds`, `PUT /api/fm/fixtures/{id}/manual-odds/{market}/{line}` (P3-P7/E4).
- `POST /api/fm/teams/{teamApid}/collect {locations|venue, period, stat|stats[]}` (P6 G2/PBI 2.1/P9, tokens vía `FmPlayerStatMap`), `GET /teams/{teamApid}/matches` (G3), `POST /teams/{teamApid}/collect-positions` (J5, whitelist period ∈ {5,10,15,20}).
- `POST /api/fm/collect {teams[], stats[], locations[], period, includePositions}` (P10: un solo orquestador sin paralelismo, throttle 2-4 s, error por equipo sin abortar; 400 solo por body inválido).
- Dev-only (Production 404): `/api/FootyMetricsTest/*` (`/navigate /intercept /eval /fetch/raw /start /start-visible /close /login-status`).

## Esquema (C:\Services\MatchEdge\matchedge.db)
- `fm_signal(..., venue_role, status, motivo)`; `fm_team_matches` índice único `(team_apid, location, period, stat, fixture_id)`; `fm_player_matches` (`perspective=team|position`); `fm_outcome` 70 RESOLVED / 108 NO_HISTORY_ELEMENT / 1 AMBIGUOUS; `fm_bookmakers` + `fm_market_odds` (100% `bookmaker_name`).

## P12 — 360° ampliado (Parte B)
- **Resolución pre-match**: los 3 fixtures del 27-sep (33608037/33608044/33608045) **no tienen filas propias** en `fm_team_matches` → `FmConfluenceReportLoader.ResolveSidesAsync` toma home/away de `fm_signal.venue_role`, el apid de `GetTeamApidByNameAsync` (`opponent_apid WHERE opponent COLLATE NOCASE`, 1 apid distinto por nombre verificado 6/6) y la competición de `GetLatestLeagueAsync`; esos apids entran en el set que puebla `TeamRows`/`PlayerRows` (raíz de C1: antes `subjectRows` vacío → evidencia 0).
- **`FmReportEvidence`** gana `competition` (nullable), `overlap_flags` (mismos `BuildOverlapFlags`/`BuildPlayerOverlapFlags` que markets/player_signals) y `data_quality` (mismo `BuildDataQuality`); el .md solo añade esas líneas si hay flag o calidad notable → plantilla P11 intacta.
- **`confluence[]`** (+ `NoteConfluence`): familias de mercado (sin prefijo `home_`/`away_`, excluye `total_*`) con **≥2 series observadas** (equipo + rival + jugadores), cada una con su línea y `last5/last10/all`; orden por familia asc, dedupe (familia, sujeto, línea), `ConfluenceCap=30`, **sin score ni ranking de fuerza**. .md: sección fija `## Confluencia descriptiva (equipo + rival + jugadores)` entre `## Evidencia histórica` y `## Cuotas`.

## Evidencia P12 (0 navs; ver "Corrección 2026-09-28 - ledger de navegación" abajo)
- `tmp/fm/p12_report_336080{37,44,45}.{json,md}` + `p12_report_check.json`: 3/3 HTTP 200, doble GET JSON y .md **byte-idénticos**, **0 palabras prohibidas** (literal E5 strippado), orden de secciones correcto, 0 ocurrencias en JSON.
- Por fixture: `player_evidence` **30/30/30** (tope), `team_evidence` **13/17/17** con `rival_context` 10/15/15 (los 3/2/2 sin rival son `total_*`), `confluence` **10/9/11** familias (49/55/59 strands), `competition` en el 100% de las entradas, `overlap_flags` 0 (ningún last5 comparte >50% con el H2H), notes 3, `kickoffUtc: null` y lados resueltos **solo por fallback** en los 3 fixtures.
- A1-A4: fixtures vía `fixtures?date=` (0 navs; `POST /fm/fixtures` devuelve 0 para esta liga — catálogo in-memory + `fixtures/league` vacío), 3 snapshots = 9 navs (1 SUSPECT/fixture por canary `vt`), `POST /collect` 96 fetches / 0 errores / 541 s (`fm_team_matches` 2110→3370, `fm_player_matches` 51701→82315, `fm_signal` 1642), `outcomes/resolve` → 0 finished (UNAVAILABLE, esperado).
- Tests: `dotnet test --filter Fm` → **75/75** (P12 +3: evidencia con competition/overlap/data_quality, confluence por familias, pre-match con kickoff nulo); `MatchEdge.UnitTests` 149/149; `MatchEdge.InfrastructureTests` 103/109 (6 FAIL **preexistentes** `DateTeamMatchingIntegrationTests`).

## P12-CIERRE — cuotas, post-mortem y Proxy-EV
- Cuotas manuales: 76 valores (`source='manual'`, lados over/under/si/no) → 33608037 **0** (partido iniciado, descartado), 33608044 **50**, 33608045 **26**; tabla `tmp/fm/p12_proxy_ev.md` (76 filas, 56 con Proxy-EV; observed tomado del lado de la fila — `(n−hits)/n` cuando el lado es contrario a `fm_signal.direction`, totales con la vista local como el builder).
- Post-mortem 27-sep (`tmp/fm/p12_postmortem.md`): **2/2 candidatos del log de seguimiento fallaron** (Grecia away_corners 3.5 over 8/10 @2,35 → 1 córner; Noruega home_shots 13.5 under 9/10 @1,85 → 18+ tiros); sí acertó Under 2.5 Alemania–Grecia (observed 2/10) → la DB no tiene `fm_outcome` resuelto para estos fixtures.
- Lección clave: **Proxy-EV sin filtro de contexto de marcador** — Grecia 0-1 defendiendo y Noruega 1-2 persiguiendo anularon la tendencia last10; n=10 es muestra corta y no distingue el acierto.
- Presupuesto de navegación: **corregido el 2026-09-28** — el "9/12" era un ledger manual de fase, no un límite de la API (ver sección de corrección abajo). Propuesta P13 — exclusión por régimen de marcador: `tmp/fm/p13_proposal.md`.

## Corrección 2026-09-28 — el ledger "9/12" no era de la API
- Texto original (traza conservada, no borrada): "Evidencia P12 (0 navs; presupuesto de navegación intacto en 9/12)" y "Presupuesto de navegación: **9/12 intacto** (cierre con 0 navs; ver Evidencia P12)". Se reubican aquí en vez de reescribir la historia.
- Qué era: un **ledger manual de fase**, igual que "máx. 10 para toda esta fase" (`docs/FM_AGENT_BRIEF_P3.md`) o "2/15 usados" (`docs/HANDOFF_REVIEW.md`). No derivaba de ninguna llamada a la API.
- Límite real (código sin tocar): `FmNavigator.MaxNavigationsPerRun = 40` **por corrida** (`src/MatchEdge.Infrastructure/Clients/FmNavigator.cs:11`), contador a 0 en cada `POST /api/fm/fixtures|snapshot|run` (`src/MatchEdge.Api/Controllers/FmSnapshotController.cs:271,304,362`); no existe en código límite acumulado entre corridas. `git log -S MaxNavigationsPerRun` → 40 desde `0a02809` (único commit que lo creó).
- Por qué se corrigió: (a) estaba autocontradictorio — el propio cierre de P12 registraba "3 snapshots = 9 navs" en el mismo bloque que afirmaba "9/12 intacto"; (b) produjo una pausa injustificada en P13 (28-sep: solo 3 de 8 fixtures snapshot). La API nunca rechazó nada: 0 ocurrencias de `Navigation budget`/429 en `tmp/fm/p12_api_out.log` y `tmp/fm/p13_api_out.log`, `tmp/fm/p13_api_err.log` vacío, 0 filas `fm_snapshot` en error.
- Corregido el **2026-09-28T21:34Z** (hora real del sistema) en el ciclo de cierre P11/P13. Origen del error de metadato: `tmp/fm/p13_evidence_build.py` (frases "9/9 navs" y "presupuesto agotado" hardcodeadas, 0 llamadas HTTP); traza completa en `tmp/fm/p13_correction_note.md`.

## Decisiones clave (vigentes)
- Solo FootyMetrics; sin scraping de cuotas (solo manual); lenguaje 100% descriptivo — sin probabilidad/EV/edge/recomendación/pick/banker (test F2 con bordes de palabra, literal E5 strippado); determinismo: sin timestamps, ties por (market, line, subject), doble render byte-idéntico.
- `venue_role` + prefijo `home_`/`away_` = rol del sujeto en este fixture y `history[]` = valor propio → `own_windows`/`venue_split`/evidencia se recalculan desde la DB; `total_*` solo `total_goals` es reproducible (K2 agrupa por (market, line), subject "Home vs Away", `subject_role=null`).
- Evidencia (P11): `sequence` = hasta 10 valores crudos ascendentes (mismo conjunto que `last10`, sin ceros rellenados), `windows` del mismo `FmWindowCalculator` que `own_windows`, `hits:null` si `n<3/4`; inclusión solo con dato observado; **EvidenceCap 30**.

## Pendientes / riesgos
- **C3**: kickoff nulo en pre-match (no hay fecha persistida en `fm_signal`) — documentado, nunca inventado; `POST /api/fm/fixtures` no sirve esta liga.
- Re-resolve de los 108 NO_HISTORY_ELEMENT cuando FM ingiera history del 2026-09-24; tope 30 deja fuera props con `all.n` pequeño (`EvidenceCap`/`ConfluenceCap`).
- Higiene de git: 4 `.txt` trackeados modificados sin staging + `scripts/p8_b_intercept_player.py` sin trackear; `teams/table` no valida `stat`; canary SUSPECT (drift) en algunos tabs.
