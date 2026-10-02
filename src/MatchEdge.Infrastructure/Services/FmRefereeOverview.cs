using System.Globalization;
using System.Text.RegularExpressions;

namespace MatchEdge.Infrastructure.Services;

// PASO A (brief 02/10): nota descriptiva de arbitro leida del overview
// persistido del fixture (meta description con nombre + tarjetas por partido y
// enlaces /referees/{id} para el referee_id). Solo lectura: la nota se muestra
// junto a la seccion de tarjetas y NO entra en lambda, Delta%, confianza,
// candidatos ni F1/F2. Fail-closed por campo: lo que no este en el HTML se
// muestra como SIN DATA VERIFICADA - NO ASUMIR (nunca se inventa).
public sealed record FmRefereeInfo(
    string? Name,
    string? RefereeId,
    double? CardsPerGame)
{
    public static readonly FmRefereeInfo None = new(null, null, null);
}

public static class FmRefereeOverview
{
    public const string NoDataText = "SIN DATA VERIFICADA - NO ASUMIR";

    // Marcador exacto de la linea en el markdown (lo usan renderer y tests).
    public const string Marker = "Árbitro (dato descriptivo):";

    private static readonly Regex MetaPattern = new(
        @"referee\s+(?<name>[^,]+?),\s+who\s+averages\s+" +
        @"(?<cards>\d+(?:[.,]\d+)?)\s+cards a game",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly Regex RefLinkPattern = new(
        @"/referees/(?<id>\d+)",
        RegexOptions.CultureInvariant);

    // Parseo puro del HTML crudo del overview. Sin overview o sin patrones:
    // None (fail-closed), jamas se rellena ningun campo.
    public static FmRefereeInfo Parse(string? overviewHtml)
    {
        if (string.IsNullOrEmpty(overviewHtml))
            return FmRefereeInfo.None;

        string? name = null;
        double? cards = null;
        string? refereeId = null;

        var meta = MetaPattern.Match(overviewHtml);
        if (meta.Success)
        {
            name = meta.Groups["name"].Value.Trim();
            if (double.TryParse(
                    meta.Groups["cards"].Value, NumberStyles.Float,
                    CultureInfo.InvariantCulture, out var parsed))
                cards = parsed;
        }

        var link = RefLinkPattern.Match(overviewHtml);
        if (link.Success)
            refereeId = link.Groups["id"].Value;

        return new FmRefereeInfo(name, refereeId, cards);
    }

    public static string FormatNote(FmRefereeInfo info)
    {
        var cards = info.CardsPerGame is double value
            ? value.ToString("0.00", CultureInfo.InvariantCulture)
            : NoDataText;
        return
            $"{Marker} {info.Name ?? NoDataText} · referee_id: " +
            $"{info.RefereeId ?? NoDataText} · tarjetas/partido promedio " +
            $"(historial FM): {cards} · Solo contexto de lectura: no entra " +
            "en lambda, Delta% ni candidatos.";
    }

    // Lee el overview persistido del fixture (0 navegaciones, 0 capturas nueva:
    // reusa la fila de fm_snapshot que ya existe). Cualquier error se cierra en
    // el texto de fail-closed: una nota descriptiva nunca debe tumbar el
    // reporte.
    public static async Task<string> LoadNoteAsync(
        FmSnapshotStore store, string fixtureId, CancellationToken ct)
    {
        try
        {
            var timing = await store.GetSnapshotTimingAsync(fixtureId, ct);
            var path = timing.OverviewRawPath;
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return FormatNote(FmRefereeInfo.None);
            return FormatNote(Parse(await File.ReadAllTextAsync(path, ct)));
        }
        catch (Exception)
        {
            return FormatNote(FmRefereeInfo.None);
        }
    }
}
