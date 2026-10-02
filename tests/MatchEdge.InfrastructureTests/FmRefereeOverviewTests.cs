using System.Text.RegularExpressions;
using MatchEdge.Infrastructure.Services;
using Xunit;

namespace MatchEdge.InfrastructureTests;

// PASO A (brief 02/10): parser de arbitro del overview persistido + inyeccion
// de la nota descriptiva en el markdown. El contrato que protegen estos tests
// es doble: (1) fail-closed por campo (nada de HTML = SIN DATA VERIFICADA -
// NO ASUMIR, nunca un dato inventado) y (2) el motor queda intacto: con la
// nota incluida, el resto del markdown (Delta%, confianza, candidatos, F1/F2)
// es byte identico al Render sin nota.
public class FmRefereeOverviewTests
{
    private const string FullOverview =
        "<title>France vs Italy: Referee, Lineups &amp; H2H | FootyMetrics</title>" +
        "<meta name=\"description\" content=\"France vs Italy (UEFA Nations " +
        "League): referee Jesus Gil Manzano, who averages 5.11 cards a game. " +
        "Predicted lineups, head-to-head results, recent form and match " +
        "statistics.\">" +
        "<a href=\"/referees/129-jesus-gil-manzano\">shirt</a>";

    private const string Scope =
        "los 8 partidos del 28/09/2026 (Nations League).";

    private const string NotesHeader = "Notas de implementacion del P13";

    // Mismo patron que FmDeterministicTests.Forbidden (F2 del contrato P11).
    private static readonly Regex Forbidden = new(
        @"recomend|apuesta|\bedge\b|value bet|elegid|banker|sub[ -]?hero|\bpick\b|\bev\b",
        RegexOptions.IgnoreCase);

    // ---------------------------------------------------------------------------
    // Parseo del overview (fail-closed por campo)
    // ---------------------------------------------------------------------------

    [Fact]
    public void Parse_FullOverview_ExtractsNameIdAndCards()
    {
        var info = FmRefereeOverview.Parse(FullOverview);

        Assert.Equal("Jesus Gil Manzano", info.Name);
        Assert.Equal("129", info.RefereeId);
        Assert.Equal(5.11, info.CardsPerGame);
    }

    [Fact]
    public void Parse_NullEmptyOrForeignHtml_ReturnsNone()
    {
        Assert.Equal(FmRefereeInfo.None, FmRefereeOverview.Parse(null));
        Assert.Equal(FmRefereeInfo.None, FmRefereeOverview.Parse(""));
        Assert.Equal(FmRefereeInfo.None, FmRefereeOverview.Parse("<html></html>"));
    }

    [Fact]
    public void Parse_MetaWithoutRefereeLink_LeavesIdNull()
    {
        var info = FmRefereeOverview.Parse(
            "<meta name=\"description\" content=\"Kazakhstan vs Moldova: " +
            "referee Marian Alexandru Barbu, who averages 4.54 cards a game.\"");

        Assert.Equal("Marian Alexandru Barbu", info.Name);
        Assert.Null(info.RefereeId);
        Assert.Equal(4.54, info.CardsPerGame);
    }

    [Fact]
    public void Parse_LinkWithoutMeta_LeavesNameAndCardsNull()
    {
        var info = FmRefereeOverview.Parse("<a href=\"/referees/7869-foo\">x</a>");

        Assert.Null(info.Name);
        Assert.Equal("7869", info.RefereeId);
        Assert.Null(info.CardsPerGame);
    }

    [Fact]
    public void FormatNote_AllFieldsPresent_UsesMarkerAndTwoDecimals()
    {
        var note = FmRefereeOverview.FormatNote(
            new FmRefereeInfo("Urs Schnyder", "5092", 4.4));

        Assert.StartsWith(FmRefereeOverview.Marker + " Urs Schnyder", note);
        Assert.Contains("referee_id: 5092", note);
        Assert.Contains("(historial FM): 4.40", note);
        Assert.DoesNotMatch(Forbidden, note);
    }

