using System.Globalization;
using System.Text;

namespace MatchEdge.Infrastructure.Services;

// P7 E5/E6: fixed-section markdown view rendered from the same FmReport object
// as GET /report. Pure string templating (no LLM): never reorders, never
// summarizes with judgement, never drops poor-data markets.
public static class FmConfluenceReportMarkdown
{
    // E5 mandates this closing line verbatim.
    public const string ClosingNote =
        "Este reporte es descriptivo. No constituye una recomendación de apuesta " +
        "ni una probabilidad validada de resultado futuro.";

    public const string OrderLabel = "orden: mayor muestra disponible, no fuerza de señal";

    public static string Render(FmReport report)
    {
        var sb = new StringBuilder();

        AppendHeader(sb, report);
        AppendMarkets(sb, report);
        AppendPlayers(sb, report);
        // P11 B4: fixed evidence section, always between "Señales de jugadores"
        // and "Cuotas" - the five pre-existing sections keep their order.
        AppendEvidence(sb, report);
        // P12: fixed confluence section, always between the evidence block and
        // "Cuotas"; the pre-existing section order is untouched.
        AppendConfluence(sb, report);
        AppendOdds(sb, report);

        // Closing note of the template: fixed wording (E5), never reworded.
        sb.Append('\n');
        sb.Append("## Nota de cierre\n");
        sb.Append(ClosingNote).Append('\n');
        return sb.ToString();
    }

    private static void AppendHeader(StringBuilder sb, FmReport report)
    {
        var f = report.Fixture;
        sb.Append("# ").Append(f.Home ?? "?").Append(" vs ").Append(f.Away ?? "?").Append('\n');
        sb.Append("- Competición: ").Append(string.IsNullOrWhiteSpace(f.Competition) ? "desconocida" : f.Competition).Append('\n');
        sb.Append("- Kickoff (UTC): ").Append(f.KickoffUtc ?? "desconocido").Append('\n');
        sb.Append("- Flags de calidad: leakage=").Append(f.LeakageFlag ? "true" : "false")
            .Append("; suspect=").Append(AnySuspect(report) ? "true" : "false").Append('\n');
        sb.Append("- sort_criteria: ").Append(report.SortCriteria).Append('\n');
    }

    private static void AppendMarkets(StringBuilder sb, FmReport report)
    {
        sb.Append('\n');
        sb.Append("## Mercados (").Append(OrderLabel).Append(")\n");

        var index = 0;
        foreach (var m in report.Markets)
        {
            index++;
            sb.Append('\n');
            sb.Append("### ").Append(index).Append(". ").Append(m.Market);
            if (m.Line is not null)
                sb.Append(" @ ").Append(Num(m.Line, "0.##"));
            sb.Append(" — ").Append(m.Subject);
            sb.Append(" (").Append(m.SubjectRole ?? "sin rol").Append(")\n");
            sb.Append("- basis: ").Append(m.Basis).Append('\n');

            AppendAttack(sb, m.TeamAttack);

            sb.Append("- Rival (contexto equivalente):\n");
            AppendAttack(sb, m.OpponentContext.ConcededEquivalent, "  ");

            AppendFlags(sb, m.OverlapFlags);
            AppendDataQuality(sb, m.DataQuality);
        }
    }

    private static void AppendPlayers(StringBuilder sb, FmReport report)
    {
        sb.Append('\n');
        sb.Append("## Señales de jugadores (").Append(OrderLabel).Append(")\n");

        if (report.PlayerSignals.Count == 0)
        {
            sb.Append("- ninguna señal de jugador persistida\n");
            return;
        }

        var index = 0;
        foreach (var p in report.PlayerSignals)
        {
            index++;
            sb.Append('\n');
            sb.Append("### ").Append(index).Append(". ").Append(p.Market);
            if (p.Line is not null)
                sb.Append(" @ ").Append(Num(p.Line, "0.##"));
            sb.Append(" — ").Append(p.Subject).Append('\n');
            sb.Append("- basis: ").Append(p.Basis).Append('\n');
            sb.Append("- FM reportado: ").Append(FmReportedText(p.FmReported)).Append('\n');
            sb.Append("- Ventanas propias:\n");
            AppendWindows(sb, p.OwnWindows, "  ");
            AppendFlags(sb, p.OverlapFlags);
            AppendDataQuality(sb, p.DataQuality, "  ");
        }
    }

