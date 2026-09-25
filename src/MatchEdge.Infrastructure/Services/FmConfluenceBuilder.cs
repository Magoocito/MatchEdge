using System.Text.Json;

namespace MatchEdge.Infrastructure.Services;

public sealed record FmConfluencePiece(
    string Kind,
    long SignalId,
    string SubjectType,
    string SubjectName,
    string Market,
    double? Line,
    string? Direction,
    int? SampleSize,
    int? Hits,
    double? ObservedHitRate,
    double? OppHits,
    double? OppSampleSize,
    string? OutcomeStatus,
    double? ActualValue,
    int? Hit,
    bool SourceConflict,
    List<FmWindowStats>? Windows);

public sealed record FmConfluenceOverlap(
    string PieceA,
    string PieceB,
    bool OverlapFlag,
    string Basis,
    int Intersection,
    int MinSize,
    double Ratio);

public sealed record FmConfluenceGroup(
    string Market,
    double? Line,
    List<FmConfluencePiece> Pieces,
    List<FmConfluenceOverlap> Overlaps);

public sealed record FmConfluenceContext(
    string Venue,
    string CompetitionScope,
    bool LeakageFlag,
    int SourceConflicts,
    int Signals,
    int Outcomes);

public sealed record FmConfluenceReport(
    string FixtureId,
    FmConfluenceContext Context,
    List<FmConfluenceGroup> Groups);

public static class FmConfluenceBuilder
{
    public const int OverlapWindowDates = 5;
    public const double OverlapThreshold = 0.5;

