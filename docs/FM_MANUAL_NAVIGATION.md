# FM_MANUAL_NAVIGATION — Guía de navegación manual en FootyMetrics (para el usuario)

Objetivo: que el usuario navegue FootyMetrics **como usuario humano** y capture evidencia JSON
que MatchEdge contrasta después con lo que la app obtiene automáticamente.
Esta guía no automatiza nada: es la vía manual complementaria (P5-D).

> Corrección de referencia: el fixture **33441811 = Portugal vs Wales** (UEFA Nations League,
> 2026-09-24, slug `33441811-uefa-nations-league-portugal-wales`, Home=Away confirmado en la web).
> El cruce **Noruega vs Portugal** es el fixture **33608044** (2026-09-27, `33608044-uefa-nations-league-norway-portugal`).
> Ambos se usan abajo como ejemplos.

---

## 1. Cómo capturar un payload (DevTools)

1. Abrir la página en Chrome con la sesión FM Premium activa.
2. `F12` → pestaña **Network**.
3. En el filtro escribir `fetch` o `XHR` (o `api/front` para ver solo las APIs de FM).
4. Interactuar con la página (cambiar pestaña, Location, Period, Statistic…).
5. Clic derecho sobre la respuesta relevante → **Copy → Copy response**.
6. Guardar el texto en `tmp/fm/` con nombre descriptivo:
   `tmp/fm/manual_trends_33441811.json`, `tmp/fm/manual_table_portugal_home.json`, etc.
7. Anotar junto al archivo: URL exacta + hora UTC aproximada.

Alternativa sin DevTools: en la consola del navegador,
`copy(await (await fetch('/api/front/teams/table?stat=corners&id=18701&period=15&location=home&group=attack&dl=true&sm=easier')).text())`
y pegar en el archivo. (El fetch manual solo sirve para tu sesión; no lo automatizamos.)

---

## 2. URLs recomendadas y qué muestran

### 2.1 Fixture (contexto, H2H, alineaciones)
- `https://www.footymetrics.com/fixtures/33441811-uefa-nations-league-portugal-wales` → Overview: H2H, alineaciones, estadísticas del partido.
- `...?tab=team-trends` → tendencias del equipo (cuotas por casa en el panel de tendencias).
- `...?tab=player-trends` → tendencias de jugadores.
- `https://www.footymetrics.com/fixtures/33608044-uefa-nations-league-norway-portugal` → próximo Portugal (visitante).

### 2.2 Pestañas de equipo (venue histórico, jugadores, contexto)
Base: `https://www.footymetrics.com/teams/<id>-<equipo>` — ejemplo Portugal:
- `?period=15&location=home` → **Team stats / Match-by-match** con Location *Home only*.
- `?period=15&location=away` → mismo panel solo visitante. (`location=all` o sin valor → 400 en la API.)
- `?tab=player&period=15&location=home` → matriz de stats por jugador y partido.
- `?tab=positions&stat=shots-on-target&period=15&location=home` → Position stats (rankings por posición, `venue=home|away|both`).
- `?tab=schedule` → calendario (render SSR, sin API propia).
- `?tab=trends` → NO explorado todavía: si lo abres, captura el payload.
- Extras: `&team-stat=corners|goals|shots...`, `&stat=expected-goals`, `&pparam=5`, selector *Leagues (4/5)*, *Opponent difficulty*.
- Team apids conocidos: Portugal `18701`, Wales `18721` (la URL usa otro id de slug; la API usa el apid).

### 2.3 Cuotas
- Panel de tendencias del fixture (`?tab=team-trends` / `?tab=player-trends`): muestra cuotas con
  **nombre de casa** (Bet365, Kambi, Paddy Power, Ladbrokes, Altenar) — es la referencia para
  verificar el mapeo id→nombre.
- `https://www.footymetrics.com/bookmakers` → catálogo de casas.
- API de equipo: `/api/front/teams/odds?tid=18701&g=attack` → cuotas de props con `bk` (solo id).

### 2.4 Otras secciones (sin endpoint mapeado aún)
`/h2h-stats`, `/position-stats`, `/referees`, `/duels`, `/player-matchups`, `/fixture-scout`,
`/props-edges`, `/player-props-finder`, `/statistics/*`, `/leagues/<id>-<slug>?tab=fixtures`.

---

## 3. Qué buscar en cada payload

| Qué buscar | Dónde se ve en el JSON | Sirve para |
|---|---|---|
| Cuota con nombre de casa | `data[].odds[].bk` (hoy solo id) o el texto visible del panel "Kambi 1.97" | verificar mapeo `fm_bookmakers` |
| Venue histórico (local/visitante) | `history[].h` (trends), `fixtures[].hid/aid` + `location=home\|away` (teams/table), `appearances[].teamHome` (position-stats), `fixtures[].h` (teams/odds) | split home/away de la evidencia |
| Venue estructural del fixture actual | `Fixture.Home/Away.name` vs `Team.name` (trends) | `venue_role` en `fm_signal` |
| Historial fecha + rival | `history[] {t, opp.name, met, v/vt/va/vf}` | cruzar con resultados y con venue |
| Jugadores | `pivotData[playerApid][fixtureApid]`, `players[]`, `position-stats.appearances[]` | análisis de jugadores |
| Contexto/rival | `fixtures[] {timestamp, opponent, hgoals, agoals, lid}` y `opponentStrength` | forma, competición, fortaleza del rival |

---

## 4. Flujo de contraste con la app (P5-D3)

1. El usuario guarda la captura manual en `tmp/fm/manual_*.json` con la URL en el nombre.
2. OpenCode la contrasta con lo que la app obtiene automáticamente (snapshots en
   `C:\Services\MatchEdge\tmp\fm\snapshots\`, payloads en `tmp/fm/p5_*.json` y `fm_market_odds`).
3. Se reportan discrepancias en: nº de filas, valores de cuota, bandera home/away, fechas del historial,
   nombre de bookmaker y ventana (period) usada.
4. Toda discrepancia se documenta con evidencia (ambos archivos) antes de cambiar código.
