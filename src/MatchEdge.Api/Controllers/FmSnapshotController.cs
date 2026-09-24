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
    private readonly FmNavigator _navigator;
    private readonly ILogger<FmSnapshotController> _logger;

    public FmSnapshotController(
        IFmFixtureResolver resolver,
        IFmFixtureSnapshotService snapshots,
        IFmOutcomeResolver outcomes,
        FmFixtureCatalog catalog,
        FmSnapshotStore store,
        FmNavigator navigator,
        ILogger<FmSnapshotController> logger)
    {
        _resolver = resolver;
        _snapshots = snapshots;
        _outcomes = outcomes;
        _catalog = catalog;
        _store = store;
        _navigator = navigator;
        _logger = logger;
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
