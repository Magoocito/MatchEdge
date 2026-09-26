# ADR-001 - Scoring Weights del Confluence Report

> Fecha: 2026-09-26
> Estado: DRAFT - Debe ser escrito por Daniel, no por IA
> Relacionado: FmConfluenceReportBuilder.cs, PBI 3.1

## Contexto
Claude definió 35% HitRate + 20% Odds + 20% League Quality + 25% Completeness sin explicación. Necesitamos ownership.

## Decisión
Definir por qué:
- HitRate 35% y no 50%: ¿por qué no todo es hit rate? ¿qué pasa si FootyMetrics tiene sesgo en home/away?
- Odds 20%: ¿por qué 1/odds es solo descriptivo y no probabilidad? P4-P7 lo prohíbe convertir hit rate en probabilidad.
- League Quality 20%: ¿por qué Nations League vale más que liga doméstica? ¿datos?
- Completeness 25%: ¿por qué premiar señales con n ≥ 10 y sin source_conflict?

## Consecuencias
- Si cambiamos weights, ¿cómo afecta a lista operable en tmp/fm/p8_e_operable.json?
- Si baja de 0.95 el Kelly cap, ¿qué pasa con MaxDailyExposure?

## Evidencia
- Link a GET /report con 2 fixtures Nations League + 1 liga doméstica
- own_windows.all.n ≥ 10 validación

## Autor
Daniel Delgado - Debe explicar con sus palabras por qué cap 0.95 y no 1.0
