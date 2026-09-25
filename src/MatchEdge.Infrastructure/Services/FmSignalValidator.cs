using System.Globalization;
using System.Text.Json;

namespace MatchEdge.Infrastructure.Services;

public static class FmSignalValidator
{
    public const string StatusOk = "OK";
    public const string StatusSuspect = "SUSPECT";

    public static (string Status, string? Motivo) Validate(FmSignalDraft s)
    {
        if (string.IsNullOrWhiteSpace(s.RecentValuesJson))
            return s.Hits is null
                ? (StatusOk, null)
                : (StatusSuspect, "history missing");

        JsonDocument doc;
        try
        {
            doc = JsonDocument.Parse(s.RecentValuesJson);
        }
        catch (Exception ex)
        {
            return (StatusSuspect, $"history unparseable: {ex.Message}");
        }

        using (doc)
        {
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                return (StatusSuspect, "history not an array");

            var field = s.SubjectType == "player" ? "v" : "vt";
            var values = new List<double>();
            foreach (var el in doc.RootElement.EnumerateArray())
            {
                if (el.ValueKind != JsonValueKind.Object ||
                    !el.TryGetProperty(field, out var v))
                    return (StatusSuspect, $"history element missing '{field}'");

                double? num = v.ValueKind switch
                {
                    JsonValueKind.Number => v.GetDouble(),
                    JsonValueKind.String when double.TryParse(
                        v.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var d) => d,
                    _ => null
                };
                if (num is null)
                    return (StatusSuspect, $"non-numeric history value '{field}'");
                values.Add(num.Value);
            }

            if (s.SampleSize is not int sample) return (StatusOk, null);
            if (sample > values.Count)
                return (StatusSuspect,
                    $"sample_size {sample} > history len {values.Count}");

            if (s.Line is double line && s.Hits is int bestCount)
            {
                var minMatches = s.SubjectType == "player" ? 3 : 4;
                if (values.Count >= minMatches)
                {
                    var (hits, window) =
                        BestWindow(values, line, s.Direction, minMatches);
                    if (hits != bestCount || window != sample)
                        return (StatusSuspect,
                            $"recomputed {hits}/{window} != bestCount {bestCount}/{sample}");
                }
            }
        }

        return (StatusOk, null);
    }

    public static (int Hits, int Window) BestWindow(
        IReadOnlyList<double> values, double line, string? direction, int minMatches)
    {
        var under = string.Equals(direction, "under", StringComparison.OrdinalIgnoreCase);
        var n = values.Count;
        var bestHits = 0;
        var bestK = minMatches;
        var bestRate = -1.0;

        for (var k = minMatches; k <= n; k++)
        {
            var hits = 0;
            for (var i = 0; i < k; i++)
            {
                if (under ? values[i] < line : values[i] > line) hits++;
            }

            var rate = (double)hits / k;
            if (rate > bestRate + 1e-9 ||
                (Math.Abs(rate - bestRate) < 1e-9 && k > bestK))
            {
                bestRate = rate;
                bestHits = hits;
                bestK = k;
            }
        }

        return (bestHits, bestK);
    }
}
