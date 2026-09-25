# Handoff — MatchEdge / FootyMetrics (P1 + P2) para revisión externa

Rama: `feature/fm-deterministic-navigation` (sin push; commits locales).
Rama de base: no se tocó `main`. Trabajo en fecha 2026-09-24.

## 1. Alcance y límites (briefs)

- **P1** = `docs/new 6.txt` (Fase 0+1: navegación determinista FootyMetrics + endpoints snapshot). Aceptado en sesión (T1–T6).
- **P2** = brief `FM_AGENT_BRIEF_P2` entregado **inline en la conversación** (NO está en el repo): V1–V7 verificaciones, B1–B3 cambios, §3 persistencia/validación, §4 resolver de outcomes, §5 `POST /api/fm/collect`, aceptación A1–A6, `docs/STATE.md` (≤30 líneas) y reporte final ≤25 líneas → PARA.
- **Límites respetados:** solo FootyMetrics. NO probabilidades/scoring/picks/EV/staking, NO Betano, NO OAuth automation. URLs manuales prohibidas (todo desde API/DOM/rutas descubiertas).
- **Descubrimiento completo previo:** `docs/fm-discovery.md` (D1–D7 + V1–V7, con evidencias). Leer D3–D5 estaba permitido en P2.

## 2. Commits (13 en la rama)

| commit | contenido |
|---|---|
| `e1da207` | docs: descubrimiento FM Fase 0 (D1–D7) con evidencias |
| `0a02809` | `FmNavigator` (serializado, reintentos, errores tipados) + `FmFixtureResolver` (JSON/RSC, sin URLs manuales) |
| `2324e58` | esquema idempotente `fm_snapshot`/`fm_signal`, parser JSON, canary, snapshot service |
| `a3c9970` | endpoints `/api/fm` fixtures\|snapshot\|run, DI, `/eval` solo Development, fixture-trends-v2 obsoleto |
| `1f95b6a` | tests FM deterministas; fix de errores de compilación preexistentes (Backtesting/OddsMatching) |
| `e26634f` | `NavigationGate` compartido (FmNavigator + scraper legacy); fetch del resolver vía `context.APIRequest` |
| `cffa1fd` | captura de HTML bajo el gate (evita navegación intermedia de otro proceso) |
| `1cdd5ad` | reintentos acotados + logging de inner-exception en fetches APIRequest |
| `161cc67` | B1: `/navigate`+`/intercept` solo en Development; B3: flag `Pipeline:LegacyWorker:Enabled` (default true) |
| `c179aa7` | B2: endpoint dev-only `probe-nonpremium` (V6, perfil Chrome vacío) |
| `a0d46dc` | **§3** + V1–V7 documentados: `FmSignalValidator`, `params_json`, `leakage_flag`, `status`/`motivo`, migraciones PRAGMA, V6 `DEGRADED` |
| `6757b07` | **§4**: tabla `fm_outcome` + `POST /api/fm/outcomes/resolve` (D0 orden 1 history / orden 2 stats-panel+fixtures-api, fetches gate-wrapped, 0 navs, idempotente por señal) |
| `a13f9d0` | mapping `home_saves`/`away_saves`→"Goalkeeper saves"; `*_cards`→AMBIGUOUS |

Build: **0 errores**. Tests: **23/23** en `FmDeterministicTests`
(`dotnet test tests\MatchEdge.InfrastructureTests\MatchEdge.InfrastructureTests.csproj --filter FullyQualifiedName~FmDeterministic`).

## 3. Verificaciones P2 (V1–V7) — detalle y evidencias

Evidencias en `tmp/fm/` (brutas en `C:\Services\MatchEdge\tmp\f m\snapshots` cuando el raw se guardó en runtime).

