using MatchEdge.Domain.Models;

namespace MatchEdge.Application.UseCases.Statistics;

/// <summary>
/// Provee estadísticas de temporada completa (TeamStatistics) respetando un corte temporal
/// (asOfDateTime), derivándolas de TeamContextStatistics en vez de consultar datos en vivo.
/// Uso: backtesting y fallback del Modelo B (EnhancedLambdaCalculator). No usar en el
/// endpoint de predicciones en producción, que debe seguir usando IStatisticsService (en vivo).
/// </summary>
public interface IHistoricalTeamStatisticsProvider
{
    Task<TeamStatistics> GetAsOfAsync(
        int teamId,
        int tournamentId,
        DateTime asOfDateTime,
        int seasonLookback = 2);
}
