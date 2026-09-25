using System.Text.Json;
using MatchEdge.Infrastructure.Clients;
using MatchEdge.Infrastructure.Services;
using Xunit;

namespace MatchEdge.InfrastructureTests;

public class FmDeterministicTests
{
    private const string PlayerJson = """
    {
      "data": [
        {
          "shortName": "S. Lukic",
          "market": "fouls_committed",
          "line": "0.5",
          "direction": "over",
          "bestCount": 10,
          "bestTotal": 10,
          "percentage": 100,
          "score": "0.5757",
          "history": [ { "h": true, "t": "2026-03-31T16:00:00.000Z", "v": 1 } ]
        }
      ],
      "pagination": { "count": 1 }
    }
    """;

    private const string TeamJson = """
    {
      "data": [
        {
          "name": "Serbia",
          "market": "total_corners",
          "line": "5.5",
          "direction": "over",
          "bestCount": 10,
          "bestTotal": 10,
          "oppCount": 8,
          "oppTotal": 10,
          "oppHitRate": "80.0",
          "history": [ { "h": true, "t": "2026-01-01T00:00:00.000Z", "va": 9.3 } ]
        }
      ],
      "pagination": { "count": 1 }
    }
    """;

    [Fact]
    public void Parser_Player_ExtractsHitsSampleAndDerivedRate()
    {
        var signals = FmTrendsJsonParser.Parse(PlayerJson, "player");

        var s = Assert.Single(signals);
        Assert.Equal("player", s.SubjectType);
        Assert.Equal("S. Lukic", s.SubjectName);
        Assert.Equal("fouls_committed", s.Market);
        Assert.Equal(10, s.Hits);
        Assert.Equal(10, s.SampleSize);
        Assert.Equal(1.0, s.ObservedHitRate);
        Assert.Equal(0.5757, s.ConfidenceScore);
        Assert.NotNull(s.RecentValuesJson);
        Assert.Null(s.OppHits);
    }

    [Fact]
    public void Parser_Team_ExtractsOpponentCounts()
    {
        var signals = FmTrendsJsonParser.Parse(TeamJson, "team");

        var s = Assert.Single(signals);
        Assert.Equal("team", s.SubjectType);
        Assert.Equal("Serbia", s.SubjectName);
        Assert.Equal(8, s.OppHits);
        Assert.Equal(10, s.OppSampleSize);
        Assert.Equal(1.0, s.ObservedHitRate);
    }

    [Fact]
    public void Parser_EmptyData_ReturnsNoSignals()
    {
        var signals = FmTrendsJsonParser.Parse("""{"data":[],"pagination":{"count":0}}""", "team");
        Assert.Empty(signals);
    }

    [Fact]
    public void Parser_MissingSample_DoesNotInventRate()
    {
        var signals = FmTrendsJsonParser.Parse(
            """{"data":[{"shortName":"X","market":"m","bestCount":3,"bestTotal":0}]}""", "player");
        var s = Assert.Single(signals);
        Assert.Equal(3, s.Hits);
        Assert.Null(s.ObservedHitRate);
    }

    [Fact]
    public void Canary_DeviationAbove30Percent_IsSuspect()
    {
        var previous = new Dictionary<string, int> { ["corners"] = 10 };
        var current = new Dictionary<string, int> { ["corners"] = 6 };
        Assert.True(FmCanary.ExceedsDeviation(current, previous));
    }

    [Fact]
    public void Canary_DeviationWithin30Percent_IsOk()
    {
        var previous = new Dictionary<string, int> { ["corners"] = 10 };
        var current = new Dictionary<string, int> { ["corners"] = 8 };
        Assert.False(FmCanary.ExceedsDeviation(current, previous));
    }

    [Fact]
    public void Canary_MarketDisappeared_IsSuspect()
    {
        var previous = new Dictionary<string, int> { ["corners"] = 10 };
        var current = new Dictionary<string, int>();
        Assert.True(FmCanary.ExceedsDeviation(current, previous));
    }

    [Fact]
    public void SessionDetection_LoginUrl_IsDetected()
    {
        Assert.True(FmNavigator.LooksLikeLoginUrl("https://www.footymetrics.com/login"));
        Assert.False(FmNavigator.LooksLikeLoginUrl("https://www.footymetrics.com/fixtures/1-x"));
    }

    [Fact]
    public void SessionDetection_SignInMarkers_IsDetected()
    {
        Assert.True(FmNavigator.LooksLikeSignedOutText("Welcome back. Sign in to continue."));
        Assert.False(FmNavigator.LooksLikeSignedOutText("Serbia vs Greece lineups"));
    }

    [Fact]
    public void NavigationBudget_IsFortyPerRun()
    {
        Assert.Equal(40, FmNavigator.MaxNavigationsPerRun);
        Assert.True(FmNavigator.MaxNavigationsPerRun / 3 >= 13,
            "run topN cap must allow at least the 4 known fixtures (3 tabs each)");
    }

    [Fact]
    public void ScopesFromUrl_DerivesVenueAndCompetitionScope()
    {
        var (venue, comp) = FmFixtureSnapshotService.ScopesFromUrl(
            "https://www.footymetrics.com/api/front/trends/fixtures/1/teams?x=1&location=match&league_only=true");
        Assert.Equal("match", venue);
        Assert.Equal("same_league", comp);
    }

    [Fact]
    public void ScopesFromUrl_DefaultsToAll()
    {
        var (venue, comp) = FmFixtureSnapshotService.ScopesFromUrl(null);
        Assert.Equal("all", venue);
        Assert.Equal("all", comp);
    }

    [Fact]
    public void ParamsJsonFromUrl_DerivesLocationAndLeagueOnly()
    {
        var json = FmFixtureSnapshotService.ParamsJsonFromUrl(
            "https://www.footymetrics.com/api/front/trends/fixtures/1/teams?location=match&league_only=true");
        Assert.Contains("\"location\":\"match\"", json);
        Assert.Contains("\"league_only\":true", json);
        Assert.Contains("\"league_only\":false",
            FmFixtureSnapshotService.ParamsJsonFromUrl(null));
    }

    [Fact]
    public void BestWindow_MatchesVerifiedV3Rule()
    {
        double[] awayShots = { 13, 13, 10, 15, 13, 18, 15, 15, 4, 30 };
        var (hits, window) = FmSignalValidator.BestWindow(awayShots, 12.5, "over", 4);
        Assert.Equal(7, hits);
        Assert.Equal(8, window);

        double[] allHit = { 7, 8, 11, 6, 16, 11, 10, 6, 8, 10 };
        Assert.Equal((10, 10), FmSignalValidator.BestWindow(allHit, 5.5, "over", 4));
    }

    [Fact]
    public void Validator_SyntheticNonNumericValue_IsSuspect()
    {
        var draft = new FmSignalDraft(
            "team", "X", "total_goals", 2.5, "over", 3, 4, 0.75, null, null, null,
            """[{"vt":"abc","t":"2026-01-01T00:00:00.000Z"},{"vt":3},{"vt":3},{"vt":3}]""");
        var (status, motivo) = FmSignalValidator.Validate(draft);
        Assert.Equal("SUSPECT", status);
        Assert.Contains("non-numeric", motivo);
    }

    [Fact]
    public void Validator_SyntheticSampleLongerThanHistory_IsSuspect()
    {
        var draft = new FmSignalDraft(
            "team", "X", "total_goals", 2.5, "over", 3, 6, 0.5, null, null, null,
            """[{"vt":3},{"vt":3},{"vt":3},{"vt":3}]""");
        var (status, motivo) = FmSignalValidator.Validate(draft);
        Assert.Equal("SUSPECT", status);
        Assert.Contains("sample_size 6 > history len 4", motivo);
    }

    [Fact]
    public void Validator_SyntheticRecomputedMismatch_IsSuspect()
    {
        var draft2 = new FmSignalDraft(
            "team", "X", "total_goals", 2.5, "over", 3, 10, 0.3, null, null, null,
            "[" + string.Join(",", Enumerable.Repeat("""{"vt":3}""", 10)) + "]");
        var (status, motivo) = FmSignalValidator.Validate(draft2);
        Assert.Equal("SUSPECT", status);
        Assert.Contains("recomputed 10/10 != bestCount 3/10", motivo);
    }