    [Fact]
    public void FormatNote_None_FailsClosedOnEveryField()
    {
        var note = FmRefereeOverview.FormatNote(FmRefereeInfo.None);

        Assert.StartsWith(FmRefereeOverview.Marker, note);
        Assert.Contains("SIN DATA VERIFICADA - NO ASUMIR", note);
        Assert.Equal(
            3, Regex.Matches(note, FmRefereeOverview.NoDataText).Count);
        Assert.DoesNotMatch(Forbidden, note);
    }

    // ---------------------------------------------------------------------------
    // Renderer: inyeccion gateada (null = salida byte identica) e integridad
    // ---------------------------------------------------------------------------

    [Fact]
    public void Render_WithoutDictionary_KeepsExactOutputOfOriginalOverloads()
    {
        var report = MakeReport();
        var expected = P13ProxyEvReportMarkdown.Render(report);

        Assert.Equal(expected, P13ProxyEvReportMarkdown.Render(
            report, P13ProxyEvReportMarkdown.Title,
            Scope,
            NotesHeader));
        Assert.Equal(expected, P13ProxyEvReportMarkdown.Render(
            report, P13ProxyEvReportMarkdown.Title,
            Scope,
            NotesHeader, refereeNotes: null));
    }

    [Fact]
    public void Render_WithRefereeNotes_InsertsThemBesideCardsAndEngineStaysByteIdentical()
    {
        var report = MakeReport();
        var without = P13ProxyEvReportMarkdown.Render(report);
        var with = P13ProxyEvReportMarkdown.Render(
            report,
            P13ProxyEvReportMarkdown.Title,
            Scope,
            NotesHeader,
            new Dictionary<string, string>
            {
                // Seccion normal: nota junto a la tabla de mercados.
                ["34122615"] = FmRefereeOverview.FormatNote(
                    new FmRefereeInfo("Jesus Gil Manzano", "129", 5.11)),
                // Sin snapshot: fail-closed visible en su seccion.
                ["34122611"] = FmRefereeOverview.FormatNote(FmRefereeInfo.None),
                // Post-partido: nota al final de sus notas.
                ["34122617"] = FmRefereeOverview.FormatNote(
                    new FmRefereeInfo("Urs Schnyder", "5092", 4.44))
            });

        Assert.DoesNotMatch(Forbidden, with);

        // El motor no cambia: quitando las lineas de la nota (y el blanco que
        // las acompana cuando vienen tras una linea en blanco), el resto del
        // documento es byte identico al render original.
        Assert.Equal(without, StripRefereeNotes(with));

        // Las 3 secciones llevan su nota.
        Assert.Equal(3, CountRefereeNotes(with));

        var lines = with.Split('\n').Select(l => l.TrimEnd('\r')).ToArray();

        // Seccion con datos: la nota queda despues de la fila de tarjetas y
        // antes de la seccion de candidatos.
        var tarjetas = Array.FindIndex(lines, l => l.Contains("Total tarjetas"));
        var notaNormal = Array.FindIndex(
            lines, l => l.Contains("- " + FmRefereeOverview.Marker) &&
                       l.Contains("referee_id: 129"));
        var candidatos = Array.FindIndex(lines, l => l.Contains("#### Candidatos"));
        Assert.InRange(tarjetas, 0, lines.Length);
        Assert.InRange(notaNormal, tarjetas + 1, candidatos);

        // Sin snapshot: la nota sigue al texto de fail-closed del builder.
        var noData = Array.FindIndex(lines, l => l.Contains(
            P13ProxyEvReportBuilder.NoDataText));
        var notaNoData = Array.FindIndex(
            lines, l => l.Contains("- " + FmRefereeOverview.Marker) &&
                       l.Contains("referee_id: SIN DATA"));
        Assert.InRange(notaNoData, noData + 1, lines.Length);

        // Post-partido: la nota cierra las notas de su seccion.
        var postText = Array.FindIndex(lines, l => l.Contains("POST-PARTIDO"));
        var notaPost = Array.FindIndex(
            lines, l => l.Contains("- " + FmRefereeOverview.Marker) &&
                       l.Contains("referee_id: 5092"));
        Assert.InRange(notaPost, postText + 1, lines.Length);
    }

