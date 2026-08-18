// Paired Bootstrap Analysis for Backtest Results
// Usage: dotnet run -- <path-to-backtest-result.json>
//
// Extracts splitOnly (HomeAwaySplit) matches, computes Brier score differences
// between Model A and Model B1, and runs paired bootstrap (N=1000) to produce
// 95% percentile CI for the difference.
//
// NOTE: PowerShell implementation was abandoned due to a subtle type-coercion bug
// where array += operations in loops caused the bootstrap mean to converge to ~1/4
// of the observed value. C# double[] with explicit indexing is correct.

using System;
using System.Text.Json;

var json = File.ReadAllText(args[0]);
var doc = JsonDocument.Parse(json);
var details = doc.RootElement.GetProperty("details");

var diffs = new List<double>();
foreach (var m in details.EnumerateArray())
{
    if (m.GetProperty("calculationMethod").GetString() != "HomeAwaySplit")
        continue;

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
    diffs.Add(brierA - brierB1);
}

int n = diffs.Count;
double obsMean = diffs.Average();
Console.WriteLine($"SplitOnly matches: {n}");
Console.WriteLine($"Observed mean diff (A - B1): {obsMean}");

// Paired bootstrap: resample paired differences with replacement
int N = 1000;
var rng = new Random(42);
var bootMeans = new double[N];

for (int iter = 0; iter < N; iter++)
{
    double sum = 0;
    for (int i = 0; i < n; i++)
    {
        int idx = rng.Next(n);
        sum += diffs[idx];
    }
    bootMeans[iter] = sum / n;
}

double bootMean = bootMeans.Average();
var sorted = bootMeans.OrderBy(x => x).ToArray();
double median = sorted[N / 2];
double lo = sorted[(int)(N * 0.025)];
double hi = sorted[(int)(N * 0.975)];

Console.WriteLine($"Bootstrap mean: {bootMean}");
Console.WriteLine($"Bootstrap median: {median}");
Console.WriteLine($"95% CI: [{lo}, {hi}]");
Console.WriteLine($"Observed inside CI: {(obsMean >= lo && obsMean <= hi ? "YES" : "NO")}");
Console.WriteLine($"Ratio boot/obs: {bootMean / obsMean}");
