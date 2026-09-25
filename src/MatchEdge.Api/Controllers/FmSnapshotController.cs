using System.Text.RegularExpressions;
using MatchEdge.Application.Clients.FootyMetrics;
using MatchEdge.Infrastructure.Clients;
using MatchEdge.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace MatchEdge.Api.Controllers;

[ApiController]
[Route("api/fm")]
public class FmSnapshotController : ControllerBase
{
    private static readonly Regex FixtureIdFromUrl = new(@"^/fixtures/(\d+)-", RegexOptions.Compiled);

    private readonly IFmFixtureResolver _resolver;
    private readonly IFmFixtureSnapshotService _snapshots;
    private readonly IFmOutcomeResolver _outcomes;
    private readonly FmFixtureCatalog _catalog;
    private readonly FmSnapshotStore _store;
    private readonly FmOutcomeStore _outcomeStore;
    private readonly FmNavigator _navigator;
    private readonly ILogger<FmSnapshotController> _logger;

    public FmSnapshotController(
        IFmFixtureResolver resolver,
        IFmFixtureSnapshotService snapshots,
        IFmOutcomeResolver outcomes,
        FmFixtureCatalog catalog,
        FmSnapshotStore store,
        FmOutcomeStore outcomeStore,
        FmNavigator navigator,
        ILogger<FmSnapshotController> logger)
    {
        _resolver = resolver;
        _snapshots = snapshots;
        _outcomes = outcomes;
        _catalog = catalog;
        _store = store;
        _outcomeStore = outcomeStore;
        _navigator = navigator;
        _logger = logger;
    }

    [HttpGet("fixtures/{fixtureId}/confluence")]
    public async Task<IActionResult> Confluence(string fixtureId, CancellationToken ct)
    {
        var signals = await _store.GetLatestSignalDetailsAsync(fixtureId, ct);
        if (signals.Count == 0)
            return NotFound(new { error = $"fixture {fixtureId} has no OK/SUSPECT signals." });

        var outcomes = await _outcomeStore.GetByFixtureAsync(fixtureId, ct);
        var leakage = await _store.HasLeakageAsync(fixtureId, ct);
        var url = await _store.GetLastSnapshotUrlAsync(fixtureId, ct);

        var (venue, compFromUrl) = FmFixtureSnapshotService.ScopesFromUrl(url);
        var paramsJson = signals.Select(s => s.ParamsJson)
            .FirstOrDefault(p => !string.IsNullOrEmpty(p));
        if (!string.IsNullOrEmpty(paramsJson))
        {
            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(paramsJson);
                if (doc.RootElement.TryGetProperty("location", out var loc) &&
                    loc.ValueKind == System.Text.Json.JsonValueKind.String)
                    venue = loc.GetString() ?? venue;
            }
            catch (System.Text.Json.JsonException)
            {
                // keep URL-derived venue
            }
        }
        var competitionScope = signals
            .Select(s => s.CompetitionScope)
            .FirstOrDefault(c => !string.IsNullOrEmpty(c)) ?? compFromUrl;

