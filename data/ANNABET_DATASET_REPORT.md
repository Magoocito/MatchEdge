# AnnaBet Historical Dataset Report — Liga 1 Perú 2023-2025

## 1. Executive Summary

- **Total matches discovered:** 137
- **Matches with complete 1X2 odds:** 137 (100.0%)
- **Missing odds:** 0
- **Duplicates removed:** 0
- **Extraction errors:** 0

## 2. Matches by Season

| Season | Matches Discovered | Complete Odds | Missing | Duplicates |
|--------|-------------------|---------------|---------|------------|
| 2023 | 69 | 69 | 0 | 0 |
| 2024 | 68 | 68 | 0 | 0 |

## 3. Matches by Month

| Season | Month | Matches |
|--------|-------|---------|
| 2023 | 2023-02 | 69 |
| 2024 | 2024-09 | 14 |
| 2024 | 2024-10 | 46 |
| 2024 | 2024-11 | 8 |

### Missing Months Analysis

- **2023:** Data from 2023-02 to 2023-02
  - **No gaps detected**
- **2024:** Data from 2024-09 to 2024-11
  - **No gaps detected**

## 4. Coverage Analysis

### Overall Coverage

- **Total expected matches (approx):** 306 per season × 3 seasons = ~918
- **Actual matches discovered:** 137
- **Coverage:** 14.9%

### Odds Completeness

- **Complete 1X2 odds:** 137 (100.0%)
- **Missing odds:** 0 (0.0%)

## 5. Duplicates

- **Duplicate records detected:** 0

## 6. Data Quality Issues

### Odds <= 1
- **Count:** 0

### Empty Odds
- **Count:** 0

### Anomalous Odds (> 50 for any outcome)
- **Count:** 7

### Detailed Anomalies

- Anomalous odds: 2023-02-11 Cienciano vs ADT de Tarma (1.03/25.05/47.95)
- Anomalous odds: 2023-02-18 Cienciano vs Sport Boys (1.03/25.05/47.95)
- Anomalous odds: 2023-02-25 Cienciano vs ADT de Tarma (1.03/25.05/47.95)
- Anomalous odds: 2023-02-28 Cienciano vs Deportivo Garcilaso (1.03/25.05/47.95)
- Anomalous odds: 2024-10-06 Cienciano vs Deportivo Garcilaso (1.03/25.05/47.95)
- Anomalous odds: 2024-10-23 Cienciano vs Alianza Atlético (1.03/25.05/47.95)
- Anomalous odds: 2024-10-31 Cienciano vs Unión Comercio (1.03/25.05/47.95)

## 7. Odds Semantics

### Known Information

- **Source:** AnnaBet.com
- **Label:** "Cuotas 1x2" (generic, no brand)
- **Bookmaker:** NOT specified
- **Timestamp:** NOT available
- **Opening/Closing:** Cannot be determined

### Assessment

**OddsType = Unknown**

The odds displayed on AnnaBet cannot be definitively classified as:
- Pre-match closing odds
- Average across bookmakers
- Best available odds
- Real-time fluctuating odds

### Recommendation

For Model A validation purposes, treat these as **"market consensus odds"**.
Do NOT implement CLV tracking until source semantics are clarified.

## 8. Limitations

1. **Odds semantics unknown** — Cannot implement true CLV
2. **No bookmaker identification** — Cannot compare across sources
3. **No timestamp** — Cannot determine when odds were captured
4. **Potential rate limiting** — Extraction used 1.5s delays
5. **Monthly page availability** — Some months may not exist
6. **Team name variations** — Names may be truncated in HTML

## 9. File Information

- **CSV file:** annabet-historical-odds-2023-2025.csv
- **Total records:** 137
- **Generated:** 2026-09-05 12:31:02 UTC

## 10. Next Steps

1. Validate 10+ records manually against AnnaBet website
2. Cross-reference with SofaScore match outcomes
3. Integrate with MatchEdge backtesting pipeline
4. Investigate odds semantics with AnnaBet (if possible)
5. Assess feasibility of CLV tracking
