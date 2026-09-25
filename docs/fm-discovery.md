# FM Discovery — Fase 0 (feature/fm-deterministic-navigation)

Ejecución: 24/09/2026. Navegaciones usadas: 7/40. Fixtures abiertos: 1/3 (33441813).
Evidencias crudas en `tmp/fm/` (gitignored). Nunca se imprimió HTML completo en conversación.

## D1 — Pantalla/URL de fixtures por liga y fecha

**RESPUESTA:** Página `https://www.footymetrics.com/leagues/<id>-<slug>?tab=fixtures`.
Los datos llegan por XHR JSON: `GET /api/front/fixtures/league?id=<leagueApid>` →
`[{date, fixtures:[...]}]` con 8 fechas. Los enlaces de fixture SÍ son `<a href="/fixtures/<id>-<slug>">` (60 enlaces en la página).
El `id` de URL (586352) NO sirve en la API; se necesita `leagueApid` (1538).

**EVIDENCIA** (`tmp/fm/d1_intercept.json`, `tmp/fm/d1_dom.json`, `tmp/fm/d1_api_raw.json`, `tmp/fm/d1f_rscresolver.json`):
```
GET https://www.footymetrics.com/api/front/fixtures/league?id=1538 [200]
{"date":"2026-09-24","fixtures":[{"id":"33441813","slug":"33441813-uefa-nations-league-serbia-greece","apid":19676694,"leagueApid":1538,"timestamp":"2026-09-24T18:45:00.000Z",...,"hasTrends":true,"stageName":"League A",...}]}
a[href*="/fixtures/"] → href="/fixtures/33441813-uefa-nations-league-serbia-greece"  (text: "LEAGUE A\n2.7\n24.2\n13:45\nSerbia\nGreece\n-")
GET /api/front/leagues → data[] {id:"586352", name:"UEFA Nations League", slug:"uefa-nations-league", logo:"https://cdn.footymetrics.com/leagues/1538.webp", ...}
leagueApid extraíble de: RSC flight `"slug":"uefa-nations-league",...,"apid":1538` o regex `leagues/(\d+)\.webp` del logo (ambos verificados =1538)
id=586352 en fixtures/league → 200 con [] (vacío); id=1538 → 200 con fixtures
```
Fecha: la API devuelve todas las fechas; se filtra por clave `date` (ej. `2026-09-24`). No hay parámetro `?date=` en la URL de la liga (dateControls=[] en DOM).

## D2 — ¿`/fixtures/<id>` sin slug funciona?

**RESPUESTA:** NO. `/fixtures/33441813` → 404 "Page not found", sin redirect. Slug obligatorio.

**EVIDENCIA** (`tmp/fm/d2_noslug.json`):
```
url=https://www.footymetrics.com/fixtures/33441813
title=Football Player Stats & Betting Trends | FootyMetrics
"Page not found\nThe page you're looking for has moved, been removed, or never existed."
```

## D3 — Origen de datos: JSON vs incrustado

**RESPUESTA:** JSON por XHR/fetch (no GraphQL, no `__NEXT_DATA__` — nextData:false; sí RSC flight de Next.js). Endpoints:

| Método | Endpoint | Claves del payload |
|---|---|---|
| GET | `/api/front/leagues` | `{type, data[]:{id,name,slug,logo,logoDark,isTop,topOrder,order,fixturesCount,Country}}` |
| GET | `/api/front/fixtures/league?id=<apid>` | `[{date, fixtures[]:{id,slug,apid,leagueApid,seasonApid,refereeApid,timestamp,state,status,homeGoals,awayGoals,lineupPredicted,lineupConfirmed,neutralVenue,homePos,awayPos,hasTrends,stageName,odd_*,Home,Away,bestTip,ts,referee}}]` — Home/Away llegan `null` |
| GET | `/api/front/trends/fixtures/<apid>/players?<params>` | `{data[], pagination:{pages,per_page,current,count,total_pages}}` |
| GET | `/api/front/trends/fixtures/<apid>/teams?<params>` | `{data[], pagination}` |
| GET | `/api/front/fixtures/<apid>/lineup\|lineup-status\|sidelined` | (no usados en Fase 1) |
| RSC | `/fixtures/<slug>` con header `RSC:1` | flight con `"Home":{"apid","name","code","slug","logo",...}` y `"Away":{...}` |

