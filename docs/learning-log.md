# Learning Log - MatchEdge + Udemy

> Objetivo: Trazar cada módulo de Udemy con un PBI de GitHub. Evidencia de que aprendiste y luego aplicaste, no que la IA hizo todo.

| Fecha | Módulo Udemy | PBI | PR | Aprendizaje con mis palabras |
|---|---|---|---|---|
| 2026-09-26 | .NET Backend Bootcamp - Windows Service & DI Lifetimes | PBI 1.1 | - | Singleton en BrowserManager deja Chrome abierto = leak. Scoped + IAsyncDisposable |
| 2026-09-26 | .NET Backend Bootcamp - Outbox & Idempotency | PBI 1.2 | - | Re-key (subject_type, subject_name, market, line) evita duplicar total_corners |
| 2026-09-26 | .NET Backend Bootcamp - External Integration + HttpClient | PBI 2.1 | #40 | Interceptar XHR teams/table y position-stats, guardar raw en tmp/fm/, documentar endpoint |
| 2026-09-26 | Modular Monolith - DDD Parser / Vertical Slice | PBI 2.2 | #40 | Mapear tackles/fouls/saves a stats_json, y descubrir que el pivot de ataque ya traía shotsCreated/chancesCreated/penalties (lo que "faltaba" era una fila, no un campo) |
| - | Modular Monolith - Deduplication | PBI 2.3 | - | total_* una sola entrada por (market,line) |

## PBI 2.1 Parte B - 2026-09-26
- Austria 321742 period=30 supersub=true
- Endpoints interceptados, counts, closure_met true

### Detalle de ejecución
- Script: `scripts/p8_b_intercept_player.py` (fetch real vía proxy local
  `http://localhost:5272/api/FootyMetricsTest/fetch/raw`, sesión FM Premium del
  navegador; `FOOTYMETRICS_GUIDE.md` no documenta headers → headers de navegador
  estándar). Throttle `time.sleep(random.uniform(2,4))` entre requests,
  secuencial 1 a 1 (P7). 32 requests, 0 paralelos, solo GET (P4-P6: sin Betano,
  sin EV, solo lectura descriptiva).
- Endpoints usados: `/api/front/position-stats?team=18643&positions={ST,RW,LW,CM|CB,GK}&stat={slug}&period=30&venue={home|away}&perspective=for&superSub=true`
  (chunks ≤4 códigos por límite de la API; `location=home,away` → `venue=home|away`)
  y `/api/front/teams/table?stat=…&id=18643&period=30&location=both&group={discipline|defense}…`
- Endpoints del PBI probados literalmente y descartados (detalle en
  `tmp/fm/p8_b_inventory.json → pbi_2_1_b.requested_endpoints_probed`):
  `/teams/table?country=Austria&season=2025` → 404; `/api/front/teams/table?…`
  → 400; `/position-stats?competition=321742&…` → HTML; `/api/front/position-stats?…`
  → 400 (usa `team=18643`, `positions=`, `venue=`, `stat=` en singular).
- Slugs PBI → FM: `tackles→tackles`, `foul_involvements→fouls-involvements`,
  `fouls_committed→fouls-committed`, `goalkeeper_saves→saves`,
  `fouls_drawn→fouls-drawn` (400) → **`fouls-won` (200, campo `foulsD`)**.
- Counts (`tmp/fm/p8_b_<stat>_player.json`, 136 apariencias por stat):
  tackles 136 (69 con value>0), foul_involvements 136 (85), fouls_drawn 136 (61),
  fouls_committed 136 (63), goalkeeper_saves 136 (31).
- `closure_met = true` (5/5 stats con count ≥ 10, regla ≥3).
- Raw e inventario en `tmp/fm/` (`.gitignore` línea 85 `tmp/fm/`, sin ficheros
  trackeados bajo `tmp/`).

## PBI 2.1 Parte C - 2026-09-26
- `player.missing_or_non_numeric` **147 → 11** (< 20 ✔);
  `team.not_reproducible` **38 intacto** ✔; `grand_total_flagged` 185 → 49.

### Detalle de ejecución
- **Root cause**: `fm_player_matches` solo tenía filas `stat=corners`
  (`group=attack`), que no traen `tackles/foulsC/foulsD/fi/saves`
  (0 presencias en 12.192 filas). `FmTeamController.Collect` fijaba
  `group=attack`, así que el mapper exigía campos que nunca se recolectaron.
- **Fix mapper**: nuevo `src/MatchEdge.Infrastructure/Services/FmPlayerStatMap.cs`
  (mercado → slug → campo → `group`); `TryPlayerValue` prueba la lista de
  campos en orden y mantiene el literal
  `stat 'X' missing or non-numeric` que usa el clasificador P8-A.
- **Fix recolección**: `Collect` deriva `group` del stat (defense/discipline/
  attack), acepta `stat` en lista (`"tackles,fouls-committed"`) y el alias del
  PBI `venue=home,away` (además de `locations`).
- **Fix dedup de equipo**: `DedupByFixtureForMarket` elige por fixture la fila
  cuyo `team_stats_json` aporta el stat del mercado (una misma fixture puede
  tener 3 filas, una por `stat`); antes la fila más reciente tapaba la de
  corners.
