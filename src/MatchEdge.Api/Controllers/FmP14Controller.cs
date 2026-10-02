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

    // 01/10/2026 Nations League matchday (league apid 1538): same engine, own
    // specs so the 29/09 report stays byte-reproducible. Volumes come from the
    // matchday brief: Germany-Serbia, Denmark-Portugal and Wales-Norway are
    // "medio" (5% threshold, Media confidence) and Greece-Netherlands is
    // "bajo" (5%, Baja). Same technical debt as P14Fixtures: hardcoded specs.
    public static readonly IReadOnlyList<P13FixtureSpec> P14FixturesOct1 = new[]
    {
        new P13FixtureSpec("33940967", "Grecia", "Paises Bajos", "bajo"),
        new P13FixtureSpec("33940968", "Alemania", "Serbia", "medio"),
        new P13FixtureSpec("33940966", "Dinamarca", "Portugal", "medio"),
        new P13FixtureSpec("33940965", "Gales", "Noruega", "medio")
    };

    private const string ReportTitleOct1 =
        "Analisis 360 + Diferencia modelo-mercado - UEFA Nations League - 01/10/2026";

    private const string ReportScopeOct1 =
        "los 4 partidos del 01/10/2026 (Nations League).";

    // 02/10/2026 Nations League matchday (league apid 1538): same engine, own
    // specs so previous matchdays stay byte-reproducible. Volumes come from the
    // 02/10 brief: France-Italy, Belgium-Turkey, Poland-Romania and
    // Bosnia-Herzegovina-Sweden are "medio" (5% threshold, Media confidence);
    // the other six are "bajo" (5%, Baja). PASO A: the markdown endpoint adds
    // the descriptive referee note (name, referee_id, cards per game from the
    // persisted overview) next to the cards section without touching lambda,
    // Delta%, confidence, candidates or F1/F2. Same technical debt as the
    // other spec lists: hardcoded fixtures.
    public static readonly IReadOnlyList<P13FixtureSpec> P14FixturesOct2 = new[]
    {
        new P13FixtureSpec("34122611", "Kazajistan", "Moldavia", "bajo"),
        new P13FixtureSpec("34122612", "Letonia", "Montenegro", "bajo"),
        new P13FixtureSpec("34122613", "Chipre", "Armenia", "bajo"),
        new P13FixtureSpec("34122615", "Francia", "Italia", "medio"),
        new P13FixtureSpec("34122616", "Belgica", "Turquia", "medio"),
        new P13FixtureSpec("34122617", "Polonia", "Rumania", "medio"),
        new P13FixtureSpec("34122618", "Bosnia y Herzegovina", "Suecia", "medio"),
        new P13FixtureSpec("34122619", "Ucrania", "Irlanda del Norte", "bajo"),
        new P13FixtureSpec("34122620", "Hungria", "Georgia", "bajo"),
        new P13FixtureSpec("34122621", "Islas Feroe", "Eslovaquia", "bajo")
    };

    private const string ReportTitleOct2 =
        "Analisis 360 + Diferencia modelo-mercado - UEFA Nations League - 02/10/2026";

    private const string ReportScopeOct2 =
        "los 10 partidos del 02/10/2026 (Nations League).";

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

    [HttpGet("p14/report-20261001")]
    public async Task<IActionResult> ReportOct1(CancellationToken ct)
    {
        var report = await P13ProxyEvReportBuilder.BuildAsync(
            P14FixturesOct1, _store, _outcomeStore, ct);
        return Ok(report);
    }

    [HttpGet("p14/report-20261001.md")]
    public async Task<IActionResult> ReportMarkdownOct1(CancellationToken ct)
    {
        var report = await P13ProxyEvReportBuilder.BuildAsync(
            P14FixturesOct1, _store, _outcomeStore, ct);
        var md = P13ProxyEvReportMarkdown.Render(
            report, ReportTitleOct1, ReportScopeOct1, ReportNotesHeader);
        _logger.LogInformation(
            "P14 01/10 report rendered: {Fixtures} fixtures, {Chars} chars",
            report.Fixtures.Count, md.Length);
        return Content(md, "text/markdown; charset=utf-8");
    }

    [HttpGet("p14/report-20261002")]
    public async Task<IActionResult> ReportOct2(CancellationToken ct)
    {
        var report = await P13ProxyEvReportBuilder.BuildAsync(
            P14FixturesOct2, _store, _outcomeStore, ct);
        return Ok(report);
    }

    // PASO A: el markdown del 02/10 anade la nota descriptiva de arbitro por
    // fixture (0 navegaciones: lee el overview ya persistido). El JSON no la
    // lleva: es solo presentacion, el motor no cambia.
    [HttpGet("p14/report-20261002.md")]
    public async Task<IActionResult> ReportMarkdownOct2(CancellationToken ct)
    {
        var report = await P13ProxyEvReportBuilder.BuildAsync(
            P14FixturesOct2, _store, _outcomeStore, ct);
        var refereeNotes = new Dictionary<string, string>(report.Fixtures.Count);
        foreach (var section in report.Fixtures)
            refereeNotes[section.FixtureId] =
                await FmRefereeOverview.LoadNoteAsync(
                    _store, section.FixtureId, ct);
        var md = P13ProxyEvReportMarkdown.Render(
            report, ReportTitleOct2, ReportScopeOct2, ReportNotesHeader,
            refereeNotes);
        _logger.LogInformation(
            "P14 02/10 report rendered: {Fixtures} fixtures, {Chars} chars",
            report.Fixtures.Count, md.Length);
        return Content(md, "text/markdown; charset=utf-8");
    }
}
