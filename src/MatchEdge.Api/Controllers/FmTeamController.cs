using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using MatchEdge.Infrastructure.Clients;
using MatchEdge.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace MatchEdge.Api.Controllers;

// PBI 2.1 C: locations accepts the PBI's venue=home,away spelling and stat
// accepts market names (goalkeeper_saves) or FM slugs (saves), comma separated.
public sealed record FmTeamCollectRequest(
    string[]? Locations, int? Period, string? Stat, string? Venue, string[]? Stats);

// P8 J5: collects /api/front/position-stats rows (perspective=for) and stores
// them in fm_player_matches with perspective 'position'.
public sealed record FmPositionCollectRequest(int? Period);

// P10 (item 5 de P2): N equipos en una sola llamada. Mismos defaults que el
// collect por equipo (stats=corners, locations=home+away, period=15) y el
// añadido includePositions, que dispara collect-positions por equipo despues
// de su teams/table.
public sealed record FmGlobalCollectRequest(
    long[]? Teams, string[]? Stats, string[]? Locations, string? Venue,
    int? Period, bool? IncludePositions);

[ApiController]
[Route("api/fm")]
public class FmTeamController : ControllerBase
{
    private static readonly string[] AllowedLocations = { "home", "away" };

    private readonly FootyMetricsBrowserManager _browserManager;
    private readonly FootyMetricsBrowserCollector _collector;
    private readonly FmSnapshotStore _store;
    private readonly ILogger<FmTeamController> _logger;

    public FmTeamController(
        FootyMetricsBrowserManager browserManager,
        FootyMetricsBrowserCollector collector,
        FmSnapshotStore store,
        ILogger<FmTeamController> logger)
    {
        _browserManager = browserManager;
        _collector = collector;
        _store = store;
        _logger = logger;
    }

    // G3: read-only. Never touches the browser.
    [HttpGet("teams/{teamApid}/matches")]
    public async Task<IActionResult> GetMatches(
        long teamApid,
        [FromQuery] string? location,
        [FromQuery] int? period,
        [FromQuery] bool includePlayers = false,
        CancellationToken ct = default)
    {
        if (!string.IsNullOrEmpty(location) &&
            Array.IndexOf(AllowedLocations, location) < 0)
            return BadRequest(new { error = "location must be home or away" });

        var rows = await _store.GetTeamMatchesAsync(
            teamApid, location, period, includePlayers, ct);

        var matches = rows.Select(r => new
        {
            r.FixtureId,
            r.FixtureApid,
            kickoffUtc = r.TsUtc.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            r.Location,
            r.Opponent,
            r.OpponentApid,
            r.League,
            r.LeagueApid,
            score = r.HGoals.HasValue && r.AGoals.HasValue ? $"{r.HGoals}-{r.AGoals}" : null,
            teamStats = ParseOrNull(r.TeamStatsJson),
            opponentStrength = ParseOrNull(r.OpponentStrengthJson),
            r.Period,
            r.Stat,
            r.Source,
            sourceTimestampUtc = r.SourceTimestampUtc.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            players = r.Players.Select(p => new
            {
                p.PlayerApid,
                p.PlayerName,
                p.Perspective,
                stats = ParseOrNull(p.StatsJson)
            })
        }).ToList();

        return Ok(new
        {
            teamApid,
            location,
            period,
            includePlayers,
            count = matches.Count,
            playerRows = matches.Sum(m => m.players.Count()),
            matches
        });
    }

    // G2: teams/table via direct API fetch (0 navigations) + persist.
    // Serialized on the same navigation gate so it can never overlap a nav.
    [HttpPost("teams/{teamApid}/collect")]
    public async Task<IActionResult> Collect(
        long teamApid,
        [FromBody] FmTeamCollectRequest? request,
        CancellationToken ct)
    {
        var locations = LocationTokens(request?.Locations, request?.Venue);
        var invalid = locations.Where(l => Array.IndexOf(AllowedLocations, l) < 0).ToArray();
        if (invalid.Length > 0)
            return BadRequest(new { error = $"invalid location(s): {string.Join(",", invalid)}" });

        var period = request?.Period is > 0 ? request.Period.Value : 15;
        var stats = StatTokens(request?.Stats, request?.Stat);
        var batch = await CollectTeamBatchAsync(teamApid, stats, locations, period, ct);
        return Ok(new
        {
            teamApid,
            period,
            stat = stats.Length == 1 ? stats[0] : null,
            stats,
            locations,
            results = batch.Response
        });
    }

