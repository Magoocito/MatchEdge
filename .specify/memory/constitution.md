<!--
Sync Impact Report
- Version change: (template placeholders) → 1.0.0
- Modified principles: all template placeholders replaced with MatchEdge principles 1–20
- Added sections: Core Principles (20), Principio rector, Governance
- Removed sections: template example comments and unused placeholder slots
- Follow-up TODOs: none
-->

# MatchEdge Engineering Constitution

**Status:** Active
**Scope:** Todo desarrollo nuevo, refactor, investigación técnica y cambio arquitectónico de MatchEdge.

## Core Principles

### 1. Evidence First

MatchEdge debe distinguir siempre entre:

```text
RAW DATA
→ OBSERVATION
→ DERIVED FEATURE
→ MODEL OUTPUT
→ MARKET DATA
→ DECISION
```

Ninguna capa puede presentarse como otra.

Los datos originales obtenidos de una fuente deben conservar procedencia, timestamp y contexto suficientes para reconstruir posteriormente el análisis.

Una métrica derivada por MatchEdge debe poder diferenciarse de un dato recibido directamente de FootyMetrics u otra fuente.

#### Regla

Nunca inventar, completar silenciosamente ni asumir un dato faltante.

Cuando el dato no exista o no pueda verificarse, utilizar un estado explícito equivalente a:

`NO_DATA`, `UNKNOWN`, `UNAVAILABLE` o el contrato específico definido por la feature.

### 2. Statistical Honesty

Una frecuencia histórica no es automáticamente una probabilidad futura.

Ejemplo:

```text
8 hits / 10 observations
```

significa únicamente que la condición fue observada 8 veces dentro de esa muestra.

No implica automáticamente:

```text
P(evento futuro) = 0.80
```

Cualquier componente que produzca:

```text
probability
fair odds
edge
expected value
candidate ranking
staking
```

debe declarar explícitamente de dónde proviene la estimación y cuál es su estado de validación.

Los modelos deben clasificarse explícitamente como:

```text
EXPERIMENTAL
VALIDATION_IN_PROGRESS
VALIDATED
DEPRECATED
```

No se puede utilizar la palabra `validated` sin evidencia de evaluación fuera de muestra apropiada.

### 3. Temporal Integrity

Todo análisis pre-match debe utilizar exclusivamente información disponible antes del kickoff.

Debe existir separación explícita entre:

```text
PRE_MATCH_DATA
POST_MATCH_OUTCOME
POST_MATCH_ANALYSIS
```

Una captura obtenida después del kickoff no puede incorporarse silenciosamente a un análisis presentado como pre-match.

Cuando no pueda verificarse temporalmente una entrada crítica, el comportamiento preferido será `fail-closed`.

La integridad temporal tiene prioridad sobre producir un resultado.

### 4. Auditability and Reproducibility

Un análisis relevante debe poder responder:

```text
¿Qué información existía?
¿Cuándo fue capturada?
¿Qué transformación se realizó?
¿Qué versión del algoritmo la procesó?
¿Qué resultado produjo?
```

Los cálculos derivados deben ser reproducibles utilizando los datos persistidos y la misma versión del algoritmo.

Siempre que sea razonable, una operación determinista debe producir el mismo resultado ante la misma entrada.

Los artefactos de evidencia son parte del producto técnico, no documentación secundaria.

### 5. Specification Before Implementation

Toda modificación de comportamiento no trivial debe comenzar por una especificación.

El flujo estándar será:

```text
CONSTITUTION
→ SPECIFY
→ PLAN
→ TASKS
→ IMPLEMENT
→ VERIFY
→ CONVERGE
```

La SPEC define principalmente:

```text
WHY
WHAT
BEHAVIOR
CONSTRAINTS
ACCEPTANCE CRITERIA
NON-GOALS
```

El PLAN define:

```text
HOW
ARCHITECTURE
COMPONENTS
DEPENDENCIES
TEST STRATEGY
MIGRATION
```

Las TASKS representan unidades pequeñas y ejecutables.

Cursor no debe comenzar implementación mientras una tarea relevante permanezca en estado `NOT_READY`.

### 6. Small, Reviewable Changes

MatchEdge adopta cambios pequeños y verificables como unidad normal de evolución.

Cada PR debe tener una intención lógica principal.

El límite normal de trabajo en progreso es:

```text
WIP = 1
```

Si una tarea contiene varios cambios independientes debe dividirse.

Un PR no debe ampliarse silenciosamente debido a oportunidades encontradas durante la implementación.

Cuando aparezca trabajo adicional:

```text
documentar
→ clasificar
→ crear deuda/PBI
→ continuar con el alcance original
```

salvo que el nuevo hallazgo sea necesario para garantizar correctness o seguridad del cambio actual.

### 7. Explicit Architecture

Las dependencias importantes deben ser explícitas.

Preferir:

```text
dependency injection
clear interfaces
small cohesive services
explicit contracts
```

sobre:

```text
hidden dependencies
service construction inside business logic
global mutable state
large multi-purpose classes
```

Una clase debe tener una responsabilidad cohesiva y un número limitado de razones para cambiar.

El tamaño de una clase es una señal para investigar, no una justificación automática para dividirla.

Los refactors deben responder a problemas concretos de cohesión, testabilidad, acoplamiento o evolución.

### 8. Opportunistic Refactoring

MatchEdge no realizará refactors masivos sin necesidad funcional.

Aplicaremos la regla:

> Leave the code you touch slightly better than you found it.

Antes de modificar una región compleja:

```text
1. comprender comportamiento actual
2. crear characterization tests cuando sean necesarios
3. realizar refactor mínimo
4. verificar comportamiento equivalente
5. implementar la nueva feature
```

Feature y refactor deben poder distinguirse conceptualmente aunque puedan formar parte del mismo ciclo.

Nunca mezclar una modificación de comportamiento con un refactor grande sin pruebas que permitan distinguir ambos efectos.

### 9. Testing as Evidence

Los tests son evidencia ejecutable del contrato.

Cada feature debe definir qué nivel de prueba necesita:

```text
unit
integration
regression
manual verification
data verification
```

No todos los cambios necesitan todos los niveles.

Un test debe proteger comportamiento relevante, no únicamente aumentar un contador.

Todo fallo encontrado durante un cambio debe clasificarse como:

```text
INTRODUCED_BY_CHANGE
PREEXISTING
UNKNOWN
```

`PREEXISTING` requiere evidencia.

Los tests relevantes y el build deben ejecutarse antes del merge.

La meta es trasladar gradualmente estos controles a CI para que GitHub actúe como quality gate independiente del agente que escribió el código.

### 10. Fail Closed on Critical Uncertainty

Cuando una incertidumbre pueda invalidar el significado de un análisis, MatchEdge debe preferir no producir dicho análisis.

Ejemplos:

```text
kickoff no verificable
fuente temporal ambigua
datos insuficientes
modelo inexistente para un mercado
mapping no confirmado
```

No generar valores sintéticos para continuar el pipeline.

La ausencia de resultado debe ser observable y diagnosticable.

### 11. Observability Without Hiding Failures

Un fallback funcional no debe ocultar el fallo técnico que lo causó.

Es válido devolver:

```text
SIN DATA VERIFICADA
```

al consumidor.

Pero internamente MatchEdge debería distinguir cuando sea relevante:

```text
FILE_NOT_FOUND
PARSE_ERROR
SOURCE_CHANGED
PERMISSION_ERROR
TIMEOUT
UNEXPECTED_ERROR
```

Los errores recuperables deben generar evidencia suficiente para diagnóstico sin contaminar la salida del usuario.

### 12. Source and Model Boundaries

Las responsabilidades conceptuales deben permanecer separadas.

```text
SOURCE ACQUISITION
        ↓
RAW PERSISTENCE
        ↓
NORMALIZATION
        ↓
FEATURE GENERATION
        ↓
MODEL
        ↓
MARKET COMPARISON
        ↓
REPORTING
```

Un renderer no debe decidir lógica de modelo.

Un scraper no debe decidir candidatos.

Una capa de persistencia no debe decidir semántica estadística.

Un controller debe principalmente coordinar contratos HTTP y delegar comportamiento.

Las excepciones existentes se tratarán como deuda técnica, no como patrón para nuevas implementaciones.

### 13. Explicit Model Validation

La existencia de código predictivo no implica que exista un modelo validado.

Para cambiar el estado de un modelo a `VALIDATED` deberá existir como mínimo una SPEC independiente que defina:

```text
target
dataset
feature cutoff
training period
validation period
test period
metrics
baselines
leakage controls
calibration evaluation
acceptance criteria
```

La evaluación debe respetar orden temporal.

La misma información utilizada para seleccionar o ajustar una estrategia no debe presentarse posteriormente como prueba independiente de su rendimiento.

### 14. Documentation as Current State

MatchEdge mantendrá pocas fuentes de documentación operativa.

#### `docs/VISION.md`

Describe la dirección conceptual del producto.

#### `docs/STATE.md`

Describe lo que existe realmente en `main`.