    // P11 A2/B4: one line per prop, always the same template:
    //   Nombre — market @ line: [secuencia] | last5 h/n | last10 h/n | all h/n
    // The teams block adds an indented rival line only when the opponent has
    // its own series (nothing is invented when it does not).
    private static void AppendEvidence(StringBuilder sb, FmReport report)
    {
        sb.Append('\n');
        sb.Append("## Evidencia histórica (ventanas fijas)\n");
        sb.Append("- observaciones descriptivas: secuencia cruda (más antiguo -> más reciente) y hits de las ventanas fijas (last5/last10/all)\n");

        sb.Append('\n');
        sb.Append("### Jugadores — evidencia con ventanas fijas\n");
        if (report.PlayerEvidence.Count == 0)
        {
            sb.Append("- sin señales de jugador con valores observados en la DB\n");
        }
        else
        {
            foreach (var e in report.PlayerEvidence)
            {
                AppendEvidenceLine(sb, e);
                AppendEvidenceContext(sb, e);
            }
        }

        sb.Append('\n');
        sb.Append("### Equipos — evidencia con ventanas fijas\n");
        if (report.TeamEvidence.Count == 0)
        {
            sb.Append("- sin señales de equipo con valores observados en la DB\n");
        }
        else
        {
            foreach (var e in report.TeamEvidence)
            {
                AppendEvidenceLine(sb, e);
                if (e.RivalContext is { } rival)
                {
                    sb.Append("  - Rival (").Append(rival.Subject).Append("): ")
                        .Append(SequenceText(rival.Sequence))
                        .Append(" | ").Append(EvidenceWindowsText(rival.Windows))
                        .Append('\n');
                }
                AppendEvidenceContext(sb, e);
            }
        }
    }

    // P12 B1/B4: extra context lines only when there is something to report,
    // so a clean fixture keeps the exact P11 line template.
    private static void AppendEvidenceContext(StringBuilder sb, FmReportEvidence e)
    {
        if (e.OverlapFlags.Count > 0)
        {
            sb.Append("  - overlap_flags:\n");
            foreach (var flag in e.OverlapFlags) sb.Append("    - ").Append(flag).Append('\n');
        }

        var q = e.DataQuality;
        if (q.Suspect || q.SourceConflict || !string.IsNullOrWhiteSpace(q.Motivo))
        {
            sb.Append("  - data_quality: suspect=").Append(q.Suspect ? "true" : "false")
                .Append(", source_conflict=").Append(q.SourceConflict ? "true" : "false")
                .Append(", motivo=\"").Append(q.Motivo).Append("\"\n");
        }
    }

    // P12: explicit confluence block (team + rival + players) grouped by
    // market family. Descriptive only: each strand shows its own line and the
    // same fixed windows as the evidence block; nothing is scored or ranked.
    private static void AppendConfluence(StringBuilder sb, FmReport report)
    {
        sb.Append('\n');
        sb.Append("## Confluencia descriptiva (equipo + rival + jugadores)\n");
        sb.Append("- familias de mercado con 2 o más series observadas en la DB; cada serie conserva su línea y sus ventanas fijas (last5/last10/all); sin score ni orden por fuerza\n");

        if (report.Confluence.Count == 0)
        {
            sb.Append("- sin familias con 2 o más series observadas\n");
            return;
        }

        foreach (var c in report.Confluence)
        {
            sb.Append('\n');
            sb.Append("### ").Append(c.Family).Append('\n');
            foreach (var t in c.Teams)
            {
                sb.Append("- ").Append(t.Subject);
                if (t.Role is not null) sb.Append(" (").Append(t.Role).Append(')');
                sb.Append(" @ ").Append(Num(t.Line, "0.##"))
                    .Append(": ").Append(EvidenceWindowsText(t.Windows)).Append('\n');
            }
            foreach (var p in c.Players)
            {
                sb.Append("- jugador ").Append(p.Subject).Append(" — ").Append(p.Market);
                sb.Append(" @ ").Append(Num(p.Line, "0.##"))
                    .Append(": ").Append(EvidenceWindowsText(p.Windows)).Append('\n');
            }
        }
    }

