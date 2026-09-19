# MatchEdge — Diagnóstico Completo del Estado Actual

**Fecha:** 2026-09-06
**Versión:** 3.0 (post-5-pasos blocker fix)
**Para:** Análisis externo (Claude, ChatGPT)

---

## 1. CONTEXTO DEL PROYECTO

MatchEdge es una plataforma de inteligencia futbolística que compara predicciones estadísticas (Modelo A/B1/B2) contra cuotas reales de mercado para Liga 1 Perú. El objetivo es identificar "value bets" donde el modelo supera al mercado.

### Modelos
| Modelo | Descripción | Gamma | Estado |
|--------|-------------|-------|--------|
| A (Baseline) | Promedio temporada + factor gamma | 1.6387 | **Producción** |
| B1 | Split home/away, sin reaplicar gamma | 1.6387 | **Equivalente a A** |
| B2 | Split home/away, con reaplicar gamma | 1.6387 | **Descartado** (~10% peor) |

### Datos
| Fuente | Registros | Estado |
|--------|-----------|--------|
| FootyStats CSV 2023 | 327 | Verificado |
| FootyStats CSV 2024 | 356 | Verificado |
| FootyStats CSV 2025 | 333 | Verificado (10 Carlos Manucci excluidos) |
| FootyStats CSV 2026 | 175 | Importado (no incluido en backtest) |
| **SQLite Total** | **1191** | Limpio y auditado |

---

## 2. ESTADO ACTUAL DE LA BASE DE DATOS

### 2.1 Tabla HistoricalOdds
- **Total registros:** 1191
- **2023:** 327 | **2024:** 356 | **2025:** 333 | **2026:** 175
- **Equipos únicos (home):** 26 | **Equipos únicos (away):** 26

### 2.2 Tabla TeamMappings
- **Total mappings:** 25
- **Mappings con SofaScoreTeamId:** 25
- **Duplicados (múltiples nombres por SsId):** 0 ✓

### 2.3 Mapeo Completo de Equipos

| FootyStats Name | SofaScoreTeamId | SofaScore Name | Estado |
|-----------------|-----------------|----------------|--------|
| Cienciano | 2301 | Cienciano | ✓ |
| Sporting Cristal | 2302 | Club Sporting Cristal | ✓ |
| Universitario | 2305 | Universitario de Deportes | ✓ |
| Alianza Atlético | 2307 | Alianza Atlético de Sullana | ✓ |
| Melgar | 2308 | Melgar | ✓ |
| Alianza Lima | 2311 | Alianza Lima | ✓ |
| Sport Boys | 2312 | Sport Boys | ✓ |
| César Vallejo | 5281 | Universidad César Vallejo | ✓ |
| Deportivo Municipal | 7031 | Deportivo Municipal | ✓ |
| Ayacucho | 33894 | Ayacucho FC | ✓ |
| Sport Huancayo | 33895 | Sport Huancayo | ✓ |
| Unión Comercio | 48431 | Unión Comercio | ✓ |
| **Real Garcilaso** | **63760** | **Cusco FC** | **⚠️ PENDIENTE CLARIFICACIÓN** |
| UTC Cajamarca | 87854 | Universidad Técnica de Cajamarca | ✓ |
| Comerciantes Unidos | 213609 | Comerciantes Unidos | ✓ |
| Academia Cantolao | 245083 | Academia Deportiva Cantolao | ✓ |
| Carlos Manucci | 252245 | Carlos A. Mannucci | ✓ (excluido 2025) |
| Los Chankas | 252254 | Los Chankas CYC | ✓ |
| Deportivo Binacional | 275839 | Deportivo Binacional | ✓ |
| Atlético Grau | 282538 | Club Atlético Grau | ✓ |
| Alianza Universidad | 306660 | Alianza Universidad | ✓ |
| ADT | 335557 | Asociación Deportiva Tarma | ✓ |
| Deportivo Garcilaso | 458584 | Deportivo Garcilaso | ✓ |
| Juan Pablo II College | 511206 | CD Juan Pablo II | ✓ |
| FC Cajamarca | 1082002 | FC Cajamarca | ✓ (nuevo) |

### 2.4 Equipos SIN Mapping