- **Hallazgo durante la validación**: el primer cierre dio **77**, no < 20.
  Añadir filas defense/discipline con `period=30` añadió 295 fixtures antiguos
  sin filas de ataque; `BuildPlayerSeries` tomaba el `skip` de *cualquier*
  fila de la serie y el motivo de un fixture viejo marcaba la señal entera
  (23 `shots` + 35 con `NO_HISTORY_ELEMENT` eran falsos positivos).
  Fix: el `skip` solo se toma de la fila del fixture actual.
- **Recolecto**: 14 equipos × `stats=["tackles","fouls-committed"]` ×
  `locations=["home","away"]`, `period=30` → 14 `POST /api/fm/teams/{id}/collect`,
  56 fetches secuenciales (2-4 s), 0 errores
  (`tmp/fm/p8c_backfill_log.json`). Antes de reiniciar la app se precapturaron
  28 raws por `fetch/raw` como seguro de sesión FM
  (`tmp/fm/p8c_raw/`, `tmp/fm/p8c_fetch_insurance.json`): 0 fixtures de los 7
  validados quedaban sin cubrir.
- **Validación**: regenerados los 7 `tmp/fm/p8_a_report_3344181*.json`
  (baseline respaldado en `tmp/fm/p8_a_baseline/`) →
  `node tmp/fm/p8_a_analyze.js` → player 11, team 38. Ventanas: 140 señales
  ganan puntos, 0 pierden; `last5`/`last10` idénticos al baseline.
- **Los 11 restantes son reales**: jugadores con `mins=0`/`rating=null` en el
  fixture actual (Musiala, Timber, L. Karl, M. Abu Fani) → FM devuelve `null`.
- Tests: `dotnet test --filter Fm` → **54/54** (1 nuevo
  `Report_PlayerStatMissingOnlyInOlderFixture_DoesNotFlagSignal`).
  Detalle y decisiones en `docs/adr/ADR-002-player-missing-mapping.md`.

## PBI 2.2 (P9) - Cierre residual P8-A + unmapped restantes - 2026-09-26
- Residuo estable: re-ejecutado `node tmp/fm/p8_a_analyze.js` sobre los 7
  reportes regenerados → player **11** / team **38** / grand **49**,
  idéntico al cierre del PR #40. Los 11 no son bug de mapper: fila a fila en
  la DB todos tienen `mins=0` o el campo nulo (Musiala, Timber, L. Karl,
  M. Abu Fani). Los 38 son `total_*` por diseño: `team_stats_json` guarda
  solo stats propias del equipo y solo `total_goals` tiene análogo directo
  (`hgoals+agoals`), que por eso no aparece en la lista.
  Evidencia: `tmp/fm/p9_a_residual.json`.

### Detalle de ejecución
- **El brief decía que `shots_created`/`chances_created`/`penalties`
  seguían sin campo; no era cierto**: el pivot de ataque (`group=attack`)
  ya los traía (704/721 filas no nulas en Austria y Portugal) y ya estaban
  en `FmPlayerStatMap`. En los 7 fixtures solo `shots_created` emite señales
  (10, todas con dato) y `chances_created`/`penalties` no tienen filas en
  `fm_signal` → por eso el clasificador da 0 `unmapped`. `docs/STATE.md`
  llevaba una frase al revés y quedó corregida.
- **Sonda live** (19 requests secuenciales, 2-4 s, 0 paralelo, vía proxy
  local): `position-stats` valida slugs (400 en `chances-created`,
  `penalty-won`, `penalty-conceded`), pero `teams/table` acepta cualquier
  `stat` (control `zz-bogus-p9` → 200) → un 200 en `teams/table` **no**
  prueba que un slug exista; solo `position-stats` valida.
- Slugs reales del dropdown de 45: `shots-created` y `penalties` (200 con
  `value` no nulo en los 2 equipos) → añadidos a `SlugByMarket`.
  `chances_created` queda pass-through: FM no tiene slug y `key-passes`
  (200) solo se apoya en una igualdad observada, no inventada
  (`keyp` == `chancesCreated` en 44/44 filas comparables de Austria).
- Tests: `dotnet build` 0 errores; `dotnet test --filter Fm` → **58/58**
  (+3 del theory de mapper, +1 de reporte que además demuestra que un
  mercado sin mapeo sigue saliendo `unmapped` con su motivo).
- Reporte en vivo (D3): doble GET de JSON y .md byte-idénticos, 0 palabras
  prohibidas fuera del literal de E5, 0 duplicados `total_*` (5 entradas,
  un solo subject "Portugal vs Wales" con `subject_role=null`).
- Evidencia: `tmp/fm/p9_a_residual.json`, `tmp/fm/p9_b_unmapped_probe.json`
  (+ crudos en `tmp/fm/p9_b_raw/`), `tmp/fm/p9_d_report_check.json`,
  `tmp/fm/p9_d_report_33441811.{json,md}`.

## Notas P8 - Parte A completada
- tmp/fm/p8_a_unmapped.json: 0 unmapped, 147 missing/non-numeric player, 38 not_reproducible total_* (total_corners 24, total_shots 7, total_shots_on_target 5, total_offsides 1, total_tackles 1) todos INSUFFICIENT_SAMPLE
- Player: tackles 40, foul_involvements 30, fouls_drawn 26, fouls_committed 22, goalkeeper_saves 19, shots 4, sot 3
- P4-P7 respetados: solo 1/odds descriptivo, sin Betano/EV, navegación secuencial 2-4s