    [Fact]
    public void Validator_SyntheticValidRow_IsOk()
    {
        var draft = new FmSignalDraft(
            "team", "X", "total_goals", 2.5, "over", 10, 10, 1.0, null, null, null,
            "[" + string.Join(",", Enumerable.Repeat("""{"vt":3}""", 10)) + "]");
        var (status, motivo) = FmSignalValidator.Validate(draft);
        Assert.Equal("OK", status);
        Assert.Null(motivo);
    }

    [Fact]
    public async Task Store_Migration_IsIdempotent_AndAddsP2Columns()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            await store.EnsureSchemaAsync();
            await store.EnsureSchemaAsync();

            var columns = new List<string>();
            await using (var conn = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={dbPath}"))
            {
                await conn.OpenAsync();
                foreach (var table in new[] { "fm_snapshot", "fm_signal" })
                {
                    await using var cmd = conn.CreateCommand();
                    cmd.CommandText = $"PRAGMA table_info({table});";
                    await using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                        columns.Add($"{table}.{reader.GetString(1)}");
                }
            }

            Assert.Contains("fm_snapshot.leakage_flag", columns);
            Assert.Contains("fm_signal.params_json", columns);
            Assert.Contains("fm_signal.status", columns);
            Assert.Contains("fm_signal.motivo", columns);

            var id = await store.InsertSnapshotAsync(
                "33441813", "team-trends", "https://x", DateTime.UtcNow,
                "p", "sha", "fm-json-v1", "OK", leakageFlag: true);
            Assert.True(id > 0);
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    [Fact]
    public async Task Store_InsertSignals_PersistsParamsStatusAndSuspect()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            var okDraft = new FmSignalDraft(
                "team", "X", "total_goals", 2.5, "over", 10, 10, 1.0, null, null, null,
                "[" + string.Join(",", Enumerable.Repeat("""{"vt":3}""", 10)) + "]");
            var badDraft = new FmSignalDraft(
                "team", "Y", "total_goals", 2.5, "over", 3, 10, 0.3, null, null, null,
                "[" + string.Join(",", Enumerable.Repeat("""{"vt":3}""", 10)) + "]");

            var snapshotId = await store.InsertSnapshotAsync(
                "33441813-x", "team-trends", "https://x", DateTime.UtcNow,
                "p", "sha", "fm-json-v1", "OK");
            var result = await store.InsertSignalsAsync(
                snapshotId, "33441813-x", new[] { okDraft, badDraft },
                "all", "all", DateTime.UtcNow, """{"location":"all","league_only":false}""");

            Assert.Equal(2, result.Inserted);
            Assert.Equal(1, result.Suspect);
            Assert.Contains("recomputed", result.FirstMotivo);

            await using var conn = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={dbPath}");
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT subject_name, status, motivo, params_json FROM fm_signal ORDER BY id";
            await using var reader = await cmd.ExecuteReaderAsync();
            Assert.True(await reader.ReadAsync());
            Assert.Equal("X", reader.GetString(0));
            Assert.Equal("OK", reader.GetString(1));
            Assert.True(reader.IsDBNull(2));
            Assert.Contains("league_only", reader.GetString(3));
            Assert.True(await reader.ReadAsync());
            Assert.Equal("Y", reader.GetString(0));
            Assert.Equal("SUSPECT", reader.GetString(1));
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    [Fact]
    public void OutcomeParser_StatsRows_OrientHomeAndAway()
    {
        var html =
            ">39<!-- -->%</span><span class=\"text-xs font-medium text-text-secondary\">Ball possession</span><span class=\"text-sm font-semibold tabular-nums text-text-primary\">61<!-- -->%</span>" +
            ">9</span><span class=\"text-xs font-medium text-text-secondary\">Total shots</span><span class=\"text-sm font-semibold tabular-nums text-text-primary\">26</span>" +
            ">2</span><span class=\"text-xs font-medium text-text-secondary\">Corners</span><span class=\"text-sm font-semibold tabular-nums text-text-primary\">6</span>";

        var stats = FmOutcomeResolver.ParseStats(html);
        Assert.Equal((9, 26), stats["Total shots"]);
        Assert.Equal((2, 6), stats["Corners"]);
        Assert.Equal((39, 61), stats["Ball possession"]);
    }

    [Fact]
    public void OutcomeParser_DuplicatePanels_IdenticalKept_DivergentDropped()
    {
        var row = ">2</span><span class=\"text-xs font-medium text-text-secondary\">Corners</span>" +
                  "<span class=\"text-sm font-semibold tabular-nums text-text-primary\">6</span>";
        var divergent = ">2</span><span class=\"text-xs font-medium text-text-secondary\">Corners</span>" +
                        "<span class=\"text-sm font-semibold tabular-nums text-text-primary\">9</span>";

        var same = FmOutcomeResolver.ParseStats(row + row);
        Assert.Equal((2, 6), same["Corners"]);

        var conflict = FmOutcomeResolver.ParseStats(row + divergent);
        Assert.False(conflict.ContainsKey("Corners"), "divergent duplicate must be dropped as ambiguous");
    }

    [Fact]
    public async Task OutcomeStore_Insert_IsIdempotentPerSignal()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmOutcomeStore($"Data Source={dbPath}");
            var draft = new FmOutcomeDraft(42, "33441813-x", 8, 1, "RESOLVED", "stats-panel", null);
            Assert.Equal(1, await store.WriteAsync(new[] { draft }, DateTime.UtcNow));
            Assert.Equal(0, await store.WriteAsync(new[] { draft }, DateTime.UtcNow));

            var byFixture = await store.GetByFixtureAsync("33441813-x");
            Assert.Single(byFixture);
            Assert.Equal(8, byFixture[42].ActualValue);
            Assert.Equal(1, byFixture[42].Hit);
            Assert.Equal("RESOLVED", byFixture[42].Status);
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    [Fact]
    public async Task OutcomeStore_UnavailableIsRetryable_FinalStatusIsNot()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmOutcomeStore($"Data Source={dbPath}");
            var unavailable = new FmOutcomeDraft(
                7, "fx-a", null, null, "UNAVAILABLE", "none", "lag", "NO_DATE_MATCH");
            Assert.Equal(1, await store.WriteAsync(new[] { unavailable }, DateTime.UtcNow));

            var resolved = new FmOutcomeDraft(7, "fx-a", 3, 1, "RESOLVED", "history", null);
            Assert.Equal(1, await store.WriteAsync(new[] { resolved }, DateTime.UtcNow));
            var after = await store.GetByFixtureAsync("fx-a");
            Assert.Equal("RESOLVED", after[7].Status);
            Assert.Equal(3, after[7].ActualValue);
            Assert.Null(after[7].UnavailableReason);

            var retried = new FmOutcomeDraft(
                7, "fx-a", null, null, "UNAVAILABLE", "none", "x", "OTHER");
            Assert.Equal(0, await store.WriteAsync(new[] { retried }, DateTime.UtcNow));
            after = await store.GetByFixtureAsync("fx-a");
            Assert.Equal("RESOLVED", after[7].Status);
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    [Fact]
    public void SourceConflict_SavesGoalsSotContradiction_IsDetected()
    {
        var signals = new List<FmSignalRow>
        {
            new(1, "team", "Portugal", "home_saves", 1.5, "over"),
            new(2, "team", "Wales", "away_goals", 0.5, "over"),
            new(3, "team", "Wales", "away_shots_on_target", 1.5, "over"),
            new(4, "team", "Wales", "total_shots", 8.5, "over"),
        };
        var drafts = new List<FmOutcomeDraft>
        {
            new(1, "fx-p", 0, 0, "RESOLVED", "stats-panel", null),
            new(2, "fx-p", 0, 0, "RESOLVED", "fixtures-api", null),
            new(3, "fx-p", 1, 0, "RESOLVED", "stats-panel", null),
        };

        var conflicts = FmOutcomeResolver.DetectSourceConflicts(
            signals, drafts, new Dictionary<long, FmOutcomeRecord>());

        Assert.Contains("home_saves", conflicts);
        Assert.Contains("away_goals", conflicts);
        Assert.Contains("away_shots_on_target", conflicts);
        Assert.DoesNotContain("total_shots", conflicts);
    }

    [Fact]
    public void SourceConflict_Consistent_NoConflict_SotAboveShots_Detected()
    {
        var consistent = new List<FmSignalRow>
        {
            new(1, "team", "Norway", "home_saves", 1.5, "over"),
            new(2, "team", "Denmark", "away_goals", 1.5, "over"),
            new(3, "team", "Denmark", "away_shots_on_target", 6.5, "over"),
        };
        var drafts = new List<FmOutcomeDraft>
        {
            new(1, "fx-n", 5, 1, "RESOLVED", "stats-panel", null),
            new(2, "fx-n", 2, 1, "RESOLVED", "fixtures-api", null),
            new(3, "fx-n", 7, 1, "RESOLVED", "stats-panel", null),
        };
        var conflicts = FmOutcomeResolver.DetectSourceConflicts(
            consistent, drafts, new Dictionary<long, FmOutcomeRecord>());
        Assert.Empty(conflicts);

        var sotAboveShots = new List<FmSignalRow>
        {
            new(10, "team", "X", "total_shots_on_target", 6.5, "over"),
            new(11, "team", "X", "total_shots", 22.5, "over"),
        };
        var bad = new List<FmOutcomeDraft>
        {
            new(10, "fx-x", 14, 1, "RESOLVED", "stats-panel", null),
            new(11, "fx-x", 10, 0, "RESOLVED", "stats-panel", null),
        };
        var conflicts2 = FmOutcomeResolver.DetectSourceConflicts(
            sotAboveShots, bad, new Dictionary<long, FmOutcomeRecord>());
        Assert.Contains("total_shots_on_target", conflicts2);
        Assert.Contains("total_shots", conflicts2);
    }

    private static string Hist(params (string Date, double V, bool Met)[] items)
    {
        var els = string.Join(",", items.Select(i =>
            $"{{\"t\":\"{i.Date}T19:00:00.000Z\"," +
            $"\"v\":{i.V.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
            $"\"met\":{(i.Met ? "true" : "false")}}}"));
        return "[" + els + "]";
    }

    [Fact]
    public void WindowCalculator_ComputesFixedWindows_FromRawHistoryIgnoringBestCount()
    {
        var history = Hist(
            ("2026-09-20", 10, true), ("2026-09-13", 9, true), ("2026-09-06", 8, true),
            ("2026-08-30", 7, true), ("2026-08-23", 6, true), ("2026-08-16", 5, false),
            ("2026-08-09", 4, false), ("2026-08-02", 3, false), ("2026-07-26", 2, false),
            ("2026-07-19", 1, false));

        var windows = FmWindowCalculator.Compute("team", history, 5.5, "over");
        Assert.Equal(3, windows.Count);

        var w5 = windows.Single(w => w.Window == "5");
        Assert.Equal(FmWindowCalculator.StatusOk, w5.Status);
        Assert.Equal(5, w5.N);
        Assert.Equal(5, w5.Hits);
        Assert.Equal(1.0, w5.ObservedRate);
        Assert.Equal(8.0, w5.Mean);
        Assert.Equal(8.0, w5.Median);
        Assert.Equal(6.0, w5.Min);
        Assert.Equal(10.0, w5.Max);

        var w10 = windows.Single(w => w.Window == "10");
        Assert.Equal(10, w10.N);
        Assert.Equal(5, w10.Hits);
        Assert.Equal(0.5, w10.ObservedRate);

        var wall = windows.Single(w => w.Window == "all");
        Assert.Equal(10, wall.N);
        Assert.Equal(5, wall.Hits);
    }

    [Fact]
    public void WindowCalculator_InsufficientSample_TeamVsPlayerThresholds()
    {
        var history = Hist(
            ("2026-09-20", 2, true), ("2026-09-13", 1, true), ("2026-09-06", 0, false));

        var team = FmWindowCalculator.Compute("team", history, 1.5, "over");
        Assert.All(team, w =>
            Assert.Equal(FmWindowCalculator.StatusInsufficient, w.Status));
        Assert.All(team, w => Assert.Null(w.Hits));

        var player = FmWindowCalculator.Compute("player", history, 1.5, "over");
        Assert.All(player, w =>
            Assert.Equal(FmWindowCalculator.StatusOk, w.Status));
        Assert.Equal(2, player.Single(w => w.Window == "5").Hits);
    }

    [Fact]
    public void Confluence_PlayerTeamSharedDates_OverlapFlagged_OpposingTeamNot()
    {
        var denmarkDates = Hist(
            ("2026-09-24", 4, true), ("2026-09-20", 6, true), ("2026-09-13", 5, true),
            ("2026-09-06", 7, true), ("2026-08-30", 3, false), ("2026-08-23", 8, true),
            ("2026-08-16", 5, false), ("2026-08-09", 9, true), ("2026-08-02", 6, true),
            ("2026-07-26", 4, false));
        var norwayDates = Hist(
            ("2026-02-10", 5, false), ("2026-02-05", 7, true), ("2026-01-30", 2, false),
            ("2026-01-24", 6, true), ("2026-01-18", 4, false), ("2026-01-12", 8, true),
            ("2026-01-06", 3, false), ("2025-12-30", 9, true), ("2025-12-24", 5, false),
            ("2025-12-18", 6, true));

        var signals = new List<FmSignalDetailRow>
        {
            new(1, "team", "Denmark", "away_shots", 8.5, "over",
                7, 10, 0.7, 4.0, 10.0, denmarkDates, null, "all", "all"),
            new(2, "team", "Norway", "away_shots", 8.5, "over",
                6, 10, 0.6, 5.0, 10.0, norwayDates, null, "all", "all"),
            new(3, "player", "E. Haaland", "shots", 8.5, "over",
                8, 10, 0.8, null, null, denmarkDates, null, "all", "all")
        };

        var report = FmConfluenceBuilder.Build(
            "fx-1", signals, new Dictionary<long, FmOutcomeRecord>(),
            leakageFlag: false, venue: "all", competitionScope: "all");

        var group = report.Groups.Single(g => g.Market == "away_shots");
        Assert.Equal(5, group.Pieces.Count); // 2 team + 2 opponent + 1 player
        Assert.Contains(group.Pieces, p => p.Kind == "team_attack" && p.Windows != null);
        Assert.Contains(group.Pieces, p => p.Kind == "opponent" && p.Windows == null);

        var playerOverlap = group.Overlaps.Single(o =>
            (o.PieceA.Contains("Denmark") && o.PieceB.Contains("E. Haaland")) ||
            (o.PieceA.Contains("E. Haaland") && o.PieceB.Contains("Denmark")));
        var teamPair = group.Overlaps.Single(o =>
            o.PieceA.Contains("Denmark") && o.PieceB.Contains("Norway"));
        var oppPairs = group.Overlaps.Where(o => o.Basis == "same_source_row").ToList();

        Assert.True(playerOverlap.OverlapFlag, "player window shares its team's match dates");
        Assert.Equal(1.0, playerOverlap.Ratio);
        Assert.False(teamPair.OverlapFlag, "opposing teams' match dates are disjoint");
        Assert.Equal(2, oppPairs.Count);
        Assert.All(oppPairs, o => Assert.True(o.OverlapFlag));
    }

    private static List<JsonElement> Elements(string jsonArray)
    {
        using var doc = JsonDocument.Parse(jsonArray);
        return doc.RootElement.EnumerateArray().Select(e => e.Clone()).ToList();
    }

    private static readonly FmSignalRow RonaldoSignal =
        new(53, "player", "C. Ronaldo", "shots", 1.5, "over");

    private static readonly DateOnly Kickoff = new(2026, 9, 24);

    [Fact]
    public void MatchHistory_NoFixtureElement_NoHistoryElement_WithNearestDelta()
    {
        var elements = Elements("""
        [
          { "t": "2026-07-06T16:00:00.000Z", "v": 3, "m": 90, "opp": { "name": "Croatia" } },
          { "t": "2025-10-11T16:00:00.000Z", "v": 1, "m": 90, "opp": { "name": "Scotland" } }
        ]
        """);

        var (draft, reason) = FmOutcomeResolver.MatchHistory(
            "33441811", "Portugal", "Wales", RonaldoSignal, Kickoff, elements);

        Assert.Null(draft);
        Assert.Equal(FmOutcomeResolver.ReasonNoHistoryElement, reason!.Value.Code);
        Assert.Contains("0/2 elements", reason.Value.Detail);
        Assert.Contains("Δ=80d", reason.Value.Detail);
        Assert.Contains("Wales", reason.Value.Detail);
    }

    [Fact]
    public void MatchHistory_SameOpponentFarDate_NoDateMatch()
    {
        var elements = Elements("""
        [
          { "t": "2026-06-06T16:00:00.000Z", "v": 2, "m": 90, "opp": { "name": "Wales" } },
          { "t": "2026-03-26T16:00:00.000Z", "v": 4, "m": 90, "opp": { "name": "Denmark" } }
        ]
        """);

        var (draft, reason) = FmOutcomeResolver.MatchHistory(
            "33441811", "Portugal", "Wales", RonaldoSignal, Kickoff, elements);

        Assert.Null(draft);
        Assert.Equal(FmOutcomeResolver.ReasonNoDateMatch, reason!.Value.Code);
        Assert.Contains("nearest same-opponent t=2026-06-06", reason.Value.Detail);
        Assert.Contains("Δ=110d", reason.Value.Detail);
    }

    [Fact]
    public void MatchHistory_InWindowOpponent_ResolvesHit()
    {
        var elements = Elements("""
        [
          { "t": "2026-09-24T18:45:00.000Z", "v": 3, "m": 90, "opp": { "name": "Wales" } }
        ]
        """);

        var (draft, reason) = FmOutcomeResolver.MatchHistory(
            "33441811", "Portugal", "Wales", RonaldoSignal, Kickoff, elements);

        Assert.Null(reason);
        Assert.NotNull(draft);
        Assert.Equal(FmOutcomeResolver.StatusResolved, draft!.Status);
        Assert.Equal(3d, draft.ActualValue);
        Assert.Equal(1, draft.Hit);
        Assert.Equal("history", draft.Source);
    }

    [Fact]
    public void MatchHistory_ZeroMinutes_NotPlayed()
    {
        var elements = Elements("""
        [
          { "t": "2026-09-24T18:45:00.000Z", "v": 0, "m": 0, "opp": { "name": "Wales" } }
        ]
        """);

        var (draft, reason) = FmOutcomeResolver.MatchHistory(
            "33441811", "Portugal", "Wales", RonaldoSignal, Kickoff, elements);

        Assert.Null(reason);
        Assert.Equal(FmOutcomeResolver.StatusNotPlayed, draft!.Status);
    }

    [Fact]
    public void MatchHistory_EmptyElements_NoHistoryElement()
    {
        var (draft, reason) = FmOutcomeResolver.MatchHistory(
            "33441811", "Portugal", "Wales", RonaldoSignal, Kickoff,
            new List<JsonElement>());

        Assert.Null(draft);
        Assert.Equal(FmOutcomeResolver.ReasonNoHistoryElement, reason!.Value.Code);
        Assert.Contains("history[] empty", reason.Value.Detail);
    }

    [Fact]
    public void ParseOdds_ExtractsSidesBookmakerAndLine()
    {
        const string json = """
        {
          "data": [
            { "shortName": "C. Ronaldo", "market": "shots", "line": "1.5",
              "odds": [ { "bk": 3, "over": 1.02, "under": null } ] },
            { "shortName": "B. Fernandes", "market": "fouls_drawn", "line": "0.5",
              "odds": [
                { "bk": 2, "over": 1.25, "under": 1.6 },
                { "bk": 4, "over": 1.3, "under": 1.55 }
              ] },
            { "shortName": "X", "market": "tackles", "line": "1.5", "odds": [] },
            { "shortName": "Y", "market": "cards", "line": "2.5",
              "odds": { "bk": 5, "over": 2.0, "under": null } }
          ],
          "pagination": { "count": 4 }
        }
        """;

        var odds = FmTrendsJsonParser.ParseOdds(json, "player");

        Assert.Equal(6, odds.Count);
        Assert.All(odds, o => Assert.Equal("player", o.SubjectType));

        var ronaldoOver = odds.Single(o => o.SubjectName == "C. Ronaldo");
        Assert.Equal("shots", ronaldoOver.Market);
        Assert.Equal(1.5, ronaldoOver.Line);
        Assert.Equal("3", ronaldoOver.Bookmaker);
        Assert.Equal(1.02, ronaldoOver.OddsValue);
        Assert.Equal("over", ronaldoOver.Side);

        var fernandes = odds.Where(o => o.SubjectName == "B. Fernandes").ToList();
        Assert.Equal(4, fernandes.Count);
        Assert.Contains(fernandes, o => o.Bookmaker == "2" && o.Side == "under" && o.OddsValue == 1.6);
        Assert.Contains(fernandes, o => o.Bookmaker == "4" && o.Side == "over" && o.OddsValue == 1.3);

        Assert.DoesNotContain(odds, o => o.SubjectName == "X");
        Assert.Single(odds, o => o.SubjectName == "Y" && o.Bookmaker == "5");
    }

    [Fact]
    public void ParseOdds_TeamRows_UsesTeamName()
    {
        const string json = """
        {
          "data": [
            { "name": "Portugal", "market": "total_corners", "line": "9.5",
              "odds": [ { "bk": "1", "over": "2.1", "under": "1.72" } ] }
          ]
        }
        """;

        var odds = FmTrendsJsonParser.ParseOdds(json, "team");

        Assert.Equal(2, odds.Count);
        Assert.All(odds, o =>
        {
            Assert.Equal("Portugal", o.SubjectName);
            Assert.Equal("1", o.Bookmaker);
        });
        Assert.Equal(2.1, odds.Single(o => o.Side == "over").OddsValue);
        Assert.Equal(1.72, odds.Single(o => o.Side == "under").OddsValue);
    }

    [Fact]
    public async Task OddsStore_CaptureDedupes_PerSnapshot_ManualInsertPersists()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            var snap1 = await store.InsertSnapshotAsync(
                "33441811", "player-trends", "http://x", DateTime.UtcNow, "p", "s", "v", "OK");
            var snap2 = await store.InsertSnapshotAsync(
                "33441811", "player-trends", "http://x", DateTime.UtcNow, "p", "s", "v", "OK");
            var odds = new List<FmOddsDraft>
            {
                new("player", "C. Ronaldo", "shots", 1.5, "3", 1.02, "over"),
                new("player", "C. Ronaldo", "shots", 1.5, "3", 0.80, "under")
            };

            Assert.Equal(2, await store.InsertOddsAsync("33441811", odds, snap1, DateTime.UtcNow));
            // same snapshot + same keys → unique index ignores the replay
            Assert.Equal(0, await store.InsertOddsAsync("33441811", odds, snap1, DateTime.UtcNow));
            // different snapshot → new capture series rows
            Assert.Equal(2, await store.InsertOddsAsync("33441811", odds, snap2, DateTime.UtcNow));

            var manualId = await store.InsertManualOddsAsync(
                "33441811", "Betano", "total_goals", 2.5, 1.90, DateTime.UtcNow);
            Assert.True(manualId > 0);

            var rows = await store.GetOddsAsync("33441811");
            Assert.Equal(5, rows.Count);
            var manual = Assert.Single(rows, r => r.Source == "manual");
            Assert.Equal("Betano", manual.Bookmaker);
            Assert.Equal(1.90, manual.OddsValue);
            Assert.Equal("manual", manual.Side);
            Assert.All(rows.Where(r => r.Source == "fm"),
                r => Assert.Equal("fm", r.Source));
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    [Fact]
    public void Parse_VenueRole_DerivedFromRowTeamAndFixture()
    {
        const string json = """
        {
          "data": [
            { "shortName": "C. Ronaldo", "market": "shots", "line": "1.5",
              "Team": { "name": "Portugal" },
              "Fixture": { "Home": { "name": "Portugal" }, "Away": { "name": "Wales" } } },
            { "shortName": "G. Bale", "market": "shots", "line": "1.5",
              "Team": { "name": "Wales" },
              "Fixture": { "Home": { "name": "Portugal" }, "Away": { "name": "Wales" } } },
            { "shortName": "No Team", "market": "shots", "line": "1.5" }
          ]
        }
        """;

        var rows = FmTrendsJsonParser.Parse(json, "player");

        Assert.Equal("home", rows.Single(r => r.SubjectName == "C. Ronaldo").VenueRole);
        Assert.Equal("away", rows.Single(r => r.SubjectName == "G. Bale").VenueRole);
        Assert.Equal("unknown", rows.Single(r => r.SubjectName == "No Team").VenueRole);
        Assert.Null(rows.Single(r => r.SubjectName == "No Team").FixtureHome);
        Assert.Equal("Portugal", rows.Single(r => r.SubjectName == "C. Ronaldo").FixtureHome);
        Assert.Equal("Wales", rows.Single(r => r.SubjectName == "C. Ronaldo").FixtureAway);
        Assert.Equal("Portugal", rows.Single(r => r.SubjectName == "C. Ronaldo").TeamName);
    }

    [Fact]
    public void Parse_VenueRole_TeamRow_AwaySide()
    {
        const string json = """
        {
          "data": [
            { "name": "Wales", "market": "total_corners", "line": "9.5",
              "Team": { "name": "Wales" },
              "Fixture": { "Home": { "name": "Portugal" }, "Away": { "name": "Wales" } } }
          ]
        }
        """;

        var row = Assert.Single(FmTrendsJsonParser.Parse(json, "team"));
        Assert.Equal("away", row.VenueRole);
    }

    [Fact]
    public async Task Store_BookmakersSeed_OddsNameAutoFill_AndBackfill()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            await store.EnsureSchemaAsync();

            using (var conn = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={dbPath}"))
            {
                await conn.OpenAsync();
                await using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM fm_bookmakers";
                    Assert.Equal(5L, Convert.ToInt64(await cmd.ExecuteScalarAsync()));
                }
                // legacy row (captured before the mapping existed)
                await using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
INSERT INTO fm_market_odds (fixture_id, bookmaker, bookmaker_name, market, odds_value, side, source, source_timestamp_utc, snapshot_id)
VALUES ('33441811', '4', NULL, 'total_corners', 1.9, 'over', 'fm', '2026-09-25 00:00:00', NULL);";
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            // re-running schema backfills the legacy NULL name
            await store.EnsureSchemaAsync();

            var snap = await store.InsertSnapshotAsync(
                "33441811", "team-trends", "http://x", DateTime.UtcNow, "p", "s", "v", "OK");
            await store.InsertOddsAsync("33441811", new List<FmOddsDraft>
            {
                new("team", "Wales", "total_corners", 9.5, "3", 1.85, "over")
            }, snap, DateTime.UtcNow);

            var rows = await store.GetOddsAsync("33441811");
            Assert.Equal(2, rows.Count);
            Assert.Equal("Ladbrokes", rows.Single(r => r.Id > 0 && r.Bookmaker == "4").BookmakerName);
            Assert.Equal("Paddy Power", rows.Single(r => r.Bookmaker == "3").BookmakerName);
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    [Fact]
    public async Task GetLastSnapshotUrl_SkipsLocMatchApiPath()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            await store.InsertSnapshotAsync(
                "33441814", "team-trends",
                "https://www.footymetrics.com/fixtures/33441814-uefa-nations-league-netherlands-germany?tab=team-trends",
                DateTime.UtcNow, "p", "s", "v", "OK");
            await store.InsertSnapshotAsync(
                "33441814", "team-trends+loc=match",
                "/api/front/trends/fixtures/19676695/teams?league_only=false&location=match",
                DateTime.UtcNow, "p", "s", "v", "EMPTY");

            var url = await store.GetLastSnapshotUrlAsync("33441814");

            Assert.StartsWith("https://www.footymetrics.com/fixtures/33441814", url);
            Assert.DoesNotContain("/api/front/", url);
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    private static string FixturePath(string name) =>
        Path.Combine(AppContext.BaseDirectory, "Fixtures", name);

    [Fact]
    public void TeamTableParser_HomePayload_DerivesLocationAndJoinsStats()
    {
        var json = File.ReadAllText(FixturePath("table_home.json"));

        var parsed = FmTeamTableParser.Parse(json, 18701, "home", 15, "corners");

        Assert.Equal(4, parsed.TeamMatches.Count);
        Assert.All(parsed.TeamMatches, m =>
        {
            Assert.Equal("home", m.Location);
            Assert.Equal(18701, m.TeamApid);
            Assert.NotEqual(18701, m.OpponentApid);
            Assert.False(string.IsNullOrEmpty(m.TeamStatsJson));
            Assert.False(string.IsNullOrEmpty(m.OpponentStrengthJson));
            Assert.False(string.IsNullOrEmpty(m.League));
            Assert.Equal(15, m.Period);
            Assert.Equal("corners", m.Stat);
        });

        var first = parsed.TeamMatches.Single(m => m.FixtureId == "33441811");
        Assert.Equal(19676692, first.FixtureApid);
        Assert.Equal("Wales", first.Opponent);
        Assert.Equal(18721, first.OpponentApid);
        Assert.Equal(1, first.HGoals);
        Assert.Equal(0, first.AGoals);
        JsonDocument.Parse(first.OpponentStrengthJson!);
        using (var stats = JsonDocument.Parse(first.TeamStatsJson!))
        {
            // team_stats_json must hold the SUBJECT team's own numbers
            // (Portugal 1-0 Wales, 10 corners), not the opponent's.
            Assert.Equal(1, stats.RootElement.GetProperty("goals").GetInt32());
            Assert.Equal(10, stats.RootElement.GetProperty("corners").GetInt32());
        }
        Assert.Contains("goals",
            parsed.TeamMatches.First(m => !string.IsNullOrEmpty(m.OpponentStrengthJson) &&
                                          m.OpponentStrengthJson != "{}").OpponentStrengthJson!);
        Assert.Equal("UEFA Nations League", first.League);
        Assert.True(first.TsUtc == new DateTime(2026, 9, 24, 18, 45, 0, DateTimeKind.Utc));

        Assert.All(parsed.Warnings, w => Assert.DoesNotContain("differs from requested", w));
        Assert.True(parsed.PlayerMatches.Count > 0);
        var ronaldo = parsed.PlayerMatches.Single(p =>
            p.PlayerApid == 580 && p.FixtureId == "33441811");
        Assert.Equal("C. Ronaldo", ronaldo.PlayerName);
        Assert.Equal("home", ronaldo.Location);
        Assert.Equal("team", ronaldo.Perspective);
        Assert.Contains("\"goals\"", ronaldo.StatsJson!);

        var fixtureIds = parsed.TeamMatches.Select(m => m.FixtureId).ToHashSet(StringComparer.Ordinal);
        Assert.All(parsed.PlayerMatches, p => Assert.Contains(p.FixtureId, fixtureIds));
        Assert.All(parsed.PlayerMatches, p => Assert.Equal("home", p.Location));
        Assert.Equal(0, parsed.PlayerMatches.Count(p => p.PlayerName is null));
    }

    [Fact]
    public void TeamTableParser_AwayPayload_UsesAidSideAndOpponentIsHome()
    {
        var json = File.ReadAllText(FixturePath("table_away.json"));

        var parsed = FmTeamTableParser.Parse(json, 18701, "away", 15, "corners");

        Assert.Equal(4, parsed.TeamMatches.Count);
        Assert.All(parsed.TeamMatches, m =>
        {
            Assert.Equal("away", m.Location);
            Assert.NotEqual(18701, m.OpponentApid);
            Assert.False(string.IsNullOrEmpty(m.TeamStatsJson));
        });
        Assert.All(parsed.Warnings, w => Assert.DoesNotContain("differs from requested", w));
        Assert.All(parsed.PlayerMatches, p => Assert.Equal("away", p.Location));
    }

    [Fact]
    public void TeamTableParser_LocationComesFromIds_NotFromRequestedParam()
    {
        const string json = """
        {
          "fixtures": [
            { "id": "9001", "apid": 770001, "timestamp": "2026-01-05T15:00:00.000Z",
              "lid": 10, "hid": 42, "aid": 77, "hgoals": 2, "agoals": 1,
              "opponent": { "apid": 77, "name": "Alpha" } }
          ],
          "leagues": { "10": { "name": "Test League" } },
          "teamStats": { "770001": { "77": { "corners": 5 } } },
          "opponentStrength": { "770001": { "goals": { "v": "1.00" } } },
          "pivotData": { "555": { "770001": { "goals": 1, "mins": 90 } } },
          "players": [ { "apid": 555, "shortName": "J. Test" } ]
        }
        """;

        var parsed = FmTeamTableParser.Parse(json, 42, "away", 15, "corners");

        var match = Assert.Single(parsed.TeamMatches);
        Assert.Equal("home", match.Location);
        Assert.Equal(77, match.OpponentApid);
        Assert.Equal("Alpha", match.Opponent);
        Assert.Equal(10, match.LeagueApid);
        Assert.Equal("Test League", match.League);
        Assert.Contains("differs from requested",
            parsed.Warnings.Single(w => w.Contains("differs from requested")));

        var player = Assert.Single(parsed.PlayerMatches);
        Assert.Equal("J. Test", player.PlayerName);
        Assert.Equal("home", player.Location);
        Assert.Equal("9001", player.FixtureId);
        Assert.True(player.TsUtc == new DateTime(2026, 1, 5, 15, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public async Task Store_UpsertTeamMatches_IsIdempotent_AndReadsBackWithPlayers()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            await store.EnsureSchemaAsync();
            var parsed = FmTeamTableParser.Parse(
                File.ReadAllText(FixturePath("table_home.json")), 18701, "home", 15, "corners");

            var first = await store.UpsertTeamMatchesAsync(
                18701, "home", 15, "corners",
                parsed.TeamMatches, parsed.PlayerMatches,
                "teams/table?location=home&period=15&stat=corners", DateTime.UtcNow);
            Assert.Equal(parsed.TeamMatches.Count, first.Written);
            Assert.Equal(parsed.TeamMatches.Count, first.Inserted);
            Assert.Equal(0, first.Updated);
            Assert.Equal(parsed.PlayerMatches.Count, first.PlayerWritten);

            var second = await store.UpsertTeamMatchesAsync(
                18701, "home", 15, "corners",
                parsed.TeamMatches, parsed.PlayerMatches,
                "teams/table?location=home&period=15&stat=corners", DateTime.UtcNow);
            Assert.Equal(parsed.TeamMatches.Count, second.Written);
            Assert.Equal(0, second.Inserted);
            Assert.Equal(parsed.TeamMatches.Count, second.Updated);

            var home = await store.GetTeamMatchesAsync(18701, "home", 15, includePlayers: true);
            Assert.Equal(4, home.Count);
            Assert.All(home, m =>
            {
                Assert.Equal("home", m.Location);
                Assert.All(m.Players, p => Assert.Equal(m.FixtureId, p.FixtureId));
                Assert.NotEmpty(m.Players);
            });

            var ronaldo = home.SelectMany(m => m.Players)
                .Where(p => p.PlayerApid == 580)
                .ToList();
            Assert.NotEmpty(ronaldo);
            Assert.All(ronaldo, p =>
            {
                Assert.Equal("C. Ronaldo", p.PlayerName);
                Assert.Equal("home", p.Location);
                Assert.NotNull(p.StatsJson);
                JsonDocument.Parse(p.StatsJson!);
            });

            Assert.Empty(await store.GetTeamMatchesAsync(18701, "away", null, false));
            Assert.Equal(4, (await store.GetTeamMatchesAsync(18701, null, null, false)).Count);

            await using (var conn = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={dbPath}"))
            {
                await conn.OpenAsync();
                await using var cmd = conn.CreateCommand();
                cmd.CommandText =
                    "SELECT (SELECT COUNT(*) FROM fm_team_matches), (SELECT COUNT(DISTINCT fixture_id) FROM fm_team_matches), (SELECT COUNT(*) FROM fm_player_matches);";
                await using var reader = await cmd.ExecuteReaderAsync();
                Assert.True(await reader.ReadAsync());
                Assert.Equal(4, reader.GetInt32(0));
                Assert.Equal(4, reader.GetInt32(1));
                Assert.Equal(parsed.PlayerMatches.Count, reader.GetInt32(2));
            }
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    [Fact]
    public async Task Store_GetPlayerMatches_FiltersByFixtureSet()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            var parsed = FmTeamTableParser.Parse(
                File.ReadAllText(FixturePath("table_away.json")), 18701, "away", 15, "corners");
            await store.UpsertTeamMatchesAsync(
                18701, "away", 15, "corners",
                parsed.TeamMatches, parsed.PlayerMatches, "teams/table", DateTime.UtcNow);

            var all = await store.GetPlayerMatchesAsync(18701, "away", 15, null);
            Assert.Equal(parsed.PlayerMatches.Count, all.Count);

            var oneFixture = parsed.PlayerMatches.First().FixtureId;
            var filtered = await store.GetPlayerMatchesAsync(
                18701, "away", 15, new[] { oneFixture });
            Assert.All(filtered, p => Assert.Equal(oneFixture, p.FixtureId));
            Assert.NotEmpty(filtered);
            Assert.True(filtered.Count < all.Count);
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    // ---- P7 (Parte E) --------------------------------------------------------

    private const string ReportFixtureId = "33441811";

    private static FmTeamMatchDraft TeamDraft(
        long teamApid, string fixtureId, DateTime ts, string location,
        long opponentApid, string opponent, int hg, int ag, string statsJson) =>
        new(teamApid, fixtureId, 900_000 + (Math.Abs(fixtureId.GetHashCode()) % 100_000),
            ts, location, opponentApid, opponent, null, "UEFA Nations League",
            hg, ag, statsJson, null, 15, "corners");

    private static string Stats(int saves, int corners) =>
        $"{{\"saves\":{saves},\"corners\":{corners},\"sh\":10,\"cards\":1}}";

    private static string PlayerStats(int shots) => $"{{\"sh\":{shots},\"sot\":1}}";

    private static string History(int entries, double value, int hits) =>
        "[" + string.Join(",", Enumerable.Range(0, entries).Select(i =>
            $"{{\"t\":\"2026-{(9 - i / 4):00}-{(20 - i * 2):00}T18:00:00.000Z\"," +
            $"\"vt\":{value},\"met\":{(i < hits ? "true" : "false")}}}")) + "]";

    private static async Task SeedReportDataAsync(
        FmSnapshotStore store, bool withTeamRows, bool withOdds)
    {
        var snapshotId = await store.InsertSnapshotAsync(
            ReportFixtureId, "team-trends", "https://www.footymetrics.com/fixtures/1-x",
            DateTime.UtcNow, "p", "s", "fm-json-v1", "OK");

        var signals = new List<FmSignalDraft>
        {
            new("team", "Portugal", "home_saves", 1.5, "over", 10, 10, 1.0,
                null, null, null, History(10, 5, 10)),
            new("team", "Wales", "away_corners", 3.5, "over", 8, 10, 0.8,
                null, null, null, History(10, 6, 8)),
            new("player", "C. Ronaldo", "shots", 1.5, "over", 6, 8, 0.75,
                null, null, null, History(8, 3, 6))
        };
        await store.InsertSignalsAsync(
            snapshotId, ReportFixtureId, signals, "all", "all", DateTime.UtcNow,
            """{"location":"all"}""");

        if (withOdds)
        {
            await store.InsertOddsAsync(ReportFixtureId, new List<FmOddsDraft>
            {
                new("team", "Portugal", "home_saves", 1.5, "1", 1.91, "over"),
                new("team", "Wales", "away_corners", 3.5, "1", 2.05, "over")
            }, snapshotId, DateTime.UtcNow);
            await store.UpsertManualOddsAsync(
                ReportFixtureId, "Betano", "home_saves", 1.5, 2.00, DateTime.UtcNow);
            await store.UpsertManualOddsAsync(
                ReportFixtureId, "Betano", "away_corners", 3.5, 1.85, DateTime.UtcNow);
        }

        if (!withTeamRows) return;

        var kickoff = new DateTime(2026, 9, 24, 18, 45, 0, DateTimeKind.Utc);
        var ptHome = new List<FmTeamMatchDraft>
        {
            TeamDraft(18701, ReportFixtureId, kickoff, "home", 18721, "Wales", 1, 0, Stats(5, 7)),
            TeamDraft(18701, "fx-pt-h1", kickoff.AddDays(-4), "home", 18721, "Wales", 2, 1, Stats(4, 8)),
            TeamDraft(18701, "fx-pt-h2", kickoff.AddDays(-8), "home", 18873, "Serbia", 1, 1, Stats(3, 5)),
            TeamDraft(18701, "fx-pt-h3", kickoff.AddDays(-12), "home", 18568, "Greece", 3, 0, Stats(6, 9)),
            TeamDraft(18701, "fx-pt-h4", kickoff.AddDays(-16), "home", 18721, "Wales", 2, 2, Stats(2, 6)),
            TeamDraft(18701, "fx-pt-h5", kickoff.AddDays(-20), "home", 18660, "Germany", 0, 1, Stats(1, 4)),
            TeamDraft(18701, "fx-pt-h6", kickoff.AddDays(-24), "home", 18873, "Serbia", 1, 0, Stats(7, 10))
        };
        var ptAway = new List<FmTeamMatchDraft>
        {
            TeamDraft(18701, "fx-pt-a1", kickoff.AddDays(-2), "away", 18721, "Wales", 1, 1, Stats(3, 6)),
            TeamDraft(18701, "fx-pt-a2", kickoff.AddDays(-6), "away", 18660, "Germany", 0, 2, Stats(2, 5)),
            TeamDraft(18701, "fx-pt-a3", kickoff.AddDays(-10), "away", 18873, "Serbia", 1, 0, Stats(5, 7)),
            TeamDraft(18701, "fx-pt-a4", kickoff.AddDays(-14), "away", 18568, "Greece", 2, 0, Stats(4, 8)),
            TeamDraft(18701, "fx-pt-a5", kickoff.AddDays(-18), "away", 18721, "Wales", 0, 0, Stats(1, 3)),
            TeamDraft(18701, "fx-pt-a6", kickoff.AddDays(-22), "away", 18660, "Germany", 1, 2, Stats(6, 9))
        };
        var waAway = new List<FmTeamMatchDraft>
        {
            TeamDraft(18721, ReportFixtureId, kickoff, "away", 18701, "Portugal", 1, 0, Stats(1, 4)),
            TeamDraft(18721, "fx-wa-a1", kickoff.AddDays(-4), "away", 18660, "Germany", 0, 3, Stats(2, 7)),
            TeamDraft(18721, "fx-wa-a2", kickoff.AddDays(-8), "away", 18643, "Austria", 1, 1, Stats(3, 5)),
            TeamDraft(18721, "fx-wa-a3", kickoff.AddDays(-12), "away", 18654, "Ireland", 2, 0, Stats(2, 9)),
            TeamDraft(18721, "fx-wa-a4", kickoff.AddDays(-16), "away", 18657, "Israel", 1, 2, Stats(4, 6)),
            TeamDraft(18721, "fx-wa-a5", kickoff.AddDays(-20), "away", 18870, "Liechtenstein", 5, 0, Stats(1, 8)),
            TeamDraft(18721, "fx-wa-a6", kickoff.AddDays(-24), "away", 27065, "Lithuania", 3, 1, Stats(2, 5))
        };
        var waHome = new List<FmTeamMatchDraft>
        {
            TeamDraft(18721, "fx-wa-h1", kickoff.AddDays(-2), "home", 18643, "Austria", 1, 0, Stats(3, 6)),
            TeamDraft(18721, "fx-wa-h2", kickoff.AddDays(-6), "home", 18654, "Ireland", 0, 1, Stats(2, 4)),
            TeamDraft(18721, "fx-wa-h3", kickoff.AddDays(-10), "home", 18657, "Israel", 2, 2, Stats(5, 7)),
            TeamDraft(18721, "fx-wa-h4", kickoff.AddDays(-14), "home", 18870, "Liechtenstein", 4, 0, Stats(1, 3)),
            TeamDraft(18721, "fx-wa-h5", kickoff.AddDays(-18), "home", 27065, "Lithuania", 2, 1, Stats(4, 9)),
            TeamDraft(18721, "fx-wa-h6", kickoff.AddDays(-22), "home", 18660, "Germany", 1, 3, Stats(2, 5))
        };

        var players = new List<FmPlayerMatchDraft>();
        foreach (var row in ptHome.Concat(ptAway).Concat(waHome).Concat(waAway))
        {
            players.Add(new FmPlayerMatchDraft(
                row.TeamApid, 580, "C. Ronaldo", row.FixtureId, row.FixtureApid,
                row.TsUtc, row.Location, "team", PlayerStats(row.TsUtc.Day % 5),
                15, "corners"));
        }

        await store.UpsertTeamMatchesAsync(
            18701, "home", 15, "corners", ptHome, players, "teams/table", DateTime.UtcNow);
        await store.UpsertTeamMatchesAsync(
            18701, "away", 15, "corners", ptAway, players, "teams/table", DateTime.UtcNow);
        await store.UpsertTeamMatchesAsync(
            18721, "home", 15, "corners", waHome, players, "teams/table", DateTime.UtcNow);
        await store.UpsertTeamMatchesAsync(
            18721, "away", 15, "corners", waAway, players, "teams/table", DateTime.UtcNow);
    }

    private static async Task<FmReport> BuildReportAsync(
        FmSnapshotStore store, FmOutcomeStore outcomes)
    {
        var input = await FmConfluenceReportLoader.LoadAsync(store, outcomes, ReportFixtureId);
        Assert.NotNull(input);
        return FmConfluenceReportBuilder.Build(ReportFixtureId, input!);
    }

    private static double StatOf(FmTeamMatchRow row, string stat)
    {
        using var doc = JsonDocument.Parse(row.TeamStatsJson!);
        return doc.RootElement.GetProperty(stat).GetDouble();
    }

    private static readonly System.Text.RegularExpressions.Regex Forbidden =
        new("recomend|apuesta|edge|value bet|elegid",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

    // F1: own_windows.last10 and venue_split equal a manual recount from
    // fm_team_matches for two markets (Portugal/home_saves, Wales/away_corners).
    [Fact]
    public async Task Report_OwnWindowsAndVenueSplit_MatchManualTeamMatchRecount()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            var outcomes = new FmOutcomeStore($"Data Source={dbPath}");
            await SeedReportDataAsync(store, withTeamRows: true, withOdds: true);

            var report = await BuildReportAsync(store, outcomes);

            Assert.Equal("sample_size_desc", report.SortCriteria);
            Assert.Equal("Portugal", report.Fixture.Home);
            Assert.Equal("Wales", report.Fixture.Away);
            Assert.Equal("2026-09-24T18:45:00Z", report.Fixture.KickoffUtc);

            foreach (var (apid, market, line, subject, stat) in new[]
                     {
                         (18701L, "home_saves", 1.5, "Portugal", "saves"),
                         (18721L, "away_corners", 3.5, "Wales", "corners")
                     })
            {
                var entry = Assert.Single(report.Markets,
                    m => m.Market == market && m.Subject == subject);
                var rows = (await store.GetTeamMatchesAsync(apid, null, null, false))
                    .GroupBy(r => r.FixtureId)
                    .Select(g => g.First())
                    .ToList();

                var last10 = rows.Take(10).ToList();
                var expectedHits = last10.Count(r => StatOf(r, stat) > line);
                var window = Assert.IsType<FmReportWindow>(entry.TeamAttack.OwnWindows["last10"]);
                Assert.Equal(last10.Count, window.N);
                Assert.Equal(expectedHits, window.Hits);

                foreach (var location in new[] { "home", "away" })
                {
                    var subset = rows.Where(r => r.Location == location).ToList();
                    var venue = Assert.IsType<FmReportVenue>(
                        entry.TeamAttack.VenueSplit[location]);
                    Assert.Equal(subset.Count, venue.N);
                    Assert.Equal(subset.Count(r => StatOf(r, stat) > line), venue.Hits);
                    Assert.Equal(
                        Math.Round(
                            (double)subset.Count(r => StatOf(r, stat) > line) / subset.Count, 4),
                        venue.Rate);
                }

                Assert.NotNull(entry.TeamAttack.FmReported);
                Assert.NotEmpty(entry.MarketOddsFm);
                var (manualValue, manualProb) = market == "home_saves"
                    ? (2.00, 0.5)
                    : (1.85, 0.541);
                Assert.Equal(manualValue, entry.ManualOdds.Value);
                Assert.Equal(manualProb, entry.ManualOdds.ImpliedProb);
            }

            var savesEntry = report.Markets.Single(m => m.Market == "home_saves");
            var opponentAll = Assert.IsType<FmReportWindow>(
                savesEntry.OpponentContext.ConcededEquivalent.OwnWindows["all"]);
            Assert.Equal(13, opponentAll.N);
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    // F2: no forbidden wording in the outputs; the mandated closing
    // disclaimer of E5 (which requires it verbatim) is stripped before the scan.
    [Fact]
    public async Task Report_SerializedJsonAndMarkdown_HaveNoForbiddenWords()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            var outcomes = new FmOutcomeStore($"Data Source={dbPath}");
            await SeedReportDataAsync(store, withTeamRows: true, withOdds: true);
            var report = await BuildReportAsync(store, outcomes);

            var json = JsonSerializer.Serialize(report);
            var md = FmConfluenceReportMarkdown.Render(report)
                .Replace(FmConfluenceReportMarkdown.ClosingNote, "");

            Assert.DoesNotMatch(Forbidden, json);
            Assert.DoesNotMatch(Forbidden, md);
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    // F3: missing data is labelled in JSON and markdown, never omitted.
    [Fact]
    public async Task Report_MissingData_MarksNoDataAndInsufficientSample()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            var outcomes = new FmOutcomeStore($"Data Source={dbPath}");
            await SeedReportDataAsync(store, withTeamRows: false, withOdds: false);
            var report = await BuildReportAsync(store, outcomes);

            var entry = Assert.Single(report.Markets, m => m.Market == "home_saves");
            Assert.All(entry.TeamAttack.OwnWindows.Values,
                v => Assert.Equal(FmConfluenceReportBuilder.StatusInsufficient, v));
            Assert.All(entry.TeamAttack.VenueSplit.Values,
                v => Assert.Equal(FmConfluenceReportBuilder.StatusNoData, v));
            Assert.Equal(FmConfluenceReportBuilder.StatusInsufficient,
                entry.OpponentContext.ConcededEquivalent.OwnWindows["all"]);
            Assert.Empty(entry.MarketOddsFm);
            Assert.Null(entry.ManualOdds.Value);
            Assert.Contains("no fm_team_matches rows", entry.DataQuality.Motivo);

            var md = FmConfluenceReportMarkdown.Render(report);
            Assert.Contains(FmConfluenceReportBuilder.StatusInsufficient, md);
            Assert.Contains(FmConfluenceReportBuilder.StatusNoData, md);
            Assert.Contains("no cargada", md);
            Assert.Contains("desconocido", md);
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    // F4: two renders of the same data are byte-identical (no timestamps).
    [Fact]
    public async Task Report_MarkdownAndJson_AreDeterministicAcrossRenders()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            var outcomes = new FmOutcomeStore($"Data Source={dbPath}");
            await SeedReportDataAsync(store, withTeamRows: true, withOdds: true);

            var first = await BuildReportAsync(store, outcomes);
            var second = await BuildReportAsync(store, outcomes);

            Assert.Equal(
                FmConfluenceReportMarkdown.Render(first),
                FmConfluenceReportMarkdown.Render(second));
            Assert.Equal(
                JsonSerializer.Serialize(first),
                JsonSerializer.Serialize(second));
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    // I2 (store side): manual odds update in place, never duplicate rows.
    [Fact]
    public async Task Store_UpsertManualOdds_UpdatesExistingRow()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            var first = await store.UpsertManualOddsAsync(
                ReportFixtureId, "Betano", "total_goals", 2.5, 1.90, DateTime.UtcNow);
            var second = await store.UpsertManualOddsAsync(
                ReportFixtureId, "Betano", "total_goals", 2.5, 2.10, DateTime.UtcNow);

            Assert.Equal(first, second);
            var rows = await store.GetOddsDetailAsync(ReportFixtureId);
            var manual = Assert.Single(rows, r => r.Source == "manual");
            Assert.Equal(2.10, manual.OddsValue);
            Assert.Equal("Betano", manual.BookmakerName);

            var otherLine = await store.UpsertManualOddsAsync(
                ReportFixtureId, "Betano", "total_goals", 3.5, 1.70, DateTime.UtcNow);
            Assert.NotEqual(first, otherLine);
            Assert.Equal(2, (await store.GetOddsDetailAsync(ReportFixtureId)).Count);
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    // P7-T3 bug found in validation: fm_outcome keeps the signal id of an older
    // snapshot while the report reads the newest copy (ids drift per capture),
    // so source_conflict/motivo were silently dropped. The loader now re-keys
    // outcomes by (subject_type, subject_name, market, line).
    [Fact]
    public async Task Report_DataQuality_JoinsOutcomeFromOlderSnapshot()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            var outcomes = new FmOutcomeStore($"Data Source={dbPath}");

            var draft = new List<FmSignalDraft>
            {
                new("team", "Portugal", "home_saves", 1.5, "over", 10, 10, 1.0,
                    null, null, null, History(10, 5, 10))
            };
            foreach (var _ in new[] { 1, 2 })
            {
                var snap = await store.InsertSnapshotAsync(
                    ReportFixtureId, "team-trends", "https://www.footymetrics.com/fixtures/1-x",
                    DateTime.UtcNow, "p", "s", "fm-json-v1", "OK");
                await store.InsertSignalsAsync(
                    snap, ReportFixtureId, draft, "all", "all", DateTime.UtcNow,
                    """{"location":"all"}""");
            }

            var ids = (await store.GetSignalIdentitiesAsync(ReportFixtureId)).Keys
                .OrderBy(i => i).ToList();
            Assert.Equal(2, ids.Count);
            var olderId = ids[0];
            var latestId = ids[1];

            await outcomes.WriteAsync(new[]
            {
                new FmOutcomeDraft(olderId, ReportFixtureId, 5.0, 1, "RESOLVED", "test", null)
            }, DateTime.UtcNow);
            await outcomes.MarkSourceConflictsAsync(new[] { olderId });

            var report = await BuildReportAsync(store, outcomes);
            var entry = Assert.Single(report.Markets, m => m.Market == "home_saves");
            Assert.True(entry.DataQuality.SourceConflict,
                "outcome written against the older snapshot id must still be visible");
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }
}