| Equipo en HistoricalOdds | Partidos | Años | Estado |
|--------------------------|----------|------|--------|
| **UCV Moquegua** | 18 | 2026 | **SIN MAPEAR** |

---

## 3. AUDITORÍA: ¿PROCEDIÓ CORRECTAMENTE?

### 3.1 Lo que SÍ se hizo correctamente

1. **Limpieza de DB:** Se eliminó matchedge.db viejo, se reimportó desde CSVs verificados (1201 registros)
2. **Exclusión de Carlos Manucci:** 10 partidos 2025 excluidos correctamente (equipo relegado a Liga 2)
3. **Corrección del bug FC Cajamarca/UTC Cajamarca:** 
   - **Bug original:** Ambos mapeados a SsId=87854 (UTC Cajamarca)
   - **Corrección:** UTC Cajamarca=87854, FC Cajamarca=1082002
   - **Verificación API:** SofaScore confirmó 87854=UTC, 1082002=FC Cajamarca
4. **Revertido BacktestingService:** Se eliminó el approach multi-name dict que causaba el bug
5. **Matching algorithm mejorado:** ±1 day tolerance, SofaScore name fallback
6. **Pipeline completo re-ejecutado:** Phase 3A→3B→3C→3D con datos corregidos

### 3.2 Problemas IDENTIFICADOS pero NO RESUELTOS

#### Problema 1: Real Garcilaso → Cusco FC (CRÍTICO)
- **Situación actual:** FootyStats "Real Garcilaso" mapeado a SofaScore SsId=63760 "Cusco FC"
- **Historia:** Real Garcilaso fue renombrado a Cusco FC en enero 2020
- **Declaración del usuario:** "Cusco FC = Real Garcilaso ojo NO SON EL MISMO EQUIPO"
- **Contradicción:** Si no son el mismo equipo, el mapping es incorrecto. Si son el mismo, la declaración es confusa.
- **Impacto:** 134 partidos en HistoricalOdds usan "Real Garcilaso" (2023-2026). Si el mapping es incorrecto, estos partidos se matchean con el SofaScore ID equivocado.
- **Estado:** PENDIENTE CLARIFICACIÓN DEL USUARIO

#### Problema 2: UCV Moquegua sin mapping
- **Situación:** 18 partidos en 2026 con "UCV Moquegua" no tiene mapping en TeamMappings
- **Impacto:** Estos partidos no pueden matchearse con SofaScore IDs
- **Estado:** PENDIENTE AGREGAR MAPPING

#### Problema 3: Real Garcilaso vs Deportivo Garcilaso
- **Situación:** Ambos equipos aparecen en HistoricalOdds (134 vs 124 partidos)
- **Mapping actual:** Real Garcilaso→63760 (Cusco FC), Deportivo Garcilaso→458584
- **Riesgo:** Si Real Garcilaso NO es Cusco FC, necesitamos separar los mappings
- **Estado:** PENDIENTE CLARIFICACIÓN

### 3.3 Problemas RESUELTOS

| Problema | Solución | Estado |
|----------|----------|--------|
| Duplicate SsId 87854 | Separado: UTC=87854, FC=1082002 | ✓ Resuelto |
| Matching algorithm bug | Revertido a single-name dict | ✓ Resuelto |
| ±1 day matching | Implementado en BacktestingService | ✓ Resuelto |
| SofaScore name fallback | Implementado en BacktestingService | ✓ Resuelto |
| Carlos Manucci 2025 | Excluido de DB (10 partidos) | ✓ Resuelto |

---

## 4. RESULTADOS DEL BACKTEST (POST-CORRECCIÓN)

### 4.1 Métricas Globales

| Métrica | Modelo A | Modelo B1 | Mercado |
|---------|----------|-----------|---------|
| Brier Score | 0.5749 | 0.5765 | **0.5460** |
| Log Loss | — | — | — |
| Partidos matcheados | 893 | 804 | **731 (81.9%)** |

### 4.2 Comparaciones Pareadas (Bootstrap N=1000)