    private static void AppendEvidenceLine(StringBuilder sb, FmReportEvidence e)
    {
        sb.Append("- ").Append(e.Subject).Append(" — ").Append(e.Market);
        if (e.Line is not null)
            sb.Append(" @ ").Append(Num(e.Line, "0.##"));
        sb.Append(": ").Append(SequenceText(e.Sequence))
            .Append(" | ").Append(EvidenceWindowsText(e.Windows)).Append('\n');
    }

    private static string SequenceText(IEnumerable<double> sequence) =>
        "[" + string.Join(",", sequence.Select(v =>
            v.ToString("0.##", CultureInfo.InvariantCulture))) + "]";

    private static string EvidenceWindowsText(Dictionary<string, FmReportEvidenceWindow> windows) =>
        string.Join(" | ", new[] { "last5", "last10", "all" }.Select(key =>
            key + " " + (windows.TryGetValue(key, out var w)
                ? (w.Hits?.ToString(CultureInfo.InvariantCulture) ?? "-") + "/" + w.N
                : "-/-")));

    private static void AppendOdds(StringBuilder sb, FmReport report)
    {
        sb.Append('\n');
        sb.Append("## Cuotas\n");

        var any = report.Markets.Any(m => m.MarketOddsFm.Count > 0);
        if (!any)
        {
            sb.Append("- sin cuotas FM persistidas para este fixture\n");
        }
        else
        {
            sb.Append('\n');
            sb.Append("| mercado | línea | casa | lado | cuota | prob. implícita | capturada |\n");
            sb.Append("|---|---|---|---|---|---|---|\n");
            foreach (var m in report.Markets)
            {
                foreach (var o in m.MarketOddsFm)
                {
                    sb.Append("| ").Append(m.Market)
                        .Append(" | ").Append(m.Line is null ? "-" : Num(m.Line, "0.##"))
                        .Append(" | ").Append(o.Bookmaker)
                        .Append(" | ").Append(o.Side ?? "-")
                        .Append(" | ").Append(Num(o.Value, "0.###"))
                        .Append(" | ").Append(Num(o.ImpliedProb, "0.000"))
                        .Append(" | ").Append(o.CapturedAt)
                        .Append(" |\n");
                }
            }
        }

        foreach (var m in report.Markets)
        {
            var manual = m.ManualOdds;
            if (manual.Count == 0)
            {
                sb.Append("- Cuota manual Betano (").Append(m.Market);
                if (m.Line is not null) sb.Append(" @ ").Append(Num(m.Line, "0.##"));
                sb.Append("): no cargada\n");
                continue;
            }
            foreach (var entry in manual)
            {
                sb.Append("- Cuota manual ").Append(entry.Bookmaker)
                    .Append(" (").Append(m.Market);
                if (m.Line is not null) sb.Append(" @ ").Append(Num(m.Line, "0.##"));
                sb.Append(", ").Append(string.IsNullOrWhiteSpace(entry.Side) ? "-" : entry.Side)
                    .Append("): ")
                    .Append(Num(entry.Value, "0.###"))
                    .Append(" (prob. impl. ").Append(Num(entry.ImpliedProb, "0.000"));
                if (entry.CapturedAt is not null)
                    sb.Append(", capturada ").Append(entry.CapturedAt);
                sb.Append(")\n");
            }
        }
    }

