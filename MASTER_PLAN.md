# 🚀 PLAN MAESTRO: Explotar FootyMetrics al Máximo
## Lo que tenemos vs lo que necesitamos hacer

---

## 🎯 RESUMEN EJECUTIVO

### Lo que descubrí:
**YA TENEMOS UN SISTEMA COMPLETO CONSTRUIDO** pero no lo estamos usando.

### Lo que nos falta:
**USARLO** y afinar la metodología.

---

## ✅ LO QUE YA TENEMOS (y no estamos usando)

### 1. **Scraping Automatizado con Playwright**
```
Endpoint: POST /api/FootyMetricsTest/start
- Abre Chrome automáticamente
- Navega a FootyMetrics
- Scrapea datos en tiempo real
```

### 2. **Scoring Engine (Motor de Puntuación)**
```csharp
// FootyMetricsScoringEngine.cs
- Calcula: hitRate, oppHitRate, sampleSize, edge
- Clasifica: Excelente / Muy Bueno / Bueno / Neutral / Descartar
- Ranking automático de picks
```

### 3. **Backtesting (Validación Histórica)**
```
Endpoint: GET /api/FootyMetricsTest/backtest/run
- Valida picks contra resultados reales
- Calcula yield, ROI, win rate
- Identifica patrones de éxito/fallo
```

### 4. **Gestión de Banca con Kelly Criterion**
```
Endpoint: POST /api/FootyMetricsTest/bankroll/initialize
- Kelly Criterion automático
- Max stake por pick
- Max exposición diaria
- Max drawdown configurables
```

### 5. **Persistencia (Base de Datos)**
```
Endpoint: POST /api/FootyMetricsTest/persist/scrape-and-save/{fixtureSlug}
- Guarda todos los trends
- Guarda picks scored
- Historial completo para análisis
```

### 6. **Pipeline Diario Automatizado**
```
Endpoint: POST /api/FootyMetricsTest/pipeline/run
- Scrapea fixtures del día
- Scrapea trends de cada fixture
- Calcula scores
- Guarda picks
- Recomienda stakes
```

---

## 📊 ENDPOINTS DISPONIBLES (que NO usamos)

### Scraping:
| Endpoint | Función | Uso Actual |
|----------|---------|------------|
| `POST /start` | Iniciar Chrome | ❌ No usado |
| `GET /status` | Verificar estado | ❌ No usado |
| `GET /scrape/fixture-trends/{slug}` | Scrapear trends de fixture | ❌ No usado |
| `GET /scrape/market-trends/{market}` | Scrapear trends por mercado | ❌ No usado |

### Scoring:
| Endpoint | Función | Uso Actual |
|----------|---------|------------|
| `GET /scoring/fixture-trends/{slug}` | Score de fixture | ❌ No usado |
| `GET /scoring/market-trends/{market}` | Score por mercado | ❌ No usado |
| `GET /scoring/team-trends/{id}` | Score por equipo | ❌ No usado |

### Backtesting:
| Endpoint | Función | Uso Actual |
|----------|---------|------------|
| `GET /backtest/run` | Ejecutar backtest | ❌ No usado |
| `GET /backtest/summary` | Resumen de backtest | ❌ No usado |
| `POST /backtest/evaluate` | Evaluar picks | ❌ No usado |

### Bankroll:
| Endpoint | Función | Uso Actual |
|----------|---------|------------|
| `POST /bankroll/initialize` | Inicializar banca | ❌ No usado |
| `GET /bankroll/state` | Ver estado | ❌ No usado |
| `POST /bankroll/calculate-stakes` | Calcular stakes | ❌ No usado |
| `POST /bankroll/apply-result` | Aplicar resultado | ❌ No usado |

### Pipeline:
| Endpoint | Función | Uso Actual |
|----------|---------|------------|
| `GET /pipeline/status` | Estado del pipeline | ❌ No usado |
| `POST /pipeline/run` | Ejecutar pipeline diario | ❌ No usado |

---

## 🔍 QUÉ NOS FALTA PARA "GARANTÍAS ALTÍSIMAS"

### ❌ Lo que NO tenemos (y necesitamos):

#### 1. **Cross-Referencing de Múltiples Fuentes**
```csharp
// Necesitamos combinar:
- FootyMetrics Trends (player props)
- Referee Stats (cards, fouls)
- H2H Stats (historial entre equipos)
- Forma reciente (últimos 5 partidos)
- Cuotas de Betano (nuestro bookmaker)
```