No es histórico ni aspiracional.

#### `docs/adr/`

Registra decisiones arquitectónicas relevantes y su razonamiento.

#### Specifications

Describen cambios concretos mediante el flujo SDD.

Si código y `STATE.md` divergen, se considera deuda que debe corregirse.

Si código y `VISION.md` divergen, se considera `ARCHITECTURE_DRIFT` y requiere una decisión explícita.

Nunca resolver una contradicción conceptual simplemente modificando documentación para que coincida con el código.

### 15. Token and Agent Efficiency

El contexto proporcionado a un agente debe ser mínimo pero suficiente.

Cursor deberá recibir normalmente:

```text
GOAL
SPEC
CURRENT TASK
RELEVANT FILES
ACCEPTANCE CRITERIA
TESTS
DO NOT TOUCH
EXPECTED OUTPUT
```

Evitar instrucciones como:

```text
read the entire repository
```

salvo una auditoría explícitamente destinada a ello.

No imprimir archivos grandes completos cuando basta con señalar rutas o fragmentos.

No repetir análisis ya persistido en documentación vigente.

El agente debe investigar archivos adicionales únicamente cuando pueda explicar por qué son necesarios para resolver la tarea actual.

### 16. Human Control of Architectural Decisions

Los agentes pueden:

```text
investigar
diagnosticar
proponer alternativas
implementar decisiones aprobadas
ejecutar tests
generar evidencia
```

Las decisiones arquitectónicas relevantes deben permanecer explícitas.

Cuando existan varias alternativas razonables con trade-offs significativos:

```text
ANALYZE
→ RECOMMEND
→ STOP
```

antes de realizar un cambio estructural irreversible o difícil de revertir.

### 17. Definition of Ready

Una tarea está `READY` cuando conocemos:

```text
problem
goal
scope
acceptance criteria
non-goals
relevant dependencies
test strategy
known risks
```

Si falta información esencial:

`NOT_READY`

La implementación no debe utilizarse para descubrir cuál era realmente el requisito.

### 18. Definition of Done

Una tarea se considera terminada únicamente cuando:

```text
spec acceptance criteria satisfied
build passes
relevant tests pass
regression checked
temporal integrity preserved
statistical assumptions explicit
no silent architecture drift
new technical debt documented
required documentation updated
implementation matches specification
```

`Code written` no significa `Done`.

### 19. SDD Convergence

Después de implementar una feature se realizará una revisión final:

```text
SPEC
↕
IMPLEMENTATION
```

Debe comprobarse:

```text
¿Se implementó todo lo especificado?
¿Se implementó algo que no estaba especificado?
¿Cambió alguna premisa?
¿Los acceptance criteria siguen siendo correctos?
¿Apareció deuda nueva?
¿STATE refleja la realidad?
```

Cuando implementación y especificación difieran, no se debe corregir automáticamente una para hacer coincidir a la otra.

Primero se determina cuál representa la decisión correcta.

### 20. Governance

Esta Constitution tiene prioridad sobre prompts individuales, briefs de implementación y decisiones temporales de un agente.

Una SPEC puede añadir restricciones específicas, pero no contradecir esta Constitution silenciosamente.

Las modificaciones a esta Constitution requieren:

```text
motivo explícito
impacto
revisión
actualización de versión
```

Versionado:

```text
PATCH
clarificación sin cambio de principio

MINOR
nuevo principio o ampliación compatible

MAJOR
cambio o eliminación de una regla fundamental
```

Toda SPEC futura debe declarar:

`Constitution Check: PASS / FAIL`

antes de pasar de PLAN a IMPLEMENT.

## Principio rector

MatchEdge debe preferir:

```text
correctness
→ evidence
→ reproducibility
→ maintainability
→ speed
```

La velocidad es valiosa únicamente cuando preserva las cuatro anteriores.

## Governance

Esta Constitution tiene prioridad sobre prompts individuales, briefs de implementación y decisiones temporales de un agente.

Una SPEC puede añadir restricciones específicas, pero no contradecir esta Constitution silenciosamente.

Las modificaciones a esta Constitution requieren:

```text
motivo explícito
impacto
revisión
actualización de versión
```

Versionado:

```text
PATCH
clarificación sin cambio de principio

MINOR
nuevo principio o ampliación compatible

MAJOR
cambio o eliminación de una regla fundamental
```

Toda SPEC futura debe declarar:

`Constitution Check: PASS / FAIL`

antes de pasar de PLAN a IMPLEMENT.

**Version**: 1.0.0 | **Ratified**: 2026-10-01 | **Last Amended**: 2026-10-01
