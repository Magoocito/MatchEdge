using MatchEdge.Infrastructure.Services;
using Xunit;

namespace MatchEdge.InfrastructureTests;

// P10: POST /api/fm/collect (item 5 de P2). Los tests cubren el orquestador
// (validacion, aislamiento de errores, orden) con delegates falsos: el
// fetch/parse/upsert reales viven en el controller de la Api, que este proyecto
// de tests no referencia, y probarlos exigiria un web host y sesion FM.
public class FmGlobalCollectTests
{
    [Fact]
    public void Validate_EmptyTeams_ReturnsError()
    {
        Assert.NotNull(FmGlobalCollectOrchestrator.Validate(
            null, 15, new[] { "home", "away" }, includePositions: false));
        Assert.Equal(
            "teams is required and must not be empty",
            FmGlobalCollectOrchestrator.Validate(
                Array.Empty<long>(), 15, new[] { "home", "away" }, includePositions: false));
    }

    [Fact]
    public void Validate_InvalidLocation_ReturnsError()
    {
        Assert.Equal(
            "invalid location(s): left,right",
            FmGlobalCollectOrchestrator.Validate(
                new[] { 18643L }, 15, new[] { "left", "right" }, includePositions: false));
    }

    [Fact]
    public void Validate_PeriodNotPositive_ReturnsError()
    {
        Assert.Equal(
            "period must be greater than 0",
            FmGlobalCollectOrchestrator.Validate(
                new[] { 18643L }, 0, new[] { "home" }, includePositions: false));
        Assert.NotNull(FmGlobalCollectOrchestrator.Validate(
            new[] { 18643L }, -30, new[] { "home" }, includePositions: false));
    }

    [Fact]
    public void Validate_IncludePositionsOutsideWhitelistPeriod_ReturnsError()
    {
        // position-stats solo admite {5,10,15,20} (whitelist P8 J3).
        Assert.NotNull(FmGlobalCollectOrchestrator.Validate(
            new[] { 18643L }, 30, new[] { "home" }, includePositions: true));
        Assert.Null(FmGlobalCollectOrchestrator.Validate(
            new[] { 18643L }, 30, new[] { "home" }, includePositions: false));
        Assert.Null(FmGlobalCollectOrchestrator.Validate(
            new[] { 18643L }, 20, new[] { "home" }, includePositions: true));
    }

    [Fact]
    public void Validate_ValidBody_ReturnsNull()
    {
        Assert.Null(FmGlobalCollectOrchestrator.Validate(
            new[] { 18643L, 18701L }, 30,
            new[] { "home", "away" }, includePositions: false));
    }

    [Fact]
    public async Task RunAsync_FailingFirstTeam_DoesNotBlockTheSecond()
    {
        var calls = new List<string>();
        var outcome = await FmGlobalCollectOrchestrator.RunAsync(
            new long[] { 18643, 18701 },
            period: 30,
            includePositions: false,
            (team, _) =>
            {
                calls.Add($"collect:{team}");
                if (team == 18643) throw new InvalidOperationException("fetch exploded");
                return Task.FromResult(new FmGlobalCollectBatch(new List<object>(), 2, 1));
            },
            collectPositions: null,
            CancellationToken.None);

        Assert.Equal(new[] { "collect:18643", "collect:18701" }, calls);
        Assert.Equal(2, outcome.Teams.Count);

        var failed = outcome.Teams[0];
        Assert.Equal("fetch exploded", failed.Error);
        Assert.Null(failed.Collect);

        var ok = outcome.Teams[1];
        Assert.Null(ok.Error);
        Assert.NotNull(ok.Collect);

        // 2 fetches del equipo sano + 1 error del equipo roto.
        Assert.Equal(2, outcome.Fetches);
        Assert.Equal(2, outcome.Errors);
        Assert.True(outcome.ElapsedMs >= 0);
    }

    [Fact]
    public async Task RunAsync_IncludePositions_RunsPositionsRightAfterEachCollect()
    {
        var calls = new List<string>();
        var outcome = await FmGlobalCollectOrchestrator.RunAsync(
            new long[] { 18643, 18701 },
            period: 15,
            includePositions: true,
            (team, _) =>
            {
                calls.Add($"collect:{team}");
                return Task.FromResult(new FmGlobalCollectBatch(new List<object>(), 4, 0));
            },
            (team, _) =>
            {
                calls.Add($"positions:{team}");
                return Task.FromResult(new FmGlobalCollectBatch(new List<object>(), 3, 0));
            },
            CancellationToken.None);

        Assert.Equal(
            new[] { "collect:18643", "positions:18643", "collect:18701", "positions:18701" },
            calls);
        Assert.Equal(8, outcome.Fetches);
        Assert.Equal(6, outcome.PositionFetches);
        Assert.Equal(0, outcome.Errors);
        Assert.All(outcome.Teams, t => Assert.NotNull(t.Positions));
    }

    [Fact]
    public async Task RunAsync_PositionsRequestOnlyCollectsPositionsWhenAsked()
    {
        var positionCalls = 0;
        var outcome = await FmGlobalCollectOrchestrator.RunAsync(
            new[] { 18643L },
            period: 15,
            includePositions: false,
            (_, _) => Task.FromResult(new FmGlobalCollectBatch(new List<object>(), 1, 0)),
            (_, _) =>
            {
                positionCalls++;
                return Task.FromResult(new FmGlobalCollectBatch(new List<object>(), 1, 0));
            },
            CancellationToken.None);

        Assert.Equal(0, positionCalls);
        Assert.Equal(0, outcome.PositionFetches);
        Assert.Null(Assert.Single(outcome.Teams).Positions);
    }

    [Fact]
    public async Task RunAsync_PositionsException_IsIsolatedPerTeam()
    {
        var outcome = await FmGlobalCollectOrchestrator.RunAsync(
            new long[] { 18643, 18701 },
            period: 15,
            includePositions: true,
            (_, _) => Task.FromResult(new FmGlobalCollectBatch(new List<object>(), 1, 0)),
            (team, _) => team == 18643
                ? throw new InvalidOperationException("positions exploded")
                : Task.FromResult(new FmGlobalCollectBatch(new List<object>(), 2, 0)),
            CancellationToken.None);

        Assert.Equal(2, outcome.Teams.Count);
        Assert.NotNull(outcome.Teams[0].Positions);
        Assert.NotNull(outcome.Teams[1].Positions);
        Assert.Equal(2, outcome.PositionFetches);
        // 1 error del equipo roto de positions + 0 del resto.
        Assert.Equal(1, outcome.Errors);
    }
}
