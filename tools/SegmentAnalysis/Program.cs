using System.Text.Json;

var jsonPath = args.Length > 0 ? args[0] : "tools/backtest-result-fixed2.json";
var json = File.ReadAllText(jsonPath);
var doc = JsonDocument.Parse(json);
var details = doc.RootElement.GetProperty("details");

// Collect all matches with market odds
var matches = new List<JsonElement>();
foreach (var m in details.EnumerateArray())
{
    bool hasMarket = m.TryGetProperty("market_HomeWinProb", out var mHw) &&
                     m.TryGetProperty("market_DrawProb", out var mD) &&
                     m.TryGetProperty("market_AwayWinProb", out var mAw) &&
                     mHw.ValueKind == JsonValueKind.Number &&
                     mD.ValueKind == JsonValueKind.Number &&
                     mAw.ValueKind == JsonValueKind.Number;
    if (hasMarket) matches.Add(m);
}

Console.WriteLine("=== PHASE 3C: SEGMENTATION ANALYSIS ===");
Console.WriteLine($"Matches with market odds: {matches.Count}");
Console.WriteLine();

// Define segments
var segments = new List<(string Name, Func<JsonElement, bool> Predicate)>();

// By odds range (market favorite probability)
segments.Add(("Favorite strong (≥60%)", m => {
    var hw = m.GetProperty("market_HomeWinProb").GetDouble();
    var dw = m.GetProperty("market_DrawProb").GetDouble();
    var aw = m.GetProperty("market_AwayWinProb").GetDouble();
    return Math.Max(hw, Math.Max(dw, aw)) >= 0.60;
}));
segments.Add(("Favorite moderate (50-60%)", m => {
    var hw = m.GetProperty("market_HomeWinProb").GetDouble();
    var dw = m.GetProperty("market_DrawProb").GetDouble();
    var aw = m.GetProperty("market_AwayWinProb").GetDouble();
    double maxP = Math.Max(hw, Math.Max(dw, aw));
    return maxP >= 0.50 && maxP < 0.60;
}));
segments.Add(("No favorite (<50%)", m => {
    var hw = m.GetProperty("market_HomeWinProb").GetDouble();
    var dw = m.GetProperty("market_DrawProb").GetDouble();
    var aw = m.GetProperty("market_AwayWinProb").GetDouble();
    return Math.Max(hw, Math.Max(dw, aw)) < 0.50;
}));

// By uncertainty (entropy)
double CalcEntropy(double h, double d, double a) {
    double[] ps = [h, d, a];
    double e = 0;
    foreach (var p in ps) if (p > 0) e -= p * Math.Log2(p);
    return e;
}
segments.Add(("Low entropy (≤1.5)", m => {
    var hw = m.GetProperty("market_HomeWinProb").GetDouble();
    var dw = m.GetProperty("market_DrawProb").GetDouble();
    var aw = m.GetProperty("market_AwayWinProb").GetDouble();
    return CalcEntropy(hw, dw, aw) <= 1.5;
}));
segments.Add(("Medium entropy (1.5-1.8)", m => {
    var hw = m.GetProperty("market_HomeWinProb").GetDouble();
    var dw = m.GetProperty("market_DrawProb").GetDouble();
    var aw = m.GetProperty("market_AwayWinProb").GetDouble();
    double e = CalcEntropy(hw, dw, aw);
    return e > 1.5 && e <= 1.8;
}));
segments.Add(("High entropy (>1.8)", m => {
    var hw = m.GetProperty("market_HomeWinProb").GetDouble();
    var dw = m.GetProperty("market_DrawProb").GetDouble();
    var aw = m.GetProperty("market_AwayWinProb").GetDouble();
    return CalcEntropy(hw, dw, aw) > 1.8;
}));

// By season
segments.Add(("Season 2023", m => {
    var d = m.GetProperty("matchDate").GetString() ?? "";
    return d.StartsWith("2023");
}));
segments.Add(("Season 2024", m => {
    var d = m.GetProperty("matchDate").GetString() ?? "";
    return d.StartsWith("2024");
}));
segments.Add(("Season 2025", m => {
    var d = m.GetProperty("matchDate").GetString() ?? "";
    return d.StartsWith("2025");
}));

// By calculation method
segments.Add(("HomeAwaySplit", m => {
    return m.GetProperty("calculationMethod").GetString() == "HomeAwaySplit";
}));
segments.Add(("SeasonAverageWithGamma", m => {
    return m.GetProperty("calculationMethod").GetString() == "SeasonAverageWithGamma";
}));

// By actual result
segments.Add(("Home wins", m => {
    return m.GetProperty("actualResult").GetString() == "H";
}));
segments.Add(("Draws", m => {
    return m.GetProperty("actualResult").GetString() == "D";
}));
segments.Add(("Away wins", m => {
    return m.GetProperty("actualResult").GetString() == "A";
}));

