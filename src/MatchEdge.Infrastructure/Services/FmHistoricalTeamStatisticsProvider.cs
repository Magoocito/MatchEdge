using MatchEdge.Application.UseCases.Statistics;
using MatchEdge.Domain.Models;

namespace MatchEdge.Infrastructure.Services;

// P13: IHistoricalTeamStatisticsProvider alternativo construido sobre
// fm_team_matches. La implementación existente (HistoricalTeamStatisticsProvider)
// lee SofaScore/SeasonService; el P13 no tiene esas temporadas para las
// selecciones, así que el fallback de EnhancedLambdaCalculator se resuelve con
// las mismas filas FM que alimentan el adaptador.
public sealed class FmHistoricalTeamStatisticsProvider : IHistoricalTeamStatisticsProvider
{
    private readonly FmSnapshotStore _store;

    public FmHistoricalTeamStatisticsProvider(FmSnapshotStore store)
    {
        _store = store;
    }

    public async Task<Domain.Models.TeamStatistics> GetAsOfAsync(
        int teamId,
        int tournamentId,
        DateTime asOfDateTime,
        int seasonLookback = 2)
    {
        var rows = await _store.GetTeamMatchesAsync(teamId, null, null, false);
        return FmTeamContextAdapter.ToTeamStatistics(rows, asOfDateTime);
    }
}