        var report = FmConfluenceBuilder.Build(
            fixtureId, signals, outcomes, leakage, venue, competitionScope);
        return Ok(report);
    }

    [HttpPost("fixtures/{fixtureId}/manual-odds")]
    public async Task<IActionResult> ManualOdds(
        string fixtureId, [FromBody] FmManualOddsRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Market))
            return BadRequest(new { error = "market is required." });
        if (request.OddsValue is null || double.IsNaN(request.OddsValue.Value) ||
            double.IsInfinity(request.OddsValue.Value) ||
            request.OddsValue.Value <= 0 || request.OddsValue.Value > 10000)
            return BadRequest(new { error = "odds_value must be a number in (0, 10000]." });
        if (request.Line is not null &&
            (double.IsNaN(request.Line.Value) || double.IsInfinity(request.Line.Value)))
            return BadRequest(new { error = "line must be a finite number." });

        var bookmaker = string.IsNullOrWhiteSpace(request.Bookmaker)
            ? "Betano" : request.Bookmaker.Trim();
        var id = await _store.InsertManualOddsAsync(
            fixtureId, bookmaker, request.Market.Trim(),
            request.Line, request.OddsValue!.Value, DateTime.UtcNow, ct);

        _logger.LogInformation(
            "Manual odds {Id} fixture {FixtureId} {Bookmaker} {Market} {Line} {Odds}",
            id, fixtureId, bookmaker, request.Market, request.Line, request.OddsValue);
        return Ok(new
        {
            id,
            fixtureId,
            bookmaker,
            market = request.Market,
            line = request.Line,
            oddsValue = request.OddsValue,
            source = "manual"
        });
    }

    [HttpPost("outcomes/resolve")]
    public async Task<IActionResult> ResolveOutcomes(
        [FromBody] FmResolveRequest? request, CancellationToken ct)
    {
        request ??= new FmResolveRequest(null, null);
        try
        {
            var report = await _outcomes.ResolveAsync(request, ct);
            return Ok(new
            {
                date = report.Date,
                finishedFixtures = report.FinishedFixtures,
                navigationsUsed = report.NavigationsUsed,
                budget = report.Budget,
                newlyResolved = report.Fixtures.Sum(f => f.Resolved),
                alreadyResolved = report.Fixtures.Sum(f => f.AlreadyResolved),
                fixtures = report.Fixtures.Select(f => new
                {
                    f.FixtureId,
                    f.Home,
                    f.Away,
                    f.KickoffUtc,
                    f.Signals,
                    f.Resolved,
                    f.AlreadyResolved,
                    lines = f.Lines
                }),
                warnings = report.Warnings
            });
        }
        catch (FmResolverException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (FmNavigationException ex)
        {
            return MapNavigationError(ex);
        }
    }

    [HttpPost("fixtures")]
    public async Task<IActionResult> Fixtures([FromBody] FmFixturesRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.League))
            return BadRequest(new { error = "league is required (no default guessing)." });

        if (!TryParseDate(request.Date, out var date))
            return BadRequest(new { error = "date must be yyyy-MM-dd." });

        _navigator.ResetRun();
        try
        {
            var fixtures = await _resolver.ResolveAsync(request.League, date, ct);
            _catalog.UpsertRange(fixtures);
            return Ok(new
            {
                league = request.League,
                date = date.ToString("yyyy-MM-dd"),
                count = fixtures.Count,
                fixtures = fixtures.Select(f => new
                {
                    fixtureId = f.FixtureId,
                    home = f.Home,
                    away = f.Away,
                    kickoffUtc = f.KickoffUtc,
                    competition = f.Competition
                })
            });
        }
        catch (FmResolverException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (FmNavigationException ex)
        {
            return MapNavigationError(ex);
        }
    }

    [HttpPost("snapshot")]
    public async Task<IActionResult> Snapshot([FromBody] FmSnapshotRequest request, CancellationToken ct)
    {
        _navigator.ResetRun();
        FmFixtureRef fixture;
        try
        {
            fixture = await ResolveFixtureRefAsync(request, ct);
        }
        catch (FmFixtureNotFoundException ex)
        {
            return NotFound(new { error = ex.Message, code = "NotFound" });
        }
        catch (FmResolverException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (FmNavigationException ex)
        {
            return MapNavigationError(ex);
        }

        try
        {
            var outcome = await _snapshots.SnapshotAsync(fixture, ct);
            return Ok(new
            {
                fixtureId = outcome.FixtureId,
                tabs = outcome.Tabs.Select(t => new
                {
                    tab = t.Tab,
                    status = t.Status,
                    signals = t.Signals,
                    rawPath = t.RawPath,
                    error = t.Error
                }),
                rawPaths = outcome.RawPaths,
                warnings = outcome.Warnings,
                partial = outcome.Partial
            });
        }
        catch (FmNavigationException ex)
        {
            return MapNavigationError(ex);
        }
    }

    [HttpPost("run")]
    public async Task<IActionResult> Run([FromBody] FmRunRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.League))
            return BadRequest(new { error = "league is required (no default guessing)." });
        if (!TryParseDate(request.Date, out var date))
            return BadRequest(new { error = "date must be yyyy-MM-dd." });

        // B4: dual-scope capture = 3 navigations per fixture (overview +
        // player/team trends); the location=match scope is a direct API fetch
        // with 0 extra navigations.
        const int maxFixturesPerRun = FmNavigator.MaxNavigationsPerRun / 3;
        var topN = Math.Clamp(request.TopN ?? 5, 1, maxFixturesPerRun);

        _navigator.ResetRun();
        var warnings = new List<string>();
        if ((request.TopN ?? 5) > maxFixturesPerRun)
            warnings.Add($"topN capped to {maxFixturesPerRun} (navigation budget {FmNavigator.MaxNavigationsPerRun}).");

        IReadOnlyList<FmFixtureRef> fixtures;
        try
        {
            fixtures = await _resolver.ResolveAsync(request.League, date, ct);
            _catalog.UpsertRange(fixtures);
        }
        catch (FmResolverException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (FmNavigationException ex)
        {
            return MapNavigationError(ex);
        }

        var selected = fixtures.Take(topN).ToList();
        var snapshotCount = 0;
        var signalCount = 0;
        var rawPaths = new List<string>();
        var anyPartial = false;

        foreach (var fixture in selected)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                var outcome = await _snapshots.SnapshotAsync(fixture, ct);
                snapshotCount++;
                anyPartial |= outcome.Partial;
                signalCount += outcome.Tabs.Sum(t => t.Signals);
                rawPaths.AddRange(outcome.RawPaths);
                warnings.AddRange(outcome.Warnings.Select(w => $"{fixture.FixtureId}: {w}"));
            }
            catch (FmNavigationException ex)
            {
                if (ex.Code == FmNavigationErrorCode.NavigationBudgetExceeded)
                {
                    warnings.Add($"budget exceeded after {snapshotCount} snapshots; remaining fixtures skipped.");
                    break;
                }
                if (ex.Code == FmNavigationErrorCode.SessionExpired)
                    return MapNavigationError(ex);
                warnings.Add($"{fixture.FixtureId}: {ex.Code} {ex.Message}");
            }
        }

        return Ok(new
        {
            league = request.League,
            date = date.ToString("yyyy-MM-dd"),
            fixturesResolved = fixtures.Count,
            fixturesSelected = selected.Count,
            snapshots = snapshotCount,
            signals = signalCount,
            navigations = _navigator.NavigationsSinceReset,
            rawPaths,
            warnings,
            partial = anyPartial
        });
    }

    private async Task<FmFixtureRef> ResolveFixtureRefAsync(FmSnapshotRequest request, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(request.Url))
            return await _resolver.DescribeAsync(request.Url, ct);

        if (string.IsNullOrWhiteSpace(request.FixtureId))
            throw new FmResolverException("Either fixtureId or url is required.");

        var catalogRef = _catalog.TryGet(request.FixtureId);
        if (catalogRef != null) return catalogRef;

        var knownUrl = await _store.GetLastSnapshotUrlAsync(request.FixtureId, ct);
        if (knownUrl == null)
            throw new FmFixtureNotFoundException(
                $"Fixture {request.FixtureId} unknown: call POST /api/fm/fixtures first, or pass url.");

        return await _resolver.DescribeAsync(knownUrl, ct);
    }

    private IActionResult MapNavigationError(FmNavigationException ex) => ex.Code switch
    {
        FmNavigationErrorCode.SessionExpired => Unauthorized(new
            { error = ex.Message, code = "SessionExpired", hint = "Login manually via /start-visible." }),
        FmNavigationErrorCode.NavigationBudgetExceeded => StatusCode(429,
            new { error = ex.Message, code = "NavigationBudgetExceeded" }),
        FmNavigationErrorCode.Timeout => StatusCode(504, new { error = ex.Message, code = "Timeout" }),
        FmNavigationErrorCode.NoData => StatusCode(502, new { error = ex.Message, code = "NoData" }),
        _ => StatusCode(502, new { error = ex.Message, code = ex.Code.ToString() })
    };

    private static bool TryParseDate(string? value, out DateOnly date)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            date = DateOnly.FromDateTime(DateTime.UtcNow);
            return true;
        }
        return DateOnly.TryParseExact(value, "yyyy-MM-dd", out date);
    }
}

public sealed class FmFixturesRequest
{
    public string? League { get; set; }
    public string? Date { get; set; }
}

public sealed class FmSnapshotRequest
{
    public string? FixtureId { get; set; }
    public string? Url { get; set; }
}

public sealed class FmRunRequest
{
    public string? League { get; set; }
    public string? Date { get; set; }
    public int? TopN { get; set; }
}

public sealed class FmManualOddsRequest
{
    public string? Bookmaker { get; set; }
    public string? Market { get; set; }
    public double? Line { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("odds_value")]
    public double? OddsValue { get; set; }
}
