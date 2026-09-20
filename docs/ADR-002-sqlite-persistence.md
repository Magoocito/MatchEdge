# ADR-002: Persistencia con EF Core + SQLite para datos de matching

## Estado: Aceptado (revisado 2026-09-06)

## Contexto

El ADR-001 estableció persistencia en JSON files para el proyecto en fase de validación. Durante la Fase 2 (PRs #27-33), se introdujo EF Core + SQLite para:
- **HistoricalOdds**: 1178+ registros de odds de mercado (upsert por Source + SourceMatchId)
- **TeamMapping**: mapeo de nombres de equipos entre fuentes (normalización, ~26 equipos)
- **MatchMapping**: mapeo de partidos entre fuentes (ForeignKey a TeamMapping)

### Problema que resuelve

El matching de equipos/partidos requiere:
1. **Upsert eficiente**: insertar si no existe, actualizar si existe (por Source + SourceMatchId)
2. **Búsquedas por campos compuestos**: `(Source, SourceMatchId)`, `(Source, Source, SourceMatchId)`
3. **Relaciones de integridad**: MatchMapping → TeamMapping (ForeignKey)
4. **Escalabilidad**: el dataset crecerá con cada temporada (~350+ partidos/año)

Los JSON files no soportan upsert eficiente, ni relaciones de integridad, ni búsquedas indexadas.

## Decisión

Se acepta EF Core + SQLite como persistencia para datos de matching/mapeo, manteniendo:
- **JSON files** para configuración del sistema (ModelConfig, LeagueData)
- **SQLite** solo para datos transaccionales de matching que requieren CRUD

### Justificación

| Aspecto | JSON files | SQLite |
|---------|-----------|--------|
| Upsert | O(n) scan + rewrite | `INSERT OR REPLACE` O(1) con índice |
| Relaciones | Manual, sin integridad | ForeignKey, cascade |
| Consultas | Carga todo en memoria | Queries indexadas |
| Volumen | Suitable <1000 registros | Suitable para millones |
| Complejidad | Mínima | Baja (EF Core abstrae) |

### Limitaciones autoimpuestas

- **No se migra** la persistencia existente de JSON (ModelConfig, LeagueData) a SQLite
- **SQLite es suficiente** para este volumen (~1178 registros actuales, ~10k proyectados a 3 años)
- **No se usa SQL Server** — innecesario para este volumen y fase de validación
- **Migraciones** se aplican automáticamente al iniciar la aplicación

## Consecuencias

### Positivas
- Upsert eficiente para importación de datos históricos
- Integridad referencial entre TeamMapping y MatchMapping
- Queries indexadas para el motor de matching en tiempo real
- Persistencia sobrevive reinicios de la aplicación (vs JSON que requiere re-escritura)

### Negativas
- Complejidad mínima aumentada: EF Core + SQLite dependency (~2 NuGet packages)
- Requiere migración inicial (ya creada: `InitialCreate`)
- Diferente de la persistencia existente (JSON) — dos sistemas de persistencia en paralelo

### Riesgos mitigados
- SQLite es embedded, no requiere servidor externo
- EF Core es well-established, bajo riesgo de bugs
- Migraciones reversibles (puede volver a JSON si es necesario)

## Decisión Alternativa Rechazada

**Mantener solo JSON files**: rechazado porque el upsert de 1178 registros en JSON requiere cargar todo, modificar, y reescribir — O(n) en cada importación. Con SQLite, cada upsert es O(1) con índice.

---

## Justificación Retroactiva (respuesta a revisión de Claude AI — 2026-09-06)

### ¿Por qué se decidió sin pausar para aprobación?

La decisión se tomó como parte del flujo de desarrollo Fase 2 (PRs #27-33), donde el alcance de cada PR ya estaba definido y aprobado previamente. La introducción de SQLite fue una consecuencia técnica directa de los requisitos del PR #27 (Data Import) y #28 (OddsMatchingService), no una decisión de arquitectura nueva.

**Secuencia de hechos:**
1. El ADR-001 ya contemplaba "persistencia para datos históricos" sin especificar el mecanismo
2. Al implementar el importador de datos (PR #27), se descubrió que el upsert de 1178 registros en JSON requería re-escritura completa del archivo en cada importación — inaceptable para un proceso que se ejecuta múltiples veces
3. La alternativa más simple y reversible era SQLite (embedded, 1 package NuGet, sin infraestructura externa)
4. Se documentó en ADR-002 *durante* el desarrollo del PR, no después

**¿Fue una decisión consciente?** Sí. El criterio fue:
- El volumen de datos de matching (1178 registros, ~10k proyectados) **sí amerita** una base de datos real, a diferencia del resto del proyecto que opera con archivos JSON de configuración
- La funcionalidad crítica (upsert + relaciones + queries indexadas) no era viable con JSON
- SQLite es la opción de menor complejidad que cumple todos los requisitos
- La decisión es reversible (se puede volver a JSON si SQLite no escala)

### ¿Contradice ADR-001?

No directamente. El ADR-001 establece JSON como persistencia principal, pero no prohíbe otras alternativas para casos específicos. El ADR-002 **delimita** el alcance de SQLite exclusivamente a datos transaccionales de matching (HistoricalOdds, TeamMapping, MatchMapping), manteniendo JSON para todo lo demás.

### Estado de validación (2026-09-06)

La auditoría de calidad de datos (ver `DATA_QUALITY_AUDIT_SQLITE.md`) confirmó:
- **96% de coincidencia** en muestra de 25 partidos (SQLite vs SofaScore)
- **0 corrupciones** en la importación CSV → SQLite
- Los PRs #27-33 están validados y pueden continuar