    // P10 (item 5 de P2): una sola llamada HTTP para N equipos. Ejecuta el
    // MISMO CollectTeamBatchAsync de arriba, un equipo tras otro, en el mismo
    // navigation gate: el orquestador no añade paralelismo ni throttle (los
    // 2-4 s entre fetches viven en el loop del batch). includePositions
    // reutiliza CollectPositionsCoreAsync del MISMO equipo, siempre despues de
    // su teams/table (position-stats necesita fm_team_matches/fm_player_matches
    // poblados). Un equipo que falla no aborta a los demas: su entrada lleva
    // error y el resto sigue; el 400 solo sale del body invalido.
    [HttpPost("collect")]
    public async Task<IActionResult> CollectGlobal(
        [FromBody] FmGlobalCollectRequest? request,
        CancellationToken ct)
    {
        var locations = LocationTokens(request?.Locations, request?.Venue);
        var period = request?.Period ?? 15;
        var includePositions = request?.IncludePositions ?? false;
        var stats = StatTokens(request?.Stats, stat: null);

        var validation = FmGlobalCollectOrchestrator.Validate(
            request?.Teams, period, locations, includePositions);
        if (validation is not null)
            return BadRequest(new { error = validation });

        var outcome = await FmGlobalCollectOrchestrator.RunAsync(
            request!.Teams!,
            period,
            includePositions,
            async (team, token) =>
            {
                var batch = await CollectTeamBatchAsync(team, stats, locations, period, token);
                _logger.LogInformation(
                    "P10 global collect team {Team}: {Fetches} fetches, {Errors} errors",
                    team, batch.Fetches, batch.Errors);
                return batch;
            },
            includePositions
                ? async (team, token) =>
                {
                    var pos = await CollectPositionsCoreAsync(team, period, token);
                    return new FmGlobalCollectBatch(
                        pos.Body,
                        pos.Fetches,
                        pos.StatusCode == StatusCodes.Status200OK
                            ? pos.Errors
                            : pos.Errors + 1);
                }
                : null,
            ct);

        return Ok(new
        {
            teams = outcome.Teams.Select(t => new
            {
                t.TeamApid,
                period,
                stat = stats.Length == 1 ? stats[0] : null,
                stats,
                locations,
                results = t.Collect,
                error = t.Error,
                positions = t.Positions
            }).ToList(),
            summary = new
            {
                teams = outcome.Teams.Count,
                fetches = outcome.Fetches,
                positionFetches = outcome.PositionFetches,
                errors = outcome.Errors,
                elapsedMs = outcome.ElapsedMs
            }
        });
    }