    private static void AppendAttack(StringBuilder sb, FmReportAttack attack, string indent = "- ")
    {
        sb.Append(indent).Append("FM reportado: ").Append(FmReportedText(attack.FmReported)).Append('\n');
        sb.Append(indent).Append("Ventanas propias:\n");
        AppendWindows(sb, attack.OwnWindows, indent + "  ");
        sb.Append(indent).Append("Split home/away:\n");
        AppendVenue(sb, attack.VenueSplit, indent + "  ");
    }

    private static void AppendWindows(
        StringBuilder sb, Dictionary<string, object> windows, string indent)
    {
        foreach (var key in new[] { "last5", "last10", "all" })
        {
            sb.Append(indent).Append(key).Append(": ");
            if (!windows.TryGetValue(key, out var value) || value is not FmReportWindow w)
            {
                sb.Append(FmConfluenceReportBuilder.StatusInsufficient).Append('\n');
                continue;
            }
            sb.Append("n=").Append(w.N)
                .Append(", hits=").Append(w.Hits?.ToString(CultureInfo.InvariantCulture) ?? "-")
                .Append(", rate=").Append(Num(w.Rate, "0.000"))
                .Append(", mean=").Append(Num(w.Mean, "0.####"))
                .Append(", median=").Append(Num(w.Median, "0.####"))
                .Append(", min=").Append(Num(w.Min, "0.####"))
                .Append(", max=").Append(Num(w.Max, "0.####"))
                .Append('\n');
        }
    }

    private static void AppendVenue(
        StringBuilder sb, Dictionary<string, object> venue, string indent)
    {
        foreach (var key in new[] { "home", "away" })
        {
            sb.Append(indent).Append(key).Append(": ");
            if (!venue.TryGetValue(key, out var value) || value is not FmReportVenue v)
            {
                sb.Append(FmConfluenceReportBuilder.StatusNoData).Append('\n');
                continue;
            }
            sb.Append("n=").Append(v.N)
                .Append(", hits=").Append(v.Hits?.ToString(CultureInfo.InvariantCulture) ?? "-")
                .Append(", rate=").Append(Num(v.Rate, "0.000"))
                .Append(", mean=").Append(Num(v.Mean, "0.####"))
                .Append(", median=").Append(Num(v.Median, "0.####"))
                .Append('\n');
        }
    }

    private static void AppendFlags(StringBuilder sb, List<string> flags)
    {
        if (flags.Count == 0)
        {
            sb.Append("- overlap_flags: ninguno\n");
            return;
        }
        sb.Append("- overlap_flags:\n");
        foreach (var flag in flags) sb.Append("  - ").Append(flag).Append('\n');
    }

    private static void AppendDataQuality(
        StringBuilder sb, FmReportDataQuality quality, string indent = "- ")
    {
        sb.Append(indent).Append("data_quality: suspect=").Append(quality.Suspect ? "true" : "false")
            .Append(", source_conflict=").Append(quality.SourceConflict ? "true" : "false")
            .Append(", motivo=\"").Append(quality.Motivo).Append("\"\n");
    }

    private static string FmReportedText(FmReportFmReported? reported)
    {
        if (reported is null) return "no disponible";
        var text = "hits " +
                   (reported.Hits?.ToString(CultureInfo.InvariantCulture) ?? "-") + "/" +
                   (reported.Sample?.ToString(CultureInfo.InvariantCulture) ?? "-");
        if (reported.BestWindow is { } best)
        {
            text += "; mejor ventana: " + best.Window + " (" +
                    (best.Hits?.ToString(CultureInfo.InvariantCulture) ?? "-") + "/" +
                    (best.N?.ToString(CultureInfo.InvariantCulture) ?? "-") + ")";
        }
        else
        {
            text += "; mejor ventana: no determinable";
        }
        return text;
    }

    private static bool AnySuspect(FmReport report) =>
        report.Markets.Any(m => m.DataQuality.Suspect) ||
        report.PlayerSignals.Any(p => p.DataQuality.Suspect);

    private static string Num(double? value, string format) =>
        value?.ToString(format, CultureInfo.InvariantCulture) ?? "-";
}
