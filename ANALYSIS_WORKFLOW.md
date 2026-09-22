# FLUJO DE ANÁLISIS DE APUESTAS - MatchEdge

## OBJETIVO
Encontrar y presentar las mejores oportunidades de apuesta de forma consistente.

## FLUJO PASO A PASO

### PASO 1: HORA Y RANGO DE BÚSQUEDA
- Verificar hora actual
- Definir rango: próximas 2 horas, mínimo 15-30 min antes del partido
- Si no hay partidos en el rango, buscar siguientes 2 horas

### PASO 2: OBTENER PREDICCIONES FM
- Scraping de `/predictios`
- Filtrar por:
  - Probabilidad ≥ 80%
  - Odds ≥ 1.20
  - Dentro del rango horario

### PASO 3: SELECCIONAR TOP 5 PARTIDOS
- Para cada partido que cumpla criterios:
  1. Buscar fixture específico en FM
  2. Navegar a Overview tab
  3. Navegar a Team Trends tab
  4. Navegar a Player Trends tab
  5. Buscar trends de Goals, BTTS, Corners, Cards

### PASO 4: ANÁLISIS PROFUNDO POR PARTIDO
Para CADA partido del Top 5:

#### 4.1 OVERVIEW
- Alineaciones confirmadas
- Forma reciente (últimos 5)
- H2H (partidos previos)
- Estadísticas del partido (posesión, tiros, córners)

#### 4.2 TEAM TRENDS
- Buscar trends de Goals (Over/Under)
- Buscar trends de BTTS
- Buscar trends de Corners
- Buscar trends de Cards
- Verificar: Hit rate, Opp. hits, H2H

#### 4.3 PLAYER TRENDS
- Buscar jugadores con trends fuertes (≥ 80% hit rate)
- Verificar si son titulares
- Buscar: Shots, Goals, Assists, Cards

#### 4.4 PREDICCIÓN FM
- Verificar probabilidad
- Verificar odds
- Verificar tendencia (+/- %)

### PASO 5: CRITERIOS DE SELECCIÓN

#### Para MARKET PICKS (Goals, BTTS, etc.):
- Probabilidad ≥ 80%
- Odds ≥ 1.20
- Trend de equipo con ≥ 80% hit rate
- H2H favorable
- Forma del equipo favorable

#### Para PLAYER PICKS (Shots, Goals, etc.):
- Hit rate ≥ 80% (mínimo 4/5 partidos)
- Odds ≥ 1.20
- Jugador confirmado como titular
- Promedio ≥ 1.0 por partido

### PASO 6: PRESENTACIÓN DE RESULTADOS

Para CADA partido del Top 5:

```
## [NOMBRE DEL PARTIDO]
**[HORA] | [LIGA] | [FECHA]**

### OVERVIEW
| Dato | [Equipo Local] | [Equipo Visitante] |
|------|----------------|---------------------|
| Posición | X | X |
| Forma | W-L-W | L-W-D |
| Predicción FM | X-X | |
| Probabilidades | X% | X% |

**H2H:**
- X partidos previos
- Promedio X goles
- X/X ambos equipos anotaron

### TEAM TRENDS
| Trend | Equipo | Hit Rate | Opp. hits | H2H |
|-------|--------|----------|-----------|-----|
| [Trend] | [X/X] | [X%] | [X/X] | [X/X] |

### PLAYER TRENDS
| Jugador | Equipo | Pos | Trend | Hit Rate | Odds |
|---------|--------|-----|-------|----------|------|
| [Nombre] | [Equipo] | [Pos] | [Trend] | [X/X] | [X.XX] |

### PREDICCIÓN FM
| Mercado | Probabilidad | Odds | ¿Cumple? |
|---------|--------------|------|----------|
| [Mercado] | [X%] | [X.XX] | ✅/❌ |

### TOP 5 OPCIONES
| # | Pick | Odds | Confianza |
|---|------|------|-----------|
| 1 | [Pick principal] | [X.XX] | X/10 |
| 2 | [Pick secundario] | [X.XX] | X/10 |
| 3 | [Pick adicional] | [X.XX] | X/10 |
| 4 | [Pick adicional] | [X.XX] | X/10 |
| 5 | [Pick adicional] | [X.XX] | X/10 |

### RECOMENDACIÓN
**Pick principal:** [Descripción]
- Confianza: X/10
- Razón: [Explicación]

**Stake sugerido:** S/X.00
```

### PASO 7: DECISIÓN FINAL
- Presentar opciones al usuario
- Esperar confirmación
- Registrar apuesta si se aprueba

## SCRIPT DE AUTOMATIZACIÓN

Ver: `scripts/analyze_match.ps1`

## PLANTILLA DE PRESENTACIÓN

Ver: `templates/match_analysis.md`

## HISTORIAL DE CAMBIOS

- 2026-09-21: Creación del flujo inicial
- Próximas mejoras: Agregar更多 fuentes de datos
