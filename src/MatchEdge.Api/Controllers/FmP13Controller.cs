using MatchEdge.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace MatchEdge.Api.Controllers;

// P13: endpoints nuevos para el builder separado Proxy-EV (delta
// modelo-mercado). Lee tablas persistidas y el snapshot overview guardado en
// disco (ahi vive el kickoff del fixture): 0 navegaciones, no toca
// FmSnapshotController ni el pipeline P7-P11.
[ApiController]
[Route("api/fm")]
public class FmP13Controller : ControllerBase
{
    // Los 8 partidos del 28/09/2026 y su volumen vienen del brief P13.
    // No hay flag manual de post-partido: el builder detecta solo si la primera
    // captura (MIN source_timestamp_utc de fm_snapshot) es posterior al kickoff
    // del fixture (kickoff leido del overview persistido). Hoy eso marca los 5
    // partidos capturados post-kickoff (captura 21:35Z-21:38Z, ver docs/STATE.md)
    // y deja los 3 capturados pre-partido (33662311, 33662312, 33662313) en modo
    // modelo; cuando esos 3 se regeneren despues de su kickoff entraran solos en
    // el mismo modo.
    public static readonly IReadOnlyList<P13FixtureSpec> P13Fixtures = new[]
    {
        new P13FixtureSpec("33662307", "Georgia", "Ucrania", "medio"),
        new P13FixtureSpec("33662308", "Letonia", "Chipre", "bajo"),
        new P13FixtureSpec("33662309", "Armenia", "Montenegro", "bajo"),
        new P13FixtureSpec("33662311", "Turquia", "Italia", "alto"),
        new P13FixtureSpec("33662312", "Belgica", "Francia", "alto"),
        new P13FixtureSpec("33662313", "Suecia", "Polonia", "medio"),
        new P13FixtureSpec("33662314", "Rumania", "Bosnia y Herzegovina", "medio"),
        new P13FixtureSpec("33662315", "Irlanda del Norte", "Hungria", "bajo")
    };

    private readonly FmSnapshotStore _store;
    private readonly FmOutcomeStore _outcomeStore;
    private readonly ILogger<FmP13Controller> _logger;

    public FmP13Controller(
        FmSnapshotStore store,
        FmOutcomeStore outcomeStore,
        ILogger<FmP13Controller> logger)
    {
        _store = store;
        _outcomeStore = outcomeStore;
        _logger = logger;
    }

    [HttpGet("p13/report")]
    public async Task<IActionResult> Report(CancellationToken ct)
    {
        var report = await P13ProxyEvReportBuilder.BuildAsync(
            P13Fixtures, _store, _outcomeStore, ct);
        return Ok(report);
    }

    [HttpGet("p13/report.md")]
    public async Task<IActionResult> ReportMarkdown(CancellationToken ct)
    {
        var report = await P13ProxyEvReportBuilder.BuildAsync(
            P13Fixtures, _store, _outcomeStore, ct);
        var md = P13ProxyEvReportMarkdown.Render(report);
        _logger.LogInformation(
            "P13 report rendered: {Fixtures} fixtures, {Chars} chars",
            report.Fixtures.Count, md.Length);
        return Content(md, "text/markdown; charset=utf-8");
    }

    [HttpGet("fixtures/{fixtureId}/proxy-ev")]
    public async Task<IActionResult> Fixture(string fixtureId, string? volume,
        CancellationToken ct)
    {
        var spec = P13Fixtures.FirstOrDefault(f =>
            string.Equals(f.FixtureId, fixtureId, StringComparison.Ordinal))
            ?? new P13FixtureSpec(fixtureId, fixtureId, fixtureId,
                NormalizeVolume(volume));

        var report = await P13ProxyEvReportBuilder.BuildAsync(
            new[] { spec }, _store, _outcomeStore, ct);
        return Ok(report.Fixtures[0]);
    }

    [HttpGet("fixtures/{fixtureId}/proxy-ev.md")]
    public async Task<IActionResult> FixtureMarkdown(string fixtureId, string? volume,
        CancellationToken ct)
    {
        var spec = P13Fixtures.FirstOrDefault(f =>
            string.Equals(f.FixtureId, fixtureId, StringComparison.Ordinal))
            ?? new P13FixtureSpec(fixtureId, fixtureId, fixtureId,
                NormalizeVolume(volume));

        var report = await P13ProxyEvReportBuilder.BuildAsync(
            new[] { spec }, _store, _outcomeStore, ct);
        return Content(
            P13ProxyEvReportMarkdown.Render(report),
            "text/markdown; charset=utf-8");
    }

    private static string NormalizeVolume(string? volume) =>
        string.Equals(volume, "alto", StringComparison.OrdinalIgnoreCase) ? "alto"
            : string.Equals(volume, "bajo", StringComparison.OrdinalIgnoreCase) ? "bajo"
            : "medio";
}
