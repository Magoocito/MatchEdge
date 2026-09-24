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
            Assert.Equal(1, await store.InsertIgnoreAsync(new[] { draft }, DateTime.UtcNow));
            Assert.Equal(0, await store.InsertIgnoreAsync(new[] { draft }, DateTime.UtcNow));

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
}
