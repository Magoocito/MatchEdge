# REPORT P4 (A+B+C) — 2026-09-25 · rama feature/fm-deterministic-navigation · tests 37/37 · PARA antes de D
A1. Las 108 NO_DATE_MATCH (3 señal-team/player verificadas, Δ 80/109/182d, 0 elementos con rival del fixture) = falta de historia, no fecha → NO_HISTORY_ELEMENT (evid. tmp/fm/p4_a1_manual_check.json).
A2. Clasificación corregida: MatchHistory() puro/ testeable (vacío→NO_HISTORY_ELEMENT, rival±1d→NO_DATE_MATCH con detalle, extracción fallida→OTHER); ComputeOutcome consume el detalle.
A3. Resolver re-corrido: 108 → 100% NO_HISTORY_ELEMENT (detalle exacto Δ/10), newlyResolved=0 (lag FM), RESOLVED=70 intactos; evid. tmp/fm/p4_a3_*.json.
B1. Descubrimiento sin navs: location=all≡base; location=match→data:[] (0 filas, 5 apids × ambos scopes); location=home|away→400; venue estructural (Team vs Fixture.Home/Away) existe sin navegar (tmp/fm/p4_b1_*.json).
B2. El SPA **borra location en soft-nav** → scope match capturado por fetch API directo (0 navs extra), NO por navegación.
B3. Persistencia: snapshot `<tab>+loc=match` con apiPath/tab_url/params `location=match` en paralelo al `location=all` (20 EMPTY match vs 42 OK+12 SUSPECT all; señales con params `{"location":"all"}`).
B4. Service dual-scope (trendUrls→WithLocationMatch→FetchJsonAsync), /run documentado 3 navs/fixture (Max/3=13/run, sin límite duro).
B5. 33441811-17 reprocesados: 7/7 dual OK (ej. 33441811: OK/36+SUSPECT/19 base, match EMPTY/0); evid. tmp/fm/p4_b5_snapshot_*.json.
C1. **EXISTEN**: `data[].odds` de tabs trends = array `{bk,over,under,yes,no,most}` (1 entrada/bk id 1-5; yes/no/most null en 1060 arrays escaneados); sin timestamp→capture time; sin mapeo id→casa (404 API, /bookmakers sin ids); evid. tmp/fm/p4_c1_odds_discovery.json.
C2. `fm_market_odds` poblada: 1.247 filas FM / 7 fixtures (bk 3-5, 1.01-6.75), índice único por snapshot, parser array tolerante a objeto (bug obj→array corregido con harness).
C3. N/A — C1 encontró odds.
C4. `POST /api/fm/fixtures/{id}/manual-odds` validado (tipo/rango), JsonPropertyName("odds_value"); inserción real id=1161 (Betano/total_goals/2.5/1.9, source=manual) + 3 rechazos 400.
Riesgos: (1) location=match devuelve data:[] en los 7 fixtures → con venue real vacío solo sirve para auditar "no hay split"; (2) canary SUSPECT/12 tabs = market-mix drift vs baseline P2; (3) sin nombre de bookmaker (solo id); (4) señales legacy sin params_json conviven con las nuevas; (5) 108 NO_HISTORY_ELEMENT pendientes de re-resolve con data FM futura.
