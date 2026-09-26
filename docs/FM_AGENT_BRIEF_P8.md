# FM_AGENT_BRIEF_P8 — Mapeo de jugadores (position-stats) + corrección de duplicados en el reporte

## ALCANCE Y REGLAS
Rigen todas las reglas vigentes de P1-P7 (docs/STATE.md, docs/VISION.md). Rama feature/fm-deterministic-navigation.
PROHIBIDO: probabilidad/EV desde hit rate, recomendaciones de apuesta, scraping/automatización de Betano, ranking por fuerza de señal.
Lee primero docs/STATE.md. No lo repitas. PARA al terminar.

## PARTE J — Mapeo de jugadores vía position-stats
J1. Descubrimiento: confirma con evidencia qué valores de `positions=` acepta /api/front/position-stats (ya explorado parcialmente en P5-P6, ver STATE.md). Para 2-3 jugadores de los ya conocidos en fm_player_matches (ej. S. Posch, M. Solomon), identifica su posición real y arma la llamada correcta.
J2. Para esos mismos jugadores, obtén el payload con positions= explícito y localiza los campos de fouls_committed, fouls_drawn, foul_involvements, tackles, goalkeeper_saves (según posición — un delantero no tendrá goalkeeper_saves, por ejemplo). Documenta en tmp/fm/p8_j1_position_stats_fields.json qué campo exacto corresponde a cada mercado hoy marcado "unmapped".
J3. Si los campos existen: extiende fm_player_matches (o crea columna/tabla complementaria si el esquema de position-stats no encaja limpio en la fila existente — decide y documenta por qué) para persistir estos valores, con perspectiva explícita (ya existe la columna `perspective`, hoy solo 'team'; agrega 'position' o el nombre que corresponda).
J4. Actualiza el mapeo mercado→campo en FmConfluenceReportBuilder (o donde viva la lógica de "unmapped") para que estos 5 mercados dejen de caer en unmapped cuando el dato está disponible. Si algún mercado sigue sin campo real en FM, déjalo unmapped con motivo explícito (no inventar).
J5. Repuebla fm_player_matches (perspectiva nueva) para los 7 fixtures ya conocidos. Reprocesa el reporte de Austria vs Israel y confirma que al menos foul_involvements y goalkeeper_saves (los más frecuentes en el reporte de ejemplo) muestran own_windows reales en vez de INSUFFICIENT_SAMPLE.

## PARTE K — Corrección de duplicados en el reporte
K1. Reproduce el bug: en el reporte de Austria vs Israel (fixture con total_goals @ 1.5 dos veces, una vista desde cada equipo), confirma en código por qué FmConfluenceReportBuilder no las está tratando como la misma entrada de mercado.
K2. Corrige: un mercado de tipo "match total" (total_goals, total_corners, etc.) debe aparecer UNA sola vez por fixture+market+line, no una por cada equipo. Decide y documenta el criterio de "subject" para estos casos (ej. mostrar el mercado sin atribuirlo a un lado, ya que es del partido completo) — no es team_attack vs opponent_context, ambos equipos son "el mismo partido".
K3. Verifica si la tabla de cuotas (fm_market_odds) tiene el mismo problema de duplicación para mercados "total" (el reporte de ejemplo mostró total_goals@1.5 con las mismas 10 filas de casas repetidas dos veces). Si es el mismo bug raíz, corrígelo en el mismo lugar; si es independiente, documenta y corrige por separado.
K4. Regresión: agrega test que confirme que un fixture con mercados "total" duplicados en el origen produce una sola entrada en el reporte JSON y en la tabla de odds.

## PRUEBAS
L1. Reprocesa Austria vs Israel (u otro de los 7 fixtures si ese ya no está disponible) y confirma: cero mercados "total" duplicados, y al menos 2 mercados de jugador que antes eran unmapped ahora muestran datos reales.
L2. Corre F1-F4 e I1-I7 de P7 nuevamente sobre el fixture corregido para confirmar que no se rompió nada de lo ya validado (determinismo, estructura, grep de palabras prohibidas).

## ACEPTACIÓN (PASS/FAIL + 1 línea)
- J: PASS si ≥2 mercados de jugador dejan de ser unmapped con datos reales verificados a mano contra la web.
- K: PASS si el reporte y la tabla de odds ya no muestran mercados "total" duplicados, con test de regresión.
- L: PASS si P7 sigue verde tras los cambios.

## CIERRE
Actualiza docs/STATE.md. Reporte ≤20 líneas: J1-J5, K1-K4, L1-L2 en una línea c/u, riesgos (máx. 3), evidencia concreta de J2 (campos encontrados o no) y K1 (causa raíz del duplicado). PARA.