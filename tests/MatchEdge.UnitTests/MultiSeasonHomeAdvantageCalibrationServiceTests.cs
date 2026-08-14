using MatchEdge.Application.Services;
using MatchEdge.Application.UseCases.Calibration;
using MatchEdge.Application.UseCases.Historical;

namespace MatchEdge.UnitTests;

public class MultiSeasonHomeAdvantageCalibrationServiceTests
{
    private const int TournamentId = 406;

    [Fact]
    public async Task CalculateAsync_NullCalibrationAsOf_UsesGetRecentSeasonIdsAsync()
    {
        var fakeSeasonService = new FakeSeasonService();
        var fakeEnumerator = new FakeHistoricalMatchEnumerator([]);
        var sut = new MultiSeasonHomeAdvantageCalibrationService(fakeEnumerator, fakeSeasonService);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.CalculateAsync(TournamentId, seasonCount: 2, calibrationAsOf: null));

        Assert.True(fakeSeasonService.GetRecentSeasonIdsAsyncCalled);
        Assert.False(fakeSeasonService.GetRecentSeasonIdsAsOfAsyncCalled);
    }

    [Fact]
    public async Task CalculateAsync_WithCalibrationAsOf_UsesGetRecentSeasonIdsAsOfAsync()
    {
        var fakeSeasonService = new FakeSeasonService();
        var fakeEnumerator = new FakeHistoricalMatchEnumerator([]);
        var sut = new MultiSeasonHomeAdvantageCalibrationService(fakeEnumerator, fakeSeasonService);

        var calibrationAsOf = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.CalculateAsync(TournamentId, seasonCount: 2, calibrationAsOf: calibrationAsOf));

        Assert.False(fakeSeasonService.GetRecentSeasonIdsAsyncCalled);
        Assert.True(fakeSeasonService.GetRecentSeasonIdsAsOfAsyncCalled);
        Assert.Equal(calibrationAsOf, fakeSeasonService.LastCalibrationAsOf);
    }

    [Fact]
    public async Task CalculateAsync_CalibrationAsOf_PassesCorrectParameters()
    {
        var fakeSeasonService = new FakeSeasonService();
        var fakeEnumerator = new FakeHistoricalMatchEnumerator([]);
        var sut = new MultiSeasonHomeAdvantageCalibrationService(fakeEnumerator, fakeSeasonService);

        var calibrationAsOf = new DateTime(2024, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.CalculateAsync(TournamentId, seasonCount: 3, calibrationAsOf: calibrationAsOf));

        Assert.Equal(TournamentId, fakeSeasonService.LastTournamentId);
        Assert.Equal(3, fakeSeasonService.LastSeasonCount);
        Assert.Equal(calibrationAsOf, fakeSeasonService.LastCalibrationAsOf);
    }

    [Fact]
    public void CalculateAsync_NullCalibrationAsOf_DefaultsPreserveBackwardCompatibility()
    {
        IMultiSeasonHomeAdvantageCalibrationService service =
            new MultiSeasonHomeAdvantageCalibrationService(
                new FakeHistoricalMatchEnumerator([]), new FakeSeasonService());

        var method = typeof(IMultiSeasonHomeAdvantageCalibrationService).GetMethod("CalculateAsync");
        var parameters = method!.GetParameters();
        var calibrationAsOfParam = parameters.First(p => p.Name == "calibrationAsOf");

        Assert.Equal(typeof(DateTime?), calibrationAsOfParam.ParameterType);
        Assert.Null(calibrationAsOfParam.DefaultValue);
    }

    [Fact]
    public async Task CalculateAsync_PassesSeasonIdsToEnumerator()
    {
        var fakeSeasonService = new FakeSeasonService { SeasonsToReturn = [2024, 2023] };
        var fakeEnumerator = new FakeHistoricalMatchEnumerator([]);
        var sut = new MultiSeasonHomeAdvantageCalibrationService(fakeEnumerator, fakeSeasonService);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.CalculateAsync(TournamentId, seasonCount: 2));

        Assert.Equal(new List<int> { 2024, 2023 }, fakeEnumerator.LastSeasonIds);
        Assert.Equal(TournamentId, fakeEnumerator.LastTournamentId);
    }
}

internal class FakeHistoricalMatchEnumerator : IHistoricalMatchEnumerator
{
    private readonly IReadOnlyList<HistoricalMatch> _matchesToReturn;

    public int? LastTournamentId { get; private set; }
    public IReadOnlyList<int>? LastSeasonIds { get; private set; }

    public FakeHistoricalMatchEnumerator(IReadOnlyList<HistoricalMatch> matchesToReturn)
    {
        _matchesToReturn = matchesToReturn;
    }

    public Task<IReadOnlyList<HistoricalMatch>> GetFinishedMatchesAsync(
        int tournamentId,
        IReadOnlyList<int> seasonIds,
        int fromRound,
        int toRound,
        IReadOnlyList<string> prefixes)
    {
        LastTournamentId = tournamentId;
        LastSeasonIds = seasonIds;
        return Task.FromResult<IReadOnlyList<HistoricalMatch>>(_matchesToReturn);
    }
}

internal class FakeSeasonService : ISeasonService
{
    public bool GetRecentSeasonIdsAsyncCalled { get; private set; }
    public bool GetRecentSeasonIdsAsOfAsyncCalled { get; private set; }
    public int? LastTournamentId { get; private set; }
    public int? LastSeasonCount { get; private set; }
    public DateTime? LastCalibrationAsOf { get; private set; }
    public List<int> SeasonsToReturn { get; set; } = [];

    public Task<int> GetCurrentSeasonAsync(int tournamentId) => Task.FromResult(0);

    public Task<List<int>> GetRecentSeasonIdsAsync(int tournamentId, int count)
    {
        GetRecentSeasonIdsAsyncCalled = true;
        LastTournamentId = tournamentId;
        LastSeasonCount = count;
        return Task.FromResult(SeasonsToReturn);
    }

    public Task<List<int>> GetRecentSeasonIdsAsOfAsync(int tournamentId, int count, DateTime asOfDateTime)
    {
        GetRecentSeasonIdsAsOfAsyncCalled = true;
        LastTournamentId = tournamentId;
        LastSeasonCount = count;
        LastCalibrationAsOf = asOfDateTime;
        return Task.FromResult(SeasonsToReturn);
    }

    public Task<string> GetSeasonNameAsync(int tournamentId, int seasonId) =>
        Task.FromResult($"Season {seasonId}");
}