    private static readonly Dictionary<string, string[]> RelatedPlayerMarkets =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["fouls_committed"] = new[] { "fouls_committed" },
            ["total_fouls"] = new[] { "fouls_committed" },
            ["offsides"] = new[] { "offsides" },
            ["total_offsides"] = new[] { "offsides" },
            ["goalkeeper_saves"] = new[] { "goalkeeper_saves" },
            ["total_saves"] = new[] { "goalkeeper_saves" },
            ["home_saves"] = new[] { "goalkeeper_saves" },
            ["away_saves"] = new[] { "goalkeeper_saves" },
            ["home_shots"] = new[] { "shots", "shots_created" },
            ["away_shots"] = new[] { "shots", "shots_created" },
            ["total_shots"] = new[] { "shots", "shots_created" },
            ["home_shots_on_target"] = new[] { "shots" },
            ["away_shots_on_target"] = new[] { "shots" },
            ["total_shots_on_target"] = new[] { "shots" },
            ["tackles"] = new[] { "tackles" }
        };

    public static FmConfluenceReport Build(
        string fixtureId,
        IReadOnlyList<FmSignalDetailRow> signals,
        IReadOnlyDictionary<long, FmOutcomeRecord> outcomes,
        bool leakageFlag,
        string venue,
        string competitionScope)
    {
        var teamSignals = signals
            .Where(s => s.SubjectType == "team" && s.Market != null)
            .ToList();
        var playerSignals = signals
            .Where(s => s.SubjectType == "player" && s.Market != null)
            .ToList();

        var groups = new List<FmConfluenceGroup>();
        foreach (var teamGroup in teamSignals
                     .GroupBy(s => (Market: s.Market!, Line: s.Line))
                     .OrderBy(g => g.Key.Item1, StringComparer.Ordinal)
                     .ThenBy(g => g.Key.Item2))
        {
            var pieces = new List<FmConfluencePiece>();
            var overlaps = new List<FmConfluenceOverlap>();

            foreach (var t in teamGroup)
            {
                var windows = FmWindowCalculator.Compute(
                    t.SubjectType, t.RecentValuesJson, t.Line, t.Direction);
                pieces.Add(ToPiece("team_attack", t, outcomes, windows));

                if (t.OppHits is not null)
                {
                    pieces.Add(new FmConfluencePiece(
                        "opponent", t.Id, t.SubjectType,
                        $"Opp. hits of {t.SubjectName}", t.Market, t.Line, t.Direction,
                        t.OppSampleSize is null ? null : (int?)t.OppSampleSize,
                        null, null, t.OppHits, t.OppSampleSize,
                        null, null, null, false, null));
                    overlaps.Add(new FmConfluenceOverlap(
                        Label("team_attack", t), Label("opponent", t),
                        true, "same_source_row",
                        (int)(t.SampleSize ?? 0), (int)(t.SampleSize ?? 0), 1.0));
                }
            }

            var related = RelatedPlayerMarkets.TryGetValue(teamGroup.Key.Market, out var pm)
                ? pm
                : Array.Empty<string>();
            foreach (var p in playerSignals
                         .Where(p => p.Line == teamGroup.Key.Line &&
                                     p.Market != null &&
                                     related.Contains(p.Market))
                         .OrderBy(p => p.SubjectName, StringComparer.Ordinal))
            {
                var windows = FmWindowCalculator.Compute(
                    p.SubjectType, p.RecentValuesJson, p.Line, p.Direction);
                pieces.Add(ToPiece("player", p, outcomes, windows));
            }

            overlaps.AddRange(ComputeDateOverlaps(pieces, signals));

            groups.Add(new FmConfluenceGroup(
                teamGroup.Key.Market, teamGroup.Key.Line, pieces, overlaps));
        }

        var context = new FmConfluenceContext(
            venue,
            competitionScope,
            leakageFlag,
            outcomes.Values.Count(o => o.SourceConflict),
            signals.Count,
            outcomes.Count);

        return new FmConfluenceReport(fixtureId, context, groups);
    }

    private static FmConfluencePiece ToPiece(
        string kind,
        FmSignalDetailRow row,
        IReadOnlyDictionary<long, FmOutcomeRecord> outcomes,
        List<FmWindowStats> windows)
    {
        outcomes.TryGetValue(row.Id, out var outcome);
        return new FmConfluencePiece(
            kind, row.Id, row.SubjectType, row.SubjectName, row.Market!, row.Line, row.Direction,
            row.SampleSize, row.Hits, row.ObservedHitRate, null, null,
            outcome?.Status, outcome?.ActualValue, outcome?.Hit,
            outcome?.SourceConflict ?? false,
            windows);
    }

    private static string Label(string kind, FmSignalDetailRow row) =>
        $"{kind}:{row.SubjectName} {row.Market}";

    private static List<FmConfluenceOverlap> ComputeDateOverlaps(
        List<FmConfluencePiece> pieces,
        IReadOnlyList<FmSignalDetailRow> signals)
    {
        var overlaps = new List<FmConfluenceOverlap>();
        var byId = signals.ToDictionary(s => s.Id);
        var dated = pieces
            .Where(p => p.Kind != "opponent" && p.Windows is not null)
            .Select(p => (Piece: p, Dates: HistoryDates(byId[p.SignalId].RecentValuesJson)))
            .ToList();

        for (var i = 0; i < dated.Count; i++)
        {
            for (var j = i + 1; j < dated.Count; j++)
            {
                var a = dated[i];
                var b = dated[j];
                var intersection = a.Dates.Intersect(b.Dates).Count();
                var minSize = Math.Min(a.Dates.Count, b.Dates.Count);
                var ratio = minSize == 0 ? 0 : (double)intersection / minSize;
                overlaps.Add(new FmConfluenceOverlap(
                    Label(a.Piece.Kind, byId[a.Piece.SignalId]),
                    Label(b.Piece.Kind, byId[b.Piece.SignalId]),
                    minSize > 0 && ratio > OverlapThreshold,
                    "shared_dates",
                    intersection, minSize, Math.Round(ratio, 4)));
            }
        }
        return overlaps;
    }

    private static HashSet<string> HistoryDates(string? historyJson, int take = OverlapWindowDates)
    {
        var dates = new HashSet<string>(StringComparer.Ordinal);
        if (string.IsNullOrWhiteSpace(historyJson)) return dates;
        using var doc = JsonDocument.Parse(historyJson);
        if (doc.RootElement.ValueKind != JsonValueKind.Array) return dates;
        foreach (var el in doc.RootElement.EnumerateArray())
        {
            if (dates.Count >= take) break;
            if (el.ValueKind != JsonValueKind.Object) continue;
            if (!el.TryGetProperty("t", out var t) || t.ValueKind != JsonValueKind.String) continue;
            var raw = t.GetString();
            if (raw is { Length: >= 10 }) dates.Add(raw[..10]);
        }
        return dates;
    }
}
