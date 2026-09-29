namespace MatchEdge.Application.UseCases.Probability;

/// <summary>
/// P13: corrección de Dixon-Coles (función tau) sobre la matriz de Poisson
/// bivariada. Es una clase NUEVA, deliberadamente al margen de
/// IProbabilityEngine / PoissonProbabilityEngine, para no alterar sus tests
/// (BttsTests, PoissonProbabilityEngineTests, MatchResultProbabilitiesTests).
///
/// Formulación clásica (Dixon &amp; Coles 1997, celdas de 0-0 a 1-1):
///   tau(0,0) = 1 - lambdaHome * lambdaAway * rho   (se recorta a 0 si el
///                                                    producto lo haría negativo)
///   tau(0,1) = 1 + lambdaHome * rho
///   tau(1,0) = 1 + lambdaAway  * rho
///   tau(1,1) = 1 - rho
///   resto de celdas = 1
/// Con rho = 0 la corrección desaparece (tau = 1 para todo x,y), que es el
/// caso de control que exige el brief P13.
/// </summary>
public static class DixonColes
{
    /// <summary>rho de trabajo del P13 (brief: "Dixon-Coles rho=0.1").</summary>
    public const double DefaultRho = 0.1;

    /// <summary>
    /// Factor de corrección tau para la celda (x,y) de la matriz de goles.
    /// </summary>
    /// <param name="x">Goles del equipo local en la celda.</param>
    /// <param name="y">Goles del equipo visitante en la celda.</param>
    /// <param name="lambdaHome">Goles esperados del equipo local.</param>
    /// <param name="lambdaAway">Goles esperados del equipo visitante.</param>
    /// <param name="rho">Parámetro de correlación de Dixon-Coles.</param>
    public static double Tau(int x, int y, double lambdaHome, double lambdaAway, double rho)
    {
        if (x < 0) throw new ArgumentException("x must be non-negative", nameof(x));
        if (y < 0) throw new ArgumentException("y must be non-negative", nameof(y));
        if (lambdaHome < 0) throw new ArgumentException("Lambda must be non-negative", nameof(lambdaHome));
        if (lambdaAway < 0) throw new ArgumentException("Lambda must be non-negative", nameof(lambdaAway));
        if (double.IsNaN(rho) || rho < -1 || rho > 1)
            throw new ArgumentOutOfRangeException(nameof(rho), "rho must be within [-1, 1].");

        if (rho == 0d) return 1d;

        return (x, y) switch
        {
            (0, 0) => Math.Max(0d, 1d - (lambdaHome * lambdaAway * rho)),
            (0, 1) => 1d + (lambdaHome * rho),
            (1, 0) => 1d + (lambdaAway * rho),
            (1, 1) => 1d - rho,
            _ => 1d
        };
    }
}