    // Cuerpo comun de los dos endpoints de collect (G2 por equipo y P10
    // global): un teams/table fetch por stat x location, serializado en el
    // navigation gate y con la pausa de 2-4 s entre fetches. Devuelve la lista
    // de results tal cual (mismo shape que siempre) mas los contadores de
    // fetches/errores que resume el orquestador global.
    private async Task<FmGlobalCollectBatch> CollectTeamBatchAsync(
        long teamApid, string[] stats, string[] locations, int period,
        CancellationToken ct)
    {
        var results = new List<object>();
        var errors = 0;

        foreach (var stat in stats)
        foreach (var location in locations)
        {
            ct.ThrowIfCancellationRequested();
            // PBI 2.1 C: the pivot payload is keyed on group, not on stat, so
            // a defense/discipline stat must never be requested with group=attack.
            var group = FmPlayerStatMap.GroupForStat(stat);
            var apiPath =
                $"/api/front/teams/table?stat={Uri.EscapeDataString(stat)}&id={teamApid}" +
                $"&period={period}&location={location}&group={group}&selectedLeagues=&half=" +
                "&dl=true&se=false&sm=easier";

            string? raw = null;
            string fetchError = "none";
            var gate = _browserManager.NavigationGate;
            await gate.WaitAsync(ct);
            try
            {
                raw = await _collector.FetchJsonAsync(apiPath, ct);
            }
            catch (Exception ex)
            {
                fetchError = ex.Message;
            }
            finally
            {
                gate.Release();
            }

            if (string.IsNullOrWhiteSpace(raw))
            {
                errors++;
                results.Add(new
                {
                    location,
                    apiPath,
                    status = "NO_DATA",
                    fetchError,
                    navigations = 0
                });
                continue;
            }

            FmTeamTableParseResult parsed;
            try
            {
                parsed = FmTeamTableParser.Parse(raw, teamApid, location, period, stat);
            }
            catch (Exception ex)
            {
                errors++;
                results.Add(new
                {
                    location,
                    apiPath,
                    status = "PARSE_ERROR",
                    error = ex.Message,
                    navigations = 0
                });
                continue;
            }

            var now = DateTime.UtcNow;
            var rawPath = await WriteRawAsync(teamApid, location, period, stat, raw, ct);
            var upsert = await _store.UpsertTeamMatchesAsync(
                teamApid, location, period, stat,
                parsed.TeamMatches, parsed.PlayerMatches,
                source: $"teams/table?location={location}&period={period}&stat={stat}",
                sourceTimestampUtc: now,
                ct);

            _logger.LogInformation(
                "G2 collect team {Team} {Stat}/{Group} {Location}: {Written} team rows ({Inserted} new), {Players} player rows, {Warnings} warning(s)",
                teamApid, stat, group, location, upsert.Written, upsert.Inserted,
                upsert.PlayerWritten, parsed.Warnings.Count);

            results.Add(new
            {
                stat,
                group,
                location,
                apiPath,
                status = parsed.TeamMatches.Count > 0 ? "OK" : "EMPTY",
                navigations = 0,
                rawPath,
                written = upsert.Written,
                inserted = upsert.Inserted,
                updated = upsert.Updated,
                playerRows = upsert.PlayerWritten,
                warnings = parsed.Warnings
            });

            await Task.Delay(TimeSpan.FromSeconds(2 + Random.Shared.NextDouble() * 2), ct);
        }

        return new FmGlobalCollectBatch(results, results.Count, errors);
    }

    // venue=home,away (PBI spelling) -> ["home","away"]
    private static string[] VenueTokens(string? venue) =>
        string.IsNullOrWhiteSpace(venue)
            ? Array.Empty<string>()
            : venue.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                   .Select(v => v.StartsWith("venue=", StringComparison.OrdinalIgnoreCase)
                       ? v["venue=".Length..]
                       : v)
                   .Select(v => v.ToLowerInvariant())
                   .Distinct(StringComparer.OrdinalIgnoreCase)
                   .ToArray();

    // locations[] / venue=home,away -> tokens, con el default home+away cuando
    // no llega ninguno de los dos (misma semantica heredada del collect por
    // equipo; la validacion de tokens es del caller).
    private static string[] LocationTokens(string[]? locations, string? venue)
    {
        var resolved = locations is { Length: > 0 }
            ? locations.Select(l => l.ToLowerInvariant()).Distinct().ToArray()
            : VenueTokens(venue);
        return resolved.Length == 0 ? new[] { "home", "away" } : resolved;
    }

