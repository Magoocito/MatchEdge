// Paired Bootstrap Analysis for Backtest Results (Extended)
// Usage: dotnet run -- <path-to-backtest-result.json>
//
// Compares:
//   1. Model A vs Model B1 (on HomeAwaySplit matches)
//   2. Model A vs Market (on matches with market odds)
//   3. Model B1 vs Market (on matches with market odds)

using System;
using System.Text.Json;

var json = File.ReadAllText(args[0]);
var doc = JsonDocument.Parse(json);
var details = doc.RootElement.GetProperty("details");

// Collect per-match differences for each comparison
var diffsA_B1 = new List<double>();   // A - B1 (HomeAwaySplit)
var diffsA_Mkt = new List<double>();  // A - Market (hasMarketOdds)
var diffsB1_Mkt = new List<double>(); // B1 - Market (hasMarketOdds)

foreach (var m in details.EnumerateArray())
{
    var method = m.GetProperty("calculationMethod").GetString()!;
    var actual = m.GetProperty("actualResult").GetString()!;
    double ah = actual == "H" ? 1.0 : 0.0;
    double ad = actual == "D" ? 1.0 : 0.0;
    double aa = actual == "A" ? 1.0 : 0.0;

    double aHw = m.GetProperty("modelA_HomeWinProb").GetDouble();
    double aD = m.GetProperty("modelA_DrawProb").GetDouble();
    double aAw = m.GetProperty("modelA_AwayWinProb").GetDouble();
    double bHw = m.GetProperty("modelB1_HomeWinProb").GetDouble();
    double bD = m.GetProperty("modelB1_DrawProb").GetDouble();
    double bAw = m.GetProperty("modelB1_AwayWinProb").GetDouble();

    double brierA = Math.Pow(aHw - ah, 2) + Math.Pow(aD - ad, 2) + Math.Pow(aAw - aa, 2);
    double brierB1 = Math.Pow(bHw - ah, 2) + Math.Pow(bD - ad, 2) + Math.Pow(bAw - aa, 2);

    // Comparison 1: A vs B1 (HomeAwaySplit only)
    if (method == "HomeAwaySplit")
    {
        diffsA_B1.Add(brierA - brierB1);
    }

    // Comparisons 2 & 3: vs Market (only if market odds exist)
    bool hasMarket = false;
    double mHw = 0, mD = 0, mAw = 0;
    if (m.TryGetProperty("market_HomeWinProb", out var mHwEl) &&
        m.TryGetProperty("market_DrawProb", out var mDEl) &&
        m.TryGetProperty("market_AwayWinProb", out var mAwEl) &&
        mHwEl.ValueKind == JsonValueKind.Number &&
        mDEl.ValueKind == JsonValueKind.Number &&
        mAwEl.ValueKind == JsonValueKind.Number)
    {
        mHw = mHwEl.GetDouble();
        mD = mDEl.GetDouble();
        mAw = mAwEl.GetDouble();
        hasMarket = true;
    }

    if (hasMarket)
    {
        double brierMkt = Math.Pow(mHw - ah, 2) + Math.Pow(mD - ad, 2) + Math.Pow(mAw - aa, 2);
        diffsA_Mkt.Add(brierA - brierMkt);
        diffsB1_Mkt.Add(brierB1 - brierMkt);
    }
}

// Run paired bootstrap for each comparison
void RunBootstrap(string label, List<double> diffs)
{
    int n = diffs.Count;
    if (n == 0) { Console.WriteLine($"{label}: no data"); return; }

    double obsMean = diffs.Average();
    int N = 1000;
    var rng = new Random(42);
    var bootMeans = new double[N];

    for (int iter = 0; iter < N; iter++)
    {
        double sum = 0;
        for (int i = 0; i < n; i++)
            sum += diffs[rng.Next(n)];
        bootMeans[iter] = sum / n;
    }

    var sorted = bootMeans.OrderBy(x => x).ToArray();
    double median = sorted[N / 2];
    double lo = sorted[(int)(N * 0.025)];
    double hi = sorted[(int)(N * 0.975)];
    bool includesZero = (lo <= 0 && hi >= 0);
    double effectSize = Math.Abs(obsMean) / 0.01; // in units of 0.01 Brier

    Console.WriteLine($"  n={n}  Observed diff={obsMean:F6}  95% CI=[{lo:F6}, {hi:F6}]  Includes 0: {includesZero}  Effect={effectSize:F2}x0.01");
    Console.WriteLine($"  Bootstrap mean={bootMeans.Average():F6}  median={median:F6}");
}

Console.WriteLine("=== PAIRED BOOTSTRAP ANALYSIS (N=1000, seed=42) ===");
Console.WriteLine();

Console.WriteLine("1. Model A vs Model B1 (HomeAwaySplit matches):");
RunBootstrap("A vs B1", diffsA_B1);

Console.WriteLine();
Console.WriteLine("2. Model A vs Market (matches with market odds):");
RunBootstrap("A vs Market", diffsA_Mkt);

Console.WriteLine();
Console.WriteLine("3. Model B1 vs Market (matches with market odds):");
RunBootstrap("B1 vs Market", diffsB1_Mkt);

Console.WriteLine();
Console.WriteLine("Interpretation:");
Console.WriteLine("  - Positive diff = FIRST model has HIGHER (worse) Brier score");
Console.WriteLine("  - Negative diff = FIRST model has LOWER (better) Brier score");
Console.WriteLine("  - CI includes 0 = no significant difference");
