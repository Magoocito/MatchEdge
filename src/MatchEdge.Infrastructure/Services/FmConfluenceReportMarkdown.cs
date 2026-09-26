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
        AppendOdds(sb, report);

        // Section 5 of 5: fixed closing note, verbatim (E5), never reworded.
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
            sb.Append("- Cuota manual ").Append(manual.Bookmaker)
                .Append(" (").Append(m.Market);
            if (m.Line is not null) sb.Append(" @ ").Append(Num(m.Line, "0.##"));
            sb.Append("): ");
            if (manual.Value is null)
            {
                sb.Append("no cargada\n");
            }
            else
            {
                sb.Append(Num(manual.Value, "0.###"))
                    .Append(" (prob. impl. ").Append(Num(manual.ImpliedProb, "0.000"));
                if (manual.CapturedAt is not null)
                    sb.Append(", capturada ").Append(manual.CapturedAt);
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
