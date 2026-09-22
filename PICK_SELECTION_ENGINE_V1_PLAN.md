# 📋 PLAN DE IMPLEMENTACIÓN - PICK SELECTION ENGINE V1
## Análisis de lo existente vs lo necesario

---

## 🔍 LO QUE YA EXISTE (reutilizar)

### Endpoints (33 existentes):
| Endpoint | Reutilizar | Notas |
|----------|------------|-------|
| `POST /start` | ✅ | Iniciar Chrome |
| `GET /status` | ✅ | Verificar browser |
| `POST /pipeline/run` | ✅ | Pipeline completo |
| `GET /persist/daily-picks` | ✅ | Ya retorna picks |
| `GET /scoring/fixture-trends/{slug}` | ✅ | Scoring por fixture |
| `GET /scoring/market-trends/{market}` | ✅ | Scoring por mercado |
| `POST /backtest/evaluate` | ✅ | Evaluar resultados |
| `GET /backtest/summary` | ✅ | Métricas aggregadas |

### Servicios (6 existentes):
| Servicio | Reutilizar | Función |
|----------|------------|---------|
| `OrchestratorService` | ✅ | Pipeline completo |
| `FootyMetricsDomScraper` | ✅ | Scraping DOM |
| `FootyMetricsScoringEngine` | ✅ | Scoring base |
| `TrendPersistenceService` | ✅ | Guardar en SQLite |
| `TrendBacktestingService` | ✅ | Backtesting |
| `BankrollManager` | ⚠️ | No usar en V1 |

### Entidades (8 existentes):
| Entidad | Reutilizar | Campos faltantes |
|---------|------------|------------------|
| `TrendResultEntity` | ✅ | Ninguno |
| `DailyPickEntity` | ⚠️ | HomeTeam, AwayTeam, League, FixtureSlug, FeaturesJson |
| `PickOutcomeEntity` | ✅ | ClosingOdds, CLV |

---

## ❌ LO QUE NO EXISTE (crear)

### 1. PredictionSnapshotEntity
**Propósito:** Almacenar el estado EXACTO en el momento de la predicción

```csharp
// CAMPOS NECESARIOS (los que faltan en DailyPickEntity):
- HomeTeam (string)
- AwayTeam (string)  
- League (string)
- FixtureSlug (string)
- HomeAwayContext (string) - "home"/"away"/"both"
- FeaturesJson (string) - JSON con todas las features
- Ranking (int) - Posición en el ranking del día
- ClosingOdds (double?) - Cuota de cierre (post-partido)
- CLV (double?) - Closing Line Value
- ActualResult (string?) - "Won"/"Lost"/"Push"/"Void"
- ResultTimestamp (DateTime?) - Cuándo se registró el resultado
```

### 2. PickSelectionEngine
**Propósito:** Seleccionar y rankear picks con explicaciones

```csharp
// FLUJO:
Input: List<FootyMetricsScrapedTrend> + List<TrendScoreResult>
  ↓
Step 1: Normalize (convertir a formato estándar)
  ↓
Step 2: Filter (calidad mínima V1)
  ↓
Step 3: Score (multi-factor, no solo CompositeScore)
  ↓
Step 4: Rank (ordenar por score compuesto)
  ↓
Step 5: Classify (CANDIDATE/STRONG/TOP)
  ↓
Step 6: Explain (generar razones)
  ↓
Output: List<SelectedPick> con razones
```

### 3. SelectedPick DTO
**Propósito:** Retornar picks con explicaciones

```csharp
// CAMPOS:
- Rank (int)
- Fixture (string) - "Team A vs Team B"
- Market (string)
- Selection (string) - "Over 1.5"
- Odds (double)
- HistoricalHitRate (double)
- SampleSize (int)
- OpponentHitRate (double)
- HistoricalEdge (double)
- CompositeScore (double)
- Classification (string) - "CANDIDATE"/"STRONG"/"TOP_PICK"
- Reasons (List<string>) - Razones de selección
- FeaturesJson (string) - Todas las features para futuro ML
```

