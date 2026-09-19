# Backtest 2023-2025 — Dataset Limpio y Auditado

**Fecha:** 2026-09-06 (actualizado)
**Datos:** FootyStats verified CSVs (2023: 327, 2024: 356, 2025: 333 post-limpieza)
**Gamma:** 1.6387 (ratio goles 2023+2024)
**Total partidos:** 893 (6 skipped por equipos nuevos sin historial)
**Cuotas mercado matcheadas:** 507 de 893 (56.8%)

## Resumen

| Métrica | Modelo A (Baseline) | Modelo B1 (Split s/gamma) | Modelo B2 (Split c/gamma) | Mercado |
|---|---|---|---|---|
| **Brier Score** | 0.5834 | 0.5848 | 0.6358 | **0.5628** |
| **Log Loss** | 0.9826 | 0.9835 | 1.0543 | **0.9521** |
| **ECE (calibración)** | 0.0660 | **0.0404** | 0.1196 | — |
| **N matches** | 893 | 893 | 893 | 507 |

## Bootstrap Pareado (N=1000, seed=42)

| Comparación | n | Diff Brier | 95% CI | Incluye 0 | Conclusión |
|---|---|---|---|---|---|
| A vs B1 | 804 | -0.0016 | [-0.0107, 0.0073] | SÍ | Sin diferencia significativa |
| A vs Mercado | 507 | +0.0281 | [+0.0145, 0.0409] | NO | Mercado significativamente mejor |
| B1 vs Mercado | 507 | +0.0292 | [+0.0151, 0.0424] | NO | Mercado significativamente mejor |

## Split por tipo de cálculo

### Modelo A

| Segmento | Brier | LogLoss | N |
|---|---|---|---|
| Split only (≥5 partidos) | 0.5803 | 0.9778 | 804 |
| Fallback (<5 partidos) | 0.6114 | 1.0252 | 89 |

### Modelo B1

| Segmento | Brier | LogLoss | N |
|---|---|---|---|
| Split only | 0.5819 | 0.9789 | 804 |
| Fallback | 0.6114 | 1.0252 | 89 |

## Calibración por outcome (Modelo A)

| Outcome | ECE | Brier |
|---|---|---|
| Home Win | 0.0911 | 0.2257 |
| Draw | 0.0570 | 0.1919 |
| Away Win | 0.0499 | 0.1658 |

## Calibración por outcome (Modelo B1)

| Outcome | ECE | Brier |
|---|---|---|
| Home Win | 0.0555 | 0.2280 |
| Draw | **0.0163** | 0.1900 |
| Away Win | 0.0494 | 0.1668 |

## Cambios desde versión anterior

1. **Carlos Manucci 2025 excluido** — 10 partidos eliminados (relegado a Liga 2)
2. **Real Garcilaso corregido** — Ahora mapea a Cusco FC (ID 63760), no a Deportivo Garcilaso (ID 458584)
3. **Matching mejorado** — 507 cuotas matcheadas (56.8%) vs 444 (49.7%) antes
4. **Dataset limpio** — Re-importado desde cero, sin duplicados

## Conclusiones

1. **El mercado sigue siendo superior** a todos los modelos (Brier 0.5628 vs 0.5834 de A)
2. **Modelo A y B1 son equivalentes** (ΔBrier = 0.0016, IC 95% incluye 0)
3. **B2 descartado** — ~10% peor que A/B1
4. **Matching mejorado** de 49.7% a 56.8% gracias a corrección de Real Garcilaso → Cusco FC
5. **50.3% de cuotas sin matchear** se debe a:
   - Equipos nuevos (Cantolao, Deportivo Municipal, UCV Moquegua)
   - Diferencias menores en fechas entre fuentes
   - Carlos Manucci excluido (relegado)
