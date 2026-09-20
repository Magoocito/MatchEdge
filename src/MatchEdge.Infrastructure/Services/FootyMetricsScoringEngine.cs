using MatchEdge.Application.Clients.FootyMetrics;

namespace MatchEdge.Infrastructure.Services;

public static class FootyMetricsScoringEngine
{
    public static TrendScoreResult Calculate(FootyMetricsTeamTrendData data)
    {
        var hitRate = ParsePercentage(data.RatePercentage);
        var oppHitRate = ParsePercentage(data.OppHitRate);
        var sampleSize = ParseSampleSize(data.HitCount);
        var oddsValue = ParseDecimal(data.Odds);

        var impliedProb = oddsValue > 0 ? (double)(1m / oddsValue) : 0.0;
        var edge = hitRate - impliedProb;

        var compositeScore = CalculateCompositeScore(hitRate, oppHitRate, sampleSize, edge);

        return new TrendScoreResult(
            data.TeamName,
            data.Market,
            data.Description.Contains("over") ? "Over" : "Under",
            ExtractLine(data.Market),
            hitRate,
            oppHitRate,
            sampleSize,
            edge,
            oddsValue,
            compositeScore,
            Classify(compositeScore, edge, sampleSize)
        );
    }

    public static TrendScoreResult CalculateFromScraped(FootyMetricsScrapedTrend trend)
    {
        var hitRate = trend.HitRate;
        var oppHitRate = trend.OppHitRate / 100.0;
        var sampleSize = trend.HitDenominator;
        var oddsValue = ParseDecimal(trend.Odds);

        var impliedProb = oddsValue > 0 ? (double)(1m / oddsValue) : 0.0;
        var edge = hitRate - impliedProb;

        var compositeScore = CalculateCompositeScore(hitRate, oppHitRate, sampleSize, edge);

        return new TrendScoreResult(
            trend.Team,
            trend.Market,
            trend.Description.Contains("over") ? "Over" : "Under",
            ExtractLine(trend.Market),
            hitRate,
            oppHitRate,
            sampleSize,
            edge,
            oddsValue,
            compositeScore,
            Classify(compositeScore, edge, sampleSize)
        );
    }

    private static double ParsePercentage(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0.0;

        var cleaned = value.Replace("%", "").Trim();
        if (double.TryParse(cleaned, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var result))
            return result / 100.0;

        return 0.0;
    }

    private static int ParseSampleSize(string hitCount)
    {
        if (string.IsNullOrWhiteSpace(hitCount))
            return 0;

        var parts = hitCount.Split('/');
        if (parts.Length == 2 && int.TryParse(parts[1], out var total))
            return total;

        return 0;
    }

    private static decimal ParseDecimal(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0;

        if (decimal.TryParse(value, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var result))
            return result;

        return 0;
    }

    private static string ExtractLine(string market)
    {
        var match = System.Text.RegularExpressions.Regex.Match(market, @"[\d.]+");
        return match.Success ? match.Value : "";
    }

    private static double CalculateCompositeScore(
        double hitRate, double oppHitRate, int sampleSize, double edge)
    {
        var hrComponent = hitRate * 0.35;
        var orComponent = (1.0 - oppHitRate) * 0.20;
        var ssComponent = Math.Min(sampleSize / 10.0, 1.0) * 0.20;
        var edgeComponent = Math.Max(edge, 0) * 0.25;

        return hrComponent + orComponent + ssComponent + edgeComponent;
    }

    private static string Classify(double score, double edge, int sample)
    {
        if (sample < 3) return "Insuficiente";
        if (score >= 0.75 && edge > 0.10) return "Excelente";
        if (score >= 0.60 && edge > 0.05) return "Muy Bueno";
        if (score >= 0.45 && edge > 0.02) return "Bueno";
        if (score >= 0.30) return "Neutral";
        return "Descartar";
    }

    public static List<TrendScoreResult> RankPicks(
        FootyMetricsTeamTrendResponse response, int topN = 10)
    {
        return response.Data
            .Select(Calculate)
            .Where(r => r.Classification != "Insuficiente" && r.Classification != "Descartar")
            .OrderByDescending(r => r.CompositeScore)
            .Take(topN)
            .ToList();
    }

    public static List<TrendScoreResult> RankScrapedPicks(
        List<FootyMetricsScrapedTrend> trends, int topN = 10)
    {
        return trends
            .Select(CalculateFromScraped)
            .Where(r => r.Classification != "Insuficiente" && r.Classification != "Descartar")
            .OrderByDescending(r => r.CompositeScore)
            .Take(topN)
            .ToList();
    }
}