### 4. /picks/today endpoint
**Propósito:** Retornar top picks del día

```csharp
// RESPONSE:
{
  "generatedAt": "...",
  "totalCandidates": 127,
  "qualifiedCandidates": 34,
  "topPicks": [
    {
      "rank": 1,
      "fixture": "Team A vs Team B",
      "market": "Goals",
      "selection": "Over 1.5",
      "odds": 1.42,
      "historicalHitRate": 0.84,
      "sampleSize": 25,
      "opponentHitRate": 0.72,
      "edge": 0.136,
      "score": 0.78,
      "classification": "TOP_PICK",
      "reasons": [
        "+ High historical hit rate (84%)",
        "+ Sample above minimum (25)",
        "+ Positive historical edge (+13.6%)",
        "- Odds relatively low (1.42)"
      ]
    }
  ]
}
```

---

## 📊 ANÁLISIS: QUÉ CAMBIOS MINIMOS HACER

### Opción A: Extender DailyPickEntity (RECOMENDADO)
**Ventajas:**
- No crear nueva entidad
- Reutilizar toda la infraestructura existente
- Backtesting ya funciona

**Desventajas:**
- Entidad crece de 17 a 28 campos
- Algunos campos son post-partido

**Implementación:**
```csharp
// Agregar campos a DailyPickEntity:
public string HomeTeam { get; set; } = string.Empty;
public string AwayTeam { get; set; } = string.Empty;
public string League { get; set; } = string.Empty;
public string FixtureSlug { get; set; } = string.Empty;
public string HomeAwayContext { get; set; } = "both";
public string FeaturesJson { get; set; } = "{}";
public int Ranking { get; set; }
public double? ClosingOdds { get; set; }
public double? CLV { get; set; }
public string? ActualResult { get; set; }
public DateTime? ResultTimestamp { get; set; }
```

### Opción B: Crear PredictionSnapshotEntity separada
**Ventajas:**
- Separación clara de concernimientos
- DailyPickEntity se mantiene simple

**Desventajas:**
- Duplicación de datos
- Más tablas que mantener
- Backtesting necesita modificar

**Decisión: OPCIÓN A** (extender DailyPickEntity)

---

## 🔧 FLUJO DE IMPLEMENTACIÓN

### Paso 1: Agregar campos a DailyPickEntity
```csharp
// Archivo: DailyPickEntity.cs
// Agregar 11 campos nuevos
```

### Paso 2: Actualizar DailyPickEntityDto
```csharp
// Archivo: ITrendPersistenceService.cs
// Agregar campos al DTO
```

### Paso 3: Crear PickSelectionEngine
```csharp
// Archivo nuevo: PickSelectionEngine.cs
// Responsabilidad: Normalizar → Filtrar → Rankear → Explicar
```

### Paso 4: Crear SelectedPick DTO
```csharp
// Archivo nuevo: SelectedPick.cs
// DTO para picks con explicaciones
```

### Paso 5: Actualizar TrendPersistenceService
```csharp
// Archivo: TrendPersistenceService.cs
// Modificar SaveScoredPicksAsync para guardar campos nuevos
```

### Paso 6: Crear endpoint /picks/today
```csharp
// Archivo: FootyMetricsTestController.cs
// Nuevo endpoint que usa PickSelectionEngine
```

### Paso 7: Actualizar OrchestratorService
```csharp
// Archivo: OrchestratorService.cs
// Usar PickSelectionEngine en pipeline
```

---

## 📐 DISEÑO DEL PICKSELECTIONENGINE