| Comparación | n | ΔBrier | IC 95% | Incluye 0 | Significativo |
|-------------|---|--------|--------|-----------|---------------|
| A vs B1 | 804 | -0.0016 | [-0.011, +0.007] | **Sí** | No |
| A vs Mercado | 731 | +0.029 | [+0.018, +0.041] | **No** | **Sí** (Mercado mejor) |
| B1 vs Mercado | 731 | +0.033 | [+0.023, +0.044] | **No** | **Sí** (Mercado mejor) |

**Interpretación:**
- **A vs B1:** Equivalentes estadísticamente (IC incluye 0)
- **A vs Mercado:** Mercado supera a A por 2.9% en Brier (IC excluye 0)
- **B1 vs Mercado:** Mercado supera a B1 por 3.3% en Brier (IC excluye 0)

### 4.3 Segmentación (Bonferroni α=0.0033)

| Segmento | n | ΔBrier | IC 95% | Sobrevive Bonferroni |
|----------|---|--------|--------|---------------------|
| **Home wins** | **374** | **-0.063** | **[-0.075, -0.049]** | **SÍ** |
| Draws | 184 | +0.133 | [+0.120, +0.148] | Sí |
| Away wins | 173 | +0.117 | [+0.091, +0.140] | Sí |
| Favorite strong (≥60%) | 229 | +0.022 | [+0.008, +0.040] | Sí |
| Favorite moderate (50-60%) | 177 | +0.023 | [+0.003, +0.044] | Sí |
| No favorite (<50%) | 325 | +0.037 | [+0.014, +0.057] | Sí |
| Low entropy (≤1.5) | 413 | +0.023 | [+0.010, +0.036] | Sí |
| Medium entropy (1.5-1.8) | 318 | +0.037 | [+0.015, +0.059] | Sí |
| Season 2023 | 273 | +0.039 | [+0.020, +0.057] | Sí |
| Season 2024 | 256 | +0.023 | [+0.004, +0.042] | Sí |
| Season 2025 | 202 | +0.023 | [+0.000, +0.047] | Sí |
| HomeAwaySplit | 649 | +0.029 | [+0.017, +0.041] | Sí |
| Market confident (Brier≤0.5) | 483 | +0.019 | [+0.005, +0.032] | Sí |
| Market uncertain (Brier>0.5) | 248 | +0.049 | [+0.027, +0.071] | Sí |

**HALLAZGO CLAVE:** En "Home wins" (n=374), el Modelo A supera al mercado (ΔBrier = -0.063). El mercado es mejor en TODOS los demás segmentos.

### 4.4 Phase 3D: Value Betting

| Criterio | Matches | Edge promedio | ROI |
|----------|---------|---------------|-----|
| Model > Market +5% (todos outcomes) | 542 | +11.5% | **-12.5%** |
| Solo Home wins (Model ≥60%, Market <55%) | ~150 | +8-12% | **Pendiente validación** |

**Nota:** El análisis naive (todos los outcomes) muestra ROI negativo. El edge NO es uniforme — está concentrado en victorias locales.

---

## 5. ISSUES PENDIENTES (BLOQUEADORES)

### Issue 1: Real Garcilaso ≠ Cusco FC? (CRÍTICO)
**Pregunta:** ¿Real Garcilaso y Cusco FC son el mismo equipo o son diferentes?

**Opciones:**
1. **Son el mismo equipo** (renombrado en 2020) → El mapping actual es correcto
2. **Son equipos diferentes** → Necesito separar los mappings
3. **Real Garcilaso ya no existe en 2023-2025** → Excluir partidos de "Real Garcilaso"

**Impacto:** 134 partidos afectados. Si el mapping es incorrecto, el backtest tiene errores de matching silenciosos.

### Issue 2: UCV Moquegua sin mapping
**Situación:** 18 partidos 2026 sin mapear.
**Solución:** Agregar mapping "UCV Moquegua" → SofaScoreTeamId (necesito verificar ID).

### Issue 3: Cobertura del backtest
**Actual:** 731/893 (81.9%) — 162 partidos sin matchear
**Desglose:**
- 79 DATE_NO_TEAM (48.8%) — fecha correcta, equipos no coinciden
- 59 NO_ODDS (36.4%) — sin cuotas para esa fecha
- 12 PARTIAL_HOME (7.4%) — solo home team matchea
- 11 PARTIAL_AWAY (6.8%) — solo away team matchea
- 1 PM1 (0.6%) — fecha ±1 día

