# P14 - Evidence: UEFA Nations League 29/09/2026

Evidencia de captura, verificacion y entregable del ciclo P14. Todos los
valores leidos de la API o de la DB (`C:\Services\MatchEdge\matchedge.db`);
nada estimado ni inventado. Sin navegaciones nuevas tras la captura.

## 1. Precondicion FindLeagueApid (Issue #45)

- Payload real: `GET /api/front/leagues` (HTTP 200, 6374 B, 21 ligas) en
  `%TEMP%\opencode\leagues_p14.json`.
- `FindLeagueEntry("UEFA Nations League")` -> `LeagueApid=1538`; la variante
  parcial `Nations League` -> `1538`; inexistente -> excepcion (fail-closed).
- El supuesto bloqueo de Issue #45 no se reproduce con el payload actual.

## 2. Fixtures del dia (fuente unica)

- `GET /api/front/fixtures?date=2026-09-29` (HTTP 200, 92353 B): 10 fixtures
  `leagueApid=1538`, `state=1 status=NS hasTrends=true`.
- Kickoffs (JSON-LD `startDate` del tab overview persistido):
  16:00Z x2 (Finlandia-Bielorrusia, Moldavia-Islas Feroe), 18:45Z x8.

## 3. Ruta de captura y limitacion descubierta

- `GET /api/front/fixtures/league?id=1538` -> `[]` (ventana vacia), igual con
  `id=586352`. Como `POST /api/fm/run` y `POST /api/fm/fixtures` dependen de
  `FmFixtureResolver.ResolveAsync` (ese endpoint), no encuentran fixtures hoy.
- Ruta usada: `POST /api/fm/snapshot {"url":"/fixtures/<slug>"}` ->
  `DescribeAsync(url)` (1 navegacion) + tabs overview/player-trends/team-trends
  (3 navegaciones) = 4 navs por fixture. Presupuesto `MaxNavigationsPerRun=40`
  es por llamada HTTP (`ResetRun` por peticion).

## 4. Snapshots (10/10, `partial=False`)

| fixture_id | equipo | tabs | 1a captura UTC | kickoff UTC | pre | leakage |
|---|---|---|---|---|---|---|
| 33700898 | Espana-Croacia | 5 | 04:39:17 | 18:45 | si | 0 |
| 33700899 | Rep. Checa-Inglaterra | 5 | 04:40:04 | 18:45 | si | 0 |
| 33700901 | Escocia-Suiza | 5 | 04:40:46 | 18:45 | si | 0 |
| 33700896 | Finlandia-Bielorrusia | 5 | 14:19:06 | 16:00 | si | 0 |
| 33700895 | Moldavia-Islas Feroe | 5 | 14:19:54 | 16:00 | si | 0 |
| 33700903 | Bulgaria-Estonia | 5 | 14:21:00 | 18:45 | si | 0 |
| 33700902 | Luxemburgo-Islandia | 5 | 14:22:01 | 18:45 | si | 0 |
| 33700905 | San Marino-Albania | 5 | 14:23:59 | 18:45 | si | 0 |
| 33700904 | Eslovaquia-Kazajistan | 5 | 14:25:11 | 18:45 | si | 0 |
| 33700900 | Eslovenia-Macedonia del Norte | 5 | 14:25:47 | 18:45 | si | 0 |

- Tabs por fixture: `overview` OK (0 senales), `player-trends`,
  `team-trends`, `player-trends+loc=match` y `team-trends+loc=match`.
- Los 5 scopes `+loc=match` devolvieron `EMPTY` (0 filas; respuesta directa
  de API `location=match`) en los 10 fixtures.
- `fm_signal` (senales persistidas, con status):

| fixture | senales | SUSPECT | fixture | senales | SUSPECT |
|---|---|---|---|---|---|
| 33700898 | 72 | 0 | 33700903 | 33 | 1 |
| 33700899 | 72 | 1 | 33700902 | 64 | 1 |
| 33700901 | 72 | 3 | 33700905 | 40 | 1 |
| 33700896 | 55 | 4 | 33700904 | 54 | 5 |
| 33700895 | 31 | 3 | 33700900 | 45 | 2 |

- Motivo unico de todas las SUSPECT: `non-numeric history value 'vt'`
  (validacion del parser; la fila queda marcada, no se descarta ni se rellena).
- `fm_market_odds` por fixture: 195, 218, 168, 100, 108, 110, 145, 125, 118,
  128 filas (primera fila siempre anterior al kickoff).