```csharp
public class PickSelectionEngine
{
    // Configuración V1 (heurística, no estadística)
    private const double MinHitRate = 0.55;      // 55% mínimo
    private const int MinSampleSize = 5;          // 5 observaciones mínimo
    private const double MinOdds = 1.20;          // Cuota mínima
    private const double MaxOdds = 5.00;          // Cuota máxima
    private const int MaxPicksPerDay = 10;        // Máximo 10 picks por día

    public List<SelectedPick> SelectPicks(
        List<FootyMetricsScrapedTrend> trends,
        List<TrendScoreResult> scoredPicks)
    {
        // Step 1: Normalize
        var normalized = Normalize(trends, scoredPicks);
        
        // Step 2: Filter
        var filtered = Filter(normalized);
        
        // Step 3: Score (multi-factor)
        var scored = Score(filtered);
        
        // Step 4: Rank
        var ranked = Rank(scored);
        
        // Step 5: Classify
        var classified = Classify(ranked);
        
        // Step 6: Explain
        var explained = Explain(classified);
        
        return explained.Take(MaxPicksPerDay).ToList();
    }

    private double CalculateMultiFactorScore(SelectedPick pick)
    {
        // Fórmula V1 (heurística):
        // Score = (HitRate * 0.30) + (SampleScore * 0.20) + (EdgeScore * 0.25) + (OddsScore * 0.15) + (OppScore * 0.10)
        
        var hitRateScore = pick.HistoricalHitRate;
        var sampleScore = Math.Min(pick.SampleSize / 20.0, 1.0); // Normalizado a 20
        var edgeScore = Math.Max(0, Math.Min(pick.HistoricalEdge / 0.30, 1.0)); // Normalizado a 30%
        var oddsScore = CalculateOddsScore(pick.Odds);
        var oppScore = pick.OpponentHitRate > 0 ? pick.OpponentHitRate : 0.5;
        
        return (hitRateScore * 0.30) + 
               (sampleScore * 0.20) + 
               (edgeScore * 0.25) + 
               (oddsScore * 0.15) + 
               (oppScore * 0.10);
    }

    private double CalculateOddsScore(double odds)
    {
        // Cuotas entre 1.50 y 2.50 son ideales
        if (odds >= 1.50 && odds <= 2.50) return 1.0;
        if (odds >= 1.20 && odds < 1.50) return 0.7;
        if (odds > 2.50 && odds <= 3.50) return 0.8;
        if (odds > 3.50 && odds <= 5.00) return 0.5;
        return 0.3;
    }

    private List<string> GenerateReasons(SelectedPick pick)
    {
        var reasons = new List<string>();
        
        if (pick.HistoricalHitRate >= 0.75)
            reasons.Add($"+ High historical hit rate ({pick.HistoricalHitRate:P0})");
        else if (pick.HistoricalHitRate >= 0.65)
            reasons.Add($"+ Good historical hit rate ({pick.HistoricalHitRate:P0})");
        else
            reasons.Add($"- Moderate historical hit rate ({pick.HistoricalHitRate:P0})");
        
        if (pick.SampleSize >= 15)
            reasons.Add($"+ Strong sample size ({pick.SampleSize})");
        else if (pick.SampleSize >= 8)
            reasons.Add($"+ Adequate sample size ({pick.SampleSize})");
        else
            reasons.Add($"- Small sample size ({pick.SampleSize})");
        
        if (pick.HistoricalEdge > 0.15)
            reasons.Add($"+ Strong historical edge (+{pick.HistoricalEdge:P1})");
        else if (pick.HistoricalEdge > 0.05)
            reasons.Add($"+ Positive historical edge (+{pick.HistoricalEdge:P1})");
        else
            reasons.Add($"- Minimal historical edge ({pick.HistoricalEdge:P1})");
        
        if (pick.Odds >= 1.50 && pick.Odds <= 2.50)
            reasons.Add("+ Good odds range (1.50-2.50)");
        else if (pick.Odds < 1.50)
            reasons.Add("- Low odds (potential low value)");
        else
            reasons.Add("- High odds (higher risk)");
        
        if (pick.OpponentHitRate > 0)
        {
            if (pick.OpponentHitRate < 0.40)
                reasons.Add($"+ Opponent low rate ({pick.OpponentHitRate:P0})");
            else
                reasons.Add($"- Opponent moderate rate ({pick.OpponentHitRate:P0})");
        }
        
        return reasons;
    }
}
```

