namespace MatchEdge.Application.Configuration;

public class MatchModelOptions
{
    /// <summary>
    /// Factor de ventaja de local. Multiplica los goles esperados del equipo local.
    /// Valor provisional basado en estudios de ligas europeas (Dixon-Coles ~1.3-1.4).
    /// Requiere calibración con histórico propio de Liga 1 Perú.
    /// </summary>
    public double HomeAdvantageFactor { get; set; } = 1.35;
}
