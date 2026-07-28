namespace MatchEdge.Application.UseCases.Probability
{
    public record MatchResultProbabilities
    {
        public double HomeWin { get; init; }
        public double Draw { get; init; }
        public double AwayWin { get; init; }
    }
}
