# RESUMEN RÁPIDO - FLUJO DE ANÁLISIS

## SIEMPRE SIGUE ESTE ORDEN:

### 1. BUSCAR PARTIDOS (1 min)
- Hora actual → Próximas 2 horas (mín 15 min antes)
- Si no hay nada, buscar siguientes 2 horas

### 2. OBTENER PREDICCIONES (2 min)
- URL: `/predictions`
- Criterios: Prob ≥ 80%, Odds ≥ 1.20
- Guardar top 5 partidos

### 3. ANÁLISIS POR PARTIDO (5 min cada uno)

**Para CADA partido del top 5:**

#### 3.1 Overview Tab
- URL: `/fixtures/{id}-{slug}`
- Buscar: Alineaciones, Forma, H2H, Estadísticas

#### 3.2 Team Trends Tab
- URL: `/fixtures/{id}-{slug}?tab=team-trends`
- Buscar: Goals, BTTS, Corners, Cards
- Verificar: Hit rate ≥ 80%, Opp. hits, H2H

#### 3.3 Player Trends Tab
- URL: `/fixtures/{id}-{slug}?tab=player-trends`
- Buscar: Shots, Goals, Assists, Cards
- Verificar: Hit rate ≥ 80%, Odds ≥ 1.20

### 4. PRESENTAR RESULTADOS (siempre usar plantilla)

```
## [PARTIDO]
**[HORA] | [LIGA]**

### OVERVIEW
[Tabla de alineaciones, forma, H2H]

### TEAM TRENDS
[Tabla de trends encontrados]

### PLAYER TRENDS
[Tabla de jugadores con trends]

### PREDICCIÓN FM
[Tabla de predicciones]

### TOP 5 OPCIONES
[Tabla de picks]

### RECOMENDACIÓN
[Pick principal + razón]
```

### 5. DECISIÓN
- Presentar opciones
- Esperar confirmación del usuario
- Registrar si se aprueba

---

## SCRIPTS DISPONIBLES:

- `scripts/analyze_match.ps1` - Análisis automatizado
- `ANALYSIS_WORKFLOW.md` - Flujo detallado
- `templates/match_analysis.md` - Plantilla de presentación

---

## EJEMPLO RÁPIDO:

Si encuentro: `https://www.footymetrics.com/fixtures/32833598-superliga-petrolul-52-csikszereda`

Hago:
1. Overview → Alineaciones, Forma, H2H
2. Team Trends → Buscar trends de Goals, BTTS, etc.
3. Player Trends → Buscar jugadores con trends fuertes
4. Presentar con plantilla
5. Dar recomendación final
