# 📊 GUÍA: FootyMetrics - Qué Tenemos y Cómo Usarlo
## Análisis completo de la plataforma

---

## 🎯 QUÉ ES FOOTYMETRICS

FootyMetrics es una plataforma de **estadísticas de fútbol a nivel Opta** diseñada para apuestas. Ofrece:

- **115+ ligas** cubiertas
- **Datos Opta-level** (la misma calidad que usan los clubs profesionales)
- **Actualización cada pocos minutos**
- **Herramientas de betting** integradas

---

## 📋 FEATURES DISPONIBLES (y cuáles NO usamos)

### ✅ LO QUE SÍ USAMOS (30% del potencial)

| Feature | Uso Actual | Potencial Real |
|---------|------------|----------------|
| **BTTS** | Sí, manual | ⭐⭐⭐ |
| **Over/Under 2.5** | Sí, básico | ⭐⭐⭐ |
| **H2H básico** | Sí, manual | ⭐⭐⭐ |
| **Forma reciente** | Sí, básico | ⭐⭐ |

### ❌ LO QUE NO USAMOS (70% desperdiciado)

#### 🔥 ALTO IMPACTO (deberíamos usar YA)

| Feature | Descripción | Cómo Nos Ayudaría |
|---------|-------------|-------------------|
| **Trends** | Jugadores en racha con cuotas | Encontrar picks de alta probabilidad |
| **Props Finder** | Mercados de player props | Shots, tackles, cards por jugador |
| **Referee Stats** | Estadísticas de árbitros | Cards, fouls, penales por árbitro |
| **H2H Stats** | Head-to-head detallado | Tendencias específicas entre equipos |
| **Fixture Scout** | Análisis pre-partido completo | Todo en un solo lugar |

#### 🟡 MEDIO IMPACTO

| Feature | Descripción | Cómo Nos Ayudaría |
|---------|-------------|-------------------|
| **Duels** | Comparación jugador vs jugador | Encontrar edges en player props |
| **Position Stats** | Estadísticas por posición | Defensas vs delanteros |
| **Corner Markets** | Over/Under 7.5-10.5 corners | Mercados adicionales |
| **Card Markets** | Tarjetas amarillas/rojas | Combinar con referee stats |
| **Fouls Markets** | Faltas por partido | Otro mercado más |
| **Watchlist** | Seguimiento de jugadores | Alertas automáticas |

#### 🟠 BAJO IMPACTO (verificar primero)

| Feature | Descripción | Nota |
|---------|-------------|------|
| **AI Tips** | Predicciones con IA | Verificar si son buenos |
| **Predictions** | Predicciones generales | Comparar con nuestros modelos |

---

## 🔍 CÓMO ACCEDER A CADA FEATURE

### 1. Trends (Jugadores en Racha)
```
URL: https://footymetrics.com/football/trends
Datos: Jugadores con Over/Under en racha + cuotas
Ejemplo: G. Boschilia - Shots Over 1.5 - 8/8 (100%) - Cuota 1.32
```

### 2. Props Finder (Player Props)
```
URL: https://footymetrics.com/football/props-finder
Datos: Mercados de player props disponibles
Filtros: Por jugador, equipo, mercado, liga
```

### 3. Referee Stats
```
URL: https://footymetrics.com/football/referees
Datos: 
- Avg yellow cards por árbitro
- Avg fouls per game
- Card rates
- Penalty rates
```

### 4. H2H Stats
```
URL: https://footymetrics.com/football/h2h
Datos:
- Últimos enfrentamientos
- Goles por partido
- BTTS rate
- Over/Under rates
```

### 5. Fixture Scout
```
URL: https://footymetrics.com/football/fixtures/[id]
Datos:
- Análisis completo del partido
- Probabilidades por mercado
- Forma de ambos equipos
- Referee asignado
```

---

## 📊 EJEMPLO: CÓMO USAR MEJOR FOOTYMETRICS

### Antes (cómo lo hacemos):
```
1. Usuario envía imagen de Betano
2. Yo busco "BTTS No" en Marsella vs PSG
3. Digo "80% BTTS No" (sin verificar)
4. Apostamos
5. Perdimos (Marsella anotó)
```

### Después (cómo deberíamos hacer):
```
1. Usuario envía imagen de Betano
2. Yo scraping de FootyMetrics:
   a. Trends: ¿Algún jugador en racha de goles?
   b. Referee: ¿Cuántas cards promedia el árbitro?
   c. H2H: ¿Últimos 5 partidos entre ellos?
   d. Fixture Scout: Análisis completo
3. Verifico datos manualmente:
   a. Marsella: 1.5 goles/partido (no es "mal equipo")
   b. PSG: 2.3 goles/partido
   c. H2H: 4/5 partidos con BTTS Sí
4. Calculo EV:
   a. Probabilidad real BTTS No: 60% (no 80%)
   b. Cuota Betano: 2.55
   c. EV = (0.60 × 2.55) - 1 = 0.53 (53% edge)
5. Decido:
   a. EV > 0.05 → SÍ es value bet
   b. Kelly: 1/4 Kelly = 12% de banca
   c. Apostar con stake calculado
```

