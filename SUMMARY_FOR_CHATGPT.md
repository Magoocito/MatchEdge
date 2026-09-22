# 📊 RESUMEN COMPLETO - PROYECTO DE APUESTAS DEPORTIVAS
## Para trabajar con ChatGPT / AI Assistant

---

## 🎯 OBJETIVO GENERAL
Crear un sistema de apuestas deportivas **consistente y rentable** usando:
- FootyMetrics (datos Opta-level)
- Análisis estadístico riguroso
- Gestión de banca disciplinada

---

## 📁 ESTRUCTURA ACTUAL DE PROYECTOS

### Proyecto 1: MatchEdge (Liga Peruana)
```
C:\Dev\Proyectos\MatchEdge\
├── src/                    # API .NET para datos de SofaScore
├── tools/                  # Herramientas de análisis
│   ├── ValueAnalysis/      # Análisis de value bets
│   ├── SegmentAnalysis/    # Segmentación por tipo de apuesta
│   ├── Backtest/           # Backtesting de modelos
│   └── ...                 # Otras herramientas
├── diagnostic_*.md         # Diagnósticos de apuestas
└── betting_history_*.md    # Historial de apuestas
```

### Proyecto 2: Sports-Betting (Referencia)
```
https://github.com/georgedouzas/sports-betting
├── Dataloaders            # Descarga de datos
├── Bettors                # Modelos de apuesta
├── Backtesting            # Validación histórica
└── MCP Server             # Integración con AI
```

---

## 🔧 HERRAMIENTAS QUE YA TENEMOS

### 1. MatchEdge API (localhost:5272)
- **Endpoint:** `/api/FootyMetricsTest/page-content`
- **Función:** Scraping de FootyMetrics
- **Limitación:** Max 3000 chars, SPAs no funcionan bien

### 2. ValueAnalysis (C#)
```csharp
// Compara modelo vs mercado
Value = (ModelProb × Odds) - 1
Si Value > 0.05 → Hay edge (apostar)
```

### 3. SegmentAnalysis (C#)
- Segmenta por: favoritos, entropía, temporada, método de cálculo
- Usa Bootstrap para intervalos de confianza
- Corrección de Bonferroni para múltiples tests

### 4. Herramientas de Datos
- SofaScoreAudit
- MatchAnalysis
- DataImporter
- FootyStatsParser

---

## 📊 QUÉ NOS OFRECE FOOTYMETRICS (lo que NO estamos usando)

### ✅ Lo que SÍ usamos:
| Feature | Uso Actual | % de Uso |
|---------|------------|----------|
| BTTS (Both Teams To Score) | Sí | 30% |
| Over/Under 2.5 goals | Sí | 25% |
| H2H básico | Sí | 20% |

### ❌ Lo que NO usamos (desperdiciado):
| Feature | Descripción | Potencial |
|---------|-------------|-----------|
| **Trends** | Jugadores en racha con cuotas | 🔥 ALTO |
| **Props Finder** | Mercados de player props | 🔥 ALTO |
| **Fixture Scout** | Análisis pre-partido completo | 🔥 ALTO |
| **Duels** | Comparación directa jugador vs jugador | MEDIO |
| **Position Stats** | Estadísticas por posición | MEDIO |
| **Referees** | Stats de árbitros (cards, fouls) | 🔥 ALTO |
| **H2H Stats** | Head-to-head detallado | 🔥 ALTO |
| **Corner Markets** | Over/Under 7.5, 8.5, 9.5, 10.5 | MEDIO |
| **Card Markets** | Tarjetas amarillas/rojas | MEDIO |
| **Fouls Markets** | Faltas por partido | MEDIO |
| **AI Tips** | Predicciones con IA | BAJO (verificar) |
| **Watchlist** | Seguimiento de jugadores/equipos | MEDIO |

---

## 📈 SISTEMA DE APUESTAS ACTUAL (el problema)

### Lo que hacemos ahora:
```
1. El usuario envía imagen de Betano
2. Yo busco 3-4 picks "buenos"
3. Hacemos combinada
4. Apostamos
5. Perdimos 😢
```

