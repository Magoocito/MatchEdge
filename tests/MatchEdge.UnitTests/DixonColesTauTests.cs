using MatchEdge.Application.UseCases.Probability;

namespace MatchEdge.UnitTests;

// P13: la corrección Dixon-Coles es código nuevo (el motor de Poisson sigue
// siendo producto de marginales), así que se valida por separado: rho = 0 debe
// desactivarla del todo y rho = 0.1 debe bajar 0-0/1-1 y subir 1-0/0-1,
// comprobando el signo sobre tau Y sobre la celda ya ajustada.
public class DixonColesTauTests
{
    private const double Rho = DixonColes.DefaultRho;
    private const double LambdaHome = 1.5;
    private const double LambdaAway = 1.2;

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 3)]
    [InlineData(4, 4)]
    public void Tau_WithZeroRho_IsOneForEveryCell(int x, int y)
    {
        Assert.Equal(1.0, DixonColes.Tau(x, y, LambdaHome, LambdaAway, 0d));
    }

    [Fact]
    public void Tau_WithDefaultRho_KeepsTheSignsTheBriefRequires()
    {
        // rho = 0.1: 0-0 y 1-1 bajan (< 1), 1-0 y 0-1 suben (> 1).
        Assert.True(DixonColes.Tau(0, 0, LambdaHome, LambdaAway, Rho) < 1.0);
        Assert.True(DixonColes.Tau(1, 1, LambdaHome, LambdaAway, Rho) < 1.0);
        Assert.True(DixonColes.Tau(1, 0, LambdaHome, LambdaAway, Rho) > 1.0);
        Assert.True(DixonColes.Tau(0, 1, LambdaHome, LambdaAway, Rho) > 1.0);

        Assert.Equal(0.82, DixonColes.Tau(0, 0, LambdaHome, LambdaAway, Rho), 10);
        Assert.Equal(0.90, DixonColes.Tau(1, 1, LambdaHome, LambdaAway, Rho), 10);
        Assert.Equal(1.15, DixonColes.Tau(0, 1, LambdaHome, LambdaAway, Rho), 10);
        Assert.Equal(1.12, DixonColes.Tau(1, 0, LambdaHome, LambdaAway, Rho), 10);
    }

    [Fact]
    public void Tau_OnCellsOutsideZeroZeroToElevenOne_IsNeutral()
    {
        Assert.Equal(1.0, DixonColes.Tau(2, 0, LambdaHome, LambdaAway, Rho), 10);
        Assert.Equal(1.0, DixonColes.Tau(0, 2, LambdaHome, LambdaAway, Rho), 10);
        Assert.Equal(1.0, DixonColes.Tau(3, 3, LambdaHome, LambdaAway, Rho), 10);
    }

    [Fact]
    public void Tau_IsNeverNegative_WhenTheProductExceedsTheInverseOfRho()
    {
        // 1 - 6*6*0.1 = -2.6 -> se recorta a 0 para no producir celdas negativas.
        Assert.Equal(0.0, DixonColes.Tau(0, 0, 6d, 6d, Rho));
    }

    [Theory]
    [InlineData(-1.01)]
    [InlineData(1.01)]
    public void Tau_WithRhoOutsideRange_Throws(double rho)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DixonColes.Tau(0, 0, LambdaHome, LambdaAway, rho));
    }

    [Fact]
    public void AdjustedMatrix_KeepsTheDirectionOfEveryChangedCell()
    {
        const int maxGoals = 8;
        var baseCells = PoissonCells(maxGoals);
        var adjusted = AdjustedCells(maxGoals);

        Assert.True(adjusted[0, 0] < baseCells[0, 0], "P(0,0) debe bajar con rho > 0");
        Assert.True(adjusted[1, 1] < baseCells[1, 1], "P(1,1) debe bajar con rho > 0");
        Assert.True(adjusted[1, 0] > baseCells[1, 0], "P(1,0) debe subir con rho > 0");
        Assert.True(adjusted[0, 1] > baseCells[0, 1], "P(0,1) debe subir con rho > 0");

        // La matriz ajustada sigue siendo una distribución (masa normalizada).
        double sum = 0;
        for (var x = 0; x <= maxGoals; x++)
        for (var y = 0; y <= maxGoals; y++)
            sum += adjusted[x, y];
        Assert.Equal(1.0, sum, 6);
    }

    [Fact]
    public void AdjustedMatrix_WithZeroRho_EqualsTheIndependentPoissonMatrix()
    {
        const int maxGoals = 8;
        var expected = Normalize(PoissonCells(maxGoals));
        var actual = AdjustedCells(maxGoals, 0d);

        for (var x = 0; x <= maxGoals; x++)
        for (var y = 0; y <= maxGoals; y++)
            Assert.Equal(expected[x, y], actual[x, y], 10);
    }

    private static double[,] Normalize(double[,] cells)
    {
        var sum = 0d;
        for (var x = 0; x < cells.GetLength(0); x++)
        for (var y = 0; y < cells.GetLength(1); y++)
            sum += cells[x, y];
        for (var x = 0; x < cells.GetLength(0); x++)
        for (var y = 0; y < cells.GetLength(1); y++)
            cells[x, y] /= sum;
        return cells;
    }

    private static double[,] PoissonCells(int maxGoals)
    {
        var cells = new double[maxGoals + 1, maxGoals + 1];
        for (var x = 0; x <= maxGoals; x++)
        for (var y = 0; y <= maxGoals; y++)
            cells[x, y] = Poisson(LambdaHome, x) * Poisson(LambdaAway, y);
        return cells;
    }

    private static double[,] AdjustedCells(int maxGoals, double rho = Rho)
    {
        var raw = new double[maxGoals + 1, maxGoals + 1];
        var sum = 0d;
        for (var x = 0; x <= maxGoals; x++)
        for (var y = 0; y <= maxGoals; y++)
        {
            var value = Poisson(LambdaHome, x) * Poisson(LambdaAway, y) *
                        DixonColes.Tau(x, y, LambdaHome, LambdaAway, rho);
            raw[x, y] = value;
            sum += value;
        }

        for (var x = 0; x <= maxGoals; x++)
        for (var y = 0; y <= maxGoals; y++)
            raw[x, y] /= sum;
        return raw;
    }

    private static double Poisson(double lambda, int x) =>
        Math.Exp(-lambda) * Math.Pow(lambda, x) / Factorial(x);

    private static double Factorial(int n)
    {
        var result = 1d;
        for (var i = 2; i <= n; i++) result *= i;
        return result;
    }
}