#### 2. **Validación de Probabilidades**
```csharp
// No confiar en "80%" de FootyMetrics
// Verificar manualmente con datos crudos
double realProbability = CalculateFromRawData(match);
double footymetricsProb = 0.80;
if (Math.Abs(realProbability - footymetricsProb) > 0.10)
{
    // Diferencia significativa - investigar
}
```

#### 3. **Sistema de Confianza Multi-Factor**
```csharp
// Score de confianza = Promedio ponderado de:
double confidence = (
    footymetricsScore * 0.30 +      // Trends
    refereeScore * 0.20 +           // Referee
    h2hScore * 0.25 +              // H2H
    formScore * 0.15 +             // Forma
    valueScore * 0.10              // Value (EV)
);
// Solo apostar si confidence > 0.75
```

#### 4. **Filtros de Calidad**
```csharp
// Filtros para "garantías altísimas":
if (hitRate < 0.80) continue;           // Mínimo 80% hit rate
if (sampleSize < 5) continue;           // Mínimo 5 observaciones
if (edge < 0.10) continue;             // Mínimo 10% edge
if (confidence < 0.75) continue;        // Mínimo 75% confianza
```

#### 5. **Kelly Criterion Conservador**
```csharp
// Usar 1/4 Kelly en vez de 1/2 Kelly
double kellyFraction = 0.25;  // Conservador
double stake = kelly * bankroll * kellyFraction;
// Nunca apostar más del 2-3% de la banca
```

---

## 📋 PLAN DE IMPLEMENTACIÓN (7 días)

### Día 1: Activar Pipeline
```bash
# 1. Iniciar Chrome para FootyMetrics
curl -X POST http://localhost:5272/api/FootyMetricsTest/start

# 2. Ejecutar pipeline diario
curl -X POST "http://localhost:5272/api/FootyMetricsTest/pipeline/run?maxFixtures=50&topPicksPerFixture=10&topPicksOverall=30"

# 3. Ver picks generados
curl "http://localhost:5272/api/FootyMetricsTest/persist/daily-picks"
```

### Día 2: Configurar Bankroll
```bash
# 1. Inicializar banca con Kelly conservador
curl -X POST "http://localhost:5272/api/FootyMetricsTest/bankroll/initialize?initialBankroll=33" \
  -H "Content-Type: application/json" \
  -d '{
    "InitialBankroll": 33,
    "KellyFraction": 0.25,
    "MaxStakePerPick": 2,
    "MaxDailyExposure": 10,
    "MaxDrawdownPercent": 0.30,
    "MinEdge": 0.10,
    "MinSampleSize": 5,
    "StakeMethod": "Kelly"
  }'

# 2. Ver configuración
curl http://localhost:5272/api/FootyMetricsTest/bankroll/config
```

### Día 3: Crear Filtros de Calidad
```csharp
// Nuevo endpoint: /api/FootyMetricsTest/picks/filtered
[HttpGet("picks/filtered")]
public async Task<IActionResult> GetFilteredPicks(
    [FromQuery] double minHitRate = 0.80,
    [FromQuery] int minSampleSize = 5,
    [FromQuery] double minEdge = 0.10,
    [FromQuery] double minConfidence = 0.75)
{
    var picks = await _persistence.GetDailyPicksAsync(DateTime.UtcNow);
    
    var filtered = picks
        .Where(p => p.HitRate >= minHitRate)
        .Where(p => p.SampleSize >= minSampleSize)
        .Where(p => p.Edge >= minEdge)
        .Where(p => p.Confidence >= minConfidence)
        .OrderByDescending(p => p.CompositeScore)
        .ToList();
    
    return Ok(new { count = filtered.Count, picks = filtered });
}
```

