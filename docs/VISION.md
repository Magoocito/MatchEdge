# MATCHEDGE + FOOTYMETRICS — PROMPT CONCEPTUAL

## 1. PROPÓSITO

MatchEdge utiliza FootyMetrics como fuente de datos estadísticos para construir un análisis descriptivo, estructurado y auditable de un partido de fútbol.

La visión es que MatchEdge no se limite a seleccionar la "mejor tendencia" que muestra FootyMetrics, sino que examine conjuntamente la información disponible sobre:

* equipo;
* rival;
* tendencias de equipo;
* tendencias de jugadores;
* historial partido a partido;
* contexto local/visitante;
* competición;
* y, posteriormente, cuotas de mercado introducidas manualmente por el usuario.

El objetivo inmediato es construir una **base de evidencia descriptiva**, no un sistema automático de recomendaciones ni un modelo predictivo validado.

---

## 2. PRINCIPIO FUNDAMENTAL: FOOTYMETRICS NO PROPORCIONA PROBABILIDADES

Los valores mostrados por FootyMetrics, incluidos:

* Hit Rate;
* Best Window;
* Best Count / Best Total;
* Opp. Hits;
* tendencias de equipos;
* tendencias de jugadores;

deben tratarse como **observaciones históricas**, no como probabilidades futuras.

Por ejemplo:

> Over 7.5 corners — 8/10

significa que la condición ocurrió 8 veces dentro de la muestra correspondiente.

NO significa automáticamente:

> Probabilidad futura = 80%.

MatchEdge debe conservar esta distinción en toda su arquitectura, documentación, análisis y salida.

---

## 3. SELECTION BIAS DE FOOTYMETRICS

Ya se identificó que FootyMetrics puede seleccionar una ventana histórica que maximiza el hit rate.

Por tanto, una señal como:

> 7/7

puede representar una ventana seleccionada después de observar los datos disponibles.

Esta información puede conservarse como:

### FM_SIGNAL

Lo que FootyMetrics reporta directamente.

Pero no debe utilizarse como estimador probabilístico ni como evidencia independiente de MatchEdge.

MatchEdge debe conservar el `history[]` crudo y calcular sus propias ventanas analíticas fijas, independientemente de la ventana seleccionada por FootyMetrics:

* últimos 5;
* últimos 10;
* últimos 15;
* últimos 20.

Estas ventanas deben considerarse **MATCHEDGE_EVIDENCE**.

La finalidad es poder estudiar la estabilidad de una señal en diferentes horizontes y no depender exclusivamente de la ventana optimizada de FootyMetrics.

---

## 4. OPP. HITS / OPPONENT TREND NO ES LO MISMO QUE CUOTA

Debe existir una separación conceptual estricta entre dos conceptos.

### Opp. Hits / Opponent Trend

Es información estadística relacionada con el comportamiento del rival dentro de FootyMetrics.

Ejemplo conceptual:

* Equipo A genera frecuentemente más de 7.5 córners.
* El rival concede frecuentemente más de 7.5 córners.

Esto representa una posible **confluencia estadística entre el comportamiento del equipo y el comportamiento del rival**.

No debe denominarse simplemente "odds" ni confundirse con una cuota de mercado.

### Cuota del mercado

Es un dato externo que el usuario consulta manualmente en el bookmaker de su elección y proporciona posteriormente a MatchEdge.

La cuota:

* NO se obtiene automáticamente;
* NO forma parte de la extracción actual de FootyMetrics;
* NO requiere estudiar la infraestructura del bookmaker;
* NO debe atribuirse a FootyMetrics.

Su función futura será permitir comparar la evidencia estadística de MatchEdge con el precio observado manualmente en el mercado.

---

## 5. ANÁLISIS 360° DESCRIPTIVO

La visión futura de MatchEdge es analizar un partido como un conjunto de evidencias relacionadas.

Ejemplo conceptual:

### Equipo

* tendencia histórica de córners;
* tiros;
* tiros a puerta;
* goles;
* tarjetas;
* otros mercados disponibles;
* ventanas propias 5/10/15/20.

### Rival

* comportamiento histórico equivalente;
* Opp. Hits / Opponent Trend;
* capacidad de conceder determinadas estadísticas;
* ventanas propias 5/10/15/20.

### Jugadores

* Player Trends disponibles;
* historial correspondiente;
* contexto de participación cuando esté disponible.

### Contexto

* local/visitante;
* competición;
* rival;
* contexto temporal;
* cualquier otra información disponible antes del kickoff.

### Mercado

Posteriormente:

* mercado observado;
* línea;
* cuota introducida manualmente por el usuario.

El sistema debe describir cómo convergen o divergen estas evidencias.

---

## 6. CONFLUENCIA NO SIGNIFICA CONFIRMACIÓN PREDICTIVA

Una señal debe poder analizarse desde diferentes dimensiones.

Por ejemplo:

```text
Equipo:
Over 7.5 corners
7/10

Rival:
concede Over 7.5 corners
8/10

Ventana 5:
5/5

Ventana 10:
7/10

Ventana 15:
10/15

Ventana 20:
13/20
```

Esto puede describirse como una **confluencia de evidencia histórica**.

Pero MatchEdge no debe transformar automáticamente esa confluencia en:

