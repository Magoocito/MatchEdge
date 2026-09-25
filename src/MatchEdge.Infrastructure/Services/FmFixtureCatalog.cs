using System.Collections.Concurrent;
using MatchEdge.Application.Clients.FootyMetrics;

namespace MatchEdge.Infrastructure.Services;

public sealed class FmFixtureCatalog
{
    private readonly ConcurrentDictionary<string, FmFixtureRef> _byId = new(StringComparer.Ordinal);

    public void UpsertRange(IEnumerable<FmFixtureRef> fixtures)
    {
        foreach (var f in fixtures) _byId[f.FixtureId] = f;
    }

    public void Upsert(FmFixtureRef fixture) => _byId[fixture.FixtureId] = fixture;

    public FmFixtureRef? TryGet(string fixtureId) =>
        _byId.TryGetValue(fixtureId, out var f) ? f : null;
}