    [Fact]
    public void Render_DictionaryWithoutEntry_AddsNothingForThatFixture()
    {
        var report = MakeReport();
        var without = P13ProxyEvReportMarkdown.Render(report);
        var with = P13ProxyEvReportMarkdown.Render(
            report,
            P13ProxyEvReportMarkdown.Title,
            Scope,
            NotesHeader,
            new Dictionary<string, string>
            {
                ["34122615"] = FmRefereeOverview.FormatNote(
                    new FmRefereeInfo("Jesus Gil Manzano", "129", 5.11))
            });

        Assert.Equal(1, CountRefereeNotes(with));
        Assert.Equal(without, StripRefereeNotes(with));
    }

    // ---------------------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------------------

    private static int CountRefereeNotes(string md) =>
        Regex.Matches(
            md, @"- " + Regex.Escape(FmRefereeOverview.Marker)).Count;

    // Quita las lineas de la nota del Paso A. Si la nota vino tras una linea en
    // blanco (seccion con tabla de mercados) tambien quita el blanco que la
    // seguia, que es el que Insert anadio; el resto no se toca.
    private static string StripRefereeNotes(string md)
    {
        var lines = md.Split('\n').ToList();
        var marker = "- " + FmRefereeOverview.Marker;
        for (var i = 0; i < lines.Count; i++)
        {
            if (!lines[i].StartsWith(marker, StringComparison.Ordinal))
                continue;
            var afterBlank = i > 0 && lines[i - 1].Trim().Length == 0;
            lines.RemoveAt(i);
            if (afterBlank && i < lines.Count && lines[i].Trim().Length == 0)
                lines.RemoveAt(i);
            i--;
        }
        return string.Join('\n', lines);
    }

    // Reporte sintetico con las tres formas de seccion que el renderer puede
    // encontrar: pre-match con datos, sin snapshot y post-partido.
    private static P13Report MakeReport() => new(
        "2026-10-01 03:00 UTC",
        new[]
        {
            new P13FixtureSection(
                FixtureId: "34122615",
                Home: "Francia",
                Away: "Italia",
                KickoffUtc: "2026-10-02T18:45:00Z",
                HasData: true,
                Volume: "medio",
                ThresholdPct: 5d,
                Confidence: "Media",
                LambdaHome: 1.5,
                LambdaAway: 1.1,
                LambdaNote: null,
                Rows: new[]
                {
                    new P13MarketRow(
                        "1X2 - local", 0.45, 2.22, "2.10", 6.6, null),
                    new P13MarketRow(
                        "Total tarjetas - over 3.5", 0.40, 2.50, "2.60",
                        -3.8, null)
                },
                PlayerRows: Array.Empty<P13MarketRow>(),
                Candidates: new[]
                {
                    new P13MarketRow(
                        "1X2 - local", 0.45, 2.22, "2.10", 6.6, null)
                },
                Notes: new[] { "nota de seccion" }),
            new P13FixtureSection(
                FixtureId: "34122611",
                Home: "Kazajistan",
                Away: "Moldavia",
                KickoffUtc: null,
                HasData: false,
                Volume: "bajo",
                ThresholdPct: 5d,
                Confidence: "Baja",
                LambdaHome: null,
                LambdaAway: null,
                LambdaNote: null,
                Rows: Array.Empty<P13MarketRow>(),
                PlayerRows: Array.Empty<P13MarketRow>(),
                Candidates: Array.Empty<P13MarketRow>(),
                Notes: Array.Empty<string>()),
            new P13FixtureSection(
                FixtureId: "34122617",
                Home: "Polonia",
                Away: "Rumania",
                KickoffUtc: "2026-10-02T18:45:00Z",
                HasData: true,
                Volume: "medio",
                ThresholdPct: 5d,
                Confidence: "Media",
                LambdaHome: 1.2,
                LambdaAway: 0.9,
                LambdaNote: null,
                Rows: Array.Empty<P13MarketRow>(),
                PlayerRows: Array.Empty<P13MarketRow>(),
                Candidates: Array.Empty<P13MarketRow>(),
                Notes: new[] { "nota post" },
                IsPostMatch: true)
        },
        new[] { "nota global" });
}
