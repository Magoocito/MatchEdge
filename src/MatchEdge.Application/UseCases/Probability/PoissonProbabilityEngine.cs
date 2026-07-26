namespace MatchEdge.Application.UseCases.Probability;

public class PoissonProbabilityEngine : IProbabilityEngine
{
    private const int MaxGoals = 15;

    public double PoissonProbability(double lambda, int x)
    {
        if (lambda < 0)
            throw new ArgumentException("Lambda must be non-negative", nameof(lambda));
        if (x < 0)
            throw new ArgumentException("x must be non-negative", nameof(x));

        return Math.Exp(-lambda) * Math.Pow(lambda, x) / Factorial(x);
    }

    public double GetOverUnderProbability(double lambdaTotal, double line, bool over)
    {
        if (lambdaTotal < 0)
            throw new ArgumentException("Lambda must be non-negative", nameof(lambdaTotal));
        if (line < 0)
            throw new ArgumentException("Line must be non-negative", nameof(line));

        double underProbability = 0;
        int maxGoalsForUnder = (int)Math.Floor(line);

        for (int x = 0; x <= Math.Min(maxGoalsForUnder, MaxGoals); x++)
        {
            underProbability += PoissonProbability(lambdaTotal, x);
        }

        double overProbability = 1 - underProbability;

        return over ? overProbability : underProbability;
    }

    private double Factorial(int n)
    {
        if (n == 0) return 1;

        double result = 1;
        for (int i = 1; i <= n; i++)
        {
            result *= i;
        }
        return result;
    }
}