### Los problemas:
| # | Problema | Impacto |
|---|----------|---------|
| 1 | **Sin backtesting** | No sabemos si los picks son realmente buenos |
| 2 | **Confiamos en "sentimiento"** | Marsella "está mal" → Marsella anotó |
| 3 | **No verificamos H2H reciente** | Porto/Benfica: 10 cards en último H2H |
| 4 | **Apostamos sin valor esperado** | No calculamos EV (Expected Value) |
| 5 | **Stake fijo** | Siempre S/3 sin importar la calidad del pick |
| 6 | **No hay registro sistemático** | No podemos aprender de errores |

---

## 🎯 QUÉ NECESITAMOS APRENDER/IMPLEMENTAR

### Fase 1: Fundamentos (1-2 días)
| # | Tarea | Herramienta |
|---|-------|-------------|
| 1 | **Calcular EV real de cada pick** | Fórmula: EV = (Prob × Cuota) - 1 |
| 2 | **Registrar TODAS las apuestas** | Excel/JSON con resultado |
| 3 | **Calcular yield real** | Ganancia / Total Apostado × 100 |
| 4 | **Identificar patrones de pérdida** | ¿Qué tipo de apuestas perdemos? |

### Fase 2: Datos (2-3 días)
| # | Tarea | Herramienta |
|---|-------|-------------|
| 1 | **Scrapear Trends de FootyMetrics** | API local |
| 2 | **Scrapear Props Finder** | API local |
| 3 | **Scrapear Referee Stats** | API local |
| 4 | **Construir base de datos local** | SQLite/JSON |

### Fase 3: Modelado (3-5 días)
| # | Tarea | Herramienta |
|---|-------|-------------|
| 1 | **Backtesting histórico** | sports-betting library |
| 2 | **Calibración de probabilidades** | Isotonic Regression |
| 3 | **Kelly Criterion para stakes** | Fórmula matemática |
| 4 | **TimeSeriesSplit validation** | Evitar data leakage |

---

## 📋 FORMATO DE ANÁLISIS PARA CADA PICK

```json
{
  "match": "Marsella vs PSG",
  "pick": "BTTS No",
  "bookmaker": "Betano",
  "odds": 2.55,
  "data": {
    "footymetrics_prob": 0.75,
    "h2h_last_5": "4/5 BTTS No",
    "team_form": {
      "marsella_goals_per_game": 1.5,
      "psg_goals_per_game": 2.3
    },
    "referee_avg_cards": 4.14,
    "referee_btts_rate": 0.45
  },
  "calculation": {
    "ev": 0.91,
    "kelly_stake": 0.15,
    "confidence": "MEDIUM",
    "edge_source": "FootyMetrics 75% vs implied 39%"
  },
  "result": null,
  "profit_loss": null
}
```

---

## 🔄 FLUJO DE TRABAJO IDEAL

```
┌─────────────────────────────────────────────────────────────┐
│                    FLUJO IDEAL                              │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  1. CAPTURAR DATOS                                          │
│     ├─ FootyMetrics: Trends, Props, Referees               │
│     ├─ H2H: Últimos 5-10 partidos                          │
│     ├─ Forma: Últimos 5 partidos local/visitante           │
│     └─ Cuotas: Betano (las que tenemos)                    │
│                                                             │
│  2. CALCULAR PROBABILIDAD REAL                              │
│     ├─ No confiar en "80%" de FootyMetrics                 │
│     ├─ Verificar con datos crudos                           │
│     ├─ Promediar múltiples fuentes                          │
│     └─ Ajustar por recency (más peso a recientes)          │
│                                                             │
│  3. CALCULAR EXPECTED VALUE                                 │
│     ├─ EV = (Prob × Cuota) - 1                             │
│     ├─ Solo apostar si EV > 0.05 (5%)                      │
│     └─ Preferir EV > 0.10 (10%)                            │
│                                                             │
│  4. DETERMINAR STAKE (Kelly Criterion)                      │
│     ├─ f = (bp - q) / b                                    │
│     ├─ b = cuota - 1                                       │
│     ├─ p = probabilidad real                                │
│     ├─ q = 1 - p                                           │
│     └─ Nunca apostar más del 5% de la banca                │
│                                                             │
│  5. REGISTRAR Y APRENDER                                    │
│     ├─ Guardar: pick, cuota, prob, EV, resultado           │
│     ├─ Calcular yield después de 20+ apuestas              │
│     ├─ Identificar patrones de éxito/fallo                 │
│     └─ Ajustar estrategia                                  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## 📊 FÓRMULAS CLAVE

### 1. Expected Value (EV)
```
EV = (Probabilidad_Real × Cuota) - 1

