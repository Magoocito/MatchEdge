namespace MatchEdge.Application.Clients.FootyMetrics;

public record FootyMetricsTeamTrendResponse(
    string FixtureSlug,
    string HomeTeam,
    string AwayTeam,
    List<FootyMetricsTeamTrendData> Data
);

public record FootyMetricsTeamTrendData(
    string TeamName,
    string TeamSlug,
    string HitCount,
    string Market,
    string Odds,
    string Description,
    string Venue,
    string OppHitRate,
    string AvgValue,
    string RatePercentage,
    string Fixture
);

public record FootyMetricsOdds(
    decimal? Under,
    decimal? Over
);

public record FootyMetricsScrapedTrend(
    string Team,
    string TeamSlug,
    string Venue,
    string HitCount,
    string Market,
    string Odds,
    string Description,
    int HitNumerator,
    int HitDenominator,
    double HitRate,
    int OppHitRate,
    double AvgValue,
    double RatePercentage,
    string Fixture
);
