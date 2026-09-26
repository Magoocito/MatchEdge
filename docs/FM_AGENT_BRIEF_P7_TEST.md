# FM_AGENT_BRIEF_P7_TEST — Validación práctica del reporte de confluencia (P7)

## ALCANCE
Rigen reglas vigentes (docs/STATE.md, docs/VISION.md, prohibiciones de P4-P7: sin scraping Betano, sin EV/probabilidad desde hit rate, sin recomendaciones). Solo lectura y validación, sin cambios de arquitectura salvo bugs que aparezcan. PARA al terminar.

## T1 — Cobertura ampliada
Ejecuta GET /report y /report.md sobre TODOS los fixtures con datos completos ya persistidos (no solo 33441811), incluyendo al menos 1 fixture de liga doméstica (de los usados en H de P6, ej. Exeter City / Granada) además de los de Nations League.
Para cada uno, documenta en tmp/fm/p7test_coverage.json: cuántos mercados salieron OK vs INSUFFICIENT_SAMPLE/NO_DATA, y si algún campo quedó nulo/vacío sin explicación.

## T2 — Verificación cruzada adicional (más allá de los 2 mercados ya hechos en F1)
Elige 3 mercados NUEVOS (no los ya verificados en P7) de fixtures distintos, mezcla equipo/jugador. Recuenta a mano desde fm_team_matches/fm_player_matches y compara contra own_windows del reporte. Documenta en tmp/fm/p7test_crosscheck.json.

## T3 — Casos límite
- Un fixture sin ninguna cuota cargada (ni FM ni manual) → confirma que manual_odds y market_odds_fm se muestran como vacío/NO_DATA, no se omiten.
- Un fixture con source_conflict=true conocido (33441811 lo tiene) → confirma que data_quality lo refleja en el reporte.
- Una carga manual de cuota inválida (negativa, texto, 0) vía PUT → confirma rechazo con error claro, no un 200 silencioso.

## T4 — Estabilidad
Corre GET /report tres veces seguidas sobre el mismo fixture sin cambios de datos entre medio. Confirma que el JSON es byte-idéntico (aparte de timestamps de captura si los hay) y el .md también.

## T5 — Prueba de lectura humana
Genera el .md de 2 fixtures distintos y pégalos completos en el reporte final (sin resumir) para que el usuario los lea directamente y confirme que el formato es entendible y útil en la práctica.

## REPORTE FINAL (≤20 líneas + los 2 .md completos de T5 como anexo)
T1-T4 en una línea c/u con hallazgos. Bugs encontrados (si hay) y si se corrigieron o quedaron documentados. Los 2 .md completos de T5 al final, sin editar. PARA.