**EVIDENCIA** (`tmp/fm/d3_player_intercept.json`, `tmp/fm/d4_players_payload.json`):
```
GET /api/front/trends/fixtures/19676694/players?page=1&sort=hr-h&min_hit=60&min_matches=3&...&league_only=false [200]
topKeys: data, pagination
player itemKeys: id,timestamp,shortName,slug,photo,pos,line,direction,key,market,vt,va,percentage,bestCount,bestTotal,history,Fixture,Team,lineup,odds,score,loc,apid,rf,rr
team itemKeys: id,timestamp,name,slug,logo,line,direction,key,type,party,market,vt,va,percentage,bestCount,oppHitRate,oppCount,oppTotal,h2hCount,h2hTotal,h2hLastAt,bestTotal,history,isNational,Fixture,Team,odds,loc,apid
```

## D4 — ¿Valores partido a partido por trend?

**RESPUESTA:** SÍ. En `data[].history[]` (10 entradas): player `history{k,h,l{logo,name},m,p,t,v,la,vo,met,opp{code,logo,name},vs,yc}`; team `history{h,l,t,va,vf,vt,met,opp}`.
Conteos: `bestCount`/`bestTotal` (hits/muestra), `percentage` (hit rate), teams: además `oppHitRate,oppCount,oppTotal,h2hCount,h2hTotal`.

**EVIDENCIA** (`tmp/fm/d4_players_payload.json`, `tmp/fm/d5_params.json`):
```
bestCount=10, bestTotal=10, percentage=100, historyLen=10
history sample: {"h":true,"l":{"name":"Friendly International"},"m":7,"t":"2026-03-31T16:00:00.000Z","v":1,"opp":{"name":"Saudi Arabia"}}
team history keys: h,l,t,va,vf,vt,met,opp
```

## D5 — Parámetros de venue / ventana / same league

**RESPUESTA:**
- Tab (URL de página): `?tab=player-trends | ?tab=team-trends` (Overview = sin `?tab`).
- **Venue:** parámetro de API `location` (NO es de la URL de página salvo que la SPA lo lea): `location=all` (Any venue) y `location=match` (Same venue) aceptados; `location=home|away` → 400 (enum no válido). UI Home/Away no emiten llamada nueva (filtrado client-side / refinado sobre `location=match`). La SPA SÍ lee `?location=` de la URL: `?tab=team-trends&location=home` → estado UI "Same venue"+"Home", llamada API `location=match`.
- **Ventana (5/10/15):** NO ENCONTRADO en tabs de fixture — `history` siempre 10; probes `window/last/limit/history/n/matches/venue/size` → 400.
- **Same league:** `league_only=true|false` (36→27 resultados, verificado).
- Otros params API teams: `page,sort,min_opp_hits,min_h2h_hits,min_hit,min_matches,directions,parties,markets,bookmakers,league_only,location`.
- Params API players: `page,sort,min_hit,min_matches,lineup(lines),pos,bookmakers,markets,league_only,location(desconocido-hasta-verificar en players)`.

**EVIDENCIA** (`tmp/fm/d5_team_intercept.json`, `tmp/fm/d5c_clickvenue.json`, `tmp/fm/d5e_clicks.json`, `tmp/fm/d5d_location.json`, `tmp/fm/d5i_state.json`, `tmp/fm/d5h_window.json`):
```
teams URL: /api/front/trends/fixtures/19676694/teams?page=1&sort=hr-h&min_opp_hits=0&min_h2h_hits=0&min_hit=70&min_matches=4&directions=over&parties=match%2Cteam&markets=...&bookmakers=...&league_only=false
click "Any venue" → ...&league_only=false&location=all
click "Same venue" → ...&location=match  (count=0)
location=home|away|same → HTTP 400
?tab=team-trends&location=home → UI aria: Same venue=true, Home=true; API llamada location=match
league_only=true → count 27 (base 36)
window/last/limit/history/n/matches/venue/size =5 → 400
```

## D6 — Señal de "página lista" por pestaña

**RESPUESTA:**
- **player-trends / team-trends:** señal fiable = respuesta de red `GET /api/front/trends/fixtures/<apid>/(players|teams)` status 200 con JSON `data[]` (+ `pagination`). Fallback DOM: contenedor grid `div.w-full.grid.grid-cols-1` con hijos > 0 y nodos texto `Opp. hits` ≥ 1.
- **Overview:** `h1` presente (`"…lineups, H2H results & statistics"`) + `script[type="application/ld+json"]` con `@type=SportsEvent` + botón Overview `aria-selected=true`.

