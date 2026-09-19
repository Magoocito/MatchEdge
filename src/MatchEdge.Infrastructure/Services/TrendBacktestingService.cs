using MatchEdge.Application.Services;
using MatchEdge.Infrastructure.Data;
using MatchEdge.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MatchEdge.Infrastructure.Services;

public class TrendBacktestingService : ITrendBacktestingService
{
    private readonly MatchEdgeDbContext _db;
    private readonly ILogger<TrendBacktestingService> _logger;

    public TrendBacktestingService(
        MatchEdgeDbContext db,
        ILogger<TrendBacktestingService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<BacktestResult> RunBacktestAsync(
        DateTime fromDate,
        DateTime toDate,
        double stakePerUnit = 1.0,
        CancellationToken ct = default)
    {
        var from = fromDate.Date;
        var to = toDate.Date.AddDays(1);

        _logger.LogInformation(
            "Running backtest from {From} to {To}", from, to);

        var picks = await _db.DailyPicks
            .Where(p => p.Date >= from && p.Date < to)
            .OrderBy(p => p.Date)
            .ToListAsync(ct);

        var outcomes = await _db.PickOutcomes
            .Where(o => o.EvaluatedAt >= from && o.EvaluatedAt < to)
            .ToListAsync(ct);

        var outcomeMap = outcomes
            .GroupBy(o => o.DailyPickId)
            .ToDictionary(g => g.Key, g => g.First());

        var won = 0;
        var lost = 0;
        var pending = 0;
        var voided = 0;
        double totalStaked = 0;
        double totalProfit = 0;
        double balance = 0;
        double peakBalance = 0;
        double maxDrawdown = 0;
        var dailyResults = new List<DayBacktestResult>();
        var currentDay = from;
        var dayPicks = new List<DailyPickEntity>();

        foreach (var pick in picks)
        {
            if (outcomeMap.TryGetValue(pick.Id, out var outcome))
            {
                if (outcome.Won)
                {
                    won++;
                    var profit = (pick.Odds - 1) * stakePerUnit;
                    totalProfit += profit;
                    balance += profit;
                }
                else
                {
                    lost++;
                    totalProfit -= stakePerUnit;
                    balance -= stakePerUnit;
                }
                totalStaked += stakePerUnit;
            }
            else if (pick.Status == "Won")
            {
                won++;
                var profit = (pick.Odds - 1) * stakePerUnit;
                totalProfit += profit;
                balance += profit;
                totalStaked += stakePerUnit;
            }
            else if (pick.Status == "Lost")
            {
                lost++;
                totalProfit -= stakePerUnit;
                balance -= stakePerUnit;
                totalStaked += stakePerUnit;
            }
            else if (pick.Status == "Voided")
            {
                voided++;
            }
            else
            {
                pending++;
            }

            if (balance > peakBalance)
                peakBalance = balance;

            var drawdown = peakBalance - balance;
            if (drawdown > maxDrawdown)
                maxDrawdown = drawdown;

            dayPicks.Add(pick);

            if (pick.Date.Date > currentDay.Date || pick == picks.Last())
            {
                var dayWon = dayPicks.Count(p => p.Status == "Won");
                var dayLost = dayPicks.Count(p => p.Status == "Lost");
                var dayProfit = dayPicks.Sum(p =>
                {
                    if (p.Status == "Won") return (p.Odds - 1) * stakePerUnit;
                    if (p.Status == "Lost") return -stakePerUnit;
                    return 0;
                });

                dailyResults.Add(new DayBacktestResult(
                    currentDay,
                    dayPicks.Count,
                    dayWon,
                    dayLost,
                    dayProfit,
                    balance
                ));

                currentDay = pick.Date.Date;
                dayPicks.Clear();
            }
        }

        var evaluated = won + lost;
        var winRate = evaluated > 0 ? (double)won / evaluated : 0;
        var roi = totalStaked > 0 ? totalProfit / totalStaked : 0;
        var yield_ = picks.Count > 0 ? totalProfit / picks.Count : 0;

        var pickDtos = picks.Select(p => MapToDto(p)).ToList();

        _logger.LogInformation(
            "Backtest complete: {Total} picks, {Won} won, {Lost} lost, " +
            "Win rate: {WinRate:P1}, ROI: {ROI:P1}, Profit: {Profit:F2}",
            picks.Count, won, lost, winRate, roi, totalProfit);

        return new BacktestResult(
            fromDate,
            toDate,
            picks.Count,
            won,
            lost,
            pending,
            voided,
            winRate,
            totalStaked,
            totalProfit,
            roi,
            yield_,
            maxDrawdown,
            peakBalance,
            pickDtos,
            dailyResults
        );
    }

    public async Task<BacktestSummary> GetSummaryAsync(
        CancellationToken ct = default)
    {
        var allPicks = await _db.DailyPicks.ToListAsync(ct);
        var allOutcomes = await _db.PickOutcomes.ToListAsync(ct);

        var won = allPicks.Count(p => p.Status == "Won");
        var lost = allPicks.Count(p => p.Status == "Lost");
        var pending = allPicks.Count(p => p.Status == "Pending");
        var evaluated = won + lost;

        var winRate = evaluated > 0 ? (double)won / evaluated : 0;
        var totalStaked = evaluated * 1.0;
        var totalProfit = allOutcomes.Sum(o => o.Profit);
        var roi = totalStaked > 0 ? totalProfit / totalStaked : 0;
        var yield_ = allPicks.Count > 0 ? totalProfit / allPicks.Count : 0;

        var lastOutcome = allOutcomes
            .OrderByDescending(o => o.EvaluatedAt)
            .FirstOrDefault();

        return new BacktestSummary(
            allPicks.Count,
            won,
            lost,
            pending,
            winRate,
            totalStaked,
            totalProfit,
            roi,
            yield_,
            totalProfit,
            lastOutcome?.EvaluatedAt ?? DateTime.MinValue
        );
    }

    public async Task<List<DailyPickEntityDto>> GetEvaluatablePicksAsync(
        DateTime date,
        CancellationToken ct = default)
    {
        var from = date.Date;
        var to = from.AddDays(1);

        return await _db.DailyPicks
            .Where(p => p.Date >= from && p.Date < to && p.Status == "Pending")
            .OrderByDescending(p => p.Score)
            .Select(p => MapToDto(p))
            .ToListAsync(ct);
    }

    private static DailyPickEntityDto MapToDto(DailyPickEntity p)
    {
        return new DailyPickEntityDto(
            p.Id,
            p.Date,
            p.CreatedAt,
            p.Team,
            p.Market,
            p.Description,
            p.Score,
            p.Classification,
            p.Edge,
            p.Odds,
            p.HitRate,
            p.SampleSize,
            p.StakeUnits,
            p.Source,
            p.Status
        );
    }
}