### Día 4: Cross-Referencing
```csharp
// Nuevo endpoint: /api/FootyMetricsTest/picks/cross-referenced
[HttpGet("picks/cross-referenced")]
public async Task<IActionResult> GetCrossReferencedPicks()
{
    var picks = await _persistence.GetDailyPicksAsync(DateTime.UtcNow);
    var crossReferenced = new List<CrossReferencedPick>();
    
    foreach (var pick in picks)
    {
        // 1. Obtener referee stats
        var referee = await GetRefereeStats(pick.RefereeName);
        
        // 2. Obtener H2H
        var h2h = await GetH2HStats(pick.HomeTeam, pick.AwayTeam);
        
        // 3. Obtener forma reciente
        var form = await GetRecentForm(pick.Team);
        
        // 4. Calcular score combinado
        var combinedScore = CalculateCombinedScore(pick, referee, h2h, form);
        
        if (combinedScore > 0.75)
        {
            crossReferenced.Add(new CrossReferencedPick
            {
                Pick = pick,
                RefereeScore = referee.Score,
                H2HScore = h2h.Score,
                FormScore = form.Score,
                CombinedScore = combinedScore
            });
        }
    }
    
    return Ok(crossReferenced.OrderByDescending(p => p.CombinedScore));
}
```

### Día 5: Validación Histórica
```bash
# 1. Ejecutar backtest de los últimos 7 días
curl "http://localhost:5272/api/FootyMetricsTest/backtest/run?fromDate=2026-09-13&toDate=2026-09-20"

# 2. Ver resumen
curl http://localhost:5272/api/FootyMetricsTest/backtest/summary

# 3. Evaluar picks pendientes
curl "http://localhost:5272/api/FootyMetricsTest/backtest/evaluatable/2026-09-20"
```

### Día 6: Dashboard de Métricas
```csharp
// Nuevo endpoint: /api/FootyMetricsTest/metrics/dashboard
[HttpGet("metrics/dashboard")]
public async Task<IActionResult> GetDashboard()
{
    var bankroll = await _bankroll.GetStateAsync();
    var todayPicks = await _persistence.GetDailyPicksAsync(DateTime.UtcNow);
    var backtest = await _backtesting.GetSummaryAsync();
    
    return Ok(new
    {
        bankroll = new
        {
            balance = bankroll.CurrentBalance,
            profit = bankroll.TotalProfit,
            roi = bankroll.ROI
        },
        today = new
        {
            totalPicks = todayPicks.Count,
            pending = todayPicks.Count(p => p.Status == "Pending"),
            won = todayPicks.Count(p => p.Status == "Won"),
            lost = todayPicks.Count(p => p.Status == "Lost")
        },
        backtest = new
        {
            yield = backtest.Yield,
            winRate = backtest.WinRate,
            totalBets = backtest.TotalBets
        }
    });
}
```

### Día 7: Flujo Completo
```bash
# FLUJO COMPLETO DIARIO:

# 1. Iniciar sistema
curl -X POST http://localhost:5272/api/FootyMetricsTest/start

# 2. Ejecutar pipeline
curl -X POST "http://localhost:5272/api/FootyMetricsTest/pipeline/run"

# 3. Ver picks filtrados
curl "http://localhost:5272/api/FootyMetricsTest/picks/filtered?minHitRate=0.80&minEdge=0.10"

# 4. Ver picks cross-referenced
curl http://localhost:5272/api/FootyMetricsTest/picks/cross-referenced

# 5. Calcular stakes con Kelly
curl -X POST "http://localhost:5272/api/FootyMetricsTest/bankroll/calculate-stakes"

# 6. Apostar (manualmente en Betano)

# 7. Registrar resultado
curl -X POST "http://localhost:5272/api/FootyMetricsTest/bankroll/apply-result" \
  -H "Content-Type: application/json" \
  -d '{"pickId": 1, "won": true, "odds": 2.55, "stakeUnits": 1.5}'

# 8. Ver dashboard
curl http://localhost:5272/api/FootyMetricsTest/metrics/dashboard
```

---

## 🎯 CRITERIOS PARA "GARANTÍAS ALTÍSIMAS"

### Filtros Obligatorios:
```csharp
// UNO SOLO de estos criterios debe fallar para descartar un pick:
if (hitRate < 0.80) return "DESCARTAR";      // Mínimo 80% hit rate
if (sampleSize < 5) return "DESCARTAR";      // Mínimo 5 observaciones
if (edge < 0.10) return "DESCARTAR";        // Mínimo 10% edge
if (confidence < 0.75) return "DESCARTAR";   // Mínimo 75% confianza
if (combinedScore < 0.70) return "DESCARTAR"; // Mínimo 70% score combinado
```