---

## 🛠️ HERRAMIENTAS QUE NECESITAMOS CREAR

### 1. FootyMetrics Scraper Mejorado
```csharp
public class FootyMetricsScraper
{
    public async Task<Trends> GetTrends(string league)
    {
        // Scrapear trends de FootyMetrics
        // Retornar jugadores en racha
    }
    
    public async Task<RefereeStats> GetRefereeStats(string refereeName)
    {
        // Scrapear stats del árbitro
        // Retornar avg cards, fouls, etc.
    }
    
    public async Task<H2HStats> GetH2HStats(string team1, string team2)
    {
        // Scrapear head-to-head
        // Retornar últimos 5-10 partidos
    }
}
```

### 2. Value Bet Calculator
```csharp
public class ValueBetCalculator
{
    public double CalculateEV(double probability, double odds)
    {
        return (probability * odds) - 1;
    }
    
    public double CalculateKelly(double probability, double odds)
    {
        double b = odds - 1;
        double p = probability;
        double q = 1 - p;
        return (b * p - q) / b;
    }
    
    public bool IsValueBet(double ev, double threshold = 0.05)
    {
        return ev > threshold;
    }
}
```

### 3. Bet Tracker
```csharp
public class BetTracker
{
    public void RecordBet(Bet bet)
    {
        // Guardar en JSON/SQLite
        // Incluir: pick, cuota, prob, EV, resultado
    }
    
    public Metrics CalculateMetrics(List<Bet> bets)
    {
        // Calcular yield, ROI, win rate, etc.
    }
    
    public List<Pattern> IdentifyPatterns(List<Bet> bets)
    {
        // Identificar patrones de éxito/fallo
    }
}
```

---

## 📈 MÉTRICAS QUE DEBEMOS RASTREAR

### Por Pick:
| Métrica | Fórmula | Objetivo |
|---------|---------|----------|
| **EV** | (Prob × Cuota) - 1 | > 0.05 |
| **Kelly** | (bp - q) / b | < 0.05 |
| **Confidence** | Subjetivo | HIGH/MEDIUM/LOW |
| **Edge Source** | Manual | FootyMetrics/Referee/H2H |

### Por Día:
| Métrica | Fórmula | Objetivo |
|---------|---------|----------|
| **Yield** | (Ganancia / Apostado) × 100 | > 5% |
| **Win Rate** | (Wins / Total) × 100 | > 55% |
| **Avg Odds** | Suma / Count | 1.80-2.50 |
| **Avg EV** | Suma / Count | > 0.05 |

### Por Semana:
| Métrica | Fórmula | Objetivo |
|---------|---------|----------|
| **ROI** | (Net Profit / Inversión) × 100 | > 10% |
| **Max Drawdown** | Máxima caída | < 20% |
| **Sharpe Ratio** | (Return - Rf) / StdDev | > 1.0 |

---

## 🎯 PLAN DE IMPLEMENTACIÓN

### Fase 1: Scraping (3 días)
- [ ] Crear scraper de Trends
- [ ] Crear scraper de Referee Stats
- [ ] Crear scraper de H2H Stats
- [ ] Crear scraper de Fixture Scout

### Fase 2: Análisis (3 días)
- [ ] Crear Value Bet Calculator
- [ ] Crear Kelly Criterion Calculator
- [ ] Crear Bet Tracker
- [ ] Crear Metrics Dashboard

### Fase 3: Integración (3 días)
- [ ] Integrar con API existente
- [ ] Crear flujo automatizado
- [ ] Paper testing (sin dinero real)
- [ ] Live testing (con dinero real)

---

## ⚠️ LIMITACIONES CONOCIDAS

### API Local (localhost:5272):
- Max 3000 chars por request
- SPAs no funcionan bien (requiere JavaScript)
- Algunos endpoints dan 400 errors

### Soluciones:
1. **Usar Playwright** para scraping completo
2. **Cache local** para no repetir requests
3. **Rate limiting** para no bloquear

---

## 📚 RECURSOS

### Documentación:
- https://footymetrics.com/docs
- https://github.com/georgedouzas/sports-betting

### Tutoriales:
- "Web Scraping with C#" - Microsoft Docs
- "Building Betting Models" - Pinnacle
- "Value Betting Guide" - Oddschecker

---

**Conclusión:** FootyMetrics es una **mina de oro** que apenas estamos explotando. Con los features de Trends, Props, Referee y H2H, podemos **triplicar** la calidad de nuestros picks.

---

**Documento creado:** 2026-09-20
**Objetivo:** Guía completa de FootyMetrics
**Estado:** Listo para implementar
