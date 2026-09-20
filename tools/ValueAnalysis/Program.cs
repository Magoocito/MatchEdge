using System.Text.Json;

var jsonPath = args.Length > 0 ? args[0] : "tools/backtest-result-corrected.json";
var json = File.ReadAllText(jsonPath);
var doc = JsonDocument.Parse(json);
var details = doc.RootElement.GetProperty("details");

var candidates = new List<(long MatchId, string Date, string Home, string Away,
    double ModelA, double MarketOdds, double Edge, string Bet, double ModelProb)>();

int totalWithMarket = 0;
int totalValue = 0;
int homeValue = 0;
int drawValue = 0;
int awayValue = 0;

foreach (var m in details.EnumerateArray())
{
    if (!m.TryGetProperty("market_HomeWinProb", out var mHw) ||
        !m.TryGetProperty("market_DrawProb", out var mD) ||
        !m.TryGetProperty("market_AwayWinProb", out var mAw) ||
        mHw.ValueKind != JsonValueKind.Number ||
        mD.ValueKind != JsonValueKind.Number ||
        mAw.ValueKind != JsonValueKind.Number) continue;

    totalWithMarket++;

    double mktH = mHw.GetDouble();
    double mktD = mD.GetDouble();
    double mktA = mAw.GetDouble();
    double modH = m.GetProperty("modelA_HomeWinProb").GetDouble();
    double modD = m.GetProperty("modelA_DrawProb").GetDouble();
    double modA = m.GetProperty("modelA_AwayWinProb").GetDouble();

    string matchDate = m.GetProperty("matchDate").GetString() ?? "";
    long matchId = m.GetProperty("matchId").GetInt64();
    string home = m.GetProperty("homeTeamId").GetInt64().ToString();
    string away = m.GetProperty("awayTeamId").GetInt64().ToString();

    double edgeH = modH - mktH;
    double edgeD = modD - mktD;
    double edgeA = modA - mktA;

    string actualResultStr = m.GetProperty("actualResult").GetString() ?? "";
    int actualResult = actualResultStr switch { "H" => 1, "D" => 0, "A" => 2, _ => -1 };

    if (edgeH > 0.05) {
        homeValue++;
        totalValue++;
        candidates.Add((matchId, matchDate, home, away, modH, 1/mktH, edgeH, "1", modH));
    }
    if (edgeD > 0.05) {
        drawValue++;
        totalValue++;
        candidates.Add((matchId, matchDate, home, away, modD, 1/mktD, edgeD, "X", modD));
    }
    if (edgeA > 0.05) {
        awayValue++;
        totalValue++;
        candidates.Add((matchId, matchDate, home, away, modA, 1/mktA, edgeA, "2", modA));
    }
}

var sorted = candidates.OrderByDescending(c => c.Edge).Take(30).ToList();

Console.WriteLine("=== PHASE 3D: EDGE/EV ANALYSIS (CORRECTED) ===");
Console.WriteLine();
Console.WriteLine($"Matches with market odds: {totalWithMarket}");
Console.WriteLine($"Value bets (>5% edge): {totalValue} ({totalValue * 100.0 / totalWithMarket:F1}% of matches)");
Console.WriteLine($"  Home (1): {homeValue} ({homeValue * 100.0 / totalValue:F1}%)");
Console.WriteLine($"  Draw (X): {drawValue} ({drawValue * 100.0 / totalValue:F1}%)");
Console.WriteLine($"  Away (2): {awayValue} ({awayValue * 100.0 / totalValue:F1}%)");
Console.WriteLine();
Console.WriteLine("=== TOP 30 VALUE BETS (by edge) ===");
Console.WriteLine("  #  | Date       | Home vs Away                    | Bet | Model% | Mkt Odds | Edge");
Console.WriteLine("  ---|------------|--------------------------------|-----|--------|----------|------");
int rank = 1;
foreach (var (id, date, home, away, modProb, mktOdds, edge, bet, _) in sorted)
{
    string d = date.Length >= 10 ? date.Substring(5, 5) : date;
    string teams = $"{home} vs {away}";
    if (teams.Length > 30) teams = teams.Substring(0, 30);
    Console.WriteLine($"  {rank,2} | {d} | {teams,-30} |  {bet,-1} | {modProb * 100,5:F1}% | {mktOdds,8:F2} | {edge * 100,4:F1}%");
    rank++;
}
Console.WriteLine();
Console.WriteLine("=== STRATEGY ANALYSIS ===");
Console.WriteLine("Filtro: Model A prob > Market implied + 5% margin");
Console.WriteLine("Apuesta: home win cuando Model A >= 60% y Market < 55%");
Console.WriteLine();
Console.WriteLine("Profit si apostar $1 en cada value bet (pagando al mercado):");
double profit = 0;
int wins = 0;
foreach (var c in candidates)
{
    int actualResult = 0;
    foreach (var m in details.EnumerateArray())
    {
        if (m.GetProperty("matchId").GetInt64() == c.MatchId)
        {
            string arStr = m.GetProperty("actualResult").GetString() ?? "";
            actualResult = arStr switch { "H" => 1, "D" => 0, "A" => 2, _ => -1 };
            break;
        }
    }

    double payout = 0;
    if (c.Bet == "1" && actualResult == 1) payout = c.MarketOdds;
    if (c.Bet == "X" && actualResult == 0) payout = c.MarketOdds;
    if (c.Bet == "2" && actualResult == 2) payout = c.MarketOdds;

    if (payout > 0) { profit += payout - 1; wins++; }
    else { profit -= 1; }
}

Console.WriteLine($"  Bets: {totalValue} | Wins: {wins} ({wins * 100.0 / totalValue:F1}%)");
Console.WriteLine($"  Profit: {profit:F2} per $1 bet | ROI: {(profit / totalValue) * 100:F1}%");
Console.WriteLine($"  Avg odds of value bets: {candidates.Average(c => c.MarketOdds):F2}");
Console.WriteLine($"  Avg edge: {candidates.Average(c => c.Edge) * 100:F1}%");
