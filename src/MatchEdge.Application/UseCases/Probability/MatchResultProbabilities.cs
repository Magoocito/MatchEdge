namespace MatchEdge.Application.UseCases
{
    public record MatchResultProbabilities
    {
        public double HomeWin { get; init; }
        public double Draw { get; init; }
        public double AwayWin { get; init; }
    }
}
