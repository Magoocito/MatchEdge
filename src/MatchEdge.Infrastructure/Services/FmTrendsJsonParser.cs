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
    string? RecentValuesJson,
    string? TeamName = null,
    string? FixtureHome = null,
    string? FixtureAway = null,
    string? VenueRole = null);

public sealed record FmOddsDraft(
    string SubjectType,
    string SubjectName,
    string? Market,
    double? Line,
    string Bookmaker,
    double OddsValue,
    string Side);

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

            // P5 B: structural venue of THIS fixture from the row itself
            // (player rows: Team.name + Fixture.Home/Away; team rows: same).
            var teamName = GetNestedString(item, "Team", "name");
            var fixtureHome = GetNestedString(item, "Fixture", "Home", "name");
            var fixtureAway = GetNestedString(item, "Fixture", "Away", "name");
            var venueRole = string.IsNullOrEmpty(teamName) ? "unknown"
                : teamName == fixtureHome ? "home"
                : teamName == fixtureAway ? "away"
                : "unknown";

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
                RecentValuesJson: recent,
                TeamName: teamName,
                FixtureHome: fixtureHome,
                FixtureAway: fixtureAway,
                VenueRole: venueRole));
        }

        return result;
    }

    // C2: odds live in data[].odds = { bk, over, under } (bookmaker ids 1..5,
    // verified by requesting a single bookmaker). One row per offered side.
    public static List<FmOddsDraft> ParseOdds(string json, string subjectType)
    {
        var result = new List<FmOddsDraft>();
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.ValueKind != JsonValueKind.Object ||
            !doc.RootElement.TryGetProperty("data", out var data) ||
            data.ValueKind != JsonValueKind.Array)
            return result;

        foreach (var item in data.EnumerateArray())
        {
            var subject = subjectType == "player"
                ? GetString(item, "shortName")
                : GetString(item, "name");
            if (string.IsNullOrWhiteSpace(subject)) continue;

            if (!item.TryGetProperty("odds", out var oddsNode)) continue;
            // odds arrives as an array of {bk, over, under} entries (one per
            // offered bookmaker); tolerate a bare object too.
            var entries = oddsNode.ValueKind switch
            {
                JsonValueKind.Array => oddsNode.EnumerateArray().ToList(),
                JsonValueKind.Object => new List<JsonElement> { oddsNode },
                _ => new List<JsonElement>()
            };
            if (entries.Count == 0) continue;

            var market = GetString(item, "market") ?? GetString(item, "key");
            var line = GetDouble(item, "line");

            foreach (var odds in entries)
            {
                if (odds.ValueKind != JsonValueKind.Object) continue;
                var bk = GetDouble(odds, "bk");
                if (bk is null) continue;

                foreach (var side in new[] { "over", "under" })
                {
                    var value = GetDouble(odds, side);
                    if (value is null or <= 0) continue;
                    result.Add(new FmOddsDraft(
                        subjectType, subject!, market, line,
                        bk.Value.ToString(CultureInfo.InvariantCulture),
                        value.Value, side));
                }
            }
        }

        return result;
    }

    private static string? GetString(JsonElement el, string prop) =>
        el.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String
            ? v.GetString()
            : null;

    private static string? GetNestedString(JsonElement el, params string[] path)
    {
        var current = el;
        foreach (var prop in path)
        {
            if (current.ValueKind != JsonValueKind.Object ||
                !current.TryGetProperty(prop, out var next))
                return null;
            current = next;
        }
        return current.ValueKind == JsonValueKind.String ? current.GetString() : null;
    }

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