**EVIDENCIA** (`tmp/fm/d6b_row.json`, `tmp/fm/d6c_overview.json`):
```
row: DIV.bg-ui-bg.rounded-xl.border.border-border.px-2.5.py-2.5.flex.flex-col.gap-2.5  (15 siblings)
list: DIV.w-full.grid.grid-cols-1.sm:grid-cols-2.gap-3  (kids=15), nodos "Opp. hits"=16
overview: h1="Serbia vs Greece lineups, H2H results & statistics", ldTypes=[...,"SportsEvent","BreadcrumbList","FAQPage"], overviewAria="true"
```

## D7 — Páginas con sesión caída / no premium / sin datos

**RESPUESTA:**
- **Sesión caída:** con sesión activa, `/login` redirige a `/` (home). Si tras navegar la URL queda en `/login` o el contenido contiene "Sign in"/"Log in" → sesión inválida (misma heurística que endpoint `/login-status` existente). No se pudo observar el estado real sin cerrar sesión (login manual prohibido).
- **No premium:** NO ENCONTRADO (no comprobable con sesión Premium activa).
- **Sin datos:** API `data:[]` + `pagination.count=0` → DOM grid vacío y texto `Showing 0`.

**EVIDENCIA** (`tmp/fm/d7_login.json`, `tmp/fm/d5e_clicks.json`, `tmp/fm/d5i_state.json`):
```
GET /login (sesión activa) → url=https://www.footymetrics.com/ (redirect home)
location=match → count=0, data=[] → showing="Showing 0"
login-status existente: loggedIn = !url.Contains("login") && !text.Contains("sign in"/"log in")
```

## Conclusión para Fase 1 (criterio del brief)

D3/D4 confirman JSON → la extracción DEBE basarse en JSON (`/api/front/trends/fixtures/<apid>/(players|teams)`), no en innerText. Los snapshots crudos se guardan desde la respuesta JSON (raw_path/sha256). El parser innerText solo queda como fallback con `parser_version` + canario.

---

## Fase P2 — Verificaciones V1–V7

### V1 — Fixtures de T1: tabla y criterio del resolver

**RESPUESTA:** Los 7 fixtures de T1 (33441811–17) son UEFA Nations League, 24/09/2026, kickoff 18:45:00Z, competicion "UEFA Nations League" (stageName League A/B/D). Criterio real del resolver: (1) leagueApid desde /api/front/leagues por match de nombre; (2) filtrado estricto `group.date == fecha pedida`; (3) dedup por fixtureId. El endpoint de liga ya NO agrupa el día 24 (solo 25+); el global sí lo conserva con state/status=FT. T1 devolvió 7 porque Andorra-Malta (16:00Z) ya había terminado a las 17:50. Sin fixtures de otra fecha/competición → sin bug.

**EVIDENCIA** (`tmp/fm/v1_unl_day.json`, `tmp/fm/v1_league_payload.json`, `tmp/fm/v1_fixtures.json`):
```
33441811 Portugal-Wales 18:45Z FT League A … 33441817 Liechtenstein-Lithuania 18:45Z FT League D (8 en total, incluye 33441810 Andorra-Malta 16:00Z)
league endpoint dates: desde 2026-09-25 (sin 09-24) → count=0 para hoy post-FT
```

### V2 — history[]: orden, longitud y campos

**RESPUESTA:** Orden `t` DESC (más reciente primero); longitud máxima 10 (puede haber menos, ej. 5); elementos NO contienen fixtureId → NO ENCONTRADO. Campos player: `t` fecha UTC, `opp` rival, `l{logo,name}`+`la` (competición+apid), `h` bool (¿el sujeto jugó de local? — Serbia vs México h=false con México de local), `v` valor, `m` minutos jugados, `p` posición, `met` ¿cumple línea?, `vo`/`vs` presentes (semántica NO ENCONTRADO). Campos team: `t`, `opp`, `l` competición, `h`, `va`/`vf` (valor lado local/visitante; vt=va+vf en party=total; vt==vf en party=for), `vt` valor usado, `met`.

**EVIDENCIA** (`tmp/fm/v3_recompute.txt`, snapshots en C:\Services\MatchEdge\tmp\fm\snapshots\33441813\):
```
times: 2026-06-05 … 2025-09-06 (desc); maxHistLen=10 en ambas pestañas
player hist0: {h,l,m:88,p:"LCAM",t,v:2,la:1082,vo,vs,met,opp} — sin fixtureId
team hist0: {h,l,t,va:6,vf:1,vt:7,met,opp} — va+vf=vt
```

### V3 — Reconstrucción de 20 señales vs bestCount/bestTotal y UI