- Navegaciones P14: 3 prioritarios x 4 = 12, resto x 4 = 28, total **40 navs**
  (presupuesto 40/corrida, 13 fixtures max a 3 navs).

## 5. Historico de equipos (colecta directa API, 0 navegaciones)

- `POST /api/fm/collect` con `locations=[home,away]`, `period=15` y 14 stats:
  goals, assists, shots, shots-on-target, corners, cards, tackles,
  fouls-committed, fouls-won, fouls-involvements, saves, offsides, penalties,
  shots-created (`FmPlayerStatMap`, slugs verificados en UI).
- 20 equipos (los 10 pares), apids extraidos del JSON-LD de cada overview:
  18710, 18588, 18718, 18645, 18706, 18708, 18570, 18634, 18778, 18777,
  18690, 18697, 18779, 18635, 18872, 18641, 18648, 18603, 18630, 18689.
- Resultado: **420 filas por equipo = 15 partidos x 2 sedes x 14 stats**.
- Incidencias resueltas (ambas verificadas luego en DB):
  - El primer lote de 14 equipos corto por timeout de cliente a las 1700 s;
    el servidor siguio hasta 13/14 y se detuvo en San Marino (8 stats).
    Re-lote de San Marino + relleno de `goals` de Islas Feroe (15 -> 30).
  - Los 6 equipos de los prioritarios solo tenian `corners` (default del
    collect); se ampliaron a los 14 stats para homogeneizar (2 lotes).

## 6. Problemas encontrados y resueltos en el camino

1. **500 en el informe**: `ArgumentException: Home team has no matches played`
   (`MatchLambdaCalculator.CalculateGoalLambdas`) porque los equipos no tenian
   filas en `fm_team_matches`. Resuelto con el collect del punto 5.
2. **500 en snapshots tras `close`+`start-visible`**:
   `ObjectDisposedException: SemaphoreSlim` en `FmNavigator.GoToAsync` (el gate
   del pipeline quedo dispuesto). Resuelto reiniciando la API (DI recrea todo).
3. **500 puntual (33700905)**: `TimeoutException 20000ms` del fetch RSC
   (redirect 307 `?_rsc`). Resuelto con reintento (200 en 72.4 s).

## 7. Entregable y verificacion

- `GET /api/fm/p14/report.md` -> `tmp/p14_uefa_2026-09-29.md` (76013 chars,
  910 lineas): titulo y alcance del 29/09, seccion `Notas de implementacion
  del P14`.
- `GET /api/fm/p14/report` (JSON): **10 fixtures**, `isPostMatch=False` en
  los 10, kickoffs correctos (16:00Z x2, 18:45Z x8).
- Volumen/umbral/confianza: 3 x `medio` -> 5% / Media; 7 x `bajo` -> 5% /
  Baja (del brief P14).
- Candidatos por fixture: 10, 10, 10, 8, 3, 7, 10, 10, 9, 7 (cap 10
  respetado); filas de equipo 25-43; filas de jugador 5-32.
- Lambdas HomeAwaySplit por fixture (ej. Espana 2.3/1.23; San Marino
  0.96/1.83; Eslovaquia 2.53/0.80).
- Regex F2 (prohibido): **0 hits** en el entregable.
- Literales `POST-PARTIDO`: 2, ambos en la nota explicativa del pie (ningun
  fixture en modo post: la deteccion automatica compara primera captura vs
  kickoff persistido y las 10 capturas son previas).
- Build `src/MatchEdge.Api`: **0 errores / 0 advertencias** (los 14 warnings
  del build sin incremental son preexistentes de SofaScore/Orchestrator).
- Tests `P13ProxyEvReportBuilderTests`: **17/17** (el overload de titulos no
  cambia el wording P13 que ellos verifican).

## 8. Cambios de codigo (logica intacta)

- `FmP14Controller.cs` (nuevo): `P14Fixtures` (10 specs) + endpoints
  `GET api/fm/p14/report[.md]`. Comentario en ingles que documenta la
  duplicacion intencional con P13 y la deuda tecnica (mover specs a config/DB
  en un ciclo futuro). No duplica ni modifica la logica Delta%/candidatos.
- `P13ProxyEvReportMarkdown.cs`: solo se parametriza el encabezado
  (titulo, alcance, cabecera de notas) via overload; `Render(report)` conserva
  el wording congelado de P13. Tablas, Delta% y candidatos sin cambios.
