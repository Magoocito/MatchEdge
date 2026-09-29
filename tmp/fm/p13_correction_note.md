# Nota de corrección P11/P13 — error de metadato en la evidencia (2026-09-28)

Archivo aparte (no operativo). Resume qué se corrigió y por qué, para que quede
trazable que el problema fue un **error de metadato generado por un script y por un
ledger manual**, no del modelo (Poisson/Dixon-Coles), ni de las cuotas, ni del
pipeline de captura.

## 1. Qué estaba mal

1. **`tmp/fm/p13_evidence_build.py` no hizo ninguna llamada HTTP.** Concatenó los
   `repmd_*.md` ya guardados en `%TEMP%\opencode` y escribió como hechos dos frases
   hardcodeadas: "Snapshot ejecutado (9/9 navs)" y "Sin snapshot (presupuesto de
   navegacion agotado) ... -> HTTP 404" (+ el "Motivo: El presupuesto (9 navs) se
   agoto" repetido en las 5 secciones). No existía ningún contador de navs leyéndose
   de la API.
2. **`docs/STATE.md` llevaba un ledger manual "9/12" autocontradictorio**: la línea
   de cabecera decía "presupuesto de navegación intacto en 9/12" mientras el mismo
   bloque registraba "3 snapshots = 9 navs" en P12. Ese ledger (como "máx. 10" en
   `docs/FM_AGENT_BRIEF_P3.md` o "2/15" en `docs/HANDOFF_REVIEW.md`) no derivaba de
   la API y fue lo que detuvo la corrida de P13 en 3 de 8 fixtures.
3. **El límite real es distinto**: `FmNavigator.MaxNavigationsPerRun = 40` por
   corrida (`FmNavigator.cs:11`), contador a 0 en cada `POST /api/fm/fixtures|snapshot|run`
   (`FmSnapshotController.cs:271,304,362`); `git log -S MaxNavigationsPerRun` → 40
   desde `0a02809` (único commit que lo creó). Los 8 fixtures necesitan 24 navs:
   caben en una sola corrida.

## 2. Evidencia dura de que la API nunca rechazó nada

- 0 ocurrencias de `Navigation budget` / `429` en `tmp/fm/p12_api_out.log` y
  `tmp/fm/p13_api_out.log`; `tmp/fm/p13_api_err.log` vacío.
- 0 filas de `fm_snapshot` en estado de error (151 filas históricas, todas OK/EMPTY).
- No existe request guardado de snapshot para 33662307/08/09/14/15 en la ventana
  original (2026-09-28 02:36–02:55Z): nunca llegaron a FM.
- Los "HTTP 404" citados venían de **nuestro propio cliente**:
  `GET /api/fm/fixtures/{id}/report[.md]` → `FmSnapshotController.cs:164-165,174-175`
  ← `FmConfluenceReportLoader.cs:14` (`signals.Count == 0`), cuerpo
  `{"error":"fixture <id> has no OK/SUSPECT signals."}`.

## 3. Qué se corrigió (timestamps reales, UTC, no reconstruidos)

| Hora (UTC) | Acción |
|---|---|
| 2026-09-28 21:34 | `docs/STATE.md`: líneas 19 y 29 sin "9/12 intacto" + sección "Corrección 2026-09-28" con el texto original conservado como traza. `MaxNavigationsPerRun` **no** se tocó. |
| 2026-09-28 21:35:26 | Primer intento de los 5 snapshots → **HTTP 400** en los 5 (body mal escapado por el shell, `'u' is an invalid start of a property name`). Sin efecto: 0 navs, 0 filas. |
| 2026-09-28 21:35:53 – 21:38:17 | Reintento con body por archivo → **HTTP 200 en los 5** (`POST /api/fm/snapshot {"url":"/fixtures/<slug>"}`): 15 navs, 25 filas `fm_snapshot` (tabs overview/player-trends/team-trends = OK; `+loc=match` = EMPTY, 0 navs), señales 50/35/49/72/72, filas `fm_market_odds` 149/94/156/132/120. |
| 2026-09-28 21:38–21:41 | `tmp/p13_evidence.md` regenerado por `tmp/fm/p13_evidence_rebuild.py` (corrección de cabecera/tabla + las 5 secciones con el informe P11 real). Copia del original: `%TEMP%\opencode\p13_evidence_orig_20260928T0359Z.md`. |
| 2026-09-28 21:42 | `tmp/p13_uefa_2026-09-28.md`: tabla resumen y secciones 1/2/3/7/8 corregidas y marcadas `POST-PARTIDO - SOLO REGISTRO HISTORICO, NO ACCIONABLE`. |
| 2026-09-28 21:45 | Esta nota. |

## 4. Verificaciones posteriores

- F2 (regex de `FmDeterministicTests`): **0 hits** en `tmp/p13_evidence.md` (tras
  retirar `ClosingNote`, como exige el propio contrato) y **0 hits crudos** en
  `tmp/p13_uefa_2026-09-28.md`.
- Las 3 secciones pre-partido (33662312/33662311/33662313) de la evidencia son
  **byte-idénticas** a las del archivo original; en el P13 solo existen 3 bloques
  "Candidatos de valor" (los mismos de antes).
- Las 2 únicas apariciones restantes de "presupuesto de navegacion agotado" /
  "SIN DATA FM - snapshot no ejecutado" son **citas del texto original** dentro de
  las bullets de corrección (traza, no afirmación vigente).

## 5. Uso de los datos nuevos (5 fixtures) — restricción

Los partidos ya terminaron (kickoffs 16:00Z y 18:45Z; captura a las 21:35–21:38Z).
Esos snapshots **no** son salida accionable de este ciclo:

- Cuotas en `fm_market_odds` = cierre; las señales pueden reflejar el resultado ya
  conocido (riesgo de fuga de información para cualquier cálculo posterior).
- Uso previsto: dataset histórico en `fm_team_matches` / `fm_signal` / `fm_snapshot`
  para calibración y backtest.
- El único output pre-partido válido de este ciclo sigue siendo el de los 3
  partidos con snapshot 02:36–02:40Z.

## 6. Pendientes conocidos

- Las etiquetas `POST-PARTIDO` son **post-proceso sobre el .md**. Si se regenera
  `GET /api/fm/p13/report.md`, el builder vuelve a emitir esas 5 secciones como
  candidatos normales (no tiene noción de "capturado después del kickoff").
  Follow-up opcional: marcar en `P13ProxyEvReportBuilder` cuando
  `min(fm_snapshot.source_timestamp_utc) > kickoff` del fixture.
- `docs/STATE.md` sigue sin commitear (junto con el resto de cambios del branch).
