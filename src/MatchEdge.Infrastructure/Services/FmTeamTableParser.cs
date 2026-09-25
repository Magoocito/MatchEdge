using System.Globalization;
using System.Text.Json;

namespace MatchEdge.Infrastructure.Services;

public sealed record FmTeamMatchDraft(
    long TeamApid,
    string FixtureId,
    long FixtureApid,
    DateTime TsUtc,
    string Location,
    long OpponentApid,
    string? Opponent,
    long? LeagueApid,
    string? League,
    int? HGoals,
    int? AGoals,
    string? TeamStatsJson,
    string? OpponentStrengthJson,
    int Period,
    string Stat);

public sealed record FmPlayerMatchDraft(
    long TeamApid,
    long PlayerApid,
    string? PlayerName,
    string FixtureId,
    long FixtureApid,
    DateTime TsUtc,
    string Location,
    string Perspective,
    string? StatsJson,
    int Period,
    string Stat);

public sealed record FmTeamTableParseResult(
    List<FmTeamMatchDraft> TeamMatches,
    List<FmPlayerMatchDraft> PlayerMatches,
    List<string> Warnings);

// P6 G1: /api/front/teams/table?...&location=home|away payload.
// Fixture-level venue comes from hid/aid (structural, same rule as the
// trends row venue_role), never from the requested location parameter.
public static class FmTeamTableParser
{
    public const string ParserVersion = "fm-team-table-v1";

    private sealed record FxInfo(
        string FixtureId,
        long FixtureApid,
        DateTime TsUtc,
        string Location,
        long OpponentApid,
        string? Opponent,
        long? LeagueApid);

    public static FmTeamTableParseResult Parse(
        string json, long teamApid, string requestedLocation, int period, string stat)
    {
        var teamMatches = new List<FmTeamMatchDraft>();
        var playerMatches = new List<FmPlayerMatchDraft>();
        var warnings = new List<string>();

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty("fixtures", out var fixtures) ||
            fixtures.ValueKind != JsonValueKind.Array)
        {
            warnings.Add("payload without fixtures[]");
            return new FmTeamTableParseResult(teamMatches, playerMatches, warnings);
        }

        var leagues = new Dictionary<string, string>();
        if (root.TryGetProperty("leagues", out var leaguesNode) &&
            leaguesNode.ValueKind == JsonValueKind.Object)
        {
            foreach (var league in leaguesNode.EnumerateObject())
            {
                var name = GetNestedString(league.Value, "name");
                if (!string.IsNullOrEmpty(name)) leagues[league.Name] = name;
            }
        }

        var names = new Dictionary<string, string>();
        if (root.TryGetProperty("players", out var playersNode) &&
            playersNode.ValueKind == JsonValueKind.Array)
        {
            foreach (var player in playersNode.EnumerateArray())
            {
                var apid = GetLong(player, "apid");
                var shortName = GetString(player, "shortName");
                if (apid.HasValue && !string.IsNullOrEmpty(shortName))
                    names[apid.Value.ToString(CultureInfo.InvariantCulture)] = shortName;
            }
        }

        var byFixtureApid = new Dictionary<string, FxInfo>();
        foreach (var fixture in fixtures.EnumerateArray())
        {
            var fixtureId = GetString(fixture, "id");
            var fixtureApid = GetLong(fixture, "apid");
            var hidden = GetLong(fixture, "hid");
            var awayId = GetLong(fixture, "aid");
            if (string.IsNullOrEmpty(fixtureId) || !fixtureApid.HasValue ||
                !hidden.HasValue || !awayId.HasValue)
            {
                warnings.Add("fixture skipped: missing id/apid/hid/aid");
                continue;
            }

            string? location = null;
            long? opponentApid = null;
            if (hidden.Value == teamApid)
            {
                location = "home";
                opponentApid = awayId.Value;
            }
            else if (awayId.Value == teamApid)
            {
                location = "away";
                opponentApid = hidden.Value;
            }

            if (location == null)
            {
                warnings.Add($"fixture {fixtureId}: team {teamApid} is neither hid nor aid");
                continue;
            }

            if (!string.Equals(location, requestedLocation, StringComparison.OrdinalIgnoreCase))
                warnings.Add($"fixture {fixtureId}: location={location} differs from requested {requestedLocation}");

            var timestamp = GetString(fixture, "timestamp");
            if (string.IsNullOrEmpty(timestamp) ||
                !DateTime.TryParse(timestamp, CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind, out var ts))
            {
                warnings.Add($"fixture {fixtureId}: unparseable timestamp '{timestamp}'");
                ts = DateTime.UtcNow;
            }

            var opponent = GetNestedString(fixture, "opponent", "name");
            long? leagueApid = GetLong(fixture, "lid");
            var info = new FxInfo(
                fixtureId, fixtureApid.Value, ts.ToUniversalTime(), location,
                opponentApid!.Value, opponent,
                leagueApid.HasValue && leagues.ContainsKey(leagueApid.Value.ToString(CultureInfo.InvariantCulture))
                    ? leagueApid
                    : null);

            byFixtureApid[info.FixtureApid.ToString(CultureInfo.InvariantCulture)] = info;

            var leagueName = leagueApid.HasValue &&
                             leagues.TryGetValue(leagueApid.Value.ToString(CultureInfo.InvariantCulture), out var ln)
                ? ln
                : null;

            teamMatches.Add(new FmTeamMatchDraft(
                TeamApid: teamApid,
                FixtureId: info.FixtureId,
                FixtureApid: info.FixtureApid,
                TsUtc: info.TsUtc,
                Location: info.Location,
                OpponentApid: info.OpponentApid,
                Opponent: info.Opponent,
                LeagueApid: info.LeagueApid ?? leagueApid,
                League: leagueName,
                HGoals: GetInt(fixture, "hgoals"),
                AGoals: GetInt(fixture, "agoals"),
                TeamStatsJson: TeamStatsFor(root, info, teamApid),
                OpponentStrengthJson: OpponentStrengthFor(root, info),
                Period: period,
                Stat: stat));
        }

