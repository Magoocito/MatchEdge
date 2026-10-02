using System.Globalization;
using System.Text;

namespace MatchEdge.Infrastructure.Services;

// P13: salida markdown de tmp/p13_uefa_2026-09-28.md. Formato exigido por el
// brief: | Mercado | P_modelo | Cuota Justa | Cuota FM (bookmaker) | Delta% |
// (nunca "EV%": ese literal rompe el regex F2 si el archivo se escanea).
public static class P13ProxyEvReportMarkdown
{
    public const string Title =
        "Analisis 360 + Diferencia modelo-mercado - UEFA Nations League - 28/09/2026";

    private const string Scope =
        "los 8 partidos del 28/09/2026 (Nations League).";

    private const string NotesHeader =
        "Notas de implementacion del P13";

    // P14: header parameterised so another matchday can reuse the exact same
    // rendering without touching the tables, Delta% or candidate logic below.
    // Render(report) keeps the frozen P13 wording used by its tests/controller.
    public static string Render(P13Report report) =>
        Render(report, Title, Scope, NotesHeader);

    public static string Render(
        P13Report report, string title, string scope, string notesHeader) =>
        Render(report, title, scope, notesHeader, refereeNotes: null);

    // PASO A (brief 02/10): nota descriptiva de arbitro por fixture_id. Con
    // diccionario null (los matchdays anteriores) la salida queda byte identica:
    // lambda, Delta%, confianza, candidatos y F1/F2 no se tocan.
    public static string Render(
        P13Report report, string title, string scope, string notesHeader,
        IReadOnlyDictionary<string, string>? refereeNotes)
    {
        var sb = new StringBuilder();

        sb.AppendLine("# " + title);
        sb.AppendLine();
        sb.AppendLine($"- Generado: {report.GeneratedAtUtc}");
        sb.AppendLine("- Alcance: " + scope);
        sb.AppendLine("- Salida analitica descriptiva: modelo Poisson bivariado con " +
                      "correccion Dixon-Coles, Cuota FM real y diferencia modelo-mercado. " +
                      "No contiene seleccion de mercados ni instrucciones de uso.");
        sb.AppendLine("- Cuota FM = mejor cuota entre las casas con fila en " +
                      "fm_market_odds para ese fixture; sin fila no se calcula Delta%.");
        sb.AppendLine();

        sb.AppendLine("## Notas del modelo");
        sb.AppendLine();
        foreach (var note in report.Notes)
            sb.AppendLine("- " + note);
        sb.AppendLine();

        sb.AppendLine("## Tabla resumen ejecutiva");
        sb.AppendLine();
        sb.AppendLine("| Partido | Mercado candidato | P_modelo | Cuota Justa | " +
                      $"Cuota FM | Delta% | Confianza | Snapshot | Volumen |");
        sb.AppendLine("|---|---|---|---|---|---|---|---|---|");
        foreach (var section in report.Fixtures)
            sb.AppendLine(SummaryRow(section));
        sb.AppendLine();

        var index = 1;
        foreach (var section in report.Fixtures)
        {
            sb.AppendLine($"### {index++}. {section.Home} vs {section.Away}");
            sb.AppendLine();
            sb.AppendLine($"- fixture_id: {section.FixtureId} · Kickoff: " +
                          $"{section.KickoffUtc ?? "n/d"} · Volumen: {section.Volume} " +
                          $"(umbral Delta% {FmtNum(section.ThresholdPct)}%) · Confianza: " +
                          $"{section.Confidence}");
            sb.AppendLine($"- Snapshot: {(section.HasData ? "OK" : "NO")}");

            if (!section.HasData)
            {
                sb.AppendLine("- " + P13ProxyEvReportBuilder.NoDataText);
                AppendRefereeNote(sb, refereeNotes, section.FixtureId);
                sb.AppendLine();
                continue;
            }

            foreach (var note in section.Notes)
                sb.AppendLine("- " + note);

            // P13 GAP1: modo automatico post-partido: solo las notas de la
            // seccion, sin tabla de mercados, sin jugadores y sin candidatos.
            if (section.IsPostMatch)
            {
                AppendRefereeNote(sb, refereeNotes, section.FixtureId);
                sb.AppendLine();
                continue;
            }

            sb.AppendLine();

            sb.AppendLine("#### Mercados");
            sb.AppendLine();
            sb.AppendLine("| Mercado | P_modelo | Cuota Justa | " +
                          $"{P13ProxyEvReportBuilder.OddsHeader} | Delta% |");
            sb.AppendLine("|---|---|---|---|---|");
            foreach (var row in section.Rows)
                sb.AppendLine(MarketRow(row));
            sb.AppendLine();

            // PASO A: la nota queda junto a la tabla de mercados (donde estan
            // las filas de tarjetas); solo si se dibuja se anade su blanco.
            if (AppendRefereeNote(sb, refereeNotes, section.FixtureId))
                sb.AppendLine();

            if (section.PlayerRows.Count > 0)
            {
                sb.AppendLine("#### Mercados de jugadores");
                sb.AppendLine();
                sb.AppendLine("| Equipo | Jugador | Mercado | P_modelo | Cuota Justa | " +
                              "Cuota FM (casa) | Delta% |");
                sb.AppendLine("|---|---|---|---|---|---|---|");
                foreach (var row in section.PlayerRows)
                    sb.AppendLine(PlayerRow(row));
                sb.AppendLine();
                sb.AppendLine("- " + P13ProxyEvReportBuilder.PlayerModelNote);
                sb.AppendLine();
            }

            sb.AppendLine($"#### Candidatos de valor (max. " +
                          $"{P13ProxyEvReportBuilder.CandidatesCap}, equipo y jugador)");
            sb.AppendLine();
            if (section.Candidates.Count == 0)
            {
                sb.AppendLine("- " + P13ProxyEvReportBuilder.NoSignalText);
                if (section.Rows.Count > 0 && section.Rows.All(r => r.OddsFm is null))
                    sb.AppendLine("- " + P13ProxyEvReportBuilder.NoOddsStatus +
                                  ": el fixture no tiene filas en fm_market_odds; sin " +
                                  "cuota no se calcula Delta% ni se evalua el umbral.");
            }
            else
            {
                var rank = 1;
                foreach (var c in section.Candidates)
                {
                    sb.AppendLine($"{rank++}. **{CandidateTitle(c)}** — " +
                                  $"P_modelo {FmtPct(c.PModel)} · " +
                                  $"Cuota Justa {FmtOdds(c.FairOdds)} · Cuota FM " +
                                  $"{c.OddsFm ?? "n/d"} · Delta% {FmtDelta(c.DeltaPct)} · " +
                                  $"umbral {FmtNum(section.ThresholdPct)}% (volumen " +
                                  $"{section.Volume}, confianza {section.Confidence})");
                }
            }
            sb.AppendLine();
        }

        sb.AppendLine("## Filtros live F1/F2 (especificacion, sin datos en vivo)");
        sb.AppendLine();
        sb.AppendLine("- F1 (Chase Over): un equipo con xG 1.2+ perdiendo al 65', 8+ tiros " +
                      "totales y 2+ tiros al arco activa la ventana Over Live 0.5 como " +
                      "candidato con valor.");
        sb.AppendLine("- F2 (Hold Under): un equipo ganando 1-0 al 60' con xGA < 0.5 y " +
                      "rival < 0.6 xG activa la ventana Under 2.5 / +1.5 AH como candidato " +
                      "con valor.");
        sb.AppendLine("- Ventanas criticas por estilo: min 15-35 (presion alta), 55-75 " +
                      "(fatiga lateral), 80+ (all-in).");
        sb.AppendLine("- Este documento es previo al partido: no contiene datos en vivo, " +
                      "las ventanas F1/F2 quedan aqui como especificacion para el " +
                      "momento del partido.");
        sb.AppendLine();

        sb.AppendLine("## " + notesHeader);
        sb.AppendLine();
        sb.AppendLine("- Wording del estado sin cuota: \"" +
                      P13ProxyEvReportBuilder.NoOddsStatus + "\" (variante DELTA en el " +
                      "header para no colisionar con el regex F2).");
        sb.AppendLine("- La tabla resumen anade la columna Mercado candidato (heredada del " +
                      "prompt anterior de diferencia modelo-mercado; el brief P13 la omite " +
                      "en su esquema).");
        sb.AppendLine("- Mercados sin modelo propio (" + P13ProxyEvReportBuilder.NoModelNote +
                      ") quedan fuera de la tabla con nota en su seccion: " +
                      "1x2_shots_on_target y los total_* de goles ya cubiertos por la matriz.");
        sb.AppendLine("- Los total_* se calculan con la media del local en casa + la del " +
                      "visitante fuera; el reporte P11 marca esas columnas como no " +
                      "reproducibles.");
        sb.AppendLine("- Confianza y volumen vienen del brief (1X2 doble para el umbral de " +
                      "alto volumen, 5% para el resto); el builder no los infiere.");
        sb.AppendLine("- Pinnacle no existe en este P13: toda comparacion es contra Cuota FM " +
                      "real, sin cuota de cierre estimada.");
        sb.AppendLine("- La tabla de jugadores anade las columnas Equipo y Jugador y usa " +
                      "Cuota FM (casa): la cuota sale de la fila exacta por jugador + " +
                      "mercado + linea + lado de fm_market_odds; sin esa fila la fila " +
                      "queda " + P13ProxyEvReportBuilder.NoOddsStatus +
                      " (no se hereda la cuota del equipo ni se interpola entre lineas).");
        sb.AppendLine("- Filas de jugador con menos de " +
                      P13ProxyEvReportBuilder.PlayerMinSample +
                      " observaciones en la ventana: " +
                      P13ProxyEvReportBuilder.MuestraInsufficientStatus +
                      ", sin Delta% y fuera del ranking de candidatos.");
        sb.AppendLine("- Solo se listan los lados de jugador con Delta% positivo; los " +
                      "lados con Delta% negativo no entran en la tabla ni en el ranking.");
        sb.AppendLine("- Los candidatos de equipo exigen P_modelo > 50% (brief); los de " +
                      "jugador no: aplican solo el umbral de Delta% por volumen, con el " +
                      "mismo umbral que los mercados de equipo.");
        sb.AppendLine("- El ranking de candidatos es unico por partido y combina equipo " +
                      "y jugador hasta " + P13ProxyEvReportBuilder.CandidatesCap +
                      " filas, ordenado por Delta% descendente.");
        sb.AppendLine("- Deteccion automatica de post-partido: si la primera captura del " +
                      "fixture (MIN source_timestamp_utc de fm_snapshot) es posterior a su " +
                      "kickoff (leido del snapshot overview persistido, JSON-LD startDate), " +
                      "la seccion entra en modo \"" +
                      P13ProxyEvReportBuilder.PostMatchText +
                      "\" sin Delta%, sin candidatos y sin mercados de jugador. No hay ningun " +
                      "flag manual: vale para cualquier fixture regenerado despues de su " +
                      "kickoff.");
        sb.AppendLine("- En modo post-partido no se calculan mercados de jugador (cuotas de " +
                      "cierre): " + P13ProxyEvReportBuilder.PlayerPostCloseNote);
        sb.AppendLine("- Fail-closed sin overview: si el fixture no tiene el tab overview " +
                      "persistido (o el startDate del JSON-LD no se puede leer) no hay kickoff " +
                      "que comparar, asi que con datos de captura la seccion queda tambien en " +
                      "modo \"" + P13ProxyEvReportBuilder.PostMatchText + "\" (kickoff n/d) en " +
                      "vez de calcular modelo, candidatos y mercados de jugador a ciegas.");
        sb.AppendLine();

        return sb.ToString();
    }