- **V1** — `tmp/fm/v1_unl_day.json`: tabla del día UNL + criterio de estado de fixture (finished/FT) usado por §4/§5.
- **V2** — campos de `history[]` de trends: `t` (fecha t DESC), `l` (línea), `n` (muestra), `met`, `v` (valor), `cnt` (aciertos), `e` (equipo/opp), `m` (minutos jugados). **No** hay `fixtureId` por elemento de history → "NO ENCONTRADO" (documentado).
- **V3 (hallazgo clave)** — `tmp/fm/v3_rule_check.json` (20/20), `v3_recompute.txt`:
  `bestCount/bestTotal` = **mejor ventana reciente = sufijo de los `k` elementos más nuevos** de history (`vals[0..k-1]`), con `k ≥ min_matches` (**equipos=4, jugadores=3**), maximizando el hit-rate; empate → ventana más larga. **No** es toda la historia salvo cuando `bestTotal==len(history)`. Además `count(met==true)` == recompute estricto `v>line` (20/20).
  UI renderiza exactamente `bestCount/bestTotal` → evidencia `tmp/fm/v3_ui_teamtrends.txt` (nav #1, 1 uso de presupuesto).
- **V4** — líneas/markets observados: equipo `away/home/total` × `corners|goals|shots|shots_on_target` (+`home_saves`, `home_cards`); jugador `fouls_committed, goalkeeper_saves, offsides, shots, shots_created, tackles`.
- **V5** — `tmp/fm/v5_variants.json` + `v5_compare.json`: `location` y `league_only` via `ScopesFromUrl`; venue rows `location=match` vacío → venue split local vía `h` (lado del fixture).
- **V6** — `tmp/fm/nonpremium/20260924212808227.json`: perfil no premium = sin redirect a `/login`, API trends en silencio (`NO_RESPONSE_15S`), página muestra "Upgrade | Sign in" → **`DEGRADED`** implementado en `FmFixtureSnapshotService` (catch: `Timeout|NoData|PageNotReady` + markers de paywall).
- **V7** — `tmp/fm/v7_global_fixtures.json`: `GET /api/front/fixtures?date=YYYY-MM-DD` es un ARRAY de ligas con `fixtures[]`, funciona sin auth → lo usa §5 cuando no se pasan `leagues`.

**Presupuesto de navegación §1: 2/15 usados** (V3 UI, V6 probe). `/eval` y `/fetch/raw` = 0 navs.

## 4. §3 — Persistencia y validación (commit `a0d46dc`)

- `fm_signal`: + `params_json` (`{"location","league_only"}`), `status` (`OK|SUSPECT`, DEFAULT OK), `motivo`. `fm_snapshot`: + `leakage_flag` (`sourceTs > KickoffUtc`). Migraciones idempotentes con `PRAGMA table_info` (`EnsureColumnAsync`).
- `FmSignalValidator.Validate` → `FmSignalInsertResult(Inserted, Suspect, FirstMotivo)`:
  - history inparseable / campo faltante / no numérico → SUSPECT;
  - `sample_size > len(history)` → SUSPECT;
  - **recompute de la ventana** `(hits,window) != (bestCount,sample)` con kmin team=4/player=3 → SUSPECT;
  - las señales **nunca se descartan**; si hay sospechosas → snapshot `status='SUSPECT'` + warning.
- Leakage: snapshot con `sourceTs > KickoffUtc` → `fm_snapshot.leakage_flag=1` (candidato a exclusión en backtests, aún sin gate de uso).

## 5. §4 — Resolver de outcomes (commits `6757b07` + `a13f9d0`)

- **Tabla** `fm_outcome(id, signal_id UNIQUE, fixture_id, resolved_at_utc, actual_value, hit, status[RESOLVED|NOT_PLAYED|UNAVAILABLE|AMBIGUOUS], source, motivo)` — idempotencia por `signal_id` (`INSERT OR IGNORE`).
- **Endpoint** `POST /api/fm/outcomes/resolve {date?, fixtureIds?}` → `IFmOutcomeResolver.ResolveAsync`; orden:
  1. **D0-1 history**: elemento de `history[]` con `t` = fecha de kickoff y `opp ∈ {home,away}`; `m==0` → `NOT_PLAYED` (nunca `hit=0`); si no está → `UNAVAILABLE` (lag de ingesta FM) y el resolver **no inventa** el valor.
  2. **D0-2**: stats panel del HTML server-rendered del overview (labels `[v1|label|v2]`, **v1 = local** verificado por coordenadas x) + **goles del fixtures-api del listado global** (`homeGoals/awayGoals`).
- Fetches con `context.APIRequest` (0 navegaciones, bajo el `NavigationGate`). `T1/D2: kill switch` NO aplica (0 navs).
- **Mapping mercado→label**: corners/shots/shots_on_target/fouls/saves/offsides (+`home/away_saves`); `*_goals` → fixtures-api; `tackles`→AMBIGUOUS (Total tackles vs Tackles won); `*_cards`→AMBIGUOUS (Yellow vs Yellow+Red).
- **Semántica de actuals**: `home/away/total_*` = lado del fixture (`home`=local del partido, no venue del sujeto). Coherente con filas venue (solo existen cuando el sujeto coincide con el lado) y verificado con casos dobles (Serbia `away_goals`=2 y Greece `home_goals`=1 para el mismo 1-2).
- **Parser de stats (`ParseStats`)**: strip de `<!-- -->`, regex anclado `>([^<>]+)</span><span class="text-xs font-medium text-text-secondary">([^<>]+)</span><span class="text-sm font-semibold tabular-nums text-text-primary">([^<>]+)</span>`, y **moda determinista** para paneles duplicados (idénticos → ok; divergentes → más frecuente; empate → se descarta label → AMBIGUOUS/UNAVAILABLE).
- **Resultado final (run3, `tmp/fm/resolve_run3.json`)**: fecha 2026-09-24, 22–23 fixtures finalizados, **navs 0/40**, idempotencia `already=176`; **`RESOLVED=70, UNAVAILABLE=108 (100% player: lag de history), AMBIGUOUS=1 (home_cards)`**. Breakdown por fixture: 33441811 (55 señales), 33441812 (72), 33441813 (52).
- **A5 (3 señales vs UI)** — cross-check con evidencia UI guardada:
  - Serbia `total_corners L5.5 actual=8` ↔ UI "Corners 8" + filas `2|Corners|6` (`d0_overview_rendered.txt`, `d0_stats_dom.json`);
  - Greece `away_shots L12.5 actual=26` ↔ filas `9|Total shots|26`;
  - goles: score UI `1 - 2` ↔ `home=1, away=2, total=3` (fixtures-api). Todos consistentes.

### Bugs encontrados y corregidos en §4 (transparencia)

1. `ObjectDisposedException JsonDocument` → clonar elementos antes de cerrar (`e.Clone()`).
2. Regex de stats con `(.*?)` capturaba HTML basura (group1 cruzaba tags) → filas descartadas silenciosamente; run1 dio solo 14 RESOLVED y 31× "label 'Corners' not present" pese a existir en el HTML. **Fix**: anclaje `[^<>]+` + comment-strip + moda.
3. Run1 tenía outcomes escritos con el bug → se ejecutó `DELETE FROM fm_outcome` (data-fix documentado, no silencioso) y se re-ejecutó; luego `DELETE` solo de filas `not mapped` tras el mapping fix.

## 6. Aceptación A1–A6 y pendientes

- **A5**: HECHO (ver §5, cross-check 3 señales vs UI con evidencias).
- **A3**: PENDIENTE — deployment a `Production` (hoy corre `ASPNETCORE_ENVIRONMENT=Development`), verificar `/navigate`+`/intercept` → 403/404 en Production, restaurar default del worker (`Pipeline__LegacyWorker__Enabled=true`, hoy se lanza con `=false`), documentar toggle en STATE.md.
- **A1/A2/A4/A6**: **estado a confirmar contra el brief P2 original** (el texto inline no está en repo). No se debe dar por hecho sin releer el brief.
- **§5 `POST /api/fm/collect`**: NO EMPEZADO. Plan: `CollectAsync(date, leagues?, maxNavs=40)`, resolución vía V7 cuando `leagues` omitido, 2 navs/fixture (tabs jugador+equipo; overview solo si faltan nombres, con fetch RSC), reanudable por `(fixtureId, tab)`, no superar `maxNavs` → reporta `pending`, saltar fixtures con kickoff pasado salvo `leakage_flag`, tests + commit.
- **`docs/STATE.md`** (≤30 líneas) y **reporte final ≤25 líneas** → PARA: pendientes.

## 7. Decisiones/desviaciones para que el revisor ponga el foco

1. **V3**: el brief (según resumen de sesión) planteaba `len(history) != sample_size` como validación; se implementó `sample_size > len(history)` porque el descubrimiento mostró que `sample_size` es la **ventana elegida** (8/10), no el largo de history. Riesgo: si el brief quería otra cosa, la validación cambiaría.
2. **`status`/`motivo` viven a nivel de `fm_signal`** (y el snapshot hereda `SUSPECT` si hay ≥1 sospechosa); el brief hablaba quizá solo de snapshot.
3. **Idempotencia de `fm_outcome` por `signal_id`** (no por fixture): re-resolver no reescribe; para re-resolver hay que borrar filas.
4. **"usa gate+budget" interpretado como fetches APIRequest bajo el gate con `navigationsUsed=0`** (no consume navs). Si el brief exigía consumir presupuesto real, cambiar.
5. **Goles salen del fixtures-api del listado**, no del overview (el stats panel no incluye marcador). Desviación menor de "D0 order 2: overview", documentada.
6. **Player outcomes**: `UNAVAILABLE` por lag de ingesta (FM no tiene aún el elemento del partido en history). `NOT_PLAYED` implementado (`m==0`) pero sin casos reales hoy.
7. **`ParseStats` con moda** para paneles duplicados/divergentes (determinista, pero es una política propia, no del brief).
8. **`DEGRADED` por markers "Upgrade|Sign in"**: vistos solo en perfil no premium; riesgo residual de falso positivo si esos markers aparecen en páginas premium.
9. **Outcome actuals = verdad de FM** (panel/listado FM), no una fuente externa; si FM se contradice (p.ej. `Portugal home_saves=0` con `Wales SOT=1` y 3 goles), se toma el panel FM tal cual.
10. **Tests preexistentes** de Backtesting/OddsMatching tocados en `1f95b6a` solo por errores de compilación previos (fuera del alcance FM, necesario para que el test suite corra).

## 8. Entorno y operación (para reproducir)

- Deploy: `C:\Services\MatchEdge` (publish -c Release), BD `C:\Services\MatchEdge\matchedge.db` (sqlite3 CLI disponible).
- Arranque (desde `C:\Services\MatchEdge`, tras cerrar el listener del 5272):
  `cmd /c set ASPNETCORE_ENVIRONMENT=Development&& set Pipeline__LegacyWorker__Enabled=false&& dotnet MatchEdge.Api.dll > C:\Services\MatchEdge\run.log 2>&1`
- **Tras cada restart** (siempre): `POST /api/FootyMetricsTest/start` y `GET /api/FootyMetricsTest/login-status` → `loggedIn=true`.
- Playwright 1.62: `context.APIRequest`, `APIRequestContextOptions.Timeout`, `page.EvaluateAsync<string>`.
- PowerShell 5.1 (sin `??`); `2`-`4` s de pausa entre navs; nunca paralelizar navegación.
- Evidencias clave en `tmp/fm/`: `v3_rule_check.json`, `v3_recompute.txt`, `v3_ui_teamtrends.txt`, `v5_variants.json`, `v5_compare.json`, `v7_global_fixtures.json`, `d0_overview_postft.html`, `d0_overview_rendered.txt`, `d0_stats_dom.json`, `d0_teams_postft.json`, `d3_player_intercept.json`, `d5_team_intercept.json`, `nonpremium/20260924212808227.json`, `resolve_run3.json`.
- Estado al cierre de este handoff: API arriba en 5272 (Development, worker off), navegador FM con sesión iniciada, `fm_outcome` = 70/108/1, presupuesto navs P2 = 2/15.
