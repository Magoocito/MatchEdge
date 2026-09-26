namespace MatchEdge.Infrastructure.Services;

// PBI 2.1 Parte C: single source of truth for the five player stats that used
// to come back from the report as "missing or non-numeric in
// fm_player_matches.stats_json" (147 of 220 player signals, 137 of them core).
//
// Three namespaces are joined here:
//   market - our signal/report market name (home_/away_ prefix already stripped)
//   slug   - FootyMetrics ?stat= value used by /api/front/teams/table and
//            /api/front/position-stats
//   field  - key inside fm_player_matches.stats_json, first numeric wins
//
// The group is NOT cosmetic: FootyMetrics keys the teams/table payload on
// `group`, so stat=tackles without group=defense returns the attack pivot and
// never carries a tackles column (verified 2026-09-26, PBI 2.1 Parte B).
public static class FmPlayerStatMap
{
    // market -> FootyMetrics stat slug (collection).
    public static readonly IReadOnlyDictionary<string, string> SlugByMarket =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["tackles"] = "tackles",
            ["foul_involvements"] = "fouls-involvements",
            ["fouls_drawn"] = "fouls-won",
            ["fouls_committed"] = "fouls-committed",
            ["goalkeeper_saves"] = "saves"
        };

    // market -> stats_json field candidates, in preference order. The extra
    // candidates are the same number under the other FM surface:
    //   teams/table discipline pivot calls foul involvements "fi"
    //   position-stats appearance rows call it "foulInvolvements"
    //   fouls_drawn is "foulsD" on both (slug fouls-drawn is a 400, use fouls-won)
    public static readonly IReadOnlyDictionary<string, string[]> FieldByMarket =
        new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["goals"] = new[] { "goals" },
            ["shots"] = new[] { "sh" },
            ["shots_on_target"] = new[] { "sot" },
            ["score_assist"] = new[] { "assists" },
            ["assists"] = new[] { "assists" },
            ["offsides"] = new[] { "offsides" },
            ["shots_created"] = new[] { "shotsCreated" },
            ["chances_created"] = new[] { "chancesCreated" },
            ["cards"] = new[] { "cards" },
            ["yellow_cards"] = new[] { "yellowCards" },
            ["penalties"] = new[] { "penalties" },
            ["fouls_committed"] = new[] { "foulsC" },
            ["fouls_drawn"] = new[] { "foulsD" },
            ["tackles"] = new[] { "tackles" },
            ["foul_involvements"] = new[] { "foulInvolvements", "fi" },
            ["goalkeeper_saves"] = new[] { "saves" }
        };

    // slug -> teams/table group. Anything not listed here stays on "attack",
    // which is what the endpoint has always used for corners/shots/goals.
    private static readonly IReadOnlyDictionary<string, string> GroupBySlug =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["tackles"] = "defense",
            ["saves"] = "defense",
            ["fouls-involvements"] = "discipline",
            ["fouls-won"] = "discipline",
            ["fouls-committed"] = "discipline"
        };

    public static string NormalizeMarket(string market)
    {
        var m = market.Trim();
        if (m.StartsWith("home_", StringComparison.Ordinal)) return m["home_".Length..];
        if (m.StartsWith("away_", StringComparison.Ordinal)) return m["away_".Length..];
        return m;
    }

    // Collection token -> FM slug. Market names are translated, anything else
    // (already a slug, or a stat we do not map) is passed through untouched.
    public static string ResolveSlug(string token)
    {
        var trimmed = token.Trim();
        if (SlugByMarket.TryGetValue(NormalizeMarket(trimmed), out var slug)) return slug;
        return trimmed;
    }

    public static string GroupForStat(string stat) =>
        GroupBySlug.TryGetValue(stat.Trim(), out var group) ? group : "attack";

    public static string[]? FieldsFor(string market) =>
        FieldByMarket.TryGetValue(NormalizeMarket(market), out var fields) ? fields : null;
}