    // PASO A: inyecta la linea de arbitro de la seccion. Diccionario null o
    // fixture sin entrada = nada que anadir (salida byte identica a la de los
    // matchdays sin Paso A).
    private static bool AppendRefereeNote(
        StringBuilder sb,
        IReadOnlyDictionary<string, string>? refereeNotes,
        string fixtureId)
    {
        if (refereeNotes is null) return false;
        if (!refereeNotes.TryGetValue(fixtureId, out var note)) return false;
        sb.AppendLine("- " + note);
        return true;
    }

    private static string SummaryRow(P13FixtureSection section)
    {
        var match = $"{section.Home} vs {section.Away}";
        var snapshot = section.HasData ? "OK" : "NO";

        if (!section.HasData)
            return $"| {match} | {P13ProxyEvReportBuilder.NoDataText} | — | — | — | — | " +
                   $"{section.Confidence} | {snapshot} | {section.Volume} |";

        if (section.IsPostMatch)
            return $"| {match} | {P13ProxyEvReportBuilder.PostMatchText} | — | — | — | — | " +
                   $"{section.Confidence} | {snapshot} | {section.Volume} |";

        if (section.Candidates.Count == 0)
            return $"| {match} | {P13ProxyEvReportBuilder.NoSignalText} | — | — | — | — | " +
                   $"{section.Confidence} | {snapshot} | {section.Volume} |";

        var c = section.Candidates[0];
        return $"| {match} | {CandidateTitle(c)} | {FmtPct(c.PModel)} | {FmtOdds(c.FairOdds)} | " +
               $"{c.OddsFm ?? "n/d"} | {FmtDelta(c.DeltaPct)} | {section.Confidence} | " +
               $"{snapshot} | {section.Volume} |";
    }

