using MatchEdge.Infrastructure.Clients;
using MatchEdge.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

namespace MatchEdge.UnitTests;

public class SofaScoreBrowserSeasonServiceTests
{
    private const int TournamentId = 406;

    [Fact]
    public async Task GetRecentSeasonIdsAsOfAsync_StartTimestampZero_FallsBackToYearParsing()
    {
        var seasons = new List<SeasonInfo>
        {
            new() { Id = 57741, Name = "2024", Year = "2024", StartTimestamp = 0 },
            new() { Id = 50000, Name = "2023", Year = "2023", StartTimestamp = 0 },
            new() { Id = 40000, Name = "2022", Year = "2022", StartTimestamp = 0 }
        };
        var collector = CreateCollector(seasons);
        var service = CreateService(collector);

        var result = await service.GetRecentSeasonIdsAsOfAsync(TournamentId, 3, new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        Assert.Equal(3, result.Count);
        Assert.Equal(57741, result[0]);
        Assert.Equal(50000, result[1]);
        Assert.Equal(40000, result[2]);
    }

    [Fact]
    public async Task GetRecentSeasonIdsAsOfAsync_StartTimestampZero_InvalidYear_Excluded()
    {
        var seasons = new List<SeasonInfo>
        {
            new() { Id = 57741, Name = "2024", Year = "2024", StartTimestamp = 0 },
            new() { Id = 50000, Name = "Unknown", Year = "unknown", StartTimestamp = 0 }
        };
        var collector = CreateCollector(seasons);
        var service = CreateService(collector);

        var result = await service.GetRecentSeasonIdsAsOfAsync(TournamentId, 3, new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        Assert.Single(result);
        Assert.Equal(57741, result[0]);
    }

    [Fact]
    public async Task GetRecentSeasonIdsAsOfAsync_StartTimestampPositive_UsesTimestamp()
    {
        var seasons = new List<SeasonInfo>
        {
            new() { Id = 57741, Name = "2024", Year = "2024", StartTimestamp = 1704067200 },
            new() { Id = 50000, Name = "2023", Year = "2023", StartTimestamp = 1672531200 }
        };
        var collector = CreateCollector(seasons);
        var service = CreateService(collector);

        var result = await service.GetRecentSeasonIdsAsOfAsync(TournamentId, 2, new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc));

        Assert.Equal(2, result.Count);
        Assert.Equal(57741, result[0]);
        Assert.Equal(50000, result[1]);
    }

    [Fact]
    public async Task GetRecentSeasonIdsAsOfAsync_MixedTimestamps_FiltersCorrectly()
    {
        var seasons = new List<SeasonInfo>
        {
            new() { Id = 57741, Name = "2024", Year = "2024", StartTimestamp = 0 },
            new() { Id = 50000, Name = "2023", Year = "2023", StartTimestamp = 1672531200 },
            new() { Id = 40000, Name = "2022", Year = "2022", StartTimestamp = 0 }
        };
        var collector = CreateCollector(seasons);
        var service = CreateService(collector);

        var result = await service.GetRecentSeasonIdsAsOfAsync(TournamentId, 3, new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Utc));

        Assert.Equal(2, result.Count);
        Assert.Equal(50000, result[0]);
        Assert.Equal(40000, result[1]);
    }

    [Fact]
    public async Task GetRecentSeasonIdsAsOfAsync_MultipleSeasonsSameYear_OrdersById()
    {
        var seasons = new List<SeasonInfo>
        {
            new() { Id = 57742, Name = "2024 Apertura", Year = "2024", StartTimestamp = 0 },
            new() { Id = 57741, Name = "2024 Clausura", Year = "2024", StartTimestamp = 0 }
        };
        var collector = CreateCollector(seasons);
        var service = CreateService(collector);

        var result = await service.GetRecentSeasonIdsAsOfAsync(TournamentId, 2, new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        Assert.Equal(2, result.Count);
        Assert.Equal(57742, result[0]);
        Assert.Equal(57741, result[1]);
    }

    [Fact]
    public async Task GetRecentSeasonIdsAsOfAsync_NoSeasons_ThrowsException()
    {
        var collector = CreateCollector(new List<SeasonInfo>());
        var service = CreateService(collector);

        await Assert.ThrowsAsync<Exception>(() =>
            service.GetRecentSeasonIdsAsOfAsync(TournamentId, 3, new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
    }

    [Fact]
    public async Task GetRecentSeasonIdsAsOfAsync_CountZero_ThrowsArgumentException()
    {
        var collector = CreateCollector(new List<SeasonInfo>());
        var service = CreateService(collector);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetRecentSeasonIdsAsOfAsync(TournamentId, 0, new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
    }

    [Fact]
    public async Task GetCurrentSeasonAsync_ReturnsFirstSeason()
    {
        var seasons = new List<SeasonInfo>
        {
            new() { Id = 57741, Name = "2024", Year = "2024", StartTimestamp = 0 }
        };
        var collector = CreateCollector(seasons);
        var service = CreateService(collector);

        var result = await service.GetCurrentSeasonAsync(TournamentId);

        Assert.Equal(57741, result);
    }

    [Fact]
    public async Task GetRecentSeasonIdsAsOfAsync_CacheHit_ReturnsCached()
    {
        var seasons = new List<SeasonInfo>
        {
            new() { Id = 57741, Name = "2024", Year = "2024", StartTimestamp = 0 }
        };
        var collector = CreateCollector(seasons);
        var service = CreateService(collector);

        var first = await service.GetRecentSeasonIdsAsOfAsync(TournamentId, 1, new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var second = await service.GetRecentSeasonIdsAsOfAsync(TournamentId, 1, new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        Assert.Single(first);
        Assert.Equal(57741, first[0]);
        Assert.Equal(1, collector.CallCount);
    }

    private static FakeBrowserCollector CreateCollector(List<SeasonInfo> seasons)
    {
        var response = new SeasonResponse { Seasons = seasons };
        var json = System.Text.Json.JsonSerializer.Serialize(response);
        return new FakeBrowserCollector(json);
    }

    private static SofaScoreBrowserSeasonService CreateService(FakeBrowserCollector collector)
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var logger = NullLogger<SofaScoreBrowserSeasonService>.Instance;
        return new SofaScoreBrowserSeasonService(collector, cache, logger);
    }

    private class FakeBrowserCollector : ISofaScoreBrowserCollector
    {
        private readonly string _json;
        public int CallCount { get; private set; }

        public FakeBrowserCollector(string json) => _json = json;

        public Task StartAsync(CancellationToken ct = default) => Task.CompletedTask;

        public Task<bool> WaitForReadyAsync(TimeSpan? timeout = null, CancellationToken ct = default)
            => Task.FromResult(true);

        public Task<string?> FetchJsonAsync(string apiPath, CancellationToken ct = default)
        {
            CallCount++;
            return Task.FromResult<string?>(_json);
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