Ejemplo:
- Prob = 75% (0.75)
- Cuota = 2.55
- EV = (0.75 × 2.55) - 1 = 0.91 (91% edge)
```

### 2. Kelly Criterion
```
f* = (bp - q) / b

Donde:
- f* = fracción óptima de la banca
- b = cuota - 1 (ganancia neta)
- p = probabilidad de ganar
- q = 1 - p (probabilidad de perder)

Ejemplo:
- Cuota = 2.55 → b = 1.55
- Prob = 0.75 → q = 0.25
- f* = (1.55 × 0.75 - 0.25) / 1.55 = 0.59 (59%)
- ⚠️ Nunca usar 100% Kelly, usar 1/4 o 1/2 Kelly
```

### 3. Yield
```
Yield = (Ganancia Total / Total Apostado) × 100

Ejemplo:
- Apostaste: S/100
- Ganaste: S/120
- Yield = (120 / 100) × 100 = 20%
```

### 4. ROI
```
ROI = (Ganancia Neta / Inversión Inicial) × 100

Ejemplo:
- Inversión: S/100
- Retorno: S/120
- ROI = (20 / 100) × 100 = 20%
```

---

## 🎯 MÉTRICAS QUE DEBEMOS RASTREAR

| Métrica | Objetivo | Alerta |
|---------|----------|--------|
| **Yield** | > 5% | < 0% |
| **ROI** | > 10% | < -10% |
| **Win Rate** | > 55% | < 45% |
| **Avg Odds** | 1.80 - 2.50 | > 3.00 |
| **Avg EV** | > 0.05 | < 0 |
| **Kelly Stake** | 1-5% bankroll | > 10% |
| **Max Drawdown** | < 20% bankroll | > 30% |

---

## 📝 PREGUNTAS PARA CHATGPT

### Sobre FootyMetrics:
1. ¿Cómo puedo scrapear los Trends de FootyMetrics de forma confiable?
2. ¿Qué mercados de player props son más rentables?
3. ¿Cómo combinar referee stats con otros datos?

### Sobre Modelado:
4. ¿Cómo implemento un modelo de regresión logística para predecir BTTS?
5. ¿Qué features son más importantes para predecir goles?
6. ¿Cómo evito overfitting en mis modelos?

### Sobre Gestión de Banca:
7. ¿Cómo implemento Kelly Criterion de forma conservadora?
8. ¿Cuál es la estrategia óptima de stake sizing?
9. ¿Cómo manejo drawdowns prolongados?

### Sobre Disciplina:
10. ¿Cómo creo un sistema que me obligue a seguir las reglas?
11. ¿Cómo elimino el sesgo emocional de mis apuestas?
12. ¿Qué métricas debo revisar diariamente?

---

## 🚀 PRÓXIMOS PASOS INMEDIATOS

### Hoy:
- [ ] Crear spreadsheet de registro de apuestas
- [ ] Definir reglas de entrada (EV > 0.05, Kelly < 5%)
- [ ] Analizar por qué perdimos la combinada de hoy

### Esta semana:
- [ ] Scrapear Trends de FootyMetrics
- [ ] Construir base de datos de picks históricos
- [ ] Implementar backtesting básico

### Próximo mes:
- [ ] Modelo predictivo funcional
- [ ] Kelly Criterion implementado
- [ ] Yield positivo constante

---

## 📚 RECURSOS PARA APRENDER

### Libros:
- "Sharp Sports Betting" - Stanford Wong
- "The Mathematics of Sports Betting" - Elihu Feustel
- "Quantitative Sports Betting" - Riddle

### Repositorios:
- https://github.com/georgedouzas/sports-betting
- https://github.com/AlanPerucky/Betting-Model
- https://github.com/dcaribou/football-predictions

### Cursos:
- Coursera: "Sports Analytics" (Duke)
- edX: "Sports Betting and Analytics"

---

**Documento creado:** 2026-09-20
**Objetivo:** Compartir con ChatGPT para trabajo conjunto
**Estado:** Listo para análisis