* probabilidad futura;
* edge;
* valor esperado;
* recomendación;
* pick.

La función del sistema en esta etapa es describir la evidencia, su consistencia, sus limitaciones y las posibles contradicciones.

---

## 7. DESCRIPTIVO VS. PREDICTIVO

MatchEdge debe distinguir explícitamente entre:

### DESCRIPTIVO

Lo que ocurrió históricamente.

Ejemplo:

> En los últimos 10 partidos, la condición ocurrió 7 veces.

### PREDICTIVO

Una estimación sobre lo que ocurrirá en un partido futuro.

Actualmente MatchEdge **NO dispone todavía de un modelo predictivo validado**.

Por tanto, no debe presentar una frecuencia histórica como una predicción.

Cualquier futura capacidad predictiva deberá desarrollarse y validarse de forma independiente utilizando datos históricos, separación temporal y evaluación fuera de muestra.

---

## 8. DATOS CRUDOS Y AUDITABILIDAD

Siempre debe conservarse la información original obtenida de FootyMetrics, incluyendo:

* `history[]`;
* timestamp de origen;
* información necesaria para reconstruir las señales;
* contexto de la extracción.

Las métricas derivadas por MatchEdge deben poder distinguirse de los valores originales de FootyMetrics.

La arquitectura debe permitir responder:

> ¿Qué mostraba FootyMetrics en ese momento?

y:

> ¿Qué cálculo hizo MatchEdge a partir de esos datos?

Esto es necesario para poder auditar posteriormente cualquier análisis.

---

## 9. CONTROL TEMPORAL Y DATA LEAKAGE

Para analizar un partido futuro, MatchEdge debe utilizar únicamente información que estuviera disponible **antes del kickoff**.

No debe utilizar información posterior al partido para construir las señales o características utilizadas en el análisis previo.

Debe mantenerse una separación temporal clara entre:

* información disponible antes del partido;
* resultado posterior;
* información utilizada posteriormente para evaluar el análisis.

---

## 10. ESTADO ACTUAL Y PRIORIDAD

Antes de construir el análisis 360°, debe cerrarse la cola de validación de Fase 2.

Estado actual conocido:

* 70 RESOLVED;
* 108 UNAVAILABLE;
* 1 AMBIGUOUS.

Problemas identificados:

* `INSERT OR IGNORE` bloqueando determinados reintentos;
* ausencia de `fixtureId` en determinados elementos de `history[]`;
* matching basado en información disponible como fecha + rival;
* contradicciones internas en determinados datos de FootyMetrics;
* selection bias derivado de las ventanas seleccionadas por FootyMetrics.

La prioridad es resolver y documentar estos problemas antes de utilizar el dataset como base del análisis posterior.

En particular, debe distinguirse entre:

* datos realmente no disponibles;
* problemas temporales de ingesta;
* problemas de matching;
* datos ambiguos;
* contradicciones de origen.

No debe asumirse que todos los `UNAVAILABLE` tienen la misma causa.

---

## 11. VENTANAS PROPIAS DE MATCHEDGE

Una vez validado el `history[]`, MatchEdge debe calcular independientemente sus ventanas:

* 5;
* 10;
* 15;
* 20.

Estas ventanas no deben depender de `bestCount/bestTotal` ni de la ventana que FootyMetrics haya seleccionado.

La finalidad es poder estudiar:

* consistencia;
* estabilidad;
* sensibilidad al tamaño de muestra;
* comportamiento reciente frente a histórico más amplio.

No se debe asumir previamente que una determinada ventana es superior a otra.

---

## 12. FUTURA COMPARACIÓN CON CUOTAS

Las cuotas del mercado serán una entrada **manual** del usuario.

Conceptualmente:

```text
FootyMetrics
     ↓
raw history[]
     ↓
MatchEdge Evidence
     ↓
análisis equipo + rival + jugadores + contexto
     ↓
resultado descriptivo
     ↓
        + cuota manual del usuario
     ↓
comparación posterior
```

La infraestructura de MatchEdge no debe asumir scraping, automatización, API ni extracción automática de cuotas.

En esta etapa, la cuota es simplemente una variable externa proporcionada manualmente para permitir análisis posteriores.

---

## 13. MODELO PREDICTIVO

Actualmente **NO existe un modelo predictivo validado**.

Por tanto:

* no convertir frecuencias en probabilidades;
* no calcular probabilidades "reales";
* no afirmar que una señal tiene capacidad predictiva sin validación;
* no calcular edge como si existiera una probabilidad validada;
* no producir recomendaciones automáticas.

La evolución futura puede incluir modelos predictivos, pero solamente después de disponer de un dataset limpio, características bien definidas y evaluación temporal/out-of-sample adecuada.

---

## 14. PRINCIPIO FINAL

MatchEdge debe evolucionar desde:

```text
"FootyMetrics muestra una tendencia fuerte"
```

hacia:

```text
"MatchEdge ha reunido y contrastado múltiples evidencias
históricas independientes de la ventana optimizada de FootyMetrics,
ha identificado sus coincidencias, divergencias, limitaciones y contexto,
y presenta el resultado de forma auditable."
```

La inteligencia del sistema debe surgir del **análisis conjunto y verificable de los datos**, no de tratar una estadística aislada de FootyMetrics como una predicción.

