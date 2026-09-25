# FM_AGENT_BRIEF_P4 — Venue real, corrección NO_DATE_MATCH, odds de FM, Parte C (reporte de confluencia)

## 0. ALCANCE Y REGLAS
Rigen docs/FM_AGENT_BRIEF.md §0/§1/§7 + docs/VISION.md (léelo, no lo repitas).
Cambio de política: la navegación YA NO tiene presupuesto máximo por fase. Mantén: 1 navegación a la vez, pausa 2–4s entre navegaciones, nunca paralelo, reintentos acotados. Explora hasta resolver cada bloqueante; documenta cuántas navegaciones usaste igualmente (para diagnóstico, no como límite).
PROHIBIDO seguir: scraping/API/automatización de Betano, cálculo de probabilidad/EV desde hit rate, recomendaciones tipo "apuesta esto". Cuota de Betano = solo input manual del usuario (campo, no fuente).
Lee primero docs/STATE.md y docs/HANDOFF_REVIEW.md + REPORT_P3.md (ya en el repo o pégalos si faltan). No repitas su contenido.
Fases con puerta: ejecuta Parte A, B, C. Antes de Parte D, PARA y reporta — es la que define el contrato de salida, y quiero revisarlo antes de fijarlo.

## PARTE A — Corregir NO_DATE_MATCH (verificar antes de tocar código)
A1. Toma 3 signal_id reales con unavailable_reason=NO_DATE_MATCH (mezcla team/player). Para cada uno, extrae su history[] crudo y compara: fecha más cercana disponible vs kickoffUtc real del fixture. Documenta en tmp/fm/p4_a1_manual_check.json: delta en días, y si existe algún elemento para ESE fixture específico en absoluto.
A2. Con eso, decide y corrige:
   - Si NINGÚN elemento cercano existe (el partido de hoy no está en history[] de ningún sujeto revisado) → el label correcto es NO_HISTORY_ELEMENT, no NO_DATE_MATCH. Corrige la lógica de clasificación.
   - Si SÍ hay un elemento pero con fecha desalineada por más de ±1 día → aumenta la tolerancia o corrige el parseo de fecha (revisa timezone: t podría venir en UTC o local de FM).
   - Si tras la corrección algunas siguen sin encontrar match → deja OTHER con el detalle exacto (no NO_DATE_MATCH genérico).
A3. Re-corre el resolver sobre las 108 filas afectadas. Reporta el delta real (cuántas pasan a RESOLVED, cuántas quedan y con qué reason correcto).

## PARTE B — Venue real (home/away) vía navegación extra
B1. Descubrimiento: para 1 fixture conocido, navega con location=match y confirma qué cambia en la respuesta JSON respecto a location=all (V5 ya lo tocó parcialmente — retómalo, no repitas navegaciones ya hechas si el resultado sigue en tmp/fm/). Verifica también si existe un parámetro directo location=home / location=away, o si el venue real se deriva de home/away structural del fixture (¿el propio equipo es local o visitante en ESE partido, dato ya conocido sin navegar?).
B2. Si location=home/away no es válido (V5 documentó que la API devuelve 400 para esos valores) pero location=match sí trae el split real, ese es el camino: 1 navegación extra por fixture (no por señal) para capturar el team-trends y player-trends con location=match, ADEMÁS del location=all ya existente.
B3. Persistencia: agrega snapshot/señales bajo params_json={"location":"match"} en paralelo a las existentes de location=all (no reemplazar, conservar ambas — permite comparar Any vs Home/Away real, tal como pedía tu método original).
B4. Actualiza FmFixtureSnapshotService para capturar ambos scopes por fixture. Actualiza el presupuesto de navegación de cualquier endpoint que dependa de esto (documentalo, sin límite duro).
B5. Reprocesa los fixtures ya existentes (33441811-17) con el nuevo scope y confirma que las señales quedan diferenciadas por params_json.

## PARTE C — Cuotas de FootyMetrics (descubrimiento primero, sin asumir)
C1. Descubrimiento con evidencia (NO asumir que existe): inspecciona el JSON de fixture/overview/trends de 2-3 fixtures. ¿Hay algún campo de cuota/odds/bookmaker en el payload? Si sí: documenta estructura exacta (casa, mercado, línea, valor, timestamp de la cuota). Si no aparece en los endpoints ya mapeados, revisa si existe un tab/endpoint adicional no explorado (p.ej. "odds" o "markets" en la navegación de FM) — 1-2 navegaciones de exploración dirigida, documentadas en tmp/fm/p4_c1_odds_discovery.json.
C2. Si existen: diseña tabla fm_market_odds(fixture_id, bookmaker, market, line, odds_value, side, source_timestamp_utc, snapshot_id). Guarda crudo, sin calcular probabilidad implícita todavía (eso es cálculo determinista trivial 1/odds, pero NO lo uses para decidir nada — solo almacénalo como dato descriptivo si lo agregas).
C3. Si NO existen (FM no expone cuotas de terceros en los datos a los que tenemos acceso): documenta NO ENCONTRADO con evidencia de dónde se buscó, y deja fm_market_odds como tabla vacía preparada para el campo manual de Betano únicamente. No inventes ni derives cuotas de ningún otro dato.
C4. Campo manual: agrega endpoint POST /api/fm/fixtures/{id}/manual-odds {bookmaker:"Betano", market, line, odds_value} → inserta en fm_market_odds con source="manual", bookmaker="Betano". Sin validación de valor razonable más allá de tipo/rango numérico básico.

## ACEPTACIÓN (solo PASS/FAIL + 1 línea)
- D1: A1-A3 — verificación manual documentada, corrección aplicada, delta real reportado (no la explicación previa sin probar).
- D2: B1-B5 — location=match capturado y persistido diferenciado de location=all para al menos los fixtures ya conocidos.
- D3: C1 — resultado de descubrimiento de odds documentado (existen con estructura, o NO ENCONTRADO con evidencia).
- D4: C4 — endpoint manual-odds funcional, probado con 1 inserción real.

## CIERRE (tras A, B, C — PARA antes de D)
Actualiza docs/STATE.md. Reporte ≤25 líneas: A1-A3, B1-B5, C1-C4 en una línea c/u, riesgos (máx. 5), y evidencia concreta de C1 (existe o no existe odds en FM). NO construyas la Parte D (reporte de confluencia con contexto+venue+odds) todavía — espera mi revisión del hallazgo de C1, porque el contrato de salida depende de si hay o no cuotas de FM que mostrar.