### Scoring de Confianza:
```csharp
double confidence = (
    footymetricsScore * 0.30 +      // Trends (lo que ya tenemos)
    refereeScore * 0.20 +           // Referee stats
    h2hScore * 0.25 +              // H2H history
    formScore * 0.15 +             // Recent form
    valueScore * 0.10              // Expected Value
);

// Clasificación:
// 0.90 - 1.00 = 🔥 GARANTÍA ALTÍSIMA
// 0.80 - 0.89 = ✅ MUY CONFIABLE
// 0.75 - 0.79 = ⚠️ CONFIABLE
// 0.70 - 0.74 = 🟡 NEUTRAL
// < 0.70     = ❌ DESCARTAR
```

---

## 📊 EJEMPLO: CÓMO FUNCIONARÍA

### Pick: "BTTS No - Marsella vs PSG"

**Paso 1: FootyMetrics Trends**
```
Hit Rate: 75% (8/11 partidos)
Opp Hit Rate: 27%
Sample Size: 11
Odds: 2.55
Edge: 75% - 39% = 36%
Score: 0.82
```

**Paso 2: Referee Stats**
```
Árbitro: Letexier
Avg Cards: 4.14
BTTS Rate: 45%
Score: 0.65
```

**Paso 3: H2H Stats**
```
Últimos 5 partidos: 4/5 BTTS No (80%)
Score: 0.85
```

**Paso 4: Forma Reciente**
```
Marsella: 1.5 goles/partido
PSG: 2.3 goles/partido
Score: 0.60
```

**Paso 5: Score Combinado**
```
Confidence = (0.82 × 0.30) + (0.65 × 0.20) + (0.85 × 0.25) + (0.60 × 0.15) + (0.91 × 0.10)
Confidence = 0.246 + 0.13 + 0.2125 + 0.09 + 0.091
Confidence = 0.7695 (76.95%)
```

**Paso 6: Decisión**
```
¿Confidence > 0.75? ✅ SÍ
¿Hit Rate > 0.80? ❌ NO (75%)
¿Edge > 0.10? ✅ SÍ (36%)

DECISIÓN: ⚠️ CONFIABLE (no GARANTÍA ALTÍSIMA)
RECOMENDACIÓN: Apostar con 1/4 Kelly ( stake = 1.5% de banca )
```

---

## 🚀 PRÓXIMOS PASOS INMEDIATOS

### Hoy:
1. [ ] Iniciar Chrome: `POST /api/FootyMetricsTest/start`
2. [ ] Ejecutar pipeline: `POST /api/FootyMetricsTest/pipeline/run`
3. [ ] Ver picks: `GET /api/FootyMetricsTest/persist/daily-picks`

### Mañana:
1. [ ] Configurar bankroll con Kelly conservador
2. [ ] Crear filtros de calidad
3. [ ] Ejecutar primer backtest

### Esta semana:
1. [ ] Implementar cross-referencing
2. [ ] Crear dashboard de métricas
3. [ ] Primer paper trading (sin dinero real)

---

## 📈 MÉTRICAS DE ÉXITO

| Métrica | Objetivo | Alerta |
|---------|----------|--------|
| **Hit Rate** | > 80% | < 70% |
| **Yield** | > 10% | < 0% |
| **ROI** | > 20% | < -10% |
| **Max Drawdown** | < 20% | > 30% |
| **Avg Edge** | > 15% | < 5% |
| **Confidence** | > 0.75 | < 0.60 |

---

## ⚠️ ADVERTENCIAS

### Lo que NO debemos hacer:
1. **No apostar sin pipeline** → Siempre usar el sistema
2. **No usar 100% Kelly** → Siempre 1/4 o 1/2 Kelly
3. **No confiar en un solo dato** → Siempre cross-reference
4. **No apostar más del 3% de banca** → Máximo 2-3% por pick
5. **No perseguir pérdidas** → Disciplina total

### Lo que SÍ debemos hacer:
1. **Registrar TODAS las apuestas** → Sin excepciones
2. **Revisar dashboard diariamente** → Análisis constante
3. **Ajustar estrategia** → Based en métricas
4. **Ser paciente** → El edge se ve a largo plazo
5. **Aprender de errores** → Cada pérdida es una lección

---

**Conclusión:** Tenemos un **sistema completo** construido pero **aprovechamos solo el 10%**. Si activamos todo lo que ya tenemos + los filtros de calidad + cross-referencing, podemos encontrar picks con **garantías altísimas**.

---

**Documento creado:** 2026-09-20
**Objetivo:** Plan maestro para explotar FootyMetrics
**Estado:** Listo para implementar