**RESPUESTA:** **20/20 coincidencias.** Regla verificada: (a) recomputar con línea y dirección (`v > line` para over) == `count(met=true)` en 20/20 (semántica de línea confirmada); (b) `bestCount/bestTotal` NO es el total de la historia: es la **mejor ventana de los últimos k partidos** (sufijo más reciente, k >= min_matches: teams=4, players=3) maximizando hit-rate, empate → ventana más larga. Coincide con el total completo solo cuando bestTotal==len(history). UI muestra exactamente bestCount/bestTotal.

**EVIDENCIA** (`tmp/fm/v3_rule_check.json`, `tmp/fm/v3_recompute.txt`, `tmp/fm/v3_ui_teamtrends.txt`):
```
V3 rule (recent best-window): 20/20
UI: "Total corners over 5.5 → 10/10 Hit"; "Corners for over 2.5 → 9/10"; "Corners for over 3.5 → 7/8" (== payload)
contraejemplo bestTotal<len: total_goals L1.5 history=10 best=7/7 (ventana k7)
```

### V4 — Cobertura: ¿todas las líneas? ¿todos los mercados?

**RESPUESTA:** No solo la mejor ni todas: el payload trae **varias líneas por sujeto+mercado** (Grecia total_corners x3: 5.5/6.5/7.5; Serbia x2) pero es el conjunto de candidatas precalculadas de FM, no todas las líneas posibles (todas → NO ENCONTRADO). Mercados: solo los del catálogo trends (param `markets=1..10` teams), observados 9 teams / 8 players → solo mercados 'trend'.

**EVIDENCIA** (`tmp/fm/v3_recompute.txt`, `tmp/fm/d5_team_intercept.json`):
```
total_corners: lines=5.5/6.5/7.5 subjects=5 (duplicados sujeto+mercado con líneas distintas)
markets param: 1,2,3,4,5,6,7,8,9,10&bookmakers=1,2,3,4,5
```

### V5 — location=all vs match y league_only on/off (1 fixture)

**RESPUESTA:** `location=all` (default) count=15; `location=match` count=0 (sin filas → comparación de history NO ENCONTRADO para venue); `league_only=true` count=19 vs false=15. En las 7 filas comunes (base vs league_only) history[] CAMBIA (n=10 → n=6, best* recalculado): league_only filtra la historia. Splits locales sin más navegaciones: liga sí (elementos `l`/`la`), venue sí (campo `h`, evidencia Serbia-México h=false). 

**EVIDENCIA** (`tmp/fm/v5_variants.json`, `tmp/fm/v5_compare.json`):
```
base15 / locMatch0 / leagueOnly19 / locMatchLeague0
commonRows=7 historyDiffs=7 bestDiffs=7: Serbia|total_goals|1.5 base 7/7 n=10 vs league 5/6 n=6
```

### V6 — Sesión no premium (perfil Chrome temporal vacío)

**RESPUESTA:** Sin redirect a /login; la página del fixture carga (title correcto, resultado "1 - 2 FULL TIME") pero con marcadores "Upgrade | Sign in" y la llamada trends NO llega en 15s (NO_RESPONSE_15S): FM retiene trends a no-premium. Detectable → status='DEGRADED' añadido al snapshot cuando falla la ready-signal y hay marcadores de paywall.

**EVIDENCIA** (`tmp/fm/nonpremium/20260924212808227.json`):
```
finalUrl=…/33441813-…?tab=team-trends redirectedToLogin=False apiState=NO_RESPONSE_15S
body: "… Upgrade Sign in Home UEFA Nations League … Serbia vs Greece … 1 - 2 FULL TIME Greece"
```

### V7 — Listado global de fixtures por fecha

**RESPUESTA:** SÍ existe: `GET /api/front/fixtures?date=YYYY-MM-DD` devuelve TODAS las ligas con fixtures de ese día (sin elegir liga): grupos {id,apid,name,slug,country,…,fixtures[]} con Home/Away poblados y `state`/`status` (5/"FT" = finalizado, 1/"NS" = no empezado). Enumerar ligas del día = nombres de los grupos. 0 navegaciones (fetch puro).

**EVIDENCIA** (`tmp/fm/v7_global_fixtures.json`, `tmp/fm/v1_unl_day.json`):
```
200: 4 ligas el 24/09: UEFA Nations League(8, sample state=5 status=FT), Friendly(8? n=6), AFCON Q(8), Botola Pro(1, NS)
groupKeys: id,apid,name,logo,slug,country,flag,…,fixtures
```
