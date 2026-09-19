namespace MatchEdge.Application.Services;

public interface IBankrollManager
{
    Task<BankrollState> GetStateAsync(CancellationToken ct = default);

    Task<BankrollState> InitializeAsync(
        double initialBankroll,
        BankrollConfig? config = null,
        CancellationToken ct = default);

    Task<List<StakeRecommendation>> CalculateStakesAsync(
        List<DailyPickEntityDto> picks,
        CancellationToken ct = default);

    Task<BankrollState> ApplyResultAsync(
        int pickId,
        bool won,
        double odds,
        double stakeUnits,
        CancellationToken ct = default);

    Task<BankrollConfig> GetConfigAsync(CancellationToken ct = default);

    Task UpdateConfigAsync(
        BankrollConfig config,
        CancellationToken ct = default);
}

public record BankrollConfig(
    double InitialBankroll = 100.0,
    double KellyFraction = 0.25,
    double MaxStakePerPick = 5.0,
    double MaxDailyExposure = 20.0,
    double MaxDrawdownPercent = 0.30,
    double MinEdge = 0.02,
    double MinSampleSize = 5,
    string StakeMethod = "Kelly",
    bool AutoUpdateStakes = true
);

public record BankrollState(
    double InitialBankroll,
    double CurrentBalance,
    double PeakBalance,
    double MaxDrawdown,
    double MaxDrawdownPercent,
    double TotalStaked,
    double TotalProfit,
    double ROI,
    double Yield,
    int TotalPicks,
    int Won,
    int Lost,
    int Pending,
    double WinRate,
    BankrollConfig Config,
    DateTime LastUpdated = default
);

public record StakeRecommendation(
    int PickId,
    string Team,
    string Market,
    double Odds,
    double Edge,
    double HitRate,
    int SampleSize,
    string Classification,
    string Method,
    double KellyStake,
    double RecommendedStake,
    bool ExceedsMaxStake,
    bool ExceedsDailyLimit,
    string Reason
);