        if (root.TryGetProperty("pivotData", out var pivot) &&
            pivot.ValueKind == JsonValueKind.Object)
        {
            foreach (var playerProp in pivot.EnumerateObject())
            {
                if (!long.TryParse(playerProp.Name, NumberStyles.Integer,
                        CultureInfo.InvariantCulture, out var playerApid))
                    continue;
                if (playerProp.Value.ValueKind != JsonValueKind.Object) continue;

                names.TryGetValue(playerProp.Name, out var playerName);

                foreach (var fixtureProp in playerProp.Value.EnumerateObject())
                {
                    if (!byFixtureApid.TryGetValue(fixtureProp.Name, out var info))
                    {
                        warnings.Add(
                            $"pivot player {playerProp.Name}: fixture {fixtureProp.Name} outside fixtures[]");
                        continue;
                    }

                    playerMatches.Add(new FmPlayerMatchDraft(
                        TeamApid: teamApid,
                        PlayerApid: playerApid,
                        PlayerName: playerName,
                        FixtureId: info.FixtureId,
                        FixtureApid: info.FixtureApid,
                        TsUtc: info.TsUtc,
                        Location: info.Location,
                        Perspective: "team",
                        StatsJson: fixtureProp.Value.GetRawText(),
                        Period: period,
                        Stat: stat));
                }
            }
        }
        else
        {
            warnings.Add("payload without pivotData{}");
        }

        return new FmTeamTableParseResult(teamMatches, playerMatches, warnings);
    }

    // teamStats = { "<fixtureApid>": { "<teamApid|opponentApid>": { ...stats } } }
    // Both sides are present: the subject team's own stats are the ones we keep.
    private static string? TeamStatsFor(JsonElement root, FxInfo info, long teamApid)
    {
        if (!root.TryGetProperty("teamStats", out var byFixture) ||
            byFixture.ValueKind != JsonValueKind.Object)
            return null;
        var key = info.FixtureApid.ToString(CultureInfo.InvariantCulture);
        if (!byFixture.TryGetProperty(key, out var byTeam) ||
            byTeam.ValueKind != JsonValueKind.Object)
            return null;

        var teamKey = teamApid.ToString(CultureInfo.InvariantCulture);
        if (byTeam.TryGetProperty(teamKey, out var ownStats))
            return ownStats.GetRawText();

        var opponentKey = info.OpponentApid.ToString(CultureInfo.InvariantCulture);
        if (byTeam.TryGetProperty(opponentKey, out var oppStats))
            return oppStats.GetRawText();

        foreach (var entry in byTeam.EnumerateObject())
            return entry.Value.GetRawText();
        return null;
    }

    private static string? OpponentStrengthFor(JsonElement root, FxInfo info)
    {
        if (!root.TryGetProperty("opponentStrength", out var byFixture) ||
            byFixture.ValueKind != JsonValueKind.Object)
            return null;
        var key = info.FixtureApid.ToString(CultureInfo.InvariantCulture);
        return byFixture.TryGetProperty(key, out var value) &&
               value.ValueKind is JsonValueKind.Object or JsonValueKind.Array
            ? value.GetRawText()
            : null;
    }

    private static string? GetString(JsonElement el, string prop) =>
        el.ValueKind == JsonValueKind.Object &&
        el.TryGetProperty(prop, out var v) &&
        v.ValueKind == JsonValueKind.String
            ? v.GetString()
            : null;

    private static long? GetLong(JsonElement el, string prop)
    {
        if (el.ValueKind != JsonValueKind.Object || !el.TryGetProperty(prop, out var v))
            return null;
        if (v.ValueKind == JsonValueKind.Number && v.TryGetInt64(out var i)) return i;
        if (v.ValueKind == JsonValueKind.String &&
            long.TryParse(v.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var s))
            return s;
        return null;
    }

    private static int? GetInt(JsonElement el, string prop)
    {
        if (el.ValueKind != JsonValueKind.Object || !el.TryGetProperty(prop, out var v))
            return null;
        if (v.ValueKind == JsonValueKind.Number && v.TryGetInt32(out var i)) return i;
        if (v.ValueKind == JsonValueKind.String &&
            int.TryParse(v.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var s))
            return s;
        return null;
    }

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
}
