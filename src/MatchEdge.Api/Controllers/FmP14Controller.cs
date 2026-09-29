using MatchEdge.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace MatchEdge.Api.Controllers;

// P14: same shape as FmP13Controller, for the 29/09/2026 Nations League
// matchday (3 medium-volume fixtures + 7 low-volume, every one with the 5%
// Delta% threshold from the brief).
//
// INTENTIONAL DUPLICATION: P13Fixtures and the P13 markdown header are frozen
// for the 28/09 matchday, so P14 ships its own fixture specs instead of
// changing P13 (its tests assert the frozen wording). The report logic is NOT
// duplicated: P14 reuses P13ProxyEvReportBuilder/P13ProxyEvReportMarkdown
// as-is (Delta%, candidate cap, automatic post-kickoff detection).
//
// TECHNICAL DEBT: fixture specs are hardcoded here per matchday; a future
// cycle should move them to config/DB so each cycle only points at a date.
[ApiController]
[Route("api/fm")]
public class FmP14Controller : ControllerBase
{
    // 29/09/2026 (league apid 1538): volume/threshold/confidence come from the
    // P14 brief. No manual post flag: the builder compares the first capture
    // (MIN source_timestamp_utc of fm_snapshot) against the kickoff read from
    // the persisted overview (JSON-LD startDate).
    public static readonly IReadOnlyList<P13FixtureSpec> P14Fixtures = new[]
    {
        new P13FixtureSpec("33700898", "Espana", "Croacia", "medio"),
        new P13FixtureSpec("33700899", "Rep. Checa", "Inglaterra", "medio"),
        new P13FixtureSpec("33700901", "Escocia", "Suiza", "medio"),
        new P13FixtureSpec("33700896", "Finlandia", "Bielorrusia", "bajo"),
        new P13FixtureSpec("33700895", "Moldavia", "Islas Feroe", "bajo"),
        new P13FixtureSpec("33700903", "Bulgaria", "Estonia", "bajo"),
        new P13FixtureSpec("33700902", "Luxemburgo", "Islandia", "bajo"),
        new P13FixtureSpec("33700905", "San Marino", "Albania", "bajo"),
        new P13FixtureSpec("33700904", "Eslovaquia", "Kazajistan", "bajo"),
        new P13FixtureSpec("33700900", "Eslovenia", "Macedonia del Norte", "bajo")
    };

    private const string ReportTitle =
        "Analisis 360 + Diferencia modelo-mercado - UEFA Nations League - 29/09/2026";

    private const string ReportScope =
        "los 10 partidos del 29/09/2026 (Nations League).";

    private const string ReportNotesHeader =
        "Notas de implementacion del P14";

    private readonly FmSnapshotStore _store;
    private readonly FmOutcomeStore _outcomeStore;
    private readonly ILogger<FmP14Controller> _logger;

    public FmP14Controller(
        FmSnapshotStore store,
        FmOutcomeStore outcomeStore,
        ILogger<FmP14Controller> logger)
    {
        _store = store;
        _outcomeStore = outcomeStore;
        _logger = logger;
    }

    [HttpGet("p14/report")]
    public async Task<IActionResult> Report(CancellationToken ct)
    {
        var report = await P13ProxyEvReportBuilder.BuildAsync(
            P14Fixtures, _store, _outcomeStore, ct);
        return Ok(report);
    }

    [HttpGet("p14/report.md")]
    public async Task<IActionResult> ReportMarkdown(CancellationToken ct)
    {
        var report = await P13ProxyEvReportBuilder.BuildAsync(
            P14Fixtures, _store, _outcomeStore, ct);
        var md = P13ProxyEvReportMarkdown.Render(
            report, ReportTitle, ReportScope, ReportNotesHeader);
        _logger.LogInformation(
            "P14 report rendered: {Fixtures} fixtures, {Chars} chars",
            report.Fixtures.Count, md.Length);
        return Content(md, "text/markdown; charset=utf-8");
    }
}
