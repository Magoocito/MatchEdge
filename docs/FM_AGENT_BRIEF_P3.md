# FM_AGENT_BRIEF_P3 — Cierre de P2 + ventanas propias + confluencia (sin cuotas, sin recomendación)

## 0. ALCANCE Y REGLAS
Rigen docs/FM_AGENT_BRIEF.md §0/§1/§7 y las reglas ya aplicadas en P1/P2. Alcance único: FootyMetrics.
PROHIBIDO en esta fase: cuotas/odds (de cualquier fuente), EV, probabilidad de apuesta, scoring de "recomendación", picks, staking, Betano, cualquier verdicto tipo "apostar/no apostar". La salida es descriptiva, no prescriptiva.
Lee primero docs/STATE.md (si existe) y docs/HANDOFF_REVIEW.md. No repitas su contenido, referencia por ruta.
Tokens: nada de volcados completos; evidencia máx. 10 líneas; JSON crudo a tmp/fm/.
Presupuesto de navegación: máx. 10 para toda esta fase (todo lo demás debe salir de datos ya guardados en fm_signal/fm_snapshot/fm_outcome).
Fases con puerta: ejecuta SOLO Parte A y Parte B. Al terminar, reporta y PARA.

## PARTE A — Cierre de cola P2 (prioridad, hazlo primero)
A1. UNAVAILABLE reintentable: modifica el resolver para que filas en status='UNAVAILABLE' puedan re-resolverse (no usar INSERT OR IGNORE para ese estado; sí mantenerlo para RESOLVED/NOT_PLAYED/AMBIGUOUS, que son finales).
A2. Diferenciar motivo dentro de UNAVAILABLE: agrega columna `unavailable_reason` con valores `NO_HISTORY_ELEMENT` (no está en history[] aún) vs `NO_DATE_MATCH` (hay elementos pero ninguno coincide con kickoffUtc±1 día) vs `OTHER`. Esto ayuda a distinguir lag de ingesta real vs bug de matching por fecha/rival (sin fixtureId en history[], ver V2).
A3. Ejecuta un re-resolve sobre los mismos fixtures del run3 (ver docs/HANDOFF_REVIEW.md §5) y reporta el delta: cuántas UNAVAILABLE pasaron a RESOLVED, y el desglose por unavailable_reason de lo que sigue sin resolver.
A4. Agrega `source_conflict` (bool) a fm_outcome: márcalo cuando dos campos relacionados de FM se contradicen (ej. home_saves=0 con goles del rival >0, o shots_on_target > shots). Documenta los casos detectados, no los corrijas ni los excluyas.
A5. Cierra A3 pendiente del handoff: confirma /navigate e /intercept → 403/404 en Production; restaura Pipeline:LegacyWorker:Enabled=true como default de despliegue (documenta el toggle usado en desarrollo).
A6. Escribe/actualiza docs/STATE.md (≤30 líneas): qué existe, endpoints, esquema (incluye columnas nuevas de esta fase), decisiones clave, pendientes reales.

## PARTE B — Ventanas propias y confluencia (determinista, sin LLM, sin cuotas)
Contexto: V3 confirmó que bestCount/bestTotal de FM es una ventana ya optimizada por hit rate (selection bias). MatchEdge debe calcular SUS PROPIAS ventanas fijas desde history[] crudo, ignorando bestCount/bestTotal para este cálculo (bestCount/bestTotal se conserva solo como dato de referencia/auditoría, no como insumo).

B1. FmWindowCalculator: para cada fm_signal, desde su history[] guardado, calcula ventanas FIJAS de tamaño 5, 10 y "todo el history disponible" (documenta la longitud máxima real observada; V2 no la confirmó). Para cada ventana: hits, n, observed_rate, media, mediana, min, max. Si n de la ventana < mínimo (team=4, player=3, mismo umbral que V3), márcala como INSUFFICIENT_SAMPLE en vez de calcularla.
B2. No calcules "probabilidad". Guarda solo hits/n/estadísticos descriptivos, igual que ya se hace en fm_signal (regla ya establecida: nunca 8/10 → 0.80 tratado como probability).
B3. FmConfluenceBuilder: para un fixture, agrupa por mercado+línea las señales de: equipo (ataque), rival (Opp. Hits / lado defensivo del mismo mercado), y si aplica, señales de jugadores relevantes. Construye un objeto de "composición" que muestre las piezas por separado (NO las sumes en un score único). Marca explícitamente cuando dos piezas provienen del mismo subconjunto de partidos (ej. si el team-window de 5 y el H2H comparten >50% de los mismos partidos) con overlap_flag=true, para que no se cuenten como evidencia independiente.
B4. Contexto: agrega campos ya disponibles sin nueva navegación: venue (del params_json existente), competition_scope, y si hay leakage_flag o source_conflict heredados del snapshot/outcome. NO investigues lesiones/alineaciones en esta fase (requeriría fuente nueva, fuera de alcance).
B5. Endpoint GET /api/fm/fixtures/{id}/confluence → devuelve el JSON de B1+B3+B4 para un fixture. Solo lectura de datos ya guardados, 0 navegaciones nuevas.

## ACEPTACIÓN (solo PASS/FAIL + 1 línea)
- C1: A1–A3 — re-resolve ejecutado, delta reportado, unavailable_reason poblado en el 100% de filas UNAVAILABLE.
- C2: A4 — al menos los casos de contradicción ya conocidos (home_saves/goles) quedan marcados.
- C3: A5 — /navigate e /intercept devuelven no autorizado en Production.
- C4: B1 — para 10 señales de muestra, ventanas 5/10 calculadas coinciden con recuento manual desde history[] (verificación cruzada, no confiar en bestCount).
- C5: B3 — al menos 1 caso real de overlap_flag=true documentado con evidencia.
- C6: B5 — endpoint responde para 2 fixtures reales sin navegar.

## CIERRE
Actualiza docs/STATE.md. Reporte final ≤25 líneas: A1–A6 y B1–B5 en una línea c/u, delta de UNAVAILABLE, riesgos (máx. 5), y una pregunta explícita para mí: "¿confirmas que la fuente de cuotas será manual (no FM) antes de construir el reporte final con comparación de mercado?". Luego PARA.