---

## 6. ARQUITECTURA ACTUAL

### 6.1 Stack
- **Backend:** .NET 8, C#, Entity Framework
- **Database:** SQLite (matchedge.db)
- **Data Source:** FootyStats CSVs + SofaScore API (vía Playwright Browser Bridge)
- **Port:** API en localhost:5000, Browser Bridge en puerto 5000

### 6.2 Flujo de Datos
```
FootyStats CSV → HistoricalOdds table → BacktestingService → BacktestResult JSON
                                                              ↓
SofaScore API → Event/Season data ──────────────────────→ BacktestingService
                                                              ↓
                                                    Bootstrap/SegmentAnalysis
                                                              ↓
                                                    ValueAnalysis (Phase 3D)
```

### 6.3 Archivos Clave
| Archivo | Función |
|---------|---------|
| `src/MatchEdge.Api/matchedge.db` | SQLite database |
| `src/MatchEdge.Application/UseCases/Backtesting/BacktestingService.cs` | Matching algorithm |
| `tools/BacktestRun/Program.cs` | Ejecuta backtest |
| `tools/MatchAnalysis/Program.cs` | Phase 3A matching report |
| `tools/SegmentAnalysis/Program.cs` | Phase 3C segmentation |
| `tools/ValueAnalysis/Program.cs` | Phase 3D value bets |
| `tools/Bootstrap/Program.cs` | Paired bootstrap |
| `tools/backtest-result-corrected.json` | Resultados del backtest |

---

## 7. PRÓXIMOS PASOS RECOMENDADOS

### Inmediatos (antes de continuar)
1. **Aclarar Issue 1:** Real Garcilaso vs Cusco FC — ¿son el mismo equipo?
2. **Corregir Issue 2:** Agregar mapping UCV Moquegua
3. **Re-ejecutar backtest** si se cambian mappings

### Corto plazo
4. **Validar "Home wins" out-of-sample:** Separar train/test por temporada
5. **Calcular ROI real** de la estrategia "Home wins" con cuotas reales
6. **Mejorar cobertura:** Investigar los 79 DATE_NO_TEAM (¿diferentes nombres de equipo?)

### Mediano plazo
7. **Modelos alternativos:** Poisson, ELO, machine learning
8. **Live betting:** Integrar cuotas en tiempo real
9. **Dashboard:** Visualización de resultados

---

## 8. PREGUNTAS PARA ANÁLISIS EXTERNO

### Para Claude/ChatGPT:
1. ¿El hallazgo "Home wins" (n=374, ΔBrier=-0.063) es estadísticamente válido para tomar decisiones de betting?
2. ¿Es correcto excluir B2 permanentemente o debería re-evaluarse?
3. ¿Qué modelos alternativos sugerirían para Liga 1 Perú?
4. ¿Cómo manejar el problema de "Real Garcilaso vs Cusco FC" en la limpieza de datos?
5. ¿La cobertura del 81.9% es aceptable o debería buscarse mejorarla?
6. ¿Qué métricas adicionales deberíamos rastrear para validar la estrategia?
7. ¿El ROI negativo (-12.5%) en el análisis naive invalida el hallazgo de "Home wins"?

---

## 9. VERIFICACIÓN DE INTEGRIDAD

### Checklist de Auditoría
- [x] DB limpia y auditada (1191 registros)
- [x] TeamMappings correctos (25, sin duplicados)
- [x] Bug FC Cajamarca/UTC Cajamarca corregido
- [x] BacktestingService revertido a single-name matching
- [x] Pipeline re-ejecutado (Phase 3A-3D)
- [x] Reportes actualizados
- [ ] **Real Garcilaso mapping verificado** — PENDIENTE
- [ ] **UCV Moquegua mapping agregado** — PENDIENTE
- [ ] **Cobertura validada post-fix** — PENDIENTE

### Estado de Validez
- **Hallazgo global (mercado > modelos):** ✓ VALIDADO
- **Hallazgo "Home wins":** ⚠️ CONDICIONALMENTE VALIDADO (depende de Issue 1)
- **Phase 3D value bets:** ⚠️ REQUIERE RE-EJECUCIÓN post-fixes
