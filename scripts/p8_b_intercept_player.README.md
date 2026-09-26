# `scripts/p8_b_intercept_player.py` — evidencia PBI 2.1 Parte B

Fetch **real** (sin mock) de los 5 stats de jugadores pendientes en
`p8_a_unmapped.json` (137/147 `missing or non-numeric`). Ejecución
`2026-09-26T11:32:01Z → 11:39:04Z`, equipo **team=18643 (321742-austria)**,
`period=30`, `supersub=true`, `positions=ST,RW,LW,CM,CB,GK`,
`location/venue=home,away`.

## BASE URL y headers

- BASE URL: `http://localhost:5272/api/FootyMetricsTest/fetch/raw?apiPath=<urlencoded>`
  (proxy local de la app MatchEdge: reutiliza la sesión FM Premium del
  navegador Playwright con perfil `persistent-footymetrics`; elegida por el
  usuario en la sesión).
- `FOOTYMETRICS_GUIDE.md` no incluye bloque de headers → se envían headers de
  navegador estándar (`Accept`, `User-Agent`, `Origin`, `Referer`).

## Endpoint pattern

```
# position-stats (captura por stat: 2 chunks de posiciones x 2 venues)
/api/front/position-stats?team=18643&positions={ST,RW,LW,CM|CB,GK}&stat={slug}
    &period=30&venue={home|away|both}&perspective=for
    &teamFormations=&oppFormations=&leagues=&superSub=true

# teams/table (2 grupos cubren los 5 stats; `group` decide el payload, `stat` no)
/api/front/teams/table?stat={slug}&id=18643&period=30&location=both
    &group={attack|defense|discipline}
    &selectedLeagues=720,732,1082,1326,1538&half=&dl=true&se=false&sm=easier
```

`positions` acepta **máximo 4 códigos** → 2 chunks (`ST,RW,LW,CM` y `CB,GK`).

## Slugs reales (PBI → FM)

| market PBI | slug usado | HTTP | campo en `stats_json` |
|---|---|---|---|
| `tackles` | `tackles` | 200 | `tackles` |
| `foul_involvements` | `fouls-involvements` | 200 | `foulInvolvements` / `fi` |
| `fouls_drawn` | **`fouls-won`** | 200 | `foulsD` |
| `fouls_committed` | `fouls-committed` | 200 | `foulsC` |
| `goalkeeper_saves` | `saves` | 200 | `saves` |

`fouls-drawn` devuelve 200 **sin `appearances`** (cuerpo de error) → fallback
a `fouls-won`, que sí trae el campo `foulsD` (intento registrado en
`slug_attempts`).

Endpoints pedidos literalmente por el PBI y por qué no encajan (4 sondas,
detalle en `tmp/fm/p8_b_inventory.json → pbi_2_1_b.requested_endpoints_probed`):

| probe | resultado |
|---|---|
| `/teams/table?country=Austria&season=2025` | `{"error":404}` |
| `/api/front/teams/table?country=Austria&season=2025` | `{"error":400}` |
| `/position-stats?competition=321742&…` | HTML (página, no API) |
| `/api/front/position-stats?competition=321742&…` | `{"error":400}` |

## Requests: 32

| parte | requests |
|---|---|
| sondas de endpoints literales | 4 |
| `teams/table` (discipline + defense) | 2 |
| intentos de slug (5 stats + 1 fallback `fouls-drawn`) | 6 |
| captura `position-stats` (5 stats × 2 chunks × 2 venues) | 20 |
| **total** | **32** |

- **Throttle**: `time.sleep(random.uniform(2, 4))` entre requests, siempre
  secuencial, **0 paralelos** (regla P7). 1 request de salud del proxy no se
  registra en el log.
- Solo GET de lectura; sin Betano, sin EV, solo 1/odds descriptivo (P4-P6).

## Conteos (136 filas por stat)

`count_basis = appearances_rows` (filas deduplicadas por
`fid/ts/pos/teamHome`):

| stat | `count` | `count_positive` (value > 0) | chunks OK |
|---|---|---|---|
| tackles | 136 | 69 | 4/4 |
| foul_involvements | 136 | 85 | 4/4 |
| fouls_drawn | 136 | 61 | 4/4 |
| fouls_committed | 136 | 63 | 4/4 |
| goalkeeper_saves | 136 | 31 | 4/4 |

`teams/table`: 57 jugadores en `pivotData` por grupo (defense y discipline).

**`closure_met = true`** — regla: ≥3 stats con `count ≥ 10` (hoy 5/5).

## Salida y uso

- Raw e inventario en `tmp/fm/` (`.gitignore` línea 85 `tmp/fm/`, 0 ficheros
  trackeados bajo `tmp/`): `p8_b_<stat>_player.json`,
  `p8_b_teams_table_<group>.json`, `p8_b_requested_probe_<n>.json`,
  `p8_b_execution_log.json` y el agregado `p8_b_inventory.json` (bloque
  `pbi_2_1_b`).
- Ejecución: `py scripts\p8_b_intercept_player.py` (requiere la app en el
  puerto 5272). Recálculo sin red desde los raw ya capturados:
  `py scripts\p8_b_intercept_player.py --recompute`.

## Relación con la Parte C

Estos 5 slugs son los que `src/MatchEdge.Infrastructure/Services/FmPlayerStatMap.cs`
usa para derivar `group` y leer `stats_json`; ver
`docs/adr/ADR-002-player-missing-mapping.md`.
