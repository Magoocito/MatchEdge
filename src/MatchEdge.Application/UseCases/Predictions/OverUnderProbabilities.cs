namespace MatchEdge.Application.UseCases.Predictions;

public record OverUnderProbabilities(
    double Over1_5,
    double Under1_5,
    double Over2_5,
    double Under2_5,
    double Over3_5,
    double Under3_5);
