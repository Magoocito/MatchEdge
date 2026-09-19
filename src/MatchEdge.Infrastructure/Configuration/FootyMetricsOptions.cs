namespace MatchEdge.Infrastructure.Configuration;

public class FootyMetricsOptions
{
    public string BaseUrl { get; set; } = "https://www.footymetrics.com";
    public string TrendsApiPath { get; set; } = "/api/front/trends/fixtures";
}
