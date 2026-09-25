using System.Text;
using System.Text.Json.Nodes;
using MatchEdge.Infrastructure.Clients;
using MatchEdge.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace MatchEdge.Api.Controllers;

public sealed record FmTeamCollectRequest(string[]? Locations, int? Period, string? Stat);

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
        var locations = request?.Locations is { Length: > 0 }
            ? request.Locations.Select(l => l.ToLowerInvariant()).Distinct().ToArray()
            : new[] { "home", "away" };
        var invalid = locations.Where(l => Array.IndexOf(AllowedLocations, l) < 0).ToArray();
        if (invalid.Length > 0)
            return BadRequest(new { error = $"invalid location(s): {string.Join(",", invalid)}" });

        var period = request?.Period is > 0 ? request.Period.Value : 15;
        var stat = string.IsNullOrWhiteSpace(request?.Stat) ? "corners" : request.Stat!;
        var results = new List<object>();

        foreach (var location in locations)
        {
            ct.ThrowIfCancellationRequested();
            var apiPath =
                $"/api/front/teams/table?stat={Uri.EscapeDataString(stat)}&id={teamApid}" +
                $"&period={period}&location={location}&group=attack&selectedLeagues=&half=" +
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
                "G2 collect team {Team} {Location}: {Written} team rows ({Inserted} new), {Players} player rows, {Warnings} warning(s)",
                teamApid, location, upsert.Written, upsert.Inserted,
                upsert.PlayerWritten, parsed.Warnings.Count);

            results.Add(new
            {
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

        return Ok(new { teamApid, period, stat, results });
    }

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
}
