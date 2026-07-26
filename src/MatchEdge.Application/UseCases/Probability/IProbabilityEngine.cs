namespace MatchEdge.Application.UseCases.Probability;

public interface IProbabilityEngine
{
    /// <summary>
    /// Calcula la probabilidad de Poisson para un número específico de eventos.
    /// </summary>
    /// <param name="lambda">Promedio esperado de goles</param>
    /// <param name="x">Número específico de goles a calcular</param>
    /// <returns>Probabilidad entre 0 y 1</returns>
    double PoissonProbability(double lambda, int x);

    /// <summary>
    /// Calcula la probabilidad de Over/Under para una línea de goles.
    /// </summary>
    /// <param name="lambdaTotal">Promedio total de goles esperados</param>
    /// <param name="line">Línea de goles (ej: 2.5)</param>
    /// <param name="over">true para Over, false para Under</param>
    /// <returns>Probabilidad entre 0 y 1</returns>
    double GetOverUnderProbability(double lambdaTotal, double line, bool over);

    /// <summary>
    /// Calcula las probabilidades de resultado de un partido (victoria local, empate, victoria visitante) utilizando la distribución de Poisson.
    /// </summary>
    /// <param name="lambdaHome">Promedio esperado de goles del equipo local</param>
    /// <param name="lambdaAway">Promedio esperado de goles del equipo visitante</param>
    /// <param name="maxGoals">Número máximo de goles a considerar para el cálculo</param>
    /// <returns>Objeto MatchResultProbabilities con las probabilidades de cada resultado</returns>
    MatchResultProbabilities GetMatchResultProbabilities(double lambdaHome, double lambdaAway, int maxGoals = 10);
}
