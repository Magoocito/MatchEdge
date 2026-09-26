# Learning Log - MatchEdge + Udemy

> Objetivo: Trazar cada módulo de Udemy con un PBI de GitHub. Evidencia de que aprendiste y luego aplicaste, no que la IA hizo todo.

| Fecha | Módulo Udemy | PBI | PR | Aprendizaje con mis palabras |
|---|---|---|---|---|
| 2026-09-26 | .NET Backend Bootcamp - Windows Service & DI Lifetimes | PBI 1.1 | - | Singleton en BrowserManager deja Chrome abierto = leak. Scoped + IAsyncDisposable |
| 2026-09-26 | .NET Backend Bootcamp - Outbox & Idempotency | PBI 1.2 | - | Re-key (subject_type, subject_name, market, line) evita duplicar total_corners |
| 2026-09-26 | .NET Backend Bootcamp - External Integration + HttpClient | PBI 2.1 | - | Interceptar XHR teams/table y position-stats, guardar raw en tmp/fm/, documentar endpoint |
| - | Modular Monolith - DDD Parser / Vertical Slice | PBI 2.2 | - | Mapear tackles 40, foul_involvements 30, fouls_drawn 26, fouls_committed 22, saves 19 a stats_json |
| - | Modular Monolith - Deduplication | PBI 2.3 | - | total_* una sola entrada por (market,line) |

## Notas P8 - Parte A completada
- tmp/fm/p8_a_unmapped.json: 0 unmapped, 147 missing/non-numeric player, 38 not_reproducible total_* (total_corners 24, total_shots 7, total_shots_on_target 5, total_offsides 1, total_tackles 1) todos INSUFFICIENT_SAMPLE
- Player: tackles 40, foul_involvements 30, fouls_drawn 26, fouls_committed 22, goalkeeper_saves 19, shots 4, sot 3
- P4-P7 respetados: solo 1/odds descriptivo, sin Betano/EV, navegación secuencial 2-4s