// By market Brier (market confidence)
segments.Add(("Market confident (Brier≤0.5)", m => {
    var hw = m.GetProperty("market_HomeWinProb").GetDouble();
    var dw = m.GetProperty("market_DrawProb").GetDouble();
    var aw = m.GetProperty("market_AwayWinProb").GetDouble();
    string actual = m.GetProperty("actualResult").GetString()!;
    double actualP = actual == "H" ? hw : actual == "D" ? dw : aw;
    return Math.Pow(1 - actualP, 2) <= 0.5;
}));
segments.Add(("Market uncertain (Brier>0.5)", m => {
    var hw = m.GetProperty("market_HomeWinProb").GetDouble();
    var dw = m.GetProperty("market_DrawProb").GetDouble();
    var aw = m.GetProperty("market_AwayWinProb").GetDouble();
    string actual = m.GetProperty("actualResult").GetString()!;
    double actualP = actual == "H" ? hw : actual == "D" ? dw : aw;
    return Math.Pow(1 - actualP, 2) > 0.5;
}));

// Bootstrap function
(double Diff, double Lo, double Hi, bool IncludesZero) RunBootstrap(List<double> diffs, int N = 1000)
{
    int n = diffs.Count;
    if (n == 0) return (0, 0, 0, true);
    double obsMean = diffs.Average();
    var rng = new Random(42);
    var bootMeans = new double[N];
    for (int iter = 0; iter < N; iter++)
    {
        double sum = 0;
        for (int i = 0; i < n; i++) sum += diffs[rng.Next(n)];
        bootMeans[iter] = sum / n;
    }
    var sorted = bootMeans.OrderBy(x => x).ToArray();
    double lo = sorted[(int)(N * 0.025)];
    double hi = sorted[(int)(N * 0.975)];
    return (obsMean, lo, hi, lo <= 0 && hi >= 0);
}

// Test each segment
var results = new List<(string Name, int N, double Diff, double Lo, double Hi, bool IncludesZero)>();

Console.WriteLine("=== SEGMENT RESULTS ===");
Console.WriteLine($"{"Segment",-35} {"N",5} {"Diff",8} {"95% CI",20} {"Incl.0",7}");
Console.WriteLine(new string('-', 80));

foreach (var (name, predicate) in segments)
{
    var segmentMatches = matches.Where(m => predicate(m)).ToList();
    if (segmentMatches.Count < 50)
    {
        Console.WriteLine($"{name,-35} {segmentMatches.Count,5} {"SKIP (<50)",8}");
        continue;
    }

    var diffs = new List<double>();
    foreach (var m in segmentMatches)
    {
        var actual = m.GetProperty("actualResult").GetString()!;
        double ah = actual == "H" ? 1.0 : 0.0;
        double ad = actual == "D" ? 1.0 : 0.0;
        double aa = actual == "A" ? 1.0 : 0.0;

        double aHw = m.GetProperty("modelA_HomeWinProb").GetDouble();
        double aD = m.GetProperty("modelA_DrawProb").GetDouble();
        double aAw = m.GetProperty("modelA_AwayWinProb").GetDouble();
        double mHw = m.GetProperty("market_HomeWinProb").GetDouble();
        double mD = m.GetProperty("market_DrawProb").GetDouble();
        double mAw = m.GetProperty("market_AwayWinProb").GetDouble();

        double brierA = Math.Pow(aHw - ah, 2) + Math.Pow(aD - ad, 2) + Math.Pow(aAw - aa, 2);
        double brierM = Math.Pow(mHw - ah, 2) + Math.Pow(mD - ad, 2) + Math.Pow(mAw - aa, 2);
        diffs.Add(brierA - brierM);
    }

    var (diff, lo, hi, inclZero) = RunBootstrap(diffs);
    results.Add((name, segmentMatches.Count, diff, lo, hi, inclZero));
    string ci = $"[{lo:F4}, {hi:F4}]";
    string incl = inclZero ? "YES" : "NO";
    Console.WriteLine($"{name,-35} {segmentMatches.Count,5} {diff,8:F4} {ci,20} {incl,7}");
}

// Bonferroni correction
Console.WriteLine();
Console.WriteLine("=== BONFERRONI CORRECTION (if >5 segments tested) ===");
int tested = results.Count;
double alpha = 0.05;
double correctedAlpha = tested > 5 ? alpha / tested : alpha;
Console.WriteLine($"Segments tested: {tested}");
Console.WriteLine($"Original α: {alpha}");
Console.WriteLine($"Corrected α (Bonferroni): {correctedAlpha:F4}");

if (tested > 5)
{
    Console.WriteLine();
    Console.WriteLine("=== SIGNIFICANT SEGMENTS (after Bonferroni) ===");
    foreach (var (name, n, diff, lo, hi, inclZero) in results.Where(r => !r.IncludesZero))
    {
        Console.WriteLine($"  {name}: n={n}, diff={diff:F4}, CI=[{lo:F4}, {hi:F4}]");
    }
    
    var significant = results.Where(r => !r.IncludesZero).ToList();
    if (significant.Count == 0)
        Console.WriteLine("  None survive Bonferroni correction");
    else
        Console.WriteLine($"  {significant.Count} segment(s) survive Bonferroni");
}

Console.WriteLine();
Console.WriteLine("=== ALL SIGNIFICANT SEGMENTS (before Bonferroni) ===");
foreach (var (name, n, diff, lo, hi, inclZero) in results.Where(r => !r.IncludesZero).OrderBy(r => r.Diff))
{
    Console.WriteLine($"  {name}: n={n}, diff={diff:F4} (market better by {Math.Abs(diff):F4}), CI=[{lo:F4}, {hi:F4}]");
}
