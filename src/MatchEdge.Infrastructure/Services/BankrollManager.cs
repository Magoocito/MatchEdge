using System.Text.Json;
using MatchEdge.Application.Services;
using MatchEdge.Infrastructure.Data;
using MatchEdge.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MatchEdge.Infrastructure.Services;

public class BankrollManager : IBankrollManager
{
    private readonly MatchEdgeDbContext _db;
    private readonly ILogger<BankrollManager> _logger;

    private const int DefaultConfigId = 1;
    private const int DefaultStateId = 1;

    public BankrollManager(
        MatchEdgeDbContext db,
        ILogger<BankrollManager> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<BankrollState> GetStateAsync(CancellationToken ct = default)
    {
        var state = await _db.BankrollStates
            .FirstOrDefaultAsync(s => s.Id == DefaultStateId, ct);

        var config = await GetConfigEntityAsync(ct);

        if (state == null)
        {
            return new BankrollState(
                InitialBankroll: config.InitialBankroll,
                CurrentBalance: config.InitialBankroll,
                PeakBalance: config.InitialBankroll,
                MaxDrawdown: 0,
                MaxDrawdownPercent: 0,
                TotalStaked: 0,
                TotalProfit: 0,
                ROI: 0,
                Yield: 0,
                TotalPicks: 0,
                Won: 0,
                Lost: 0,
                Pending: 0,
                WinRate: 0,
                Config: MapToConfig(config),
                LastUpdated: DateTime.UtcNow);
        }

        var picks = await _db.DailyPicks.ToListAsync(ct);
        var outcomes = await _db.PickOutcomes.ToListAsync(ct);

        var won = picks.Count(p => p.Status == "Won");
        var lost = picks.Count(p => p.Status == "Lost");
        var pending = picks.Count(p => p.Status == "Pending");
        var evaluated = won + lost;
        var winRate = evaluated > 0 ? (double)won / evaluated : 0;

        return new BankrollState(
            InitialBankroll: state.InitialBankroll,
            CurrentBalance: state.CurrentBalance,
            PeakBalance: state.PeakBalance,
            MaxDrawdown: state.MaxDrawdown,
            MaxDrawdownPercent: state.MaxDrawdownPercent,
            TotalStaked: state.TotalStaked,
            TotalProfit: state.TotalProfit,
            ROI: state.TotalStaked > 0 ? state.TotalProfit / state.TotalStaked : 0,
            Yield: picks.Count > 0 ? state.TotalProfit / picks.Count : 0,
            TotalPicks: picks.Count,
            Won: won,
            Lost: lost,
            Pending: pending,
            WinRate: winRate,
            Config: MapToConfig(config),
            LastUpdated: state.LastUpdated);
    }

    public async Task<BankrollState> InitializeAsync(
        double initialBankroll,
        BankrollConfig? config = null,
        CancellationToken ct = default)
    {
        if (config != null)
        {
            await UpdateConfigAsync(config, ct);
        }

        var configEntity = await GetConfigEntityAsync(ct);

        var state = await _db.BankrollStates
            .FirstOrDefaultAsync(s => s.Id == DefaultStateId, ct);

        if (state == null)
        {
            state = new BankrollStateEntity
            {
                Id = DefaultStateId,
                InitialBankroll = initialBankroll,
                CurrentBalance = initialBankroll,
                PeakBalance = initialBankroll,
                MaxDrawdown = 0,
                MaxDrawdownPercent = 0,
                TotalStaked = 0,
                TotalProfit = 0,
                TotalPicks = 0,
                Won = 0,
                Lost = 0,
                Pending = 0,
                LastUpdated = DateTime.UtcNow,
                ConfigJson = JsonSerializer.Serialize(MapToConfig(configEntity))
            };
            _db.BankrollStates.Add(state);
        }
        else
        {
            state.InitialBankroll = initialBankroll;
            state.CurrentBalance = initialBankroll;
            state.PeakBalance = initialBankroll;
            state.MaxDrawdown = 0;
            state.MaxDrawdownPercent = 0;
            state.LastUpdated = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Bankroll initialized: {Amount} units", initialBankroll);

        return await GetStateAsync(ct);
    }

    public async Task<List<StakeRecommendation>> CalculateStakesAsync(
        List<DailyPickEntityDto> picks,
        CancellationToken ct = default)
    {
        var config = await GetConfigEntityAsync(ct);
        var state = await GetStateAsync(ct);

        var todayExposure = await GetTodayExposureAsync(ct);
        var recommendations = new List<StakeRecommendation>();

        foreach (var pick in picks)
        {
            var recommendation = CalculateSingleStake(
                pick, config, state.CurrentBalance, todayExposure);
            recommendations.Add(recommendation);
        }

        return recommendations
            .OrderByDescending(r => r.RecommendedStake)
            .ToList();
    }

    public async Task<BankrollState> ApplyResultAsync(
        int pickId,
        bool won,
        double odds,
        double stakeUnits,
        CancellationToken ct = default)
    {
        var state = await _db.BankrollStates
            .FirstOrDefaultAsync(s => s.Id == DefaultStateId, ct);

        if (state == null)
        {
            _logger.LogWarning("Bankroll not initialized. Using defaults.");
            state = new BankrollStateEntity
            {
                Id = DefaultStateId,
                InitialBankroll = 100,
                CurrentBalance = 100,
                PeakBalance = 100,
                LastUpdated = DateTime.UtcNow
            };
            _db.BankrollStates.Add(state);
        }

        double profit;
        if (won)
        {
            profit = (odds - 1) * stakeUnits;
        }
        else
        {
            profit = -stakeUnits;
        }

        state.CurrentBalance += profit;
        state.TotalStaked += stakeUnits;
        state.TotalProfit += profit;
        state.TotalPicks++;

        if (won) state.Won++;
        else state.Lost++;

        if (state.CurrentBalance > state.PeakBalance)
            state.PeakBalance = state.CurrentBalance;

        var drawdown = state.PeakBalance - state.CurrentBalance;
        if (drawdown > state.MaxDrawdown)
        {
            state.MaxDrawdown = drawdown;
            state.MaxDrawdownPercent = state.PeakBalance > 0
                ? drawdown / state.PeakBalance
                : 0;
        }

        state.LastUpdated = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Result applied: {Outcome} | Profit: {Profit:F2} | Balance: {Balance:F2}",
            won ? "WON" : "LOST", profit, state.CurrentBalance);

        return await GetStateAsync(ct);
    }

    public async Task<BankrollConfig> GetConfigAsync(CancellationToken ct = default)
    {
        var entity = await GetConfigEntityAsync(ct);
        return MapToConfig(entity);
    }

    public async Task UpdateConfigAsync(
        BankrollConfig config,
        CancellationToken ct = default)
    {
        var entity = await _db.BankrollConfigs
            .FirstOrDefaultAsync(c => c.Id == DefaultConfigId, ct);

        if (entity == null)
        {
            entity = new BankrollConfigEntity
            {
                Id = DefaultConfigId,
                InitialBankroll = config.InitialBankroll,
                KellyFraction = config.KellyFraction,
                MaxStakePerPick = config.MaxStakePerPick,
                MaxDailyExposure = config.MaxDailyExposure,
                MaxDrawdownPercent = config.MaxDrawdownPercent,
                MinEdge = config.MinEdge,
                MinSampleSize = config.MinSampleSize,
                StakeMethod = config.StakeMethod,
                AutoUpdateStakes = config.AutoUpdateStakes,
                UpdatedAt = DateTime.UtcNow
            };
            _db.BankrollConfigs.Add(entity);
        }
        else
        {
            entity.InitialBankroll = config.InitialBankroll;
            entity.KellyFraction = config.KellyFraction;
            entity.MaxStakePerPick = config.MaxStakePerPick;
            entity.MaxDailyExposure = config.MaxDailyExposure;
            entity.MaxDrawdownPercent = config.MaxDrawdownPercent;
            entity.MinEdge = config.MinEdge;
            entity.MinSampleSize = config.MinSampleSize;
            entity.StakeMethod = config.StakeMethod;
            entity.AutoUpdateStakes = config.AutoUpdateStakes;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
    }

    private StakeRecommendation CalculateSingleStake(
        DailyPickEntityDto pick,
        BankrollConfigEntity config,
        double currentBalance,
        double todayExposure)
    {
        var edge = pick.Edge;
        var hitRate = pick.HitRate;
        var odds = pick.Odds;
        var sampleSize = pick.SampleSize;

        if (odds <= 1 || hitRate <= 0)
        {
            return CreateRecommendation(
                pick, 0, "Flat", "Invalid odds or hit rate");
        }

        if (edge < config.MinEdge)
        {
            return CreateRecommendation(
                pick, 0, config.StakeMethod,
                $"Edge {edge:F4} below minimum {config.MinEdge:F4}");
        }

        if (sampleSize < config.MinSampleSize)
        {
            return CreateRecommendation(
                pick, 0, config.StakeMethod,
                $"Sample size {sampleSize} below minimum {config.MinSampleSize}");
        }

        double stake;
        var method = config.StakeMethod;

        if (method == "Kelly")
        {
            stake = CalculateKellyStake(hitRate, odds, config.KellyFraction);
        }
        else if (method == "FractionalKelly")
        {
            stake = CalculateKellyStake(hitRate, odds, config.KellyFraction);
        }
        else
        {
            stake = CalculateFlatStake(config.MaxStakePerPick);
            method = "Flat";
        }

        var exceedsMax = stake > config.MaxStakePerPick;
        if (exceedsMax)
            stake = config.MaxStakePerPick;

        var exceedsDaily = todayExposure + stake > config.MaxDailyExposure;
        if (exceedsDaily)
        {
            var remaining = Math.Max(0, config.MaxDailyExposure - todayExposure);
            stake = Math.Min(stake, remaining);
        }

        stake = Math.Max(0, Math.Round(stake, 2));

        var reason = exceedsMax
            ? $"Capped at max stake {config.MaxStakePerPick}"
            : exceedsDaily
                ? $"Capped at daily limit (remaining: {config.MaxDailyExposure - todayExposure:F2})"
                : $"Kelly: {CalculateKellyStake(hitRate, odds, 1.0):F4} * {config.KellyFraction}";

        return new StakeRecommendation(
            pick.Id,
            pick.Team,
            pick.Market,
            odds,
            edge,
            hitRate,
            sampleSize,
            pick.Classification,
            method,
            CalculateKellyStake(hitRate, odds, 1.0),
            stake,
            exceedsMax,
            exceedsDaily,
            reason);
    }

    private static double CalculateKellyStake(
        double hitRate, double odds, double fraction)
    {
        if (odds <= 1 || hitRate <= 0)
            return 0;

        var b = odds - 1;
        var p = Math.Min(hitRate, 0.95);
        var q = 1 - p;

        var kelly = (b * p - q) / b;
        return Math.Max(0, kelly * fraction);
    }

    private static double CalculateFlatStake(double maxStake)
    {
        return maxStake;
    }

    private async Task<double> GetTodayExposureAsync(CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        return await _db.DailyPicks
            .Where(p => p.Date >= today && p.Date < tomorrow && p.Status != "Voided")
            .SumAsync(p => p.StakeUnits, ct);
    }

    private async Task<BankrollConfigEntity> GetConfigEntityAsync(CancellationToken ct)
    {
        var entity = await _db.BankrollConfigs
            .FirstOrDefaultAsync(c => c.Id == DefaultConfigId, ct);

        if (entity == null)
        {
            entity = new BankrollConfigEntity
            {
                Id = DefaultConfigId,
                InitialBankroll = 100,
                KellyFraction = 0.25,
                MaxStakePerPick = 5,
                MaxDailyExposure = 20,
                MaxDrawdownPercent = 0.30,
                MinEdge = 0.02,
                MinSampleSize = 5,
                StakeMethod = "Kelly",
                AutoUpdateStakes = true,
                UpdatedAt = DateTime.UtcNow
            };
            _db.BankrollConfigs.Add(entity);
            await _db.SaveChangesAsync(ct);
        }

        return entity;
    }

    private static StakeRecommendation CreateRecommendation(
        DailyPickEntityDto pick, double stake, string method, string reason)
    {
        return new StakeRecommendation(
            pick.Id,
            pick.Team,
            pick.Market,
            pick.Odds,
            pick.Edge,
            pick.HitRate,
            pick.SampleSize,
            pick.Classification,
            method,
            0,
            stake,
            false,
            false,
            reason);
    }

    private static BankrollConfig MapToConfig(BankrollConfigEntity entity)
    {
        return new BankrollConfig(
            entity.InitialBankroll,
            entity.KellyFraction,
            entity.MaxStakePerPick,
            entity.MaxDailyExposure,
            entity.MaxDrawdownPercent,
            entity.MinEdge,
            entity.MinSampleSize,
            entity.StakeMethod,
            entity.AutoUpdateStakes);
    }
}
