using System.Globalization;
using System.Text.Json;

namespace MatchEdge.Infrastructure.Services;

public sealed record FmSignalDraft(
    string SubjectType,
    string SubjectName,
    string? Market,
    double? Line,
    string? Direction,
    int? Hits,
    int? SampleSize,
    double? ObservedHitRate,
    double? OppHits,
    int? OppSampleSize,
    double? ConfidenceScore,
    string? RecentValuesJson);

public static class FmTrendsJsonParser
{
    public const string ParserVersion = "fm-json-v1";

    public static List<FmSignalDraft> Parse(string json, string subjectType)
    {
        var result = new List<FmSignalDraft>();
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.ValueKind != JsonValueKind.Object ||
            !doc.RootElement.TryGetProperty("data", out var data) ||
            data.ValueKind != JsonValueKind.Array)
            return result;

        foreach (var item in data.EnumerateArray())
        {
            var name = subjectType == "player"
                ? GetString(item, "shortName")
                : GetString(item, "name");
            if (string.IsNullOrWhiteSpace(name)) continue;

            var hits = GetInt(item, "bestCount");
            var sample = GetInt(item, "bestTotal");
            double? observed = hits.HasValue && sample is > 0
                ? hits.Value / (double)sample.Value
                : null;

            var oppHits = GetDouble(item, "oppCount");
            var oppSample = GetInt(item, "oppTotal");

            string? recent = null;
            if (item.TryGetProperty("history", out var hist) &&
                hist.ValueKind == JsonValueKind.Array)
                recent = hist.GetRawText();

            result.Add(new FmSignalDraft(
                SubjectType: subjectType,
                SubjectName: name!,
                Market: GetString(item, "market") ?? GetString(item, "key"),
                Line: GetDouble(item, "line"),
                Direction: GetString(item, "direction"),
                Hits: hits,
                SampleSize: sample,
                ObservedHitRate: observed,
                OppHits: oppHits,
                OppSampleSize: oppSample,
                ConfidenceScore: GetDouble(item, "score"),
                RecentValuesJson: recent));
        }

        return result;
    }

    private static string? GetString(JsonElement el, string prop) =>
        el.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String
            ? v.GetString()
            : null;

    private static int? GetInt(JsonElement el, string prop)
    {
        if (!el.TryGetProperty(prop, out var v)) return null;
        if (v.ValueKind == JsonValueKind.Number && v.TryGetInt32(out var i)) return i;
        if (v.ValueKind == JsonValueKind.String &&
            int.TryParse(v.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var s))
            return s;
        return null;
    }

    private static double? GetDouble(JsonElement el, string prop)
    {
        if (!el.TryGetProperty(prop, out var v)) return null;
        if (v.ValueKind == JsonValueKind.Number) return v.GetDouble();
        if (v.ValueKind == JsonValueKind.String &&
            double.TryParse(v.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
            return d;
        return null;
    }
}

public static class FmCanary
{
    public const double DefaultThreshold = 0.30;

    public static bool ExceedsDeviation(
        IReadOnlyDictionary<string, int> current,
        IReadOnlyDictionary<string, int> previous,
        double threshold = DefaultThreshold)
    {
        foreach (var (market, prevCount) in previous)
        {
            if (prevCount <= 0) continue;
            if (!current.TryGetValue(market, out var curCount)) curCount = 0;
            var deviation = Math.Abs(curCount - prevCount) / (double)prevCount;
            if (deviation > threshold) return true;
        }
        return false;
    }
}
