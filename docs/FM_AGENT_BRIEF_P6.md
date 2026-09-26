# FM_AGENT_BRIEF_P6 — Implementar C3, ampliar cobertura de venue histórico

## ALCANCE
Continúa feature/fm-deterministic-navigation. Rigen reglas vigentes (navegación sin límite duro, ritmo cortés 1 a la vez + pausa 2–4s, prohibiciones de P4/P5: no scraping de Betano, no probabilidad/EV desde hit rate, no recomendaciones de apuesta).
Lee docs/STATE.md y REPORT_P5 primero. No los repitas.
PARA al terminar — no construyas Parte E todavía.

## G — Implementar persistencia (C3 de P5)
G1. Crea las tablas fm_team_matches y fm_player_matches tal como quedaron diseñadas en STATE.md (P5-C3). Migración idempotente.
G2. Puebla ambas tablas para los 7 fixtures ya conocidos (33441811-17), usando teams/table y position-stats/pivotData, con location=home y location=away por equipo.
G3. Implementa GET /api/fm/teams/{teamApid}/matches?location=home|away&period=15 (solo lectura de lo persistido, 0 navegaciones).
G4. Test: para Portugal y Gales, confirma que las filas persistidas coinciden con el crosscheck ya hecho en P5 (p5_c2_history_h_crosscheck). Documenta evidencia.

## H — Ampliar cobertura de venue histórico (fuera de UEFA Nations League)
H1. Elige 3-4 equipos de 2 ligas domésticas distintas (no selecciones nacionales). Preferir equipos ya conocidos o fáciles de resolver vía /api/fm/fixtures.
H2. Repite el crosscheck de P5-C2 (history[].h vs teams/table?location=home|away) para esos equipos. Documenta acuerdo/discrepancias con evidencia en tmp/fm/.
H3. Si aparece alguna discrepancia nueva (liga con estructura distinta, playoffs, etc.), documenta el caso y decide si el método sigue siendo válido en general o necesita ajuste.

## ACEPTACIÓN
- G: PASS si las 2 tablas existen, están pobladas para los 7 fixtures, y el endpoint de lectura responde sin navegar.
- H: PASS si ≥3 equipos de ligas domésticas confirman el mismo acuerdo history[].h vs teams/table, o se documenta con evidencia cualquier caso divergente.
- STATE.md actualizado.

## CIERRE
Reporte ≤15 líneas: G1-G4 y H1-H3 en una línea c/u, riesgos (máx. 3), evidencia concreta de G4 y H2.
PARA. No construyas Parte E.