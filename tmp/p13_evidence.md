# P13 - Evidencia nativa 360 (8 partidos) - UEFA Nations League 2026-09-28

- Fuente: salida nativa `GET /api/fm/fixtures/{id}/report.md` (informe P11, sin Delta% ni comparacion modelo-mercado).
- Escaneo F2: como en FmDeterministicTests, antes de escanear se elimina la nota de cierre mandatoria (FmConfluenceReportMarkdown.ClosingNote); sin ella el archivo no contiene terminos del regex F2.
- Snapshot pre-partido (9 navs, 3 fixtures): 33662312, 33662311, 33662313 (3 tabs por fixture: overview, player-trends, team-trends), capturado 2026-09-28T02:36:17Z-02:40:24Z. Unico output pre-partido valido de este ciclo.
- Snapshot post-partido (15 navs, 5 fixtures): 33662307, 33662308, 33662309, 33662314, 33662315, capturado 2026-09-28T21:35:53Z-21:38:17Z (25 filas fm_snapshot, tabs OK). REGISTRO HISTORICO POST-PARTIDO - NO ACCIONABLE (partido ya jugado; cuotas de cierre, senales con resultado conocido).
- Correccion 2026-09-28 (metadato, no modelo): la linea original "Sin snapshot (presupuesto de navegacion agotado): ... -> HTTP 404" estaba hardcodeada en tmp/fm/p13_evidence_build.py (script que no hizo ninguna llamada HTTP); el ledger "9/12" de docs/STATE.md era manual y autocontradictorio. Texto corregido: No intentado - pausa del operador por lectura de un ledger manual desactualizado (docs/STATE.md); limite real de la API: 40 navs/corrida, 24 necesarias para los 8 fixtures. Corregido el 2026-09-28.
- Estado de los 5 fixtures en tabla y secciones: No intentado - pausa del operador por lectura de un ledger manual desactualizado (docs/STATE.md); limite real de la API: 40 navs/corrida, 24 necesarias para los 8 fixtures. Corregido el 2026-09-28.
- Historial de equipo colectado aparte (0 navs): 16 equipos, 32 fetches de equipo + 121 fetches de posiciones, 0 errores, 480 filas nuevas en fm_team_matches (period=15, home+away, stat=corners).
- Generado: 2026-09-28 03:59 UTC (script tmp/fm/p13_evidence_build.py, sin llamadas HTTP) · Actualizado: 2026-09-28T21:41Z (5 secciones post-partido reales, snapshots 21:35:53Z-21:38:17Z)

| fixtureId | Partido | Kickoff (UTC) | Estado | Senales |
|---|---|---|---|---|
| 33662312 | Belgium vs France | 2026-09-28T18:45:00.000Z | REPORT OK | 72 |
| 33662311 | Türkiye vs Italy | 2026-09-28T18:45:00.000Z | REPORT OK | 72 |
| 33662313 | Sweden vs Poland | 2026-09-28T18:45:00.000Z | REPORT OK | 72 |
| 33662307 | Georgia vs Ukraine | 2026-09-28T16:00:00.000Z | REGISTRO HISTORICO POST-PARTIDO (captura 21:35:56Z-21:36:24Z) | 50 |
| 33662308 | Latvia vs Cyprus | 2026-09-28T16:00:00.000Z | REGISTRO HISTORICO POST-PARTIDO (captura 21:36:28Z-21:36:55Z) | 35 |
| 33662309 | Armenia vs Montenegro | 2026-09-28T16:00:00.000Z | REGISTRO HISTORICO POST-PARTIDO (captura 21:36:57Z-21:37:26Z) | 49 |
| 33662314 | Romania vs Bosnia and Herzegovina | 2026-09-28T18:45:00.000Z | REGISTRO HISTORICO POST-PARTIDO (captura 21:37:28Z-21:37:52Z) | 72 |
| 33662315 | Northern Ireland vs Hungary | 2026-09-28T18:45:00.000Z | REGISTRO HISTORICO POST-PARTIDO (captura 21:37:54Z-21:38:17Z) | 72 |

---

## Belgium vs France (33662312) - kickoff 2026-09-28T18:45:00.000Z

# Belgium vs France
- Competición: UEFA Nations League
- Kickoff (UTC): desconocido
- Flags de calidad: leakage=false; suspect=true
- sort_criteria: sample_size_desc

## Mercados (orden: mayor muestra disponible, no fuerza de señal)

### 1. away_cards @ 2.5 — France (away)
- basis: team_own_stats
- FM reportado: hits 8/10; mejor ventana: 10 (8/10)
- Ventanas propias:
-   last5: n=5, hits=3, rate=0.600, mean=1.6, median=2, min=0, max=3
-   last10: n=10, hits=8, rate=0.800, mean=0.9, median=0, min=0, max=3
-   all: n=30, hits=23, rate=0.767, mean=1.6333, median=1, min=0, max=7
- Split home/away:
-   home: n=15, hits=14, rate=0.933, mean=0.9333, median=1
-   away: n=15, hits=9, rate=0.600, mean=2.3333, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=1.6, median=1, min=0, max=5
    last10: n=10, hits=8, rate=0.800, mean=1.8, median=2, min=0, max=5
    all: n=30, hits=22, rate=0.733, mean=1.5667, median=1, min=0, max=5
  Split home/away:
    home: n=15, hits=10, rate=0.667, mean=1.6667, median=2
    away: n=15, hits=12, rate=0.800, mean=1.4667, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 2. away_corners @ 2.5 — France (away)
- basis: team_own_stats
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=6, median=5, min=3, max=12
-   last10: n=10, hits=10, rate=1.000, mean=5.9, median=5, min=3, max=12
-   all: n=30, hits=28, rate=0.933, mean=6.3, median=6, min=2, max=16
- Split home/away:
-   home: n=15, hits=14, rate=0.933, mean=6.5333, median=6
-   away: n=15, hits=14, rate=0.933, mean=6.0667, median=6
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=5.2, median=5, min=1, max=8
    last10: n=10, hits=7, rate=0.700, mean=5.2, median=4, min=1, max=14
    all: n=30, hits=27, rate=0.900, mean=7.5, median=7, min=1, max=15
  Split home/away:
    home: n=15, hits=14, rate=0.933, mean=8.4, median=7
    away: n=15, hits=13, rate=0.867, mean=6.6, median=6
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 3. away_corners @ 3.5 — France (away)
- basis: team_own_stats
- FM reportado: hits 8/10; mejor ventana: 10 (8/10)
- Ventanas propias:
-   last5: n=5, hits=3, rate=0.600, mean=6, median=5, min=3, max=12
-   last10: n=10, hits=8, rate=0.800, mean=5.9, median=5, min=3, max=12
-   all: n=30, hits=24, rate=0.800, mean=6.3, median=6, min=2, max=16
- Split home/away:
-   home: n=15, hits=12, rate=0.800, mean=6.5333, median=6
-   away: n=15, hits=12, rate=0.800, mean=6.0667, median=6
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=5.2, median=5, min=1, max=8
    last10: n=10, hits=7, rate=0.700, mean=5.2, median=4, min=1, max=14
    all: n=30, hits=27, rate=0.900, mean=7.5, median=7, min=1, max=15
  Split home/away:
    home: n=15, hits=14, rate=0.933, mean=8.4, median=7
    away: n=15, hits=13, rate=0.867, mean=6.6, median=6
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 4. away_corners @ 7.5 — France (away)
- basis: team_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=6, median=5, min=3, max=12
-   last10: n=10, hits=8, rate=0.800, mean=5.9, median=5, min=3, max=12
-   all: n=30, hits=20, rate=0.667, mean=6.3, median=6, min=2, max=16
- Split home/away:
-   home: n=15, hits=10, rate=0.667, mean=6.5333, median=6
-   away: n=15, hits=10, rate=0.667, mean=6.0667, median=6
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=5.2, median=5, min=1, max=8
    last10: n=10, hits=7, rate=0.700, mean=5.2, median=4, min=1, max=14
    all: n=30, hits=17, rate=0.567, mean=7.5, median=7, min=1, max=15
  Split home/away:
    home: n=15, hits=8, rate=0.533, mean=8.4, median=7
    away: n=15, hits=9, rate=0.600, mean=6.6, median=6
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 5. away_goals @ 0.5 — France (away)
- basis: team_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=1.6, median=1, min=0, max=4
-   last10: n=10, hits=9, rate=0.900, mean=2.4, median=3, min=0, max=4
-   all: n=30, hits=27, rate=0.900, mean=2.2333, median=2, min=0, max=4
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=2.2, median=2
-   away: n=15, hits=14, rate=0.933, mean=2.2667, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=3, median=3, min=1, max=5
    last10: n=10, hits=9, rate=0.900, mean=2.4, median=2, min=0, max=5
    all: n=30, hits=24, rate=0.800, mean=2.4333, median=2, min=0, max=7
  Split home/away:
    home: n=15, hits=11, rate=0.733, mean=2.5333, median=3
    away: n=15, hits=13, rate=0.867, mean=2.3333, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 6. away_goals @ 1.5 — Belgium (home)
- basis: team_own_stats
- FM reportado: hits 8/9; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=1, rate=0.200, mean=3, median=3, min=1, max=5
-   last10: n=10, hits=4, rate=0.400, mean=2.4, median=2, min=0, max=5
-   all: n=30, hits=13, rate=0.433, mean=2.4333, median=2, min=0, max=7
- Split home/away:
-   home: n=15, hits=6, rate=0.400, mean=2.5333, median=3
-   away: n=15, hits=7, rate=0.467, mean=2.3333, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=1.6, median=1, min=0, max=4
    last10: n=10, hits=3, rate=0.300, mean=2.4, median=3, min=0, max=4
    all: n=30, hits=8, rate=0.267, mean=2.2333, median=2, min=0, max=4
  Split home/away:
    home: n=15, hits=4, rate=0.267, mean=2.2, median=2
    away: n=15, hits=4, rate=0.267, mean=2.2667, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 7. away_goals @ 2.5 — Belgium (home)
- basis: team_own_stats
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
-   last5: n=5, hits=2, rate=0.400, mean=3, median=3, min=1, max=5
-   last10: n=10, hits=6, rate=0.600, mean=2.4, median=2, min=0, max=5
-   all: n=30, hits=17, rate=0.567, mean=2.4333, median=2, min=0, max=7
- Split home/away:
-   home: n=15, hits=7, rate=0.467, mean=2.5333, median=3
-   away: n=15, hits=10, rate=0.667, mean=2.3333, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=1.6, median=1, min=0, max=4
    last10: n=10, hits=4, rate=0.400, mean=2.4, median=3, min=0, max=4
    all: n=30, hits=17, rate=0.567, mean=2.2333, median=2, min=0, max=4
  Split home/away:
    home: n=15, hits=8, rate=0.533, mean=2.2, median=2
    away: n=15, hits=9, rate=0.600, mean=2.2667, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 8. away_shots_on_target @ 3.5 — France (away)
- basis: team_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=5.8, median=5, min=3, max=9
-   last10: n=10, hits=9, rate=0.900, mean=6.8, median=6.5, min=3, max=12
-   all: n=30, hits=26, rate=0.867, mean=6.5333, median=6, min=3, max=12
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=7.4, median=8
-   away: n=15, hits=13, rate=0.867, mean=5.6667, median=5
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=6.4, median=7, min=2, max=10
    last10: n=10, hits=7, rate=0.700, mean=6, median=6, min=2, max=12
    all: n=30, hits=26, rate=0.867, mean=6.5667, median=6, min=2, max=15
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=7.4, median=7
    away: n=15, hits=13, rate=0.867, mean=5.7333, median=4
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 9. away_shots_on_target @ 4.5 — France (away)
- basis: team_own_stats
- FM reportado: hits 8/10; mejor ventana: 10 (8/10)
- Ventanas propias:
-   last5: n=5, hits=3, rate=0.600, mean=5.8, median=5, min=3, max=9
-   last10: n=10, hits=8, rate=0.800, mean=6.8, median=6.5, min=3, max=12
-   all: n=30, hits=22, rate=0.733, mean=6.5333, median=6, min=3, max=12
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=7.4, median=8
-   away: n=15, hits=9, rate=0.600, mean=5.6667, median=5
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=6.4, median=7, min=2, max=10
    last10: n=10, hits=6, rate=0.600, mean=6, median=6, min=2, max=12
    all: n=30, hits=19, rate=0.633, mean=6.5667, median=6, min=2, max=15
  Split home/away:
    home: n=15, hits=12, rate=0.800, mean=7.4, median=7
    away: n=15, hits=7, rate=0.467, mean=5.7333, median=4
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 10. home_cards @ 2.5 — Belgium (home)
- basis: team_own_stats
- FM reportado: hits 7/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=1.6, median=1, min=0, max=5
-   last10: n=10, hits=8, rate=0.800, mean=1.8, median=2, min=0, max=5
-   all: n=30, hits=22, rate=0.733, mean=1.5667, median=1, min=0, max=5
- Split home/away:
-   home: n=15, hits=10, rate=0.667, mean=1.6667, median=2
-   away: n=15, hits=12, rate=0.800, mean=1.4667, median=1
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=1.6, median=2, min=0, max=3
    last10: n=10, hits=8, rate=0.800, mean=0.9, median=0, min=0, max=3
    all: n=30, hits=23, rate=0.767, mean=1.6333, median=1, min=0, max=7
  Split home/away:
    home: n=15, hits=14, rate=0.933, mean=0.9333, median=1
    away: n=15, hits=9, rate=0.600, mean=2.3333, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 11. home_goals @ 0.5 — Belgium (home)
- basis: team_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=3, median=3, min=1, max=5
-   last10: n=10, hits=9, rate=0.900, mean=2.4, median=2, min=0, max=5
-   all: n=30, hits=24, rate=0.800, mean=2.4333, median=2, min=0, max=7
- Split home/away:
-   home: n=15, hits=11, rate=0.733, mean=2.5333, median=3
-   away: n=15, hits=13, rate=0.867, mean=2.3333, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=1.6, median=1, min=0, max=4
    last10: n=10, hits=9, rate=0.900, mean=2.4, median=3, min=0, max=4
    all: n=30, hits=27, rate=0.900, mean=2.2333, median=2, min=0, max=4
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=2.2, median=2
    away: n=15, hits=14, rate=0.933, mean=2.2667, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 12. home_goals @ 1.5 — France (away)
- basis: team_own_stats
- FM reportado: hits 8/10; mejor ventana: 10 (8/10)
- Ventanas propias:
-   last5: n=5, hits=3, rate=0.600, mean=1.6, median=1, min=0, max=4
-   last10: n=10, hits=3, rate=0.300, mean=2.4, median=3, min=0, max=4
-   all: n=30, hits=8, rate=0.267, mean=2.2333, median=2, min=0, max=4
- Split home/away:
-   home: n=15, hits=4, rate=0.267, mean=2.2, median=2
-   away: n=15, hits=4, rate=0.267, mean=2.2667, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=1, rate=0.200, mean=3, median=3, min=1, max=5
    last10: n=10, hits=4, rate=0.400, mean=2.4, median=2, min=0, max=5
    all: n=30, hits=13, rate=0.433, mean=2.4333, median=2, min=0, max=7
  Split home/away:
    home: n=15, hits=6, rate=0.400, mean=2.5333, median=3
    away: n=15, hits=7, rate=0.467, mean=2.3333, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 13. home_goals @ 2.5 — France (away)
- basis: team_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=1.6, median=1, min=0, max=4
-   last10: n=10, hits=4, rate=0.400, mean=2.4, median=3, min=0, max=4
-   all: n=30, hits=17, rate=0.567, mean=2.2333, median=2, min=0, max=4
- Split home/away:
-   home: n=15, hits=8, rate=0.533, mean=2.2, median=2
-   away: n=15, hits=9, rate=0.600, mean=2.2667, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=2, rate=0.400, mean=3, median=3, min=1, max=5
    last10: n=10, hits=6, rate=0.600, mean=2.4, median=2, min=0, max=5
    all: n=30, hits=17, rate=0.567, mean=2.4333, median=2, min=0, max=7
  Split home/away:
    home: n=15, hits=7, rate=0.467, mean=2.5333, median=3
    away: n=15, hits=10, rate=0.667, mean=2.3333, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 14. home_shots @ 10.5 — Belgium (home)
- basis: team_own_stats
- FM reportado: hits 6/7; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=18.6, median=19, min=5, max=35
-   last10: n=10, hits=7, rate=0.700, mean=17.1, median=17, min=5, max=35
-   all: n=30, hits=25, rate=0.833, mean=18.3, median=19, min=5, max=35
- Split home/away:
-   home: n=15, hits=15, rate=1.000, mean=21.5333, median=20
-   away: n=15, hits=10, rate=0.667, mean=15.0667, median=15
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=15.6, median=15, min=10, max=22
    last10: n=10, hits=9, rate=0.900, mean=17.8, median=18.5, min=10, max=27
    all: n=30, hits=26, rate=0.867, mean=17.8, median=16.5, min=6, max=33
  Split home/away:
    home: n=15, hits=14, rate=0.933, mean=21, median=22
    away: n=15, hits=12, rate=0.800, mean=14.6, median=15
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 15. home_shots @ 11.5 — Belgium (home)
- basis: team_own_stats
- FM reportado: hits 6/7; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=18.6, median=19, min=5, max=35
-   last10: n=10, hits=7, rate=0.700, mean=17.1, median=17, min=5, max=35
-   all: n=30, hits=24, rate=0.800, mean=18.3, median=19, min=5, max=35
- Split home/away:
-   home: n=15, hits=14, rate=0.933, mean=21.5333, median=20
-   away: n=15, hits=10, rate=0.667, mean=15.0667, median=15
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=15.6, median=15, min=10, max=22
    last10: n=10, hits=8, rate=0.800, mean=17.8, median=18.5, min=10, max=27
    all: n=30, hits=25, rate=0.833, mean=17.8, median=16.5, min=6, max=33
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=21, median=22
    away: n=15, hits=12, rate=0.800, mean=14.6, median=15
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 16. home_shots @ 12.5 — Belgium (home)
- basis: team_own_stats
- FM reportado: hits 6/7; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=18.6, median=19, min=5, max=35
-   last10: n=10, hits=7, rate=0.700, mean=17.1, median=17, min=5, max=35
-   all: n=30, hits=22, rate=0.733, mean=18.3, median=19, min=5, max=35
- Split home/away:
-   home: n=15, hits=14, rate=0.933, mean=21.5333, median=20
-   away: n=15, hits=8, rate=0.533, mean=15.0667, median=15
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=15.6, median=15, min=10, max=22
    last10: n=10, hits=7, rate=0.700, mean=17.8, median=18.5, min=10, max=27
    all: n=30, hits=23, rate=0.767, mean=17.8, median=16.5, min=6, max=33
  Split home/away:
    home: n=15, hits=12, rate=0.800, mean=21, median=22
    away: n=15, hits=11, rate=0.733, mean=14.6, median=15
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 17. home_shots_on_target @ 2.5 — Belgium (home)
- basis: team_own_stats
- FM reportado: hits 7/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=6.4, median=7, min=2, max=10
-   last10: n=10, hits=8, rate=0.800, mean=6, median=6, min=2, max=12
-   all: n=30, hits=28, rate=0.933, mean=6.5667, median=6, min=2, max=15
- Split home/away:
-   home: n=15, hits=15, rate=1.000, mean=7.4, median=7
-   away: n=15, hits=13, rate=0.867, mean=5.7333, median=4
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=5.8, median=5, min=3, max=9
    last10: n=10, hits=10, rate=1.000, mean=6.8, median=6.5, min=3, max=12
    all: n=30, hits=30, rate=1.000, mean=6.5333, median=6, min=3, max=12
  Split home/away:
    home: n=15, hits=15, rate=1.000, mean=7.4, median=8
    away: n=15, hits=15, rate=1.000, mean=5.6667, median=5
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 18. total_goals @ 1.5 — Belgium vs France (sin rol)
- basis: match_total_goals
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=4.2, median=5, min=2, max=6
-   last10: n=10, hits=9, rate=0.900, mean=3.2, median=2.5, min=0, max=6
-   all: n=30, hits=25, rate=0.833, mean=3.4333, median=3, min=0, max=7
- Split home/away:
-   home: n=15, hits=11, rate=0.733, mean=3.2667, median=3
-   away: n=15, hits=14, rate=0.933, mean=3.6, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=3.2, median=2, min=1, max=10
    last10: n=10, hits=8, rate=0.800, mean=3.5, median=3, min=1, max=10
    all: n=30, hits=27, rate=0.900, mean=3.3667, median=3, min=0, max=10
  Split home/away:
    home: n=15, hits=14, rate=0.933, mean=3.2667, median=3
    away: n=15, hits=13, rate=0.867, mean=3.4667, median=3
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 19. total_goals @ 3.5 — Belgium vs France (sin rol)
- basis: match_total_goals
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=3.2, median=2, min=1, max=10
-   last10: n=10, hits=6, rate=0.600, mean=3.5, median=3, min=1, max=10
-   all: n=30, hits=18, rate=0.600, mean=3.3667, median=3, min=0, max=10
- Split home/away:
-   home: n=15, hits=10, rate=0.667, mean=3.2667, median=3
-   away: n=15, hits=8, rate=0.533, mean=3.4667, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=2, rate=0.400, mean=4.2, median=5, min=2, max=6
    last10: n=10, hits=6, rate=0.600, mean=3.2, median=2.5, min=0, max=6
    all: n=30, hits=17, rate=0.567, mean=3.4333, median=3, min=0, max=7
  Split home/away:
    home: n=15, hits=9, rate=0.600, mean=3.2667, median=3
    away: n=15, hits=8, rate=0.533, mean=3.6, median=3
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 20. 1x2_shots_on_target — France (away)
- basis: unmapped
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=true, source_conflict=false, motivo="non-numeric history value 'vt'; market '1x2_shots_on_target' has no mapping to fm_team_matches.team_stats_json"

### 21. btts — France (away)
- basis: unmapped
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=true, source_conflict=false, motivo="non-numeric history value 'vt'; market 'btts' has no mapping to fm_team_matches.team_stats_json"

### 22. total_cards @ 3.5 — Belgium vs France (sin rol)
- basis: not_reproducible
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 23. total_cards @ 4.5 — Belgium vs France (sin rol)
- basis: not_reproducible
- FM reportado: hits 7/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 24. total_cards @ 5.5 — Belgium vs France (sin rol)
- basis: not_reproducible
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 25. total_corners @ 6.5 — Belgium vs France (sin rol)
- basis: not_reproducible
- FM reportado: hits 7/7; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 26. total_corners @ 7.5 — Belgium vs France (sin rol)
- basis: not_reproducible
- FM reportado: hits 6/7; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 27. total_corners @ 10.5 — Belgium vs France (sin rol)
- basis: not_reproducible
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 28. total_corners @ 11.5 — Belgium vs France (sin rol)
- basis: not_reproducible
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 29. total_corners @ 12.5 — Belgium vs France (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 30. total_shots_on_target @ 6.5 — Belgium vs France (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 31. total_shots_on_target @ 7.5 — Belgium vs France (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 32. total_shots_on_target @ 8.5 — Belgium vs France (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

## Señales de jugadores (orden: mayor muestra disponible, no fuerza de señal)

### 1. shots_created @ 1.5 — M. Olise
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=2.8, median=2, min=1, max=6
  last10: n=10, hits=6, rate=0.600, mean=3.1, median=3, min=0, max=6
  all: n=29, hits=17, rate=0.586, mean=2.6897, median=3, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 2. shots @ 0.5 — D. Lukébakio
- basis: player_own_stats
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1.2, median=1, min=0, max=3
  last10: n=10, hits=6, rate=0.600, mean=1.2, median=1, min=0, max=3
  all: n=28, hits=13, rate=0.464, mean=0.9286, median=0, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 3. score_assist @ 0.5 — C. De Ketelaere
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=2, rate=0.400, mean=0.4, median=0, min=0, max=1
  last10: n=10, hits=2, rate=0.200, mean=0.2, median=0, min=0, max=1
  all: n=27, hits=4, rate=0.148, mean=0.1852, median=0, min=0, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 4. shots @ 0.5 — C. De Ketelaere
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.6, median=2, min=0, max=3
  last10: n=10, hits=6, rate=0.600, mean=1.1, median=1, min=0, max=3
  all: n=27, hits=13, rate=0.482, mean=0.8519, median=0, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 5. shots @ 1.5 — C. De Ketelaere
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1.6, median=2, min=0, max=3
  last10: n=10, hits=4, rate=0.400, mean=1.1, median=1, min=0, max=3
  all: n=27, hits=8, rate=0.296, mean=0.8519, median=0, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 6. shots_on_target @ 0.5 — C. De Ketelaere
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.4, median=1, min=0, max=3
  last10: n=10, hits=5, rate=0.500, mean=0.8, median=0.5, min=0, max=3
  all: n=27, hits=11, rate=0.407, mean=0.5556, median=0, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 7. shots @ 0.5 — B. Barcola
- basis: player_own_stats
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=1.2, median=1, min=1, max=2
  last10: n=10, hits=9, rate=0.900, mean=1.2, median=1, min=0, max=3
  all: n=25, hits=20, rate=0.800, mean=1.28, median=1, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 8. shots @ 0.5 — A. Rabiot
- basis: player_own_stats
- FM reportado: hits 7/7; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=1.6, median=1, min=1, max=3
  last10: n=10, hits=8, rate=0.800, mean=1.2, median=1, min=0, max=3
  all: n=24, hits=15, rate=0.625, mean=0.9583, median=1, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 9. shots @ 0.5 — K. De Bruyne
- basis: player_own_stats
- FM reportado: hits 7/7; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.8, median=2, min=0, max=7
  last10: n=10, hits=8, rate=0.800, mean=2.8, median=2.5, min=0, max=7
  all: n=24, hits=22, rate=0.917, mean=3.375, median=3, min=0, max=7
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 10. shots @ 0.5 — Y. Tielemans
- basis: player_own_stats
- FM reportado: hits 7/7; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=2.2, median=2, min=1, max=4
  last10: n=10, hits=8, rate=0.800, mean=1.7, median=2, min=0, max=4
  all: n=24, hits=18, rate=0.750, mean=1.7083, median=2, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 11. shots @ 1.5 — K. De Bruyne
- basis: player_own_stats
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.8, median=2, min=0, max=7
  last10: n=10, hits=7, rate=0.700, mean=2.8, median=2.5, min=0, max=7
  all: n=24, hits=21, rate=0.875, mean=3.375, median=3, min=0, max=7
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 12. shots_on_target @ 0.5 — K. De Bruyne
- basis: player_own_stats
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1, median=1, min=0, max=2
  last10: n=10, hits=6, rate=0.600, mean=1, median=1, min=0, max=3
  all: n=24, hits=19, rate=0.792, mean=1.3333, median=1, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 13. shots_on_target @ 0.5 — O. Dembélé
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.2, median=1, min=0, max=2
  last10: n=10, hits=6, rate=0.600, mean=1.1, median=1, min=0, max=3
  all: n=22, hits=12, rate=0.546, mean=0.9091, median=1, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 14. tackles @ 0.5 — M. Olise
- basis: player_own_stats
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=2.8, median=3, min=1, max=5
  last10: n=10, hits=9, rate=0.900, mean=2.1, median=2, min=0, max=5
  all: n=13, hits=10, rate=0.769, mean=1.6923, median=1, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 15. tackles @ 1.5 — M. Olise
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.8, median=3, min=1, max=5
  last10: n=10, hits=6, rate=0.600, mean=2.1, median=2, min=0, max=5
  all: n=13, hits=6, rate=0.462, mean=1.6923, median=1, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 16. tackles @ 2.5 — M. Olise
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=2.8, median=3, min=1, max=5
  last10: n=10, hits=4, rate=0.400, mean=2.1, median=2, min=0, max=5
  all: n=13, hits=4, rate=0.308, mean=1.6923, median=1, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 17. tackles @ 0.5 — K. De Bruyne
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=0.8, median=1, min=0, max=1
  last10: n=10, hits=6, rate=0.600, mean=0.6, median=1, min=0, max=1
  all: n=12, hits=7, rate=0.583, mean=0.6667, median=1, min=0, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 18. tackles @ 0.5 — A. Rabiot
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.8, median=2, min=0, max=3
  last10: n=10, hits=8, rate=0.800, mean=1.5, median=1.5, min=0, max=3
  all: n=11, hits=9, rate=0.818, mean=1.6364, median=2, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 19. tackles @ 1.5 — A. Rabiot
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1.8, median=2, min=0, max=3
  last10: n=10, hits=5, rate=0.500, mean=1.5, median=1.5, min=0, max=3
  all: n=11, hits=6, rate=0.546, mean=1.6364, median=2, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 20. foul_involvements @ 1.5 — Y. Tielemans
- basis: player_own_stats
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=3.2, median=2, min=2, max=6
  last10: n=10, hits=10, rate=1.000, mean=3.2, median=3, min=2, max=6
  all: n=10, hits=10, rate=1.000, mean=3.2, median=3, min=2, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 21. fouls_committed @ 0.5 — B. Mechele
- basis: player_own_stats
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=1.4, median=1, min=1, max=2
  last10: n=10, hits=7, rate=0.700, mean=1, median=1, min=0, max=2
  all: n=10, hits=7, rate=0.700, mean=1, median=1, min=0, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 22. fouls_committed @ 0.5 — N. Raskin
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=2.4, median=2, min=1, max=5
  last10: n=10, hits=9, rate=0.900, mean=2, median=1, min=0, max=5
  all: n=10, hits=9, rate=0.900, mean=2, median=1, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 23. fouls_committed @ 0.5 — Y. Tielemans
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.4, median=1, min=0, max=4
  last10: n=10, hits=8, rate=0.800, mean=1.2, median=1, min=0, max=4
  all: n=10, hits=8, rate=0.800, mean=1.2, median=1, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 24. fouls_drawn @ 0.5 — Y. Tielemans
- basis: player_own_stats
- FM reportado: hits 8/8; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=1.8, median=2, min=1, max=3
  last10: n=10, hits=10, rate=1.000, mean=2, median=2, min=1, max=3
  all: n=10, hits=10, rate=1.000, mean=2, median=2, min=1, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 25. tackles @ 0.5 — B. Mechele
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.2, median=1, min=0, max=2
  last10: n=10, hits=6, rate=0.600, mean=1.4, median=1, min=0, max=6
  all: n=10, hits=6, rate=0.600, mean=1.4, median=1, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 26. fouls_committed @ 0.5 — N. Ngoy
- basis: player_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=1.4, median=1, min=1, max=2
  last10: n=8, hits=6, rate=0.750, mean=1, median=1, min=0, max=2
  all: n=8, hits=6, rate=0.750, mean=1, median=1, min=0, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 27. tackles @ 0.5 — N. Ngoy
- basis: player_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=1.6, median=2, min=1, max=2
  last10: n=8, hits=6, rate=0.750, mean=1.125, median=1, min=0, max=2
  all: n=8, hits=6, rate=0.750, mean=1.125, median=1, min=0, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 28. shots @ 0.5 — M. Fofana
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.4, median=1, min=0, max=3
  last10: n=7, hits=4, rate=0.571, mean=1, median=1, min=0, max=3
  all: n=7, hits=4, rate=0.571, mean=1, median=1, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 29. foul_involvements @ 1.5 — M. Koné
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=3.8, median=3, min=1, max=9
  last10: n=6, hits=5, rate=0.833, mean=3.5, median=2.5, min=1, max=9
  all: n=6, hits=5, rate=0.833, mean=3.5, median=2.5, min=1, max=9
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 30. fouls_drawn @ 0.5 — M. Koné
- basis: player_own_stats
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=2, median=2, min=1, max=4
  last10: n=6, hits=6, rate=1.000, mean=1.8333, median=1.5, min=1, max=4
  all: n=6, hits=6, rate=1.000, mean=1.8333, median=1.5, min=1, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 31. fouls_committed @ 0.5 — M. Lacroix
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=4, hits=4, rate=1.000, mean=1.5, median=1.5, min=1, max=2
  last10: n=4, hits=4, rate=1.000, mean=1.5, median=1.5, min=1, max=2
  all: n=4, hits=4, rate=1.000, mean=1.5, median=1.5, min=1, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 32. goalkeeper_saves @ 1.5 — S. Lammens
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=3, hits=2, rate=0.667, mean=3.3333, median=3, min=1, max=6
  last10: n=3, hits=2, rate=0.667, mean=3.3333, median=3, min=1, max=6
  all: n=3, hits=2, rate=0.667, mean=3.3333, median=3, min=1, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 33. foul_involvements @ 1.5 — K. De Winter
- basis: player_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
  last5: INSUFFICIENT_SAMPLE
  last10: INSUFFICIENT_SAMPLE
  all: INSUFFICIENT_SAMPLE
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 34. fouls_committed @ 0.5 — K. De Winter
- basis: player_own_stats
- FM reportado: hits 7/7; mejor ventana: 10 (7/7)
- Ventanas propias:
  last5: INSUFFICIENT_SAMPLE
  last10: INSUFFICIENT_SAMPLE
  all: INSUFFICIENT_SAMPLE
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 35. fouls_drawn @ 0.5 — M. Godts
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: 5 (3/3)
- Ventanas propias:
  last5: INSUFFICIENT_SAMPLE
  last10: INSUFFICIENT_SAMPLE
  all: INSUFFICIENT_SAMPLE
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 36. tackles @ 0.5 — D. Lukébakio
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: INSUFFICIENT_SAMPLE
  last10: INSUFFICIENT_SAMPLE
  all: INSUFFICIENT_SAMPLE
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

## Evidencia histórica (ventanas fijas)
- observaciones descriptivas: secuencia cruda (más antiguo -> más reciente) y hits de las ventanas fijas (last5/last10/all)

### Jugadores — evidencia con ventanas fijas
- M. Olise — shots_created @ 1.5: [1,6,5,0,5,1,1,2,6,4] | last5 3/5 | last10 6/10 | all 17/29
- D. Lukébakio — shots @ 0.5: [1,0,3,0,2,0,3,2,0,1] | last5 3/5 | last10 6/10 | all 13/28
- C. De Ketelaere — score_assist @ 0.5: [0,0,0,0,0,0,0,1,0,1] | last5 2/5 | last10 2/10 | all 4/27
- C. De Ketelaere — shots @ 0.5: [1,0,2,0,0,1,0,3,2,2] | last5 4/5 | last10 6/10 | all 13/27
- C. De Ketelaere — shots @ 1.5: [1,0,2,0,0,1,0,3,2,2] | last5 3/5 | last10 4/10 | all 8/27
- C. De Ketelaere — shots_on_target @ 0.5: [0,0,1,0,0,1,0,3,1,2] | last5 4/5 | last10 5/10 | all 11/27
- B. Barcola — shots @ 0.5: [1,1,1,0,3,1,1,1,2,1] | last5 5/5 | last10 9/10 | all 20/25
- A. Rabiot — shots @ 0.5: [1,0,1,0,2,3,1,1,1,2] | last5 5/5 | last10 8/10 | all 15/24
- K. De Bruyne — shots @ 0.5: [0,1,4,4,5,7,2,0,2,3] | last5 4/5 | last10 8/10 | all 22/24
- Y. Tielemans — shots @ 0.5: [0,0,2,2,2,2,1,4,2,2] | last5 5/5 | last10 8/10 | all 18/24
- K. De Bruyne — shots @ 1.5: [0,1,4,4,5,7,2,0,2,3] | last5 4/5 | last10 7/10 | all 21/24
- K. De Bruyne — shots_on_target @ 0.5: [0,0,3,1,1,2,0,0,1,2] | last5 3/5 | last10 6/10 | all 19/24
- O. Dembélé — shots_on_target @ 0.5: [0,0,2,3,0,0,1,2,2,1] | last5 4/5 | last10 6/10 | all 12/22
- M. Olise — tackles @ 0.5: [3,2,1,0,1,1,2,3,5,3] | last5 5/5 | last10 9/10 | all 10/13
- M. Olise — tackles @ 1.5: [3,2,1,0,1,1,2,3,5,3] | last5 4/5 | last10 6/10 | all 6/13
- M. Olise — tackles @ 2.5: [3,2,1,0,1,1,2,3,5,3] | last5 3/5 | last10 4/10 | all 4/13
- K. De Bruyne — tackles @ 0.5: [1,1,0,0,0,1,1,0,1,1] | last5 4/5 | last10 6/10 | all 7/12
- A. Rabiot — tackles @ 0.5: [2,2,0,1,1,0,1,3,2,3] | last5 4/5 | last10 8/10 | all 9/11
- A. Rabiot — tackles @ 1.5: [2,2,0,1,1,0,1,3,2,3] | last5 3/5 | last10 5/10 | all 6/11
- Y. Tielemans — foul_involvements @ 1.5: [2,5,3,3,3,2,4,6,2,2] | last5 5/5 | last10 10/10 | all 10/10
- B. Mechele — fouls_committed @ 0.5: [0,0,0,2,1,2,2,1,1,1] | last5 5/5 | last10 7/10 | all 7/10
- N. Raskin — fouls_committed @ 0.5: [1,1,0,5,1,2,1,3,5,1] | last5 5/5 | last10 9/10 | all 9/10
- Y. Tielemans — fouls_committed @ 0.5: [0,2,1,1,1,0,1,4,1,1] | last5 4/5 | last10 8/10 | all 8/10
- Y. Tielemans — fouls_drawn @ 0.5: [2,3,2,2,2,2,3,2,1,1] | last5 5/5 | last10 10/10 | all 10/10
- B. Mechele — tackles @ 0.5: [6,0,0,2,0,1,0,2,2,1] | last5 4/5 | last10 6/10 | all 6/10
- N. Ngoy — fouls_committed @ 0.5: [0,1,0,2,2,1,1,1] | last5 5/5 | last10 6/8 | all 6/8
- N. Ngoy — tackles @ 0.5: [0,1,0,2,2,1,2,1] | last5 5/5 | last10 6/8 | all 6/8
- M. Fofana — shots @ 0.5: [0,0,0,3,1,2,1] | last5 4/5 | last10 4/7 | all 4/7
- M. Koné — foul_involvements @ 1.5: [2,3,1,4,2,9] | last5 4/5 | last10 5/6 | all 5/6
- M. Koné — fouls_drawn @ 0.5: [1,1,1,2,2,4] | last5 5/5 | last10 6/6 | all 6/6

### Equipos — evidencia con ventanas fijas
- France — away_cards @ 2.5: [0,0,0,1,0,3,0,2,0,3] | last5 3/5 | last10 8/10 | all 23/30
  - Rival (Belgium): [3,2,1,2,2,0,1,0,2,5] | last5 4/5 | last10 8/10 | all 22/30
- France — away_corners @ 2.5: [5,6,4,5,9,12,5,7,3,3] | last5 5/5 | last10 10/10 | all 28/30
  - Rival (Belgium): [2,4,14,2,4,8,4,5,1,8] | last5 4/5 | last10 7/10 | all 27/30
- France — away_corners @ 3.5: [5,6,4,5,9,12,5,7,3,3] | last5 3/5 | last10 8/10 | all 24/30
  - Rival (Belgium): [2,4,14,2,4,8,4,5,1,8] | last5 4/5 | last10 7/10 | all 27/30
- France — away_corners @ 7.5: [5,6,4,5,9,12,5,7,3,3] | last5 4/5 | last10 8/10 | all 20/30
  - Rival (Belgium): [2,4,14,2,4,8,4,5,1,8] | last5 3/5 | last10 7/10 | all 17/30
- France — away_goals @ 0.5: [3,3,3,4,3,1,2,0,4,1] | last5 4/5 | last10 9/10 | all 27/30
  - Rival (Belgium): [1,2,5,1,0,5,3,4,1,2] | last5 5/5 | last10 9/10 | all 24/30
- Belgium — away_goals @ 1.5: [1,2,5,1,0,5,3,4,1,2] | last5 1/5 | last10 4/10 | all 13/30
  - Rival (France): [3,3,3,4,3,1,2,0,4,1] | last5 3/5 | last10 3/10 | all 8/30
- Belgium — away_goals @ 2.5: [1,2,5,1,0,5,3,4,1,2] | last5 2/5 | last10 6/10 | all 17/30
  - Rival (France): [3,3,3,4,3,1,2,0,4,1] | last5 4/5 | last10 4/10 | all 17/30
- France — away_shots_on_target @ 3.5: [5,8,5,9,12,5,8,3,9,4] | last5 4/5 | last10 9/10 | all 26/30
  - Rival (Belgium): [2,4,12,3,7,10,5,7,2,8] | last5 4/5 | last10 7/10 | all 26/30
- France — away_shots_on_target @ 4.5: [5,8,5,9,12,5,8,3,9,4] | last5 3/5 | last10 8/10 | all 22/30
  - Rival (Belgium): [2,4,12,3,7,10,5,7,2,8] | last5 4/5 | last10 6/10 | all 19/30
- Belgium — home_cards @ 2.5: [3,2,1,2,2,0,1,0,2,5] | last5 4/5 | last10 8/10 | all 22/30
  - Rival (France): [0,0,0,1,0,3,0,2,0,3] | last5 3/5 | last10 8/10 | all 23/30
- Belgium — home_goals @ 0.5: [1,2,5,1,0,5,3,4,1,2] | last5 5/5 | last10 9/10 | all 24/30
  - Rival (France): [3,3,3,4,3,1,2,0,4,1] | last5 4/5 | last10 9/10 | all 27/30
- France — home_goals @ 1.5: [3,3,3,4,3,1,2,0,4,1] | last5 3/5 | last10 3/10 | all 8/30
  - Rival (Belgium): [1,2,5,1,0,5,3,4,1,2] | last5 1/5 | last10 4/10 | all 13/30
- France — home_goals @ 2.5: [3,3,3,4,3,1,2,0,4,1] | last5 4/5 | last10 4/10 | all 17/30
  - Rival (Belgium): [1,2,5,1,0,5,3,4,1,2] | last5 2/5 | last10 6/10 | all 17/30
- Belgium — home_shots @ 10.5: [5,8,27,15,23,35,19,15,5,19] | last5 4/5 | last10 7/10 | all 25/30
  - Rival (France): [27,11,19,18,25,15,22,10,19,12] | last5 4/5 | last10 9/10 | all 26/30
- Belgium — home_shots @ 11.5: [5,8,27,15,23,35,19,15,5,19] | last5 4/5 | last10 7/10 | all 24/30
  - Rival (France): [27,11,19,18,25,15,22,10,19,12] | last5 4/5 | last10 8/10 | all 25/30
- Belgium — home_shots @ 12.5: [5,8,27,15,23,35,19,15,5,19] | last5 4/5 | last10 7/10 | all 22/30
  - Rival (France): [27,11,19,18,25,15,22,10,19,12] | last5 3/5 | last10 7/10 | all 23/30
- Belgium — home_shots_on_target @ 2.5: [2,4,12,3,7,10,5,7,2,8] | last5 4/5 | last10 8/10 | all 28/30
  - Rival (France): [5,8,5,9,12,5,8,3,9,4] | last5 5/5 | last10 10/10 | all 30/30
- Belgium vs France — total_goals @ 1.5: [2,2,5,2,0,6,5,5,3,2] | last5 5/5 | last10 9/10 | all 25/30
- Belgium vs France — total_goals @ 3.5: [4,4,3,5,3,1,2,2,10,1] | last5 4/5 | last10 6/10 | all 18/30

## Confluencia descriptiva (equipo + rival + jugadores)
- familias de mercado con 2 o más series observadas en la DB; cada serie conserva su línea y sus ventanas fijas (last5/last10/all); sin score ni orden por fuerza

### cards
- France (away) @ 2.5: last5 3/5 | last10 8/10 | all 23/30
- Belgium (home) @ 2.5: last5 4/5 | last10 8/10 | all 22/30

### corners
- France (away) @ 2.5: last5 5/5 | last10 10/10 | all 28/30
- Belgium (home) @ 2.5: last5 4/5 | last10 7/10 | all 27/30
- France (away) @ 3.5: last5 3/5 | last10 8/10 | all 24/30
- Belgium (home) @ 3.5: last5 4/5 | last10 7/10 | all 27/30
- France (away) @ 7.5: last5 4/5 | last10 8/10 | all 20/30
- Belgium (home) @ 7.5: last5 3/5 | last10 7/10 | all 17/30

### foul_involvements
- jugador Y. Tielemans — foul_involvements @ 1.5: last5 5/5 | last10 10/10 | all 10/10
- jugador M. Koné — foul_involvements @ 1.5: last5 4/5 | last10 5/6 | all 5/6

### fouls_committed
- jugador B. Mechele — fouls_committed @ 0.5: last5 5/5 | last10 7/10 | all 7/10
- jugador N. Raskin — fouls_committed @ 0.5: last5 5/5 | last10 9/10 | all 9/10
- jugador Y. Tielemans — fouls_committed @ 0.5: last5 4/5 | last10 8/10 | all 8/10
- jugador N. Ngoy — fouls_committed @ 0.5: last5 5/5 | last10 6/8 | all 6/8

### fouls_drawn
- jugador Y. Tielemans — fouls_drawn @ 0.5: last5 5/5 | last10 10/10 | all 10/10
- jugador M. Koné — fouls_drawn @ 0.5: last5 5/5 | last10 6/6 | all 6/6

### goals
- France (away) @ 0.5: last5 4/5 | last10 9/10 | all 27/30
- Belgium (home) @ 0.5: last5 5/5 | last10 9/10 | all 24/30
- Belgium (home) @ 1.5: last5 1/5 | last10 4/10 | all 13/30
- France (away) @ 1.5: last5 3/5 | last10 3/10 | all 8/30
- Belgium (home) @ 2.5: last5 2/5 | last10 6/10 | all 17/30
- France (away) @ 2.5: last5 4/5 | last10 4/10 | all 17/30

### shots
- Belgium (home) @ 10.5: last5 4/5 | last10 7/10 | all 25/30
- France (away) @ 10.5: last5 4/5 | last10 9/10 | all 26/30
- Belgium (home) @ 11.5: last5 4/5 | last10 7/10 | all 24/30
- France (away) @ 11.5: last5 4/5 | last10 8/10 | all 25/30
- Belgium (home) @ 12.5: last5 4/5 | last10 7/10 | all 22/30
- France (away) @ 12.5: last5 3/5 | last10 7/10 | all 23/30
- jugador D. Lukébakio — shots @ 0.5: last5 3/5 | last10 6/10 | all 13/28
- jugador C. De Ketelaere — shots @ 0.5: last5 4/5 | last10 6/10 | all 13/27
- jugador C. De Ketelaere — shots @ 1.5: last5 3/5 | last10 4/10 | all 8/27
- jugador B. Barcola — shots @ 0.5: last5 5/5 | last10 9/10 | all 20/25
- jugador A. Rabiot — shots @ 0.5: last5 5/5 | last10 8/10 | all 15/24
- jugador K. De Bruyne — shots @ 0.5: last5 4/5 | last10 8/10 | all 22/24
- jugador Y. Tielemans — shots @ 0.5: last5 5/5 | last10 8/10 | all 18/24
- jugador K. De Bruyne — shots @ 1.5: last5 4/5 | last10 7/10 | all 21/24
- jugador M. Fofana — shots @ 0.5: last5 4/5 | last10 4/7 | all 4/7

### shots_on_target
- France (away) @ 3.5: last5 4/5 | last10 9/10 | all 26/30
- Belgium (home) @ 3.5: last5 4/5 | last10 7/10 | all 26/30
- France (away) @ 4.5: last5 3/5 | last10 8/10 | all 22/30
- Belgium (home) @ 4.5: last5 4/5 | last10 6/10 | all 19/30
- Belgium (home) @ 2.5: last5 4/5 | last10 8/10 | all 28/30
- France (away) @ 2.5: last5 5/5 | last10 10/10 | all 30/30
- jugador C. De Ketelaere — shots_on_target @ 0.5: last5 4/5 | last10 5/10 | all 11/27
- jugador K. De Bruyne — shots_on_target @ 0.5: last5 3/5 | last10 6/10 | all 19/24
- jugador O. Dembélé — shots_on_target @ 0.5: last5 4/5 | last10 6/10 | all 12/22

### tackles
- jugador M. Olise — tackles @ 0.5: last5 5/5 | last10 9/10 | all 10/13
- jugador M. Olise — tackles @ 1.5: last5 4/5 | last10 6/10 | all 6/13
- jugador M. Olise — tackles @ 2.5: last5 3/5 | last10 4/10 | all 4/13
- jugador K. De Bruyne — tackles @ 0.5: last5 4/5 | last10 6/10 | all 7/12
- jugador A. Rabiot — tackles @ 0.5: last5 4/5 | last10 8/10 | all 9/11
- jugador A. Rabiot — tackles @ 1.5: last5 3/5 | last10 5/10 | all 6/11
- jugador B. Mechele — tackles @ 0.5: last5 4/5 | last10 6/10 | all 6/10
- jugador N. Ngoy — tackles @ 0.5: last5 5/5 | last10 6/8 | all 6/8

## Cuotas

| mercado | línea | casa | lado | cuota | prob. implícita | capturada |
|---|---|---|---|---|---|---|
| away_cards | 2.5 | Ladbrokes | over | 3 | 0.333 | 2026-09-28T02:36:28.909Z |
| away_cards | 2.5 | Ladbrokes | under | 1.35 | 0.741 | 2026-09-28T02:36:28.909Z |
| away_corners | 2.5 | Kambi | over | 1.12 | 0.893 | 2026-09-28T02:36:28.909Z |
| away_corners | 2.5 | Kambi | under | 4.8 | 0.208 | 2026-09-28T02:36:28.909Z |
| away_corners | 3.5 | Kambi | over | 1.34 | 0.746 | 2026-09-28T02:36:28.909Z |
| away_corners | 3.5 | Kambi | under | 2.85 | 0.351 | 2026-09-28T02:36:28.909Z |
| away_corners | 3.5 | Ladbrokes | over | 1.33 | 0.752 | 2026-09-28T02:36:28.909Z |
| away_corners | 3.5 | Ladbrokes | under | 3 | 0.333 | 2026-09-28T02:36:28.909Z |
| away_corners | 7.5 | Kambi | over | 4.4 | 0.227 | 2026-09-28T02:36:28.909Z |
| away_corners | 7.5 | Kambi | under | 1.14 | 0.877 | 2026-09-28T02:36:28.909Z |
| away_goals | 0.5 | Bet365 | over | 1.13 | 0.885 | 2026-09-28T02:36:28.909Z |
| away_goals | 0.5 | Bet365 | under | 6 | 0.167 | 2026-09-28T02:36:28.909Z |
| away_goals | 0.5 | Kambi | over | 1.1 | 0.909 | 2026-09-28T02:36:28.909Z |
| away_goals | 0.5 | Kambi | under | 5.4 | 0.185 | 2026-09-28T02:36:28.909Z |
| away_goals | 0.5 | Ladbrokes | over | 1.12 | 0.893 | 2026-09-28T02:36:28.909Z |
| away_goals | 0.5 | Ladbrokes | under | 5.5 | 0.182 | 2026-09-28T02:36:28.909Z |
| away_goals | 1.5 | Bet365 | over | 1.62 | 0.617 | 2026-09-28T02:36:28.909Z |
| away_goals | 1.5 | Bet365 | under | 2.2 | 0.455 | 2026-09-28T02:36:28.909Z |
| away_goals | 1.5 | Kambi | over | 1.62 | 0.617 | 2026-09-28T02:36:28.909Z |
| away_goals | 1.5 | Kambi | under | 2.14 | 0.467 | 2026-09-28T02:36:28.909Z |
| away_goals | 1.5 | Ladbrokes | over | 1.67 | 0.599 | 2026-09-28T02:36:28.909Z |
| away_goals | 1.5 | Ladbrokes | under | 2.1 | 0.476 | 2026-09-28T02:36:28.909Z |
| away_goals | 2.5 | Bet365 | over | 3 | 0.333 | 2026-09-28T02:36:28.909Z |
| away_goals | 2.5 | Bet365 | under | 1.36 | 0.735 | 2026-09-28T02:36:28.909Z |
| away_goals | 2.5 | Kambi | over | 3 | 0.333 | 2026-09-28T02:36:28.909Z |
| away_goals | 2.5 | Kambi | under | 1.32 | 0.758 | 2026-09-28T02:36:28.909Z |
| away_goals | 2.5 | Ladbrokes | over | 3.1 | 0.323 | 2026-09-28T02:36:28.909Z |
| away_goals | 2.5 | Ladbrokes | under | 1.33 | 0.752 | 2026-09-28T02:36:28.909Z |
| away_shots_on_target | 3.5 | Kambi | over | 1.21 | 0.826 | 2026-09-28T02:36:28.909Z |
| away_shots_on_target | 3.5 | Kambi | under | 3.25 | 0.308 | 2026-09-28T02:36:28.909Z |
| away_shots_on_target | 4.5 | Kambi | over | 1.52 | 0.658 | 2026-09-28T02:36:28.909Z |
| away_shots_on_target | 4.5 | Kambi | under | 2.1 | 0.476 | 2026-09-28T02:36:28.909Z |
| home_cards | 2.5 | Bet365 | over | 2.2 | 0.455 | 2026-09-28T02:36:28.909Z |
| home_cards | 2.5 | Bet365 | under | 1.62 | 0.617 | 2026-09-28T02:36:28.909Z |
| home_cards | 2.5 | Ladbrokes | over | 2.3 | 0.435 | 2026-09-28T02:36:28.909Z |
| home_cards | 2.5 | Ladbrokes | under | 1.55 | 0.645 | 2026-09-28T02:36:28.909Z |
| home_goals | 0.5 | Bet365 | over | 1.29 | 0.775 | 2026-09-28T02:36:28.909Z |
| home_goals | 0.5 | Bet365 | under | 3.5 | 0.286 | 2026-09-28T02:36:28.909Z |
| home_goals | 0.5 | Kambi | over | 1.29 | 0.775 | 2026-09-28T02:36:28.909Z |
| home_goals | 0.5 | Kambi | under | 3.3 | 0.303 | 2026-09-28T02:36:28.909Z |
| home_goals | 0.5 | Ladbrokes | over | 1.28 | 0.781 | 2026-09-28T02:36:28.909Z |
| home_goals | 0.5 | Ladbrokes | under | 3.4 | 0.294 | 2026-09-28T02:36:28.909Z |
| home_goals | 1.5 | Bet365 | over | 2.38 | 0.420 | 2026-09-28T02:36:28.909Z |
| home_goals | 1.5 | Bet365 | under | 1.53 | 0.654 | 2026-09-28T02:36:28.909Z |
| home_goals | 1.5 | Kambi | over | 2.43 | 0.412 | 2026-09-28T02:36:28.909Z |
| home_goals | 1.5 | Kambi | under | 1.5 | 0.667 | 2026-09-28T02:36:28.909Z |
| home_goals | 1.5 | Ladbrokes | over | 2.37 | 0.422 | 2026-09-28T02:36:28.909Z |
| home_goals | 1.5 | Ladbrokes | under | 1.53 | 0.654 | 2026-09-28T02:36:28.909Z |
| home_goals | 2.5 | Bet365 | over | 6 | 0.167 | 2026-09-28T02:36:28.909Z |
| home_goals | 2.5 | Bet365 | under | 1.13 | 0.885 | 2026-09-28T02:36:28.909Z |
| home_goals | 2.5 | Kambi | over | 5.75 | 0.174 | 2026-09-28T02:36:28.909Z |
| home_goals | 2.5 | Kambi | under | 1.11 | 0.901 | 2026-09-28T02:36:28.909Z |
| home_goals | 2.5 | Ladbrokes | over | 5.25 | 0.190 | 2026-09-28T02:36:28.909Z |
| home_goals | 2.5 | Ladbrokes | under | 1.13 | 0.885 | 2026-09-28T02:36:28.909Z |
| home_shots | 10.5 | Kambi | over | 1.57 | 0.637 | 2026-09-28T02:36:28.909Z |
| home_shots | 10.5 | Kambi | under | 2 | 0.500 | 2026-09-28T02:36:28.909Z |
| home_shots | 11.5 | Kambi | over | 1.86 | 0.538 | 2026-09-28T02:36:28.909Z |
| home_shots | 11.5 | Kambi | under | 1.67 | 0.599 | 2026-09-28T02:36:28.909Z |
| home_shots | 12.5 | Bet365 | over | 1.83 | 0.546 | 2026-09-28T02:36:28.909Z |
| home_shots | 12.5 | Bet365 | under | 1.83 | 0.546 | 2026-09-28T02:36:28.909Z |
| home_shots | 12.5 | Kambi | over | 2.25 | 0.444 | 2026-09-28T02:36:28.909Z |
| home_shots | 12.5 | Kambi | under | 1.45 | 0.690 | 2026-09-28T02:36:28.909Z |
| home_shots_on_target | 2.5 | Kambi | over | 1.25 | 0.800 | 2026-09-28T02:36:28.909Z |
| home_shots_on_target | 2.5 | Kambi | under | 3 | 0.333 | 2026-09-28T02:36:28.909Z |
| total_goals | 1.5 | Bet365 | over | 1.14 | 0.877 | 2026-09-28T02:36:28.909Z |
| total_goals | 1.5 | Bet365 | under | 5.5 | 0.182 | 2026-09-28T02:36:28.909Z |
| total_goals | 1.5 | Kambi | over | 1.14 | 0.877 | 2026-09-28T02:36:28.909Z |
| total_goals | 1.5 | Kambi | under | 5.2 | 0.192 | 2026-09-28T02:36:28.909Z |
| total_goals | 1.5 | Ladbrokes | over | 1.14 | 0.877 | 2026-09-28T02:36:28.909Z |
| total_goals | 1.5 | Ladbrokes | under | 5 | 0.200 | 2026-09-28T02:36:28.909Z |
| total_goals | 1.5 | Paddy Power | over | 1.11 | 0.901 | 2026-09-28T02:36:28.909Z |
| total_goals | 1.5 | Paddy Power | under | 5.5 | 0.182 | 2026-09-28T02:36:28.909Z |
| total_goals | 3.5 | Altenar | over | 2.14 | 0.467 | 2026-09-28T02:36:28.909Z |
| total_goals | 3.5 | Altenar | under | 1.6 | 0.625 | 2026-09-28T02:36:28.909Z |
| total_goals | 3.5 | Bet365 | over | 2.2 | 0.455 | 2026-09-28T02:36:28.909Z |
| total_goals | 3.5 | Bet365 | under | 1.62 | 0.617 | 2026-09-28T02:36:28.909Z |
| total_goals | 3.5 | Kambi | over | 2.33 | 0.429 | 2026-09-28T02:36:28.909Z |
| total_goals | 3.5 | Kambi | under | 1.57 | 0.637 | 2026-09-28T02:36:28.909Z |
| total_goals | 3.5 | Ladbrokes | over | 2.25 | 0.444 | 2026-09-28T02:36:28.909Z |
| total_goals | 3.5 | Ladbrokes | under | 1.57 | 0.637 | 2026-09-28T02:36:28.909Z |
| total_goals | 3.5 | Paddy Power | over | 2.25 | 0.444 | 2026-09-28T02:36:28.909Z |
| total_goals | 3.5 | Paddy Power | under | 1.57 | 0.637 | 2026-09-28T02:36:28.909Z |
| total_cards | 3.5 | Kambi | over | 1.53 | 0.654 | 2026-09-28T02:36:28.909Z |
| total_cards | 3.5 | Kambi | under | 2.3 | 0.435 | 2026-09-28T02:36:28.909Z |
| total_cards | 3.5 | Ladbrokes | over | 1.53 | 0.654 | 2026-09-28T02:36:28.909Z |
| total_cards | 3.5 | Ladbrokes | under | 2.37 | 0.422 | 2026-09-28T02:36:28.909Z |
| total_cards | 3.5 | Paddy Power | over | 1.53 | 0.654 | 2026-09-28T02:36:28.909Z |
| total_cards | 3.5 | Paddy Power | under | 2.3 | 0.435 | 2026-09-28T02:36:28.909Z |
| total_cards | 4.5 | Bet365 | over | 2.2 | 0.455 | 2026-09-28T02:36:28.909Z |
| total_cards | 4.5 | Bet365 | under | 1.62 | 0.617 | 2026-09-28T02:36:28.909Z |
| total_cards | 4.5 | Kambi | over | 2.08 | 0.481 | 2026-09-28T02:36:28.909Z |
| total_cards | 4.5 | Kambi | under | 1.64 | 0.610 | 2026-09-28T02:36:28.909Z |
| total_cards | 4.5 | Ladbrokes | over | 2.2 | 0.455 | 2026-09-28T02:36:28.909Z |
| total_cards | 4.5 | Ladbrokes | under | 1.6 | 0.625 | 2026-09-28T02:36:28.909Z |
| total_cards | 4.5 | Paddy Power | over | 2.2 | 0.455 | 2026-09-28T02:36:28.909Z |
| total_cards | 4.5 | Paddy Power | under | 1.57 | 0.637 | 2026-09-28T02:36:28.909Z |
| total_cards | 5.5 | Kambi | over | 3 | 0.333 | 2026-09-28T02:36:28.909Z |
| total_cards | 5.5 | Kambi | under | 1.32 | 0.758 | 2026-09-28T02:36:28.909Z |
| total_cards | 5.5 | Ladbrokes | over | 3.4 | 0.294 | 2026-09-28T02:36:28.909Z |
| total_cards | 5.5 | Ladbrokes | under | 1.28 | 0.781 | 2026-09-28T02:36:28.909Z |
| total_corners | 6.5 | Bet365 | over | 1.2 | 0.833 | 2026-09-28T02:36:28.909Z |
| total_corners | 6.5 | Kambi | over | 1.16 | 0.862 | 2026-09-28T02:36:28.909Z |
| total_corners | 6.5 | Kambi | under | 4.4 | 0.227 | 2026-09-28T02:36:28.909Z |
| total_corners | 6.5 | Ladbrokes | over | 1.2 | 0.833 | 2026-09-28T02:36:28.909Z |
| total_corners | 6.5 | Ladbrokes | under | 4.2 | 0.238 | 2026-09-28T02:36:28.909Z |
| total_corners | 7.5 | Kambi | over | 1.32 | 0.758 | 2026-09-28T02:36:28.909Z |
| total_corners | 7.5 | Kambi | under | 3.05 | 0.328 | 2026-09-28T02:36:28.909Z |
| total_corners | 7.5 | Ladbrokes | over | 1.4 | 0.714 | 2026-09-28T02:36:28.909Z |
| total_corners | 7.5 | Ladbrokes | under | 2.8 | 0.357 | 2026-09-28T02:36:28.909Z |
| total_corners | 10.5 | Kambi | over | 2.43 | 0.412 | 2026-09-28T02:36:28.909Z |
| total_corners | 10.5 | Kambi | under | 1.48 | 0.676 | 2026-09-28T02:36:28.909Z |
| total_corners | 10.5 | Ladbrokes | over | 2.9 | 0.345 | 2026-09-28T02:36:28.909Z |
| total_corners | 10.5 | Ladbrokes | under | 1.36 | 0.735 | 2026-09-28T02:36:28.909Z |
| total_corners | 10.5 | Paddy Power | over | 2.88 | 0.347 | 2026-09-28T02:36:28.909Z |
| total_corners | 10.5 | Paddy Power | under | 1.36 | 0.735 | 2026-09-28T02:36:28.909Z |
| total_corners | 11.5 | Kambi | over | 3.2 | 0.312 | 2026-09-28T02:36:28.909Z |
| total_corners | 11.5 | Kambi | under | 1.29 | 0.775 | 2026-09-28T02:36:28.909Z |
| total_corners | 11.5 | Ladbrokes | over | 4.2 | 0.238 | 2026-09-28T02:36:28.909Z |
| total_corners | 11.5 | Ladbrokes | under | 1.2 | 0.833 | 2026-09-28T02:36:28.909Z |
| total_corners | 11.5 | Paddy Power | over | 4 | 0.250 | 2026-09-28T02:36:28.909Z |
| total_corners | 11.5 | Paddy Power | under | 1.2 | 0.833 | 2026-09-28T02:36:28.909Z |
| total_corners | 12.5 | Kambi | over | 4.2 | 0.238 | 2026-09-28T02:36:28.909Z |
| total_corners | 12.5 | Kambi | under | 1.18 | 0.847 | 2026-09-28T02:36:28.909Z |
| total_corners | 12.5 | Paddy Power | over | 6.5 | 0.154 | 2026-09-28T02:36:28.909Z |
| total_corners | 12.5 | Paddy Power | under | 1.08 | 0.926 | 2026-09-28T02:36:28.909Z |
| total_shots_on_target | 6.5 | Kambi | over | 1.16 | 0.862 | 2026-09-28T02:36:28.909Z |
| total_shots_on_target | 6.5 | Kambi | under | 3.9 | 0.256 | 2026-09-28T02:36:28.909Z |
| total_shots_on_target | 7.5 | Kambi | over | 1.35 | 0.741 | 2026-09-28T02:36:28.909Z |
| total_shots_on_target | 7.5 | Kambi | under | 2.7 | 0.370 | 2026-09-28T02:36:28.909Z |
| total_shots_on_target | 8.5 | Kambi | over | 1.64 | 0.610 | 2026-09-28T02:36:28.909Z |
| total_shots_on_target | 8.5 | Kambi | under | 2 | 0.500 | 2026-09-28T02:36:28.909Z |
- Cuota manual Betano (away_cards @ 2.5): no cargada
- Cuota manual Betano (away_corners @ 2.5): no cargada
- Cuota manual Betano (away_corners @ 3.5): no cargada
- Cuota manual Betano (away_corners @ 7.5): no cargada
- Cuota manual Betano (away_goals @ 0.5): no cargada
- Cuota manual Betano (away_goals @ 1.5): no cargada
- Cuota manual Betano (away_goals @ 2.5): no cargada
- Cuota manual Betano (away_shots_on_target @ 3.5): no cargada
- Cuota manual Betano (away_shots_on_target @ 4.5): no cargada
- Cuota manual Betano (home_cards @ 2.5): no cargada
- Cuota manual Betano (home_goals @ 0.5): no cargada
- Cuota manual Betano (home_goals @ 1.5): no cargada
- Cuota manual Betano (home_goals @ 2.5): no cargada
- Cuota manual Betano (home_shots @ 10.5): no cargada
- Cuota manual Betano (home_shots @ 11.5): no cargada
- Cuota manual Betano (home_shots @ 12.5): no cargada
- Cuota manual Betano (home_shots_on_target @ 2.5): no cargada
- Cuota manual Betano (total_goals @ 1.5): no cargada
- Cuota manual Betano (total_goals @ 3.5): no cargada
- Cuota manual Betano (1x2_shots_on_target): no cargada
- Cuota manual Betano (btts): no cargada
- Cuota manual Betano (total_cards @ 3.5): no cargada
- Cuota manual Betano (total_cards @ 4.5): no cargada
- Cuota manual Betano (total_cards @ 5.5): no cargada
- Cuota manual Betano (total_corners @ 6.5): no cargada
- Cuota manual Betano (total_corners @ 7.5): no cargada
- Cuota manual Betano (total_corners @ 10.5): no cargada
- Cuota manual Betano (total_corners @ 11.5): no cargada
- Cuota manual Betano (total_corners @ 12.5): no cargada
- Cuota manual Betano (total_shots_on_target @ 6.5): no cargada
- Cuota manual Betano (total_shots_on_target @ 7.5): no cargada
- Cuota manual Betano (total_shots_on_target @ 8.5): no cargada

## Nota de cierre
Este reporte es descriptivo. No constituye una recomendación de apuesta ni una probabilidad validada de resultado futuro.

---

## Türkiye vs Italy (33662311) - kickoff 2026-09-28T18:45:00.000Z

# Türkiye vs Italy
- Competición: UEFA Nations League
- Kickoff (UTC): desconocido
- Flags de calidad: leakage=false; suspect=true
- sort_criteria: sample_size_desc

## Mercados (orden: mayor muestra disponible, no fuerza de señal)

### 1. away_cards @ 3.5 — Italy (away)
- basis: team_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=2.4, median=3, min=1, max=4
-   last10: n=10, hits=9, rate=0.900, mean=2, median=2.5, min=0, max=4
-   all: n=30, hits=27, rate=0.900, mean=1.8, median=2, min=0, max=4
- Split home/away:
-   home: n=15, hits=15, rate=1.000, mean=1.6, median=2
-   away: n=15, hits=12, rate=0.800, mean=2, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=1.2, median=1, min=0, max=3
    last10: n=10, hits=8, rate=0.800, mean=2.1, median=1.5, min=0, max=5
    all: n=30, hits=24, rate=0.800, mean=2.5333, median=2, min=0, max=11
  Split home/away:
    home: n=15, hits=15, rate=1.000, mean=1.6, median=1
    away: n=15, hits=9, rate=0.600, mean=3.4667, median=3
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 2. away_corners @ 2.5 — Italy (away)
- basis: team_own_stats
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=6, median=4, min=3, max=12
-   last10: n=10, hits=10, rate=1.000, mean=6.5, median=5, min=3, max=13
-   all: n=30, hits=25, rate=0.833, mean=6.0333, median=5.5, min=0, max=13
- Split home/away:
-   home: n=15, hits=14, rate=0.933, mean=6.7333, median=6
-   away: n=15, hits=11, rate=0.733, mean=5.3333, median=4
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=7.8, median=8, min=2, max=12
    last10: n=10, hits=8, rate=0.800, mean=7.5, median=8.5, min=2, max=13
    all: n=30, hits=27, rate=0.900, mean=6.5, median=6.5, min=2, max=13
  Split home/away:
    home: n=15, hits=14, rate=0.933, mean=7.3333, median=8
    away: n=15, hits=13, rate=0.867, mean=5.6667, median=5
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 3. away_corners @ 3.5 — Italy (away)
- basis: team_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=6, median=4, min=3, max=12
-   last10: n=10, hits=9, rate=0.900, mean=6.5, median=5, min=3, max=13
-   all: n=30, hits=23, rate=0.767, mean=6.0333, median=5.5, min=0, max=13
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=6.7333, median=6
-   away: n=15, hits=10, rate=0.667, mean=5.3333, median=4
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=7.8, median=8, min=2, max=12
    last10: n=10, hits=7, rate=0.700, mean=7.5, median=8.5, min=2, max=13
    all: n=30, hits=23, rate=0.767, mean=6.5, median=6.5, min=2, max=13
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=7.3333, median=8
    away: n=15, hits=10, rate=0.667, mean=5.6667, median=5
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 4. away_goals @ 0.5 — Italy (away)
- basis: team_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=1, median=1, min=0, max=2
-   last10: n=10, hits=9, rate=0.900, mean=1.9, median=1.5, min=0, max=5
-   all: n=30, hits=25, rate=0.833, mean=1.8667, median=2, min=0, max=5
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=2.0667, median=2
-   away: n=15, hits=12, rate=0.800, mean=1.6667, median=1
- Rival (contexto equivalente):
  FM reportado: hits 5/5; mejor ventana: 5 (5/5)
  Ventanas propias:
    last5: n=5, hits=2, rate=0.400, mean=1, median=0, min=0, max=3
    last10: n=10, hits=7, rate=0.700, mean=1.5, median=1.5, min=0, max=4
    all: n=30, hits=22, rate=0.733, mean=1.9, median=2, min=0, max=6
  Split home/away:
    home: n=15, hits=10, rate=0.667, mean=1.8667, median=2
    away: n=15, hits=12, rate=0.800, mean=1.9333, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 5. away_goals @ 0.5 — Türkiye (home)
- basis: team_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: n=5, hits=2, rate=0.400, mean=1, median=0, min=0, max=3
-   last10: n=10, hits=7, rate=0.700, mean=1.5, median=1.5, min=0, max=4
-   all: n=30, hits=22, rate=0.733, mean=1.9, median=2, min=0, max=6
- Split home/away:
-   home: n=15, hits=10, rate=0.667, mean=1.8667, median=2
-   away: n=15, hits=12, rate=0.800, mean=1.9333, median=2
- Rival (contexto equivalente):
  FM reportado: hits 9/10; mejor ventana: 10 (9/10)
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=1, median=1, min=0, max=2
    last10: n=10, hits=9, rate=0.900, mean=1.9, median=1.5, min=0, max=5
    all: n=30, hits=25, rate=0.833, mean=1.8667, median=2, min=0, max=5
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=2.0667, median=2
    away: n=15, hits=12, rate=0.800, mean=1.6667, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 6. away_goals @ 2.5 — Italy (away)
- basis: team_own_stats
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1, median=1, min=0, max=2
-   last10: n=10, hits=7, rate=0.700, mean=1.9, median=1.5, min=0, max=5
-   all: n=30, hits=22, rate=0.733, mean=1.8667, median=2, min=0, max=5
- Split home/away:
-   home: n=15, hits=11, rate=0.733, mean=2.0667, median=2
-   away: n=15, hits=11, rate=0.733, mean=1.6667, median=1
- Rival (contexto equivalente):
  FM reportado: hits 10/10; mejor ventana: 10 (10/10)
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=1, median=0, min=0, max=3
    last10: n=10, hits=8, rate=0.800, mean=1.5, median=1.5, min=0, max=4
    all: n=30, hits=19, rate=0.633, mean=1.9, median=2, min=0, max=6
  Split home/away:
    home: n=15, hits=8, rate=0.533, mean=1.8667, median=2
    away: n=15, hits=11, rate=0.733, mean=1.9333, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 7. away_goals @ 2.5 — Türkiye (home)
- basis: team_own_stats
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=1, median=0, min=0, max=3
-   last10: n=10, hits=8, rate=0.800, mean=1.5, median=1.5, min=0, max=4
-   all: n=30, hits=19, rate=0.633, mean=1.9, median=2, min=0, max=6
- Split home/away:
-   home: n=15, hits=8, rate=0.533, mean=1.8667, median=2
-   away: n=15, hits=11, rate=0.733, mean=1.9333, median=2
- Rival (contexto equivalente):
  FM reportado: hits 6/6; mejor ventana: no determinable
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=1, median=1, min=0, max=2
    last10: n=10, hits=7, rate=0.700, mean=1.9, median=1.5, min=0, max=5
    all: n=30, hits=22, rate=0.733, mean=1.8667, median=2, min=0, max=5
  Split home/away:
    home: n=15, hits=11, rate=0.733, mean=2.0667, median=2
    away: n=15, hits=11, rate=0.733, mean=1.6667, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 8. away_shots @ 10.5 — Italy (away)
- basis: team_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: n=5, hits=3, rate=0.600, mean=14.8, median=15, min=9, max=21
-   last10: n=10, hits=8, rate=0.800, mean=17.4, median=16.5, min=9, max=28
-   all: n=30, hits=22, rate=0.733, mean=15.1, median=13, min=4, max=40
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=17.6667, median=17
-   away: n=15, hits=9, rate=0.600, mean=12.5333, median=11
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=19.8, median=16, min=9, max=32
    last10: n=10, hits=9, rate=0.900, mean=17.8, median=16, min=9, max=32
    all: n=30, hits=22, rate=0.733, mean=16.1333, median=16, min=5, max=32
  Split home/away:
    home: n=15, hits=12, rate=0.800, mean=17.6, median=18
    away: n=15, hits=10, rate=0.667, mean=14.6667, median=15
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 9. away_shots @ 12.5 — Italy (away)
- basis: team_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: n=5, hits=3, rate=0.600, mean=14.8, median=15, min=9, max=21
-   last10: n=10, hits=8, rate=0.800, mean=17.4, median=16.5, min=9, max=28
-   all: n=30, hits=16, rate=0.533, mean=15.1, median=13, min=4, max=40
- Split home/away:
-   home: n=15, hits=12, rate=0.800, mean=17.6667, median=17
-   away: n=15, hits=4, rate=0.267, mean=12.5333, median=11
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=19.8, median=16, min=9, max=32
    last10: n=10, hits=7, rate=0.700, mean=17.8, median=16, min=9, max=32
    all: n=30, hits=18, rate=0.600, mean=16.1333, median=16, min=5, max=32
  Split home/away:
    home: n=15, hits=9, rate=0.600, mean=17.6, median=18
    away: n=15, hits=9, rate=0.600, mean=14.6667, median=15
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 10. away_shots @ 13.5 — Italy (away)
- basis: team_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: n=5, hits=3, rate=0.600, mean=14.8, median=15, min=9, max=21
-   last10: n=10, hits=8, rate=0.800, mean=17.4, median=16.5, min=9, max=28
-   all: n=30, hits=14, rate=0.467, mean=15.1, median=13, min=4, max=40
- Split home/away:
-   home: n=15, hits=10, rate=0.667, mean=17.6667, median=17
-   away: n=15, hits=4, rate=0.267, mean=12.5333, median=11
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=19.8, median=16, min=9, max=32
    last10: n=10, hits=6, rate=0.600, mean=17.8, median=16, min=9, max=32
    all: n=30, hits=17, rate=0.567, mean=16.1333, median=16, min=5, max=32
  Split home/away:
    home: n=15, hits=9, rate=0.600, mean=17.6, median=18
    away: n=15, hits=8, rate=0.533, mean=14.6667, median=15
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 11. away_shots @ 14.5 — Italy (away)
- basis: team_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: n=5, hits=3, rate=0.600, mean=14.8, median=15, min=9, max=21
-   last10: n=10, hits=8, rate=0.800, mean=17.4, median=16.5, min=9, max=28
-   all: n=30, hits=14, rate=0.467, mean=15.1, median=13, min=4, max=40
- Split home/away:
-   home: n=15, hits=10, rate=0.667, mean=17.6667, median=17
-   away: n=15, hits=4, rate=0.267, mean=12.5333, median=11
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=19.8, median=16, min=9, max=32
    last10: n=10, hits=6, rate=0.600, mean=17.8, median=16, min=9, max=32
    all: n=30, hits=17, rate=0.567, mean=16.1333, median=16, min=5, max=32
  Split home/away:
    home: n=15, hits=9, rate=0.600, mean=17.6, median=18
    away: n=15, hits=8, rate=0.533, mean=14.6667, median=15
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 12. home_cards @ 1.5 — Türkiye (home)
- basis: team_own_stats
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=1.2, median=1, min=0, max=3
-   last10: n=10, hits=5, rate=0.500, mean=2.1, median=1.5, min=0, max=5
-   all: n=30, hits=13, rate=0.433, mean=2.5333, median=2, min=0, max=11
- Split home/away:
-   home: n=15, hits=8, rate=0.533, mean=1.6, median=1
-   away: n=15, hits=5, rate=0.333, mean=3.4667, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=2, rate=0.400, mean=2.4, median=3, min=1, max=4
    last10: n=10, hits=4, rate=0.400, mean=2, median=2.5, min=0, max=4
    all: n=30, hits=13, rate=0.433, mean=1.8, median=2, min=0, max=4
  Split home/away:
    home: n=15, hits=7, rate=0.467, mean=1.6, median=2
    away: n=15, hits=6, rate=0.400, mean=2, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 13. home_cards @ 2.5 — Türkiye (home)
- basis: team_own_stats
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=1.2, median=1, min=0, max=3
-   last10: n=10, hits=6, rate=0.600, mean=2.1, median=1.5, min=0, max=5
-   all: n=30, hits=17, rate=0.567, mean=2.5333, median=2, min=0, max=11
- Split home/away:
-   home: n=15, hits=10, rate=0.667, mean=1.6, median=1
-   away: n=15, hits=7, rate=0.467, mean=3.4667, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=2, rate=0.400, mean=2.4, median=3, min=1, max=4
    last10: n=10, hits=5, rate=0.500, mean=2, median=2.5, min=0, max=4
    all: n=30, hits=21, rate=0.700, mean=1.8, median=2, min=0, max=4
  Split home/away:
    home: n=15, hits=12, rate=0.800, mean=1.6, median=2
    away: n=15, hits=9, rate=0.600, mean=2, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 14. home_corners @ 2.5 — Türkiye (home)
- basis: team_own_stats
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=7.8, median=8, min=2, max=12
-   last10: n=10, hits=8, rate=0.800, mean=7.5, median=8.5, min=2, max=13
-   all: n=30, hits=27, rate=0.900, mean=6.5, median=6.5, min=2, max=13
- Split home/away:
-   home: n=15, hits=14, rate=0.933, mean=7.3333, median=8
-   away: n=15, hits=13, rate=0.867, mean=5.6667, median=5
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=6, median=4, min=3, max=12
    last10: n=10, hits=10, rate=1.000, mean=6.5, median=5, min=3, max=13
    all: n=30, hits=25, rate=0.833, mean=6.0333, median=5.5, min=0, max=13
  Split home/away:
    home: n=15, hits=14, rate=0.933, mean=6.7333, median=6
    away: n=15, hits=11, rate=0.733, mean=5.3333, median=4
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 15. home_corners @ 3.5 — Türkiye (home)
- basis: team_own_stats
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=7.8, median=8, min=2, max=12
-   last10: n=10, hits=7, rate=0.700, mean=7.5, median=8.5, min=2, max=13
-   all: n=30, hits=23, rate=0.767, mean=6.5, median=6.5, min=2, max=13
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=7.3333, median=8
-   away: n=15, hits=10, rate=0.667, mean=5.6667, median=5
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=6, median=4, min=3, max=12
    last10: n=10, hits=9, rate=0.900, mean=6.5, median=5, min=3, max=13
    all: n=30, hits=23, rate=0.767, mean=6.0333, median=5.5, min=0, max=13
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=6.7333, median=6
    away: n=15, hits=10, rate=0.667, mean=5.3333, median=4
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 16. home_corners @ 4.5 — Türkiye (home)
- basis: team_own_stats
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=7.8, median=8, min=2, max=12
-   last10: n=10, hits=7, rate=0.700, mean=7.5, median=8.5, min=2, max=13
-   all: n=30, hits=21, rate=0.700, mean=6.5, median=6.5, min=2, max=13
- Split home/away:
-   home: n=15, hits=12, rate=0.800, mean=7.3333, median=8
-   away: n=15, hits=9, rate=0.600, mean=5.6667, median=5
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=2, rate=0.400, mean=6, median=4, min=3, max=12
    last10: n=10, hits=5, rate=0.500, mean=6.5, median=5, min=3, max=13
    all: n=30, hits=17, rate=0.567, mean=6.0333, median=5.5, min=0, max=13
  Split home/away:
    home: n=15, hits=10, rate=0.667, mean=6.7333, median=6
    away: n=15, hits=7, rate=0.467, mean=5.3333, median=4
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 17. home_goals @ 2.5 — Italy (away)
- basis: team_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1, median=1, min=0, max=2
-   last10: n=10, hits=7, rate=0.700, mean=1.9, median=1.5, min=0, max=5
-   all: n=30, hits=22, rate=0.733, mean=1.8667, median=2, min=0, max=5
- Split home/away:
-   home: n=15, hits=11, rate=0.733, mean=2.0667, median=2
-   away: n=15, hits=11, rate=0.733, mean=1.6667, median=1
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=1, median=0, min=0, max=3
    last10: n=10, hits=8, rate=0.800, mean=1.5, median=1.5, min=0, max=4
    all: n=30, hits=19, rate=0.633, mean=1.9, median=2, min=0, max=6
  Split home/away:
    home: n=15, hits=8, rate=0.533, mean=1.8667, median=2
    away: n=15, hits=11, rate=0.733, mean=1.9333, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 18. total_goals @ 2.5 — Türkiye vs Italy (sin rol)
- basis: match_total_goals
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.6, median=2, min=1, max=2
-   last10: n=10, hits=6, rate=0.600, mean=3.1, median=2, min=1, max=9
-   all: n=30, hits=14, rate=0.467, mean=3.0667, median=3, min=0, max=9
- Split home/away:
-   home: n=15, hits=5, rate=0.333, mean=3.2667, median=3
-   away: n=15, hits=9, rate=0.600, mean=2.8667, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=2.4, median=2, min=1, max=5
    last10: n=10, hits=6, rate=0.600, mean=2.4, median=2, min=1, max=5
    all: n=30, hits=10, rate=0.333, mean=3.1, median=3, min=0, max=7
  Split home/away:
    home: n=15, hits=6, rate=0.400, mean=3, median=4
    away: n=15, hits=4, rate=0.267, mean=3.2, median=3
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 19. total_goals @ 3.5 — Türkiye vs Italy (sin rol)
- basis: match_total_goals
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.6, median=2, min=1, max=2
-   last10: n=10, hits=7, rate=0.700, mean=3.1, median=2, min=1, max=9
-   all: n=30, hits=20, rate=0.667, mean=3.0667, median=3, min=0, max=9
- Split home/away:
-   home: n=15, hits=9, rate=0.600, mean=3.2667, median=3
-   away: n=15, hits=11, rate=0.733, mean=2.8667, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=2.4, median=2, min=1, max=5
    last10: n=10, hits=7, rate=0.700, mean=2.4, median=2, min=1, max=5
    all: n=30, hits=17, rate=0.567, mean=3.1, median=3, min=0, max=7
  Split home/away:
    home: n=15, hits=7, rate=0.467, mean=3, median=4
    away: n=15, hits=10, rate=0.667, mean=3.2, median=3
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 20. 1x2_corners — Italy (away)
- basis: unmapped
- FM reportado: hits 7/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=true, source_conflict=false, motivo="non-numeric history value 'vt'; market '1x2_corners' has no mapping to fm_team_matches.team_stats_json"

### 21. 1x2_offsides — Italy (away)
- basis: unmapped
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=true, source_conflict=false, motivo="non-numeric history value 'vt'; market '1x2_offsides' has no mapping to fm_team_matches.team_stats_json"

### 22. 1x2_shots — Italy (away)
- basis: unmapped
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=true, source_conflict=false, motivo="non-numeric history value 'vt'; market '1x2_shots' has no mapping to fm_team_matches.team_stats_json"

### 23. btts — Italy (away)
- basis: unmapped
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=true, source_conflict=false, motivo="non-numeric history value 'vt'; market 'btts' has no mapping to fm_team_matches.team_stats_json"

### 24. total_cards @ 3.5 — Türkiye vs Italy (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 25. total_cards @ 4.5 — Türkiye vs Italy (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 26. total_cards @ 5.5 — Türkiye vs Italy (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 27. total_corners @ 5.5 — Türkiye vs Italy (sin rol)
- basis: not_reproducible
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 28. total_corners @ 6.5 — Türkiye vs Italy (sin rol)
- basis: not_reproducible
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 29. total_corners @ 7.5 — Türkiye vs Italy (sin rol)
- basis: not_reproducible
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 30. total_corners @ 8.5 — Türkiye vs Italy (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 31. total_offsides @ 3.5 — Türkiye vs Italy (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 32. total_shots_on_target @ 10.5 — Türkiye vs Italy (sin rol)
- basis: not_reproducible
- FM reportado: hits 7/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

## Señales de jugadores (orden: mayor muestra disponible, no fuerza de señal)

### 1. shots @ 0.5 — D. Frattesi
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=0.8, median=1, min=0, max=2
  last10: n=10, hits=5, rate=0.500, mean=1.2, median=0.5, min=0, max=7
  all: n=28, hits=14, rate=0.500, mean=1.1429, median=0.5, min=0, max=7
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 2. shots @ 0.5 — B. Alper Yılmaz
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.4, median=1, min=0, max=4
  last10: n=10, hits=6, rate=0.600, mean=1.2, median=1, min=0, max=4
  all: n=26, hits=13, rate=0.500, mean=0.8077, median=0.5, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 3. shots @ 0.5 — İ. Yüksek
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1.2, median=1, min=0, max=3
  last10: n=10, hits=6, rate=0.600, mean=1.1, median=1, min=0, max=3
  all: n=26, hits=11, rate=0.423, mean=0.7308, median=0, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 4. shots @ 0.5 — A. Bardakcı
- basis: player_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=1.2, median=1, min=1, max=2
  last10: n=10, hits=8, rate=0.800, mean=0.9, median=1, min=0, max=2
  all: n=24, hits=15, rate=0.625, mean=0.8333, median=1, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 5. shots @ 0.5 — S. Tonali
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=2.4, median=2, min=1, max=4
  last10: n=10, hits=8, rate=0.800, mean=2, median=2, min=0, max=4
  all: n=18, hits=13, rate=0.722, mean=1.5, median=1.5, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 6. shots @ 1.5 — S. Tonali
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.4, median=2, min=1, max=4
  last10: n=10, hits=7, rate=0.700, mean=2, median=2, min=0, max=4
  all: n=18, hits=9, rate=0.500, mean=1.5, median=1.5, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 7. shots @ 0.5 — R. Calafiori
- basis: player_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.8, median=2, min=0, max=4
  last10: n=10, hits=5, rate=0.500, mean=1, median=0.5, min=0, max=4
  all: n=16, hits=7, rate=0.438, mean=0.8125, median=0, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 8. foul_involvements @ 1.5 — A. Güler
- basis: player_own_stats
- FM reportado: hits 8/8; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=4.4, median=5, min=2, max=7
  last10: n=10, hits=9, rate=0.900, mean=3.8, median=3, min=1, max=7
  all: n=13, hits=11, rate=0.846, mean=3.3846, median=3, min=0, max=7
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 9. foul_involvements @ 2.5 — A. Güler
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=4.4, median=5, min=2, max=7
  last10: n=10, hits=7, rate=0.700, mean=3.8, median=3, min=1, max=7
  all: n=13, hits=8, rate=0.615, mean=3.3846, median=3, min=0, max=7
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 10. foul_involvements @ 3.5 — A. Güler
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=4.4, median=5, min=2, max=7
  last10: n=10, hits=4, rate=0.400, mean=3.8, median=3, min=1, max=7
  all: n=13, hits=5, rate=0.385, mean=3.3846, median=3, min=0, max=7
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 11. foul_involvements @ 4.5 — A. Güler
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=4.4, median=5, min=2, max=7
  last10: n=10, hits=4, rate=0.400, mean=3.8, median=3, min=1, max=7
  all: n=13, hits=4, rate=0.308, mean=3.3846, median=3, min=0, max=7
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 12. fouls_drawn @ 0.5 — A. Güler
- basis: player_own_stats
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=3.6, median=2, min=2, max=6
  last10: n=10, hits=10, rate=1.000, mean=3, median=2, min=1, max=6
  all: n=13, hits=12, rate=0.923, mean=2.6923, median=2, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 13. fouls_drawn @ 1.5 — A. Güler
- basis: player_own_stats
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=3.6, median=2, min=2, max=6
  last10: n=10, hits=8, rate=0.800, mean=3, median=2, min=1, max=6
  all: n=13, hits=10, rate=0.769, mean=2.6923, median=2, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 14. tackles @ 0.5 — A. Güler
- basis: player_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=1.2, median=1, min=1, max=2
  last10: n=10, hits=8, rate=0.800, mean=0.9, median=1, min=0, max=2
  all: n=13, hits=10, rate=0.769, mean=0.8462, median=1, min=0, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 15. shots_on_target @ 0.5 — M. Kean
- basis: player_own_stats
- FM reportado: hits 8/8; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=2, median=2, min=1, max=3
  last10: n=10, hits=9, rate=0.900, mean=1.6, median=1.5, min=0, max=3
  all: n=12, hits=10, rate=0.833, mean=1.4167, median=1, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 16. tackles @ 0.5 — A. Bardakcı
- basis: player_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=2.4, median=2, min=1, max=4
  last10: n=10, hits=8, rate=0.800, mean=1.7, median=1.5, min=0, max=4
  all: n=12, hits=9, rate=0.750, mean=1.5833, median=1.5, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 17. tackles @ 1.5 — A. Bardakcı
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.4, median=2, min=1, max=4
  last10: n=10, hits=5, rate=0.500, mean=1.7, median=1.5, min=0, max=4
  all: n=12, hits=6, rate=0.500, mean=1.5833, median=1.5, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 18. fouls_committed @ 0.5 — İ. Yüksek
- basis: player_own_stats
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=1.6, median=1, min=1, max=3
  last10: n=10, hits=10, rate=1.000, mean=1.9, median=2, min=1, max=4
  all: n=11, hits=11, rate=1.000, mean=1.9091, median=2, min=1, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 19. shots @ 1.5 — P. Esposito
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.8, median=3, min=1, max=4
  last10: n=10, hits=7, rate=0.700, mean=2.6, median=3, min=0, max=5
  all: n=11, hits=7, rate=0.636, mean=2.4545, median=3, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 20. shots @ 2.5 — P. Esposito
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.8, median=3, min=1, max=4
  last10: n=10, hits=6, rate=0.600, mean=2.6, median=3, min=0, max=5
  all: n=11, hits=6, rate=0.546, mean=2.4545, median=3, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 21. shots_on_target @ 0.5 — P. Esposito
- basis: player_own_stats
- FM reportado: hits 8/8; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=1.6, median=2, min=1, max=2
  last10: n=10, hits=9, rate=0.900, mean=1.5, median=2, min=0, max=2
  all: n=11, hits=9, rate=0.818, mean=1.3636, median=2, min=0, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 22. shots_on_target @ 1.5 — P. Esposito
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1.6, median=2, min=1, max=2
  last10: n=10, hits=6, rate=0.600, mean=1.5, median=2, min=0, max=2
  all: n=11, hits=6, rate=0.546, mean=1.3636, median=2, min=0, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 23. tackles @ 0.5 — N. Barella
- basis: player_own_stats
- FM reportado: hits 7/7; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=2.6, median=2, min=1, max=7
  last10: n=10, hits=10, rate=1.000, mean=2, median=1.5, min=1, max=7
  all: n=11, hits=11, rate=1.000, mean=2, median=2, min=1, max=7
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 24. tackles @ 1.5 — İ. Yüksek
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=3, median=3, min=0, max=5
  last10: n=10, hits=8, rate=0.800, mean=3.1, median=3, min=0, max=5
  all: n=11, hits=9, rate=0.818, mean=3.0909, median=3, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 25. tackles @ 2.5 — İ. Yüksek
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=3, median=3, min=0, max=5
  last10: n=10, hits=8, rate=0.800, mean=3.1, median=3, min=0, max=5
  all: n=11, hits=9, rate=0.818, mean=3.0909, median=3, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 26. fouls_committed @ 0.5 — A. Bastoni
- basis: player_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=1.6, median=1, min=1, max=3
  last10: n=10, hits=7, rate=0.700, mean=1.1, median=1, min=0, max=3
  all: n=10, hits=7, rate=0.700, mean=1.1, median=1, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 27. tackles @ 0.5 — Z. Çelik
- basis: player_own_stats
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=2.6, median=2, min=1, max=4
  last10: n=8, hits=8, rate=1.000, mean=2.25, median=2, min=1, max=4
  all: n=8, hits=8, rate=1.000, mean=2.25, median=2, min=1, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 28. tackles @ 0.5 — R. Calafiori
- basis: player_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=1.4, median=1, min=1, max=3
  last10: n=7, hits=6, rate=0.857, mean=1.1429, median=1, min=0, max=3
  all: n=7, hits=6, rate=0.857, mean=1.1429, median=1, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 29. foul_involvements @ 1.5 — B. Alper Yılmaz
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.8, median=3, min=0, max=5
  last10: n=6, hits=5, rate=0.833, mean=2.8333, median=3, min=0, max=5
  all: n=6, hits=5, rate=0.833, mean=2.8333, median=3, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 30. fouls_committed @ 0.5 — B. Alper Yılmaz
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.2, median=1, min=0, max=2
  last10: n=6, hits=5, rate=0.833, mean=1.3333, median=1.5, min=0, max=2
  all: n=6, hits=5, rate=0.833, mean=1.3333, median=1.5, min=0, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 31. fouls_committed @ 0.5 — G. Raspadori
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=1, median=1, min=1, max=1
  last10: n=6, hits=5, rate=0.833, mean=0.8333, median=1, min=0, max=1
  all: n=6, hits=5, rate=0.833, mean=0.8333, median=1, min=0, max=1
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 32. fouls_committed @ 1.5 — B. Alper Yılmaz
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=2, rate=0.400, mean=1.2, median=1, min=0, max=2
  last10: n=6, hits=3, rate=0.500, mean=1.3333, median=1.5, min=0, max=2
  all: n=6, hits=3, rate=0.500, mean=1.3333, median=1.5, min=0, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 33. foul_involvements @ 1.5 — P. Esposito
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=4, hits=4, rate=1.000, mean=3.5, median=3, min=2, max=6
  last10: n=4, hits=4, rate=1.000, mean=3.5, median=3, min=2, max=6
  all: n=4, hits=4, rate=1.000, mean=3.5, median=3, min=2, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 34. fouls_committed @ 0.5 — O. Kabak
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=4, hits=3, rate=0.750, mean=1.25, median=1, min=0, max=3
  last10: n=4, hits=3, rate=0.750, mean=1.25, median=1, min=0, max=3
  all: n=4, hits=3, rate=0.750, mean=1.25, median=1, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 35. fouls_drawn @ 0.5 — P. Esposito
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=4, hits=4, rate=1.000, mean=2.25, median=2, min=1, max=4
  last10: n=4, hits=4, rate=1.000, mean=2.25, median=2, min=1, max=4
  all: n=4, hits=4, rate=1.000, mean=2.25, median=2, min=1, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 36. fouls_drawn @ 0.5 — S. Özcan
- basis: player_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
  last5: n=4, hits=4, rate=1.000, mean=1.25, median=1, min=1, max=2
  last10: n=4, hits=4, rate=1.000, mean=1.25, median=1, min=1, max=2
  all: n=4, hits=4, rate=1.000, mean=1.25, median=1, min=1, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

## Evidencia histórica (ventanas fijas)
- observaciones descriptivas: secuencia cruda (más antiguo -> más reciente) y hits de las ventanas fijas (last5/last10/all)

### Jugadores — evidencia con ventanas fijas
- D. Frattesi — shots @ 0.5: [7,0,1,0,0,2,1,0,0,1] | last5 3/5 | last10 5/10 | all 14/28
- B. Alper Yılmaz — shots @ 0.5: [0,4,0,0,1,4,0,1,1,1] | last5 4/5 | last10 6/10 | all 13/26
- İ. Yüksek — shots @ 0.5: [1,2,2,0,0,0,3,2,0,1] | last5 3/5 | last10 6/10 | all 11/26
- A. Bardakcı — shots @ 0.5: [1,0,1,1,0,1,1,1,1,2] | last5 5/5 | last10 8/10 | all 15/24
- S. Tonali — shots @ 0.5: [0,2,3,3,0,3,2,4,1,2] | last5 5/5 | last10 8/10 | all 13/18
- S. Tonali — shots @ 1.5: [0,2,3,3,0,3,2,4,1,2] | last5 4/5 | last10 7/10 | all 9/18
- R. Calafiori — shots @ 0.5: [0,0,0,1,0,4,1,2,0,2] | last5 4/5 | last10 5/10 | all 7/16
- A. Güler — foul_involvements @ 1.5: [3,6,1,3,3,2,2,6,5,7] | last5 5/5 | last10 9/10 | all 11/13
- A. Güler — foul_involvements @ 2.5: [3,6,1,3,3,2,2,6,5,7] | last5 3/5 | last10 7/10 | all 8/13
- A. Güler — foul_involvements @ 3.5: [3,6,1,3,3,2,2,6,5,7] | last5 3/5 | last10 4/10 | all 5/13
- A. Güler — foul_involvements @ 4.5: [3,6,1,3,3,2,2,6,5,7] | last5 3/5 | last10 4/10 | all 4/13
- A. Güler — fouls_drawn @ 0.5: [3,5,1,2,1,2,2,6,2,6] | last5 5/5 | last10 10/10 | all 12/13
- A. Güler — fouls_drawn @ 1.5: [3,5,1,2,1,2,2,6,2,6] | last5 5/5 | last10 8/10 | all 10/13
- A. Güler — tackles @ 0.5: [0,1,1,1,0,1,1,1,2,1] | last5 5/5 | last10 8/10 | all 10/13
- M. Kean — shots_on_target @ 0.5: [0,1,1,2,2,3,1,3,1,2] | last5 5/5 | last10 9/10 | all 10/12
- A. Bardakcı — tackles @ 0.5: [0,3,1,1,0,1,2,2,4,3] | last5 5/5 | last10 8/10 | all 9/12
- A. Bardakcı — tackles @ 1.5: [0,3,1,1,0,1,2,2,4,3] | last5 4/5 | last10 5/10 | all 6/12
- İ. Yüksek — fouls_committed @ 0.5: [1,2,4,2,2,1,3,1,1,2] | last5 5/5 | last10 10/10 | all 11/11
- P. Esposito — shots @ 1.5: [0,2,1,5,4,1,3,4,3,3] | last5 4/5 | last10 7/10 | all 7/11
- P. Esposito — shots @ 2.5: [0,2,1,5,4,1,3,4,3,3] | last5 4/5 | last10 6/10 | all 6/11
- P. Esposito — shots_on_target @ 0.5: [0,2,1,2,2,1,1,2,2,2] | last5 5/5 | last10 9/10 | all 9/11
- P. Esposito — shots_on_target @ 1.5: [0,2,1,2,2,1,1,2,2,2] | last5 3/5 | last10 6/10 | all 6/11
- N. Barella — tackles @ 0.5: [1,2,1,1,2,7,1,1,2,2] | last5 5/5 | last10 10/10 | all 11/11
- İ. Yüksek — tackles @ 1.5: [1,4,3,3,5,5,0,3,4,3] | last5 4/5 | last10 8/10 | all 9/11
- İ. Yüksek — tackles @ 2.5: [1,4,3,3,5,5,0,3,4,3] | last5 4/5 | last10 8/10 | all 9/11
- A. Bastoni — fouls_committed @ 0.5: [0,0,1,0,2,1,1,1,3,2] | last5 5/5 | last10 7/10 | all 7/10
- Z. Çelik — tackles @ 0.5: [1,3,1,4,2,1,4,2] | last5 5/5 | last10 8/8 | all 8/8
- R. Calafiori — tackles @ 0.5: [0,1,1,1,3,1,1] | last5 5/5 | last10 6/7 | all 6/7
- B. Alper Yılmaz — foul_involvements @ 1.5: [3,2,5,0,3,4] | last5 4/5 | last10 5/6 | all 5/6
- B. Alper Yılmaz — fouls_committed @ 0.5: [2,1,1,0,2,2] | last5 4/5 | last10 5/6 | all 5/6

### Equipos — evidencia con ventanas fijas
- Italy — away_cards @ 3.5: [0,3,2,0,3,1,3,1,4,3] | last5 4/5 | last10 9/10 | all 27/30
  - Rival (Türkiye): [3,5,2,5,0,1,1,1,0,3] | last5 5/5 | last10 8/10 | all 24/30
- Italy — away_corners @ 2.5: [4,8,4,13,6,12,4,7,3,4] | last5 5/5 | last10 10/10 | all 25/30
  - Rival (Türkiye): [9,3,9,2,13,12,8,12,2,5] | last5 4/5 | last10 8/10 | all 27/30
- Italy — away_corners @ 3.5: [4,8,4,13,6,12,4,7,3,4] | last5 4/5 | last10 9/10 | all 23/30
  - Rival (Türkiye): [9,3,9,2,13,12,8,12,2,5] | last5 4/5 | last10 7/10 | all 23/30
- Italy — away_goals @ 0.5: [5,3,3,2,1,2,1,1,1,0] | last5 4/5 | last10 9/10 | all 25/30
  - Rival (Türkiye): [2,2,1,1,4,2,0,0,3,0] | last5 2/5 | last10 7/10 | all 22/30
- Türkiye — away_goals @ 0.5: [2,2,1,1,4,2,0,0,3,0] | last5 2/5 | last10 7/10 | all 22/30
  - Rival (Italy): [5,3,3,2,1,2,1,1,1,0] | last5 4/5 | last10 9/10 | all 25/30
- Italy — away_goals @ 2.5: [5,3,3,2,1,2,1,1,1,0] | last5 5/5 | last10 7/10 | all 22/30
  - Rival (Türkiye): [2,2,1,1,4,2,0,0,3,0] | last5 4/5 | last10 8/10 | all 19/30
- Türkiye — away_goals @ 2.5: [2,2,1,1,4,2,0,0,3,0] | last5 4/5 | last10 8/10 | all 19/30
  - Rival (Italy): [5,3,3,2,1,2,1,1,1,0] | last5 5/5 | last10 7/10 | all 22/30
- Italy — away_shots @ 10.5: [17,24,16,28,15,19,9,15,10,21] | last5 3/5 | last10 8/10 | all 22/30
  - Rival (Türkiye): [19,13,16,12,19,16,30,32,9,12] | last5 4/5 | last10 9/10 | all 22/30
- Italy — away_shots @ 12.5: [17,24,16,28,15,19,9,15,10,21] | last5 3/5 | last10 8/10 | all 16/30
  - Rival (Türkiye): [19,13,16,12,19,16,30,32,9,12] | last5 3/5 | last10 7/10 | all 18/30
- Italy — away_shots @ 13.5: [17,24,16,28,15,19,9,15,10,21] | last5 3/5 | last10 8/10 | all 14/30
  - Rival (Türkiye): [19,13,16,12,19,16,30,32,9,12] | last5 3/5 | last10 6/10 | all 17/30
- Italy — away_shots @ 14.5: [17,24,16,28,15,19,9,15,10,21] | last5 3/5 | last10 8/10 | all 14/30
  - Rival (Türkiye): [19,13,16,12,19,16,30,32,9,12] | last5 3/5 | last10 6/10 | all 17/30
- Türkiye — home_cards @ 1.5: [3,5,2,5,0,1,1,1,0,3] | last5 4/5 | last10 5/10 | all 13/30
  - Rival (Italy): [0,3,2,0,3,1,3,1,4,3] | last5 2/5 | last10 4/10 | all 13/30
- Türkiye — home_cards @ 2.5: [3,5,2,5,0,1,1,1,0,3] | last5 4/5 | last10 6/10 | all 17/30
  - Rival (Italy): [0,3,2,0,3,1,3,1,4,3] | last5 2/5 | last10 5/10 | all 21/30
- Türkiye — home_corners @ 2.5: [9,3,9,2,13,12,8,12,2,5] | last5 4/5 | last10 8/10 | all 27/30
  - Rival (Italy): [4,8,4,13,6,12,4,7,3,4] | last5 5/5 | last10 10/10 | all 25/30
- Türkiye — home_corners @ 3.5: [9,3,9,2,13,12,8,12,2,5] | last5 4/5 | last10 7/10 | all 23/30
  - Rival (Italy): [4,8,4,13,6,12,4,7,3,4] | last5 4/5 | last10 9/10 | all 23/30
- Türkiye — home_corners @ 4.5: [9,3,9,2,13,12,8,12,2,5] | last5 4/5 | last10 7/10 | all 21/30
  - Rival (Italy): [4,8,4,13,6,12,4,7,3,4] | last5 2/5 | last10 5/10 | all 17/30
- Italy — home_goals @ 2.5: [5,3,3,2,1,2,1,1,1,0] | last5 5/5 | last10 7/10 | all 22/30
  - Rival (Türkiye): [2,2,1,1,4,2,0,0,3,0] | last5 4/5 | last10 8/10 | all 19/30
- Türkiye vs Italy — total_goals @ 2.5: [9,4,3,2,5,2,2,1,1,2] | last5 5/5 | last10 6/10 | all 14/30
- Türkiye vs Italy — total_goals @ 3.5: [9,4,3,2,5,2,2,1,1,2] | last5 5/5 | last10 7/10 | all 20/30

## Confluencia descriptiva (equipo + rival + jugadores)
- familias de mercado con 2 o más series observadas en la DB; cada serie conserva su línea y sus ventanas fijas (last5/last10/all); sin score ni orden por fuerza

### cards
- Italy (away) @ 3.5: last5 4/5 | last10 9/10 | all 27/30
- Türkiye (home) @ 3.5: last5 5/5 | last10 8/10 | all 24/30
- Türkiye (home) @ 1.5: last5 4/5 | last10 5/10 | all 13/30
- Italy (away) @ 1.5: last5 2/5 | last10 4/10 | all 13/30
- Türkiye (home) @ 2.5: last5 4/5 | last10 6/10 | all 17/30
- Italy (away) @ 2.5: last5 2/5 | last10 5/10 | all 21/30

### corners
- Italy (away) @ 2.5: last5 5/5 | last10 10/10 | all 25/30
- Türkiye (home) @ 2.5: last5 4/5 | last10 8/10 | all 27/30
- Italy (away) @ 3.5: last5 4/5 | last10 9/10 | all 23/30
- Türkiye (home) @ 3.5: last5 4/5 | last10 7/10 | all 23/30
- Türkiye (home) @ 4.5: last5 4/5 | last10 7/10 | all 21/30
- Italy (away) @ 4.5: last5 2/5 | last10 5/10 | all 17/30

### foul_involvements
- jugador A. Güler — foul_involvements @ 1.5: last5 5/5 | last10 9/10 | all 11/13
- jugador A. Güler — foul_involvements @ 2.5: last5 3/5 | last10 7/10 | all 8/13
- jugador A. Güler — foul_involvements @ 3.5: last5 3/5 | last10 4/10 | all 5/13
- jugador A. Güler — foul_involvements @ 4.5: last5 3/5 | last10 4/10 | all 4/13
- jugador B. Alper Yılmaz — foul_involvements @ 1.5: last5 4/5 | last10 5/6 | all 5/6

### fouls_committed
- jugador İ. Yüksek — fouls_committed @ 0.5: last5 5/5 | last10 10/10 | all 11/11
- jugador A. Bastoni — fouls_committed @ 0.5: last5 5/5 | last10 7/10 | all 7/10
- jugador B. Alper Yılmaz — fouls_committed @ 0.5: last5 4/5 | last10 5/6 | all 5/6

### fouls_drawn
- jugador A. Güler — fouls_drawn @ 0.5: last5 5/5 | last10 10/10 | all 12/13
- jugador A. Güler — fouls_drawn @ 1.5: last5 5/5 | last10 8/10 | all 10/13

### goals
- Italy (away) @ 0.5: last5 4/5 | last10 9/10 | all 25/30
- Türkiye (home) @ 0.5: last5 2/5 | last10 7/10 | all 22/30
- Italy (away) @ 2.5: last5 5/5 | last10 7/10 | all 22/30
- Türkiye (home) @ 2.5: last5 4/5 | last10 8/10 | all 19/30

### shots
- Italy (away) @ 10.5: last5 3/5 | last10 8/10 | all 22/30
- Türkiye (home) @ 10.5: last5 4/5 | last10 9/10 | all 22/30
- Italy (away) @ 12.5: last5 3/5 | last10 8/10 | all 16/30
- Türkiye (home) @ 12.5: last5 3/5 | last10 7/10 | all 18/30
- Italy (away) @ 13.5: last5 3/5 | last10 8/10 | all 14/30
- Türkiye (home) @ 13.5: last5 3/5 | last10 6/10 | all 17/30
- Italy (away) @ 14.5: last5 3/5 | last10 8/10 | all 14/30
- Türkiye (home) @ 14.5: last5 3/5 | last10 6/10 | all 17/30
- jugador D. Frattesi — shots @ 0.5: last5 3/5 | last10 5/10 | all 14/28
- jugador B. Alper Yılmaz — shots @ 0.5: last5 4/5 | last10 6/10 | all 13/26
- jugador İ. Yüksek — shots @ 0.5: last5 3/5 | last10 6/10 | all 11/26
- jugador A. Bardakcı — shots @ 0.5: last5 5/5 | last10 8/10 | all 15/24
- jugador S. Tonali — shots @ 0.5: last5 5/5 | last10 8/10 | all 13/18
- jugador S. Tonali — shots @ 1.5: last5 4/5 | last10 7/10 | all 9/18
- jugador R. Calafiori — shots @ 0.5: last5 4/5 | last10 5/10 | all 7/16
- jugador P. Esposito — shots @ 1.5: last5 4/5 | last10 7/10 | all 7/11
- jugador P. Esposito — shots @ 2.5: last5 4/5 | last10 6/10 | all 6/11

### shots_on_target
- jugador M. Kean — shots_on_target @ 0.5: last5 5/5 | last10 9/10 | all 10/12
- jugador P. Esposito — shots_on_target @ 0.5: last5 5/5 | last10 9/10 | all 9/11
- jugador P. Esposito — shots_on_target @ 1.5: last5 3/5 | last10 6/10 | all 6/11

### tackles
- jugador A. Güler — tackles @ 0.5: last5 5/5 | last10 8/10 | all 10/13
- jugador A. Bardakcı — tackles @ 0.5: last5 5/5 | last10 8/10 | all 9/12
- jugador A. Bardakcı — tackles @ 1.5: last5 4/5 | last10 5/10 | all 6/12
- jugador N. Barella — tackles @ 0.5: last5 5/5 | last10 10/10 | all 11/11
- jugador İ. Yüksek — tackles @ 1.5: last5 4/5 | last10 8/10 | all 9/11
- jugador İ. Yüksek — tackles @ 2.5: last5 4/5 | last10 8/10 | all 9/11
- jugador Z. Çelik — tackles @ 0.5: last5 5/5 | last10 8/8 | all 8/8
- jugador R. Calafiori — tackles @ 0.5: last5 5/5 | last10 6/7 | all 6/7

## Cuotas

| mercado | línea | casa | lado | cuota | prob. implícita | capturada |
|---|---|---|---|---|---|---|
| away_corners | 2.5 | Kambi | over | 1.22 | 0.820 | 2026-09-28T02:39:39.679Z |
| away_corners | 2.5 | Kambi | under | 3.55 | 0.282 | 2026-09-28T02:39:39.679Z |
| away_corners | 3.5 | Kambi | over | 1.53 | 0.654 | 2026-09-28T02:39:39.679Z |
| away_corners | 3.5 | Kambi | under | 2.23 | 0.448 | 2026-09-28T02:39:39.679Z |
| away_corners | 3.5 | Ladbrokes | over | 1.55 | 0.645 | 2026-09-28T02:39:39.679Z |
| away_corners | 3.5 | Ladbrokes | under | 2.3 | 0.435 | 2026-09-28T02:39:39.679Z |
| away_goals | 0.5 | Bet365 | over | 1.22 | 0.820 | 2026-09-28T02:39:39.679Z |
| away_goals | 0.5 | Bet365 | under | 4 | 0.250 | 2026-09-28T02:39:39.679Z |
| away_goals | 0.5 | Kambi | over | 1.23 | 0.813 | 2026-09-28T02:39:39.679Z |
| away_goals | 0.5 | Kambi | under | 3.6 | 0.278 | 2026-09-28T02:39:39.679Z |
| away_goals | 0.5 | Ladbrokes | over | 1.22 | 0.820 | 2026-09-28T02:39:39.679Z |
| away_goals | 0.5 | Ladbrokes | under | 3.75 | 0.267 | 2026-09-28T02:39:39.679Z |
| away_goals | 0.5 | Bet365 | over | 1.22 | 0.820 | 2026-09-28T02:39:39.679Z |
| away_goals | 0.5 | Bet365 | under | 4 | 0.250 | 2026-09-28T02:39:39.679Z |
| away_goals | 0.5 | Kambi | over | 1.23 | 0.813 | 2026-09-28T02:39:39.679Z |
| away_goals | 0.5 | Kambi | under | 3.6 | 0.278 | 2026-09-28T02:39:39.679Z |
| away_goals | 0.5 | Ladbrokes | over | 1.22 | 0.820 | 2026-09-28T02:39:39.679Z |
| away_goals | 0.5 | Ladbrokes | under | 3.75 | 0.267 | 2026-09-28T02:39:39.679Z |
| away_goals | 2.5 | Bet365 | over | 5 | 0.200 | 2026-09-28T02:39:39.679Z |
| away_goals | 2.5 | Bet365 | under | 1.17 | 0.855 | 2026-09-28T02:39:39.679Z |
| away_goals | 2.5 | Kambi | over | 4.8 | 0.208 | 2026-09-28T02:39:39.679Z |
| away_goals | 2.5 | Kambi | under | 1.13 | 0.885 | 2026-09-28T02:39:39.679Z |
| away_goals | 2.5 | Ladbrokes | over | 4.8 | 0.208 | 2026-09-28T02:39:39.679Z |
| away_goals | 2.5 | Ladbrokes | under | 1.15 | 0.870 | 2026-09-28T02:39:39.679Z |
| away_goals | 2.5 | Bet365 | over | 5 | 0.200 | 2026-09-28T02:39:39.679Z |
| away_goals | 2.5 | Bet365 | under | 1.17 | 0.855 | 2026-09-28T02:39:39.679Z |
| away_goals | 2.5 | Kambi | over | 4.8 | 0.208 | 2026-09-28T02:39:39.679Z |
| away_goals | 2.5 | Kambi | under | 1.13 | 0.885 | 2026-09-28T02:39:39.679Z |
| away_goals | 2.5 | Ladbrokes | over | 4.8 | 0.208 | 2026-09-28T02:39:39.679Z |
| away_goals | 2.5 | Ladbrokes | under | 1.15 | 0.870 | 2026-09-28T02:39:39.679Z |
| away_shots | 12.5 | Bet365 | over | 1.83 | 0.546 | 2026-09-28T02:39:39.679Z |
| away_shots | 12.5 | Bet365 | under | 1.83 | 0.546 | 2026-09-28T02:39:39.679Z |
| away_shots | 12.5 | Kambi | over | 1.82 | 0.549 | 2026-09-28T02:39:39.679Z |
| away_shots | 12.5 | Kambi | under | 1.71 | 0.585 | 2026-09-28T02:39:39.679Z |
| away_shots | 13.5 | Kambi | over | 2.16 | 0.463 | 2026-09-28T02:39:39.679Z |
| away_shots | 13.5 | Kambi | under | 1.49 | 0.671 | 2026-09-28T02:39:39.679Z |
| home_cards | 1.5 | Ladbrokes | over | 1.4 | 0.714 | 2026-09-28T02:39:39.679Z |
| home_cards | 1.5 | Ladbrokes | under | 2.75 | 0.364 | 2026-09-28T02:39:39.679Z |
| home_cards | 2.5 | Bet365 | over | 2.1 | 0.476 | 2026-09-28T02:39:39.679Z |
| home_cards | 2.5 | Bet365 | under | 1.67 | 0.599 | 2026-09-28T02:39:39.679Z |
| home_cards | 2.5 | Ladbrokes | over | 2.37 | 0.422 | 2026-09-28T02:39:39.679Z |
| home_cards | 2.5 | Ladbrokes | under | 1.53 | 0.654 | 2026-09-28T02:39:39.679Z |
| home_corners | 2.5 | Kambi | over | 1.15 | 0.870 | 2026-09-28T02:39:39.679Z |
| home_corners | 2.5 | Kambi | under | 4.35 | 0.230 | 2026-09-28T02:39:39.679Z |
| home_corners | 3.5 | Kambi | over | 1.37 | 0.730 | 2026-09-28T02:39:39.679Z |
| home_corners | 3.5 | Kambi | under | 2.7 | 0.370 | 2026-09-28T02:39:39.679Z |
| home_corners | 3.5 | Ladbrokes | over | 1.44 | 0.694 | 2026-09-28T02:39:39.679Z |
| home_corners | 3.5 | Ladbrokes | under | 2.62 | 0.382 | 2026-09-28T02:39:39.679Z |
| home_corners | 4.5 | Bet365 | over | 1.83 | 0.546 | 2026-09-28T02:39:39.679Z |
| home_corners | 4.5 | Bet365 | under | 1.83 | 0.546 | 2026-09-28T02:39:39.679Z |
| home_corners | 4.5 | Kambi | over | 1.75 | 0.571 | 2026-09-28T02:39:39.679Z |
| home_corners | 4.5 | Kambi | under | 1.89 | 0.529 | 2026-09-28T02:39:39.679Z |
| home_corners | 4.5 | Ladbrokes | over | 1.85 | 0.541 | 2026-09-28T02:39:39.679Z |
| home_corners | 4.5 | Ladbrokes | under | 1.83 | 0.546 | 2026-09-28T02:39:39.679Z |
| home_goals | 2.5 | Bet365 | over | 5 | 0.200 | 2026-09-28T02:39:39.679Z |
| home_goals | 2.5 | Bet365 | under | 1.17 | 0.855 | 2026-09-28T02:39:39.679Z |
| home_goals | 2.5 | Kambi | over | 4.6 | 0.217 | 2026-09-28T02:39:39.679Z |
| home_goals | 2.5 | Kambi | under | 1.16 | 0.862 | 2026-09-28T02:39:39.679Z |
| home_goals | 2.5 | Ladbrokes | over | 4.8 | 0.208 | 2026-09-28T02:39:39.679Z |
| home_goals | 2.5 | Ladbrokes | under | 1.15 | 0.870 | 2026-09-28T02:39:39.679Z |
| total_goals | 2.5 | Altenar | over | 1.67 | 0.599 | 2026-09-28T02:39:39.679Z |
| total_goals | 2.5 | Altenar | under | 2.05 | 0.488 | 2026-09-28T02:39:39.679Z |
| total_goals | 2.5 | Bet365 | over | 1.7 | 0.588 | 2026-09-28T02:39:39.679Z |
| total_goals | 2.5 | Bet365 | under | 2.1 | 0.476 | 2026-09-28T02:39:39.679Z |
| total_goals | 2.5 | Kambi | over | 1.73 | 0.578 | 2026-09-28T02:39:39.679Z |
| total_goals | 2.5 | Kambi | under | 2.06 | 0.485 | 2026-09-28T02:39:39.679Z |
| total_goals | 2.5 | Ladbrokes | over | 1.7 | 0.588 | 2026-09-28T02:39:39.679Z |
| total_goals | 2.5 | Ladbrokes | under | 2.05 | 0.488 | 2026-09-28T02:39:39.679Z |
| total_goals | 2.5 | Paddy Power | over | 1.65 | 0.606 | 2026-09-28T02:39:39.679Z |
| total_goals | 2.5 | Paddy Power | under | 2.1 | 0.476 | 2026-09-28T02:39:39.679Z |
| total_goals | 3.5 | Bet365 | over | 2.63 | 0.380 | 2026-09-28T02:39:39.679Z |
| total_goals | 3.5 | Bet365 | under | 1.44 | 0.694 | 2026-09-28T02:39:39.679Z |
| total_goals | 3.5 | Kambi | over | 2.85 | 0.351 | 2026-09-28T02:39:39.679Z |
| total_goals | 3.5 | Kambi | under | 1.4 | 0.714 | 2026-09-28T02:39:39.679Z |
| total_goals | 3.5 | Ladbrokes | over | 2.75 | 0.364 | 2026-09-28T02:39:39.679Z |
| total_goals | 3.5 | Ladbrokes | under | 1.4 | 0.714 | 2026-09-28T02:39:39.679Z |
| total_goals | 3.5 | Paddy Power | over | 2.88 | 0.347 | 2026-09-28T02:39:39.679Z |
| total_goals | 3.5 | Paddy Power | under | 1.36 | 0.735 | 2026-09-28T02:39:39.679Z |
| total_cards | 3.5 | Kambi | over | 1.41 | 0.709 | 2026-09-28T02:39:39.679Z |
| total_cards | 3.5 | Kambi | under | 2.63 | 0.380 | 2026-09-28T02:39:39.679Z |
| total_cards | 3.5 | Ladbrokes | over | 1.44 | 0.694 | 2026-09-28T02:39:39.679Z |
| total_cards | 3.5 | Ladbrokes | under | 2.62 | 0.382 | 2026-09-28T02:39:39.679Z |
| total_cards | 3.5 | Paddy Power | over | 1.5 | 0.667 | 2026-09-28T02:39:39.679Z |
| total_cards | 3.5 | Paddy Power | under | 2.38 | 0.420 | 2026-09-28T02:39:39.679Z |
| total_cards | 4.5 | Bet365 | over | 1.83 | 0.546 | 2026-09-28T02:39:39.679Z |
| total_cards | 4.5 | Bet365 | under | 1.83 | 0.546 | 2026-09-28T02:39:39.679Z |
| total_cards | 4.5 | Kambi | over | 1.85 | 0.541 | 2026-09-28T02:39:39.679Z |
| total_cards | 4.5 | Kambi | under | 1.82 | 0.549 | 2026-09-28T02:39:39.679Z |
| total_cards | 4.5 | Ladbrokes | over | 1.95 | 0.513 | 2026-09-28T02:39:39.679Z |
| total_cards | 4.5 | Ladbrokes | under | 1.75 | 0.571 | 2026-09-28T02:39:39.679Z |
| total_cards | 4.5 | Paddy Power | over | 2.05 | 0.488 | 2026-09-28T02:39:39.679Z |
| total_cards | 4.5 | Paddy Power | under | 1.67 | 0.599 | 2026-09-28T02:39:39.679Z |
| total_cards | 5.5 | Kambi | over | 2.55 | 0.392 | 2026-09-28T02:39:39.679Z |
| total_cards | 5.5 | Kambi | under | 1.43 | 0.699 | 2026-09-28T02:39:39.679Z |
| total_cards | 5.5 | Ladbrokes | over | 3 | 0.333 | 2026-09-28T02:39:39.679Z |
| total_cards | 5.5 | Ladbrokes | under | 1.35 | 0.741 | 2026-09-28T02:39:39.679Z |
| total_corners | 5.5 | Bet365 | over | 1.1 | 0.909 | 2026-09-28T02:39:39.679Z |
| total_corners | 5.5 | Bet365 | under | 7 | 0.143 | 2026-09-28T02:39:39.679Z |
| total_corners | 6.5 | Bet365 | over | 1.2 | 0.833 | 2026-09-28T02:39:39.679Z |
| total_corners | 6.5 | Kambi | over | 1.19 | 0.840 | 2026-09-28T02:39:39.679Z |
| total_corners | 6.5 | Kambi | under | 4.1 | 0.244 | 2026-09-28T02:39:39.679Z |
| total_corners | 6.5 | Ladbrokes | over | 1.18 | 0.847 | 2026-09-28T02:39:39.679Z |
| total_corners | 6.5 | Ladbrokes | under | 4.5 | 0.222 | 2026-09-28T02:39:39.679Z |
| total_corners | 7.5 | Kambi | over | 1.34 | 0.746 | 2026-09-28T02:39:39.679Z |
| total_corners | 7.5 | Kambi | under | 2.9 | 0.345 | 2026-09-28T02:39:39.679Z |
| total_corners | 7.5 | Ladbrokes | over | 1.36 | 0.735 | 2026-09-28T02:39:39.679Z |
| total_corners | 7.5 | Ladbrokes | under | 2.9 | 0.345 | 2026-09-28T02:39:39.679Z |
| total_corners | 8.5 | Bet365 | over | 1.67 | 0.599 | 2026-09-28T02:39:39.679Z |
| total_corners | 8.5 | Bet365 | under | 2.1 | 0.476 | 2026-09-28T02:39:39.679Z |
| total_corners | 8.5 | Kambi | over | 1.58 | 0.633 | 2026-09-28T02:39:39.679Z |
| total_corners | 8.5 | Kambi | under | 2.2 | 0.455 | 2026-09-28T02:39:39.679Z |
| total_corners | 8.5 | Ladbrokes | over | 1.65 | 0.606 | 2026-09-28T02:39:39.679Z |
| total_corners | 8.5 | Ladbrokes | under | 2.1 | 0.476 | 2026-09-28T02:39:39.679Z |
| total_corners | 8.5 | Paddy Power | over | 1.75 | 0.571 | 2026-09-28T02:39:39.679Z |
| total_corners | 8.5 | Paddy Power | under | 1.95 | 0.513 | 2026-09-28T02:39:39.679Z |
| total_offsides | 3.5 | Kambi | over | 1.87 | 0.535 | 2026-09-28T02:39:39.679Z |
| total_shots_on_target | 10.5 | Kambi | over | 2.85 | 0.351 | 2026-09-28T02:39:39.679Z |
| total_shots_on_target | 10.5 | Kambi | under | 1.32 | 0.758 | 2026-09-28T02:39:39.679Z |
- Cuota manual Betano (away_cards @ 3.5): no cargada
- Cuota manual Betano (away_corners @ 2.5): no cargada
- Cuota manual Betano (away_corners @ 3.5): no cargada
- Cuota manual Betano (away_goals @ 0.5): no cargada
- Cuota manual Betano (away_goals @ 0.5): no cargada
- Cuota manual Betano (away_goals @ 2.5): no cargada
- Cuota manual Betano (away_goals @ 2.5): no cargada
- Cuota manual Betano (away_shots @ 10.5): no cargada
- Cuota manual Betano (away_shots @ 12.5): no cargada
- Cuota manual Betano (away_shots @ 13.5): no cargada
- Cuota manual Betano (away_shots @ 14.5): no cargada
- Cuota manual Betano (home_cards @ 1.5): no cargada
- Cuota manual Betano (home_cards @ 2.5): no cargada
- Cuota manual Betano (home_corners @ 2.5): no cargada
- Cuota manual Betano (home_corners @ 3.5): no cargada
- Cuota manual Betano (home_corners @ 4.5): no cargada
- Cuota manual Betano (home_goals @ 2.5): no cargada
- Cuota manual Betano (total_goals @ 2.5): no cargada
- Cuota manual Betano (total_goals @ 3.5): no cargada
- Cuota manual Betano (1x2_corners): no cargada
- Cuota manual Betano (1x2_offsides): no cargada
- Cuota manual Betano (1x2_shots): no cargada
- Cuota manual Betano (btts): no cargada
- Cuota manual Betano (total_cards @ 3.5): no cargada
- Cuota manual Betano (total_cards @ 4.5): no cargada
- Cuota manual Betano (total_cards @ 5.5): no cargada
- Cuota manual Betano (total_corners @ 5.5): no cargada
- Cuota manual Betano (total_corners @ 6.5): no cargada
- Cuota manual Betano (total_corners @ 7.5): no cargada
- Cuota manual Betano (total_corners @ 8.5): no cargada
- Cuota manual Betano (total_offsides @ 3.5): no cargada
- Cuota manual Betano (total_shots_on_target @ 10.5): no cargada

## Nota de cierre
Este reporte es descriptivo. No constituye una recomendación de apuesta ni una probabilidad validada de resultado futuro.

---

## Sweden vs Poland (33662313) - kickoff 2026-09-28T18:45:00.000Z

# Sweden vs Poland
- Competición: UEFA Nations League
- Kickoff (UTC): desconocido
- Flags de calidad: leakage=false; suspect=true
- sort_criteria: sample_size_desc

## Mercados (orden: mayor muestra disponible, no fuerza de señal)

### 1. away_cards @ 1.5 — Poland (away)
- basis: team_own_stats
- FM reportado: hits 7/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=2, median=2, min=0, max=3
-   last10: n=10, hits=7, rate=0.700, mean=1.8, median=2, min=0, max=3
-   all: n=30, hits=18, rate=0.600, mean=1.9333, median=2, min=0, max=7
- Split home/away:
-   home: n=15, hits=8, rate=0.533, mean=1.6667, median=2
-   away: n=15, hits=10, rate=0.667, mean=2.2, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=2, rate=0.400, mean=1, median=0, min=0, max=3
    last10: n=10, hits=4, rate=0.400, mean=1.1, median=1, min=0, max=3
    all: n=30, hits=14, rate=0.467, mean=1.5, median=1, min=0, max=4
  Split home/away:
    home: n=15, hits=4, rate=0.267, mean=1, median=1
    away: n=15, hits=10, rate=0.667, mean=2, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 2. away_cards @ 3.5 — Poland (away)
- basis: team_own_stats
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=2, median=2, min=0, max=3
-   last10: n=10, hits=10, rate=1.000, mean=1.8, median=2, min=0, max=3
-   all: n=30, hits=26, rate=0.867, mean=1.9333, median=2, min=0, max=7
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=1.6667, median=2
-   away: n=15, hits=13, rate=0.867, mean=2.2, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=1, median=0, min=0, max=3
    last10: n=10, hits=10, rate=1.000, mean=1.1, median=1, min=0, max=3
    all: n=30, hits=28, rate=0.933, mean=1.5, median=1, min=0, max=4
  Split home/away:
    home: n=15, hits=15, rate=1.000, mean=1, median=1
    away: n=15, hits=13, rate=0.867, mean=2, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 3. away_corners @ 2.5 — Poland (away)
- basis: team_own_stats
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=5.8, median=6, min=3, max=9
-   last10: n=10, hits=9, rate=0.900, mean=6.3, median=6.5, min=2, max=9
-   all: n=30, hits=26, rate=0.867, mean=5.2, median=4.5, min=2, max=11
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=5.3333, median=5
-   away: n=15, hits=13, rate=0.867, mean=5.0667, median=4
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=4.2, median=4, min=1, max=8
    last10: n=10, hits=6, rate=0.600, mean=3.4, median=3, min=1, max=8
    all: n=30, hits=25, rate=0.833, mean=5.5667, median=5, min=1, max=13
  Split home/away:
    home: n=15, hits=12, rate=0.800, mean=5.8667, median=5
    away: n=15, hits=13, rate=0.867, mean=5.2667, median=5
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 4. away_goals @ 0.5 — Sweden (home)
- basis: team_own_stats
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=1.8, median=1, min=0, max=5
-   last10: n=10, hits=9, rate=0.900, mean=1.9, median=1.5, min=0, max=5
-   all: n=30, hits=24, rate=0.800, mean=1.9333, median=2, min=0, max=6
- Split home/away:
-   home: n=15, hits=12, rate=0.800, mean=2.4, median=2
-   away: n=15, hits=12, rate=0.800, mean=1.4667, median=1
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=1.2, median=2, min=0, max=2
    last10: n=10, hits=8, rate=0.800, mean=1.6, median=2, min=0, max=3
    all: n=30, hits=25, rate=0.833, mean=1.3667, median=1, min=0, max=3
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=1.4, median=1
    away: n=15, hits=12, rate=0.800, mean=1.3333, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 5. away_goals @ 2.5 — Poland (away)
- basis: team_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.2, median=2, min=0, max=2
-   last10: n=10, hits=8, rate=0.800, mean=1.6, median=2, min=0, max=3
-   all: n=30, hits=26, rate=0.867, mean=1.3667, median=1, min=0, max=3
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=1.4, median=1
-   away: n=15, hits=13, rate=0.867, mean=1.3333, median=1
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=1.8, median=1, min=0, max=5
    last10: n=10, hits=7, rate=0.700, mean=1.9, median=1.5, min=0, max=5
    all: n=30, hits=21, rate=0.700, mean=1.9333, median=2, min=0, max=6
  Split home/away:
    home: n=15, hits=9, rate=0.600, mean=2.4, median=2
    away: n=15, hits=12, rate=0.800, mean=1.4667, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 6. away_shots @ 10.5 — Poland (away)
- basis: team_own_stats
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=14.6, median=15, min=12, max=17
-   last10: n=10, hits=10, rate=1.000, mean=14.7, median=15, min=12, max=18
-   all: n=30, hits=26, rate=0.867, mean=14.6333, median=13.5, min=6, max=28
- Split home/away:
-   home: n=15, hits=15, rate=1.000, mean=16.5333, median=15
-   away: n=15, hits=11, rate=0.733, mean=12.7333, median=12
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=11.8, median=11, min=8, max=16
    last10: n=10, hits=6, rate=0.600, mean=11.1, median=11, min=8, max=16
    all: n=30, hits=21, rate=0.700, mean=14.2333, median=13, min=6, max=34
  Split home/away:
    home: n=15, hits=12, rate=0.800, mean=15.2667, median=13
    away: n=15, hits=9, rate=0.600, mean=13.2, median=13
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 7. away_shots @ 11.5 — Poland (away)
- basis: team_own_stats
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=14.6, median=15, min=12, max=17
-   last10: n=10, hits=10, rate=1.000, mean=14.7, median=15, min=12, max=18
-   all: n=30, hits=23, rate=0.767, mean=14.6333, median=13.5, min=6, max=28
- Split home/away:
-   home: n=15, hits=15, rate=1.000, mean=16.5333, median=15
-   away: n=15, hits=8, rate=0.533, mean=12.7333, median=12
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=2, rate=0.400, mean=11.8, median=11, min=8, max=16
    last10: n=10, hits=3, rate=0.300, mean=11.1, median=11, min=8, max=16
    all: n=30, hits=17, rate=0.567, mean=14.2333, median=13, min=6, max=34
  Split home/away:
    home: n=15, hits=9, rate=0.600, mean=15.2667, median=13
    away: n=15, hits=8, rate=0.533, mean=13.2, median=13
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 8. away_shots_on_target @ 3.5 — Poland (away)
- basis: team_own_stats
- FM reportado: hits 7/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=5, median=4, min=2, max=8
-   last10: n=10, hits=8, rate=0.800, mean=4.9, median=4.5, min=2, max=8
-   all: n=30, hits=20, rate=0.667, mean=4.7, median=4, min=0, max=12
- Split home/away:
-   home: n=15, hits=12, rate=0.800, mean=5.4667, median=5
-   away: n=15, hits=8, rate=0.533, mean=3.9333, median=4
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=5.4, median=5, min=3, max=8
    last10: n=10, hits=8, rate=0.800, mean=4.8, median=5, min=2, max=8
    all: n=30, hits=23, rate=0.767, mean=5.8333, median=5, min=0, max=14
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=6.3333, median=6
    away: n=15, hits=10, rate=0.667, mean=5.3333, median=5
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 9. home_cards @ 2.5 — Sweden (home)
- basis: team_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=1, median=0, min=0, max=3
-   last10: n=10, hits=9, rate=0.900, mean=1.1, median=1, min=0, max=3
-   all: n=30, hits=24, rate=0.800, mean=1.5, median=1, min=0, max=4
- Split home/away:
-   home: n=15, hits=14, rate=0.933, mean=1, median=1
-   away: n=15, hits=10, rate=0.667, mean=2, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=2, median=2, min=0, max=3
    last10: n=10, hits=8, rate=0.800, mean=1.8, median=2, min=0, max=3
    all: n=30, hits=21, rate=0.700, mean=1.9333, median=2, min=0, max=7
  Split home/away:
    home: n=15, hits=12, rate=0.800, mean=1.6667, median=2
    away: n=15, hits=9, rate=0.600, mean=2.2, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 10. home_corners @ 5.5 — Sweden (home)
- basis: team_own_stats
- FM reportado: hits 8/9; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=4.2, median=4, min=1, max=8
-   last10: n=10, hits=8, rate=0.800, mean=3.4, median=3, min=1, max=8
-   all: n=30, hits=17, rate=0.567, mean=5.5667, median=5, min=1, max=13
- Split home/away:
-   home: n=15, hits=9, rate=0.600, mean=5.8667, median=5
-   away: n=15, hits=8, rate=0.533, mean=5.2667, median=5
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=2, rate=0.400, mean=5.8, median=6, min=3, max=9
    last10: n=10, hits=3, rate=0.300, mean=6.3, median=6.5, min=2, max=9
    all: n=30, hits=17, rate=0.567, mean=5.2, median=4.5, min=2, max=11
  Split home/away:
    home: n=15, hits=8, rate=0.533, mean=5.3333, median=5
    away: n=15, hits=9, rate=0.600, mean=5.0667, median=4
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 11. home_corners @ 6.5 — Sweden (home)
- basis: team_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=4.2, median=4, min=1, max=8
-   last10: n=10, hits=9, rate=0.900, mean=3.4, median=3, min=1, max=8
-   all: n=30, hits=20, rate=0.667, mean=5.5667, median=5, min=1, max=13
- Split home/away:
-   home: n=15, hits=11, rate=0.733, mean=5.8667, median=5
-   away: n=15, hits=9, rate=0.600, mean=5.2667, median=5
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=5.8, median=6, min=3, max=9
    last10: n=10, hits=5, rate=0.500, mean=6.3, median=6.5, min=2, max=9
    all: n=30, hits=21, rate=0.700, mean=5.2, median=4.5, min=2, max=11
  Split home/away:
    home: n=15, hits=10, rate=0.667, mean=5.3333, median=5
    away: n=15, hits=11, rate=0.733, mean=5.0667, median=4
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 12. home_corners @ 7.5 — Sweden (home)
- basis: team_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=4.2, median=4, min=1, max=8
-   last10: n=10, hits=9, rate=0.900, mean=3.4, median=3, min=1, max=8
-   all: n=30, hits=21, rate=0.700, mean=5.5667, median=5, min=1, max=13
- Split home/away:
-   home: n=15, hits=11, rate=0.733, mean=5.8667, median=5
-   away: n=15, hits=10, rate=0.667, mean=5.2667, median=5
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=5.8, median=6, min=3, max=9
    last10: n=10, hits=6, rate=0.600, mean=6.3, median=6.5, min=2, max=9
    all: n=30, hits=24, rate=0.800, mean=5.2, median=4.5, min=2, max=11
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=5.3333, median=5
    away: n=15, hits=11, rate=0.733, mean=5.0667, median=4
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 13. home_goals @ 0.5 — Sweden (home)
- basis: team_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=1.8, median=1, min=0, max=5
-   last10: n=10, hits=9, rate=0.900, mean=1.9, median=1.5, min=0, max=5
-   all: n=30, hits=24, rate=0.800, mean=1.9333, median=2, min=0, max=6
- Split home/away:
-   home: n=15, hits=12, rate=0.800, mean=2.4, median=2
-   away: n=15, hits=12, rate=0.800, mean=1.4667, median=1
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=1.2, median=2, min=0, max=2
    last10: n=10, hits=8, rate=0.800, mean=1.6, median=2, min=0, max=3
    all: n=30, hits=25, rate=0.833, mean=1.3667, median=1, min=0, max=3
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=1.4, median=1
    away: n=15, hits=12, rate=0.800, mean=1.3333, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 14. home_goals @ 2.5 — Poland (away)
- basis: team_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.2, median=2, min=0, max=2
-   last10: n=10, hits=8, rate=0.800, mean=1.6, median=2, min=0, max=3
-   all: n=30, hits=26, rate=0.867, mean=1.3667, median=1, min=0, max=3
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=1.4, median=1
-   away: n=15, hits=13, rate=0.867, mean=1.3333, median=1
- Rival (contexto equivalente):
  FM reportado: hits 4/4; mejor ventana: no determinable
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=1.8, median=1, min=0, max=5
    last10: n=10, hits=7, rate=0.700, mean=1.9, median=1.5, min=0, max=5
    all: n=30, hits=21, rate=0.700, mean=1.9333, median=2, min=0, max=6
  Split home/away:
    home: n=15, hits=9, rate=0.600, mean=2.4, median=2
    away: n=15, hits=12, rate=0.800, mean=1.4667, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 15. home_goals @ 2.5 — Sweden (home)
- basis: team_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=1.8, median=1, min=0, max=5
-   last10: n=10, hits=7, rate=0.700, mean=1.9, median=1.5, min=0, max=5
-   all: n=30, hits=21, rate=0.700, mean=1.9333, median=2, min=0, max=6
- Split home/away:
-   home: n=15, hits=9, rate=0.600, mean=2.4, median=2
-   away: n=15, hits=12, rate=0.800, mean=1.4667, median=1
- Rival (contexto equivalente):
  FM reportado: hits 9/10; mejor ventana: 10 (9/10)
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=1.2, median=2, min=0, max=2
    last10: n=10, hits=8, rate=0.800, mean=1.6, median=2, min=0, max=3
    all: n=30, hits=26, rate=0.867, mean=1.3667, median=1, min=0, max=3
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=1.4, median=1
    away: n=15, hits=13, rate=0.867, mean=1.3333, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 16. home_shots @ 13.5 — Sweden (home)
- basis: team_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=11.8, median=11, min=8, max=16
-   last10: n=10, hits=9, rate=0.900, mean=11.1, median=11, min=8, max=16
-   all: n=30, hits=19, rate=0.633, mean=14.2333, median=13, min=6, max=34
- Split home/away:
-   home: n=15, hits=10, rate=0.667, mean=15.2667, median=13
-   away: n=15, hits=9, rate=0.600, mean=13.2, median=13
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=1, rate=0.200, mean=14.6, median=15, min=12, max=17
    last10: n=10, hits=3, rate=0.300, mean=14.7, median=15, min=12, max=18
    all: n=30, hits=15, rate=0.500, mean=14.6333, median=13.5, min=6, max=28
  Split home/away:
    home: n=15, hits=4, rate=0.267, mean=16.5333, median=15
    away: n=15, hits=11, rate=0.733, mean=12.7333, median=12
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 17. home_shots @ 15.5 — Sweden (home)
- basis: team_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=11.8, median=11, min=8, max=16
-   last10: n=10, hits=9, rate=0.900, mean=11.1, median=11, min=8, max=16
-   all: n=30, hits=21, rate=0.700, mean=14.2333, median=13, min=6, max=34
- Split home/away:
-   home: n=15, hits=10, rate=0.667, mean=15.2667, median=13
-   away: n=15, hits=11, rate=0.733, mean=13.2, median=13
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=14.6, median=15, min=12, max=17
    last10: n=10, hits=7, rate=0.700, mean=14.7, median=15, min=12, max=18
    all: n=30, hits=21, rate=0.700, mean=14.6333, median=13.5, min=6, max=28
  Split home/away:
    home: n=15, hits=9, rate=0.600, mean=16.5333, median=15
    away: n=15, hits=12, rate=0.800, mean=12.7333, median=12
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 18. home_shots_on_target @ 3.5 — Sweden (home)
- basis: team_own_stats
- FM reportado: hits 8/9; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=5.4, median=5, min=3, max=8
-   last10: n=10, hits=8, rate=0.800, mean=4.8, median=5, min=2, max=8
-   all: n=30, hits=23, rate=0.767, mean=5.8333, median=5, min=0, max=14
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=6.3333, median=6
-   away: n=15, hits=10, rate=0.667, mean=5.3333, median=5
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=5, median=4, min=2, max=8
    last10: n=10, hits=8, rate=0.800, mean=4.9, median=4.5, min=2, max=8
    all: n=30, hits=20, rate=0.667, mean=4.7, median=4, min=0, max=12
  Split home/away:
    home: n=15, hits=12, rate=0.800, mean=5.4667, median=5
    away: n=15, hits=8, rate=0.533, mean=3.9333, median=4
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 19. total_goals @ 1.5 — Sweden vs Poland (sin rol)
- basis: match_total_goals
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=4, median=3, min=2, max=6
-   last10: n=10, hits=10, rate=1.000, mean=3.9, median=4, min=2, max=6
-   all: n=30, hits=27, rate=0.900, mean=3.6333, median=3, min=1, max=7
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=3.6667, median=3
-   away: n=15, hits=14, rate=0.933, mean=3.6, median=4
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=2.8, median=3, min=0, max=5
    last10: n=10, hits=8, rate=0.800, mean=2.8, median=2.5, min=0, max=5
    all: n=30, hits=24, rate=0.800, mean=2.8667, median=2.5, min=0, max=6
  Split home/away:
    home: n=15, hits=12, rate=0.800, mean=2.7333, median=3
    away: n=15, hits=12, rate=0.800, mean=3, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 20. total_goals @ 2.5 — Sweden vs Poland (sin rol)
- basis: match_total_goals
- FM reportado: hits 8/9; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=4, median=3, min=2, max=6
-   last10: n=10, hits=8, rate=0.800, mean=3.9, median=4, min=2, max=6
-   all: n=30, hits=22, rate=0.733, mean=3.6333, median=3, min=1, max=7
- Split home/away:
-   home: n=15, hits=11, rate=0.733, mean=3.6667, median=3
-   away: n=15, hits=11, rate=0.733, mean=3.6, median=4
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=2.8, median=3, min=0, max=5
    last10: n=10, hits=5, rate=0.500, mean=2.8, median=2.5, min=0, max=5
    all: n=30, hits=15, rate=0.500, mean=2.8667, median=2.5, min=0, max=6
  Split home/away:
    home: n=15, hits=8, rate=0.533, mean=2.7333, median=3
    away: n=15, hits=7, rate=0.467, mean=3, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 21. 1x2_shots — Poland (away)
- basis: unmapped
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=true, source_conflict=false, motivo="non-numeric history value 'vt'; market '1x2_shots' has no mapping to fm_team_matches.team_stats_json"

### 22. btts — Sweden (home)
- basis: unmapped
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=true, source_conflict=false, motivo="non-numeric history value 'vt'; market 'btts' has no mapping to fm_team_matches.team_stats_json"

### 23. total_cards @ 2.5 — Sweden vs Poland (sin rol)
- basis: not_reproducible
- FM reportado: hits 8/9; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 24. total_cards @ 3.5 — Sweden vs Poland (sin rol)
- basis: not_reproducible
- FM reportado: hits 6/7; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 25. total_cards @ 4.5 — Sweden vs Poland (sin rol)
- basis: not_reproducible
- FM reportado: hits 6/7; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 26. total_cards @ 5.5 — Sweden vs Poland (sin rol)
- basis: not_reproducible
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 27. total_corners @ 5.5 — Sweden vs Poland (sin rol)
- basis: not_reproducible
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 28. total_corners @ 10.5 — Sweden vs Poland (sin rol)
- basis: not_reproducible
- FM reportado: hits 7/7; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 29. total_corners @ 11.5 — Sweden vs Poland (sin rol)
- basis: not_reproducible
- FM reportado: hits 8/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 30. total_corners @ 12.5 — Sweden vs Poland (sin rol)
- basis: not_reproducible
- FM reportado: hits 9/9; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 31. total_offsides @ 3.5 — Sweden vs Poland (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 32. total_shots @ 24.5 — Sweden vs Poland (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 33. total_shots_on_target @ 7.5 — Sweden vs Poland (sin rol)
- basis: not_reproducible
- FM reportado: hits 8/9; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

## Señales de jugadores (orden: mayor muestra disponible, no fuerza de señal)

### 1. shots @ 0.5 — P. Zielinski
- basis: player_own_stats
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=2, median=2, min=0, max=4
  last10: n=10, hits=6, rate=0.600, mean=1.8, median=2, min=0, max=4
  all: n=27, hits=19, rate=0.704, mean=1.3704, median=1, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 2. shots @ 1.5 — P. Zielinski
- basis: player_own_stats
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=2, median=2, min=0, max=4
  last10: n=10, hits=6, rate=0.600, mean=1.8, median=2, min=0, max=4
  all: n=27, hits=8, rate=0.296, mean=1.3704, median=1, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 3. shots @ 1.5 — A. Isak
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.2, median=2, min=1, max=3
  last10: n=10, hits=6, rate=0.600, mean=2, median=2, min=0, max=6
  all: n=23, hits=15, rate=0.652, mean=2.7826, median=2, min=0, max=11
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 4. shots_on_target @ 0.5 — A. Isak
- basis: player_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=1.4, median=1, min=1, max=2
  last10: n=10, hits=7, rate=0.700, mean=1, median=1, min=0, max=2
  all: n=23, hits=17, rate=0.739, mean=1.3478, median=1, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 5. shots_on_target @ 0.5 — V. Gyökeres
- basis: player_own_stats
- FM reportado: hits 8/9; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.4, median=1, min=0, max=3
  last10: n=10, hits=8, rate=0.800, mean=1.4, median=1, min=0, max=3
  all: n=20, hits=16, rate=0.800, mean=1.65, median=1.5, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 6. shots @ 0.5 — M. Cash
- basis: player_own_stats
- FM reportado: hits 8/8; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.4, median=1, min=0, max=4
  last10: n=10, hits=8, rate=0.800, mean=1.9, median=1.5, min=0, max=5
  all: n=16, hits=11, rate=0.688, mean=1.5625, median=1, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 7. shots @ 0.5 — S. Nanasi
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=0.6, median=1, min=0, max=1
  last10: n=10, hits=5, rate=0.500, mean=0.9, median=0.5, min=0, max=3
  all: n=16, hits=7, rate=0.438, mean=0.75, median=0, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 8. shots @ 0.5 — M. Skoras
- basis: player_own_stats
- FM reportado: hits 5/6; mejor ventana: 10 (5/6)
- Ventanas propias:
  last5: n=5, hits=2, rate=0.400, mean=0.4, median=0, min=0, max=1
  last10: n=10, hits=5, rate=0.500, mean=1, median=0.5, min=0, max=5
  all: n=15, hits=5, rate=0.333, mean=0.6667, median=0, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 9. foul_involvements @ 1.5 — Y. Ayari
- basis: player_own_stats
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=3.6, median=3, min=2, max=6
  last10: n=10, hits=8, rate=0.800, mean=2.9, median=3, min=1, max=6
  all: n=14, hits=11, rate=0.786, mean=3.0714, median=3, min=1, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 10. foul_involvements @ 2.5 — Y. Ayari
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=3.6, median=3, min=2, max=6
  last10: n=10, hits=7, rate=0.700, mean=2.9, median=3, min=1, max=6
  all: n=14, hits=10, rate=0.714, mean=3.0714, median=3, min=1, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 11. fouls_committed @ 0.5 — Y. Ayari
- basis: player_own_stats
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.4, median=1, min=0, max=3
  last10: n=10, hits=8, rate=0.800, mean=1.4, median=1, min=0, max=3
  all: n=14, hits=11, rate=0.786, mean=1.5, median=1, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 12. fouls_drawn @ 0.5 — S. Szymanski
- basis: player_own_stats
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1.8, median=1, min=0, max=5
  last10: n=10, hits=7, rate=0.700, mean=1.9, median=1.5, min=0, max=5
  all: n=14, hits=10, rate=0.714, mean=1.6429, median=1, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 13. fouls_drawn @ 0.5 — Y. Ayari
- basis: player_own_stats
- FM reportado: hits 7/7; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=2.2, median=2, min=1, max=3
  last10: n=10, hits=8, rate=0.800, mean=1.5, median=1.5, min=0, max=3
  all: n=14, hits=11, rate=0.786, mean=1.5714, median=1.5, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 14. fouls_drawn @ 1.5 — Y. Ayari
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.2, median=2, min=1, max=3
  last10: n=10, hits=5, rate=0.500, mean=1.5, median=1.5, min=0, max=3
  all: n=14, hits=7, rate=0.500, mean=1.5714, median=1.5, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 15. tackles @ 0.5 — J. Kiwior
- basis: player_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.2, median=1, min=0, max=2
  last10: n=10, hits=9, rate=0.900, mean=2.3, median=2.5, min=0, max=5
  all: n=13, hits=10, rate=0.769, mean=2.0769, median=2, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 16. foul_involvements @ 1.5 — V. Gyökeres
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.8, median=3, min=0, max=4
  last10: n=10, hits=8, rate=0.800, mean=2.8, median=3, min=0, max=4
  all: n=12, hits=9, rate=0.750, mean=2.8333, median=3, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 17. foul_involvements @ 2.5 — V. Gyökeres
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.8, median=3, min=0, max=4
  last10: n=10, hits=8, rate=0.800, mean=2.8, median=3, min=0, max=4
  all: n=12, hits=9, rate=0.750, mean=2.8333, median=3, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 18. fouls_drawn @ 0.5 — V. Gyökeres
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2, median=2, min=0, max=4
  last10: n=10, hits=9, rate=0.900, mean=1.9, median=2, min=0, max=4
  all: n=12, hits=10, rate=0.833, mean=1.6667, median=1.5, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 19. fouls_drawn @ 1.5 — V. Gyökeres
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=2, median=2, min=0, max=4
  last10: n=10, hits=6, rate=0.600, mean=1.9, median=2, min=0, max=4
  all: n=12, hits=6, rate=0.500, mean=1.6667, median=1.5, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 20. foul_involvements @ 1.5 — R. Lewandowski
- basis: player_own_stats
- FM reportado: hits 8/9; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=2.4, median=2, min=0, max=6
  last10: n=10, hits=8, rate=0.800, mean=2.9, median=2.5, min=0, max=6
  all: n=11, hits=8, rate=0.727, mean=2.7273, median=2, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 21. fouls_drawn @ 0.5 — J. Bednarek
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=0.6, median=1, min=0, max=1
  last10: n=10, hits=3, rate=0.300, mean=0.3, median=0, min=0, max=1
  all: n=11, hits=4, rate=0.364, mean=0.3636, median=0, min=0, max=1
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 22. tackles @ 0.5 — G. Gudmundsson
- basis: player_own_stats
- FM reportado: hits 7/8; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=1.6, median=2, min=1, max=2
  last10: n=10, hits=9, rate=0.900, mean=1.3, median=1, min=0, max=2
  all: n=11, hits=10, rate=0.909, mean=1.4545, median=1, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 23. tackles @ 0.5 — G. Lagerbielke
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=2.6, median=2, min=0, max=9
  last10: n=10, hits=7, rate=0.700, mean=1.7, median=1, min=0, max=9
  all: n=11, hits=8, rate=0.727, mean=1.8182, median=1, min=0, max=9
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 24. tackles @ 1.5 — G. Lagerbielke
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=2.6, median=2, min=0, max=9
  last10: n=10, hits=3, rate=0.300, mean=1.7, median=1, min=0, max=9
  all: n=11, hits=4, rate=0.364, mean=1.8182, median=1, min=0, max=9
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 25. fouls_committed @ 0.5 — M. Cash
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.8, median=1, min=0, max=4
  last10: n=10, hits=7, rate=0.700, mean=1.2, median=1, min=0, max=4
  all: n=10, hits=7, rate=0.700, mean=1.2, median=1, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 26. tackles @ 0.5 — A. Bernhardsson
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.2, median=1, min=0, max=3
  last10: n=10, hits=9, rate=0.900, mean=1.4, median=1, min=0, max=3
  all: n=10, hits=9, rate=0.900, mean=1.4, median=1, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 27. tackles @ 0.5 — M. Cash
- basis: player_own_stats
- FM reportado: hits 8/8; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=2.4, median=2, min=1, max=5
  last10: n=10, hits=8, rate=0.800, mean=1.9, median=1, min=0, max=5
  all: n=10, hits=8, rate=0.800, mean=1.9, median=1, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 28. fouls_drawn @ 0.5 — M. Skoras
- basis: player_own_stats
- FM reportado: hits 5/6; mejor ventana: 10 (5/6)
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.4, median=2, min=0, max=5
  last10: n=6, hits=5, rate=0.833, mean=2.1667, median=1.5, min=0, max=5
  all: n=6, hits=5, rate=0.833, mean=2.1667, median=1.5, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 29. tackles @ 0.5 — M. Skoras
- basis: player_own_stats
- FM reportado: hits 6/6; mejor ventana: 10 (6/6)
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=2.8, median=3, min=1, max=5
  last10: n=6, hits=6, rate=1.000, mean=2.5, median=2.5, min=1, max=5
  all: n=6, hits=6, rate=1.000, mean=2.5, median=2.5, min=1, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 30. foul_involvements @ 1.5 — L. Bergvall
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1.8, median=2, min=0, max=4
  last10: n=5, hits=3, rate=0.600, mean=1.8, median=2, min=0, max=4
  all: n=5, hits=3, rate=0.600, mean=1.8, median=2, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 31. fouls_drawn @ 0.5 — L. Bergvall
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1, median=1, min=0, max=2
  last10: n=5, hits=3, rate=0.600, mean=1, median=1, min=0, max=2
  all: n=5, hits=3, rate=0.600, mean=1, median=1, min=0, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 32. tackles @ 0.5 — L. Bergvall
- basis: player_own_stats
- FM reportado: hits 7/8; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.2, median=1, min=0, max=2
  last10: n=5, hits=4, rate=0.800, mean=1.2, median=1, min=0, max=2
  all: n=5, hits=4, rate=0.800, mean=1.2, median=1, min=0, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 33. goalkeeper_saves @ 1.5 — V. Johansson
- basis: player_own_stats
- FM reportado: hits 6/7; mejor ventana: no determinable
- Ventanas propias:
  last5: n=4, hits=3, rate=0.750, mean=2.25, median=2.5, min=0, max=4
  last10: n=4, hits=3, rate=0.750, mean=2.25, median=2.5, min=0, max=4
  all: n=4, hits=3, rate=0.750, mean=2.25, median=2.5, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 34. tackles @ 0.5 — P. Frankowski
- basis: player_own_stats
- FM reportado: hits 5/6; mejor ventana: 10 (5/6)
- Ventanas propias:
  last5: n=3, hits=3, rate=1.000, mean=2, median=1, min=1, max=4
  last10: n=3, hits=3, rate=1.000, mean=2, median=1, min=1, max=4
  all: n=3, hits=3, rate=1.000, mean=2, median=1, min=1, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 35. fouls_committed @ 0.5 — K. Urbański
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: INSUFFICIENT_SAMPLE
  last10: INSUFFICIENT_SAMPLE
  all: INSUFFICIENT_SAMPLE
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 36. tackles @ 0.5 — B. Zeneli
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: INSUFFICIENT_SAMPLE
  last10: INSUFFICIENT_SAMPLE
  all: INSUFFICIENT_SAMPLE
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

## Evidencia histórica (ventanas fijas)
- observaciones descriptivas: secuencia cruda (más antiguo -> más reciente) y hits de las ventanas fijas (last5/last10/all)

### Jugadores — evidencia con ventanas fijas
- P. Zielinski — shots @ 0.5: [0,3,0,2,3,4,2,0,0,4] | last5 3/5 | last10 6/10 | all 19/27
- P. Zielinski — shots @ 1.5: [0,3,0,2,3,4,2,0,0,4] | last5 3/5 | last10 6/10 | all 8/27
- A. Isak — shots @ 1.5: [6,0,0,2,1,3,1,2,2,3] | last5 4/5 | last10 6/10 | all 15/23
- A. Isak — shots_on_target @ 0.5: [2,0,0,1,0,2,1,2,1,1] | last5 5/5 | last10 7/10 | all 17/23
- V. Gyökeres — shots_on_target @ 0.5: [0,1,3,1,2,2,3,0,1,1] | last5 4/5 | last10 8/10 | all 16/20
- M. Cash — shots @ 0.5: [5,3,2,0,2,1,0,1,1,4] | last5 4/5 | last10 8/10 | all 11/16
- S. Nanasi — shots @ 0.5: [3,0,3,0,0,0,1,1,0,1] | last5 3/5 | last10 5/10 | all 7/16
- M. Skoras — shots @ 0.5: [0,0,5,1,2,1,1,0,0,0] | last5 2/5 | last10 5/10 | all 5/15
- Y. Ayari — foul_involvements @ 1.5: [1,3,3,1,3,2,3,3,4,6] | last5 5/5 | last10 8/10 | all 11/14
- Y. Ayari — foul_involvements @ 2.5: [1,3,3,1,3,2,3,3,4,6] | last5 4/5 | last10 7/10 | all 10/14
- Y. Ayari — fouls_committed @ 0.5: [1,2,3,0,1,1,1,0,2,3] | last5 4/5 | last10 8/10 | all 11/14
- S. Szymanski — fouls_drawn @ 0.5: [2,3,0,4,1,1,3,0,0,5] | last5 3/5 | last10 7/10 | all 10/14
- Y. Ayari — fouls_drawn @ 0.5: [0,1,0,1,2,1,2,3,2,3] | last5 5/5 | last10 8/10 | all 11/14
- Y. Ayari — fouls_drawn @ 1.5: [0,1,0,1,2,1,2,3,2,3] | last5 4/5 | last10 5/10 | all 7/14
- J. Kiwior — tackles @ 0.5: [3,3,3,5,3,1,2,1,0,2] | last5 4/5 | last10 9/10 | all 10/13
- V. Gyökeres — foul_involvements @ 1.5: [3,3,1,3,4,0,3,4,4,3] | last5 4/5 | last10 8/10 | all 9/12
- V. Gyökeres — foul_involvements @ 2.5: [3,3,1,3,4,0,3,4,4,3] | last5 4/5 | last10 8/10 | all 9/12
- V. Gyökeres — fouls_drawn @ 0.5: [3,1,1,2,2,0,1,2,4,3] | last5 4/5 | last10 9/10 | all 10/12
- V. Gyökeres — fouls_drawn @ 1.5: [3,1,1,2,2,0,1,2,4,3] | last5 3/5 | last10 6/10 | all 6/12
- R. Lewandowski — foul_involvements @ 1.5: [3,2,6,4,2,6,4,0,0,2] | last5 3/5 | last10 8/10 | all 8/11
- J. Bednarek — fouls_drawn @ 0.5: [0,0,0,0,0,0,1,1,0,1] | last5 3/5 | last10 3/10 | all 4/11
- G. Gudmundsson — tackles @ 0.5: [1,2,0,1,1,1,2,1,2,2] | last5 5/5 | last10 9/10 | all 10/11
- G. Lagerbielke — tackles @ 0.5: [1,1,0,1,1,0,0,9,2,2] | last5 3/5 | last10 7/10 | all 8/11
- G. Lagerbielke — tackles @ 1.5: [1,1,0,1,1,0,0,9,2,2] | last5 3/5 | last10 3/10 | all 4/11
- M. Cash — fouls_committed @ 0.5: [1,0,0,1,1,1,0,1,3,4] | last5 4/5 | last10 7/10 | all 7/10
- A. Bernhardsson — tackles @ 0.5: [3,1,1,1,2,0,3,1,1,1] | last5 4/5 | last10 9/10 | all 9/10
- M. Cash — tackles @ 0.5: [0,0,1,5,1,5,3,1,1,2] | last5 5/5 | last10 8/10 | all 8/10
- M. Skoras — fouls_drawn @ 0.5: [1,4,1,5,0,2] | last5 4/5 | last10 5/6 | all 5/6
- M. Skoras — tackles @ 0.5: [1,5,3,1,2,3] | last5 5/5 | last10 6/6 | all 6/6
- L. Bergvall — foul_involvements @ 1.5: [1,2,0,4,2] | last5 3/5 | last10 3/5 | all 3/5

### Equipos — evidencia con ventanas fijas
- Poland — away_cards @ 1.5: [1,1,2,2,2,2,3,0,2,3] | last5 4/5 | last10 7/10 | all 18/30
  - Rival (Sweden): [2,1,2,1,0,0,3,2,0,0] | last5 2/5 | last10 4/10 | all 14/30
- Poland — away_cards @ 3.5: [1,1,2,2,2,2,3,0,2,3] | last5 5/5 | last10 10/10 | all 26/30
  - Rival (Sweden): [2,1,2,1,0,0,3,2,0,0] | last5 5/5 | last10 10/10 | all 28/30
- Poland — away_corners @ 2.5: [8,7,8,2,9,6,9,6,5,3] | last5 5/5 | last10 9/10 | all 26/30
  - Rival (Sweden): [6,3,2,1,1,4,5,8,1,3] | last5 4/5 | last10 6/10 | all 25/30
- Sweden — away_goals @ 0.5: [1,3,3,1,2,5,1,1,0,2] | last5 4/5 | last10 9/10 | all 24/30
  - Rival (Poland): [3,1,2,1,3,2,2,0,2,0] | last5 3/5 | last10 8/10 | all 25/30
- Poland — away_goals @ 2.5: [3,1,2,1,3,2,2,0,2,0] | last5 5/5 | last10 8/10 | all 26/30
  - Rival (Sweden): [1,3,3,1,2,5,1,1,0,2] | last5 4/5 | last10 7/10 | all 21/30
- Poland — away_shots @ 10.5: [15,17,12,12,18,15,15,17,12,14] | last5 5/5 | last10 10/10 | all 26/30
  - Rival (Sweden): [11,10,9,9,13,13,16,11,8,11] | last5 4/5 | last10 6/10 | all 21/30
- Poland — away_shots @ 11.5: [15,17,12,12,18,15,15,17,12,14] | last5 5/5 | last10 10/10 | all 23/30
  - Rival (Sweden): [11,10,9,9,13,13,16,11,8,11] | last5 2/5 | last10 3/10 | all 17/30
- Poland — away_shots_on_target @ 3.5: [6,3,6,5,4,4,7,2,8,4] | last5 4/5 | last10 8/10 | all 20/30
  - Rival (Sweden): [2,4,5,5,5,7,8,5,3,4] | last5 4/5 | last10 8/10 | all 23/30
- Sweden — home_cards @ 2.5: [2,1,2,1,0,0,3,2,0,0] | last5 4/5 | last10 9/10 | all 24/30
  - Rival (Poland): [1,1,2,2,2,2,3,0,2,3] | last5 3/5 | last10 8/10 | all 21/30
- Sweden — home_corners @ 5.5: [6,3,2,1,1,4,5,8,1,3] | last5 4/5 | last10 8/10 | all 17/30
  - Rival (Poland): [8,7,8,2,9,6,9,6,5,3] | last5 2/5 | last10 3/10 | all 17/30
- Sweden — home_corners @ 6.5: [6,3,2,1,1,4,5,8,1,3] | last5 4/5 | last10 9/10 | all 20/30
  - Rival (Poland): [8,7,8,2,9,6,9,6,5,3] | last5 4/5 | last10 5/10 | all 21/30
- Sweden — home_corners @ 7.5: [6,3,2,1,1,4,5,8,1,3] | last5 4/5 | last10 9/10 | all 21/30
  - Rival (Poland): [8,7,8,2,9,6,9,6,5,3] | last5 4/5 | last10 6/10 | all 24/30
- Sweden — home_goals @ 0.5: [1,3,3,1,2,5,1,1,0,2] | last5 4/5 | last10 9/10 | all 24/30
  - Rival (Poland): [3,1,2,1,3,2,2,0,2,0] | last5 3/5 | last10 8/10 | all 25/30
- Poland — home_goals @ 2.5: [3,1,2,1,3,2,2,0,2,0] | last5 5/5 | last10 8/10 | all 26/30
  - Rival (Sweden): [1,3,3,1,2,5,1,1,0,2] | last5 4/5 | last10 7/10 | all 21/30
- Sweden — home_goals @ 2.5: [1,3,3,1,2,5,1,1,0,2] | last5 4/5 | last10 7/10 | all 21/30
  - Rival (Poland): [3,1,2,1,3,2,2,0,2,0] | last5 5/5 | last10 8/10 | all 26/30
- Sweden — home_shots @ 13.5: [11,10,9,9,13,13,16,11,8,11] | last5 4/5 | last10 9/10 | all 19/30
  - Rival (Poland): [15,17,12,12,18,15,15,17,12,14] | last5 1/5 | last10 3/10 | all 15/30
- Sweden — home_shots @ 15.5: [11,10,9,9,13,13,16,11,8,11] | last5 4/5 | last10 9/10 | all 21/30
  - Rival (Poland): [15,17,12,12,18,15,15,17,12,14] | last5 4/5 | last10 7/10 | all 21/30
- Sweden — home_shots_on_target @ 3.5: [2,4,5,5,5,7,8,5,3,4] | last5 4/5 | last10 8/10 | all 23/30
  - Rival (Poland): [6,3,6,5,4,4,7,2,8,4] | last5 4/5 | last10 8/10 | all 20/30
- Sweden vs Poland — total_goals @ 1.5: [2,4,5,4,4,6,6,2,3,3] | last5 5/5 | last10 10/10 | all 27/30
- Sweden vs Poland — total_goals @ 2.5: [2,4,5,4,4,6,6,2,3,3] | last5 4/5 | last10 8/10 | all 22/30

## Confluencia descriptiva (equipo + rival + jugadores)
- familias de mercado con 2 o más series observadas en la DB; cada serie conserva su línea y sus ventanas fijas (last5/last10/all); sin score ni orden por fuerza

### cards
- Poland (away) @ 1.5: last5 4/5 | last10 7/10 | all 18/30
- Sweden (home) @ 1.5: last5 2/5 | last10 4/10 | all 14/30
- Poland (away) @ 3.5: last5 5/5 | last10 10/10 | all 26/30
- Sweden (home) @ 3.5: last5 5/5 | last10 10/10 | all 28/30
- Sweden (home) @ 2.5: last5 4/5 | last10 9/10 | all 24/30
- Poland (away) @ 2.5: last5 3/5 | last10 8/10 | all 21/30

### corners
- Poland (away) @ 2.5: last5 5/5 | last10 9/10 | all 26/30
- Sweden (home) @ 2.5: last5 4/5 | last10 6/10 | all 25/30
- Sweden (home) @ 5.5: last5 4/5 | last10 8/10 | all 17/30
- Poland (away) @ 5.5: last5 2/5 | last10 3/10 | all 17/30
- Sweden (home) @ 6.5: last5 4/5 | last10 9/10 | all 20/30
- Poland (away) @ 6.5: last5 4/5 | last10 5/10 | all 21/30
- Sweden (home) @ 7.5: last5 4/5 | last10 9/10 | all 21/30
- Poland (away) @ 7.5: last5 4/5 | last10 6/10 | all 24/30

### foul_involvements
- jugador Y. Ayari — foul_involvements @ 1.5: last5 5/5 | last10 8/10 | all 11/14
- jugador Y. Ayari — foul_involvements @ 2.5: last5 4/5 | last10 7/10 | all 10/14
- jugador V. Gyökeres — foul_involvements @ 1.5: last5 4/5 | last10 8/10 | all 9/12
- jugador V. Gyökeres — foul_involvements @ 2.5: last5 4/5 | last10 8/10 | all 9/12
- jugador R. Lewandowski — foul_involvements @ 1.5: last5 3/5 | last10 8/10 | all 8/11
- jugador L. Bergvall — foul_involvements @ 1.5: last5 3/5 | last10 3/5 | all 3/5

### fouls_committed
- jugador Y. Ayari — fouls_committed @ 0.5: last5 4/5 | last10 8/10 | all 11/14
- jugador M. Cash — fouls_committed @ 0.5: last5 4/5 | last10 7/10 | all 7/10

### fouls_drawn
- jugador S. Szymanski — fouls_drawn @ 0.5: last5 3/5 | last10 7/10 | all 10/14
- jugador Y. Ayari — fouls_drawn @ 0.5: last5 5/5 | last10 8/10 | all 11/14
- jugador Y. Ayari — fouls_drawn @ 1.5: last5 4/5 | last10 5/10 | all 7/14
- jugador V. Gyökeres — fouls_drawn @ 0.5: last5 4/5 | last10 9/10 | all 10/12
- jugador V. Gyökeres — fouls_drawn @ 1.5: last5 3/5 | last10 6/10 | all 6/12
- jugador J. Bednarek — fouls_drawn @ 0.5: last5 3/5 | last10 3/10 | all 4/11
- jugador M. Skoras — fouls_drawn @ 0.5: last5 4/5 | last10 5/6 | all 5/6

### goals
- Sweden (home) @ 0.5: last5 4/5 | last10 9/10 | all 24/30
- Poland (away) @ 0.5: last5 3/5 | last10 8/10 | all 25/30
- Poland (away) @ 2.5: last5 5/5 | last10 8/10 | all 26/30
- Sweden (home) @ 2.5: last5 4/5 | last10 7/10 | all 21/30

### shots
- Poland (away) @ 10.5: last5 5/5 | last10 10/10 | all 26/30
- Sweden (home) @ 10.5: last5 4/5 | last10 6/10 | all 21/30
- Poland (away) @ 11.5: last5 5/5 | last10 10/10 | all 23/30
- Sweden (home) @ 11.5: last5 2/5 | last10 3/10 | all 17/30
- Sweden (home) @ 13.5: last5 4/5 | last10 9/10 | all 19/30
- Poland (away) @ 13.5: last5 1/5 | last10 3/10 | all 15/30
- Sweden (home) @ 15.5: last5 4/5 | last10 9/10 | all 21/30
- Poland (away) @ 15.5: last5 4/5 | last10 7/10 | all 21/30
- jugador P. Zielinski — shots @ 0.5: last5 3/5 | last10 6/10 | all 19/27
- jugador P. Zielinski — shots @ 1.5: last5 3/5 | last10 6/10 | all 8/27
- jugador A. Isak — shots @ 1.5: last5 4/5 | last10 6/10 | all 15/23
- jugador M. Cash — shots @ 0.5: last5 4/5 | last10 8/10 | all 11/16
- jugador S. Nanasi — shots @ 0.5: last5 3/5 | last10 5/10 | all 7/16
- jugador M. Skoras — shots @ 0.5: last5 2/5 | last10 5/10 | all 5/15

### shots_on_target
- Poland (away) @ 3.5: last5 4/5 | last10 8/10 | all 20/30
- Sweden (home) @ 3.5: last5 4/5 | last10 8/10 | all 23/30
- jugador A. Isak — shots_on_target @ 0.5: last5 5/5 | last10 7/10 | all 17/23
- jugador V. Gyökeres — shots_on_target @ 0.5: last5 4/5 | last10 8/10 | all 16/20

### tackles
- jugador J. Kiwior — tackles @ 0.5: last5 4/5 | last10 9/10 | all 10/13
- jugador G. Gudmundsson — tackles @ 0.5: last5 5/5 | last10 9/10 | all 10/11
- jugador G. Lagerbielke — tackles @ 0.5: last5 3/5 | last10 7/10 | all 8/11
- jugador G. Lagerbielke — tackles @ 1.5: last5 3/5 | last10 3/10 | all 4/11
- jugador A. Bernhardsson — tackles @ 0.5: last5 4/5 | last10 9/10 | all 9/10
- jugador M. Cash — tackles @ 0.5: last5 5/5 | last10 8/10 | all 8/10
- jugador M. Skoras — tackles @ 0.5: last5 5/5 | last10 6/6 | all 6/6

## Cuotas

| mercado | línea | casa | lado | cuota | prob. implícita | capturada |
|---|---|---|---|---|---|---|
| away_cards | 1.5 | Ladbrokes | over | 1.44 | 0.694 | 2026-09-28T02:40:11.637Z |
| away_cards | 1.5 | Ladbrokes | under | 2.6 | 0.385 | 2026-09-28T02:40:11.637Z |
| away_corners | 2.5 | Kambi | over | 1.24 | 0.806 | 2026-09-28T02:40:11.637Z |
| away_corners | 2.5 | Kambi | under | 3.4 | 0.294 | 2026-09-28T02:40:11.637Z |
| away_goals | 0.5 | Bet365 | over | 1.3 | 0.769 | 2026-09-28T02:40:11.637Z |
| away_goals | 0.5 | Bet365 | under | 3.4 | 0.294 | 2026-09-28T02:40:11.637Z |
| away_goals | 0.5 | Kambi | over | 1.32 | 0.758 | 2026-09-28T02:40:11.637Z |
| away_goals | 0.5 | Kambi | under | 3.05 | 0.328 | 2026-09-28T02:40:11.637Z |
| away_goals | 0.5 | Ladbrokes | over | 1.3 | 0.769 | 2026-09-28T02:40:11.637Z |
| away_goals | 0.5 | Ladbrokes | under | 3.2 | 0.312 | 2026-09-28T02:40:11.637Z |
| away_goals | 2.5 | Bet365 | over | 6 | 0.167 | 2026-09-28T02:40:11.637Z |
| away_goals | 2.5 | Bet365 | under | 1.13 | 0.885 | 2026-09-28T02:40:11.637Z |
| away_goals | 2.5 | Kambi | over | 6.1 | 0.164 | 2026-09-28T02:40:11.637Z |
| away_goals | 2.5 | Kambi | under | 1.07 | 0.935 | 2026-09-28T02:40:11.637Z |
| away_goals | 2.5 | Ladbrokes | over | 6 | 0.167 | 2026-09-28T02:40:11.637Z |
| away_goals | 2.5 | Ladbrokes | under | 1.1 | 0.909 | 2026-09-28T02:40:11.637Z |
| away_shots | 10.5 | Bet365 | over | 1.83 | 0.546 | 2026-09-28T02:40:11.637Z |
| away_shots | 10.5 | Bet365 | under | 1.83 | 0.546 | 2026-09-28T02:40:11.637Z |
| away_shots | 10.5 | Kambi | over | 1.79 | 0.559 | 2026-09-28T02:40:11.637Z |
| away_shots | 10.5 | Kambi | under | 1.73 | 0.578 | 2026-09-28T02:40:11.637Z |
| away_shots | 11.5 | Kambi | over | 2.17 | 0.461 | 2026-09-28T02:40:11.637Z |
| away_shots | 11.5 | Kambi | under | 1.48 | 0.676 | 2026-09-28T02:40:11.637Z |
| away_shots_on_target | 3.5 | Bet365 | over | 1.83 | 0.546 | 2026-09-28T02:40:11.637Z |
| away_shots_on_target | 3.5 | Bet365 | under | 1.83 | 0.546 | 2026-09-28T02:40:11.637Z |
| away_shots_on_target | 3.5 | Kambi | over | 1.61 | 0.621 | 2026-09-28T02:40:11.637Z |
| away_shots_on_target | 3.5 | Kambi | under | 1.94 | 0.515 | 2026-09-28T02:40:11.637Z |
| home_corners | 5.5 | Kambi | over | 2.28 | 0.439 | 2026-09-28T02:40:11.637Z |
| home_corners | 5.5 | Kambi | under | 1.52 | 0.658 | 2026-09-28T02:40:11.637Z |
| home_corners | 6.5 | Kambi | over | 3.15 | 0.317 | 2026-09-28T02:40:11.637Z |
| home_corners | 6.5 | Kambi | under | 1.28 | 0.781 | 2026-09-28T02:40:11.637Z |
| home_corners | 7.5 | Kambi | over | 4.4 | 0.227 | 2026-09-28T02:40:11.637Z |
| home_corners | 7.5 | Kambi | under | 1.14 | 0.877 | 2026-09-28T02:40:11.637Z |
| home_goals | 0.5 | Bet365 | over | 1.18 | 0.847 | 2026-09-28T02:40:11.637Z |
| home_goals | 0.5 | Bet365 | under | 4.5 | 0.222 | 2026-09-28T02:40:11.637Z |
| home_goals | 0.5 | Kambi | over | 1.18 | 0.847 | 2026-09-28T02:40:11.637Z |
| home_goals | 0.5 | Kambi | under | 4.25 | 0.235 | 2026-09-28T02:40:11.637Z |
| home_goals | 0.5 | Ladbrokes | over | 1.18 | 0.847 | 2026-09-28T02:40:11.637Z |
| home_goals | 0.5 | Ladbrokes | under | 4.4 | 0.227 | 2026-09-28T02:40:11.637Z |
| home_goals | 2.5 | Bet365 | over | 4 | 0.250 | 2026-09-28T02:40:11.637Z |
| home_goals | 2.5 | Bet365 | under | 1.22 | 0.820 | 2026-09-28T02:40:11.637Z |
| home_goals | 2.5 | Kambi | over | 3.9 | 0.256 | 2026-09-28T02:40:11.637Z |
| home_goals | 2.5 | Kambi | under | 1.22 | 0.820 | 2026-09-28T02:40:11.637Z |
| home_goals | 2.5 | Ladbrokes | over | 3.8 | 0.263 | 2026-09-28T02:40:11.637Z |
| home_goals | 2.5 | Ladbrokes | under | 1.22 | 0.820 | 2026-09-28T02:40:11.637Z |
| home_goals | 2.5 | Bet365 | over | 4 | 0.250 | 2026-09-28T02:40:11.637Z |
| home_goals | 2.5 | Bet365 | under | 1.22 | 0.820 | 2026-09-28T02:40:11.637Z |
| home_goals | 2.5 | Kambi | over | 3.9 | 0.256 | 2026-09-28T02:40:11.637Z |
| home_goals | 2.5 | Kambi | under | 1.22 | 0.820 | 2026-09-28T02:40:11.637Z |
| home_goals | 2.5 | Ladbrokes | over | 3.8 | 0.263 | 2026-09-28T02:40:11.637Z |
| home_goals | 2.5 | Ladbrokes | under | 1.22 | 0.820 | 2026-09-28T02:40:11.637Z |
| home_shots | 13.5 | Bet365 | over | 2 | 0.500 | 2026-09-28T02:40:11.637Z |
| home_shots | 13.5 | Bet365 | under | 1.73 | 0.578 | 2026-09-28T02:40:11.637Z |
| home_shots_on_target | 3.5 | Kambi | over | 1.33 | 0.752 | 2026-09-28T02:40:11.637Z |
| home_shots_on_target | 3.5 | Kambi | under | 2.6 | 0.385 | 2026-09-28T02:40:11.637Z |
| total_goals | 1.5 | Bet365 | over | 1.2 | 0.833 | 2026-09-28T02:40:11.637Z |
| total_goals | 1.5 | Bet365 | under | 4.33 | 0.231 | 2026-09-28T02:40:11.637Z |
| total_goals | 1.5 | Kambi | over | 1.22 | 0.820 | 2026-09-28T02:40:11.637Z |
| total_goals | 1.5 | Kambi | under | 4 | 0.250 | 2026-09-28T02:40:11.637Z |
| total_goals | 1.5 | Ladbrokes | over | 1.2 | 0.833 | 2026-09-28T02:40:11.637Z |
| total_goals | 1.5 | Ladbrokes | under | 4 | 0.250 | 2026-09-28T02:40:11.637Z |
| total_goals | 1.5 | Paddy Power | over | 1.17 | 0.855 | 2026-09-28T02:40:11.637Z |
| total_goals | 1.5 | Paddy Power | under | 4.5 | 0.222 | 2026-09-28T02:40:11.637Z |
| total_goals | 2.5 | Altenar | over | 1.67 | 0.599 | 2026-09-28T02:40:11.637Z |
| total_goals | 2.5 | Altenar | under | 2.05 | 0.488 | 2026-09-28T02:40:11.637Z |
| total_goals | 2.5 | Bet365 | over | 1.7 | 0.588 | 2026-09-28T02:40:11.637Z |
| total_goals | 2.5 | Bet365 | under | 2.1 | 0.476 | 2026-09-28T02:40:11.637Z |
| total_goals | 2.5 | Kambi | over | 1.76 | 0.568 | 2026-09-28T02:40:11.637Z |
| total_goals | 2.5 | Kambi | under | 2.02 | 0.495 | 2026-09-28T02:40:11.637Z |
| total_goals | 2.5 | Ladbrokes | over | 1.7 | 0.588 | 2026-09-28T02:40:11.637Z |
| total_goals | 2.5 | Ladbrokes | under | 2.05 | 0.488 | 2026-09-28T02:40:11.637Z |
| total_goals | 2.5 | Paddy Power | over | 1.65 | 0.606 | 2026-09-28T02:40:11.637Z |
| total_goals | 2.5 | Paddy Power | under | 2.1 | 0.476 | 2026-09-28T02:40:11.637Z |
| total_cards | 2.5 | Ladbrokes | over | 1.3 | 0.769 | 2026-09-28T02:40:11.637Z |
| total_cards | 2.5 | Ladbrokes | under | 3.2 | 0.312 | 2026-09-28T02:40:11.637Z |
| total_cards | 3.5 | Bet365 | over | 1.8 | 0.556 | 2026-09-28T02:40:11.637Z |
| total_cards | 3.5 | Bet365 | under | 1.91 | 0.524 | 2026-09-28T02:40:11.637Z |
| total_cards | 3.5 | Kambi | over | 1.63 | 0.613 | 2026-09-28T02:40:11.637Z |
| total_cards | 3.5 | Kambi | under | 2.1 | 0.476 | 2026-09-28T02:40:11.637Z |
| total_cards | 3.5 | Ladbrokes | over | 1.8 | 0.556 | 2026-09-28T02:40:11.637Z |
| total_cards | 3.5 | Ladbrokes | under | 1.91 | 0.524 | 2026-09-28T02:40:11.637Z |
| total_cards | 4.5 | Kambi | over | 2.23 | 0.448 | 2026-09-28T02:40:11.637Z |
| total_cards | 4.5 | Kambi | under | 1.56 | 0.641 | 2026-09-28T02:40:11.637Z |
| total_cards | 4.5 | Ladbrokes | over | 2.75 | 0.364 | 2026-09-28T02:40:11.637Z |
| total_cards | 4.5 | Ladbrokes | under | 1.4 | 0.714 | 2026-09-28T02:40:11.637Z |
| total_cards | 5.5 | Kambi | over | 3.25 | 0.308 | 2026-09-28T02:40:11.637Z |
| total_cards | 5.5 | Kambi | under | 1.28 | 0.781 | 2026-09-28T02:40:11.637Z |
| total_corners | 5.5 | Bet365 | over | 1.1 | 0.909 | 2026-09-28T02:40:11.637Z |
| total_corners | 5.5 | Bet365 | under | 7 | 0.143 | 2026-09-28T02:40:11.637Z |
| total_corners | 10.5 | Kambi | over | 2.55 | 0.392 | 2026-09-28T02:40:11.637Z |
| total_corners | 10.5 | Kambi | under | 1.44 | 0.694 | 2026-09-28T02:40:11.637Z |
| total_corners | 10.5 | Ladbrokes | over | 3 | 0.333 | 2026-09-28T02:40:11.637Z |
| total_corners | 10.5 | Ladbrokes | under | 1.33 | 0.752 | 2026-09-28T02:40:11.637Z |
| total_corners | 10.5 | Paddy Power | over | 3.2 | 0.312 | 2026-09-28T02:40:11.637Z |
| total_corners | 10.5 | Paddy Power | under | 1.29 | 0.775 | 2026-09-28T02:40:11.637Z |
| total_corners | 11.5 | Kambi | over | 3.35 | 0.299 | 2026-09-28T02:40:11.637Z |
| total_corners | 11.5 | Kambi | under | 1.26 | 0.794 | 2026-09-28T02:40:11.637Z |
| total_corners | 11.5 | Paddy Power | over | 4.5 | 0.222 | 2026-09-28T02:40:11.637Z |
| total_corners | 11.5 | Paddy Power | under | 1.15 | 0.870 | 2026-09-28T02:40:11.637Z |
| total_corners | 12.5 | Kambi | over | 4.6 | 0.217 | 2026-09-28T02:40:11.637Z |
| total_corners | 12.5 | Kambi | under | 1.14 | 0.877 | 2026-09-28T02:40:11.637Z |
| total_corners | 12.5 | Paddy Power | over | 6.5 | 0.154 | 2026-09-28T02:40:11.637Z |
| total_corners | 12.5 | Paddy Power | under | 1.07 | 0.935 | 2026-09-28T02:40:11.637Z |
| total_offsides | 3.5 | Kambi | over | 1.48 | 0.676 | 2026-09-28T02:40:11.637Z |
| total_shots | 24.5 | Bet365 | over | 2 | 0.500 | 2026-09-28T02:40:11.637Z |
| total_shots | 24.5 | Bet365 | under | 1.73 | 0.578 | 2026-09-28T02:40:11.637Z |
| total_shots | 24.5 | Kambi | over | 2.18 | 0.459 | 2026-09-28T02:40:11.637Z |
| total_shots | 24.5 | Kambi | under | 1.53 | 0.654 | 2026-09-28T02:40:11.637Z |
| total_shots_on_target | 7.5 | Kambi | over | 1.44 | 0.694 | 2026-09-28T02:40:11.637Z |
| total_shots_on_target | 7.5 | Kambi | under | 2.4 | 0.417 | 2026-09-28T02:40:11.637Z |
- Cuota manual Betano (away_cards @ 1.5): no cargada
- Cuota manual Betano (away_cards @ 3.5): no cargada
- Cuota manual Betano (away_corners @ 2.5): no cargada
- Cuota manual Betano (away_goals @ 0.5): no cargada
- Cuota manual Betano (away_goals @ 2.5): no cargada
- Cuota manual Betano (away_shots @ 10.5): no cargada
- Cuota manual Betano (away_shots @ 11.5): no cargada
- Cuota manual Betano (away_shots_on_target @ 3.5): no cargada
- Cuota manual Betano (home_cards @ 2.5): no cargada
- Cuota manual Betano (home_corners @ 5.5): no cargada
- Cuota manual Betano (home_corners @ 6.5): no cargada
- Cuota manual Betano (home_corners @ 7.5): no cargada
- Cuota manual Betano (home_goals @ 0.5): no cargada
- Cuota manual Betano (home_goals @ 2.5): no cargada
- Cuota manual Betano (home_goals @ 2.5): no cargada
- Cuota manual Betano (home_shots @ 13.5): no cargada
- Cuota manual Betano (home_shots @ 15.5): no cargada
- Cuota manual Betano (home_shots_on_target @ 3.5): no cargada
- Cuota manual Betano (total_goals @ 1.5): no cargada
- Cuota manual Betano (total_goals @ 2.5): no cargada
- Cuota manual Betano (1x2_shots): no cargada
- Cuota manual Betano (btts): no cargada
- Cuota manual Betano (total_cards @ 2.5): no cargada
- Cuota manual Betano (total_cards @ 3.5): no cargada
- Cuota manual Betano (total_cards @ 4.5): no cargada
- Cuota manual Betano (total_cards @ 5.5): no cargada
- Cuota manual Betano (total_corners @ 5.5): no cargada
- Cuota manual Betano (total_corners @ 10.5): no cargada
- Cuota manual Betano (total_corners @ 11.5): no cargada
- Cuota manual Betano (total_corners @ 12.5): no cargada
- Cuota manual Betano (total_offsides @ 3.5): no cargada
- Cuota manual Betano (total_shots @ 24.5): no cargada
- Cuota manual Betano (total_shots_on_target @ 7.5): no cargada

## Nota de cierre
Este reporte es descriptivo. No constituye una recomendación de apuesta ni una probabilidad validada de resultado futuro.

---

## Georgia vs Ukraine (33662307) - kickoff 2026-09-28T16:00:00.000Z

POST-PARTIDO - SOLO REGISTRO HISTORICO, NO ACCIONABLE

- Motivo de la marca: partido ya jugado (kickoff 2026-09-28T16:00:00.000Z); las cuotas en fm_market_odds son de cierre y las senales pueden reflejar el resultado ya conocido. Uso unico: dataset historico para calibracion/backtest.
- Snapshot capturado post-partido: 21:35:56Z-21:36:24Z (tabs overview/player-trends/team-trends, 3 navs; +2 fetch de scope partidos sin nav).
- No intentado - pausa del operador por lectura de un ledger manual desactualizado (docs/STATE.md); limite real de la API: 40 navs/corrida, 24 necesarias para los 8 fixtures. Corregido el 2026-09-28.
- Senales OK/SUSPECT persistidas: 50. El informe P11 nativo (descriptivo, sin Delta% ni candidatos) se lista debajo, tal como lo genera GET /api/fm/fixtures/{id}/report.md.

# Georgia vs Ukraine
- Competición: UEFA Nations League
- Kickoff (UTC): desconocido
- Flags de calidad: leakage=false; suspect=true
- sort_criteria: sample_size_desc

## Mercados (orden: mayor muestra disponible, no fuerza de señal)

### 1. away_cards @ 1.5 — Ukraine (away)
- basis: team_own_stats
- FM reportado: hits 8/9; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=2.8, median=2, min=1, max=7
-   last10: n=10, hits=8, rate=0.800, mean=2.6, median=2, min=0, max=7
-   all: n=30, hits=19, rate=0.633, mean=2.0667, median=2, min=0, max=7
- Split home/away:
-   home: n=15, hits=8, rate=0.533, mean=1.5333, median=2
-   away: n=15, hits=11, rate=0.733, mean=2.6, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=1, rate=0.200, mean=1.2, median=1, min=1, max=2
    last10: n=10, hits=3, rate=0.300, mean=1.1, median=1, min=0, max=2
    all: n=30, hits=14, rate=0.467, mean=1.6333, median=1, min=0, max=4
  Split home/away:
    home: n=15, hits=6, rate=0.400, mean=1.6, median=1
    away: n=15, hits=8, rate=0.533, mean=1.6667, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 2. away_cards @ 2.5 — Ukraine (away)
- basis: team_own_stats
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=2.8, median=2, min=1, max=7
-   last10: n=10, hits=6, rate=0.600, mean=2.6, median=2, min=0, max=7
-   all: n=30, hits=21, rate=0.700, mean=2.0667, median=2, min=0, max=7
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=1.5333, median=2
-   away: n=15, hits=8, rate=0.533, mean=2.6, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=1.2, median=1, min=1, max=2
    last10: n=10, hits=10, rate=1.000, mean=1.1, median=1, min=0, max=2
    all: n=30, hits=25, rate=0.833, mean=1.6333, median=1, min=0, max=4
  Split home/away:
    home: n=15, hits=12, rate=0.800, mean=1.6, median=1
    away: n=15, hits=13, rate=0.867, mean=1.6667, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 3. away_cards @ 3.5 — Ukraine (away)
- basis: team_own_stats
- FM reportado: hits 6/7; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=2.8, median=2, min=1, max=7
-   last10: n=10, hits=8, rate=0.800, mean=2.6, median=2, min=0, max=7
-   all: n=30, hits=25, rate=0.833, mean=2.0667, median=2, min=0, max=7
- Split home/away:
-   home: n=15, hits=14, rate=0.933, mean=1.5333, median=2
-   away: n=15, hits=11, rate=0.733, mean=2.6, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=1.2, median=1, min=1, max=2
    last10: n=10, hits=10, rate=1.000, mean=1.1, median=1, min=0, max=2
    all: n=30, hits=26, rate=0.867, mean=1.6333, median=1, min=0, max=4
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=1.6, median=1
    away: n=15, hits=13, rate=0.867, mean=1.6667, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 4. away_goals @ 0.5 — Ukraine (away)
- basis: team_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.4, median=1, min=1, max=2
-   last10: n=10, hits=8, rate=0.800, mean=1.5, median=1, min=0, max=5
-   all: n=30, hits=24, rate=0.800, mean=1.4333, median=1, min=0, max=5
- Split home/away:
-   home: n=15, hits=12, rate=0.800, mean=1.2, median=1
-   away: n=15, hits=12, rate=0.800, mean=1.6667, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=1.4, median=2, min=0, max=2
    last10: n=10, hits=7, rate=0.700, mean=1.2, median=1, min=0, max=3
    all: n=30, hits=24, rate=0.800, mean=1.4667, median=1, min=0, max=6
  Split home/away:
    home: n=15, hits=12, rate=0.800, mean=1.7333, median=1
    away: n=15, hits=12, rate=0.800, mean=1.2, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 5. away_goals @ 1.5 — Georgia (home)
- basis: team_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=2, rate=0.400, mean=1.4, median=2, min=0, max=2
-   last10: n=10, hits=6, rate=0.600, mean=1.2, median=1, min=0, max=3
-   all: n=30, hits=19, rate=0.633, mean=1.4667, median=1, min=0, max=6
- Split home/away:
-   home: n=15, hits=8, rate=0.533, mean=1.7333, median=1
-   away: n=15, hits=11, rate=0.733, mean=1.2, median=1
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=1.4, median=1, min=1, max=2
    last10: n=10, hits=6, rate=0.600, mean=1.5, median=1, min=0, max=5
    all: n=30, hits=17, rate=0.567, mean=1.4333, median=1, min=0, max=5
  Split home/away:
    home: n=15, hits=10, rate=0.667, mean=1.2, median=1
    away: n=15, hits=7, rate=0.467, mean=1.6667, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 6. away_goals @ 2.5 — Georgia (home)
- basis: team_own_stats
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.4, median=2, min=0, max=2
-   last10: n=10, hits=9, rate=0.900, mean=1.2, median=1, min=0, max=3
-   all: n=30, hits=25, rate=0.833, mean=1.4667, median=1, min=0, max=6
- Split home/away:
-   home: n=15, hits=12, rate=0.800, mean=1.7333, median=1
-   away: n=15, hits=13, rate=0.867, mean=1.2, median=1
- Rival (contexto equivalente):
  FM reportado: hits 7/7; mejor ventana: no determinable
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=1.4, median=1, min=1, max=2
    last10: n=10, hits=9, rate=0.900, mean=1.5, median=1, min=0, max=5
    all: n=30, hits=27, rate=0.900, mean=1.4333, median=1, min=0, max=5
  Split home/away:
    home: n=15, hits=14, rate=0.933, mean=1.2, median=1
    away: n=15, hits=13, rate=0.867, mean=1.6667, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 7. away_goals @ 2.5 — Ukraine (away)
- basis: team_own_stats
- FM reportado: hits 7/7; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.4, median=1, min=1, max=2
-   last10: n=10, hits=9, rate=0.900, mean=1.5, median=1, min=0, max=5
-   all: n=30, hits=27, rate=0.900, mean=1.4333, median=1, min=0, max=5
- Split home/away:
-   home: n=15, hits=14, rate=0.933, mean=1.2, median=1
-   away: n=15, hits=13, rate=0.867, mean=1.6667, median=2
- Rival (contexto equivalente):
  FM reportado: hits 6/6; mejor ventana: no determinable
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=1.4, median=2, min=0, max=2
    last10: n=10, hits=9, rate=0.900, mean=1.2, median=1, min=0, max=3
    all: n=30, hits=25, rate=0.833, mean=1.4667, median=1, min=0, max=6
  Split home/away:
    home: n=15, hits=12, rate=0.800, mean=1.7333, median=1
    away: n=15, hits=13, rate=0.867, mean=1.2, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 8. away_shots_on_target @ 2.5 — Ukraine (away)
- basis: team_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=4.6, median=4, min=3, max=6
-   last10: n=10, hits=9, rate=0.900, mean=4.5, median=4.5, min=0, max=7
-   all: n=30, hits=25, rate=0.833, mean=4.6333, median=4.5, min=0, max=8
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=4.6667, median=4
-   away: n=15, hits=12, rate=0.800, mean=4.6, median=5
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=4.6, median=5, min=4, max=5
    last10: n=10, hits=8, rate=0.800, mean=3.7, median=4, min=1, max=5
    all: n=30, hits=23, rate=0.767, mean=3.8667, median=4, min=0, max=10
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=4.2667, median=4
    away: n=15, hits=10, rate=0.667, mean=3.4667, median=3
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 9. home_cards @ 2.5 — Georgia (home)
- basis: team_own_stats
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.2, median=1, min=1, max=2
-   last10: n=10, hits=10, rate=1.000, mean=1.1, median=1, min=0, max=2
-   all: n=30, hits=25, rate=0.833, mean=1.6333, median=1, min=0, max=4
- Split home/away:
-   home: n=15, hits=12, rate=0.800, mean=1.6, median=1
-   away: n=15, hits=13, rate=0.867, mean=1.6667, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=2.8, median=2, min=1, max=7
    last10: n=10, hits=6, rate=0.600, mean=2.6, median=2, min=0, max=7
    all: n=30, hits=21, rate=0.700, mean=2.0667, median=2, min=0, max=7
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=1.5333, median=2
    away: n=15, hits=8, rate=0.533, mean=2.6, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 10. home_cards @ 3.5 — Georgia (home)
- basis: team_own_stats
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.2, median=1, min=1, max=2
-   last10: n=10, hits=10, rate=1.000, mean=1.1, median=1, min=0, max=2
-   all: n=30, hits=26, rate=0.867, mean=1.6333, median=1, min=0, max=4
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=1.6, median=1
-   away: n=15, hits=13, rate=0.867, mean=1.6667, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=2.8, median=2, min=1, max=7
    last10: n=10, hits=8, rate=0.800, mean=2.6, median=2, min=0, max=7
    all: n=30, hits=25, rate=0.833, mean=2.0667, median=2, min=0, max=7
  Split home/away:
    home: n=15, hits=14, rate=0.933, mean=1.5333, median=2
    away: n=15, hits=11, rate=0.733, mean=2.6, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 11. home_corners @ 3.5 — Georgia (home)
- basis: team_own_stats
- FM reportado: hits 8/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=6.4, median=6, min=4, max=9
-   last10: n=10, hits=9, rate=0.900, mean=5, median=4.5, min=0, max=9
-   all: n=30, hits=20, rate=0.667, mean=4.4667, median=4, min=0, max=11
- Split home/away:
-   home: n=15, hits=12, rate=0.800, mean=5.3333, median=5
-   away: n=15, hits=8, rate=0.533, mean=3.6, median=4
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=2, rate=0.400, mean=4.6, median=3, min=2, max=9
    last10: n=10, hits=5, rate=0.500, mean=4.5, median=3.5, min=0, max=9
    all: n=30, hits=18, rate=0.600, mean=4.8667, median=4, min=0, max=11
  Split home/away:
    home: n=15, hits=9, rate=0.600, mean=5.0667, median=4
    away: n=15, hits=9, rate=0.600, mean=4.6667, median=4
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 12. home_goals @ 0.5 — Georgia (home)
- basis: team_own_stats
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=1.4, median=2, min=0, max=2
-   last10: n=10, hits=7, rate=0.700, mean=1.2, median=1, min=0, max=3
-   all: n=30, hits=24, rate=0.800, mean=1.4667, median=1, min=0, max=6
- Split home/away:
-   home: n=15, hits=12, rate=0.800, mean=1.7333, median=1
-   away: n=15, hits=12, rate=0.800, mean=1.2, median=1
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=1.4, median=1, min=1, max=2
    last10: n=10, hits=8, rate=0.800, mean=1.5, median=1, min=0, max=5
    all: n=30, hits=24, rate=0.800, mean=1.4333, median=1, min=0, max=5
  Split home/away:
    home: n=15, hits=12, rate=0.800, mean=1.2, median=1
    away: n=15, hits=12, rate=0.800, mean=1.6667, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 13. home_goals @ 2.5 — Georgia (home)
- basis: team_own_stats
- FM reportado: hits 9/9; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.4, median=2, min=0, max=2
-   last10: n=10, hits=9, rate=0.900, mean=1.2, median=1, min=0, max=3
-   all: n=30, hits=25, rate=0.833, mean=1.4667, median=1, min=0, max=6
- Split home/away:
-   home: n=15, hits=12, rate=0.800, mean=1.7333, median=1
-   away: n=15, hits=13, rate=0.867, mean=1.2, median=1
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=1.4, median=1, min=1, max=2
    last10: n=10, hits=9, rate=0.900, mean=1.5, median=1, min=0, max=5
    all: n=30, hits=27, rate=0.900, mean=1.4333, median=1, min=0, max=5
  Split home/away:
    home: n=15, hits=14, rate=0.933, mean=1.2, median=1
    away: n=15, hits=13, rate=0.867, mean=1.6667, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 14. home_shots_on_target @ 3.5 — Georgia (home)
- basis: team_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=4.6, median=5, min=4, max=5
-   last10: n=10, hits=6, rate=0.600, mean=3.7, median=4, min=1, max=5
-   all: n=30, hits=17, rate=0.567, mean=3.8667, median=4, min=0, max=10
- Split home/away:
-   home: n=15, hits=10, rate=0.667, mean=4.2667, median=4
-   away: n=15, hits=7, rate=0.467, mean=3.4667, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=4.6, median=4, min=3, max=6
    last10: n=10, hits=7, rate=0.700, mean=4.5, median=4.5, min=0, max=7
    all: n=30, hits=19, rate=0.633, mean=4.6333, median=4.5, min=0, max=8
  Split home/away:
    home: n=15, hits=10, rate=0.667, mean=4.6667, median=4
    away: n=15, hits=9, rate=0.600, mean=4.6, median=5
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 15. home_shots_on_target @ 5.5 — Georgia (home)
- basis: team_own_stats
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=4.6, median=5, min=4, max=5
-   last10: n=10, hits=10, rate=1.000, mean=3.7, median=4, min=1, max=5
-   all: n=30, hits=26, rate=0.867, mean=3.8667, median=4, min=0, max=10
- Split home/away:
-   home: n=15, hits=12, rate=0.800, mean=4.2667, median=4
-   away: n=15, hits=14, rate=0.933, mean=3.4667, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=4.6, median=4, min=3, max=6
    last10: n=10, hits=6, rate=0.600, mean=4.5, median=4.5, min=0, max=7
    all: n=30, hits=19, rate=0.633, mean=4.6333, median=4.5, min=0, max=8
  Split home/away:
    home: n=15, hits=9, rate=0.600, mean=4.6667, median=4
    away: n=15, hits=10, rate=0.667, mean=4.6, median=5
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 16. total_goals @ 1.5 — Georgia vs Ukraine (sin rol)
- basis: match_total_goals
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=2.2, median=2, min=1, max=4
-   last10: n=10, hits=9, rate=0.900, mean=2.8, median=2.5, min=1, max=5
-   all: n=30, hits=25, rate=0.833, mean=2.9333, median=3, min=1, max=7
- Split home/away:
-   home: n=15, hits=12, rate=0.800, mean=2.8667, median=2
-   away: n=15, hits=13, rate=0.867, mean=3, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=2, median=2, min=1, max=4
    last10: n=10, hits=8, rate=0.800, mean=2.9, median=2, min=1, max=8
    all: n=30, hits=24, rate=0.800, mean=2.7667, median=3, min=0, max=8
  Split home/away:
    home: n=15, hits=10, rate=0.667, mean=2, median=2
    away: n=15, hits=14, rate=0.933, mean=3.5333, median=3
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 17. total_goals @ 2.5 — Georgia vs Ukraine (sin rol)
- basis: match_total_goals
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=2.2, median=2, min=1, max=4
-   last10: n=10, hits=5, rate=0.500, mean=2.8, median=2.5, min=1, max=5
-   all: n=30, hits=14, rate=0.467, mean=2.9333, median=3, min=1, max=7
- Split home/away:
-   home: n=15, hits=9, rate=0.600, mean=2.8667, median=2
-   away: n=15, hits=5, rate=0.333, mean=3, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=2, median=2, min=1, max=4
    last10: n=10, hits=6, rate=0.600, mean=2.9, median=2, min=1, max=8
    all: n=30, hits=14, rate=0.467, mean=2.7667, median=3, min=0, max=8
  Split home/away:
    home: n=15, hits=10, rate=0.667, mean=2, median=2
    away: n=15, hits=4, rate=0.267, mean=3.5333, median=3
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 18. total_goals @ 3.5 — Georgia vs Ukraine (sin rol)
- basis: match_total_goals
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=2.2, median=2, min=1, max=4
-   last10: n=10, hits=7, rate=0.700, mean=2.8, median=2.5, min=1, max=5
-   all: n=30, hits=20, rate=0.667, mean=2.9333, median=3, min=1, max=7
- Split home/away:
-   home: n=15, hits=10, rate=0.667, mean=2.8667, median=2
-   away: n=15, hits=10, rate=0.667, mean=3, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=2, median=2, min=1, max=4
    last10: n=10, hits=7, rate=0.700, mean=2.9, median=2, min=1, max=8
    all: n=30, hits=22, rate=0.733, mean=2.7667, median=3, min=0, max=8
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=2, median=2
    away: n=15, hits=9, rate=0.600, mean=3.5333, median=3
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 19. 1x2_cards — Ukraine (away)
- basis: unmapped
- FM reportado: hits 8/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=true, source_conflict=false, motivo="non-numeric history value 'vt'; market '1x2_cards' has no mapping to fm_team_matches.team_stats_json"

### 20. total_cards @ 3.5 — Georgia vs Ukraine (sin rol)
- basis: not_reproducible
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 21. total_cards @ 4.5 — Georgia vs Ukraine (sin rol)
- basis: not_reproducible
- FM reportado: hits 6/7; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 22. total_cards @ 5.5 — Georgia vs Ukraine (sin rol)
- basis: not_reproducible
- FM reportado: hits 7/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 23. total_corners @ 5.5 — Georgia vs Ukraine (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 24. total_corners @ 6.5 — Georgia vs Ukraine (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 25. total_corners @ 7.5 — Georgia vs Ukraine (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 26. total_corners @ 8.5 — Georgia vs Ukraine (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 27. total_corners @ 11.5 — Georgia vs Ukraine (sin rol)
- basis: not_reproducible
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 28. total_corners @ 12.5 — Georgia vs Ukraine (sin rol)
- basis: not_reproducible
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 29. total_shots_on_target @ 6.5 — Georgia vs Ukraine (sin rol)
- basis: not_reproducible
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 30. total_shots_on_target @ 8.5 — Georgia vs Ukraine (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 31. total_shots_on_target @ 9.5 — Georgia vs Ukraine (sin rol)
- basis: not_reproducible
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

## Señales de jugadores (orden: mayor muestra disponible, no fuerza de señal)

### 1. shots @ 1.5 — K. Kvaratskhelia
- basis: player_own_stats
- FM reportado: hits 8/10; mejor ventana: 10 (8/10)
- Ventanas propias:
  last5: n=5, hits=1, rate=0.200, mean=0.8, median=0, min=0, max=3
  last10: n=10, hits=5, rate=0.500, mean=1.6, median=1.5, min=0, max=4
  all: n=27, hits=17, rate=0.630, mean=2.5926, median=3, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 2. shots @ 2.5 — K. Kvaratskhelia
- basis: player_own_stats
- FM reportado: hits 7/10; mejor ventana: 10 (7/10)
- Ventanas propias:
  last5: n=5, hits=1, rate=0.200, mean=0.8, median=0, min=0, max=3
  last10: n=10, hits=4, rate=0.400, mean=1.6, median=1.5, min=0, max=4
  all: n=27, hits=15, rate=0.556, mean=2.5926, median=3, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 3. shots_on_target @ 0.5 — K. Kvaratskhelia
- basis: player_own_stats
- FM reportado: hits 8/10; mejor ventana: 10 (8/10)
- Ventanas propias:
  last5: n=5, hits=2, rate=0.400, mean=0.6, median=0, min=0, max=2
  last10: n=10, hits=5, rate=0.500, mean=0.6, median=0.5, min=0, max=2
  all: n=27, hits=13, rate=0.482, mean=0.6667, median=0, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 4. foul_involvements @ 1.5 — A. Mekvabishvili
- basis: player_own_stats
- FM reportado: hits 3/5; mejor ventana: 5 (3/5)
- Ventanas propias:
  last5: n=5, hits=2, rate=0.400, mean=1.4, median=1, min=0, max=4
  last10: n=10, hits=4, rate=0.400, mean=1.7, median=1, min=0, max=6
  all: n=12, hits=5, rate=0.417, mean=1.6667, median=1, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 5. fouls_committed @ 1.5 — A. Mekvabishvili
- basis: player_own_stats
- FM reportado: hits 3/5; mejor ventana: 5 (3/5)
- Ventanas propias:
  last5: n=5, hits=2, rate=0.400, mean=1.4, median=1, min=0, max=4
  last10: n=10, hits=3, rate=0.300, mean=1.3, median=0.5, min=0, max=5
  all: n=12, hits=4, rate=0.333, mean=1.25, median=0.5, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 6. foul_involvements @ 1.5 — G. Kochorashvili
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=3, median=2, min=0, max=7
  last10: n=10, hits=6, rate=0.600, mean=3, median=2, min=0, max=7
  all: n=10, hits=6, rate=0.600, mean=3, median=2, min=0, max=7
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 7. foul_involvements @ 1.5 — S. Goglichidze
- basis: player_own_stats
- FM reportado: hits 6/8; mejor ventana: 10 (6/8)
- Ventanas propias:
  last5: n=5, hits=2, rate=0.400, mean=1.4, median=0, min=0, max=4
  last10: n=10, hits=6, rate=0.600, mean=1.9, median=2.5, min=0, max=4
  all: n=10, hits=6, rate=0.600, mean=1.9, median=2.5, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 8. foul_involvements @ 1.5 — G. Mikautadze
- basis: player_own_stats
- FM reportado: hits 4/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=2, rate=0.400, mean=0.8, median=0, min=0, max=2
  last10: n=9, hits=4, rate=0.444, mean=1.1111, median=1, min=0, max=3
  all: n=9, hits=4, rate=0.444, mean=1.1111, median=1, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 9. foul_involvements @ 1.5 — O. Kakabadze
- basis: player_own_stats
- FM reportado: hits 4/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1.4, median=2, min=0, max=2
  last10: n=7, hits=4, rate=0.571, mean=1.4286, median=2, min=0, max=2
  all: n=7, hits=4, rate=0.571, mean=1.4286, median=2, min=0, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 10. foul_involvements @ 1.5 — V. Tsygankov
- basis: player_own_stats
- FM reportado: hits 3/5; mejor ventana: 5 (3/5)
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=3, median=2, min=0, max=7
  last10: n=6, hits=5, rate=0.833, mean=2.8333, median=2, min=0, max=7
  all: n=6, hits=5, rate=0.833, mean=2.8333, median=2, min=0, max=7
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 11. foul_involvements @ 1.5 — V. Vanat
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1.6, median=2, min=0, max=3
  last10: n=6, hits=4, rate=0.667, mean=1.8333, median=2, min=0, max=3
  all: n=6, hits=4, rate=0.667, mean=1.8333, median=2, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 12. foul_involvements @ 1.5 — O. Ocheretko
- basis: player_own_stats
- FM reportado: hits 3/5; mejor ventana: 5 (3/5)
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.2, median=3, min=0, max=3
  last10: n=5, hits=4, rate=0.800, mean=2.2, median=3, min=0, max=3
  all: n=5, hits=4, rate=0.800, mean=2.2, median=3, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 13. foul_involvements @ 2.5 — O. Ocheretko
- basis: player_own_stats
- FM reportado: hits 3/5; mejor ventana: 5 (3/5)
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=2.2, median=3, min=0, max=3
  last10: n=5, hits=3, rate=0.600, mean=2.2, median=3, min=0, max=3
  all: n=5, hits=3, rate=0.600, mean=2.2, median=3, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 14. tackles @ 1.5 — O. Ocheretko
- basis: player_own_stats
- FM reportado: hits 4/5; mejor ventana: 5 (4/5)
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=3.8, median=5, min=0, max=6
  last10: n=5, hits=4, rate=0.800, mean=3.8, median=5, min=0, max=6
  all: n=5, hits=4, rate=0.800, mean=3.8, median=5, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

## Evidencia histórica (ventanas fijas)
- observaciones descriptivas: secuencia cruda (más antiguo -> más reciente) y hits de las ventanas fijas (last5/last10/all)

### Jugadores — evidencia con ventanas fijas
- K. Kvaratskhelia — shots @ 1.5: [3,0,4,2,3,3,0,0,0,1] | last5 1/5 | last10 5/10 | all 17/27
- K. Kvaratskhelia — shots @ 2.5: [3,0,4,2,3,3,0,0,0,1] | last5 1/5 | last10 4/10 | all 15/27
- K. Kvaratskhelia — shots_on_target @ 0.5: [1,0,1,1,0,2,0,0,0,1] | last5 2/5 | last10 5/10 | all 13/27
- A. Mekvabishvili — foul_involvements @ 1.5: [0,1,3,0,6,2,1,0,0,4] | last5 2/5 | last10 4/10 | all 5/12
- A. Mekvabishvili — fouls_committed @ 1.5: [0,0,1,0,5,2,1,0,0,4] | last5 2/5 | last10 3/10 | all 4/12
- G. Kochorashvili — foul_involvements @ 1.5: [2,1,5,0,7,7,2,0,0,6] | last5 3/5 | last10 6/10 | all 6/10
- S. Goglichidze — foul_involvements @ 1.5: [3,3,3,2,1,3,0,0,0,4] | last5 2/5 | last10 6/10 | all 6/10
- G. Mikautadze — foul_involvements @ 1.5: [0,1,2,3,2,0,0,0,2] | last5 2/5 | last10 4/9 | all 4/9
- O. Kakabadze — foul_involvements @ 1.5: [1,2,2,2,1,0,2] | last5 3/5 | last10 4/7 | all 4/7
- V. Tsygankov — foul_involvements @ 1.5: [2,7,2,2,0,4] | last5 4/5 | last10 5/6 | all 5/6
- V. Vanat — foul_involvements @ 1.5: [3,1,0,2,3,2] | last5 3/5 | last10 4/6 | all 4/6
- O. Ocheretko — foul_involvements @ 1.5: [2,3,3,0,3] | last5 4/5 | last10 4/5 | all 4/5
- O. Ocheretko — foul_involvements @ 2.5: [2,3,3,0,3] | last5 3/5 | last10 3/5 | all 3/5
- O. Ocheretko — tackles @ 1.5: [5,2,6,0,6] | last5 4/5 | last10 4/5 | all 4/5

### Equipos — evidencia con ventanas fijas
- Ukraine — away_cards @ 1.5: [0,3,4,3,2,2,2,1,2,7] | last5 4/5 | last10 8/10 | all 19/30
  - Rival (Georgia): [2,0,1,0,2,1,1,2,1,1] | last5 1/5 | last10 3/10 | all 14/30
- Ukraine — away_cards @ 2.5: [0,3,4,3,2,2,2,1,2,7] | last5 4/5 | last10 6/10 | all 21/30
  - Rival (Georgia): [2,0,1,0,2,1,1,2,1,1] | last5 5/5 | last10 10/10 | all 25/30
- Ukraine — away_cards @ 3.5: [0,3,4,3,2,2,2,1,2,7] | last5 4/5 | last10 8/10 | all 25/30
  - Rival (Georgia): [2,0,1,0,2,1,1,2,1,1] | last5 5/5 | last10 10/10 | all 26/30
- Ukraine — away_goals @ 0.5: [0,1,5,2,0,2,1,1,2,1] | last5 5/5 | last10 8/10 | all 24/30
  - Rival (Georgia): [3,0,1,0,1,2,2,1,2,0] | last5 4/5 | last10 7/10 | all 24/30
- Georgia — away_goals @ 1.5: [3,0,1,0,1,2,2,1,2,0] | last5 2/5 | last10 6/10 | all 19/30
  - Rival (Ukraine): [0,1,5,2,0,2,1,1,2,1] | last5 3/5 | last10 6/10 | all 17/30
- Georgia — away_goals @ 2.5: [3,0,1,0,1,2,2,1,2,0] | last5 5/5 | last10 9/10 | all 25/30
  - Rival (Ukraine): [0,1,5,2,0,2,1,1,2,1] | last5 5/5 | last10 9/10 | all 27/30
- Ukraine — away_goals @ 2.5: [0,1,5,2,0,2,1,1,2,1] | last5 5/5 | last10 9/10 | all 27/30
  - Rival (Georgia): [3,0,1,0,1,2,2,1,2,0] | last5 5/5 | last10 9/10 | all 25/30
- Ukraine — away_shots_on_target @ 2.5: [3,5,7,7,0,6,4,6,4,3] | last5 5/5 | last10 9/10 | all 25/30
  - Rival (Georgia): [5,1,3,2,3,4,5,4,5,5] | last5 5/5 | last10 8/10 | all 23/30
- Georgia — home_cards @ 2.5: [2,0,1,0,2,1,1,2,1,1] | last5 5/5 | last10 10/10 | all 25/30
  - Rival (Ukraine): [0,3,4,3,2,2,2,1,2,7] | last5 4/5 | last10 6/10 | all 21/30
- Georgia — home_cards @ 3.5: [2,0,1,0,2,1,1,2,1,1] | last5 5/5 | last10 10/10 | all 26/30
  - Rival (Ukraine): [0,3,4,3,2,2,2,1,2,7] | last5 4/5 | last10 8/10 | all 25/30
- Georgia — home_corners @ 3.5: [4,0,6,4,4,6,4,5,8,9] | last5 5/5 | last10 9/10 | all 20/30
  - Rival (Ukraine): [4,9,2,7,0,3,9,3,6,2] | last5 2/5 | last10 5/10 | all 18/30
- Georgia — home_goals @ 0.5: [3,0,1,0,1,2,2,1,2,0] | last5 4/5 | last10 7/10 | all 24/30
  - Rival (Ukraine): [0,1,5,2,0,2,1,1,2,1] | last5 5/5 | last10 8/10 | all 24/30
- Georgia — home_goals @ 2.5: [3,0,1,0,1,2,2,1,2,0] | last5 5/5 | last10 9/10 | all 25/30
  - Rival (Ukraine): [0,1,5,2,0,2,1,1,2,1] | last5 5/5 | last10 9/10 | all 27/30
- Georgia — home_shots_on_target @ 3.5: [5,1,3,2,3,4,5,4,5,5] | last5 5/5 | last10 6/10 | all 17/30
  - Rival (Ukraine): [3,5,7,7,0,6,4,6,4,3] | last5 4/5 | last10 7/10 | all 19/30
- Georgia — home_shots_on_target @ 5.5: [5,1,3,2,3,4,5,4,5,5] | last5 5/5 | last10 10/10 | all 26/30
  - Rival (Ukraine): [3,5,7,7,0,6,4,6,4,3] | last5 3/5 | last10 6/10 | all 19/30
- Georgia vs Ukraine — total_goals @ 1.5: [3,2,5,4,3,4,2,2,2,1] | last5 4/5 | last10 9/10 | all 25/30
- Georgia vs Ukraine — total_goals @ 2.5: [3,2,5,4,3,4,2,2,2,1] | last5 4/5 | last10 5/10 | all 14/30
- Georgia vs Ukraine — total_goals @ 3.5: [3,2,5,4,3,4,2,2,2,1] | last5 4/5 | last10 7/10 | all 20/30

## Confluencia descriptiva (equipo + rival + jugadores)
- familias de mercado con 2 o más series observadas en la DB; cada serie conserva su línea y sus ventanas fijas (last5/last10/all); sin score ni orden por fuerza

### cards
- Ukraine (away) @ 1.5: last5 4/5 | last10 8/10 | all 19/30
- Georgia (home) @ 1.5: last5 1/5 | last10 3/10 | all 14/30
- Ukraine (away) @ 2.5: last5 4/5 | last10 6/10 | all 21/30
- Georgia (home) @ 2.5: last5 5/5 | last10 10/10 | all 25/30
- Ukraine (away) @ 3.5: last5 4/5 | last10 8/10 | all 25/30
- Georgia (home) @ 3.5: last5 5/5 | last10 10/10 | all 26/30

### corners
- Georgia (home) @ 3.5: last5 5/5 | last10 9/10 | all 20/30
- Ukraine (away) @ 3.5: last5 2/5 | last10 5/10 | all 18/30

### foul_involvements
- jugador A. Mekvabishvili — foul_involvements @ 1.5: last5 2/5 | last10 4/10 | all 5/12
- jugador G. Kochorashvili — foul_involvements @ 1.5: last5 3/5 | last10 6/10 | all 6/10
- jugador S. Goglichidze — foul_involvements @ 1.5: last5 2/5 | last10 6/10 | all 6/10
- jugador G. Mikautadze — foul_involvements @ 1.5: last5 2/5 | last10 4/9 | all 4/9
- jugador O. Kakabadze — foul_involvements @ 1.5: last5 3/5 | last10 4/7 | all 4/7
- jugador V. Tsygankov — foul_involvements @ 1.5: last5 4/5 | last10 5/6 | all 5/6
- jugador V. Vanat — foul_involvements @ 1.5: last5 3/5 | last10 4/6 | all 4/6
- jugador O. Ocheretko — foul_involvements @ 1.5: last5 4/5 | last10 4/5 | all 4/5
- jugador O. Ocheretko — foul_involvements @ 2.5: last5 3/5 | last10 3/5 | all 3/5

### goals
- Ukraine (away) @ 0.5: last5 5/5 | last10 8/10 | all 24/30
- Georgia (home) @ 0.5: last5 4/5 | last10 7/10 | all 24/30
- Georgia (home) @ 1.5: last5 2/5 | last10 6/10 | all 19/30
- Ukraine (away) @ 1.5: last5 3/5 | last10 6/10 | all 17/30
- Georgia (home) @ 2.5: last5 5/5 | last10 9/10 | all 25/30
- Ukraine (away) @ 2.5: last5 5/5 | last10 9/10 | all 27/30

### shots
- jugador K. Kvaratskhelia — shots @ 1.5: last5 1/5 | last10 5/10 | all 17/27
- jugador K. Kvaratskhelia — shots @ 2.5: last5 1/5 | last10 4/10 | all 15/27

### shots_on_target
- Ukraine (away) @ 2.5: last5 5/5 | last10 9/10 | all 25/30
- Georgia (home) @ 2.5: last5 5/5 | last10 8/10 | all 23/30
- Georgia (home) @ 3.5: last5 5/5 | last10 6/10 | all 17/30
- Ukraine (away) @ 3.5: last5 4/5 | last10 7/10 | all 19/30
- Georgia (home) @ 5.5: last5 5/5 | last10 10/10 | all 26/30
- Ukraine (away) @ 5.5: last5 3/5 | last10 6/10 | all 19/30
- jugador K. Kvaratskhelia — shots_on_target @ 0.5: last5 2/5 | last10 5/10 | all 13/27

## Cuotas

| mercado | línea | casa | lado | cuota | prob. implícita | capturada |
|---|---|---|---|---|---|---|
| away_cards | 1.5 | Ladbrokes | over | 1.3 | 0.769 | 2026-09-28T21:36:10.168Z |
| away_cards | 1.5 | Ladbrokes | under | 3.25 | 0.308 | 2026-09-28T21:36:10.168Z |
| away_cards | 2.5 | Bet365 | over | 1.8 | 0.556 | 2026-09-28T21:36:10.168Z |
| away_cards | 2.5 | Bet365 | under | 1.91 | 0.524 | 2026-09-28T21:36:10.168Z |
| away_cards | 2.5 | Ladbrokes | over | 2 | 0.500 | 2026-09-28T21:36:10.168Z |
| away_cards | 2.5 | Ladbrokes | under | 1.7 | 0.588 | 2026-09-28T21:36:10.168Z |
| away_goals | 0.5 | Bet365 | over | 1.36 | 0.735 | 2026-09-28T21:36:10.168Z |
| away_goals | 0.5 | Bet365 | under | 3 | 0.333 | 2026-09-28T21:36:10.168Z |
| away_goals | 0.5 | Kambi | over | 1.36 | 0.735 | 2026-09-28T21:36:10.168Z |
| away_goals | 0.5 | Kambi | under | 2.85 | 0.351 | 2026-09-28T21:36:10.168Z |
| away_goals | 0.5 | Ladbrokes | over | 1.35 | 0.741 | 2026-09-28T21:36:10.168Z |
| away_goals | 0.5 | Ladbrokes | under | 3 | 0.333 | 2026-09-28T21:36:10.168Z |
| away_goals | 1.5 | Bet365 | over | 2.75 | 0.364 | 2026-09-28T21:36:10.168Z |
| away_goals | 1.5 | Bet365 | under | 1.4 | 0.714 | 2026-09-28T21:36:10.168Z |
| away_goals | 1.5 | Kambi | over | 2.8 | 0.357 | 2026-09-28T21:36:10.168Z |
| away_goals | 1.5 | Kambi | under | 1.36 | 0.735 | 2026-09-28T21:36:10.168Z |
| away_goals | 1.5 | Ladbrokes | over | 2.7 | 0.370 | 2026-09-28T21:36:10.168Z |
| away_goals | 1.5 | Ladbrokes | under | 1.4 | 0.714 | 2026-09-28T21:36:10.168Z |
| away_goals | 2.5 | Bet365 | over | 7 | 0.143 | 2026-09-28T21:36:10.168Z |
| away_goals | 2.5 | Bet365 | under | 1.1 | 0.909 | 2026-09-28T21:36:10.168Z |
| away_goals | 2.5 | Kambi | over | 6.75 | 0.148 | 2026-09-28T21:36:10.168Z |
| away_goals | 2.5 | Kambi | under | 1.05 | 0.952 | 2026-09-28T21:36:10.168Z |
| away_goals | 2.5 | Ladbrokes | over | 6.5 | 0.154 | 2026-09-28T21:36:10.168Z |
| away_goals | 2.5 | Ladbrokes | under | 1.08 | 0.926 | 2026-09-28T21:36:10.168Z |
| away_goals | 2.5 | Bet365 | over | 7 | 0.143 | 2026-09-28T21:36:10.168Z |
| away_goals | 2.5 | Bet365 | under | 1.1 | 0.909 | 2026-09-28T21:36:10.168Z |
| away_goals | 2.5 | Kambi | over | 6.75 | 0.148 | 2026-09-28T21:36:10.168Z |
| away_goals | 2.5 | Kambi | under | 1.05 | 0.952 | 2026-09-28T21:36:10.168Z |
| away_goals | 2.5 | Ladbrokes | over | 6.5 | 0.154 | 2026-09-28T21:36:10.168Z |
| away_goals | 2.5 | Ladbrokes | under | 1.08 | 0.926 | 2026-09-28T21:36:10.168Z |
| away_shots_on_target | 2.5 | Kambi | over | 1.28 | 0.781 | 2026-09-28T21:36:10.168Z |
| away_shots_on_target | 2.5 | Kambi | under | 2.8 | 0.357 | 2026-09-28T21:36:10.168Z |
| home_cards | 2.5 | Bet365 | over | 2.2 | 0.455 | 2026-09-28T21:36:10.168Z |
| home_cards | 2.5 | Bet365 | under | 1.62 | 0.617 | 2026-09-28T21:36:10.168Z |
| home_cards | 2.5 | Ladbrokes | over | 2.1 | 0.476 | 2026-09-28T21:36:10.168Z |
| home_cards | 2.5 | Ladbrokes | under | 1.67 | 0.599 | 2026-09-28T21:36:10.168Z |
| home_corners | 3.5 | Kambi | over | 1.38 | 0.725 | 2026-09-28T21:36:10.168Z |
| home_corners | 3.5 | Kambi | under | 2.63 | 0.380 | 2026-09-28T21:36:10.168Z |
| home_goals | 0.5 | Bet365 | over | 1.36 | 0.735 | 2026-09-28T21:36:10.168Z |
| home_goals | 0.5 | Bet365 | under | 3 | 0.333 | 2026-09-28T21:36:10.168Z |
| home_goals | 0.5 | Kambi | over | 1.22 | 0.820 | 2026-09-28T21:36:10.168Z |
| home_goals | 0.5 | Kambi | under | 3.9 | 0.256 | 2026-09-28T21:36:10.168Z |
| home_goals | 0.5 | Ladbrokes | over | 1.18 | 0.847 | 2026-09-28T21:36:10.168Z |
| home_goals | 0.5 | Ladbrokes | under | 4.2 | 0.238 | 2026-09-28T21:36:10.168Z |
| home_goals | 2.5 | Bet365 | over | 7 | 0.143 | 2026-09-28T21:36:10.168Z |
| home_goals | 2.5 | Bet365 | under | 1.1 | 0.909 | 2026-09-28T21:36:10.168Z |
| home_goals | 2.5 | Kambi | over | 4.4 | 0.227 | 2026-09-28T21:36:10.168Z |
| home_goals | 2.5 | Kambi | under | 1.17 | 0.855 | 2026-09-28T21:36:10.168Z |
| home_goals | 2.5 | Ladbrokes | over | 4 | 0.250 | 2026-09-28T21:36:10.168Z |
| home_goals | 2.5 | Ladbrokes | under | 1.2 | 0.833 | 2026-09-28T21:36:10.168Z |
| home_shots_on_target | 3.5 | Kambi | over | 1.35 | 0.741 | 2026-09-28T21:36:10.168Z |
| home_shots_on_target | 3.5 | Kambi | under | 2.5 | 0.400 | 2026-09-28T21:36:10.168Z |
| home_shots_on_target | 5.5 | Kambi | over | 2.6 | 0.385 | 2026-09-28T21:36:10.168Z |
| home_shots_on_target | 5.5 | Kambi | under | 1.33 | 0.752 | 2026-09-28T21:36:10.168Z |
| total_goals | 1.5 | Bet365 | over | 1.25 | 0.800 | 2026-09-28T21:36:10.168Z |
| total_goals | 1.5 | Bet365 | under | 3.75 | 0.267 | 2026-09-28T21:36:10.168Z |
| total_goals | 1.5 | Kambi | over | 1.29 | 0.775 | 2026-09-28T21:36:10.168Z |
| total_goals | 1.5 | Kambi | under | 3.45 | 0.290 | 2026-09-28T21:36:10.168Z |
| total_goals | 1.5 | Ladbrokes | over | 1.22 | 0.820 | 2026-09-28T21:36:10.168Z |
| total_goals | 1.5 | Ladbrokes | under | 3.75 | 0.267 | 2026-09-28T21:36:10.168Z |
| total_goals | 1.5 | Paddy Power | over | 1.2 | 0.833 | 2026-09-28T21:36:10.168Z |
| total_goals | 1.5 | Paddy Power | under | 4 | 0.250 | 2026-09-28T21:36:10.168Z |
| total_goals | 2.5 | Altenar | over | 1.87 | 0.535 | 2026-09-28T21:36:10.168Z |
| total_goals | 2.5 | Altenar | under | 1.8 | 0.556 | 2026-09-28T21:36:10.168Z |
| total_goals | 2.5 | Bet365 | over | 2.2 | 0.455 | 2026-09-28T21:36:10.168Z |
| total_goals | 2.5 | Bet365 | under | 1.65 | 0.606 | 2026-09-28T21:36:10.168Z |
| total_goals | 2.5 | Kambi | over | 1.91 | 0.524 | 2026-09-28T21:36:10.168Z |
| total_goals | 2.5 | Kambi | under | 1.86 | 0.538 | 2026-09-28T21:36:10.168Z |
| total_goals | 2.5 | Ladbrokes | over | 1.75 | 0.571 | 2026-09-28T21:36:10.168Z |
| total_goals | 2.5 | Ladbrokes | under | 1.95 | 0.513 | 2026-09-28T21:36:10.168Z |
| total_goals | 2.5 | Paddy Power | over | 1.75 | 0.571 | 2026-09-28T21:36:10.168Z |
| total_goals | 2.5 | Paddy Power | under | 1.95 | 0.513 | 2026-09-28T21:36:10.168Z |
| total_goals | 3.5 | Bet365 | over | 3.25 | 0.308 | 2026-09-28T21:36:10.168Z |
| total_goals | 3.5 | Bet365 | under | 1.33 | 0.752 | 2026-09-28T21:36:10.168Z |
| total_goals | 3.5 | Kambi | over | 3.25 | 0.308 | 2026-09-28T21:36:10.168Z |
| total_goals | 3.5 | Kambi | under | 1.32 | 0.758 | 2026-09-28T21:36:10.168Z |
| total_goals | 3.5 | Ladbrokes | over | 2.9 | 0.345 | 2026-09-28T21:36:10.168Z |
| total_goals | 3.5 | Ladbrokes | under | 1.36 | 0.735 | 2026-09-28T21:36:10.168Z |
| total_goals | 3.5 | Paddy Power | over | 3 | 0.333 | 2026-09-28T21:36:10.168Z |
| total_goals | 3.5 | Paddy Power | under | 1.33 | 0.752 | 2026-09-28T21:36:10.168Z |
| total_cards | 3.5 | Ladbrokes | over | 1.3 | 0.769 | 2026-09-28T21:36:10.168Z |
| total_cards | 3.5 | Ladbrokes | under | 3.25 | 0.308 | 2026-09-28T21:36:10.168Z |
| total_cards | 3.5 | Paddy Power | over | 1.29 | 0.775 | 2026-09-28T21:36:10.168Z |
| total_cards | 3.5 | Paddy Power | under | 3.2 | 0.312 | 2026-09-28T21:36:10.168Z |
| total_cards | 4.5 | Bet365 | over | 1.62 | 0.617 | 2026-09-28T21:36:10.168Z |
| total_cards | 4.5 | Bet365 | under | 2.2 | 0.455 | 2026-09-28T21:36:10.168Z |
| total_cards | 4.5 | Kambi | over | 1.61 | 0.621 | 2026-09-28T21:36:10.168Z |
| total_cards | 4.5 | Kambi | under | 2.02 | 0.495 | 2026-09-28T21:36:10.168Z |
| total_cards | 4.5 | Ladbrokes | over | 1.67 | 0.599 | 2026-09-28T21:36:10.168Z |
| total_cards | 4.5 | Ladbrokes | under | 2.05 | 0.488 | 2026-09-28T21:36:10.168Z |
| total_cards | 4.5 | Paddy Power | over | 1.73 | 0.578 | 2026-09-28T21:36:10.168Z |
| total_cards | 4.5 | Paddy Power | under | 1.95 | 0.513 | 2026-09-28T21:36:10.168Z |
| total_cards | 5.5 | Kambi | over | 2.12 | 0.472 | 2026-09-28T21:36:10.168Z |
| total_cards | 5.5 | Kambi | under | 1.55 | 0.645 | 2026-09-28T21:36:10.168Z |
| total_cards | 5.5 | Ladbrokes | over | 2.4 | 0.417 | 2026-09-28T21:36:10.168Z |
| total_cards | 5.5 | Ladbrokes | under | 1.5 | 0.667 | 2026-09-28T21:36:10.168Z |
| total_corners | 5.5 | Bet365 | over | 1.11 | 0.901 | 2026-09-28T21:36:10.168Z |
| total_corners | 5.5 | Bet365 | under | 6.5 | 0.154 | 2026-09-28T21:36:10.168Z |
| total_corners | 5.5 | Kambi | over | 1.1 | 0.909 | 2026-09-28T21:36:10.168Z |
| total_corners | 5.5 | Kambi | under | 5.5 | 0.182 | 2026-09-28T21:36:10.168Z |
| total_corners | 6.5 | Bet365 | over | 1.25 | 0.800 | 2026-09-28T21:36:10.168Z |
| total_corners | 6.5 | Kambi | over | 1.22 | 0.820 | 2026-09-28T21:36:10.168Z |
| total_corners | 6.5 | Kambi | under | 3.7 | 0.270 | 2026-09-28T21:36:10.168Z |
| total_corners | 6.5 | Ladbrokes | over | 1.22 | 0.820 | 2026-09-28T21:36:10.168Z |
| total_corners | 6.5 | Ladbrokes | under | 3.75 | 0.267 | 2026-09-28T21:36:10.168Z |
| total_corners | 7.5 | Kambi | over | 1.41 | 0.709 | 2026-09-28T21:36:10.168Z |
| total_corners | 7.5 | Kambi | under | 2.63 | 0.380 | 2026-09-28T21:36:10.168Z |
| total_corners | 7.5 | Ladbrokes | over | 1.44 | 0.694 | 2026-09-28T21:36:10.168Z |
| total_corners | 7.5 | Ladbrokes | under | 2.5 | 0.400 | 2026-09-28T21:36:10.168Z |
| total_corners | 8.5 | Bet365 | over | 1.83 | 0.546 | 2026-09-28T21:36:10.168Z |
| total_corners | 8.5 | Bet365 | under | 1.98 | 0.505 | 2026-09-28T21:36:10.168Z |
| total_corners | 8.5 | Kambi | over | 1.68 | 0.595 | 2026-09-28T21:36:10.168Z |
| total_corners | 8.5 | Kambi | under | 2 | 0.500 | 2026-09-28T21:36:10.168Z |
| total_corners | 8.5 | Ladbrokes | over | 1.8 | 0.556 | 2026-09-28T21:36:10.168Z |
| total_corners | 8.5 | Ladbrokes | under | 1.91 | 0.524 | 2026-09-28T21:36:10.168Z |
| total_corners | 8.5 | Paddy Power | over | 1.67 | 0.599 | 2026-09-28T21:36:10.168Z |
| total_corners | 8.5 | Paddy Power | under | 2.05 | 0.488 | 2026-09-28T21:36:10.168Z |
| total_corners | 11.5 | Kambi | over | 3.65 | 0.274 | 2026-09-28T21:36:10.168Z |
| total_corners | 11.5 | Kambi | under | 1.22 | 0.820 | 2026-09-28T21:36:10.168Z |
| total_corners | 11.5 | Paddy Power | over | 4.33 | 0.231 | 2026-09-28T21:36:10.168Z |
| total_corners | 11.5 | Paddy Power | under | 1.17 | 0.855 | 2026-09-28T21:36:10.168Z |
| total_corners | 12.5 | Kambi | over | 4.9 | 0.204 | 2026-09-28T21:36:10.168Z |
| total_corners | 12.5 | Kambi | under | 1.13 | 0.885 | 2026-09-28T21:36:10.168Z |
| total_corners | 12.5 | Paddy Power | over | 6.5 | 0.154 | 2026-09-28T21:36:10.168Z |
| total_corners | 12.5 | Paddy Power | under | 1.07 | 0.935 | 2026-09-28T21:36:10.168Z |
| total_shots_on_target | 6.5 | Kambi | over | 1.28 | 0.781 | 2026-09-28T21:36:10.168Z |
| total_shots_on_target | 6.5 | Kambi | under | 3 | 0.333 | 2026-09-28T21:36:10.168Z |
| total_shots_on_target | 8.5 | Kambi | over | 1.95 | 0.513 | 2026-09-28T21:36:10.168Z |
| total_shots_on_target | 8.5 | Kambi | under | 1.67 | 0.599 | 2026-09-28T21:36:10.168Z |
| total_shots_on_target | 9.5 | Kambi | over | 2.6 | 0.385 | 2026-09-28T21:36:10.168Z |
| total_shots_on_target | 9.5 | Kambi | under | 1.38 | 0.725 | 2026-09-28T21:36:10.168Z |
- Cuota manual Betano (away_cards @ 1.5): no cargada
- Cuota manual Betano (away_cards @ 2.5): no cargada
- Cuota manual Betano (away_cards @ 3.5): no cargada
- Cuota manual Betano (away_goals @ 0.5): no cargada
- Cuota manual Betano (away_goals @ 1.5): no cargada
- Cuota manual Betano (away_goals @ 2.5): no cargada
- Cuota manual Betano (away_goals @ 2.5): no cargada
- Cuota manual Betano (away_shots_on_target @ 2.5): no cargada
- Cuota manual Betano (home_cards @ 2.5): no cargada
- Cuota manual Betano (home_cards @ 3.5): no cargada
- Cuota manual Betano (home_corners @ 3.5): no cargada
- Cuota manual Betano (home_goals @ 0.5): no cargada
- Cuota manual Betano (home_goals @ 2.5): no cargada
- Cuota manual Betano (home_shots_on_target @ 3.5): no cargada
- Cuota manual Betano (home_shots_on_target @ 5.5): no cargada
- Cuota manual Betano (total_goals @ 1.5): no cargada
- Cuota manual Betano (total_goals @ 2.5): no cargada
- Cuota manual Betano (total_goals @ 3.5): no cargada
- Cuota manual Betano (1x2_cards): no cargada
- Cuota manual Betano (total_cards @ 3.5): no cargada
- Cuota manual Betano (total_cards @ 4.5): no cargada
- Cuota manual Betano (total_cards @ 5.5): no cargada
- Cuota manual Betano (total_corners @ 5.5): no cargada
- Cuota manual Betano (total_corners @ 6.5): no cargada
- Cuota manual Betano (total_corners @ 7.5): no cargada
- Cuota manual Betano (total_corners @ 8.5): no cargada
- Cuota manual Betano (total_corners @ 11.5): no cargada
- Cuota manual Betano (total_corners @ 12.5): no cargada
- Cuota manual Betano (total_shots_on_target @ 6.5): no cargada
- Cuota manual Betano (total_shots_on_target @ 8.5): no cargada
- Cuota manual Betano (total_shots_on_target @ 9.5): no cargada

## Nota de cierre
Este reporte es descriptivo. No constituye una recomendación de apuesta ni una probabilidad validada de resultado futuro.

---

## Latvia vs Cyprus (33662308) - kickoff 2026-09-28T16:00:00.000Z

POST-PARTIDO - SOLO REGISTRO HISTORICO, NO ACCIONABLE

- Motivo de la marca: partido ya jugado (kickoff 2026-09-28T16:00:00.000Z); las cuotas en fm_market_odds son de cierre y las senales pueden reflejar el resultado ya conocido. Uso unico: dataset historico para calibracion/backtest.
- Snapshot capturado post-partido: 21:36:28Z-21:36:55Z (tabs overview/player-trends/team-trends, 3 navs; +2 fetch de scope partidos sin nav).
- No intentado - pausa del operador por lectura de un ledger manual desactualizado (docs/STATE.md); limite real de la API: 40 navs/corrida, 24 necesarias para los 8 fixtures. Corregido el 2026-09-28.
- Senales OK/SUSPECT persistidas: 35. El informe P11 nativo (descriptivo, sin Delta% ni candidatos) se lista debajo, tal como lo genera GET /api/fm/fixtures/{id}/report.md.

# Latvia vs Cyprus
- Competición: UEFA Nations League
- Kickoff (UTC): desconocido
- Flags de calidad: leakage=false; suspect=true
- sort_criteria: sample_size_desc

## Mercados (orden: mayor muestra disponible, no fuerza de señal)

### 1. away_corners @ 3.5 — Cyprus (away)
- basis: team_own_stats
- FM reportado: hits 6/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=3, rate=0.600, mean=4.4, median=4, min=1, max=10
-   last10: n=10, hits=7, rate=0.700, mean=4.8, median=5, min=1, max=10
-   all: n=30, hits=18, rate=0.600, mean=4.3, median=4.5, min=0, max=10
- Split home/away:
-   home: n=15, hits=11, rate=0.733, mean=4.4, median=5
-   away: n=15, hits=7, rate=0.467, mean=4.2, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=5, median=5, min=2, max=9
    last10: n=10, hits=6, rate=0.600, mean=4.5, median=4.5, min=2, max=9
    all: n=30, hits=14, rate=0.467, mean=3.5333, median=3, min=0, max=9
  Split home/away:
    home: n=15, hits=9, rate=0.600, mean=4.3333, median=5
    away: n=15, hits=5, rate=0.333, mean=2.7333, median=3
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 2. away_corners @ 5.5 — Cyprus (away)
- basis: team_own_stats
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=4.4, median=4, min=1, max=10
-   last10: n=10, hits=7, rate=0.700, mean=4.8, median=5, min=1, max=10
-   all: n=30, hits=20, rate=0.667, mean=4.3, median=4.5, min=0, max=10
- Split home/away:
-   home: n=15, hits=10, rate=0.667, mean=4.4, median=5
-   away: n=15, hits=10, rate=0.667, mean=4.2, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=5, median=5, min=2, max=9
    last10: n=10, hits=7, rate=0.700, mean=4.5, median=4.5, min=2, max=9
    all: n=30, hits=25, rate=0.833, mean=3.5333, median=3, min=0, max=9
  Split home/away:
    home: n=15, hits=11, rate=0.733, mean=4.3333, median=5
    away: n=15, hits=14, rate=0.933, mean=2.7333, median=3
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 3. away_goals @ 0.5 — Cyprus (away)
- basis: team_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=1.4, median=1, min=0, max=3
-   last10: n=10, hits=8, rate=0.800, mean=1.7, median=2, min=0, max=4
-   all: n=30, hits=18, rate=0.600, mean=1.1333, median=1, min=0, max=4
- Split home/away:
-   home: n=15, hits=8, rate=0.533, mean=1, median=1
-   away: n=15, hits=10, rate=0.667, mean=1.2667, median=1
- Rival (contexto equivalente):
  FM reportado: hits 7/10; mejor ventana: 10 (7/10)
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=0.6, median=1, min=0, max=1
    last10: n=10, hits=5, rate=0.500, mean=0.6, median=0.5, min=0, max=2
    all: n=30, hits=16, rate=0.533, mean=0.6333, median=1, min=0, max=2
  Split home/away:
    home: n=15, hits=9, rate=0.600, mean=0.8, median=1
    away: n=15, hits=7, rate=0.467, mean=0.4667, median=0
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 4. away_goals @ 0.5 — Latvia (home)
- basis: team_own_stats
- FM reportado: hits 7/10; mejor ventana: 10 (7/10)
- Ventanas propias:
-   last5: n=5, hits=3, rate=0.600, mean=0.6, median=1, min=0, max=1
-   last10: n=10, hits=5, rate=0.500, mean=0.6, median=0.5, min=0, max=2
-   all: n=30, hits=16, rate=0.533, mean=0.6333, median=1, min=0, max=2
- Split home/away:
-   home: n=15, hits=9, rate=0.600, mean=0.8, median=1
-   away: n=15, hits=7, rate=0.467, mean=0.4667, median=0
- Rival (contexto equivalente):
  FM reportado: hits 4/4; mejor ventana: no determinable
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=1.4, median=1, min=0, max=3
    last10: n=10, hits=8, rate=0.800, mean=1.7, median=2, min=0, max=4
    all: n=30, hits=18, rate=0.600, mean=1.1333, median=1, min=0, max=4
  Split home/away:
    home: n=15, hits=8, rate=0.533, mean=1, median=1
    away: n=15, hits=10, rate=0.667, mean=1.2667, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 5. home_corners @ 2.5 — Latvia (home)
- basis: team_own_stats
- FM reportado: hits 8/10; mejor ventana: 10 (8/10)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=5, median=5, min=2, max=9
-   last10: n=10, hits=8, rate=0.800, mean=4.5, median=4.5, min=2, max=9
-   all: n=30, hits=19, rate=0.633, mean=3.5333, median=3, min=0, max=9
- Split home/away:
-   home: n=15, hits=11, rate=0.733, mean=4.3333, median=5
-   away: n=15, hits=8, rate=0.533, mean=2.7333, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=4.4, median=4, min=1, max=10
    last10: n=10, hits=8, rate=0.800, mean=4.8, median=5, min=1, max=10
    all: n=30, hits=21, rate=0.700, mean=4.3, median=4.5, min=0, max=10
  Split home/away:
    home: n=15, hits=12, rate=0.800, mean=4.4, median=5
    away: n=15, hits=9, rate=0.600, mean=4.2, median=3
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 6. home_corners @ 5.5 — Latvia (home)
- basis: team_own_stats
- FM reportado: hits 7/9; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=3, rate=0.600, mean=5, median=5, min=2, max=9
-   last10: n=10, hits=7, rate=0.700, mean=4.5, median=4.5, min=2, max=9
-   all: n=30, hits=25, rate=0.833, mean=3.5333, median=3, min=0, max=9
- Split home/away:
-   home: n=15, hits=11, rate=0.733, mean=4.3333, median=5
-   away: n=15, hits=14, rate=0.933, mean=2.7333, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=4.4, median=4, min=1, max=10
    last10: n=10, hits=7, rate=0.700, mean=4.8, median=5, min=1, max=10
    all: n=30, hits=20, rate=0.667, mean=4.3, median=4.5, min=0, max=10
  Split home/away:
    home: n=15, hits=10, rate=0.667, mean=4.4, median=5
    away: n=15, hits=10, rate=0.667, mean=4.2, median=3
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 7. home_goals @ 0.5 — Cyprus (away)
- basis: team_own_stats
- FM reportado: hits 6/7; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=1.4, median=1, min=0, max=3
-   last10: n=10, hits=8, rate=0.800, mean=1.7, median=2, min=0, max=4
-   all: n=30, hits=18, rate=0.600, mean=1.1333, median=1, min=0, max=4
- Split home/away:
-   home: n=15, hits=8, rate=0.533, mean=1, median=1
-   away: n=15, hits=10, rate=0.667, mean=1.2667, median=1
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=0.6, median=1, min=0, max=1
    last10: n=10, hits=5, rate=0.500, mean=0.6, median=0.5, min=0, max=2
    all: n=30, hits=16, rate=0.533, mean=0.6333, median=1, min=0, max=2
  Split home/away:
    home: n=15, hits=9, rate=0.600, mean=0.8, median=1
    away: n=15, hits=7, rate=0.467, mean=0.4667, median=0
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 8. home_goals @ 1.5 — Latvia (home)
- basis: team_own_stats
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=0.6, median=1, min=0, max=1
-   last10: n=10, hits=9, rate=0.900, mean=0.6, median=0.5, min=0, max=2
-   all: n=30, hits=27, rate=0.900, mean=0.6333, median=1, min=0, max=2
- Split home/away:
-   home: n=15, hits=12, rate=0.800, mean=0.8, median=1
-   away: n=15, hits=15, rate=1.000, mean=0.4667, median=0
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=1.4, median=1, min=0, max=3
    last10: n=10, hits=4, rate=0.400, mean=1.7, median=2, min=0, max=4
    all: n=30, hits=19, rate=0.633, mean=1.1333, median=1, min=0, max=4
  Split home/away:
    home: n=15, hits=9, rate=0.600, mean=1, median=1
    away: n=15, hits=10, rate=0.667, mean=1.2667, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 9. total_goals @ 1.5 — Latvia vs Cyprus (sin rol)
- basis: match_total_goals
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=2.6, median=2, min=1, max=5
-   last10: n=10, hits=9, rate=0.900, mean=3.3, median=3.5, min=1, max=6
-   all: n=30, hits=26, rate=0.867, mean=3.2667, median=3, min=1, max=6
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=3.2, median=3
-   away: n=15, hits=13, rate=0.867, mean=3.3333, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=2, rate=0.400, mean=1.4, median=1, min=0, max=3
    last10: n=10, hits=5, rate=0.500, mean=2, median=1.5, min=0, max=5
    all: n=30, hits=20, rate=0.667, mean=2.3333, median=2, min=0, max=5
  Split home/away:
    home: n=15, hits=11, rate=0.733, mean=2.4, median=2
    away: n=15, hits=9, rate=0.600, mean=2.2667, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 10. total_goals @ 2.5 — Latvia vs Cyprus (sin rol)
- basis: match_total_goals
- FM reportado: hits 4/5; mejor ventana: 5 (4/5)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=1.4, median=1, min=0, max=3
-   last10: n=10, hits=7, rate=0.700, mean=2, median=1.5, min=0, max=5
-   all: n=30, hits=18, rate=0.600, mean=2.3333, median=2, min=0, max=5
- Split home/away:
-   home: n=15, hits=9, rate=0.600, mean=2.4, median=2
-   away: n=15, hits=9, rate=0.600, mean=2.2667, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=2.6, median=2, min=1, max=5
    last10: n=10, hits=4, rate=0.400, mean=3.3, median=3.5, min=1, max=6
    all: n=30, hits=10, rate=0.333, mean=3.2667, median=3, min=1, max=6
  Split home/away:
    home: n=15, hits=5, rate=0.333, mean=3.2, median=3
    away: n=15, hits=5, rate=0.333, mean=3.3333, median=3
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 11. total_goals @ 3.5 — Latvia vs Cyprus (sin rol)
- basis: match_total_goals
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.4, median=1, min=0, max=3
-   last10: n=10, hits=8, rate=0.800, mean=2, median=1.5, min=0, max=5
-   all: n=30, hits=24, rate=0.800, mean=2.3333, median=2, min=0, max=5
- Split home/away:
-   home: n=15, hits=12, rate=0.800, mean=2.4, median=2
-   away: n=15, hits=12, rate=0.800, mean=2.2667, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=2.6, median=2, min=1, max=5
    last10: n=10, hits=5, rate=0.500, mean=3.3, median=3.5, min=1, max=6
    all: n=30, hits=16, rate=0.533, mean=3.2667, median=3, min=1, max=6
  Split home/away:
    home: n=15, hits=8, rate=0.533, mean=3.2, median=3
    away: n=15, hits=8, rate=0.533, mean=3.3333, median=3
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 12. 1x2_cards — Cyprus (away)
- basis: unmapped
- FM reportado: hits 6/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=true, source_conflict=false, motivo="non-numeric history value 'vt'; market '1x2_cards' has no mapping to fm_team_matches.team_stats_json"

### 13. 1x2_offsides — Latvia (home)
- basis: unmapped
- FM reportado: hits 4/5; mejor ventana: 5 (4/5)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=true, source_conflict=false, motivo="non-numeric history value 'vt'; market '1x2_offsides' has no mapping to fm_team_matches.team_stats_json"

### 14. btts — Latvia (home)
- basis: unmapped
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=true, source_conflict=false, motivo="non-numeric history value 'vt'; market 'btts' has no mapping to fm_team_matches.team_stats_json"

### 15. total_corners @ 6.5 — Latvia vs Cyprus (sin rol)
- basis: not_reproducible
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 16. total_corners @ 7.5 — Latvia vs Cyprus (sin rol)
- basis: not_reproducible
- FM reportado: hits 6/7; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 17. total_corners @ 8.5 — Latvia vs Cyprus (sin rol)
- basis: not_reproducible
- FM reportado: hits 6/7; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 18. total_corners @ 9.5 — Latvia vs Cyprus (sin rol)
- basis: not_reproducible
- FM reportado: hits 7/10; mejor ventana: 10 (7/10)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 19. total_corners @ 10.5 — Latvia vs Cyprus (sin rol)
- basis: not_reproducible
- FM reportado: hits 7/10; mejor ventana: 10 (7/10)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 20. total_corners @ 11.5 — Latvia vs Cyprus (sin rol)
- basis: not_reproducible
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

## Señales de jugadores (orden: mayor muestra disponible, no fuerza de señal)

### 1. cards @ 0.5 — R. Savalnieks
- basis: player_own_stats
- FM reportado: hits 3/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=2, rate=0.400, mean=0.4, median=0, min=0, max=1
  last10: n=10, hits=3, rate=0.300, mean=0.3, median=0, min=0, max=1
  all: n=26, hits=6, rate=0.231, mean=0.2308, median=0, min=0, max=1
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 2. shots @ 1.5 — I. Pittas
- basis: player_own_stats
- FM reportado: hits 7/10; mejor ventana: 10 (7/10)
- Ventanas propias:
  last5: n=5, hits=1, rate=0.200, mean=0.6, median=0, min=0, max=3
  last10: n=10, hits=5, rate=0.500, mean=1.8, median=1.5, min=0, max=5
  all: n=25, hits=11, rate=0.440, mean=1.6, median=1, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 3. shots @ 2.5 — I. Pittas
- basis: player_own_stats
- FM reportado: hits 7/10; mejor ventana: 10 (7/10)
- Ventanas propias:
  last5: n=5, hits=1, rate=0.200, mean=0.6, median=0, min=0, max=3
  last10: n=10, hits=5, rate=0.500, mean=1.8, median=1.5, min=0, max=5
  all: n=25, hits=8, rate=0.320, mean=1.6, median=1, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 4. shots @ 1.5 — V. Gutkovskis
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2, median=2, min=0, max=3
  last10: n=10, hits=6, rate=0.600, mean=1.6, median=2, min=0, max=3
  all: n=23, hits=9, rate=0.391, mean=1.1739, median=1, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 5. shots_on_target @ 0.5 — R. Uldrikis
- basis: player_own_stats
- FM reportado: hits 4/6; mejor ventana: 10 (4/6)
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=0.8, median=1, min=0, max=2
  last10: n=10, hits=4, rate=0.400, mean=0.5, median=0, min=0, max=2
  all: n=18, hits=8, rate=0.444, mean=0.5556, median=0, min=0, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 6. tackles @ 1.5 — G. Kastanos
- basis: player_own_stats
- FM reportado: hits 4/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=1, rate=0.200, mean=1.2, median=0, min=0, max=6
  last10: n=10, hits=4, rate=0.400, mean=1.7, median=1, min=0, max=6
  all: n=14, hits=6, rate=0.429, mean=1.7857, median=1, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 7. tackles @ 1.5 — A. Ciganiks
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.4, median=3, min=0, max=4
  last10: n=10, hits=7, rate=0.700, mean=2.4, median=3, min=0, max=4
  all: n=13, hits=8, rate=0.615, mean=2.1538, median=3, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 8. tackles @ 2.5 — A. Ciganiks
- basis: player_own_stats
- FM reportado: hits 5/7; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=2.4, median=3, min=0, max=4
  last10: n=10, hits=6, rate=0.600, mean=2.4, median=3, min=0, max=4
  all: n=13, hits=7, rate=0.539, mean=2.1538, median=3, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 9. foul_involvements @ 1.5 — A. Cernomordijs
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.2, median=3, min=0, max=3
  last10: n=10, hits=8, rate=0.800, mean=2.2, median=2, min=0, max=4
  all: n=12, hits=8, rate=0.667, mean=2, median=2, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 10. tackles @ 1.5 — L. Vapne
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.6, median=3, min=0, max=4
  last10: n=9, hits=4, rate=0.444, mean=1.5556, median=1, min=0, max=4
  all: n=9, hits=4, rate=0.444, mean=1.5556, median=1, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 11. goalkeeper_saves @ 1.5 — Fabiano
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: 5 (4/4)
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.2, median=2, min=0, max=4
  last10: n=5, hits=4, rate=0.800, mean=2.2, median=2, min=0, max=4
  all: n=5, hits=4, rate=0.800, mean=2.2, median=2, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 12. foul_involvements @ 1.5 — K. Artymatas
- basis: player_own_stats
- FM reportado: hits 6/9; mejor ventana: 10 (6/9)
- Ventanas propias:
  last5: n=3, hits=2, rate=0.667, mean=3.3333, median=4, min=0, max=6
  last10: n=3, hits=2, rate=0.667, mean=3.3333, median=4, min=0, max=6
  all: n=3, hits=2, rate=0.667, mean=3.3333, median=4, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 13. foul_involvements @ 2.5 — K. Artymatas
- basis: player_own_stats
- FM reportado: hits 6/9; mejor ventana: 10 (6/9)
- Ventanas propias:
  last5: n=3, hits=2, rate=0.667, mean=3.3333, median=4, min=0, max=6
  last10: n=3, hits=2, rate=0.667, mean=3.3333, median=4, min=0, max=6
  all: n=3, hits=2, rate=0.667, mean=3.3333, median=4, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

## Evidencia histórica (ventanas fijas)
- observaciones descriptivas: secuencia cruda (más antiguo -> más reciente) y hits de las ventanas fijas (last5/last10/all)

### Jugadores — evidencia con ventanas fijas
- R. Savalnieks — cards @ 0.5: [0,0,0,0,1,0,1,1,0,0] | last5 2/5 | last10 3/10 | all 6/26
- I. Pittas — shots @ 1.5: [5,4,3,3,0,0,0,0,0,3] | last5 1/5 | last10 5/10 | all 11/25
- I. Pittas — shots @ 2.5: [5,4,3,3,0,0,0,0,0,3] | last5 1/5 | last10 5/10 | all 8/25
- V. Gutkovskis — shots @ 1.5: [2,0,2,1,1,0,3,3,2,2] | last5 4/5 | last10 6/10 | all 9/23
- R. Uldrikis — shots_on_target @ 0.5: [0,0,0,0,1,1,1,2,0,0] | last5 3/5 | last10 4/10 | all 8/18
- G. Kastanos — tackles @ 1.5: [1,1,4,2,3,0,0,0,0,6] | last5 1/5 | last10 4/10 | all 6/14
- A. Ciganiks — tackles @ 1.5: [4,1,0,4,3,3,0,4,3,2] | last5 4/5 | last10 7/10 | all 8/13
- A. Ciganiks — tackles @ 2.5: [4,1,0,4,3,3,0,4,3,2] | last5 3/5 | last10 6/10 | all 7/13
- A. Cernomordijs — foul_involvements @ 1.5: [2,1,4,2,2,3,0,3,3,2] | last5 4/5 | last10 8/10 | all 8/12
- L. Vapne — tackles @ 1.5: [1,0,0,0,0,3,4,4,2] | last5 4/5 | last10 4/9 | all 4/9
- Fabiano — goalkeeper_saves @ 1.5: [2,3,4,0,2] | last5 4/5 | last10 4/5 | all 4/5
- K. Artymatas — foul_involvements @ 1.5: [0,4,6] | last5 2/3 | last10 2/3 | all 2/3
- K. Artymatas — foul_involvements @ 2.5: [0,4,6] | last5 2/3 | last10 2/3 | all 2/3

### Equipos — evidencia con ventanas fijas
- Cyprus — away_corners @ 3.5: [7,3,5,6,5,5,4,2,10,1] | last5 3/5 | last10 7/10 | all 18/30
  - Rival (Latvia): [6,3,4,5,2,5,3,6,9,2] | last5 3/5 | last10 6/10 | all 14/30
- Cyprus — away_corners @ 5.5: [7,3,5,6,5,5,4,2,10,1] | last5 4/5 | last10 7/10 | all 20/30
  - Rival (Latvia): [6,3,4,5,2,5,3,6,9,2] | last5 3/5 | last10 7/10 | all 25/30
- Cyprus — away_goals @ 0.5: [2,2,4,0,2,0,3,1,2,1] | last5 4/5 | last10 8/10 | all 18/30
  - Rival (Latvia): [1,0,0,2,0,0,1,1,1,0] | last5 3/5 | last10 5/10 | all 16/30
- Latvia — away_goals @ 0.5: [1,0,0,2,0,0,1,1,1,0] | last5 3/5 | last10 5/10 | all 16/30
  - Rival (Cyprus): [2,2,4,0,2,0,3,1,2,1] | last5 4/5 | last10 8/10 | all 18/30
- Latvia — home_corners @ 2.5: [6,3,4,5,2,5,3,6,9,2] | last5 4/5 | last10 8/10 | all 19/30
  - Rival (Cyprus): [7,3,5,6,5,5,4,2,10,1] | last5 3/5 | last10 8/10 | all 21/30
- Latvia — home_corners @ 5.5: [6,3,4,5,2,5,3,6,9,2] | last5 3/5 | last10 7/10 | all 25/30
  - Rival (Cyprus): [7,3,5,6,5,5,4,2,10,1] | last5 4/5 | last10 7/10 | all 20/30
- Cyprus — home_goals @ 0.5: [2,2,4,0,2,0,3,1,2,1] | last5 4/5 | last10 8/10 | all 18/30
  - Rival (Latvia): [1,0,0,2,0,0,1,1,1,0] | last5 3/5 | last10 5/10 | all 16/30
- Latvia — home_goals @ 1.5: [1,0,0,2,0,0,1,1,1,0] | last5 5/5 | last10 9/10 | all 27/30
  - Rival (Cyprus): [2,2,4,0,2,0,3,1,2,1] | last5 3/5 | last10 4/10 | all 19/30
- Latvia vs Cyprus — total_goals @ 1.5: [4,4,4,2,6,1,5,2,2,3] | last5 4/5 | last10 9/10 | all 26/30
- Latvia vs Cyprus — total_goals @ 2.5: [2,1,1,4,5,0,3,1,1,2] | last5 4/5 | last10 7/10 | all 18/30
- Latvia vs Cyprus — total_goals @ 3.5: [2,1,1,4,5,0,3,1,1,2] | last5 5/5 | last10 8/10 | all 24/30

## Confluencia descriptiva (equipo + rival + jugadores)
- familias de mercado con 2 o más series observadas en la DB; cada serie conserva su línea y sus ventanas fijas (last5/last10/all); sin score ni orden por fuerza

### corners
- Cyprus (away) @ 3.5: last5 3/5 | last10 7/10 | all 18/30
- Latvia (home) @ 3.5: last5 3/5 | last10 6/10 | all 14/30
- Cyprus (away) @ 5.5: last5 4/5 | last10 7/10 | all 20/30
- Latvia (home) @ 5.5: last5 3/5 | last10 7/10 | all 25/30
- Latvia (home) @ 2.5: last5 4/5 | last10 8/10 | all 19/30
- Cyprus (away) @ 2.5: last5 3/5 | last10 8/10 | all 21/30

### foul_involvements
- jugador A. Cernomordijs — foul_involvements @ 1.5: last5 4/5 | last10 8/10 | all 8/12
- jugador K. Artymatas — foul_involvements @ 1.5: last5 2/3 | last10 2/3 | all 2/3
- jugador K. Artymatas — foul_involvements @ 2.5: last5 2/3 | last10 2/3 | all 2/3

### goals
- Cyprus (away) @ 0.5: last5 4/5 | last10 8/10 | all 18/30
- Latvia (home) @ 0.5: last5 3/5 | last10 5/10 | all 16/30
- Latvia (home) @ 1.5: last5 5/5 | last10 9/10 | all 27/30
- Cyprus (away) @ 1.5: last5 3/5 | last10 4/10 | all 19/30

### shots
- jugador I. Pittas — shots @ 1.5: last5 1/5 | last10 5/10 | all 11/25
- jugador I. Pittas — shots @ 2.5: last5 1/5 | last10 5/10 | all 8/25
- jugador V. Gutkovskis — shots @ 1.5: last5 4/5 | last10 6/10 | all 9/23

### tackles
- jugador G. Kastanos — tackles @ 1.5: last5 1/5 | last10 4/10 | all 6/14
- jugador A. Ciganiks — tackles @ 1.5: last5 4/5 | last10 7/10 | all 8/13
- jugador A. Ciganiks — tackles @ 2.5: last5 3/5 | last10 6/10 | all 7/13
- jugador L. Vapne — tackles @ 1.5: last5 4/5 | last10 4/9 | all 4/9

## Cuotas

| mercado | línea | casa | lado | cuota | prob. implícita | capturada |
|---|---|---|---|---|---|---|
| away_corners | 3.5 | Kambi | over | 1.45 | 0.690 | 2026-09-28T21:36:41.657Z |
| away_corners | 3.5 | Kambi | under | 2.43 | 0.412 | 2026-09-28T21:36:41.657Z |
| away_corners | 5.5 | Kambi | over | 2.63 | 0.380 | 2026-09-28T21:36:41.657Z |
| away_corners | 5.5 | Kambi | under | 1.38 | 0.725 | 2026-09-28T21:36:41.657Z |
| away_goals | 0.5 | Bet365 | over | 1.4 | 0.714 | 2026-09-28T21:36:41.657Z |
| away_goals | 0.5 | Bet365 | under | 2.75 | 0.364 | 2026-09-28T21:36:41.657Z |
| away_goals | 0.5 | Kambi | over | 1.38 | 0.725 | 2026-09-28T21:36:41.657Z |
| away_goals | 0.5 | Kambi | under | 2.75 | 0.364 | 2026-09-28T21:36:41.657Z |
| away_goals | 0.5 | Ladbrokes | over | 1.35 | 0.741 | 2026-09-28T21:36:41.657Z |
| away_goals | 0.5 | Ladbrokes | under | 2.9 | 0.345 | 2026-09-28T21:36:41.657Z |
| away_goals | 0.5 | Bet365 | over | 1.4 | 0.714 | 2026-09-28T21:36:41.657Z |
| away_goals | 0.5 | Bet365 | under | 2.75 | 0.364 | 2026-09-28T21:36:41.657Z |
| away_goals | 0.5 | Kambi | over | 1.38 | 0.725 | 2026-09-28T21:36:41.657Z |
| away_goals | 0.5 | Kambi | under | 2.75 | 0.364 | 2026-09-28T21:36:41.657Z |
| away_goals | 0.5 | Ladbrokes | over | 1.35 | 0.741 | 2026-09-28T21:36:41.657Z |
| away_goals | 0.5 | Ladbrokes | under | 2.9 | 0.345 | 2026-09-28T21:36:41.657Z |
| home_corners | 2.5 | Kambi | over | 1.25 | 0.800 | 2026-09-28T21:36:41.657Z |
| home_corners | 2.5 | Kambi | under | 3.25 | 0.308 | 2026-09-28T21:36:41.657Z |
| home_corners | 5.5 | Kambi | over | 3 | 0.333 | 2026-09-28T21:36:41.657Z |
| home_corners | 5.5 | Kambi | under | 1.29 | 0.775 | 2026-09-28T21:36:41.657Z |
| home_goals | 0.5 | Bet365 | over | 1.44 | 0.694 | 2026-09-28T21:36:41.657Z |
| home_goals | 0.5 | Bet365 | under | 2.63 | 0.380 | 2026-09-28T21:36:41.657Z |
| home_goals | 0.5 | Kambi | over | 1.45 | 0.690 | 2026-09-28T21:36:41.657Z |
| home_goals | 0.5 | Kambi | under | 2.48 | 0.403 | 2026-09-28T21:36:41.657Z |
| home_goals | 0.5 | Ladbrokes | over | 1.48 | 0.676 | 2026-09-28T21:36:41.657Z |
| home_goals | 0.5 | Ladbrokes | under | 2.5 | 0.400 | 2026-09-28T21:36:41.657Z |
| home_goals | 1.5 | Bet365 | over | 3 | 0.333 | 2026-09-28T21:36:41.657Z |
| home_goals | 1.5 | Bet365 | under | 1.36 | 0.735 | 2026-09-28T21:36:41.657Z |
| home_goals | 1.5 | Kambi | over | 3.15 | 0.317 | 2026-09-28T21:36:41.657Z |
| home_goals | 1.5 | Kambi | under | 1.29 | 0.775 | 2026-09-28T21:36:41.657Z |
| home_goals | 1.5 | Ladbrokes | over | 3.3 | 0.303 | 2026-09-28T21:36:41.657Z |
| home_goals | 1.5 | Ladbrokes | under | 1.28 | 0.781 | 2026-09-28T21:36:41.657Z |
| total_goals | 1.5 | Bet365 | over | 1.5 | 0.667 | 2026-09-28T21:36:41.657Z |
| total_goals | 1.5 | Bet365 | under | 2.5 | 0.400 | 2026-09-28T21:36:41.657Z |
| total_goals | 1.5 | Kambi | over | 1.49 | 0.671 | 2026-09-28T21:36:41.657Z |
| total_goals | 1.5 | Kambi | under | 2.5 | 0.400 | 2026-09-28T21:36:41.657Z |
| total_goals | 1.5 | Ladbrokes | over | 1.48 | 0.676 | 2026-09-28T21:36:41.657Z |
| total_goals | 1.5 | Ladbrokes | under | 2.5 | 0.400 | 2026-09-28T21:36:41.657Z |
| total_goals | 1.5 | Paddy Power | over | 1.44 | 0.694 | 2026-09-28T21:36:41.657Z |
| total_goals | 1.5 | Paddy Power | under | 2.63 | 0.380 | 2026-09-28T21:36:41.657Z |
| total_goals | 2.5 | Altenar | over | 2.45 | 0.408 | 2026-09-28T21:36:41.657Z |
| total_goals | 2.5 | Altenar | under | 1.46 | 0.685 | 2026-09-28T21:36:41.657Z |
| total_goals | 2.5 | Bet365 | over | 2.5 | 0.400 | 2026-09-28T21:36:41.657Z |
| total_goals | 2.5 | Bet365 | under | 1.5 | 0.667 | 2026-09-28T21:36:41.657Z |
| total_goals | 2.5 | Kambi | over | 2.5 | 0.400 | 2026-09-28T21:36:41.657Z |
| total_goals | 2.5 | Kambi | under | 1.49 | 0.671 | 2026-09-28T21:36:41.657Z |
| total_goals | 2.5 | Ladbrokes | over | 2.45 | 0.408 | 2026-09-28T21:36:41.657Z |
| total_goals | 2.5 | Ladbrokes | under | 1.48 | 0.676 | 2026-09-28T21:36:41.657Z |
| total_goals | 2.5 | Paddy Power | over | 2.5 | 0.400 | 2026-09-28T21:36:41.657Z |
| total_goals | 2.5 | Paddy Power | under | 1.5 | 0.667 | 2026-09-28T21:36:41.657Z |
| total_goals | 3.5 | Bet365 | over | 4.5 | 0.222 | 2026-09-28T21:36:41.657Z |
| total_goals | 3.5 | Bet365 | under | 1.17 | 0.855 | 2026-09-28T21:36:41.657Z |
| total_goals | 3.5 | Kambi | over | 4.8 | 0.208 | 2026-09-28T21:36:41.657Z |
| total_goals | 3.5 | Kambi | under | 1.16 | 0.862 | 2026-09-28T21:36:41.657Z |
| total_goals | 3.5 | Ladbrokes | over | 4.6 | 0.217 | 2026-09-28T21:36:41.657Z |
| total_goals | 3.5 | Ladbrokes | under | 1.17 | 0.855 | 2026-09-28T21:36:41.657Z |
| total_goals | 3.5 | Paddy Power | over | 5 | 0.200 | 2026-09-28T21:36:41.657Z |
| total_goals | 3.5 | Paddy Power | under | 1.14 | 0.877 | 2026-09-28T21:36:41.657Z |
| total_corners | 6.5 | Bet365 | over | 1.18 | 0.847 | 2026-09-28T21:36:41.657Z |
| total_corners | 6.5 | Kambi | over | 1.24 | 0.806 | 2026-09-28T21:36:41.657Z |
| total_corners | 6.5 | Kambi | under | 3.55 | 0.282 | 2026-09-28T21:36:41.657Z |
| total_corners | 7.5 | Kambi | over | 1.44 | 0.694 | 2026-09-28T21:36:41.657Z |
| total_corners | 7.5 | Kambi | under | 2.55 | 0.392 | 2026-09-28T21:36:41.657Z |
| total_corners | 7.5 | Ladbrokes | over | 1.35 | 0.741 | 2026-09-28T21:36:41.657Z |
| total_corners | 7.5 | Ladbrokes | under | 3 | 0.333 | 2026-09-28T21:36:41.657Z |
| total_corners | 8.5 | Kambi | over | 1.74 | 0.575 | 2026-09-28T21:36:41.657Z |
| total_corners | 8.5 | Kambi | under | 1.94 | 0.515 | 2026-09-28T21:36:41.657Z |
| total_corners | 8.5 | Ladbrokes | over | 1.61 | 0.621 | 2026-09-28T21:36:41.657Z |
| total_corners | 8.5 | Ladbrokes | under | 2.15 | 0.465 | 2026-09-28T21:36:41.657Z |
| total_corners | 9.5 | Bet365 | over | 2.1 | 0.476 | 2026-09-28T21:36:41.657Z |
| total_corners | 9.5 | Bet365 | under | 1.67 | 0.599 | 2026-09-28T21:36:41.657Z |
| total_corners | 9.5 | Kambi | over | 2.2 | 0.455 | 2026-09-28T21:36:41.657Z |
| total_corners | 9.5 | Kambi | under | 1.57 | 0.637 | 2026-09-28T21:36:41.657Z |
| total_corners | 9.5 | Ladbrokes | over | 2.05 | 0.488 | 2026-09-28T21:36:41.657Z |
| total_corners | 9.5 | Ladbrokes | under | 1.7 | 0.588 | 2026-09-28T21:36:41.657Z |
| total_corners | 10.5 | Kambi | over | 2.88 | 0.347 | 2026-09-28T21:36:41.657Z |
| total_corners | 10.5 | Kambi | under | 1.35 | 0.741 | 2026-09-28T21:36:41.657Z |
| total_corners | 10.5 | Ladbrokes | over | 2.75 | 0.364 | 2026-09-28T21:36:41.657Z |
| total_corners | 10.5 | Ladbrokes | under | 1.4 | 0.714 | 2026-09-28T21:36:41.657Z |
| total_corners | 11.5 | Kambi | over | 3.85 | 0.260 | 2026-09-28T21:36:41.657Z |
| total_corners | 11.5 | Kambi | under | 1.2 | 0.833 | 2026-09-28T21:36:41.657Z |
| total_corners | 11.5 | Ladbrokes | over | 3.8 | 0.263 | 2026-09-28T21:36:41.657Z |
| total_corners | 11.5 | Ladbrokes | under | 1.22 | 0.820 | 2026-09-28T21:36:41.657Z |
- Cuota manual Betano (away_corners @ 3.5): no cargada
- Cuota manual Betano (away_corners @ 5.5): no cargada
- Cuota manual Betano (away_goals @ 0.5): no cargada
- Cuota manual Betano (away_goals @ 0.5): no cargada
- Cuota manual Betano (home_corners @ 2.5): no cargada
- Cuota manual Betano (home_corners @ 5.5): no cargada
- Cuota manual Betano (home_goals @ 0.5): no cargada
- Cuota manual Betano (home_goals @ 1.5): no cargada
- Cuota manual Betano (total_goals @ 1.5): no cargada
- Cuota manual Betano (total_goals @ 2.5): no cargada
- Cuota manual Betano (total_goals @ 3.5): no cargada
- Cuota manual Betano (1x2_cards): no cargada
- Cuota manual Betano (1x2_offsides): no cargada
- Cuota manual Betano (btts): no cargada
- Cuota manual Betano (total_corners @ 6.5): no cargada
- Cuota manual Betano (total_corners @ 7.5): no cargada
- Cuota manual Betano (total_corners @ 8.5): no cargada
- Cuota manual Betano (total_corners @ 9.5): no cargada
- Cuota manual Betano (total_corners @ 10.5): no cargada
- Cuota manual Betano (total_corners @ 11.5): no cargada

## Nota de cierre
Este reporte es descriptivo. No constituye una recomendación de apuesta ni una probabilidad validada de resultado futuro.

---

## Armenia vs Montenegro (33662309) - kickoff 2026-09-28T16:00:00.000Z

POST-PARTIDO - SOLO REGISTRO HISTORICO, NO ACCIONABLE

- Motivo de la marca: partido ya jugado (kickoff 2026-09-28T16:00:00.000Z); las cuotas en fm_market_odds son de cierre y las senales pueden reflejar el resultado ya conocido. Uso unico: dataset historico para calibracion/backtest.
- Snapshot capturado post-partido: 21:36:57Z-21:37:26Z (tabs overview/player-trends/team-trends, 3 navs; +2 fetch de scope partidos sin nav).
- No intentado - pausa del operador por lectura de un ledger manual desactualizado (docs/STATE.md); limite real de la API: 40 navs/corrida, 24 necesarias para los 8 fixtures. Corregido el 2026-09-28.
- Senales OK/SUSPECT persistidas: 49. El informe P11 nativo (descriptivo, sin Delta% ni candidatos) se lista debajo, tal como lo genera GET /api/fm/fixtures/{id}/report.md.

# Armenia vs Montenegro
- Competición: UEFA Nations League
- Kickoff (UTC): desconocido
- Flags de calidad: leakage=false; suspect=true
- sort_criteria: sample_size_desc

## Mercados (orden: mayor muestra disponible, no fuerza de señal)

### 1. away_goals @ 0.5 — Armenia (home)
- basis: team_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.2, median=1, min=1, max=2
-   last10: n=10, hits=6, rate=0.600, mean=0.8, median=1, min=0, max=2
-   all: n=30, hits=18, rate=0.600, mean=1.0333, median=1, min=0, max=4
- Split home/away:
-   home: n=15, hits=8, rate=0.533, mean=0.9333, median=1
-   away: n=15, hits=10, rate=0.667, mean=1.1333, median=1
- Rival (contexto equivalente):
  FM reportado: hits 8/8; mejor ventana: no determinable
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=1.8, median=2, min=1, max=2
    last10: n=10, hits=8, rate=0.800, mean=1.5, median=2, min=0, max=2
    all: n=30, hits=21, rate=0.700, mean=1.2, median=1, min=0, max=3
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=1.6, median=2
    away: n=15, hits=8, rate=0.533, mean=0.8, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 2. away_goals @ 0.5 — Montenegro (away)
- basis: team_own_stats
- FM reportado: hits 8/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.8, median=2, min=1, max=2
-   last10: n=10, hits=8, rate=0.800, mean=1.5, median=2, min=0, max=2
-   all: n=30, hits=21, rate=0.700, mean=1.2, median=1, min=0, max=3
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=1.6, median=2
-   away: n=15, hits=8, rate=0.533, mean=0.8, median=1
- Rival (contexto equivalente):
  FM reportado: hits 9/10; mejor ventana: 10 (9/10)
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=1.2, median=1, min=1, max=2
    last10: n=10, hits=6, rate=0.600, mean=0.8, median=1, min=0, max=2
    all: n=30, hits=18, rate=0.600, mean=1.0333, median=1, min=0, max=4
  Split home/away:
    home: n=15, hits=8, rate=0.533, mean=0.9333, median=1
    away: n=15, hits=10, rate=0.667, mean=1.1333, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 3. away_goals @ 1.5 — Montenegro (away)
- basis: team_own_stats
- FM reportado: hits 7/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=1.8, median=2, min=1, max=2
-   last10: n=10, hits=7, rate=0.700, mean=1.5, median=2, min=0, max=2
-   all: n=30, hits=13, rate=0.433, mean=1.2, median=1, min=0, max=3
- Split home/away:
-   home: n=15, hits=9, rate=0.600, mean=1.6, median=2
-   away: n=15, hits=4, rate=0.267, mean=0.8, median=1
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=1, rate=0.200, mean=1.2, median=1, min=1, max=2
    last10: n=10, hits=2, rate=0.200, mean=0.8, median=1, min=0, max=2
    all: n=30, hits=9, rate=0.300, mean=1.0333, median=1, min=0, max=4
  Split home/away:
    home: n=15, hits=4, rate=0.267, mean=0.9333, median=1
    away: n=15, hits=5, rate=0.333, mean=1.1333, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 4. away_goals @ 2.5 — Armenia (home)
- basis: team_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.2, median=1, min=1, max=2
-   last10: n=10, hits=10, rate=1.000, mean=0.8, median=1, min=0, max=2
-   all: n=30, hits=28, rate=0.933, mean=1.0333, median=1, min=0, max=4
- Split home/away:
-   home: n=15, hits=14, rate=0.933, mean=0.9333, median=1
-   away: n=15, hits=14, rate=0.933, mean=1.1333, median=1
- Rival (contexto equivalente):
  FM reportado: hits 10/10; mejor ventana: 10 (10/10)
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=1.8, median=2, min=1, max=2
    last10: n=10, hits=10, rate=1.000, mean=1.5, median=2, min=0, max=2
    all: n=30, hits=28, rate=0.933, mean=1.2, median=1, min=0, max=3
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=1.6, median=2
    away: n=15, hits=15, rate=1.000, mean=0.8, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 5. away_goals @ 2.5 — Montenegro (away)
- basis: team_own_stats
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.8, median=2, min=1, max=2
-   last10: n=10, hits=10, rate=1.000, mean=1.5, median=2, min=0, max=2
-   all: n=30, hits=28, rate=0.933, mean=1.2, median=1, min=0, max=3
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=1.6, median=2
-   away: n=15, hits=15, rate=1.000, mean=0.8, median=1
- Rival (contexto equivalente):
  FM reportado: hits 4/4; mejor ventana: no determinable
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=1.2, median=1, min=1, max=2
    last10: n=10, hits=10, rate=1.000, mean=0.8, median=1, min=0, max=2
    all: n=30, hits=28, rate=0.933, mean=1.0333, median=1, min=0, max=4
  Split home/away:
    home: n=15, hits=14, rate=0.933, mean=0.9333, median=1
    away: n=15, hits=14, rate=0.933, mean=1.1333, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 6. home_corners @ 3.5 — Armenia (home)
- basis: team_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=10.2, median=11, min=1, max=15
-   last10: n=10, hits=4, rate=0.400, mean=5.7, median=2, min=0, max=15
-   all: n=30, hits=19, rate=0.633, mean=5.8, median=4.5, min=0, max=16
- Split home/away:
-   home: n=15, hits=12, rate=0.800, mean=7.3333, median=6
-   away: n=15, hits=7, rate=0.467, mean=4.2667, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=6.6, median=8, min=2, max=11
    last10: n=10, hits=5, rate=0.500, mean=5.3, median=4, min=1, max=11
    all: n=30, hits=20, rate=0.667, mean=5.1, median=5, min=1, max=12
  Split home/away:
    home: n=15, hits=12, rate=0.800, mean=6.1333, median=6
    away: n=15, hits=8, rate=0.533, mean=4.0667, median=4
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 7. home_corners @ 4.5 — Armenia (home)
- basis: team_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=10.2, median=11, min=1, max=15
-   last10: n=10, hits=4, rate=0.400, mean=5.7, median=2, min=0, max=15
-   all: n=30, hits=15, rate=0.500, mean=5.8, median=4.5, min=0, max=16
- Split home/away:
-   home: n=15, hits=10, rate=0.667, mean=7.3333, median=6
-   away: n=15, hits=5, rate=0.333, mean=4.2667, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=6.6, median=8, min=2, max=11
    last10: n=10, hits=5, rate=0.500, mean=5.3, median=4, min=1, max=11
    all: n=30, hits=16, rate=0.533, mean=5.1, median=5, min=1, max=12
  Split home/away:
    home: n=15, hits=11, rate=0.733, mean=6.1333, median=6
    away: n=15, hits=5, rate=0.333, mean=4.0667, median=4
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 8. home_corners @ 5.5 — Armenia (home)
- basis: team_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=10.2, median=11, min=1, max=15
-   last10: n=10, hits=4, rate=0.400, mean=5.7, median=2, min=0, max=15
-   all: n=30, hits=12, rate=0.400, mean=5.8, median=4.5, min=0, max=16
- Split home/away:
-   home: n=15, hits=8, rate=0.533, mean=7.3333, median=6
-   away: n=15, hits=4, rate=0.267, mean=4.2667, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=6.6, median=8, min=2, max=11
    last10: n=10, hits=4, rate=0.400, mean=5.3, median=4, min=1, max=11
    all: n=30, hits=10, rate=0.333, mean=5.1, median=5, min=1, max=12
  Split home/away:
    home: n=15, hits=8, rate=0.533, mean=6.1333, median=6
    away: n=15, hits=2, rate=0.133, mean=4.0667, median=4
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 9. home_corners @ 6.5 — Armenia (home)
- basis: team_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=10.2, median=11, min=1, max=15
-   last10: n=10, hits=4, rate=0.400, mean=5.7, median=2, min=0, max=15
-   all: n=30, hits=10, rate=0.333, mean=5.8, median=4.5, min=0, max=16
- Split home/away:
-   home: n=15, hits=7, rate=0.467, mean=7.3333, median=6
-   away: n=15, hits=3, rate=0.200, mean=4.2667, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=6.6, median=8, min=2, max=11
    last10: n=10, hits=4, rate=0.400, mean=5.3, median=4, min=1, max=11
    all: n=30, hits=9, rate=0.300, mean=5.1, median=5, min=1, max=12
  Split home/away:
    home: n=15, hits=7, rate=0.467, mean=6.1333, median=6
    away: n=15, hits=2, rate=0.133, mean=4.0667, median=4
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 10. home_goals @ 0.5 — Armenia (home)
- basis: team_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.2, median=1, min=1, max=2
-   last10: n=10, hits=6, rate=0.600, mean=0.8, median=1, min=0, max=2
-   all: n=30, hits=18, rate=0.600, mean=1.0333, median=1, min=0, max=4
- Split home/away:
-   home: n=15, hits=8, rate=0.533, mean=0.9333, median=1
-   away: n=15, hits=10, rate=0.667, mean=1.1333, median=1
- Rival (contexto equivalente):
  FM reportado: hits 8/10; mejor ventana: 10 (8/10)
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=1.8, median=2, min=1, max=2
    last10: n=10, hits=8, rate=0.800, mean=1.5, median=2, min=0, max=2
    all: n=30, hits=21, rate=0.700, mean=1.2, median=1, min=0, max=3
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=1.6, median=2
    away: n=15, hits=8, rate=0.533, mean=0.8, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 11. home_goals @ 0.5 — Montenegro (away)
- basis: team_own_stats
- FM reportado: hits 8/10; mejor ventana: 10 (8/10)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.8, median=2, min=1, max=2
-   last10: n=10, hits=8, rate=0.800, mean=1.5, median=2, min=0, max=2
-   all: n=30, hits=21, rate=0.700, mean=1.2, median=1, min=0, max=3
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=1.6, median=2
-   away: n=15, hits=8, rate=0.533, mean=0.8, median=1
- Rival (contexto equivalente):
  FM reportado: hits 5/5; mejor ventana: 5 (5/5)
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=1.2, median=1, min=1, max=2
    last10: n=10, hits=6, rate=0.600, mean=0.8, median=1, min=0, max=2
    all: n=30, hits=18, rate=0.600, mean=1.0333, median=1, min=0, max=4
  Split home/away:
    home: n=15, hits=8, rate=0.533, mean=0.9333, median=1
    away: n=15, hits=10, rate=0.667, mean=1.1333, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 12. home_goals @ 1.5 — Armenia (home)
- basis: team_own_stats
- FM reportado: hits 7/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=1.2, median=1, min=1, max=2
-   last10: n=10, hits=8, rate=0.800, mean=0.8, median=1, min=0, max=2
-   all: n=30, hits=21, rate=0.700, mean=1.0333, median=1, min=0, max=4
- Split home/away:
-   home: n=15, hits=11, rate=0.733, mean=0.9333, median=1
-   away: n=15, hits=10, rate=0.667, mean=1.1333, median=1
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=1, rate=0.200, mean=1.8, median=2, min=1, max=2
    last10: n=10, hits=3, rate=0.300, mean=1.5, median=2, min=0, max=2
    all: n=30, hits=17, rate=0.567, mean=1.2, median=1, min=0, max=3
  Split home/away:
    home: n=15, hits=6, rate=0.400, mean=1.6, median=2
    away: n=15, hits=11, rate=0.733, mean=0.8, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 13. home_goals @ 2.5 — Armenia (home)
- basis: team_own_stats
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.2, median=1, min=1, max=2
-   last10: n=10, hits=10, rate=1.000, mean=0.8, median=1, min=0, max=2
-   all: n=30, hits=28, rate=0.933, mean=1.0333, median=1, min=0, max=4
- Split home/away:
-   home: n=15, hits=14, rate=0.933, mean=0.9333, median=1
-   away: n=15, hits=14, rate=0.933, mean=1.1333, median=1
- Rival (contexto equivalente):
  FM reportado: hits 4/5; mejor ventana: 5 (4/5)
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=1.8, median=2, min=1, max=2
    last10: n=10, hits=10, rate=1.000, mean=1.5, median=2, min=0, max=2
    all: n=30, hits=28, rate=0.933, mean=1.2, median=1, min=0, max=3
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=1.6, median=2
    away: n=15, hits=15, rate=1.000, mean=0.8, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 14. home_goals @ 2.5 — Montenegro (away)
- basis: team_own_stats
- FM reportado: hits 4/5; mejor ventana: 5 (4/5)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.8, median=2, min=1, max=2
-   last10: n=10, hits=10, rate=1.000, mean=1.5, median=2, min=0, max=2
-   all: n=30, hits=28, rate=0.933, mean=1.2, median=1, min=0, max=3
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=1.6, median=2
-   away: n=15, hits=15, rate=1.000, mean=0.8, median=1
- Rival (contexto equivalente):
  FM reportado: hits 10/10; mejor ventana: 10 (10/10)
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=1.2, median=1, min=1, max=2
    last10: n=10, hits=10, rate=1.000, mean=0.8, median=1, min=0, max=2
    all: n=30, hits=28, rate=0.933, mean=1.0333, median=1, min=0, max=4
  Split home/away:
    home: n=15, hits=14, rate=0.933, mean=0.9333, median=1
    away: n=15, hits=14, rate=0.933, mean=1.1333, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 15. total_goals @ 1.5 — Armenia vs Montenegro (sin rol)
- basis: match_total_goals
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=3.8, median=2, min=2, max=10
-   last10: n=10, hits=8, rate=0.800, mean=3.1, median=2, min=1, max=10
-   all: n=30, hits=24, rate=0.800, mean=3.1, median=2.5, min=1, max=10
- Split home/away:
-   home: n=15, hits=11, rate=0.733, mean=2.4, median=2
-   away: n=15, hits=13, rate=0.867, mean=3.8, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=3, median=3, min=1, max=5
    last10: n=10, hits=9, rate=0.900, mean=3.4, median=3.5, min=1, max=5
    all: n=30, hits=24, rate=0.800, mean=2.8, median=3, min=1, max=5
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=3, median=3
    away: n=15, hits=11, rate=0.733, mean=2.6, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 16. total_goals @ 2.5 — Armenia vs Montenegro (sin rol)
- basis: match_total_goals
- FM reportado: hits 8/10; mejor ventana: 10 (8/10)
- Ventanas propias:
-   last5: n=5, hits=3, rate=0.600, mean=3, median=3, min=1, max=5
-   last10: n=10, hits=8, rate=0.800, mean=3.4, median=3.5, min=1, max=5
-   all: n=30, hits=16, rate=0.533, mean=2.8, median=3, min=1, max=5
- Split home/away:
-   home: n=15, hits=9, rate=0.600, mean=3, median=3
-   away: n=15, hits=7, rate=0.467, mean=2.6, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=2, rate=0.400, mean=3.8, median=2, min=2, max=10
    last10: n=10, hits=4, rate=0.400, mean=3.1, median=2, min=1, max=10
    all: n=30, hits=15, rate=0.500, mean=3.1, median=2.5, min=1, max=10
  Split home/away:
    home: n=15, hits=6, rate=0.400, mean=2.4, median=2
    away: n=15, hits=9, rate=0.600, mean=3.8, median=3
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 17. total_goals @ 3.5 — Armenia vs Montenegro (sin rol)
- basis: match_total_goals
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=3.8, median=2, min=2, max=10
-   last10: n=10, hits=8, rate=0.800, mean=3.1, median=2, min=1, max=10
-   all: n=30, hits=22, rate=0.733, mean=3.1, median=2.5, min=1, max=10
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=2.4, median=2
-   away: n=15, hits=9, rate=0.600, mean=3.8, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=3, median=3, min=1, max=5
    last10: n=10, hits=5, rate=0.500, mean=3.4, median=3.5, min=1, max=5
    all: n=30, hits=18, rate=0.600, mean=2.8, median=3, min=1, max=5
  Split home/away:
    home: n=15, hits=9, rate=0.600, mean=3, median=3
    away: n=15, hits=9, rate=0.600, mean=2.6, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 18. 1x2_corners — Armenia (home)
- basis: unmapped
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: hits 7/8; mejor ventana: no determinable
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=true, source_conflict=false, motivo="non-numeric history value 'vt'; market '1x2_corners' has no mapping to fm_team_matches.team_stats_json"

### 19. 1x2_corners — Montenegro (away)
- basis: unmapped
- FM reportado: hits 7/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: hits 4/4; mejor ventana: no determinable
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=true, source_conflict=false, motivo="non-numeric history value 'vt'; market '1x2_corners' has no mapping to fm_team_matches.team_stats_json"

### 20. 1x2_shots — Armenia (home)
- basis: unmapped
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=true, source_conflict=false, motivo="non-numeric history value 'vt'; market '1x2_shots' has no mapping to fm_team_matches.team_stats_json"

### 21. 1x2_shots_on_target — Montenegro (away)
- basis: unmapped
- FM reportado: hits 4/5; mejor ventana: 5 (4/5)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=true, source_conflict=false, motivo="non-numeric history value 'vt'; market '1x2_shots_on_target' has no mapping to fm_team_matches.team_stats_json"

### 22. total_cards @ 4.5 — Armenia vs Montenegro (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 23. total_corners @ 6.5 — Armenia vs Montenegro (sin rol)
- basis: not_reproducible
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 24. total_corners @ 7.5 — Armenia vs Montenegro (sin rol)
- basis: not_reproducible
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 25. total_corners @ 8.5 — Armenia vs Montenegro (sin rol)
- basis: not_reproducible
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 26. total_corners @ 9.5 — Armenia vs Montenegro (sin rol)
- basis: not_reproducible
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 27. total_corners @ 10.5 — Armenia vs Montenegro (sin rol)
- basis: not_reproducible
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 28. total_corners @ 11.5 — Armenia vs Montenegro (sin rol)
- basis: not_reproducible
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 29. total_corners @ 12.5 — Armenia vs Montenegro (sin rol)
- basis: not_reproducible
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

## Señales de jugadores (orden: mayor muestra disponible, no fuerza de señal)

### 1. goals @ 0.5 — N. Krstović
- basis: player_own_stats
- FM reportado: hits 3/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=0.6, median=1, min=0, max=1
  last10: n=10, hits=3, rate=0.300, mean=0.3, median=0, min=0, max=1
  all: n=26, hits=7, rate=0.269, mean=0.3462, median=0, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 2. score_assist @ 0.5 — N. Krstović
- basis: player_own_stats
- FM reportado: hits 3/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=1, rate=0.200, mean=0.2, median=0, min=0, max=1
  last10: n=10, hits=2, rate=0.200, mean=0.2, median=0, min=0, max=1
  all: n=26, hits=3, rate=0.115, mean=0.1154, median=0, min=0, max=1
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 3. shots @ 1.5 — N. Krstović
- basis: player_own_stats
- FM reportado: hits 8/10; mejor ventana: 10 (8/10)
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=3.2, median=4, min=0, max=8
  last10: n=10, hits=8, rate=0.800, mean=4.2, median=3.5, min=0, max=11
  all: n=26, hits=19, rate=0.731, mean=3.1923, median=3, min=0, max=11
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 4. goals @ 0.5 — M. Osmajic
- basis: player_own_stats
- FM reportado: hits 4/5; mejor ventana: 5 (4/5)
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=0.8, median=1, min=0, max=2
  last10: n=10, hits=5, rate=0.500, mean=0.6, median=0.5, min=0, max=2
  all: n=20, hits=5, rate=0.250, mean=0.3, median=0, min=0, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 5. score_assist @ 0.5 — M. Osmajic
- basis: player_own_stats
- FM reportado: hits 4/5; mejor ventana: 5 (4/5)
- Ventanas propias:
  last5: n=5, hits=0, rate=0.000, mean=0, median=0, min=0, max=0
  last10: n=10, hits=0, rate=0.000, mean=0, median=0, min=0, max=0
  all: n=20, hits=0, rate=0.000, mean=0, median=0, min=0, max=0
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 6. foul_involvements @ 1.5 — N. Tiknizyan
- basis: player_own_stats
- FM reportado: hits 4/5; mejor ventana: 5 (4/5)
- Ventanas propias:
  last5: n=5, hits=1, rate=0.200, mean=0.6, median=0, min=0, max=2
  last10: n=10, hits=4, rate=0.400, mean=1.2, median=1, min=0, max=3
  all: n=15, hits=4, rate=0.267, mean=0.8667, median=1, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 7. foul_involvements @ 1.5 — E. Spertsyan
- basis: player_own_stats
- FM reportado: hits 6/7; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=2, rate=0.400, mean=1.4, median=1, min=0, max=4
  last10: n=10, hits=6, rate=0.600, mean=1.9, median=2, min=0, max=5
  all: n=12, hits=6, rate=0.500, mean=1.6667, median=1.5, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 8. foul_involvements @ 1.5 — M. Jankovic
- basis: player_own_stats
- FM reportado: hits 6/9; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=2, median=3, min=0, max=4
  last10: n=9, hits=6, rate=0.667, mean=3.2222, median=3, min=0, max=8
  all: n=9, hits=6, rate=0.667, mean=3.2222, median=3, min=0, max=8
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 9. foul_involvements @ 2.5 — M. Jankovic
- basis: player_own_stats
- FM reportado: hits 6/9; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=2, median=3, min=0, max=4
  last10: n=9, hits=6, rate=0.667, mean=3.2222, median=3, min=0, max=8
  all: n=9, hits=6, rate=0.667, mean=3.2222, median=3, min=0, max=8
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 10. tackles @ 1.5 — U. Iwu
- basis: player_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=2.8, median=3, min=2, max=4
  last10: n=8, hits=7, rate=0.875, mean=2.75, median=2.5, min=1, max=5
  all: n=8, hits=7, rate=0.875, mean=2.75, median=2.5, min=1, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 11. tackles @ 2.5 — U. Iwu
- basis: player_own_stats
- FM reportado: hits 3/5; mejor ventana: 5 (3/5)
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=2.8, median=3, min=2, max=4
  last10: n=8, hits=4, rate=0.500, mean=2.75, median=2.5, min=1, max=5
  all: n=8, hits=4, rate=0.500, mean=2.75, median=2.5, min=1, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 12. foul_involvements @ 1.5 — A. Serobyan
- basis: player_own_stats
- FM reportado: hits 3/5; mejor ventana: 5 (3/5)
- Ventanas propias:
  last5: n=5, hits=1, rate=0.200, mean=0.8, median=0, min=0, max=4
  last10: n=5, hits=1, rate=0.200, mean=0.8, median=0, min=0, max=4
  all: n=5, hits=1, rate=0.200, mean=0.8, median=0, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 13. foul_involvements @ 1.5 — S. Rubežić
- basis: player_own_stats
- FM reportado: hits 3/5; mejor ventana: 5 (3/5)
- Ventanas propias:
  last5: n=4, hits=1, rate=0.250, mean=1.5, median=0, min=0, max=6
  last10: n=4, hits=1, rate=0.250, mean=1.5, median=0, min=0, max=6
  all: n=4, hits=1, rate=0.250, mean=1.5, median=0, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

## Evidencia histórica (ventanas fijas)
- observaciones descriptivas: secuencia cruda (más antiguo -> más reciente) y hits de las ventanas fijas (last5/last10/all)

### Jugadores — evidencia con ventanas fijas
- N. Krstović — goals @ 0.5: [0,0,0,0,0,0,1,1,0,1] | last5 3/5 | last10 3/10 | all 7/26
- N. Krstović — score_assist @ 0.5: [1,0,0,0,0,0,0,0,0,1] | last5 1/5 | last10 2/10 | all 3/26
- N. Krstović — shots @ 1.5: [11,2,8,2,3,0,4,4,0,8] | last5 3/5 | last10 8/10 | all 19/26
- M. Osmajic — goals @ 0.5: [0,0,1,0,1,1,2,0,0,1] | last5 3/5 | last10 5/10 | all 5/20
- M. Osmajic — score_assist @ 0.5: [0,0,0,0,0,0,0,0,0,0] | last5 0/5 | last10 0/10 | all 0/20
- N. Tiknizyan — foul_involvements @ 1.5: [1,1,2,2,3,2,0,0,0,1] | last5 1/5 | last10 4/10 | all 4/15
- E. Spertsyan — foul_involvements @ 1.5: [0,2,5,3,2,4,1,0,0,2] | last5 2/5 | last10 6/10 | all 6/12
- M. Jankovic — foul_involvements @ 1.5: [7,3,8,1,4,3,0,0,3] | last5 3/5 | last10 6/9 | all 6/9
- M. Jankovic — foul_involvements @ 2.5: [7,3,8,1,4,3,0,0,3] | last5 3/5 | last10 6/9 | all 6/9
- U. Iwu — tackles @ 1.5: [2,5,1,3,4,2,3,2] | last5 5/5 | last10 7/8 | all 7/8
- U. Iwu — tackles @ 2.5: [2,5,1,3,4,2,3,2] | last5 3/5 | last10 4/8 | all 4/8
- A. Serobyan — foul_involvements @ 1.5: [0,0,0,0,4] | last5 1/5 | last10 1/5 | all 1/5
- S. Rubežić — foul_involvements @ 1.5: [0,6,0,0] | last5 1/4 | last10 1/4 | all 1/4

### Equipos — evidencia con ventanas fijas
- Armenia — away_goals @ 0.5: [0,2,0,0,0,1,1,1,1,2] | last5 5/5 | last10 6/10 | all 18/30
  - Rival (Montenegro): [0,0,2,2,2,2,2,1,2,2] | last5 5/5 | last10 8/10 | all 21/30
- Montenegro — away_goals @ 0.5: [0,0,2,2,2,2,2,1,2,2] | last5 5/5 | last10 8/10 | all 21/30
  - Rival (Armenia): [0,2,0,0,0,1,1,1,1,2] | last5 5/5 | last10 6/10 | all 18/30
- Montenegro — away_goals @ 1.5: [0,0,2,2,2,2,2,1,2,2] | last5 4/5 | last10 7/10 | all 13/30
  - Rival (Armenia): [0,2,0,0,0,1,1,1,1,2] | last5 1/5 | last10 2/10 | all 9/30
- Armenia — away_goals @ 2.5: [0,2,0,0,0,1,1,1,1,2] | last5 5/5 | last10 10/10 | all 28/30
  - Rival (Montenegro): [0,0,2,2,2,2,2,1,2,2] | last5 5/5 | last10 10/10 | all 28/30
- Montenegro — away_goals @ 2.5: [0,0,2,2,2,2,2,1,2,2] | last5 5/5 | last10 10/10 | all 28/30
  - Rival (Armenia): [0,2,0,0,0,1,1,1,1,2] | last5 5/5 | last10 10/10 | all 28/30
- Armenia — home_corners @ 3.5: [2,1,1,0,2,1,11,14,15,10] | last5 4/5 | last10 4/10 | all 19/30
  - Rival (Montenegro): [3,2,1,9,5,9,11,2,3,8] | last5 3/5 | last10 5/10 | all 20/30
- Armenia — home_corners @ 4.5: [2,1,1,0,2,1,11,14,15,10] | last5 4/5 | last10 4/10 | all 15/30
  - Rival (Montenegro): [3,2,1,9,5,9,11,2,3,8] | last5 3/5 | last10 5/10 | all 16/30
- Armenia — home_corners @ 5.5: [2,1,1,0,2,1,11,14,15,10] | last5 4/5 | last10 4/10 | all 12/30
  - Rival (Montenegro): [3,2,1,9,5,9,11,2,3,8] | last5 3/5 | last10 4/10 | all 10/30
- Armenia — home_corners @ 6.5: [2,1,1,0,2,1,11,14,15,10] | last5 4/5 | last10 4/10 | all 10/30
  - Rival (Montenegro): [3,2,1,9,5,9,11,2,3,8] | last5 3/5 | last10 4/10 | all 9/30
- Armenia — home_goals @ 0.5: [0,2,0,0,0,1,1,1,1,2] | last5 5/5 | last10 6/10 | all 18/30
  - Rival (Montenegro): [0,0,2,2,2,2,2,1,2,2] | last5 5/5 | last10 8/10 | all 21/30
- Montenegro — home_goals @ 0.5: [0,0,2,2,2,2,2,1,2,2] | last5 5/5 | last10 8/10 | all 21/30
  - Rival (Armenia): [0,2,0,0,0,1,1,1,1,2] | last5 5/5 | last10 6/10 | all 18/30
- Armenia — home_goals @ 1.5: [0,2,0,0,0,1,1,1,1,2] | last5 4/5 | last10 8/10 | all 21/30
  - Rival (Montenegro): [0,0,2,2,2,2,2,1,2,2] | last5 1/5 | last10 3/10 | all 17/30
- Armenia — home_goals @ 2.5: [0,2,0,0,0,1,1,1,1,2] | last5 5/5 | last10 10/10 | all 28/30
  - Rival (Montenegro): [0,0,2,2,2,2,2,1,2,2] | last5 5/5 | last10 10/10 | all 28/30
- Montenegro — home_goals @ 2.5: [0,0,2,2,2,2,2,1,2,2] | last5 5/5 | last10 10/10 | all 28/30
  - Rival (Armenia): [0,2,0,0,0,1,1,1,1,2] | last5 5/5 | last10 10/10 | all 28/30
- Armenia vs Montenegro — total_goals @ 1.5: [5,3,2,1,1,10,3,2,2,2] | last5 5/5 | last10 8/10 | all 24/30
- Armenia vs Montenegro — total_goals @ 2.5: [4,4,3,3,5,2,5,1,4,3] | last5 3/5 | last10 8/10 | all 16/30
- Armenia vs Montenegro — total_goals @ 3.5: [5,3,2,1,1,10,3,2,2,2] | last5 4/5 | last10 8/10 | all 22/30

## Confluencia descriptiva (equipo + rival + jugadores)
- familias de mercado con 2 o más series observadas en la DB; cada serie conserva su línea y sus ventanas fijas (last5/last10/all); sin score ni orden por fuerza

### corners
- Armenia (home) @ 3.5: last5 4/5 | last10 4/10 | all 19/30
- Montenegro (away) @ 3.5: last5 3/5 | last10 5/10 | all 20/30
- Armenia (home) @ 4.5: last5 4/5 | last10 4/10 | all 15/30
- Montenegro (away) @ 4.5: last5 3/5 | last10 5/10 | all 16/30
- Armenia (home) @ 5.5: last5 4/5 | last10 4/10 | all 12/30
- Montenegro (away) @ 5.5: last5 3/5 | last10 4/10 | all 10/30
- Armenia (home) @ 6.5: last5 4/5 | last10 4/10 | all 10/30
- Montenegro (away) @ 6.5: last5 3/5 | last10 4/10 | all 9/30

### foul_involvements
- jugador N. Tiknizyan — foul_involvements @ 1.5: last5 1/5 | last10 4/10 | all 4/15
- jugador E. Spertsyan — foul_involvements @ 1.5: last5 2/5 | last10 6/10 | all 6/12
- jugador M. Jankovic — foul_involvements @ 1.5: last5 3/5 | last10 6/9 | all 6/9
- jugador M. Jankovic — foul_involvements @ 2.5: last5 3/5 | last10 6/9 | all 6/9
- jugador A. Serobyan — foul_involvements @ 1.5: last5 1/5 | last10 1/5 | all 1/5
- jugador S. Rubežić — foul_involvements @ 1.5: last5 1/4 | last10 1/4 | all 1/4

### goals
- Armenia (home) @ 0.5: last5 5/5 | last10 6/10 | all 18/30
- Montenegro (away) @ 0.5: last5 5/5 | last10 8/10 | all 21/30
- Montenegro (away) @ 1.5: last5 4/5 | last10 7/10 | all 13/30
- Armenia (home) @ 1.5: last5 1/5 | last10 2/10 | all 9/30
- Armenia (home) @ 2.5: last5 5/5 | last10 10/10 | all 28/30
- Montenegro (away) @ 2.5: last5 5/5 | last10 10/10 | all 28/30
- jugador N. Krstović — goals @ 0.5: last5 3/5 | last10 3/10 | all 7/26
- jugador M. Osmajic — goals @ 0.5: last5 3/5 | last10 5/10 | all 5/20

### score_assist
- jugador N. Krstović — score_assist @ 0.5: last5 1/5 | last10 2/10 | all 3/26
- jugador M. Osmajic — score_assist @ 0.5: last5 0/5 | last10 0/10 | all 0/20

### tackles
- jugador U. Iwu — tackles @ 1.5: last5 5/5 | last10 7/8 | all 7/8
- jugador U. Iwu — tackles @ 2.5: last5 3/5 | last10 4/8 | all 4/8

## Cuotas

| mercado | línea | casa | lado | cuota | prob. implícita | capturada |
|---|---|---|---|---|---|---|
| away_goals | 0.5 | Bet365 | over | 1.33 | 0.752 | 2026-09-28T21:37:15.351Z |
| away_goals | 0.5 | Bet365 | under | 3.25 | 0.308 | 2026-09-28T21:37:15.351Z |
| away_goals | 0.5 | Kambi | over | 1.29 | 0.775 | 2026-09-28T21:37:15.351Z |
| away_goals | 0.5 | Kambi | under | 3.15 | 0.317 | 2026-09-28T21:37:15.351Z |
| away_goals | 0.5 | Ladbrokes | over | 1.25 | 0.800 | 2026-09-28T21:37:15.351Z |
| away_goals | 0.5 | Ladbrokes | under | 3.5 | 0.286 | 2026-09-28T21:37:15.351Z |
| away_goals | 0.5 | Bet365 | over | 1.33 | 0.752 | 2026-09-28T21:37:15.351Z |
| away_goals | 0.5 | Bet365 | under | 3.25 | 0.308 | 2026-09-28T21:37:15.351Z |
| away_goals | 0.5 | Kambi | over | 1.29 | 0.775 | 2026-09-28T21:37:15.351Z |
| away_goals | 0.5 | Kambi | under | 3.15 | 0.317 | 2026-09-28T21:37:15.351Z |
| away_goals | 0.5 | Ladbrokes | over | 1.25 | 0.800 | 2026-09-28T21:37:15.351Z |
| away_goals | 0.5 | Ladbrokes | under | 3.5 | 0.286 | 2026-09-28T21:37:15.351Z |
| away_goals | 1.5 | Bet365 | over | 2.5 | 0.400 | 2026-09-28T21:37:15.351Z |
| away_goals | 1.5 | Bet365 | under | 1.5 | 0.667 | 2026-09-28T21:37:15.351Z |
| away_goals | 1.5 | Kambi | over | 2.43 | 0.412 | 2026-09-28T21:37:15.351Z |
| away_goals | 1.5 | Kambi | under | 1.48 | 0.676 | 2026-09-28T21:37:15.351Z |
| away_goals | 1.5 | Ladbrokes | over | 2.37 | 0.422 | 2026-09-28T21:37:15.351Z |
| away_goals | 1.5 | Ladbrokes | under | 1.53 | 0.654 | 2026-09-28T21:37:15.351Z |
| away_goals | 2.5 | Bet365 | over | 5.5 | 0.182 | 2026-09-28T21:37:15.351Z |
| away_goals | 2.5 | Bet365 | under | 1.13 | 0.885 | 2026-09-28T21:37:15.351Z |
| away_goals | 2.5 | Ladbrokes | over | 5.25 | 0.190 | 2026-09-28T21:37:15.351Z |
| away_goals | 2.5 | Ladbrokes | under | 1.12 | 0.893 | 2026-09-28T21:37:15.351Z |
| away_goals | 2.5 | Bet365 | over | 5.5 | 0.182 | 2026-09-28T21:37:15.351Z |
| away_goals | 2.5 | Bet365 | under | 1.13 | 0.885 | 2026-09-28T21:37:15.351Z |
| away_goals | 2.5 | Ladbrokes | over | 5.25 | 0.190 | 2026-09-28T21:37:15.351Z |
| away_goals | 2.5 | Ladbrokes | under | 1.12 | 0.893 | 2026-09-28T21:37:15.351Z |
| home_corners | 3.5 | Kambi | over | 1.43 | 0.699 | 2026-09-28T21:37:15.351Z |
| home_corners | 3.5 | Kambi | under | 2.48 | 0.403 | 2026-09-28T21:37:15.351Z |
| home_corners | 4.5 | Bet365 | over | 1.83 | 0.546 | 2026-09-28T21:37:15.351Z |
| home_corners | 4.5 | Bet365 | under | 1.83 | 0.546 | 2026-09-28T21:37:15.351Z |
| home_corners | 4.5 | Kambi | over | 1.85 | 0.541 | 2026-09-28T21:37:15.351Z |
| home_corners | 4.5 | Kambi | under | 1.79 | 0.559 | 2026-09-28T21:37:15.351Z |
| home_corners | 4.5 | Ladbrokes | over | 1.85 | 0.541 | 2026-09-28T21:37:15.351Z |
| home_corners | 4.5 | Ladbrokes | under | 1.85 | 0.541 | 2026-09-28T21:37:15.351Z |
| home_corners | 5.5 | Kambi | over | 2.5 | 0.400 | 2026-09-28T21:37:15.351Z |
| home_corners | 5.5 | Kambi | under | 1.42 | 0.704 | 2026-09-28T21:37:15.351Z |
| home_corners | 6.5 | Kambi | over | 3.5 | 0.286 | 2026-09-28T21:37:15.351Z |
| home_corners | 6.5 | Kambi | under | 1.22 | 0.820 | 2026-09-28T21:37:15.351Z |
| home_goals | 0.5 | Bet365 | over | 1.36 | 0.735 | 2026-09-28T21:37:15.351Z |
| home_goals | 0.5 | Bet365 | under | 3 | 0.333 | 2026-09-28T21:37:15.351Z |
| home_goals | 0.5 | Kambi | over | 1.32 | 0.758 | 2026-09-28T21:37:15.351Z |
| home_goals | 0.5 | Kambi | under | 3 | 0.333 | 2026-09-28T21:37:15.351Z |
| home_goals | 0.5 | Ladbrokes | over | 1.33 | 0.752 | 2026-09-28T21:37:15.351Z |
| home_goals | 0.5 | Ladbrokes | under | 3.1 | 0.323 | 2026-09-28T21:37:15.351Z |
| home_goals | 0.5 | Bet365 | over | 1.36 | 0.735 | 2026-09-28T21:37:15.351Z |
| home_goals | 0.5 | Bet365 | under | 3 | 0.333 | 2026-09-28T21:37:15.351Z |
| home_goals | 0.5 | Kambi | over | 1.32 | 0.758 | 2026-09-28T21:37:15.351Z |
| home_goals | 0.5 | Kambi | under | 3 | 0.333 | 2026-09-28T21:37:15.351Z |
| home_goals | 0.5 | Ladbrokes | over | 1.33 | 0.752 | 2026-09-28T21:37:15.351Z |
| home_goals | 0.5 | Ladbrokes | under | 3.1 | 0.323 | 2026-09-28T21:37:15.351Z |
| home_goals | 1.5 | Bet365 | over | 2.63 | 0.380 | 2026-09-28T21:37:15.351Z |
| home_goals | 1.5 | Bet365 | under | 1.44 | 0.694 | 2026-09-28T21:37:15.351Z |
| home_goals | 1.5 | Kambi | over | 2.6 | 0.385 | 2026-09-28T21:37:15.351Z |
| home_goals | 1.5 | Kambi | under | 1.42 | 0.704 | 2026-09-28T21:37:15.351Z |
| home_goals | 1.5 | Ladbrokes | over | 2.7 | 0.370 | 2026-09-28T21:37:15.351Z |
| home_goals | 1.5 | Ladbrokes | under | 1.4 | 0.714 | 2026-09-28T21:37:15.351Z |
| home_goals | 2.5 | Bet365 | over | 6.5 | 0.154 | 2026-09-28T21:37:15.351Z |
| home_goals | 2.5 | Bet365 | under | 1.1 | 0.909 | 2026-09-28T21:37:15.351Z |
| home_goals | 2.5 | Ladbrokes | over | 6.5 | 0.154 | 2026-09-28T21:37:15.351Z |
| home_goals | 2.5 | Ladbrokes | under | 1.09 | 0.917 | 2026-09-28T21:37:15.351Z |
| home_goals | 2.5 | Bet365 | over | 6.5 | 0.154 | 2026-09-28T21:37:15.351Z |
| home_goals | 2.5 | Bet365 | under | 1.1 | 0.909 | 2026-09-28T21:37:15.351Z |
| home_goals | 2.5 | Ladbrokes | over | 6.5 | 0.154 | 2026-09-28T21:37:15.351Z |
| home_goals | 2.5 | Ladbrokes | under | 1.09 | 0.917 | 2026-09-28T21:37:15.351Z |
| total_goals | 1.5 | Bet365 | over | 1.33 | 0.752 | 2026-09-28T21:37:15.351Z |
| total_goals | 1.5 | Bet365 | under | 3.25 | 0.308 | 2026-09-28T21:37:15.351Z |
| total_goals | 1.5 | Kambi | over | 1.33 | 0.752 | 2026-09-28T21:37:15.351Z |
| total_goals | 1.5 | Kambi | under | 3.15 | 0.317 | 2026-09-28T21:37:15.351Z |
| total_goals | 1.5 | Ladbrokes | over | 1.3 | 0.769 | 2026-09-28T21:37:15.351Z |
| total_goals | 1.5 | Ladbrokes | under | 3.25 | 0.308 | 2026-09-28T21:37:15.351Z |
| total_goals | 1.5 | Paddy Power | over | 1.29 | 0.775 | 2026-09-28T21:37:15.351Z |
| total_goals | 1.5 | Paddy Power | under | 3.25 | 0.308 | 2026-09-28T21:37:15.351Z |
| total_goals | 2.5 | Altenar | over | 2 | 0.500 | 2026-09-28T21:37:15.351Z |
| total_goals | 2.5 | Altenar | under | 1.69 | 0.592 | 2026-09-28T21:37:15.351Z |
| total_goals | 2.5 | Bet365 | over | 2.08 | 0.481 | 2026-09-28T21:37:15.351Z |
| total_goals | 2.5 | Bet365 | under | 1.73 | 0.578 | 2026-09-28T21:37:15.351Z |
| total_goals | 2.5 | Kambi | over | 2.05 | 0.488 | 2026-09-28T21:37:15.351Z |
| total_goals | 2.5 | Kambi | under | 1.73 | 0.578 | 2026-09-28T21:37:15.351Z |
| total_goals | 2.5 | Ladbrokes | over | 1.95 | 0.513 | 2026-09-28T21:37:15.351Z |
| total_goals | 2.5 | Ladbrokes | under | 1.75 | 0.571 | 2026-09-28T21:37:15.351Z |
| total_goals | 2.5 | Paddy Power | over | 2 | 0.500 | 2026-09-28T21:37:15.351Z |
| total_goals | 2.5 | Paddy Power | under | 1.73 | 0.578 | 2026-09-28T21:37:15.351Z |
| total_goals | 3.5 | Bet365 | over | 3.75 | 0.267 | 2026-09-28T21:37:15.351Z |
| total_goals | 3.5 | Bet365 | under | 1.25 | 0.800 | 2026-09-28T21:37:15.351Z |
| total_goals | 3.5 | Kambi | over | 3.65 | 0.274 | 2026-09-28T21:37:15.351Z |
| total_goals | 3.5 | Kambi | under | 1.26 | 0.794 | 2026-09-28T21:37:15.351Z |
| total_goals | 3.5 | Ladbrokes | over | 3.4 | 0.294 | 2026-09-28T21:37:15.351Z |
| total_goals | 3.5 | Ladbrokes | under | 1.28 | 0.781 | 2026-09-28T21:37:15.351Z |
| total_goals | 3.5 | Paddy Power | over | 3.25 | 0.308 | 2026-09-28T21:37:15.351Z |
| total_goals | 3.5 | Paddy Power | under | 1.29 | 0.775 | 2026-09-28T21:37:15.351Z |
| total_cards | 4.5 | Kambi | over | 1.95 | 0.513 | 2026-09-28T21:37:15.351Z |
| total_cards | 4.5 | Kambi | under | 1.66 | 0.602 | 2026-09-28T21:37:15.351Z |
| total_corners | 6.5 | Bet365 | over | 1.18 | 0.847 | 2026-09-28T21:37:15.351Z |
| total_corners | 6.5 | Kambi | over | 1.18 | 0.847 | 2026-09-28T21:37:15.351Z |
| total_corners | 6.5 | Kambi | under | 4.1 | 0.244 | 2026-09-28T21:37:15.351Z |
| total_corners | 7.5 | Kambi | over | 1.34 | 0.746 | 2026-09-28T21:37:15.351Z |
| total_corners | 7.5 | Kambi | under | 2.9 | 0.345 | 2026-09-28T21:37:15.351Z |
| total_corners | 7.5 | Ladbrokes | over | 1.33 | 0.752 | 2026-09-28T21:37:15.351Z |
| total_corners | 7.5 | Ladbrokes | under | 3.1 | 0.323 | 2026-09-28T21:37:15.351Z |
| total_corners | 8.5 | Kambi | over | 1.58 | 0.633 | 2026-09-28T21:37:15.351Z |
| total_corners | 8.5 | Kambi | under | 2.17 | 0.461 | 2026-09-28T21:37:15.351Z |
| total_corners | 8.5 | Ladbrokes | over | 1.6 | 0.625 | 2026-09-28T21:37:15.351Z |
| total_corners | 8.5 | Ladbrokes | under | 2.2 | 0.455 | 2026-09-28T21:37:15.351Z |
| total_corners | 9.5 | Bet365 | over | 2 | 0.500 | 2026-09-28T21:37:15.351Z |
| total_corners | 9.5 | Bet365 | under | 1.73 | 0.578 | 2026-09-28T21:37:15.351Z |
| total_corners | 9.5 | Kambi | over | 1.95 | 0.513 | 2026-09-28T21:37:15.351Z |
| total_corners | 9.5 | Kambi | under | 1.73 | 0.578 | 2026-09-28T21:37:15.351Z |
| total_corners | 9.5 | Ladbrokes | over | 2 | 0.500 | 2026-09-28T21:37:15.351Z |
| total_corners | 9.5 | Ladbrokes | under | 1.73 | 0.578 | 2026-09-28T21:37:15.351Z |
| total_corners | 10.5 | Kambi | over | 2.48 | 0.403 | 2026-09-28T21:37:15.351Z |
| total_corners | 10.5 | Kambi | under | 1.45 | 0.690 | 2026-09-28T21:37:15.351Z |
| total_corners | 10.5 | Ladbrokes | over | 2.7 | 0.370 | 2026-09-28T21:37:15.351Z |
| total_corners | 10.5 | Ladbrokes | under | 1.4 | 0.714 | 2026-09-28T21:37:15.351Z |
| total_corners | 11.5 | Kambi | over | 3.25 | 0.308 | 2026-09-28T21:37:15.351Z |
| total_corners | 11.5 | Kambi | under | 1.27 | 0.787 | 2026-09-28T21:37:15.351Z |
| total_corners | 11.5 | Ladbrokes | over | 3.7 | 0.270 | 2026-09-28T21:37:15.351Z |
| total_corners | 11.5 | Ladbrokes | under | 1.25 | 0.800 | 2026-09-28T21:37:15.351Z |
| total_corners | 12.5 | Kambi | over | 4.3 | 0.233 | 2026-09-28T21:37:15.351Z |
| total_corners | 12.5 | Kambi | under | 1.16 | 0.862 | 2026-09-28T21:37:15.351Z |
- Cuota manual Betano (away_goals @ 0.5): no cargada
- Cuota manual Betano (away_goals @ 0.5): no cargada
- Cuota manual Betano (away_goals @ 1.5): no cargada
- Cuota manual Betano (away_goals @ 2.5): no cargada
- Cuota manual Betano (away_goals @ 2.5): no cargada
- Cuota manual Betano (home_corners @ 3.5): no cargada
- Cuota manual Betano (home_corners @ 4.5): no cargada
- Cuota manual Betano (home_corners @ 5.5): no cargada
- Cuota manual Betano (home_corners @ 6.5): no cargada
- Cuota manual Betano (home_goals @ 0.5): no cargada
- Cuota manual Betano (home_goals @ 0.5): no cargada
- Cuota manual Betano (home_goals @ 1.5): no cargada
- Cuota manual Betano (home_goals @ 2.5): no cargada
- Cuota manual Betano (home_goals @ 2.5): no cargada
- Cuota manual Betano (total_goals @ 1.5): no cargada
- Cuota manual Betano (total_goals @ 2.5): no cargada
- Cuota manual Betano (total_goals @ 3.5): no cargada
- Cuota manual Betano (1x2_corners): no cargada
- Cuota manual Betano (1x2_corners): no cargada
- Cuota manual Betano (1x2_shots): no cargada
- Cuota manual Betano (1x2_shots_on_target): no cargada
- Cuota manual Betano (total_cards @ 4.5): no cargada
- Cuota manual Betano (total_corners @ 6.5): no cargada
- Cuota manual Betano (total_corners @ 7.5): no cargada
- Cuota manual Betano (total_corners @ 8.5): no cargada
- Cuota manual Betano (total_corners @ 9.5): no cargada
- Cuota manual Betano (total_corners @ 10.5): no cargada
- Cuota manual Betano (total_corners @ 11.5): no cargada
- Cuota manual Betano (total_corners @ 12.5): no cargada

## Nota de cierre
Este reporte es descriptivo. No constituye una recomendación de apuesta ni una probabilidad validada de resultado futuro.

---

## Romania vs Bosnia and Herzegovina (33662314) - kickoff 2026-09-28T18:45:00.000Z

POST-PARTIDO - SOLO REGISTRO HISTORICO, NO ACCIONABLE

- Motivo de la marca: partido ya jugado (kickoff 2026-09-28T18:45:00.000Z); las cuotas en fm_market_odds son de cierre y las senales pueden reflejar el resultado ya conocido. Uso unico: dataset historico para calibracion/backtest.
- Snapshot capturado post-partido: 21:37:28Z-21:37:52Z (tabs overview/player-trends/team-trends, 3 navs; +2 fetch de scope partidos sin nav).
- No intentado - pausa del operador por lectura de un ledger manual desactualizado (docs/STATE.md); limite real de la API: 40 navs/corrida, 24 necesarias para los 8 fixtures. Corregido el 2026-09-28.
- Senales OK/SUSPECT persistidas: 72. El informe P11 nativo (descriptivo, sin Delta% ni candidatos) se lista debajo, tal como lo genera GET /api/fm/fixtures/{id}/report.md.

# Romania vs Bosnia and Herzegovina
- Competición: UEFA Nations League
- Kickoff (UTC): desconocido
- Flags de calidad: leakage=false; suspect=false
- sort_criteria: sample_size_desc

## Mercados (orden: mayor muestra disponible, no fuerza de señal)

### 1. away_cards @ 3.5 — Bosnia and Herzegovina (away)
- basis: team_own_stats
- FM reportado: hits 7/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=2.6, median=3, min=1, max=5
-   last10: n=10, hits=8, rate=0.800, mean=2.2, median=2.5, min=0, max=5
-   all: n=30, hits=23, rate=0.767, mean=2.4667, median=2, min=0, max=5
- Split home/away:
-   home: n=15, hits=11, rate=0.733, mean=2.4, median=2
-   away: n=15, hits=12, rate=0.800, mean=2.5333, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=2.2, median=1, min=0, max=5
    last10: n=10, hits=8, rate=0.800, mean=1.8, median=1.5, min=0, max=5
    all: n=30, hits=26, rate=0.867, mean=1.9333, median=2, min=0, max=5
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=1.6667, median=1
    away: n=15, hits=13, rate=0.867, mean=2.2, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 2. away_corners @ 4.5 — Bosnia and Herzegovina (away)
- basis: team_own_stats
- FM reportado: hits 8/10; mejor ventana: 10 (8/10)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=3.6, median=3, min=3, max=5
-   last10: n=10, hits=7, rate=0.700, mean=4.5, median=3, min=2, max=10
-   all: n=30, hits=21, rate=0.700, mean=3.9333, median=3, min=0, max=11
- Split home/away:
-   home: n=15, hits=10, rate=0.667, mean=4.4667, median=4
-   away: n=15, hits=11, rate=0.733, mean=3.4, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=2, rate=0.400, mean=5, median=6, min=0, max=10
    last10: n=10, hits=5, rate=0.500, mean=4.2, median=4.5, min=0, max=10
    all: n=30, hits=14, rate=0.467, mean=4.8, median=5, min=0, max=10
  Split home/away:
    home: n=15, hits=6, rate=0.400, mean=5.2, median=5
    away: n=15, hits=8, rate=0.533, mean=4.4, median=4
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 3. away_corners @ 5.5 — Bosnia and Herzegovina (away)
- basis: team_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=3.6, median=3, min=3, max=5
-   last10: n=10, hits=8, rate=0.800, mean=4.5, median=3, min=2, max=10
-   all: n=30, hits=25, rate=0.833, mean=3.9333, median=3, min=0, max=11
- Split home/away:
-   home: n=15, hits=12, rate=0.800, mean=4.4667, median=4
-   away: n=15, hits=13, rate=0.867, mean=3.4, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=2, rate=0.400, mean=5, median=6, min=0, max=10
    last10: n=10, hits=5, rate=0.500, mean=4.2, median=4.5, min=0, max=10
    all: n=30, hits=18, rate=0.600, mean=4.8, median=5, min=0, max=10
  Split home/away:
    home: n=15, hits=10, rate=0.667, mean=5.2, median=5
    away: n=15, hits=8, rate=0.533, mean=4.4, median=4
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 4. away_goals @ 0.5 — Romania (home)
- basis: team_own_stats
- FM reportado: hits 7/7; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=3, rate=0.600, mean=0.8, median=1, min=0, max=2
-   last10: n=10, hits=8, rate=0.800, mean=1.7, median=1, min=0, max=7
-   all: n=30, hits=22, rate=0.733, mean=1.6667, median=1, min=0, max=7
- Split home/away:
-   home: n=15, hits=10, rate=0.667, mean=1.7333, median=1
-   away: n=15, hits=12, rate=0.800, mean=1.6, median=1
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=1, median=1, min=0, max=3
    last10: n=10, hits=7, rate=0.700, mean=0.9, median=1, min=0, max=3
    all: n=30, hits=22, rate=0.733, mean=1.2667, median=1, min=0, max=6
  Split home/away:
    home: n=15, hits=11, rate=0.733, mean=1.1333, median=1
    away: n=15, hits=11, rate=0.733, mean=1.4, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 5. away_goals @ 1.5 — Bosnia and Herzegovina (away)
- basis: team_own_stats
- FM reportado: hits 7/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=1, median=1, min=0, max=3
-   last10: n=10, hits=9, rate=0.900, mean=0.9, median=1, min=0, max=3
-   all: n=30, hits=22, rate=0.733, mean=1.2667, median=1, min=0, max=6
- Split home/away:
-   home: n=15, hits=11, rate=0.733, mean=1.1333, median=1
-   away: n=15, hits=11, rate=0.733, mean=1.4, median=1
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=0.8, median=1, min=0, max=2
    last10: n=10, hits=6, rate=0.600, mean=1.7, median=1, min=0, max=7
    all: n=30, hits=16, rate=0.533, mean=1.6667, median=1, min=0, max=7
  Split home/away:
    home: n=15, hits=8, rate=0.533, mean=1.7333, median=1
    away: n=15, hits=8, rate=0.533, mean=1.6, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 6. away_goals @ 2.5 — Bosnia and Herzegovina (away)
- basis: team_own_stats
- FM reportado: hits 7/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=1, median=1, min=0, max=3
-   last10: n=10, hits=9, rate=0.900, mean=0.9, median=1, min=0, max=3
-   all: n=30, hits=26, rate=0.867, mean=1.2667, median=1, min=0, max=6
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=1.1333, median=1
-   away: n=15, hits=13, rate=0.867, mean=1.4, median=1
- Rival (contexto equivalente):
  FM reportado: hits 6/6; mejor ventana: no determinable
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=0.8, median=1, min=0, max=2
    last10: n=10, hits=9, rate=0.900, mean=1.7, median=1, min=0, max=7
    all: n=30, hits=23, rate=0.767, mean=1.6667, median=1, min=0, max=7
  Split home/away:
    home: n=15, hits=11, rate=0.733, mean=1.7333, median=1
    away: n=15, hits=12, rate=0.800, mean=1.6, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 7. away_goals @ 2.5 — Romania (home)
- basis: team_own_stats
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=0.8, median=1, min=0, max=2
-   last10: n=10, hits=9, rate=0.900, mean=1.7, median=1, min=0, max=7
-   all: n=30, hits=23, rate=0.767, mean=1.6667, median=1, min=0, max=7
- Split home/away:
-   home: n=15, hits=11, rate=0.733, mean=1.7333, median=1
-   away: n=15, hits=12, rate=0.800, mean=1.6, median=1
- Rival (contexto equivalente):
  FM reportado: hits 7/8; mejor ventana: no determinable
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=1, median=1, min=0, max=3
    last10: n=10, hits=9, rate=0.900, mean=0.9, median=1, min=0, max=3
    all: n=30, hits=26, rate=0.867, mean=1.2667, median=1, min=0, max=6
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=1.1333, median=1
    away: n=15, hits=13, rate=0.867, mean=1.4, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 8. away_shots @ 10.5 — Bosnia and Herzegovina (away)
- basis: team_own_stats
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=9.2, median=9, min=5, max=14
-   last10: n=10, hits=6, rate=0.600, mean=11.2, median=9.5, min=1, max=30
-   all: n=30, hits=17, rate=0.567, mean=10.9, median=9.5, min=1, max=30
- Split home/away:
-   home: n=15, hits=7, rate=0.467, mean=12.4, median=12
-   away: n=15, hits=10, rate=0.667, mean=9.4, median=8
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=10.4, median=9, min=5, max=22
    last10: n=10, hits=7, rate=0.700, mean=11.6, median=9.5, min=5, max=25
    all: n=30, hits=13, rate=0.433, mean=13.2, median=11, min=5, max=31
  Split home/away:
    home: n=15, hits=5, rate=0.333, mean=14.8, median=12
    away: n=15, hits=8, rate=0.533, mean=11.6, median=10
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 9. away_shots @ 12.5 — Bosnia and Herzegovina (away)
- basis: team_own_stats
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=9.2, median=9, min=5, max=14
-   last10: n=10, hits=6, rate=0.600, mean=11.2, median=9.5, min=1, max=30
-   all: n=30, hits=19, rate=0.633, mean=10.9, median=9.5, min=1, max=30
- Split home/away:
-   home: n=15, hits=8, rate=0.533, mean=12.4, median=12
-   away: n=15, hits=11, rate=0.733, mean=9.4, median=8
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=10.4, median=9, min=5, max=22
    last10: n=10, hits=8, rate=0.800, mean=11.6, median=9.5, min=5, max=25
    all: n=30, hits=17, rate=0.567, mean=13.2, median=11, min=5, max=31
  Split home/away:
    home: n=15, hits=8, rate=0.533, mean=14.8, median=12
    away: n=15, hits=9, rate=0.600, mean=11.6, median=10
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 10. away_shots_on_target @ 3.5 — Bosnia and Herzegovina (away)
- basis: team_own_stats
- FM reportado: hits 7/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=3, median=3, min=1, max=5
-   last10: n=10, hits=7, rate=0.700, mean=3.6, median=3, min=1, max=11
-   all: n=30, hits=20, rate=0.667, mean=3.5667, median=3, min=0, max=11
- Split home/away:
-   home: n=15, hits=9, rate=0.600, mean=3.7333, median=3
-   away: n=15, hits=11, rate=0.733, mean=3.4, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=2.8, median=1, min=0, max=8
    last10: n=10, hits=5, rate=0.500, mean=4.4, median=3.5, min=0, max=13
    all: n=30, hits=11, rate=0.367, mean=5.0333, median=5, min=0, max=13
  Split home/away:
    home: n=15, hits=6, rate=0.400, mean=5.4667, median=5
    away: n=15, hits=5, rate=0.333, mean=4.6, median=5
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 11. away_shots_on_target @ 4.5 — Bosnia and Herzegovina (away)
- basis: team_own_stats
- FM reportado: hits 8/9; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=3, median=3, min=1, max=5
-   last10: n=10, hits=7, rate=0.700, mean=3.6, median=3, min=1, max=11
-   all: n=30, hits=22, rate=0.733, mean=3.5667, median=3, min=0, max=11
- Split home/away:
-   home: n=15, hits=11, rate=0.733, mean=3.7333, median=3
-   away: n=15, hits=11, rate=0.733, mean=3.4, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=2.8, median=1, min=0, max=8
    last10: n=10, hits=6, rate=0.600, mean=4.4, median=3.5, min=0, max=13
    all: n=30, hits=14, rate=0.467, mean=5.0333, median=5, min=0, max=13
  Split home/away:
    home: n=15, hits=7, rate=0.467, mean=5.4667, median=5
    away: n=15, hits=7, rate=0.467, mean=4.6, median=5
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 12. home_goals @ 0.5 — Bosnia and Herzegovina (away)
- basis: team_own_stats
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=3, rate=0.600, mean=1, median=1, min=0, max=3
-   last10: n=10, hits=7, rate=0.700, mean=0.9, median=1, min=0, max=3
-   all: n=30, hits=22, rate=0.733, mean=1.2667, median=1, min=0, max=6
- Split home/away:
-   home: n=15, hits=11, rate=0.733, mean=1.1333, median=1
-   away: n=15, hits=11, rate=0.733, mean=1.4, median=1
- Rival (contexto equivalente):
  FM reportado: hits 8/10; mejor ventana: 10 (8/10)
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=0.8, median=1, min=0, max=2
    last10: n=10, hits=8, rate=0.800, mean=1.7, median=1, min=0, max=7
    all: n=30, hits=22, rate=0.733, mean=1.6667, median=1, min=0, max=7
  Split home/away:
    home: n=15, hits=10, rate=0.667, mean=1.7333, median=1
    away: n=15, hits=12, rate=0.800, mean=1.6, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 13. home_goals @ 0.5 — Romania (home)
- basis: team_own_stats
- FM reportado: hits 8/10; mejor ventana: 10 (8/10)
- Ventanas propias:
-   last5: n=5, hits=3, rate=0.600, mean=0.8, median=1, min=0, max=2
-   last10: n=10, hits=8, rate=0.800, mean=1.7, median=1, min=0, max=7
-   all: n=30, hits=22, rate=0.733, mean=1.6667, median=1, min=0, max=7
- Split home/away:
-   home: n=15, hits=10, rate=0.667, mean=1.7333, median=1
-   away: n=15, hits=12, rate=0.800, mean=1.6, median=1
- Rival (contexto equivalente):
  FM reportado: hits 5/6; mejor ventana: no determinable
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=1, median=1, min=0, max=3
    last10: n=10, hits=7, rate=0.700, mean=0.9, median=1, min=0, max=3
    all: n=30, hits=22, rate=0.733, mean=1.2667, median=1, min=0, max=6
  Split home/away:
    home: n=15, hits=11, rate=0.733, mean=1.1333, median=1
    away: n=15, hits=11, rate=0.733, mean=1.4, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 14. home_goals @ 2.5 — Bosnia and Herzegovina (away)
- basis: team_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=1, median=1, min=0, max=3
-   last10: n=10, hits=9, rate=0.900, mean=0.9, median=1, min=0, max=3
-   all: n=30, hits=26, rate=0.867, mean=1.2667, median=1, min=0, max=6
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=1.1333, median=1
-   away: n=15, hits=13, rate=0.867, mean=1.4, median=1
- Rival (contexto equivalente):
  FM reportado: hits 5/5; mejor ventana: 5 (5/5)
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=0.8, median=1, min=0, max=2
    last10: n=10, hits=9, rate=0.900, mean=1.7, median=1, min=0, max=7
    all: n=30, hits=23, rate=0.767, mean=1.6667, median=1, min=0, max=7
  Split home/away:
    home: n=15, hits=11, rate=0.733, mean=1.7333, median=1
    away: n=15, hits=12, rate=0.800, mean=1.6, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 15. home_goals @ 2.5 — Romania (home)
- basis: team_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=0.8, median=1, min=0, max=2
-   last10: n=10, hits=9, rate=0.900, mean=1.7, median=1, min=0, max=7
-   all: n=30, hits=23, rate=0.767, mean=1.6667, median=1, min=0, max=7
- Split home/away:
-   home: n=15, hits=11, rate=0.733, mean=1.7333, median=1
-   away: n=15, hits=12, rate=0.800, mean=1.6, median=1
- Rival (contexto equivalente):
  FM reportado: hits 9/10; mejor ventana: 10 (9/10)
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=1, median=1, min=0, max=3
    last10: n=10, hits=9, rate=0.900, mean=0.9, median=1, min=0, max=3
    all: n=30, hits=26, rate=0.867, mean=1.2667, median=1, min=0, max=6
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=1.1333, median=1
    away: n=15, hits=13, rate=0.867, mean=1.4, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 16. home_shots @ 13.5 — Romania (home)
- basis: team_own_stats
- FM reportado: hits 8/10; mejor ventana: 10 (8/10)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=10.4, median=9, min=5, max=22
-   last10: n=10, hits=8, rate=0.800, mean=11.6, median=9.5, min=5, max=25
-   all: n=30, hits=18, rate=0.600, mean=13.2, median=11, min=5, max=31
- Split home/away:
-   home: n=15, hits=8, rate=0.533, mean=14.8, median=12
-   away: n=15, hits=10, rate=0.667, mean=11.6, median=10
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=9.2, median=9, min=5, max=14
    last10: n=10, hits=7, rate=0.700, mean=11.2, median=9.5, min=1, max=30
    all: n=30, hits=20, rate=0.667, mean=10.9, median=9.5, min=1, max=30
  Split home/away:
    home: n=15, hits=9, rate=0.600, mean=12.4, median=12
    away: n=15, hits=11, rate=0.733, mean=9.4, median=8
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 17. home_shots @ 15.5 — Romania (home)
- basis: team_own_stats
- FM reportado: hits 8/10; mejor ventana: 10 (8/10)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=10.4, median=9, min=5, max=22
-   last10: n=10, hits=8, rate=0.800, mean=11.6, median=9.5, min=5, max=25
-   all: n=30, hits=22, rate=0.733, mean=13.2, median=11, min=5, max=31
- Split home/away:
-   home: n=15, hits=10, rate=0.667, mean=14.8, median=12
-   away: n=15, hits=12, rate=0.800, mean=11.6, median=10
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=9.2, median=9, min=5, max=14
    last10: n=10, hits=9, rate=0.900, mean=11.2, median=9.5, min=1, max=30
    all: n=30, hits=25, rate=0.833, mean=10.9, median=9.5, min=1, max=30
  Split home/away:
    home: n=15, hits=11, rate=0.733, mean=12.4, median=12
    away: n=15, hits=14, rate=0.933, mean=9.4, median=8
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 18. total_goals @ 1.5 — Romania vs Bosnia and Herzegovina (sin rol)
- basis: match_total_goals
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=2.2, median=2, min=1, max=3
-   last10: n=10, hits=8, rate=0.800, mean=3.1, median=3, min=1, max=8
-   all: n=30, hits=24, rate=0.800, mean=2.8333, median=3, min=0, max=8
- Split home/away:
-   home: n=15, hits=10, rate=0.667, mean=2.6, median=3
-   away: n=15, hits=14, rate=0.933, mean=3.0667, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=2.6, median=2, min=0, max=5
    last10: n=10, hits=8, rate=0.800, mean=2.1, median=2, min=0, max=5
    all: n=30, hits=25, rate=0.833, mean=2.9333, median=3, min=0, max=7
  Split home/away:
    home: n=15, hits=13, rate=0.867, mean=2.6667, median=3
    away: n=15, hits=12, rate=0.800, mean=3.2, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 19. total_goals @ 3.5 — Romania vs Bosnia and Herzegovina (sin rol)
- basis: match_total_goals
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=2.2, median=2, min=1, max=3
-   last10: n=10, hits=7, rate=0.700, mean=3.1, median=3, min=1, max=8
-   all: n=30, hits=23, rate=0.767, mean=2.8333, median=3, min=0, max=8
- Split home/away:
-   home: n=15, hits=12, rate=0.800, mean=2.6, median=3
-   away: n=15, hits=11, rate=0.733, mean=3.0667, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=2.6, median=2, min=0, max=5
    last10: n=10, hits=8, rate=0.800, mean=2.1, median=2, min=0, max=5
    all: n=30, hits=21, rate=0.700, mean=2.9333, median=3, min=0, max=7
  Split home/away:
    home: n=15, hits=12, rate=0.800, mean=2.6667, median=3
    away: n=15, hits=9, rate=0.600, mean=3.2, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 20. total_cards @ 5.5 — Romania vs Bosnia and Herzegovina (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 21. total_cards @ 6.5 — Romania vs Bosnia and Herzegovina (sin rol)
- basis: not_reproducible
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 22. total_corners @ 5.5 — Romania vs Bosnia and Herzegovina (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 23. total_corners @ 6.5 — Romania vs Bosnia and Herzegovina (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 24. total_corners @ 10.5 — Romania vs Bosnia and Herzegovina (sin rol)
- basis: not_reproducible
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 25. total_corners @ 11.5 — Romania vs Bosnia and Herzegovina (sin rol)
- basis: not_reproducible
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 26. total_corners @ 12.5 — Romania vs Bosnia and Herzegovina (sin rol)
- basis: not_reproducible
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 27. total_shots @ 24.5 — Romania vs Bosnia and Herzegovina (sin rol)
- basis: not_reproducible
- FM reportado: hits 8/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 28. total_shots @ 26.5 — Romania vs Bosnia and Herzegovina (sin rol)
- basis: not_reproducible
- FM reportado: hits 8/10; mejor ventana: 10 (8/10)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 29. total_shots_on_target @ 8.5 — Romania vs Bosnia and Herzegovina (sin rol)
- basis: not_reproducible
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 30. total_shots_on_target @ 9.5 — Romania vs Bosnia and Herzegovina (sin rol)
- basis: not_reproducible
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

## Señales de jugadores (orden: mayor muestra disponible, no fuerza de señal)

### 1. shots @ 0.5 — E. Demirovic
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1, median=1, min=0, max=2
  last10: n=10, hits=7, rate=0.700, mean=1.6, median=1, min=0, max=6
  all: n=23, hits=18, rate=0.783, mean=1.4783, median=1, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 2. shots @ 0.5 — N. Stanciu
- basis: player_own_stats
- FM reportado: hits 6/7; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.4, median=1, min=0, max=3
  last10: n=10, hits=8, rate=0.800, mean=1.6, median=1.5, min=0, max=4
  all: n=23, hits=17, rate=0.739, mean=1.5217, median=1, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 3. shots @ 0.5 — V. Mihăilă
- basis: player_own_stats
- FM reportado: hits 7/9; mejor ventana: 10 (7/9)
- Ventanas propias:
  last5: n=5, hits=2, rate=0.400, mean=0.6, median=0, min=0, max=2
  last10: n=10, hits=6, rate=0.600, mean=0.9, median=1, min=0, max=2
  all: n=22, hits=12, rate=0.546, mean=1.2273, median=1, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 4. shots @ 0.5 — E. Bajraktarevic
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1, median=1, min=0, max=2
  last10: n=10, hits=6, rate=0.600, mean=1.1, median=1, min=0, max=3
  all: n=21, hits=10, rate=0.476, mean=0.9048, median=0, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 5. shots @ 0.5 — A. Memić
- basis: player_own_stats
- FM reportado: hits 5/7; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=0.6, median=1, min=0, max=1
  last10: n=10, hits=5, rate=0.500, mean=0.7, median=0.5, min=0, max=2
  all: n=19, hits=7, rate=0.368, mean=0.4737, median=0, min=0, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 6. shots @ 0.5 — K. Alajbegovic
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1.2, median=2, min=0, max=2
  last10: n=10, hits=6, rate=0.600, mean=1.3, median=1.5, min=0, max=3
  all: n=15, hits=9, rate=0.600, mean=1.3333, median=1, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 7. shots @ 0.5 — S. Radeljic
- basis: player_own_stats
- FM reportado: hits 4/6; mejor ventana: 10 (4/6)
- Ventanas propias:
  last5: n=5, hits=1, rate=0.200, mean=0.2, median=0, min=0, max=1
  last10: n=10, hits=2, rate=0.200, mean=0.2, median=0, min=0, max=1
  all: n=15, hits=4, rate=0.267, mean=0.3333, median=0, min=0, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 8. shots @ 1.5 — K. Alajbegovic
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1.2, median=2, min=0, max=2
  last10: n=10, hits=5, rate=0.500, mean=1.3, median=1.5, min=0, max=3
  all: n=15, hits=6, rate=0.400, mean=1.3333, median=1, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 9. fouls_committed @ 0.5 — N. Katic
- basis: player_own_stats
- FM reportado: hits 3/5; mejor ventana: 5 (3/5)
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=0.8, median=1, min=0, max=2
  last10: n=10, hits=6, rate=0.600, mean=1.3, median=1, min=0, max=4
  all: n=12, hits=7, rate=0.583, mean=1.3333, median=1, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 10. tackles @ 1.5 — N. Katic
- basis: player_own_stats
- FM reportado: hits 4/5; mejor ventana: 5 (4/5)
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.4, median=2, min=0, max=5
  last10: n=10, hits=6, rate=0.600, mean=1.9, median=2, min=0, max=5
  all: n=12, hits=7, rate=0.583, mean=1.8333, median=2, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 11. foul_involvements @ 1.5 — E. Demirovic
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=3.2, median=4, min=0, max=5
  last10: n=10, hits=7, rate=0.700, mean=3.1, median=4, min=0, max=6
  all: n=10, hits=7, rate=0.700, mean=3.1, median=4, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 12. foul_involvements @ 1.5 — E. Džeko
- basis: player_own_stats
- FM reportado: hits 5/7; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=2, rate=0.400, mean=1.4, median=0, min=0, max=4
  last10: n=10, hits=6, rate=0.600, mean=2.2, median=2.5, min=0, max=5
  all: n=10, hits=6, rate=0.600, mean=2.2, median=2.5, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 13. foul_involvements @ 1.5 — I. Šunjić
- basis: player_own_stats
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.8, median=2, min=1, max=2
  last10: n=10, hits=8, rate=0.800, mean=2.5, median=2, min=0, max=6
  all: n=10, hits=8, rate=0.800, mean=2.5, median=2, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 14. foul_involvements @ 2.5 — E. Demirovic
- basis: player_own_stats
- FM reportado: hits 8/10; mejor ventana: 10 (8/10)
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=3.2, median=4, min=0, max=5
  last10: n=10, hits=6, rate=0.600, mean=3.1, median=4, min=0, max=6
  all: n=10, hits=6, rate=0.600, mean=3.1, median=4, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 15. foul_involvements @ 2.5 — E. Džeko
- basis: player_own_stats
- FM reportado: hits 4/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=2, rate=0.400, mean=1.4, median=0, min=0, max=4
  last10: n=10, hits=5, rate=0.500, mean=2.2, median=2.5, min=0, max=5
  all: n=10, hits=5, rate=0.500, mean=2.2, median=2.5, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 16. foul_involvements @ 3.5 — E. Demirovic
- basis: player_own_stats
- FM reportado: hits 5/7; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=3.2, median=4, min=0, max=5
  last10: n=10, hits=6, rate=0.600, mean=3.1, median=4, min=0, max=6
  all: n=10, hits=6, rate=0.600, mean=3.1, median=4, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 17. fouls_committed @ 1.5 — E. Demirovic
- basis: player_own_stats
- FM reportado: hits 7/10; mejor ventana: 10 (7/10)
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=2.4, median=3, min=0, max=4
  last10: n=10, hits=5, rate=0.500, mean=1.9, median=1.5, min=0, max=4
  all: n=10, hits=5, rate=0.500, mean=1.9, median=1.5, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 18. fouls_committed @ 1.5 — I. Šunjić
- basis: player_own_stats
- FM reportado: hits 7/10; mejor ventana: 10 (7/10)
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1.6, median=2, min=1, max=2
  last10: n=10, hits=6, rate=0.600, mean=2, median=2, min=0, max=6
  all: n=10, hits=6, rate=0.600, mean=2, median=2, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 19. fouls_committed @ 2.5 — E. Demirovic
- basis: player_own_stats
- FM reportado: hits 7/10; mejor ventana: 10 (7/10)
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=2.4, median=3, min=0, max=4
  last10: n=10, hits=4, rate=0.400, mean=1.9, median=1.5, min=0, max=4
  all: n=10, hits=4, rate=0.400, mean=1.9, median=1.5, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 20. fouls_drawn @ 1.5 — E. Džeko
- basis: player_own_stats
- FM reportado: hits 5/7; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=2, rate=0.400, mean=0.8, median=0, min=0, max=2
  last10: n=10, hits=6, rate=0.600, mean=1.5, median=2, min=0, max=3
  all: n=10, hits=6, rate=0.600, mean=1.5, median=2, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 21. fouls_committed @ 0.5 — A. Memić
- basis: player_own_stats
- FM reportado: hits 4/5; mejor ventana: 5 (4/5)
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=0.8, median=1, min=0, max=2
  last10: n=9, hits=5, rate=0.556, mean=0.6667, median=1, min=0, max=2
  all: n=9, hits=5, rate=0.556, mean=0.6667, median=1, min=0, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 22. foul_involvements @ 1.5 — S. Kolasinac
- basis: player_own_stats
- FM reportado: hits 4/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=2.8, median=3, min=1, max=5
  last10: n=8, hits=4, rate=0.500, mean=2.25, median=2, min=0, max=5
  all: n=8, hits=4, rate=0.500, mean=2.25, median=2, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 23. foul_involvements @ 2.5 — S. Kolasinac
- basis: player_own_stats
- FM reportado: hits 4/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=2.8, median=3, min=1, max=5
  last10: n=8, hits=4, rate=0.500, mean=2.25, median=2, min=0, max=5
  all: n=8, hits=4, rate=0.500, mean=2.25, median=2, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 24. fouls_committed @ 0.5 — K. Alajbegovic
- basis: player_own_stats
- FM reportado: hits 4/5; mejor ventana: 5 (4/5)
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1, median=1, min=0, max=2
  last10: n=8, hits=5, rate=0.625, mean=1.25, median=1, min=0, max=4
  all: n=8, hits=5, rate=0.625, mean=1.25, median=1, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 25. fouls_committed @ 0.5 — S. Kolasinac
- basis: player_own_stats
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.6, median=2, min=0, max=3
  last10: n=8, hits=6, rate=0.750, mean=1.375, median=1.5, min=0, max=3
  all: n=8, hits=6, rate=0.750, mean=1.375, median=1.5, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 26. fouls_committed @ 1.5 — S. Kolasinac
- basis: player_own_stats
- FM reportado: hits 4/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1.6, median=2, min=0, max=3
  last10: n=8, hits=4, rate=0.500, mean=1.375, median=1.5, min=0, max=3
  all: n=8, hits=4, rate=0.500, mean=1.375, median=1.5, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 27. tackles @ 1.5 — S. Kolasinac
- basis: player_own_stats
- FM reportado: hits 4/5; mejor ventana: 5 (4/5)
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2, median=2, min=1, max=3
  last10: n=8, hits=5, rate=0.625, mean=1.625, median=2, min=0, max=3
  all: n=8, hits=5, rate=0.625, mean=1.625, median=2, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 28. foul_involvements @ 1.5 — N. Stanciu
- basis: player_own_stats
- FM reportado: hits 7/10; mejor ventana: 10 (7/10)
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.8, median=2, min=0, max=3
  last10: n=7, hits=6, rate=0.857, mean=2.5714, median=2, min=0, max=6
  all: n=7, hits=6, rate=0.857, mean=2.5714, median=2, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 29. foul_involvements @ 1.5 — V. Dragomir
- basis: player_own_stats
- FM reportado: hits 7/9; mejor ventana: 10 (7/9)
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.8, median=2, min=0, max=3
  last10: n=7, hits=6, rate=0.857, mean=2.8571, median=2, min=0, max=7
  all: n=7, hits=6, rate=0.857, mean=2.8571, median=2, min=0, max=7
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 30. fouls_committed @ 0.5 — N. Stanciu
- basis: player_own_stats
- FM reportado: hits 5/7; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.2, median=1, min=0, max=3
  last10: n=7, hits=5, rate=0.714, mean=1, median=1, min=0, max=3
  all: n=7, hits=5, rate=0.714, mean=1, median=1, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 31. fouls_committed @ 0.5 — V. Dragomir
- basis: player_own_stats
- FM reportado: hits 7/9; mejor ventana: 10 (7/9)
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.2, median=1, min=0, max=2
  last10: n=7, hits=6, rate=0.857, mean=1.7143, median=1, min=0, max=5
  all: n=7, hits=6, rate=0.857, mean=1.7143, median=1, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 32. shots @ 0.5 — E. Mahmic
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.2, median=1, min=0, max=2
  last10: n=7, hits=4, rate=0.571, mean=0.8571, median=1, min=0, max=2
  all: n=7, hits=4, rate=0.571, mean=0.8571, median=1, min=0, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 33. foul_involvements @ 1.5 — D. Bîrligea
- basis: player_own_stats
- FM reportado: hits 3/5; mejor ventana: 5 (3/5)
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1.8, median=2, min=0, max=3
  last10: n=6, hits=3, rate=0.500, mean=1.6667, median=1.5, min=0, max=3
  all: n=6, hits=3, rate=0.500, mean=1.6667, median=1.5, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 34. foul_involvements @ 1.5 — V. Mihăilă
- basis: player_own_stats
- FM reportado: hits 3/5; mejor ventana: 5 (3/5)
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1.4, median=2, min=0, max=3
  last10: n=5, hits=3, rate=0.600, mean=1.4, median=2, min=0, max=3
  all: n=5, hits=3, rate=0.600, mean=1.4, median=2, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 35. foul_involvements @ 1.5 — S. Radeljic
- basis: player_own_stats
- FM reportado: hits 3/5; mejor ventana: 5 (3/5)
- Ventanas propias:
  last5: n=4, hits=2, rate=0.500, mean=1.75, median=1.5, min=0, max=4
  last10: n=4, hits=2, rate=0.500, mean=1.75, median=1.5, min=0, max=4
  all: n=4, hits=2, rate=0.500, mean=1.75, median=1.5, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 36. foul_involvements @ 1.5 — L. Munteanu
- basis: player_own_stats
- FM reportado: hits 3/5; mejor ventana: 5 (3/5)
- Ventanas propias:
  last5: n=3, hits=2, rate=0.667, mean=3, median=2, min=1, max=6
  last10: n=3, hits=2, rate=0.667, mean=3, median=2, min=1, max=6
  all: n=3, hits=2, rate=0.667, mean=3, median=2, min=1, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

## Evidencia histórica (ventanas fijas)
- observaciones descriptivas: secuencia cruda (más antiguo -> más reciente) y hits de las ventanas fijas (last5/last10/all)

### Jugadores — evidencia con ventanas fijas
- E. Demirovic — shots @ 0.5: [1,4,6,0,0,1,0,1,2,1] | last5 4/5 | last10 7/10 | all 18/23
- N. Stanciu — shots @ 0.5: [2,4,0,1,2,2,1,0,3,1] | last5 4/5 | last10 8/10 | all 17/23
- V. Mihăilă — shots @ 0.5: [1,2,1,0,2,2,0,0,0,1] | last5 2/5 | last10 6/10 | all 12/22
- E. Bajraktarevic — shots @ 0.5: [2,1,3,0,0,0,0,2,1,2] | last5 3/5 | last10 6/10 | all 10/21
- A. Memić — shots @ 0.5: [2,0,2,0,0,1,1,0,1,0] | last5 3/5 | last10 5/10 | all 7/19
- K. Alajbegovic — shots @ 0.5: [1,3,3,0,0,0,0,2,2,2] | last5 3/5 | last10 6/10 | all 9/15
- S. Radeljic — shots @ 0.5: [1,0,0,0,0,0,0,1,0,0] | last5 1/5 | last10 2/10 | all 4/15
- K. Alajbegovic — shots @ 1.5: [1,3,3,0,0,0,0,2,2,2] | last5 3/5 | last10 5/10 | all 6/15
- N. Katic — fouls_committed @ 0.5: [2,4,3,0,0,2,1,0,1,0] | last5 3/5 | last10 6/10 | all 7/12
- N. Katic — tackles @ 1.5: [1,2,4,0,0,5,3,0,2,2] | last5 4/5 | last10 6/10 | all 7/12
- E. Demirovic — foul_involvements @ 1.5: [6,4,4,1,0,5,0,5,4,2] | last5 4/5 | last10 7/10 | all 7/10
- E. Džeko — foul_involvements @ 1.5: [1,2,4,5,3,0,4,3,0,0] | last5 2/5 | last10 6/10 | all 6/10
- I. Šunjić — foul_involvements @ 1.5: [6,6,0,2,2,2,2,1,2,2] | last5 4/5 | last10 8/10 | all 8/10
- E. Demirovic — foul_involvements @ 2.5: [6,4,4,1,0,5,0,5,4,2] | last5 3/5 | last10 6/10 | all 6/10
- E. Džeko — foul_involvements @ 2.5: [1,2,4,5,3,0,4,3,0,0] | last5 2/5 | last10 5/10 | all 5/10
- E. Demirovic — foul_involvements @ 3.5: [6,4,4,1,0,5,0,5,4,2] | last5 3/5 | last10 6/10 | all 6/10
- E. Demirovic — fouls_committed @ 1.5: [3,1,2,1,0,4,0,4,3,1] | last5 3/5 | last10 5/10 | all 5/10
- I. Šunjić — fouls_committed @ 1.5: [3,6,0,2,1,2,2,1,2,1] | last5 3/5 | last10 6/10 | all 6/10
- E. Demirovic — fouls_committed @ 2.5: [3,1,2,1,0,4,0,4,3,1] | last5 3/5 | last10 4/10 | all 4/10
- E. Džeko — fouls_drawn @ 1.5: [1,2,3,3,2,0,2,2,0,0] | last5 2/5 | last10 6/10 | all 6/10
- A. Memić — fouls_committed @ 0.5: [1,0,0,1,0,1,0,1,2] | last5 3/5 | last10 5/9 | all 5/9
- S. Kolasinac — foul_involvements @ 1.5: [3,1,0,5,3,1,1,4] | last5 3/5 | last10 4/8 | all 4/8
- S. Kolasinac — foul_involvements @ 2.5: [3,1,0,5,3,1,1,4] | last5 3/5 | last10 4/8 | all 4/8
- K. Alajbegovic — fouls_committed @ 0.5: [1,4,0,0,1,2,0,2] | last5 3/5 | last10 5/8 | all 5/8
- S. Kolasinac — fouls_committed @ 0.5: [2,1,0,3,2,0,1,2] | last5 4/5 | last10 6/8 | all 6/8
- S. Kolasinac — fouls_committed @ 1.5: [2,1,0,3,2,0,1,2] | last5 3/5 | last10 4/8 | all 4/8
- S. Kolasinac — tackles @ 1.5: [2,1,0,2,3,1,2,2] | last5 4/5 | last10 5/8 | all 5/8
- N. Stanciu — foul_involvements @ 1.5: [6,3,2,2,2,0,3] | last5 4/5 | last10 6/7 | all 6/7
- V. Dragomir — foul_involvements @ 1.5: [7,4,2,2,3,0,2] | last5 4/5 | last10 6/7 | all 6/7
- N. Stanciu — fouls_committed @ 0.5: [1,0,1,1,1,0,3] | last5 4/5 | last10 5/7 | all 5/7

### Equipos — evidencia con ventanas fijas
- Bosnia and Herzegovina — away_cards @ 3.5: [2,4,3,0,0,3,3,1,1,5] | last5 4/5 | last10 8/10 | all 23/30
  - Rival (Romania): [2,0,2,3,0,1,0,4,1,5] | last5 3/5 | last10 8/10 | all 26/30
- Bosnia and Herzegovina — away_corners @ 4.5: [2,3,10,2,10,4,3,5,3,3] | last5 4/5 | last10 7/10 | all 21/30
  - Rival (Romania): [1,7,2,1,6,3,6,6,10,0] | last5 2/5 | last10 5/10 | all 14/30
- Bosnia and Herzegovina — away_corners @ 5.5: [2,3,10,2,10,4,3,5,3,3] | last5 5/5 | last10 8/10 | all 25/30
  - Rival (Romania): [1,7,2,1,6,3,6,6,10,0] | last5 2/5 | last10 5/10 | all 18/30
- Romania — away_goals @ 0.5: [2,2,1,1,7,0,0,1,2,1] | last5 3/5 | last10 8/10 | all 22/30
  - Rival (Bosnia and Herzegovina): [1,1,1,0,1,1,1,3,0,0] | last5 3/5 | last10 7/10 | all 22/30
- Bosnia and Herzegovina — away_goals @ 1.5: [1,1,1,0,1,1,1,3,0,0] | last5 4/5 | last10 9/10 | all 22/30
  - Rival (Romania): [2,2,1,1,7,0,0,1,2,1] | last5 4/5 | last10 6/10 | all 16/30
- Bosnia and Herzegovina — away_goals @ 2.5: [1,1,1,0,1,1,1,3,0,0] | last5 4/5 | last10 9/10 | all 26/30
  - Rival (Romania): [2,2,1,1,7,0,0,1,2,1] | last5 5/5 | last10 9/10 | all 23/30
- Romania — away_goals @ 2.5: [2,2,1,1,7,0,0,1,2,1] | last5 5/5 | last10 9/10 | all 23/30
  - Rival (Bosnia and Herzegovina): [1,1,1,0,1,1,1,3,0,0] | last5 4/5 | last10 9/10 | all 26/30
- Bosnia and Herzegovina — away_shots @ 10.5: [8,14,30,13,1,8,5,14,10,9] | last5 4/5 | last10 6/10 | all 17/30
  - Rival (Romania): [8,12,10,9,25,6,9,5,22,10] | last5 4/5 | last10 7/10 | all 13/30
- Bosnia and Herzegovina — away_shots @ 12.5: [8,14,30,13,1,8,5,14,10,9] | last5 4/5 | last10 6/10 | all 19/30
  - Rival (Romania): [8,12,10,9,25,6,9,5,22,10] | last5 4/5 | last10 8/10 | all 17/30
- Bosnia and Herzegovina — away_shots_on_target @ 3.5: [1,5,11,3,1,3,3,5,3,1] | last5 4/5 | last10 7/10 | all 20/30
  - Rival (Romania): [5,7,2,3,13,0,4,1,8,1] | last5 3/5 | last10 5/10 | all 11/30
- Bosnia and Herzegovina — away_shots_on_target @ 4.5: [1,5,11,3,1,3,3,5,3,1] | last5 4/5 | last10 7/10 | all 22/30
  - Rival (Romania): [5,7,2,3,13,0,4,1,8,1] | last5 4/5 | last10 6/10 | all 14/30
- Bosnia and Herzegovina — home_goals @ 0.5: [1,1,1,0,1,1,1,3,0,0] | last5 3/5 | last10 7/10 | all 22/30
  - Rival (Romania): [2,2,1,1,7,0,0,1,2,1] | last5 3/5 | last10 8/10 | all 22/30
- Romania — home_goals @ 0.5: [2,2,1,1,7,0,0,1,2,1] | last5 3/5 | last10 8/10 | all 22/30
  - Rival (Bosnia and Herzegovina): [1,1,1,0,1,1,1,3,0,0] | last5 3/5 | last10 7/10 | all 22/30
- Bosnia and Herzegovina — home_goals @ 2.5: [1,1,1,0,1,1,1,3,0,0] | last5 4/5 | last10 9/10 | all 26/30
  - Rival (Romania): [2,2,1,1,7,0,0,1,2,1] | last5 5/5 | last10 9/10 | all 23/30
- Romania — home_goals @ 2.5: [2,2,1,1,7,0,0,1,2,1] | last5 5/5 | last10 9/10 | all 23/30
  - Rival (Bosnia and Herzegovina): [1,1,1,0,1,1,1,3,0,0] | last5 4/5 | last10 9/10 | all 26/30
- Romania — home_shots @ 13.5: [8,12,10,9,25,6,9,5,22,10] | last5 4/5 | last10 8/10 | all 18/30
  - Rival (Bosnia and Herzegovina): [8,14,30,13,1,8,5,14,10,9] | last5 4/5 | last10 7/10 | all 20/30
- Romania — home_shots @ 15.5: [8,12,10,9,25,6,9,5,22,10] | last5 4/5 | last10 8/10 | all 22/30
  - Rival (Bosnia and Herzegovina): [8,14,30,13,1,8,5,14,10,9] | last5 5/5 | last10 9/10 | all 25/30
- Romania vs Bosnia and Herzegovina — total_goals @ 1.5: [4,3,1,4,8,1,2,2,3,3] | last5 4/5 | last10 8/10 | all 24/30
- Romania vs Bosnia and Herzegovina — total_goals @ 3.5: [4,3,1,4,8,1,2,2,3,3] | last5 5/5 | last10 7/10 | all 23/30

## Confluencia descriptiva (equipo + rival + jugadores)
- familias de mercado con 2 o más series observadas en la DB; cada serie conserva su línea y sus ventanas fijas (last5/last10/all); sin score ni orden por fuerza

### cards
- Bosnia and Herzegovina (away) @ 3.5: last5 4/5 | last10 8/10 | all 23/30
- Romania (home) @ 3.5: last5 3/5 | last10 8/10 | all 26/30

### corners
- Bosnia and Herzegovina (away) @ 4.5: last5 4/5 | last10 7/10 | all 21/30
- Romania (home) @ 4.5: last5 2/5 | last10 5/10 | all 14/30
- Bosnia and Herzegovina (away) @ 5.5: last5 5/5 | last10 8/10 | all 25/30
- Romania (home) @ 5.5: last5 2/5 | last10 5/10 | all 18/30

### foul_involvements
- jugador E. Demirovic — foul_involvements @ 1.5: last5 4/5 | last10 7/10 | all 7/10
- jugador E. Džeko — foul_involvements @ 1.5: last5 2/5 | last10 6/10 | all 6/10
- jugador I. Šunjić — foul_involvements @ 1.5: last5 4/5 | last10 8/10 | all 8/10
- jugador E. Demirovic — foul_involvements @ 2.5: last5 3/5 | last10 6/10 | all 6/10
- jugador E. Džeko — foul_involvements @ 2.5: last5 2/5 | last10 5/10 | all 5/10
- jugador E. Demirovic — foul_involvements @ 3.5: last5 3/5 | last10 6/10 | all 6/10
- jugador S. Kolasinac — foul_involvements @ 1.5: last5 3/5 | last10 4/8 | all 4/8
- jugador S. Kolasinac — foul_involvements @ 2.5: last5 3/5 | last10 4/8 | all 4/8
- jugador N. Stanciu — foul_involvements @ 1.5: last5 4/5 | last10 6/7 | all 6/7
- jugador V. Dragomir — foul_involvements @ 1.5: last5 4/5 | last10 6/7 | all 6/7

### fouls_committed
- jugador N. Katic — fouls_committed @ 0.5: last5 3/5 | last10 6/10 | all 7/12
- jugador E. Demirovic — fouls_committed @ 1.5: last5 3/5 | last10 5/10 | all 5/10
- jugador I. Šunjić — fouls_committed @ 1.5: last5 3/5 | last10 6/10 | all 6/10
- jugador E. Demirovic — fouls_committed @ 2.5: last5 3/5 | last10 4/10 | all 4/10
- jugador A. Memić — fouls_committed @ 0.5: last5 3/5 | last10 5/9 | all 5/9
- jugador K. Alajbegovic — fouls_committed @ 0.5: last5 3/5 | last10 5/8 | all 5/8
- jugador S. Kolasinac — fouls_committed @ 0.5: last5 4/5 | last10 6/8 | all 6/8
- jugador S. Kolasinac — fouls_committed @ 1.5: last5 3/5 | last10 4/8 | all 4/8
- jugador N. Stanciu — fouls_committed @ 0.5: last5 4/5 | last10 5/7 | all 5/7

### goals
- Romania (home) @ 0.5: last5 3/5 | last10 8/10 | all 22/30
- Bosnia and Herzegovina (away) @ 0.5: last5 3/5 | last10 7/10 | all 22/30
- Bosnia and Herzegovina (away) @ 1.5: last5 4/5 | last10 9/10 | all 22/30
- Romania (home) @ 1.5: last5 4/5 | last10 6/10 | all 16/30
- Bosnia and Herzegovina (away) @ 2.5: last5 4/5 | last10 9/10 | all 26/30
- Romania (home) @ 2.5: last5 5/5 | last10 9/10 | all 23/30

### shots
- Bosnia and Herzegovina (away) @ 10.5: last5 4/5 | last10 6/10 | all 17/30
- Romania (home) @ 10.5: last5 4/5 | last10 7/10 | all 13/30
- Bosnia and Herzegovina (away) @ 12.5: last5 4/5 | last10 6/10 | all 19/30
- Romania (home) @ 12.5: last5 4/5 | last10 8/10 | all 17/30
- Romania (home) @ 13.5: last5 4/5 | last10 8/10 | all 18/30
- Bosnia and Herzegovina (away) @ 13.5: last5 4/5 | last10 7/10 | all 20/30
- Romania (home) @ 15.5: last5 4/5 | last10 8/10 | all 22/30
- Bosnia and Herzegovina (away) @ 15.5: last5 5/5 | last10 9/10 | all 25/30
- jugador E. Demirovic — shots @ 0.5: last5 4/5 | last10 7/10 | all 18/23
- jugador N. Stanciu — shots @ 0.5: last5 4/5 | last10 8/10 | all 17/23
- jugador V. Mihăilă — shots @ 0.5: last5 2/5 | last10 6/10 | all 12/22
- jugador E. Bajraktarevic — shots @ 0.5: last5 3/5 | last10 6/10 | all 10/21
- jugador A. Memić — shots @ 0.5: last5 3/5 | last10 5/10 | all 7/19
- jugador K. Alajbegovic — shots @ 0.5: last5 3/5 | last10 6/10 | all 9/15
- jugador S. Radeljic — shots @ 0.5: last5 1/5 | last10 2/10 | all 4/15
- jugador K. Alajbegovic — shots @ 1.5: last5 3/5 | last10 5/10 | all 6/15

### shots_on_target
- Bosnia and Herzegovina (away) @ 3.5: last5 4/5 | last10 7/10 | all 20/30
- Romania (home) @ 3.5: last5 3/5 | last10 5/10 | all 11/30
- Bosnia and Herzegovina (away) @ 4.5: last5 4/5 | last10 7/10 | all 22/30
- Romania (home) @ 4.5: last5 4/5 | last10 6/10 | all 14/30

### tackles
- jugador N. Katic — tackles @ 1.5: last5 4/5 | last10 6/10 | all 7/12
- jugador S. Kolasinac — tackles @ 1.5: last5 4/5 | last10 5/8 | all 5/8

## Cuotas

| mercado | línea | casa | lado | cuota | prob. implícita | capturada |
|---|---|---|---|---|---|---|
| away_cards | 3.5 | Ladbrokes | over | 2.8 | 0.357 | 2026-09-28T21:37:41.073Z |
| away_cards | 3.5 | Ladbrokes | under | 1.36 | 0.735 | 2026-09-28T21:37:41.073Z |
| away_corners | 4.5 | Kambi | over | 2.15 | 0.465 | 2026-09-28T21:37:41.073Z |
| away_corners | 4.5 | Kambi | under | 1.58 | 0.633 | 2026-09-28T21:37:41.073Z |
| away_corners | 5.5 | Kambi | over | 3.05 | 0.328 | 2026-09-28T21:37:41.073Z |
| away_corners | 5.5 | Kambi | under | 1.29 | 0.775 | 2026-09-28T21:37:41.073Z |
| away_goals | 0.5 | Kambi | over | 1.34 | 0.746 | 2026-09-28T21:37:41.073Z |
| away_goals | 0.5 | Kambi | under | 2.95 | 0.339 | 2026-09-28T21:37:41.073Z |
| away_goals | 0.5 | Ladbrokes | over | 1.33 | 0.752 | 2026-09-28T21:37:41.073Z |
| away_goals | 0.5 | Ladbrokes | under | 3.1 | 0.323 | 2026-09-28T21:37:41.073Z |
| away_goals | 1.5 | Bet365 | over | 2.5 | 0.400 | 2026-09-28T21:37:41.073Z |
| away_goals | 1.5 | Bet365 | under | 1.5 | 0.667 | 2026-09-28T21:37:41.073Z |
| away_goals | 1.5 | Kambi | over | 2.7 | 0.370 | 2026-09-28T21:37:41.073Z |
| away_goals | 1.5 | Kambi | under | 1.4 | 0.714 | 2026-09-28T21:37:41.073Z |
| away_goals | 1.5 | Ladbrokes | over | 2.6 | 0.385 | 2026-09-28T21:37:41.073Z |
| away_goals | 1.5 | Ladbrokes | under | 1.44 | 0.694 | 2026-09-28T21:37:41.073Z |
| away_goals | 2.5 | Bet365 | over | 6 | 0.167 | 2026-09-28T21:37:41.073Z |
| away_goals | 2.5 | Bet365 | under | 1.13 | 0.885 | 2026-09-28T21:37:41.073Z |
| away_goals | 2.5 | Kambi | over | 6.4 | 0.156 | 2026-09-28T21:37:41.073Z |
| away_goals | 2.5 | Kambi | under | 1.06 | 0.943 | 2026-09-28T21:37:41.073Z |
| away_goals | 2.5 | Ladbrokes | over | 6.5 | 0.154 | 2026-09-28T21:37:41.073Z |
| away_goals | 2.5 | Ladbrokes | under | 1.09 | 0.917 | 2026-09-28T21:37:41.073Z |
| away_goals | 2.5 | Bet365 | over | 6 | 0.167 | 2026-09-28T21:37:41.073Z |
| away_goals | 2.5 | Bet365 | under | 1.13 | 0.885 | 2026-09-28T21:37:41.073Z |
| away_goals | 2.5 | Kambi | over | 6.4 | 0.156 | 2026-09-28T21:37:41.073Z |
| away_goals | 2.5 | Kambi | under | 1.06 | 0.943 | 2026-09-28T21:37:41.073Z |
| away_goals | 2.5 | Ladbrokes | over | 6.5 | 0.154 | 2026-09-28T21:37:41.073Z |
| away_goals | 2.5 | Ladbrokes | under | 1.09 | 0.917 | 2026-09-28T21:37:41.073Z |
| away_shots_on_target | 3.5 | Kambi | over | 1.67 | 0.599 | 2026-09-28T21:37:41.073Z |
| away_shots_on_target | 3.5 | Kambi | under | 1.86 | 0.538 | 2026-09-28T21:37:41.073Z |
| away_shots_on_target | 4.5 | Kambi | over | 2.48 | 0.403 | 2026-09-28T21:37:41.073Z |
| away_shots_on_target | 4.5 | Kambi | under | 1.37 | 0.730 | 2026-09-28T21:37:41.073Z |
| home_goals | 0.5 | Kambi | over | 1.29 | 0.775 | 2026-09-28T21:37:41.073Z |
| home_goals | 0.5 | Kambi | under | 3.3 | 0.303 | 2026-09-28T21:37:41.073Z |
| home_goals | 0.5 | Ladbrokes | over | 1.28 | 0.781 | 2026-09-28T21:37:41.073Z |
| home_goals | 0.5 | Ladbrokes | under | 3.3 | 0.303 | 2026-09-28T21:37:41.073Z |
| home_goals | 0.5 | Kambi | over | 1.29 | 0.775 | 2026-09-28T21:37:41.073Z |
| home_goals | 0.5 | Kambi | under | 3.3 | 0.303 | 2026-09-28T21:37:41.073Z |
| home_goals | 0.5 | Ladbrokes | over | 1.28 | 0.781 | 2026-09-28T21:37:41.073Z |
| home_goals | 0.5 | Ladbrokes | under | 3.3 | 0.303 | 2026-09-28T21:37:41.073Z |
| home_goals | 2.5 | Bet365 | over | 6 | 0.167 | 2026-09-28T21:37:41.073Z |
| home_goals | 2.5 | Bet365 | under | 1.13 | 0.885 | 2026-09-28T21:37:41.073Z |
| home_goals | 2.5 | Kambi | over | 5.5 | 0.182 | 2026-09-28T21:37:41.073Z |
| home_goals | 2.5 | Kambi | under | 1.11 | 0.901 | 2026-09-28T21:37:41.073Z |
| home_goals | 2.5 | Ladbrokes | over | 5.5 | 0.182 | 2026-09-28T21:37:41.073Z |
| home_goals | 2.5 | Ladbrokes | under | 1.12 | 0.893 | 2026-09-28T21:37:41.073Z |
| home_goals | 2.5 | Bet365 | over | 6 | 0.167 | 2026-09-28T21:37:41.073Z |
| home_goals | 2.5 | Bet365 | under | 1.13 | 0.885 | 2026-09-28T21:37:41.073Z |
| home_goals | 2.5 | Kambi | over | 5.5 | 0.182 | 2026-09-28T21:37:41.073Z |
| home_goals | 2.5 | Kambi | under | 1.11 | 0.901 | 2026-09-28T21:37:41.073Z |
| home_goals | 2.5 | Ladbrokes | over | 5.5 | 0.182 | 2026-09-28T21:37:41.073Z |
| home_goals | 2.5 | Ladbrokes | under | 1.12 | 0.893 | 2026-09-28T21:37:41.073Z |
| total_goals | 1.5 | Bet365 | over | 1.29 | 0.775 | 2026-09-28T21:37:41.073Z |
| total_goals | 1.5 | Bet365 | under | 3.5 | 0.286 | 2026-09-28T21:37:41.073Z |
| total_goals | 1.5 | Kambi | over | 1.34 | 0.746 | 2026-09-28T21:37:41.073Z |
| total_goals | 1.5 | Kambi | under | 3.15 | 0.317 | 2026-09-28T21:37:41.073Z |
| total_goals | 1.5 | Ladbrokes | over | 1.3 | 0.769 | 2026-09-28T21:37:41.073Z |
| total_goals | 1.5 | Ladbrokes | under | 3.2 | 0.312 | 2026-09-28T21:37:41.073Z |
| total_goals | 1.5 | Paddy Power | over | 1.29 | 0.775 | 2026-09-28T21:37:41.073Z |
| total_goals | 1.5 | Paddy Power | under | 3.3 | 0.303 | 2026-09-28T21:37:41.073Z |
| total_goals | 3.5 | Bet365 | over | 3.4 | 0.294 | 2026-09-28T21:37:41.073Z |
| total_goals | 3.5 | Bet365 | under | 1.3 | 0.769 | 2026-09-28T21:37:41.073Z |
| total_goals | 3.5 | Kambi | over | 3.7 | 0.270 | 2026-09-28T21:37:41.073Z |
| total_goals | 3.5 | Kambi | under | 1.26 | 0.794 | 2026-09-28T21:37:41.073Z |
| total_goals | 3.5 | Ladbrokes | over | 3.5 | 0.286 | 2026-09-28T21:37:41.073Z |
| total_goals | 3.5 | Ladbrokes | under | 1.25 | 0.800 | 2026-09-28T21:37:41.073Z |
| total_goals | 3.5 | Paddy Power | over | 3.3 | 0.303 | 2026-09-28T21:37:41.073Z |
| total_goals | 3.5 | Paddy Power | under | 1.29 | 0.775 | 2026-09-28T21:37:41.073Z |
| total_cards | 5.5 | Kambi | over | 2.05 | 0.488 | 2026-09-28T21:37:41.073Z |
| total_cards | 5.5 | Kambi | under | 1.6 | 0.625 | 2026-09-28T21:37:41.073Z |
| total_cards | 5.5 | Ladbrokes | over | 2.1 | 0.476 | 2026-09-28T21:37:41.073Z |
| total_cards | 5.5 | Ladbrokes | under | 1.67 | 0.599 | 2026-09-28T21:37:41.073Z |
| total_cards | 6.5 | Ladbrokes | over | 3.1 | 0.323 | 2026-09-28T21:37:41.073Z |
| total_cards | 6.5 | Ladbrokes | under | 1.33 | 0.752 | 2026-09-28T21:37:41.073Z |
| total_corners | 5.5 | Kambi | over | 1.11 | 0.901 | 2026-09-28T21:37:41.073Z |
| total_corners | 5.5 | Kambi | under | 5.2 | 0.192 | 2026-09-28T21:37:41.073Z |
| total_corners | 6.5 | Kambi | over | 1.24 | 0.806 | 2026-09-28T21:37:41.073Z |
| total_corners | 6.5 | Kambi | under | 3.5 | 0.286 | 2026-09-28T21:37:41.073Z |
| total_corners | 6.5 | Ladbrokes | over | 1.2 | 0.833 | 2026-09-28T21:37:41.073Z |
| total_corners | 6.5 | Ladbrokes | under | 4 | 0.250 | 2026-09-28T21:37:41.073Z |
| total_corners | 10.5 | Kambi | over | 2.85 | 0.351 | 2026-09-28T21:37:41.073Z |
| total_corners | 10.5 | Kambi | under | 1.35 | 0.741 | 2026-09-28T21:37:41.073Z |
| total_corners | 10.5 | Ladbrokes | over | 3.1 | 0.323 | 2026-09-28T21:37:41.073Z |
| total_corners | 10.5 | Ladbrokes | under | 1.33 | 0.752 | 2026-09-28T21:37:41.073Z |
| total_corners | 10.5 | Paddy Power | over | 3.1 | 0.323 | 2026-09-28T21:37:41.073Z |
| total_corners | 10.5 | Paddy Power | under | 1.3 | 0.769 | 2026-09-28T21:37:41.073Z |
| total_corners | 11.5 | Kambi | over | 3.85 | 0.260 | 2026-09-28T21:37:41.073Z |
| total_corners | 11.5 | Kambi | under | 1.2 | 0.833 | 2026-09-28T21:37:41.073Z |
| total_corners | 11.5 | Paddy Power | over | 4.33 | 0.231 | 2026-09-28T21:37:41.073Z |
| total_corners | 11.5 | Paddy Power | under | 1.17 | 0.855 | 2026-09-28T21:37:41.073Z |
| total_corners | 12.5 | Kambi | over | 5.1 | 0.196 | 2026-09-28T21:37:41.073Z |
| total_corners | 12.5 | Kambi | under | 1.11 | 0.901 | 2026-09-28T21:37:41.073Z |
| total_corners | 12.5 | Paddy Power | over | 6 | 0.167 | 2026-09-28T21:37:41.073Z |
| total_corners | 12.5 | Paddy Power | under | 1.08 | 0.926 | 2026-09-28T21:37:41.073Z |
| total_shots_on_target | 8.5 | Kambi | over | 2.1 | 0.476 | 2026-09-28T21:37:41.073Z |
| total_shots_on_target | 8.5 | Kambi | under | 1.58 | 0.633 | 2026-09-28T21:37:41.073Z |
| total_shots_on_target | 9.5 | Kambi | over | 2.8 | 0.357 | 2026-09-28T21:37:41.073Z |
| total_shots_on_target | 9.5 | Kambi | under | 1.32 | 0.758 | 2026-09-28T21:37:41.073Z |
- Cuota manual Betano (away_cards @ 3.5): no cargada
- Cuota manual Betano (away_corners @ 4.5): no cargada
- Cuota manual Betano (away_corners @ 5.5): no cargada
- Cuota manual Betano (away_goals @ 0.5): no cargada
- Cuota manual Betano (away_goals @ 1.5): no cargada
- Cuota manual Betano (away_goals @ 2.5): no cargada
- Cuota manual Betano (away_goals @ 2.5): no cargada
- Cuota manual Betano (away_shots @ 10.5): no cargada
- Cuota manual Betano (away_shots @ 12.5): no cargada
- Cuota manual Betano (away_shots_on_target @ 3.5): no cargada
- Cuota manual Betano (away_shots_on_target @ 4.5): no cargada
- Cuota manual Betano (home_goals @ 0.5): no cargada
- Cuota manual Betano (home_goals @ 0.5): no cargada
- Cuota manual Betano (home_goals @ 2.5): no cargada
- Cuota manual Betano (home_goals @ 2.5): no cargada
- Cuota manual Betano (home_shots @ 13.5): no cargada
- Cuota manual Betano (home_shots @ 15.5): no cargada
- Cuota manual Betano (total_goals @ 1.5): no cargada
- Cuota manual Betano (total_goals @ 3.5): no cargada
- Cuota manual Betano (total_cards @ 5.5): no cargada
- Cuota manual Betano (total_cards @ 6.5): no cargada
- Cuota manual Betano (total_corners @ 5.5): no cargada
- Cuota manual Betano (total_corners @ 6.5): no cargada
- Cuota manual Betano (total_corners @ 10.5): no cargada
- Cuota manual Betano (total_corners @ 11.5): no cargada
- Cuota manual Betano (total_corners @ 12.5): no cargada
- Cuota manual Betano (total_shots @ 24.5): no cargada
- Cuota manual Betano (total_shots @ 26.5): no cargada
- Cuota manual Betano (total_shots_on_target @ 8.5): no cargada
- Cuota manual Betano (total_shots_on_target @ 9.5): no cargada

## Nota de cierre
Este reporte es descriptivo. No constituye una recomendación de apuesta ni una probabilidad validada de resultado futuro.

---

## Northern Ireland vs Hungary (33662315) - kickoff 2026-09-28T18:45:00.000Z

POST-PARTIDO - SOLO REGISTRO HISTORICO, NO ACCIONABLE

- Motivo de la marca: partido ya jugado (kickoff 2026-09-28T18:45:00.000Z); las cuotas en fm_market_odds son de cierre y las senales pueden reflejar el resultado ya conocido. Uso unico: dataset historico para calibracion/backtest.
- Snapshot capturado post-partido: 21:37:54Z-21:38:17Z (tabs overview/player-trends/team-trends, 3 navs; +2 fetch de scope partidos sin nav).
- No intentado - pausa del operador por lectura de un ledger manual desactualizado (docs/STATE.md); limite real de la API: 40 navs/corrida, 24 necesarias para los 8 fixtures. Corregido el 2026-09-28.
- Senales OK/SUSPECT persistidas: 72. El informe P11 nativo (descriptivo, sin Delta% ni candidatos) se lista debajo, tal como lo genera GET /api/fm/fixtures/{id}/report.md.

# Northern Ireland vs Hungary
- Competición: UEFA Nations League
- Kickoff (UTC): desconocido
- Flags de calidad: leakage=false; suspect=true
- sort_criteria: sample_size_desc

## Mercados (orden: mayor muestra disponible, no fuerza de señal)

### 1. away_corners @ 4.5 — Hungary (away)
- basis: team_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=4.4, median=3, min=3, max=9
-   last10: n=10, hits=6, rate=0.600, mean=5.2, median=4, min=1, max=10
-   all: n=30, hits=17, rate=0.567, mean=4.7667, median=4, min=0, max=13
- Split home/away:
-   home: n=15, hits=9, rate=0.600, mean=4.8, median=3
-   away: n=15, hits=8, rate=0.533, mean=4.7333, median=4
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=1.6, median=1, min=1, max=3
    last10: n=10, hits=7, rate=0.700, mean=3, median=2.5, min=1, max=6
    all: n=30, hits=16, rate=0.533, mean=4.0333, median=4, min=0, max=10
  Split home/away:
    home: n=15, hits=5, rate=0.333, mean=5.4, median=5
    away: n=15, hits=11, rate=0.733, mean=2.6667, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 2. away_corners @ 5.5 — Hungary (away)
- basis: team_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=4.4, median=3, min=3, max=9
-   last10: n=10, hits=6, rate=0.600, mean=5.2, median=4, min=1, max=10
-   all: n=30, hits=18, rate=0.600, mean=4.7667, median=4, min=0, max=13
- Split home/away:
-   home: n=15, hits=10, rate=0.667, mean=4.8, median=3
-   away: n=15, hits=8, rate=0.533, mean=4.7333, median=4
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=1.6, median=1, min=1, max=3
    last10: n=10, hits=9, rate=0.900, mean=3, median=2.5, min=1, max=6
    all: n=30, hits=22, rate=0.733, mean=4.0333, median=4, min=0, max=10
  Split home/away:
    home: n=15, hits=8, rate=0.533, mean=5.4, median=5
    away: n=15, hits=14, rate=0.933, mean=2.6667, median=2
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 3. away_shots @ 10.5 — Hungary (away)
- basis: team_own_stats
- FM reportado: hits 9/9; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=16.6, median=17, min=12, max=20
-   last10: n=10, hits=9, rate=0.900, mean=15, median=16.5, min=5, max=20
-   all: n=30, hits=22, rate=0.733, mean=13.6333, median=13.5, min=5, max=26
- Split home/away:
-   home: n=15, hits=11, rate=0.733, mean=13.6667, median=15
-   away: n=15, hits=11, rate=0.733, mean=13.6, median=12
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=1, rate=0.200, mean=8.6, median=9, min=4, max=12
    last10: n=10, hits=3, rate=0.300, mean=8.7, median=9, min=3, max=12
    all: n=30, hits=12, rate=0.400, mean=9.6, median=9, min=2, max=18
  Split home/away:
    home: n=15, hits=8, rate=0.533, mean=11.1333, median=11
    away: n=15, hits=4, rate=0.267, mean=8.0667, median=8
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 4. away_shots @ 12.5 — Hungary (away)
- basis: team_own_stats
- FM reportado: hits 8/9; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=16.6, median=17, min=12, max=20
-   last10: n=10, hits=8, rate=0.800, mean=15, median=16.5, min=5, max=20
-   all: n=30, hits=17, rate=0.567, mean=13.6333, median=13.5, min=5, max=26
- Split home/away:
-   home: n=15, hits=10, rate=0.667, mean=13.6667, median=15
-   away: n=15, hits=7, rate=0.467, mean=13.6, median=12
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=0, rate=0.000, mean=8.6, median=9, min=4, max=12
    last10: n=10, hits=0, rate=0.000, mean=8.7, median=9, min=3, max=12
    all: n=30, hits=6, rate=0.200, mean=9.6, median=9, min=2, max=18
  Split home/away:
    home: n=15, hits=4, rate=0.267, mean=11.1333, median=11
    away: n=15, hits=2, rate=0.133, mean=8.0667, median=8
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 5. away_shots_on_target @ 2.5 — Hungary (away)
- basis: team_own_stats
- FM reportado: hits 8/9; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=4.2, median=4, min=2, max=6
-   last10: n=10, hits=8, rate=0.800, mean=4.4, median=4, min=2, max=7
-   all: n=30, hits=22, rate=0.733, mean=4.4, median=5, min=1, max=8
- Split home/away:
-   home: n=15, hits=10, rate=0.667, mean=4.1333, median=4
-   away: n=15, hits=12, rate=0.800, mean=4.6667, median=5
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=3.2, median=4, min=1, max=5
    last10: n=10, hits=6, rate=0.600, mean=2.9, median=3, min=1, max=5
    all: n=30, hits=18, rate=0.600, mean=3.2333, median=3, min=0, max=7
  Split home/away:
    home: n=15, hits=10, rate=0.667, mean=3.3333, median=3
    away: n=15, hits=8, rate=0.533, mean=3.1333, median=3
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 6. home_corners @ 3.5 — Northern Ireland (home)
- basis: team_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.6, median=1, min=1, max=3
-   last10: n=10, hits=6, rate=0.600, mean=3, median=2.5, min=1, max=6
-   all: n=30, hits=13, rate=0.433, mean=4.0333, median=4, min=0, max=10
- Split home/away:
-   home: n=15, hits=3, rate=0.200, mean=5.4, median=5
-   away: n=15, hits=10, rate=0.667, mean=2.6667, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=4.4, median=3, min=3, max=9
    last10: n=10, hits=4, rate=0.400, mean=5.2, median=4, min=1, max=10
    all: n=30, hits=14, rate=0.467, mean=4.7667, median=4, min=0, max=13
  Split home/away:
    home: n=15, hits=8, rate=0.533, mean=4.8, median=3
    away: n=15, hits=6, rate=0.400, mean=4.7333, median=4
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 7. home_corners @ 4.5 — Northern Ireland (home)
- basis: team_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.6, median=1, min=1, max=3
-   last10: n=10, hits=7, rate=0.700, mean=3, median=2.5, min=1, max=6
-   all: n=30, hits=16, rate=0.533, mean=4.0333, median=4, min=0, max=10
- Split home/away:
-   home: n=15, hits=5, rate=0.333, mean=5.4, median=5
-   away: n=15, hits=11, rate=0.733, mean=2.6667, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=4.4, median=3, min=3, max=9
    last10: n=10, hits=6, rate=0.600, mean=5.2, median=4, min=1, max=10
    all: n=30, hits=17, rate=0.567, mean=4.7667, median=4, min=0, max=13
  Split home/away:
    home: n=15, hits=9, rate=0.600, mean=4.8, median=3
    away: n=15, hits=8, rate=0.533, mean=4.7333, median=4
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 8. home_corners @ 5.5 — Northern Ireland (home)
- basis: team_own_stats
- FM reportado: hits 8/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=1.6, median=1, min=1, max=3
-   last10: n=10, hits=9, rate=0.900, mean=3, median=2.5, min=1, max=6
-   all: n=30, hits=22, rate=0.733, mean=4.0333, median=4, min=0, max=10
- Split home/away:
-   home: n=15, hits=8, rate=0.533, mean=5.4, median=5
-   away: n=15, hits=14, rate=0.933, mean=2.6667, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=4.4, median=3, min=3, max=9
    last10: n=10, hits=6, rate=0.600, mean=5.2, median=4, min=1, max=10
    all: n=30, hits=18, rate=0.600, mean=4.7667, median=4, min=0, max=13
  Split home/away:
    home: n=15, hits=10, rate=0.667, mean=4.8, median=3
    away: n=15, hits=8, rate=0.533, mean=4.7333, median=4
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 9. home_goals @ 0.5 — Northern Ireland (home)
- basis: team_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=0.8, median=1, min=0, max=1
-   last10: n=10, hits=7, rate=0.700, mean=0.8, median=1, min=0, max=2
-   all: n=30, hits=22, rate=0.733, mean=1.2, median=1, min=0, max=5
- Split home/away:
-   home: n=15, hits=11, rate=0.733, mean=1.4667, median=1
-   away: n=15, hits=11, rate=0.733, mean=0.9333, median=1
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=1.2, median=1, min=0, max=3
    last10: n=10, hits=8, rate=0.800, mean=1.5, median=2, min=0, max=3
    all: n=30, hits=21, rate=0.700, mean=1.2, median=1, min=0, max=3
  Split home/away:
    home: n=15, hits=10, rate=0.667, mean=1.2, median=1
    away: n=15, hits=11, rate=0.733, mean=1.2, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 10. home_goals @ 1.5 — Hungary (away)
- basis: team_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: n=5, hits=3, rate=0.600, mean=1.2, median=1, min=0, max=3
-   last10: n=10, hits=4, rate=0.400, mean=1.5, median=2, min=0, max=3
-   all: n=30, hits=17, rate=0.567, mean=1.2, median=1, min=0, max=3
- Split home/away:
-   home: n=15, hits=9, rate=0.600, mean=1.2, median=1
-   away: n=15, hits=8, rate=0.533, mean=1.2, median=1
- Rival (contexto equivalente):
  FM reportado: hits 8/8; mejor ventana: no determinable
  Ventanas propias:
    last5: n=5, hits=5, rate=1.000, mean=0.8, median=1, min=0, max=1
    last10: n=10, hits=9, rate=0.900, mean=0.8, median=1, min=0, max=2
    all: n=30, hits=21, rate=0.700, mean=1.2, median=1, min=0, max=5
  Split home/away:
    home: n=15, hits=8, rate=0.533, mean=1.4667, median=1
    away: n=15, hits=13, rate=0.867, mean=0.9333, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 11. home_goals @ 1.5 — Northern Ireland (home)
- basis: team_own_stats
- FM reportado: hits 8/8; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=0.8, median=1, min=0, max=1
-   last10: n=10, hits=9, rate=0.900, mean=0.8, median=1, min=0, max=2
-   all: n=30, hits=21, rate=0.700, mean=1.2, median=1, min=0, max=5
- Split home/away:
-   home: n=15, hits=8, rate=0.533, mean=1.4667, median=1
-   away: n=15, hits=13, rate=0.867, mean=0.9333, median=1
- Rival (contexto equivalente):
  FM reportado: hits 5/5; mejor ventana: 5 (5/5)
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=1.2, median=1, min=0, max=3
    last10: n=10, hits=4, rate=0.400, mean=1.5, median=2, min=0, max=3
    all: n=30, hits=17, rate=0.567, mean=1.2, median=1, min=0, max=3
  Split home/away:
    home: n=15, hits=9, rate=0.600, mean=1.2, median=1
    away: n=15, hits=8, rate=0.533, mean=1.2, median=1
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 12. home_shots @ 10.5 — Northern Ireland (home)
- basis: team_own_stats
- FM reportado: hits 6/7; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=8.6, median=9, min=4, max=12
-   last10: n=10, hits=7, rate=0.700, mean=8.7, median=9, min=3, max=12
-   all: n=30, hits=18, rate=0.600, mean=9.6, median=9, min=2, max=18
- Split home/away:
-   home: n=15, hits=7, rate=0.467, mean=11.1333, median=11
-   away: n=15, hits=11, rate=0.733, mean=8.0667, median=8
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=0, rate=0.000, mean=16.6, median=17, min=12, max=20
    last10: n=10, hits=1, rate=0.100, mean=15, median=16.5, min=5, max=20
    all: n=30, hits=8, rate=0.267, mean=13.6333, median=13.5, min=5, max=26
  Split home/away:
    home: n=15, hits=4, rate=0.267, mean=13.6667, median=15
    away: n=15, hits=4, rate=0.267, mean=13.6, median=12
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 13. home_shots @ 12.5 — Northern Ireland (home)
- basis: team_own_stats
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
-   last5: n=5, hits=5, rate=1.000, mean=8.6, median=9, min=4, max=12
-   last10: n=10, hits=10, rate=1.000, mean=8.7, median=9, min=3, max=12
-   all: n=30, hits=24, rate=0.800, mean=9.6, median=9, min=2, max=18
- Split home/away:
-   home: n=15, hits=11, rate=0.733, mean=11.1333, median=11
-   away: n=15, hits=13, rate=0.867, mean=8.0667, median=8
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=1, rate=0.200, mean=16.6, median=17, min=12, max=20
    last10: n=10, hits=2, rate=0.200, mean=15, median=16.5, min=5, max=20
    all: n=30, hits=13, rate=0.433, mean=13.6333, median=13.5, min=5, max=26
  Split home/away:
    home: n=15, hits=5, rate=0.333, mean=13.6667, median=15
    away: n=15, hits=8, rate=0.533, mean=13.6, median=12
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 14. home_shots_on_target @ 4.5 — Northern Ireland (home)
- basis: team_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=3.2, median=4, min=1, max=5
-   last10: n=10, hits=9, rate=0.900, mean=2.9, median=3, min=1, max=5
-   all: n=30, hits=22, rate=0.733, mean=3.2333, median=3, min=0, max=7
- Split home/away:
-   home: n=15, hits=12, rate=0.800, mean=3.3333, median=3
-   away: n=15, hits=10, rate=0.667, mean=3.1333, median=3
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=4.2, median=4, min=2, max=6
    last10: n=10, hits=6, rate=0.600, mean=4.4, median=4, min=2, max=7
    all: n=30, hits=14, rate=0.467, mean=4.4, median=5, min=1, max=8
  Split home/away:
    home: n=15, hits=8, rate=0.533, mean=4.1333, median=4
    away: n=15, hits=6, rate=0.400, mean=4.6667, median=5
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 15. total_goals @ 2.5 — Northern Ireland vs Hungary (sin rol)
- basis: match_total_goals
- FM reportado: hits 8/9; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=2, median=2, min=1, max=4
-   last10: n=10, hits=8, rate=0.800, mean=1.9, median=1.5, min=1, max=4
-   all: n=30, hits=21, rate=0.700, mean=2.2667, median=2, min=0, max=6
- Split home/away:
-   home: n=15, hits=13, rate=0.867, mean=1.8, median=2
-   away: n=15, hits=8, rate=0.533, mean=2.7333, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=3, rate=0.600, mean=1.8, median=1, min=0, max=4
    last10: n=10, hits=5, rate=0.500, mean=2.6, median=2.5, min=0, max=5
    all: n=30, hits=13, rate=0.433, mean=2.7, median=3, min=0, max=5
  Split home/away:
    home: n=15, hits=8, rate=0.533, mean=2.4667, median=2
    away: n=15, hits=5, rate=0.333, mean=2.9333, median=3
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 16. total_goals @ 3.5 — Northern Ireland vs Hungary (sin rol)
- basis: match_total_goals
- FM reportado: hits 8/9; mejor ventana: no determinable
- Ventanas propias:
-   last5: n=5, hits=4, rate=0.800, mean=2, median=2, min=1, max=4
-   last10: n=10, hits=8, rate=0.800, mean=1.9, median=1.5, min=1, max=4
-   all: n=30, hits=23, rate=0.767, mean=2.2667, median=2, min=0, max=6
- Split home/away:
-   home: n=15, hits=14, rate=0.933, mean=1.8, median=2
-   away: n=15, hits=9, rate=0.600, mean=2.7333, median=2
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: n=5, hits=4, rate=0.800, mean=1.8, median=1, min=0, max=4
    last10: n=10, hits=6, rate=0.600, mean=2.6, median=2.5, min=0, max=5
    all: n=30, hits=19, rate=0.633, mean=2.7, median=3, min=0, max=5
  Split home/away:
    home: n=15, hits=11, rate=0.733, mean=2.4667, median=2
    away: n=15, hits=8, rate=0.533, mean=2.9333, median=3
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo=""

### 17. 1x2_shots — Hungary (away)
- basis: unmapped
- FM reportado: hits 7/7; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=true, source_conflict=false, motivo="non-numeric history value 'vt'; market '1x2_shots' has no mapping to fm_team_matches.team_stats_json"

### 18. total_cards @ 2.5 — Northern Ireland vs Hungary (sin rol)
- basis: not_reproducible
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 19. total_cards @ 4.5 — Northern Ireland vs Hungary (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 20. total_cards @ 5.5 — Northern Ireland vs Hungary (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 21. total_corners @ 5.5 — Northern Ireland vs Hungary (sin rol)
- basis: not_reproducible
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 22. total_corners @ 6.5 — Northern Ireland vs Hungary (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 23. total_corners @ 7.5 — Northern Ireland vs Hungary (sin rol)
- basis: not_reproducible
- FM reportado: hits 8/9; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 24. total_corners @ 10.5 — Northern Ireland vs Hungary (sin rol)
- basis: not_reproducible
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 25. total_corners @ 11.5 — Northern Ireland vs Hungary (sin rol)
- basis: not_reproducible
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 26. total_corners @ 12.5 — Northern Ireland vs Hungary (sin rol)
- basis: not_reproducible
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 27. total_shots @ 21.5 — Northern Ireland vs Hungary (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 28. total_shots_on_target @ 5.5 — Northern Ireland vs Hungary (sin rol)
- basis: not_reproducible
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 29. total_shots_on_target @ 6.5 — Northern Ireland vs Hungary (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

### 30. total_shots_on_target @ 8.5 — Northern Ireland vs Hungary (sin rol)
- basis: not_reproducible
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
-   last5: INSUFFICIENT_SAMPLE
-   last10: INSUFFICIENT_SAMPLE
-   all: INSUFFICIENT_SAMPLE
- Split home/away:
-   home: NO_DATA
-   away: NO_DATA
- Rival (contexto equivalente):
  FM reportado: no disponible
  Ventanas propias:
    last5: INSUFFICIENT_SAMPLE
    last10: INSUFFICIENT_SAMPLE
    all: INSUFFICIENT_SAMPLE
  Split home/away:
    home: NO_DATA
    away: NO_DATA
- overlap_flags: ninguno
- data_quality: suspect=false, source_conflict=false, motivo="match-total basis not reproducible from fm_team_matches (team_stats_json stores team-own stats only)"

## Señales de jugadores (orden: mayor muestra disponible, no fuerza de señal)

### 1. shots @ 0.5 — I. Price
- basis: player_own_stats
- FM reportado: hits 7/8; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.4, median=1, min=0, max=3
  last10: n=10, hits=8, rate=0.800, mean=1.8, median=1.5, min=0, max=5
  all: n=30, hits=25, rate=0.833, mean=1.8333, median=2, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 2. shots @ 1.5 — D. Szoboszlai
- basis: player_own_stats
- FM reportado: hits 8/9; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.6, median=3, min=1, max=4
  last10: n=10, hits=8, rate=0.800, mean=2.8, median=3, min=1, max=4
  all: n=30, hits=20, rate=0.667, mean=2.4667, median=2.5, min=0, max=9
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 3. shots_created @ 1.5 — D. Szoboszlai
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=3.8, median=4, min=1, max=7
  last10: n=10, hits=8, rate=0.800, mean=3.9, median=4, min=0, max=7
  all: n=30, hits=24, rate=0.800, mean=3.9, median=4, min=0, max=8
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 4. shots_created @ 2.5 — D. Szoboszlai
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=3.8, median=4, min=1, max=7
  last10: n=10, hits=8, rate=0.800, mean=3.9, median=4, min=0, max=7
  all: n=30, hits=20, rate=0.667, mean=3.9, median=4, min=0, max=8
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 5. shots_created @ 3.5 — D. Szoboszlai
- basis: player_own_stats
- FM reportado: hits 7/9; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=3.8, median=4, min=1, max=7
  last10: n=10, hits=7, rate=0.700, mean=3.9, median=4, min=0, max=7
  all: n=30, hits=18, rate=0.600, mean=3.9, median=4, min=0, max=8
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 6. shots @ 0.5 — S. Charles
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1.4, median=1, min=0, max=5
  last10: n=10, hits=7, rate=0.700, mean=1.3, median=1, min=0, max=5
  all: n=28, hits=16, rate=0.571, mean=0.8571, median=1, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 7. shots @ 0.5 — A. Schäfer
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.2, median=1, min=0, max=2
  last10: n=10, hits=6, rate=0.600, mean=0.8, median=1, min=0, max=2
  all: n=23, hits=13, rate=0.565, mean=0.8696, median=1, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 8. shots @ 0.5 — M. Kerkez
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.4, median=1, min=0, max=4
  last10: n=10, hits=5, rate=0.500, mean=0.8, median=0.5, min=0, max=4
  all: n=23, hits=11, rate=0.478, mean=0.6087, median=0, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 9. foul_involvements @ 1.5 — D. Szoboszlai
- basis: player_own_stats
- FM reportado: hits 5/5; mejor ventana: 5 (5/5)
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=3.6, median=3, min=2, max=6
  last10: n=10, hits=8, rate=0.800, mean=2.6, median=2.5, min=0, max=6
  all: n=15, hits=12, rate=0.800, mean=2.8667, median=3, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 10. foul_involvements @ 2.5 — D. Szoboszlai
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=3.6, median=3, min=2, max=6
  last10: n=10, hits=5, rate=0.500, mean=2.6, median=2.5, min=0, max=6
  all: n=15, hits=8, rate=0.533, mean=2.8667, median=3, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 11. fouls_committed @ 0.5 — D. Szoboszlai
- basis: player_own_stats
- FM reportado: hits 4/5; mejor ventana: 5 (4/5)
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1, median=1, min=0, max=2
  last10: n=10, hits=5, rate=0.500, mean=0.6, median=0.5, min=0, max=2
  all: n=15, hits=9, rate=0.600, mean=0.8, median=1, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 12. fouls_drawn @ 0.5 — D. Szoboszlai
- basis: player_own_stats
- FM reportado: hits 7/7; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=2.6, median=2, min=1, max=6
  last10: n=10, hits=9, rate=0.900, mean=2, median=2, min=0, max=6
  all: n=15, hits=13, rate=0.867, mean=2.0667, median=2, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 13. fouls_drawn @ 0.5 — W. Orbán
- basis: player_own_stats
- FM reportado: hits 7/8; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.2, median=1, min=0, max=3
  last10: n=10, hits=7, rate=0.700, mean=1, median=1, min=0, max=3
  all: n=15, hits=9, rate=0.600, mean=0.8667, median=1, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 14. shots @ 0.5 — J. Donley
- basis: player_own_stats
- FM reportado: hits 5/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.6, median=1, min=0, max=3
  last10: n=10, hits=5, rate=0.500, mean=0.9, median=0.5, min=0, max=3
  all: n=15, hits=7, rate=0.467, mean=0.7333, median=0, min=0, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 15. foul_involvements @ 1.5 — S. Charles
- basis: player_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.8, median=3, min=1, max=4
  last10: n=10, hits=9, rate=0.900, mean=3.5, median=3, min=1, max=7
  all: n=13, hits=12, rate=0.923, mean=3.3846, median=3, min=1, max=7
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 16. foul_involvements @ 2.5 — S. Charles
- basis: player_own_stats
- FM reportado: hits 7/8; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.8, median=3, min=1, max=4
  last10: n=10, hits=8, rate=0.800, mean=3.5, median=3, min=1, max=7
  all: n=13, hits=9, rate=0.692, mean=3.3846, median=3, min=1, max=7
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 17. fouls_committed @ 0.5 — S. Charles
- basis: player_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1, median=1, min=0, max=2
  last10: n=10, hits=9, rate=0.900, mean=1.6, median=1.5, min=0, max=4
  all: n=13, hits=11, rate=0.846, mean=1.3846, median=1, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 18. fouls_drawn @ 0.5 — S. Charles
- basis: player_own_stats
- FM reportado: hits 9/10; mejor ventana: 10 (9/10)
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.8, median=2, min=0, max=4
  last10: n=10, hits=9, rate=0.900, mean=1.9, median=1.5, min=0, max=4
  all: n=13, hits=12, rate=0.923, mean=2, median=2, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 19. tackles @ 1.5 — J. Devenny
- basis: player_own_stats
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=3.4, median=3, min=3, max=4
  last10: n=10, hits=10, rate=1.000, mean=3.8, median=3.5, min=2, max=7
  all: n=13, hits=12, rate=0.923, mean=3.3077, median=3, min=0, max=7
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 20. tackles @ 1.5 — S. Charles
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1.6, median=2, min=0, max=3
  last10: n=10, hits=6, rate=0.600, mean=2.1, median=2, min=0, max=6
  all: n=13, hits=8, rate=0.615, mean=2.0769, median=2, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 21. tackles @ 2.5 — J. Devenny
- basis: player_own_stats
- FM reportado: hits 9/9; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=3.4, median=3, min=3, max=4
  last10: n=10, hits=9, rate=0.900, mean=3.8, median=3.5, min=2, max=7
  all: n=13, hits=10, rate=0.769, mean=3.3077, median=3, min=0, max=7
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 22. foul_involvements @ 1.5 — A. Schäfer
- basis: player_own_stats
- FM reportado: hits 8/10; mejor ventana: 10 (8/10)
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.6, median=3, min=1, max=4
  last10: n=10, hits=8, rate=0.800, mean=2.9, median=3, min=1, max=7
  all: n=12, hits=10, rate=0.833, mean=2.8333, median=3, min=1, max=7
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 23. foul_involvements @ 1.5 — M. Kerkez
- basis: player_own_stats
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=2.6, median=2, min=2, max=4
  last10: n=10, hits=8, rate=0.800, mean=2.1, median=2, min=1, max=4
  all: n=12, hits=10, rate=0.833, mean=2.1667, median=2, min=1, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 24. fouls_committed @ 0.5 — A. Schäfer
- basis: player_own_stats
- FM reportado: hits 4/5; mejor ventana: 5 (4/5)
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=1.2, median=1, min=0, max=2
  last10: n=10, hits=6, rate=0.600, mean=1.2, median=1, min=0, max=5
  all: n=12, hits=7, rate=0.583, mean=1.0833, median=1, min=0, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 25. fouls_drawn @ 0.5 — A. Schäfer
- basis: player_own_stats
- FM reportado: hits 10/10; mejor ventana: 10 (10/10)
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=1.4, median=1, min=1, max=2
  last10: n=10, hits=10, rate=1.000, mean=1.7, median=2, min=1, max=3
  all: n=12, hits=12, rate=1.000, mean=1.75, median=2, min=1, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 26. fouls_drawn @ 0.5 — M. Kerkez
- basis: player_own_stats
- FM reportado: hits 6/6; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=2, median=2, min=1, max=4
  last10: n=10, hits=9, rate=0.900, mean=1.5, median=1, min=0, max=4
  all: n=12, hits=11, rate=0.917, mean=1.5833, median=1.5, min=0, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 27. tackles @ 1.5 — T. Hume
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=2, median=2, min=0, max=5
  last10: n=10, hits=6, rate=0.600, mean=1.9, median=2, min=0, max=5
  all: n=12, hits=7, rate=0.583, mean=2.1667, median=2, min=0, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 28. goalkeeper_saves @ 1.5 — B. Tóth
- basis: player_own_stats
- FM reportado: hits 7/8; mejor ventana: 10 (7/8)
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.8, median=2, min=1, max=6
  last10: n=8, hits=7, rate=0.875, mean=2.75, median=2, min=1, max=6
  all: n=8, hits=7, rate=0.875, mean=2.75, median=2, min=1, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 29. goalkeeper_saves @ 1.5 — P. Charles
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=5, rate=1.000, mean=3, median=2, min=2, max=5
  last10: n=7, hits=5, rate=0.714, mean=2.4286, median=2, min=1, max=5
  all: n=7, hits=5, rate=0.714, mean=2.4286, median=2, min=1, max=5
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 30. foul_involvements @ 1.5 — D. Ballard
- basis: player_own_stats
- FM reportado: hits 4/4; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=3.2, median=3, min=1, max=6
  last10: n=6, hits=5, rate=0.833, mean=3.1667, median=3, min=1, max=6
  all: n=6, hits=5, rate=0.833, mean=3.1667, median=3, min=1, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 31. foul_involvements @ 2.5 — D. Ballard
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=3.2, median=3, min=1, max=6
  last10: n=6, hits=4, rate=0.667, mean=3.1667, median=3, min=1, max=6
  all: n=6, hits=4, rate=0.667, mean=3.1667, median=3, min=1, max=6
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 32. fouls_committed @ 1.5 — D. Ballard
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=3, rate=0.600, mean=1.8, median=2, min=1, max=3
  last10: n=6, hits=3, rate=0.500, mean=1.6667, median=1.5, min=1, max=3
  all: n=6, hits=3, rate=0.500, mean=1.6667, median=1.5, min=1, max=3
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 33. shots @ 0.5 — Á. Csongvai
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: 5 (3/3)
- Ventanas propias:
  last5: n=5, hits=2, rate=0.400, mean=0.6, median=0, min=0, max=2
  last10: n=6, hits=3, rate=0.500, mean=0.6667, median=0.5, min=0, max=2
  all: n=6, hits=3, rate=0.500, mean=0.6667, median=0.5, min=0, max=2
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 34. tackles @ 1.5 — M. Vitális
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: n=5, hits=4, rate=0.800, mean=2.4, median=2, min=1, max=4
  last10: n=5, hits=4, rate=0.800, mean=2.4, median=2, min=1, max=4
  all: n=5, hits=4, rate=0.800, mean=2.4, median=2, min=1, max=4
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 35. foul_involvements @ 1.5 — A. Osváth
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: no determinable
- Ventanas propias:
  last5: INSUFFICIENT_SAMPLE
  last10: INSUFFICIENT_SAMPLE
  all: INSUFFICIENT_SAMPLE
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

### 36. foul_involvements @ 1.5 — D. Bárány
- basis: player_own_stats
- FM reportado: hits 3/3; mejor ventana: 5 (3/3)
- Ventanas propias:
  last5: INSUFFICIENT_SAMPLE
  last10: INSUFFICIENT_SAMPLE
  all: INSUFFICIENT_SAMPLE
- overlap_flags: ninguno
  data_quality: suspect=false, source_conflict=false, motivo=""

## Evidencia histórica (ventanas fijas)
- observaciones descriptivas: secuencia cruda (más antiguo -> más reciente) y hits de las ventanas fijas (last5/last10/all)

### Jugadores — evidencia con ventanas fijas
- I. Price — shots @ 0.5: [1,0,2,5,3,1,2,1,0,3] | last5 4/5 | last10 8/10 | all 25/30
- D. Szoboszlai — shots @ 1.5: [1,4,4,2,4,3,2,3,4,1] | last5 4/5 | last10 8/10 | all 20/30
- D. Szoboszlai — shots_created @ 1.5: [0,4,4,6,6,4,1,4,7,3] | last5 4/5 | last10 8/10 | all 24/30
- D. Szoboszlai — shots_created @ 2.5: [0,4,4,6,6,4,1,4,7,3] | last5 4/5 | last10 8/10 | all 20/30
- D. Szoboszlai — shots_created @ 3.5: [0,4,4,6,6,4,1,4,7,3] | last5 3/5 | last10 7/10 | all 18/30
- S. Charles — shots @ 0.5: [1,1,0,3,1,0,0,1,1,5] | last5 3/5 | last10 7/10 | all 16/28
- A. Schäfer — shots @ 0.5: [1,0,0,1,0,0,1,2,2,1] | last5 4/5 | last10 6/10 | all 13/23
- M. Kerkez — shots @ 0.5: [0,1,0,0,0,0,1,1,4,1] | last5 4/5 | last10 5/10 | all 11/23
- D. Szoboszlai — foul_involvements @ 1.5: [2,3,0,2,1,3,2,3,6,4] | last5 5/5 | last10 8/10 | all 12/15
- D. Szoboszlai — foul_involvements @ 2.5: [2,3,0,2,1,3,2,3,6,4] | last5 4/5 | last10 5/10 | all 8/15
- D. Szoboszlai — fouls_committed @ 0.5: [0,1,0,0,0,1,1,2,0,1] | last5 4/5 | last10 5/10 | all 9/15
- D. Szoboszlai — fouls_drawn @ 0.5: [2,2,0,2,1,2,1,1,6,3] | last5 5/5 | last10 9/10 | all 13/15
- W. Orbán — fouls_drawn @ 0.5: [0,0,1,2,1,1,1,0,3,1] | last5 4/5 | last10 7/10 | all 9/15
- J. Donley — shots @ 0.5: [0,0,0,0,1,1,3,3,0,1] | last5 4/5 | last10 5/10 | all 7/15
- S. Charles — foul_involvements @ 1.5: [7,2,3,3,6,3,3,3,1,4] | last5 4/5 | last10 9/10 | all 12/13
- S. Charles — foul_involvements @ 2.5: [7,2,3,3,6,3,3,3,1,4] | last5 4/5 | last10 8/10 | all 9/13
- S. Charles — fouls_committed @ 0.5: [4,1,2,2,2,1,2,1,1,0] | last5 4/5 | last10 9/10 | all 11/13
- S. Charles — fouls_drawn @ 0.5: [3,1,1,1,4,2,1,2,0,4] | last5 4/5 | last10 9/10 | all 12/13
- J. Devenny — tackles @ 1.5: [2,7,3,5,4,4,4,3,3,3] | last5 5/5 | last10 10/10 | all 12/13
- S. Charles — tackles @ 1.5: [4,0,0,6,3,1,0,2,2,3] | last5 3/5 | last10 6/10 | all 8/13
- J. Devenny — tackles @ 2.5: [2,7,3,5,4,4,4,3,3,3] | last5 5/5 | last10 9/10 | all 10/13
- A. Schäfer — foul_involvements @ 1.5: [3,3,2,7,1,3,2,1,3,4] | last5 4/5 | last10 8/10 | all 10/12
- M. Kerkez — foul_involvements @ 1.5: [2,2,1,1,2,4,3,2,2,2] | last5 5/5 | last10 8/10 | all 10/12
- A. Schäfer — fouls_committed @ 0.5: [0,1,0,5,0,1,1,0,2,2] | last5 4/5 | last10 6/10 | all 7/12
- A. Schäfer — fouls_drawn @ 0.5: [3,2,2,2,1,2,1,1,1,2] | last5 5/5 | last10 10/10 | all 12/12
- M. Kerkez — fouls_drawn @ 0.5: [2,1,1,0,1,4,2,1,2,1] | last5 5/5 | last10 9/10 | all 11/12
- T. Hume — tackles @ 1.5: [2,2,1,1,3,0,2,1,2,5] | last5 3/5 | last10 6/10 | all 7/12
- B. Tóth — goalkeeper_saves @ 1.5: [2,4,2,6,2,3,1,2] | last5 4/5 | last10 7/8 | all 7/8
- P. Charles — goalkeeper_saves @ 1.5: [1,1,2,5,2,2,4] | last5 5/5 | last10 5/7 | all 5/7
- D. Ballard — foul_involvements @ 1.5: [3,1,2,6,3,4] | last5 4/5 | last10 5/6 | all 5/6

### Equipos — evidencia con ventanas fijas
- Hungary — away_corners @ 4.5: [1,10,4,7,8,9,3,3,3,4] | last5 4/5 | last10 6/10 | all 17/30
  - Rival (Northern Ireland): [5,6,4,2,5,3,2,1,1,1] | last5 5/5 | last10 7/10 | all 16/30
- Hungary — away_corners @ 5.5: [1,10,4,7,8,9,3,3,3,4] | last5 4/5 | last10 6/10 | all 18/30
  - Rival (Northern Ireland): [5,6,4,2,5,3,2,1,1,1] | last5 5/5 | last10 9/10 | all 22/30
- Hungary — away_shots @ 10.5: [5,13,17,14,18,16,12,18,20,17] | last5 5/5 | last10 9/10 | all 22/30
  - Rival (Northern Ireland): [3,11,12,9,9,8,10,9,4,12] | last5 1/5 | last10 3/10 | all 12/30
- Hungary — away_shots @ 12.5: [5,13,17,14,18,16,12,18,20,17] | last5 4/5 | last10 8/10 | all 17/30
  - Rival (Northern Ireland): [3,11,12,9,9,8,10,9,4,12] | last5 0/5 | last10 0/10 | all 6/30
- Hungary — away_shots_on_target @ 2.5: [2,4,7,3,7,6,3,2,6,4] | last5 4/5 | last10 8/10 | all 22/30
  - Rival (Northern Ireland): [1,4,3,2,3,1,4,4,2,5] | last5 3/5 | last10 6/10 | all 18/30
- Northern Ireland — home_corners @ 3.5: [5,6,4,2,5,3,2,1,1,1] | last5 5/5 | last10 6/10 | all 13/30
  - Rival (Hungary): [1,10,4,7,8,9,3,3,3,4] | last5 3/5 | last10 4/10 | all 14/30
- Northern Ireland — home_corners @ 4.5: [5,6,4,2,5,3,2,1,1,1] | last5 5/5 | last10 7/10 | all 16/30
  - Rival (Hungary): [1,10,4,7,8,9,3,3,3,4] | last5 4/5 | last10 6/10 | all 17/30
- Northern Ireland — home_corners @ 5.5: [5,6,4,2,5,3,2,1,1,1] | last5 5/5 | last10 9/10 | all 22/30
  - Rival (Hungary): [1,10,4,7,8,9,3,3,3,4] | last5 4/5 | last10 6/10 | all 18/30
- Northern Ireland — home_goals @ 0.5: [1,2,0,0,1,0,1,1,1,1] | last5 4/5 | last10 7/10 | all 22/30
  - Rival (Hungary): [2,2,2,1,2,1,0,2,3,0] | last5 3/5 | last10 8/10 | all 21/30
- Hungary — home_goals @ 1.5: [2,2,2,1,2,1,0,2,3,0] | last5 3/5 | last10 4/10 | all 17/30
  - Rival (Northern Ireland): [1,2,0,0,1,0,1,1,1,1] | last5 5/5 | last10 9/10 | all 21/30
- Northern Ireland — home_goals @ 1.5: [1,2,0,0,1,0,1,1,1,1] | last5 5/5 | last10 9/10 | all 21/30
  - Rival (Hungary): [2,2,2,1,2,1,0,2,3,0] | last5 3/5 | last10 4/10 | all 17/30
- Northern Ireland — home_shots @ 10.5: [3,11,12,9,9,8,10,9,4,12] | last5 4/5 | last10 7/10 | all 18/30
  - Rival (Hungary): [5,13,17,14,18,16,12,18,20,17] | last5 0/5 | last10 1/10 | all 8/30
- Northern Ireland — home_shots @ 12.5: [3,11,12,9,9,8,10,9,4,12] | last5 5/5 | last10 10/10 | all 24/30
  - Rival (Hungary): [5,13,17,14,18,16,12,18,20,17] | last5 1/5 | last10 2/10 | all 13/30
- Northern Ireland — home_shots_on_target @ 4.5: [1,4,3,2,3,1,4,4,2,5] | last5 4/5 | last10 9/10 | all 22/30
  - Rival (Hungary): [2,4,7,3,7,6,3,2,6,4] | last5 3/5 | last10 6/10 | all 14/30
- Northern Ireland vs Hungary — total_goals @ 2.5: [4,2,1,1,1,2,2,1,4,1] | last5 4/5 | last10 8/10 | all 21/30
- Northern Ireland vs Hungary — total_goals @ 3.5: [4,2,1,1,1,2,2,1,4,1] | last5 4/5 | last10 8/10 | all 23/30

## Confluencia descriptiva (equipo + rival + jugadores)
- familias de mercado con 2 o más series observadas en la DB; cada serie conserva su línea y sus ventanas fijas (last5/last10/all); sin score ni orden por fuerza

### corners
- Hungary (away) @ 4.5: last5 4/5 | last10 6/10 | all 17/30
- Northern Ireland (home) @ 4.5: last5 5/5 | last10 7/10 | all 16/30
- Hungary (away) @ 5.5: last5 4/5 | last10 6/10 | all 18/30
- Northern Ireland (home) @ 5.5: last5 5/5 | last10 9/10 | all 22/30
- Northern Ireland (home) @ 3.5: last5 5/5 | last10 6/10 | all 13/30
- Hungary (away) @ 3.5: last5 3/5 | last10 4/10 | all 14/30

### foul_involvements
- jugador D. Szoboszlai — foul_involvements @ 1.5: last5 5/5 | last10 8/10 | all 12/15
- jugador D. Szoboszlai — foul_involvements @ 2.5: last5 4/5 | last10 5/10 | all 8/15
- jugador S. Charles — foul_involvements @ 1.5: last5 4/5 | last10 9/10 | all 12/13
- jugador S. Charles — foul_involvements @ 2.5: last5 4/5 | last10 8/10 | all 9/13
- jugador A. Schäfer — foul_involvements @ 1.5: last5 4/5 | last10 8/10 | all 10/12
- jugador M. Kerkez — foul_involvements @ 1.5: last5 5/5 | last10 8/10 | all 10/12
- jugador D. Ballard — foul_involvements @ 1.5: last5 4/5 | last10 5/6 | all 5/6

### fouls_committed
- jugador D. Szoboszlai — fouls_committed @ 0.5: last5 4/5 | last10 5/10 | all 9/15
- jugador S. Charles — fouls_committed @ 0.5: last5 4/5 | last10 9/10 | all 11/13
- jugador A. Schäfer — fouls_committed @ 0.5: last5 4/5 | last10 6/10 | all 7/12

### fouls_drawn
- jugador D. Szoboszlai — fouls_drawn @ 0.5: last5 5/5 | last10 9/10 | all 13/15
- jugador W. Orbán — fouls_drawn @ 0.5: last5 4/5 | last10 7/10 | all 9/15
- jugador S. Charles — fouls_drawn @ 0.5: last5 4/5 | last10 9/10 | all 12/13
- jugador A. Schäfer — fouls_drawn @ 0.5: last5 5/5 | last10 10/10 | all 12/12
- jugador M. Kerkez — fouls_drawn @ 0.5: last5 5/5 | last10 9/10 | all 11/12

### goalkeeper_saves
- jugador B. Tóth — goalkeeper_saves @ 1.5: last5 4/5 | last10 7/8 | all 7/8
- jugador P. Charles — goalkeeper_saves @ 1.5: last5 5/5 | last10 5/7 | all 5/7

### goals
- Northern Ireland (home) @ 0.5: last5 4/5 | last10 7/10 | all 22/30
- Hungary (away) @ 0.5: last5 3/5 | last10 8/10 | all 21/30
- Hungary (away) @ 1.5: last5 3/5 | last10 4/10 | all 17/30
- Northern Ireland (home) @ 1.5: last5 5/5 | last10 9/10 | all 21/30

### shots
- Hungary (away) @ 10.5: last5 5/5 | last10 9/10 | all 22/30
- Northern Ireland (home) @ 10.5: last5 1/5 | last10 3/10 | all 12/30
- Hungary (away) @ 12.5: last5 4/5 | last10 8/10 | all 17/30
- Northern Ireland (home) @ 12.5: last5 0/5 | last10 0/10 | all 6/30
- jugador I. Price — shots @ 0.5: last5 4/5 | last10 8/10 | all 25/30
- jugador D. Szoboszlai — shots @ 1.5: last5 4/5 | last10 8/10 | all 20/30
- jugador S. Charles — shots @ 0.5: last5 3/5 | last10 7/10 | all 16/28
- jugador A. Schäfer — shots @ 0.5: last5 4/5 | last10 6/10 | all 13/23
- jugador M. Kerkez — shots @ 0.5: last5 4/5 | last10 5/10 | all 11/23
- jugador J. Donley — shots @ 0.5: last5 4/5 | last10 5/10 | all 7/15

### shots_created
- jugador D. Szoboszlai — shots_created @ 1.5: last5 4/5 | last10 8/10 | all 24/30
- jugador D. Szoboszlai — shots_created @ 2.5: last5 4/5 | last10 8/10 | all 20/30
- jugador D. Szoboszlai — shots_created @ 3.5: last5 3/5 | last10 7/10 | all 18/30

### shots_on_target
- Hungary (away) @ 2.5: last5 4/5 | last10 8/10 | all 22/30
- Northern Ireland (home) @ 2.5: last5 3/5 | last10 6/10 | all 18/30
- Northern Ireland (home) @ 4.5: last5 4/5 | last10 9/10 | all 22/30
- Hungary (away) @ 4.5: last5 3/5 | last10 6/10 | all 14/30

### tackles
- jugador J. Devenny — tackles @ 1.5: last5 5/5 | last10 10/10 | all 12/13
- jugador S. Charles — tackles @ 1.5: last5 3/5 | last10 6/10 | all 8/13
- jugador J. Devenny — tackles @ 2.5: last5 5/5 | last10 9/10 | all 10/13
- jugador T. Hume — tackles @ 1.5: last5 3/5 | last10 6/10 | all 7/12

## Cuotas

| mercado | línea | casa | lado | cuota | prob. implícita | capturada |
|---|---|---|---|---|---|---|
| away_corners | 4.5 | Kambi | over | 2.04 | 0.490 | 2026-09-28T21:38:06.025Z |
| away_corners | 4.5 | Kambi | under | 1.65 | 0.606 | 2026-09-28T21:38:06.025Z |
| away_corners | 4.5 | Ladbrokes | over | 1.91 | 0.524 | 2026-09-28T21:38:06.025Z |
| away_corners | 4.5 | Ladbrokes | under | 1.8 | 0.556 | 2026-09-28T21:38:06.025Z |
| away_corners | 5.5 | Kambi | over | 2.85 | 0.351 | 2026-09-28T21:38:06.025Z |
| away_corners | 5.5 | Kambi | under | 1.33 | 0.752 | 2026-09-28T21:38:06.025Z |
| away_shots_on_target | 2.5 | Kambi | over | 1.32 | 0.758 | 2026-09-28T21:38:06.025Z |
| away_shots_on_target | 2.5 | Kambi | under | 2.63 | 0.380 | 2026-09-28T21:38:06.025Z |
| home_corners | 3.5 | Kambi | over | 1.58 | 0.633 | 2026-09-28T21:38:06.025Z |
| home_corners | 3.5 | Kambi | under | 2.15 | 0.465 | 2026-09-28T21:38:06.025Z |
| home_corners | 3.5 | Ladbrokes | over | 1.7 | 0.588 | 2026-09-28T21:38:06.025Z |
| home_corners | 3.5 | Ladbrokes | under | 2.05 | 0.488 | 2026-09-28T21:38:06.025Z |
| home_corners | 4.5 | Kambi | over | 2.12 | 0.472 | 2026-09-28T21:38:06.025Z |
| home_corners | 4.5 | Kambi | under | 1.6 | 0.625 | 2026-09-28T21:38:06.025Z |
| home_corners | 5.5 | Kambi | over | 3 | 0.333 | 2026-09-28T21:38:06.025Z |
| home_corners | 5.5 | Kambi | under | 1.3 | 0.769 | 2026-09-28T21:38:06.025Z |
| home_goals | 0.5 | Kambi | over | 1.46 | 0.685 | 2026-09-28T21:38:06.025Z |
| home_goals | 0.5 | Kambi | under | 2.55 | 0.392 | 2026-09-28T21:38:06.025Z |
| home_goals | 0.5 | Ladbrokes | over | 1.44 | 0.694 | 2026-09-28T21:38:06.025Z |
| home_goals | 0.5 | Ladbrokes | under | 2.5 | 0.400 | 2026-09-28T21:38:06.025Z |
| home_goals | 1.5 | Bet365 | over | 3.25 | 0.308 | 2026-09-28T21:38:06.025Z |
| home_goals | 1.5 | Bet365 | under | 1.33 | 0.752 | 2026-09-28T21:38:06.025Z |
| home_goals | 1.5 | Kambi | over | 3.2 | 0.312 | 2026-09-28T21:38:06.025Z |
| home_goals | 1.5 | Kambi | under | 1.3 | 0.769 | 2026-09-28T21:38:06.025Z |
| home_goals | 1.5 | Ladbrokes | over | 3.25 | 0.308 | 2026-09-28T21:38:06.025Z |
| home_goals | 1.5 | Ladbrokes | under | 1.3 | 0.769 | 2026-09-28T21:38:06.025Z |
| home_goals | 1.5 | Bet365 | over | 3.25 | 0.308 | 2026-09-28T21:38:06.025Z |
| home_goals | 1.5 | Bet365 | under | 1.33 | 0.752 | 2026-09-28T21:38:06.025Z |
| home_goals | 1.5 | Kambi | over | 3.2 | 0.312 | 2026-09-28T21:38:06.025Z |
| home_goals | 1.5 | Kambi | under | 1.3 | 0.769 | 2026-09-28T21:38:06.025Z |
| home_goals | 1.5 | Ladbrokes | over | 3.25 | 0.308 | 2026-09-28T21:38:06.025Z |
| home_goals | 1.5 | Ladbrokes | under | 1.3 | 0.769 | 2026-09-28T21:38:06.025Z |
| home_shots_on_target | 4.5 | Kambi | over | 3.05 | 0.328 | 2026-09-28T21:38:06.025Z |
| home_shots_on_target | 4.5 | Kambi | under | 1.24 | 0.806 | 2026-09-28T21:38:06.025Z |
| total_goals | 2.5 | Bet365 | over | 2.68 | 0.373 | 2026-09-28T21:38:06.025Z |
| total_goals | 2.5 | Bet365 | under | 1.45 | 0.690 | 2026-09-28T21:38:06.025Z |
| total_goals | 2.5 | Kambi | over | 2.7 | 0.370 | 2026-09-28T21:38:06.025Z |
| total_goals | 2.5 | Kambi | under | 1.44 | 0.694 | 2026-09-28T21:38:06.025Z |
| total_goals | 2.5 | Ladbrokes | over | 2.7 | 0.370 | 2026-09-28T21:38:06.025Z |
| total_goals | 2.5 | Ladbrokes | under | 1.4 | 0.714 | 2026-09-28T21:38:06.025Z |
| total_goals | 2.5 | Paddy Power | over | 2.75 | 0.364 | 2026-09-28T21:38:06.025Z |
| total_goals | 2.5 | Paddy Power | under | 1.4 | 0.714 | 2026-09-28T21:38:06.025Z |
| total_goals | 3.5 | Bet365 | over | 5.5 | 0.182 | 2026-09-28T21:38:06.025Z |
| total_goals | 3.5 | Bet365 | under | 1.14 | 0.877 | 2026-09-28T21:38:06.025Z |
| total_goals | 3.5 | Kambi | over | 5.3 | 0.189 | 2026-09-28T21:38:06.025Z |
| total_goals | 3.5 | Kambi | under | 1.13 | 0.885 | 2026-09-28T21:38:06.025Z |
| total_goals | 3.5 | Ladbrokes | over | 5.5 | 0.182 | 2026-09-28T21:38:06.025Z |
| total_goals | 3.5 | Ladbrokes | under | 1.12 | 0.893 | 2026-09-28T21:38:06.025Z |
| total_goals | 3.5 | Paddy Power | over | 5.5 | 0.182 | 2026-09-28T21:38:06.025Z |
| total_goals | 3.5 | Paddy Power | under | 1.11 | 0.901 | 2026-09-28T21:38:06.025Z |
| total_cards | 2.5 | Paddy Power | over | 1.17 | 0.855 | 2026-09-28T21:38:06.025Z |
| total_cards | 2.5 | Paddy Power | under | 4.33 | 0.231 | 2026-09-28T21:38:06.025Z |
| total_cards | 4.5 | Kambi | over | 2.02 | 0.495 | 2026-09-28T21:38:06.025Z |
| total_cards | 4.5 | Kambi | under | 1.6 | 0.625 | 2026-09-28T21:38:06.025Z |
| total_cards | 4.5 | Ladbrokes | over | 2.1 | 0.476 | 2026-09-28T21:38:06.025Z |
| total_cards | 4.5 | Ladbrokes | under | 1.65 | 0.606 | 2026-09-28T21:38:06.025Z |
| total_cards | 4.5 | Paddy Power | over | 2.15 | 0.465 | 2026-09-28T21:38:06.025Z |
| total_cards | 4.5 | Paddy Power | under | 1.6 | 0.625 | 2026-09-28T21:38:06.025Z |
| total_cards | 5.5 | Ladbrokes | over | 3.25 | 0.308 | 2026-09-28T21:38:06.025Z |
| total_cards | 5.5 | Ladbrokes | under | 1.28 | 0.781 | 2026-09-28T21:38:06.025Z |
| total_corners | 5.5 | Kambi | over | 1.12 | 0.893 | 2026-09-28T21:38:06.025Z |
| total_corners | 5.5 | Kambi | under | 4.9 | 0.204 | 2026-09-28T21:38:06.025Z |
| total_corners | 6.5 | Kambi | over | 1.26 | 0.794 | 2026-09-28T21:38:06.025Z |
| total_corners | 6.5 | Kambi | under | 3.4 | 0.294 | 2026-09-28T21:38:06.025Z |
| total_corners | 6.5 | Ladbrokes | over | 1.25 | 0.800 | 2026-09-28T21:38:06.025Z |
| total_corners | 6.5 | Ladbrokes | under | 3.7 | 0.270 | 2026-09-28T21:38:06.025Z |
| total_corners | 7.5 | Kambi | over | 1.47 | 0.680 | 2026-09-28T21:38:06.025Z |
| total_corners | 7.5 | Kambi | under | 2.43 | 0.412 | 2026-09-28T21:38:06.025Z |
| total_corners | 7.5 | Ladbrokes | over | 1.48 | 0.676 | 2026-09-28T21:38:06.025Z |
| total_corners | 7.5 | Ladbrokes | under | 2.5 | 0.400 | 2026-09-28T21:38:06.025Z |
| total_corners | 10.5 | Kambi | over | 3.05 | 0.328 | 2026-09-28T21:38:06.025Z |
| total_corners | 10.5 | Kambi | under | 1.3 | 0.769 | 2026-09-28T21:38:06.025Z |
| total_corners | 10.5 | Ladbrokes | over | 3.3 | 0.303 | 2026-09-28T21:38:06.025Z |
| total_corners | 10.5 | Ladbrokes | under | 1.28 | 0.781 | 2026-09-28T21:38:06.025Z |
| total_corners | 10.5 | Paddy Power | over | 3.2 | 0.312 | 2026-09-28T21:38:06.025Z |
| total_corners | 10.5 | Paddy Power | under | 1.29 | 0.775 | 2026-09-28T21:38:06.025Z |
| total_corners | 11.5 | Kambi | over | 4 | 0.250 | 2026-09-28T21:38:06.025Z |
| total_corners | 11.5 | Kambi | under | 1.18 | 0.847 | 2026-09-28T21:38:06.025Z |
| total_corners | 11.5 | Paddy Power | over | 4.5 | 0.222 | 2026-09-28T21:38:06.025Z |
| total_corners | 11.5 | Paddy Power | under | 1.15 | 0.870 | 2026-09-28T21:38:06.025Z |
| total_corners | 12.5 | Kambi | over | 5.5 | 0.182 | 2026-09-28T21:38:06.025Z |
| total_corners | 12.5 | Kambi | under | 1.1 | 0.909 | 2026-09-28T21:38:06.025Z |
| total_corners | 12.5 | Paddy Power | over | 6.5 | 0.154 | 2026-09-28T21:38:06.025Z |
| total_corners | 12.5 | Paddy Power | under | 1.07 | 0.935 | 2026-09-28T21:38:06.025Z |
| total_shots_on_target | 5.5 | Kambi | over | 1.32 | 0.758 | 2026-09-28T21:38:06.025Z |
| total_shots_on_target | 5.5 | Kambi | under | 2.85 | 0.351 | 2026-09-28T21:38:06.025Z |
| total_shots_on_target | 6.5 | Kambi | over | 1.64 | 0.610 | 2026-09-28T21:38:06.025Z |
| total_shots_on_target | 6.5 | Kambi | under | 1.98 | 0.505 | 2026-09-28T21:38:06.025Z |
| total_shots_on_target | 8.5 | Kambi | over | 3 | 0.333 | 2026-09-28T21:38:06.025Z |
| total_shots_on_target | 8.5 | Kambi | under | 1.28 | 0.781 | 2026-09-28T21:38:06.025Z |
- Cuota manual Betano (away_corners @ 4.5): no cargada
- Cuota manual Betano (away_corners @ 5.5): no cargada
- Cuota manual Betano (away_shots @ 10.5): no cargada
- Cuota manual Betano (away_shots @ 12.5): no cargada
- Cuota manual Betano (away_shots_on_target @ 2.5): no cargada
- Cuota manual Betano (home_corners @ 3.5): no cargada
- Cuota manual Betano (home_corners @ 4.5): no cargada
- Cuota manual Betano (home_corners @ 5.5): no cargada
- Cuota manual Betano (home_goals @ 0.5): no cargada
- Cuota manual Betano (home_goals @ 1.5): no cargada
- Cuota manual Betano (home_goals @ 1.5): no cargada
- Cuota manual Betano (home_shots @ 10.5): no cargada
- Cuota manual Betano (home_shots @ 12.5): no cargada
- Cuota manual Betano (home_shots_on_target @ 4.5): no cargada
- Cuota manual Betano (total_goals @ 2.5): no cargada
- Cuota manual Betano (total_goals @ 3.5): no cargada
- Cuota manual Betano (1x2_shots): no cargada
- Cuota manual Betano (total_cards @ 2.5): no cargada
- Cuota manual Betano (total_cards @ 4.5): no cargada
- Cuota manual Betano (total_cards @ 5.5): no cargada
- Cuota manual Betano (total_corners @ 5.5): no cargada
- Cuota manual Betano (total_corners @ 6.5): no cargada
- Cuota manual Betano (total_corners @ 7.5): no cargada
- Cuota manual Betano (total_corners @ 10.5): no cargada
- Cuota manual Betano (total_corners @ 11.5): no cargada
- Cuota manual Betano (total_corners @ 12.5): no cargada
- Cuota manual Betano (total_shots @ 21.5): no cargada
- Cuota manual Betano (total_shots_on_target @ 5.5): no cargada
- Cuota manual Betano (total_shots_on_target @ 6.5): no cargada
- Cuota manual Betano (total_shots_on_target @ 8.5): no cargada

## Nota de cierre
Este reporte es descriptivo. No constituye una recomendación de apuesta ni una probabilidad validada de resultado futuro.