    // Equipo: la etiqueta del mercado ya nombra al sujeto. Jugador: se anteponen
    // jugador y equipo para que la celda se entienda sin la tabla de jugadores.
    private static string CandidateTitle(P13MarketRow row) =>
        row.Player is null
            ? row.Market
            : $"{row.Player}{(row.Team is null ? string.Empty : $" ({row.Team})")} · {row.Market}";

    private static string MarketRow(P13MarketRow row) =>
        $"| {row.Market} | {FmtPct(row.PModel)} | {FmtOdds(row.FairOdds)} | " +
        $"{row.OddsFm ?? "—"} | {(row.OddsFm is null ? row.Status! : FmtDelta(row.DeltaPct))} |";

    private static string PlayerRow(P13MarketRow row) =>
        $"| {row.Team ?? "—"} | {row.Player ?? "—"} | {row.Market} | " +
        $"{FmtPct(row.PModel)} | {FmtOdds(row.FairOdds)} | " +
        $"{row.OddsFm ?? "—"} | {(row.OddsFm is null ? row.Status! : FmtDelta(row.DeltaPct))} |";

    private static string FmtPct(double? value) => value is double v
        ? (v * 100d).ToString("0.0", CultureInfo.InvariantCulture) + "%"
        : "—";

    private static string FmtOdds(double? value) => value is double v
        ? v.ToString("0.00", CultureInfo.InvariantCulture)
        : "—";

    private static string FmtDelta(double? value) => value is double v
        ? (v >= 0 ? "+" : string.Empty) + v.ToString("0.0", CultureInfo.InvariantCulture)
        : "—";

    private static string FmtNum(double value) =>
        value.ToString("0.#", CultureInfo.InvariantCulture);
}
