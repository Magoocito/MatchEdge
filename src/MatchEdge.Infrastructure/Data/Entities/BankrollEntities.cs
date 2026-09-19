namespace MatchEdge.Infrastructure.Data.Entities;

public class BankrollStateEntity
{
    public int Id { get; set; }
    public double InitialBankroll { get; set; }
    public double CurrentBalance { get; set; }
    public double PeakBalance { get; set; }
    public double MaxDrawdown { get; set; }
    public double MaxDrawdownPercent { get; set; }
    public double TotalStaked { get; set; }
    public double TotalProfit { get; set; }
    public int TotalPicks { get; set; }
    public int Won { get; set; }
    public int Lost { get; set; }
    public int Pending { get; set; }
    public DateTime LastUpdated { get; set; }

    public string ConfigJson { get; set; } = "{}";
}

public class BankrollConfigEntity
{
    public int Id { get; set; }
    public double InitialBankroll { get; set; }
    public double KellyFraction { get; set; } = 0.25;
    public double MaxStakePerPick { get; set; } = 5.0;
    public double MaxDailyExposure { get; set; } = 20.0;
    public double MaxDrawdownPercent { get; set; } = 0.30;
    public double MinEdge { get; set; } = 0.02;
    public double MinSampleSize { get; set; } = 5;
    public string StakeMethod { get; set; } = "Kelly";
    public bool AutoUpdateStakes { get; set; } = true;
    public DateTime UpdatedAt { get; set; }
}
