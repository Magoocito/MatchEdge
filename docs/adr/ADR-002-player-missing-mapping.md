# ADR-002: Mapeo de mercados de jugadores a `fm_player_matches.stats_json`

- Estado: Aceptado — 2026-09-26
- PBI: 2.1 **Parte C (FIX MAPPER PLAYER)**
- Validación: P8-A sobre los 7 fixtures (`tmp/fm/p8_a_analyze.js`)

## Contexto / problema

La validación P8-A marcaba **147 señales de jugador** con
`player.missing_or_non_numeric` (`grand_total_flagged = 185` = 147 + 38
`team.not_reproducible`), repartidas así:

| market | campo esperado | flagged |
|---|---|---|
| tackles | `tackles` | 40 |
| foul_involvements | `foulInvolvements` / `fi` | 30 |
| fouls_drawn | `foulsD` | 26 |
| fouls_committed | `foulsC` | 22 |
| goalkeeper_saves | `saves` | 19 |
| resto (shots, sot, goals, score_assist, shots_created) | `sh`, `sot`, … | 10 |

Causa raíz: `fm_player_matches` se recolectó solo con `stat=corners`
(`teams/table?location=home|away&period=15&stat=corners` → `group=attack`),
cuyo `pivotData` **no contiene** `tackles`, `foulsC`, `foulsD`, `fi` ni `saves`
(0 presencias en las 12.192 filas `perspective='team'`). El mapper exigía
esos campos en el `stats_json` de la fila corners → 147 fallos.

Además, `FmTeamController.Collect` fijaba `group=attack`, así que ningún
`stat` de defensa/disciplina podía recolectarse aunque se pidiera.

## Decisión

1. **`FmPlayerStatMap`** (nuevo, `src/MatchEdge.Infrastructure/Services/`):
   mapa único mercado → slug FM → campo de `stats_json` → `group`.
   Los 5 slugs se validaron contra la API en la Parte B:
   `tackles`, `fouls-involvements`, `fouls-committed`, `saves`,
   `fouls-drawn`→400→**`fouls-won`** (campo `foulsD`).
2. **`group` derivado del stat** en `FmTeamController.Collect`
   (`FmPlayerStatMap.GroupForStat`: tackles/saves → `defense`,
   fouls-* → `discipline`, resto → `attack`); el endpoint además admite
   `stat` en lista separada por comas y el alias de venue del PBI
   `venue=home,away` (además de `locations=["home","away"]`).
3. **Candidatos múltiples** en `FmConfluenceReportBuilder.TryPlayerValue`:
   se prueban en orden todos los campos del mercado (p.ej.
   `foulInvolvements` y `fi`); el motivo conserva el literal
   `stat 'X' missing or non-numeric` que exige el clasificador P8-A.
4. **Dedup stat-aware de filas de equipo** (`DedupByFixtureForMarket`):
   una misma `fixture_id` puede tener varias filas (una por `stat`); se
   elige la fila cuyo `team_stats_json` aporta el stat del mercado para que
   la fila defense/discipline no oculte la de corners.
5. **`skip` acotado al fixture actual** (`BuildPlayerSeries`): solo la fila
   del fixture del reporte puede marcar el motivo. Las filas antiguas pueden
   ser de defensa/disciplina solas (sus stats de ataque nunca se recolectaron)
   y antes envenenaban el `motivo` de toda la ventana.

## Recolecto realizado

14 equipos (los de los fixtures 33441811-17) × `stats=["tackles",
"fouls-committed"]` × `locations=["home","away"]` con `period=30`
(14 `POST /api/fm/teams/{id}/collect`, 56 fetches secuenciales, 2-4 s,
0 errores). Log: `tmp/fm/p8c_backfill_log.json`; seguro de datos (raws por
si el reinicio perdiera la sesión FM): `tmp/fm/p8c_raw/` +
`tmp/fm/p8c_fetch_insurance.json`.

## Consecuencias

| métrica | antes | después |
|---|---|---|
| `player.missing_or_non_numeric` | 147 | **11** (< 20 ✔) |
| `team.not_reproducible` | 38 | 38 (intacto ✔) |
| `grand_total_flagged` | 185 | **49** |

- Los 11 restantes son reales: jugadores con `mins=0`/`rating=null` en el
  fixture actual (no jugaron) → FM devuelve `null`.
- Ventanas: 140 señales de jugador ganan puntos, 0 pierden, 0 desaparecen;
  `last5`/`last10` idénticos al baseline; solo crece `all`
  (`tmp/fm/p8_a_baseline/` vs `tmp/fm/p8_a_report_*.json`).
- Regresión cubierta por test:
  `Report_PlayerStatMissingOnlyInOlderFixture_DoesNotFlagSignal`.
- Riesgo asumido: una señal solo se marca si falta el stat en el fixture
  actual; una ventana con filas antiguas sin stats de ataque queda más
  corta sin explicación en `motivo` (documentado aquí a propósito).

## Alternativas descartadas

- Pedir los 5 stats con `position-stats` por fixture (3.061 filas cubrían
  solo ~22/48 jugadores por partido y `saves` solo al GK) → no cierra 147.
- Re-escribir las filas corners con el pivot completo → destruiría la
  evidencia de la Parte B y el índice único por `stat`.

## P9 (2026-09-26): los 3 mercados residuales

Extensión tras PR #40 (PBI 2.2 / brief P9). Sonda live de
`shots_created`, `chances_created` y `penalties` sobre 18643 y 18701
(`tmp/fm/p9_b_unmapped_probe.json`, 19 requests secuenciales 2-4 s):

- **Los 3 tienen campo real en `pivotData` del pivot de ataque**
  (`shotsCreated`, `chancesCreated`, `penalties`; 704/721 filas no nulas)
  y ya estaban en `FieldByMarket` → en los 7 fixtures hay **0**
  `player.unmapped` (`shots_created` emite 10 señales con dato;
  `chances_created`/`penalties` no tienen filas en `fm_signal`).
- **Nuevo en `SlugByMarket`**: `shots_created → shots-created`,
  `penalties → penalties` (ambos 200 en
  `position-stats` con `value` no nulo y presentes en el dropdown de 45
  opciones). `chances_created` **no** recibe slug: `chances-created` → 400
  y no existe en la UI; su dato se recolecta gratis con cualquier stat de
  ataque (`group=attack`). No se mapea a `key-passes` para no inventar una
  equivalencia (aunque `keyp == chancesCreated` en 44/44 filas de Austria).
- **Regla de evidencia de slugs**: `teams/table` no valida `stat`
  (control `zz-bogus-p9` → 200), así que solo `position-stats` (400) y el
  dropdown de la UI sirven para afirmar que un slug existe.
- Tests: `PlayerStatMap_ResidualMarkets_UseProbeBackedFieldsAndSlugs`
  (theory ×3) y `Report_ResidualPlayerMarkets_AreMappedAndUnknownStaysUnmapped`
  (además fija que un mercado sin mapeo sigue con motivo `has no mapping`).
