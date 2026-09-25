# REPORT_P5 — MatchEdge / FootyMetrics (cierre P5) + ejecución P6 (G/H)

Rama `feature/fm-deterministic-navigation`. Estado vivo: `docs/STATE.md`. Evidencia: `tmp/fm/`.

## P5 — aceptación (una línea por ítem)
- A1: payload `data[].odds` mapea `bk` numérico (1..5) → lado over/under; sin nombre de casa.
- A2: búsqueda exhaustiva de "Bet365/Kambi/…" en 66 JSONs de trends, RSC del fixture y `/api/front/bookmakers*` → **sin nombre en ninguna API**.
- A3: nombre encontrado en el chunk Next.js `/_next/static/chunks/3ji7afwyc_r93.js` (`{1:Bet365,2:Kambi,3:Paddy Power,4:Ladbrokes,5:Altenar}`) — 1 nav (`tmp/fm/p5_a3_bk_mapping_nav.json`).
- A4: `fm_bookmakers` sembrada + backfill de `fm_market_odds.bookmaker_name` (1.996/1.996, 0 null).
- A5: `GET /api/fm/fixtures/{id}/odds` devuelve `bookmakerName` (`tmp/fm/p5_a5_odds_endpoint_check.json`).
- B1-B3: `venue_role` (home|away|unknown) derivado de `Team.name` vs `Fixture.Home/Away.name` y persistido en `fm_signal`; verificado 7/7 fixtures (`tmp/fm/p5_b3_verification_all7.json`); 366 señales con venue_role (147/219/0).
- B4: distinción sin ambigüedad documentada en `STATE.md` — estructural = el partido actual (`venue_role`); histórico = split `home/away` de `history[]`.
- C1: lista explorados vs no explorados (`tmp/fm/p5_c1_explored_vs_not.json`); `fixtures?date=` con fecha pasada descartado.
- C2: **venue histórico CONFIRMADO**: `history[].h` vs `teams/table?location=` → Portugal 10/10, Wales 10/10, 0 discrepancias; 4 navs dirigidas + payloads (`p5_c2_endpoints.json`, `p5_c2_table_*`, `p5_c2_position_stats.json`).
- C3: diseño de `fm_team_matches`/`fm_player_matches` fijado (implementado en P6, ver abajo).
- C4: `manual-odds` (P4) y exploración de `teams/odds`, `teams/lineup` (vacío → descartado).
- D1-D3: `docs/FM_MANUAL_NAVIGATION.md` con guía y ejemplos; usuario contrasta URLs manuales contra lo automático.
- Corrección de hecho: fixture **33441811 = Portugal vs Wales** (el brief lo llamaba Norway-Portugal; ese es 33608044).

## P6 — G (persistencia) y H (validación doméstica)
- G1: `fm_team_matches` + `fm_player_matches` (índices únicos) en `FmSnapshotStore.EnsureSchemaAsync` + `FmTeamTableParser` (deriva `location` de `hid/aid`, junta `teamStats[fixture][teamApid]`, `opponentStrength`, `pivotData`→jugadores con nombre desde `players[]`).
- G2: `POST /api/fm/teams/{teamApid}/collect` — `teams/table` por fetch directo bajo el navigation gate (**0 navegaciones**), upsert idempotente, raw en `tmp/fm/teams/<apId>/`. Poblado: **14 selecciones** (fixtures 33441811-17) + **4 clubes** de H → **540 filas de equipo (270 home / 270 away) y 12.192 de jugador / 1.082 jugadores**.
- G3: `GET /api/fm/teams/{teamApid}/matches?location=&period=&includePlayers=` — solo lectura, sin navegador (smoke: Gales 15 partidos + 347 filas de jugador, `tmp/fm/p6_final_counts.json`).
- G4: Portugal y Gales — **20/20 history[] con location correcta, 0 discrepancias** (`tmp/fm/p6_g4_crosscheck.json`, idéntico a la referencia P5).
- H1: 2 ligas domésticas exploradas por API (0 navs): League Two/England (apid 14) y La Liga 2/Spain (apid 567), fixtures con `hasTrends=true`.
- H2: 4 clubes (Exeter City, Bristol Rovers, Granada, FC Andorra) — **30/30 history[] coincide con `fm_team_matches`, 0 discrepancias** (`tmp/fm/p6_h2_crosscheck.json`).
- H3: discrepancias documentadas abajo.

## Riesgos y discrepancias (máx. 5)
1. Nombres de rival no sirven de clave: trends usa el largo ("Rotherham United") y `teams/table` el corto ("Rotherham"); 9 variantes en H2 → cruce **por fecha** (equipo = 1 partido/día). El mojibake (`LeganÃ©s`) está solo en el archivo capturado por `Invoke-WebRequest` sin charset, no en la respuesta de la app.
2. `opponentStrength` llega `{}` para el fixture más reciente (33441811) y con datos para los anteriores; se persiste tal cual.
3. `position-stats` exige `positions=` (vacío → 400; `ST,RW,LW` OK) → `fm_player_matches` hoy solo con perspectiva `team` (fuente `pivotData`); perspectiva rival pendiente.
4. `fixtures/league?id=` solo devuelve ventana corta (7 días para 1538) y `[]` para las ligas domésticas; `fixtures?date=` responde hoy/ayer (+1) → vía H = `fixtures?date=` → `trends/fixtures/<apid>/teams`.
5. Canary SUSPECT (market-mix drift) en algunos team/player tabs y `source_conflict=4` en 33441811 (preexistente); `DateTeamMatchingIntegrationTests` 6 FAIL preexistentes.

## Tests / navegación
- `FmDeterministicTests` **46/46 PASS** (5 nuevos P6: parser con payloads reales home/away, location por ids, upsert idempotente + lectura, filtro por fixture) · `MatchEdge.UnitTests` 149/149 PASS.
- Navegación: P6 **0 navegaciones**; P5 4 navs + ~12 fetches + 12 navs de reprocesado. Siempre 1 a la vez, pausa 2-4s, nunca paralelo.
