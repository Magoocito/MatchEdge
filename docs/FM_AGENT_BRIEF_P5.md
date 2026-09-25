# FM_AGENT_BRIEF_P5 — Mapeo bookmaker IDs, venue estructural, venue histórico, exploración dirigida

## 0. ALCANCE Y REGLAS
Rigen docs/FM_AGENT_BRIEF.md §0/§1/§7 (en `docs/new 6.txt`) + docs/VISION.md + docs/FM_AGENT_BRIEF_P4.md (léelos, no los repitas).
Política vigente: navegación sin presupuesto máximo por fase. Mantén: 1 navegación a la vez, pausa 2–4s, nunca paralelo, reintentos acotados. Documenta cuántas navegaciones usaste (diagnóstico, no límite).
PROHIBIDO: scraping/API/automatización de Betano, cálculo de probabilidad/EV desde hit rate, recomendaciones tipo "apuesta esto". Cuota de Betano = solo input manual del usuario.
Lee primero docs/STATE.md + REPORT_P4.md. No repitas su contenido.

## PARTE A — Mapeo bookmaker IDs → nombres de casas
A1. Evidencia ya existente: en la web de FM, por cada mercado en trends, se muestran cuotas con nombre de casa (ej. Kambi 1.97, Bet365 1.91, Ladbrokes 1.91). En el JSON guardado, data[].odds trae solo {bk: id, over, under, yes, no, most} con ids 1-5, sin nombre. En fm_market_odds se guardaron 1.247 filas con bk 3-5.
A2. Descubrimiento con evidencia (NO asumir): inspecciona el JSON crudo de 2-3 fixtures ya guardados en tmp/fm/ y busca cualquier campo que contenga el nombre de la casa (bookmaker, bk_name, house, provider, etc.) en cualquier nivel del payload. Documenta en tmp/fm/p5_a2_bk_name_search.json.
A3. Si el nombre NO aparece en el JSON: navega 1-2 veces de forma dirigida a la página de trends de un fixture conocido y captura el payload completo de la respuesta que alimenta el panel de cuotas (el que muestra Kambi/Bet365/Ladbrokes). Busca en ese payload el campo que mapea id → nombre. Documenta en tmp/fm/p5_a3_bk_mapping_nav.json.
A4. Si se encuentra el mapeo: crea tabla fm_bookmakers(bk_id, bk_name, source_timestamp_utc) y pobla con los ids 1-5. Actualiza fm_market_odds para que las filas existentes se puedan unir al nombre (JOIN o backfill). Si el mapeo no existe en ningún payload: documenta NO ENCONTRADO con evidencia de dónde se buscó, y deja el mapeo como tarea pendiente explícita.
A5. Verifica que el endpoint GET de odds por fixture devuelva el nombre de la casa cuando exista el mapeo.

## PARTE B — Venue estructural del fixture actual (NO el split histórico)
B1. El reporte P4 confirmó que venue estructural (Team vs Fixture.Home/Away) existe sin navegar. Implementa la derivación: para cada señal/fixture, marca si el equipo es local o visitante en ESE partido usando datos ya conocidos.
B2. Persiste venue_role (home/away/unknown) en las señales o en el snapshot. Documenta en qué tabla/columna queda.
B3. Confirma con 2-3 fixtures de ejemplo que la derivación es correcta comparando contra lo que muestra la web.
B4. Aclaración explícita (obligatoria): venue_role (B1-B3) es el contexto del fixture actual (local/visitante en ESE partido), NO el split histórico de rendimiento local/visitante que P4 buscaba vía location=match (que resultó vacío). Documenta en STATE.md esta distinción sin ambigüedad:
   "venue estructural (fixture actual): resuelto.
    Venue histórico (split de history[] por local/visitante): sigue sin fuente confirmada."

## PARTE C — Exploración dirigida de pestañas/endpoints no explorados
Principio: la suscripción se paga para explorar. No dejar nada al azar. Cada hallazgo se documenta con evidencia en tmp/fm/.
C1. Lista los tabs/endpoints YA explorados (de STATE.md y briefs anteriores) y los que NO. Identifica candidatos prometedores:
   - pestañas de la web que aún no tienen endpoint mapeado (ej. "same league only", paneles de jugadores, H2H, standings, cualquier tab visible en la UI de un fixture);
   - el fixtures-api global usado en el outcome resolver: verificar si acepta consultas por fecha pasada + equipo. Si sí, evaluar si permite cruzar cada elemento de history[] (fecha+rival) para derivar venue histórico sin depender de location=match.
C2. Navega de forma dirigida (1 navegación por candidato, pausa 2–4s) a 2-4 candidatos y captura el payload. Para cada uno documenta: qué trae, qué campos nuevos aporta, si es útil para el análisis 360° (contexto, confluencia, venue histórico, jugadores, cuotas).
C3. Si un candidato trae datos valiosos: diseña la persistencia (tabla/columnas) y el endpoint de lectura. Si no trae nada útil: documenta por qué se descarta.
C4. Prioriza por valor para el análisis 360°: primero lo que aporta contexto de partido (rival, competición, forma), luego lo que aporta venue histórico o datos de jugadores, luego lo demás.

## PARTE D — Guía de navegación manual para el usuario
D1. Crea docs/FM_MANUAL_NAVIGATION.md con una guía paso a paso para que el usuario navegue FootyMetrics como usuario humano y capture evidencia útil:
   - Qué URLs visitar (fixture, trends, odds, jugadores, H2H, standings).
   - Qué pestañas/paneles abrir y qué datos muestran (cuotas por casa, venue, historial, tendencias).
   - Cómo abrir DevTools (F12) → pestaña Network → filtrar por "fetch" o "XHR" → capturar el payload JSON de cada respuesta relevante.
   - Cómo copiar el payload (clic derecho → Copy → Copy response) y guardarlo en tmp/fm/ con nombre descriptivo (ej. tmp/fm/manual_trends_33441811.json).
   - Qué buscar en cada payload: campos de odds con nombre de casa, campos de venue/location, campos de historial con fecha+rival, campos de jugadores.
D2. Incluye ejemplos concretos con el fixture 33441811 (Norway vs Portugal, UEFA Nations League) como referencia.
D3. El usuario irá pasando URLs y capturas manuales; OpenCode debe contrastarlas con lo que la app obtiene automáticamente y reportar discrepancias.

## ACEPTACIÓN (solo PASS/FAIL + 1 línea)
- D1: A1-A5 — mapeo bk id→nombre encontrado con evidencia, o NO ENCONTRADO documentado. fm_bookmakers poblada si existe.
- D2: B1-B4 — venue_role derivado y persistido, verificado con 2-3 fixtures, y distinción venue estructural vs venue histórico documentada sin ambigüedad en STATE.md.
- D3: C1-C4 — lista de tabs explorados vs no explorados (incluyendo el candidato fixtures-api por fecha pasada), 2-4 navegaciones dirigidas documentadas con payloads, decisiones de persistir/descartar justificadas.
- D4: docs/FM_MANUAL_NAVIGATION.md creado con guía completa y ejemplos concretos.
- D5: docs/STATE.md actualizado con todo lo anterior.

## CIERRE
Reporte ≤25 líneas: A1-A5, B1-B4, C1-C4, D1-D3 en una línea c/u, riesgos (máx. 5), evidencia concreta de A4 (mapeo existe o no) y de C2 (qué tabs se exploraron y qué aportan).
NO construyas la Parte E (reporte de confluencia con contexto+venue+odds) — esa se diseña después, cuando el mapeo de casas y el estado real del venue histórico estén resueltos.