    // "tackles,fouls-committed" | "goalkeeper_saves" | null -> FM slugs.
    private static string[] StatTokens(string[]? stats, string? stat)
    {
        var raw = stats is { Length: > 0 }
            ? stats
            : string.IsNullOrWhiteSpace(stat)
                ? new[] { "corners" }
                : stat!.Split(
                    ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return raw.Select(FmPlayerStatMap.ResolveSlug)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    // P8 J3/J5: position-stats (0 navigations, same gate as collect).
    // Breakdown probe = GK chunk with stat=fouls-involvements (gives the
    // position codes with apps>0), then one fetch per chunk of <=4 codes
    // (max accepted by the endpoint) plus stat=saves for the GK chunk.
    // Rows are persisted as fm_player_matches with perspective 'position',
    // stat 'position-stats', period from the request (default 20, max window).
    [HttpPost("teams/{teamApid}/collect-positions")]
    public async Task<IActionResult> CollectPositions(
        long teamApid,
        [FromBody] FmPositionCollectRequest? request,
        CancellationToken ct)
    {
        var period = request?.Period is > 0 ? request.Period.Value : 20;
        var batch = await CollectPositionsCoreAsync(teamApid, period, ct);
        if (batch.StatusCode == StatusCodes.Status409Conflict)
            return Conflict(batch.Body);
        if (batch.StatusCode == StatusCodes.Status502BadGateway)
            return StatusCode(StatusCodes.Status502BadGateway, batch.Body);
        return Ok(batch.Body);
    }

    // P10: el mismo trabajo de la action de arriba devuelto como batch, para
    // que el orquestador global lo reutilice sin copiar codigo.
    private async Task<FmPositionsBatch> CollectPositionsCoreAsync(
        long teamApid, int period, CancellationToken ct)
    {
        var source = $"position-stats?period={period}&venue=both&perspective=for";

        var indexRows = await _store.GetTeamFixtureIndexAsync(teamApid, ct);
        if (indexRows.Count == 0)
            return FmPositionsBatch.Conflict(new
            {
                error = $"no fm_team_matches rows for team {teamApid}; run POST /api/fm/teams/{teamApid}/collect first"
            });
        var fixtureByApid = new Dictionary<long, FmTeamFixtureIndexRow>();
        foreach (var r in indexRows)
            if (!fixtureByApid.ContainsKey(r.FixtureApid))
                fixtureByApid[r.FixtureApid] = r;

        var playerApidByName = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
        foreach (var r in await _store.GetPlayerMatchesAsync(teamApid, null, null, null, ct))
        {
            var name = r.PlayerName?.Trim();
            if (!string.IsNullOrEmpty(name) && !playerApidByName.ContainsKey(name))
                playerApidByName[name] = r.PlayerApid;
        }
        if (playerApidByName.Count == 0)
            return FmPositionsBatch.Conflict(new
            {
                error = $"no fm_player_matches rows for team {teamApid}; run POST /api/fm/teams/{teamApid}/collect first"
            });

        var fetches = new List<object>();
        var fetchCount = 0;
        var errorCount = 0;
        var entries = new Dictionary<string, PositionEntry>(StringComparer.Ordinal);
        var skippedFixture = new HashSet<long>();
        var skippedPlayer = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var warnings = new List<string>();

        async Task<string?> FetchAsync(string apiPath, string label, bool delay)
        {
            ct.ThrowIfCancellationRequested();
            string? raw = null;
            var error = "none";
            var gate = _browserManager.NavigationGate;
            await gate.WaitAsync(ct);
            try
            {
                raw = await _collector.FetchJsonAsync(apiPath, ct);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            finally
            {
                gate.Release();
            }

            var status = string.IsNullOrWhiteSpace(raw) ? "NO_DATA" : "OK";
            fetches.Add(new { label, apiPath, status, error });
            fetchCount++;
            if (status != "OK") errorCount++;
            if (string.IsNullOrWhiteSpace(raw))
            {
                warnings.Add($"{label}: {error}");
                return null;
            }

            await WritePositionRawAsync(teamApid, period, label, raw, ct);
            if (delay)
                await Task.Delay(TimeSpan.FromSeconds(2 + Random.Shared.NextDouble() * 2), ct);
            return raw;
        }

        // valueKey: which metric the payload's value column carries for this
        // fetch ("foulInvolvements" for stat=fouls-involvements, "saves" for
        // stat=saves). Scalar columns come from the fouls fetch only, so the
        // saves fetch cannot clobber them with its own zeros.
        void MergeAppearances(string raw, string? valueKey, bool includeScalars)
        {
            JsonNode? root;
            try { root = JsonNode.Parse(raw); }
            catch (JsonException)
            {
                warnings.Add("payload is not valid JSON");
                return;
            }
            if (root?["appearances"] is not JsonArray apps)
            {
                warnings.Add("payload has no appearances[]");
                return;
            }

            foreach (var item in apps)
            {
                if (item is not JsonObject o) continue;
                if (o["fid"] is not { } fidNode ||
                    !long.TryParse(fidNode.ToString(), out var fid)) continue;
                var playerName = o["player"]?["name"]?.GetValue<string>()?.Trim();
                if (string.IsNullOrEmpty(playerName)) continue;

                var key = Key(fid, playerName!);
                if (!entries.TryGetValue(key, out var entry))
                {
                    entry = new PositionEntry
                    {
                        Fid = fid,
                        PlayerName = playerName!,
                        Ts = ParseTs(o["ts"]?.GetValue<string>()),
                        Stats = new JsonObject()
                    };
                    entries[key] = entry;
                }

                if (valueKey is not null && o["value"] is { } vn &&
                    vn.GetValueKind() == JsonValueKind.Number &&
                    !entry.Stats.ContainsKey(valueKey))
                {
                    entry.Stats[valueKey] = vn.DeepClone();
                }
                if (includeScalars)
                {
                    foreach (var prop in new[]
                             {
                                 "pos", "mins", "rating", "foulsC", "foulsD", "tackles",
                                 "tacklesWon", "interceptions", "yellowCards", "redCards",
                                 "goals", "assists", "sh", "sot", "keyp", "passes", "touches"
                             })
                    {
                        if (o[prop] is { } v && v.GetValueKind() != JsonValueKind.Null &&
                            !entry.Stats.ContainsKey(prop))
                        {
                            entry.Stats[prop] = v.DeepClone();
                        }
                    }
                }
            }
        }

        // 1) GK chunk = breakdown probe (position codes with apps>0).
        var gkPath = BuildPositionPath(teamApid, period, "GK", "fouls-involvements");
        var gkRaw = await FetchAsync(gkPath, "gk_fouls", delay: false);
        if (gkRaw is null)
            return FmPositionsBatch.BadGateway(
                new { error = "breakdown probe failed", fetches }, fetchCount, errorCount);

        var codes = new List<string>();
        try
        {
            var root = JsonNode.Parse(gkRaw);
            if (root?["breakdown"] is JsonArray breakdown)
            {
                foreach (var b in breakdown)
                {
                    var pos = b?["pos"]?.GetValue<string>()?.Trim();
                    var apps = b?["apps"]?.GetValue<int?>() ?? 0;
                    if (!string.IsNullOrEmpty(pos) && pos != "GK" && apps > 0 && !codes.Contains(pos))
                        codes.Add(pos);
                }
            }
        }
        catch (JsonException)
        {
            return FmPositionsBatch.BadGateway(
                new { error = "breakdown is not valid JSON", fetches }, fetchCount, errorCount);
        }
        if (codes.Count == 0)
            warnings.Add("breakdown returned no non-GK positions with apps>0");

        MergeAppearances(gkRaw, "foulInvolvements", includeScalars: true);

        // 2) saves for the GK chunk.
        var gkSavesRaw = await FetchAsync(
            BuildPositionPath(teamApid, period, "GK", "saves"), "gk_saves", delay: true);
        if (gkSavesRaw is not null) MergeAppearances(gkSavesRaw, "saves", includeScalars: false);

        // 3) remaining positions in chunks of <=4 (endpoint hard limit).
        var chunkIndex = 0;
        for (var i = 0; i < codes.Count; i += 4)
        {
            var chunk = string.Join(",", codes.Skip(i).Take(4));
            var raw = await FetchAsync(
                BuildPositionPath(teamApid, period, chunk, "fouls-involvements"),
                $"chunk{chunkIndex++}_fouls",
                delay: i + 4 < codes.Count);
            if (raw is not null) MergeAppearances(raw, "foulInvolvements", includeScalars: true);
        }

        // 4) resolve against fm_team_matches + fm_player_matches and upsert.
        var now = DateTime.UtcNow;
        var rows = new List<FmPlayerMatchRow>();
        foreach (var e in entries.Values)
        {
            if (!fixtureByApid.TryGetValue(e.Fid, out var fx))
            {
                skippedFixture.Add(e.Fid);
                continue;
            }
            if (!playerApidByName.TryGetValue(e.PlayerName.Trim(), out var playerApid))
            {
                skippedPlayer.Add(e.PlayerName);
                continue;
            }
            if (e.Ts is null)
            {
                warnings.Add($"no ts for {e.PlayerName} @ {e.Fid}");
                continue;
            }
            rows.Add(new FmPlayerMatchRow(
                TeamApid: teamApid,
                PlayerApid: playerApid,
                PlayerName: e.PlayerName,
                FixtureId: fx.FixtureId,
                FixtureApid: e.Fid,
                TsUtc: e.Ts!.Value,
                Location: fx.Location,
                Perspective: "position",
                StatsJson: e.Stats.ToJsonString(),
                Period: period,
                Stat: "position-stats",
                Source: source,
                SourceTimestampUtc: now));
        }

        var written = rows.Count == 0
            ? 0
            : await _store.UpsertPlayerMatchesAsync(teamApid, rows, source, now, ct);

        _logger.LogInformation(
            "P8 collect-positions team {Team}: {Rows} rows merged, {Written} upserted, {SkippedFx} unknown fixture, {SkippedPl} unknown player, {Warn} warning(s)",
            teamApid, entries.Count, written, skippedFixture.Count, skippedPlayer.Count, warnings.Count);

        return FmPositionsBatch.Ok(new
        {
            teamApid,
            period,
            source,
            positionCodes = codes,
            fetches,
            merged = entries.Count,
            upserted = written,
            skippedUnknownFixture = skippedFixture.OrderBy(x => x).Take(10).ToArray(),
            skippedUnknownPlayer = skippedPlayer.OrderBy(x => x).Take(10).ToArray(),
            warnings = warnings.Take(20).ToArray(),
            navigations = 0
        }, fetchCount, errorCount);
    }

    private static string BuildPositionPath(
        long teamApid, int period, string positions, string stat) =>
        $"/api/front/position-stats?team={teamApid}" +
        $"&positions={Uri.EscapeDataString(positions)}&stat={Uri.EscapeDataString(stat)}" +
        $"&period={period}&venue=both&perspective=for&teamFormations=&oppFormations=" +
        "&leagues=&superSub=false";

    private static string Key(long fid, string name) =>
        fid.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\u001f" + name;

    // Resultado de CollectPositionsCoreAsync: la action lo traduce a 409/502/200
    // y el orquestador global lee Body + contadores (fetches/errores).
    private sealed record FmPositionsBatch(object Body, int StatusCode, int Fetches, int Errors)
    {
        public static FmPositionsBatch Ok(object body, int fetches, int errors) =>
            new(body, StatusCodes.Status200OK, fetches, errors);

        public static FmPositionsBatch Conflict(object body) =>
            new(body, StatusCodes.Status409Conflict, 0, 0);

        public static FmPositionsBatch BadGateway(object body, int fetches, int errors) =>
            new(body, StatusCodes.Status502BadGateway, fetches, errors);
    }

    private sealed class PositionEntry
    {
        public long Fid;
        public string PlayerName = "";
        public DateTime? Ts;
        public JsonObject Stats = new();
    }

    private static DateTime? ParseTs(string? ts) =>
        string.IsNullOrWhiteSpace(ts)
            ? null
            : DateTime.Parse(ts, System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.AssumeUniversal |
                System.Globalization.DateTimeStyles.AdjustToUniversal);

    private static object? ParseOrNull(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try
        {
            return JsonNode.Parse(json);
        }
        catch
        {
            return json;
        }
    }

    private static async Task<string> WriteRawAsync(
        long teamApid, string location, int period, string stat, string content,
        CancellationToken ct)
    {
        var dir = Path.Combine(
            AppContext.BaseDirectory, "tmp", "fm", "teams",
            teamApid.ToString(System.Globalization.CultureInfo.InvariantCulture));
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir,
            $"{location}_p{period}_{stat}_{DateTime.UtcNow:yyyyMMddHHmmssfff}.json");
        await System.IO.File.WriteAllTextAsync(path, content, new UTF8Encoding(false), ct);
        return path;
    }

    private static async Task WritePositionRawAsync(
        long teamApid, int period, string label, string content, CancellationToken ct)
    {
        var dir = Path.Combine(
            AppContext.BaseDirectory, "tmp", "fm", "teams",
            teamApid.ToString(System.Globalization.CultureInfo.InvariantCulture));
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir,
            $"positions_p{period}_{label}_{DateTime.UtcNow:yyyyMMddHHmmssfff}.json");
        await System.IO.File.WriteAllTextAsync(path, content, new UTF8Encoding(false), ct);
    }
}
