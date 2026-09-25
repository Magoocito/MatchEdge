using System.Text.Json;

namespace MatchEdge.Infrastructure.Services;

public sealed record FmWindowStats(
    string Window,
    int N,
    int? Hits,
    double? ObservedRate,
    double? Mean,
    double? Median,
    double? Min,
    double? Max,
    string Status);

public static class FmWindowCalculator
{
    public const string StatusOk = "OK";
    public const string StatusInsufficient = "INSUFFICIENT_SAMPLE";

    public static int MinSample(string subjectType) =>
        string.Equals(subjectType, "player", StringComparison.OrdinalIgnoreCase) ? 3 : 4;

    public static List<FmWindowStats> Compute(
        string subjectType, string? historyJson, double? line, string? direction)
    {
        var series = ParseSeries(historyJson, line, direction);
        var min = MinSample(subjectType);
        return new List<FmWindowStats>
        {
            ComputeWindow("5", series.Take(5).ToList(), min),
            ComputeWindow("10", series.Take(10).ToList(), min),
            ComputeWindow("all", series, min)
        };
    }

    private static FmWindowStats ComputeWindow(string label, List<(double V, bool Met)> slice, int min)
    {
        if (slice.Count < min)
            return new FmWindowStats(label, slice.Count, null, null, null, null, null, null,
                StatusInsufficient);

        var hits = slice.Count(x => x.Met);
        var values = slice.Select(x => x.V).OrderBy(v => v).ToList();
        var mean = values.Average();
        var median = values.Count % 2 == 1
            ? values[values.Count / 2]
            : (values[values.Count / 2 - 1] + values[values.Count / 2]) / 2.0;
        return new FmWindowStats(
            label, slice.Count, hits, (double)hits / slice.Count,
            mean, median, values[0], values[^1], StatusOk);
    }

    private static List<(double V, bool Met)> ParseSeries(
        string? historyJson, double? line, string? direction)
    {
        var result = new List<(double, bool)>();
        if (string.IsNullOrWhiteSpace(historyJson)) return result;

        List<JsonElement> elements;
        using (var doc = JsonDocument.Parse(historyJson))
        {
            if (doc.RootElement.ValueKind != JsonValueKind.Array) return result;
            elements = doc.RootElement.EnumerateArray().Select(e => e.Clone()).ToList();
        }

        // history[] is stored newest-first (t DESC); windows take the newest-k suffix.
        // Players carry the market value in "v"; teams carry it in "vt" (with "vf" as twin).
        foreach (var el in elements)
        {
            if (el.ValueKind != JsonValueKind.Object) continue;
            if (!TryGetDouble(el, "v", out var v) &&
                !TryGetDouble(el, "vt", out v) &&
                !TryGetDouble(el, "vf", out v))
                continue;

            bool met;
            if (el.TryGetProperty("met", out var metEl) && metEl.ValueKind is JsonValueKind.True or JsonValueKind.False)
            {
                met = metEl.GetBoolean();
            }
            else if (line is not null && direction is not null)
            {
                met = string.Equals(direction, "under", StringComparison.OrdinalIgnoreCase)
                    ? v < line.Value
                    : v > line.Value;
            }
            else
            {
                continue;
            }
            result.Add((v, met));
        }
        return result;
    }

    private static bool TryGetDouble(JsonElement el, string prop, out double value)
    {
        value = 0;
        if (!el.TryGetProperty(prop, out var v)) return false;
        switch (v.ValueKind)
        {
            case JsonValueKind.Number:
                value = v.GetDouble();
                return true;
            case JsonValueKind.String when double.TryParse(
                v.GetString(), System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out var parsed):
                value = parsed;
                return true;
            default:
                return false;
        }
    }
}
