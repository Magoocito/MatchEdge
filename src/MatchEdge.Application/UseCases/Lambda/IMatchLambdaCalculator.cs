using MatchEdge.Domain.Models;

namespace MatchEdge.Application.UseCases.Lambda;

public interface IMatchLambdaCalculator
{
    /// <summary>
    /// Calcula los lambdas esperados de goles para un partido, basándose en las estadísticas
    /// acumuladas de temporada de cada equipo y aplicando el factor de ventaja de local.
    /// </summary>
    /// <param name="home">Estadísticas del equipo local (acumuladas de temporada)</param>
    /// <param name="away">Estadísticas del equipo visitante (acumuladas de temporada)</param>
    /// <returns>Tupla con (lambdaHome, lambdaAway)</returns>
    (double lambdaHome, double lambdaAway) CalculateGoalLambdas(
        TeamStatistics home,
        TeamStatistics away);
}