---

## 📊 MÉTRICAS A REGISTRAR (V1)

### Pick Performance (ya existe en DailyPickEntity + PickOutcome):
- Number of picks
- Wins / Losses
- Win Rate
- Profit / ROI / Yield
- Maximum Drawdown

### Prediction Quality (nuevo):
- Historical Hit Rate distribution
- Sample Size distribution
- Edge distribution
- Classification distribution

### Market Quality (nuevo, cuando tengamos ClosingOdds):
- Closing Odds
- CLV (Closing Line Value)

---

## 🎯 UMBRALES V1 (HEURÍSTICOS, NO ESTADÍSTICOS)

**IMPORTANTE:** Estos son umbrales operacionales iniciales. No están validados estadísticamente.

```csharp
// Filtros mínimos para V1:
MinHitRate = 0.55        // 55% histórico mínimo
MinSampleSize = 5        // 5 observaciones mínimo
MinOdds = 1.20           // Cuota mínima
MaxOdds = 5.00           // Cuota máxima
MaxPicksPerDay = 10      // Máximo 10 picks por día

// Clasificación V1:
CANDIDATE:    Cumple filtros mínimos
STRONG:       HitRate >= 0.65 AND SampleSize >= 10 AND Edge > 0.05
TOP_PICK:     Score >= 0.70 AND HitRate >= 0.70 AND SampleSize >= 8
```

**Nota:** Estos umbrales se ajustarán después de observar la distribución real de los datos.

---

## 📁 ARCHIVOS A MODIFICAR/CREAR

### Modificar:
1. `src/MatchEdge.Infrastructure/Data/Entities/DailyPickEntity.cs` - Agregar 11 campos
2. `src/MatchEdge.Application/Services/ITrendPersistenceService.cs` - Actualizar DTO
3. `src/MatchEdge.Infrastructure/Services/TrendPersistenceService.cs` - Guardar campos nuevos
4. `src/MatchEdge.Infrastructure/Services/OrchestratorService.cs` - Usar PickSelectionEngine
5. `src/MatchEdge.Api/Controllers/FootyMetricsTestController.cs` - Nuevo endpoint

### Crear:
1. `src/MatchEdge.Application/Services/IPickSelectionEngine.cs` - Interfaz
2. `src/MatchEdge.Infrastructure/Services/PickSelectionEngine.cs` - Implementación
3. `src/MatchEdge.Application/Services/SelectedPick.cs` - DTO

---

## ⏱️ ESTIMACIÓN DE TIEMPO

| Tarea | Tiempo |
|-------|--------|
| Actualizar DailyPickEntity | 15 min |
| Actualizar DTO | 10 min |
| Crear PickSelectionEngine | 45 min |
| Crear SelectedPick DTO | 10 min |
| Actualizar TrendPersistenceService | 20 min |
| Crear endpoint /picks/today | 20 min |
| Actualizar OrchestratorService | 15 min |
| Testing manual | 30 min |
| **TOTAL** | **~2.5 horas** |

---

## 🚀 PRÓXIMOS PASOS DESPUÉS DE V1

1. **Observar distribución de datos** - Ajustar umbrales
2. **Agregar ClosingOdds** - Para calcular CLV
3. **Crear dashboard de métricas** - Visualización
4. **Analizar resultados** - Qué funciona, qué no
5. **V2: Modelo de calibración** - Convertir HitRate a probabilidad calibrada
6. **V3: ML simple** - Logistic Regression para mejorar ranking

---

**Plan creado:** 2026-09-20
**Estado:** Listo para implementación
**Estimación:** 2.5 horas
