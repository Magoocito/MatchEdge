using MatchEdge.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchEdge.Infrastructure.Data;

public class MatchEdgeDbContext : DbContext
{
    public DbSet<HistoricalOddsEntity> HistoricalOdds => Set<HistoricalOddsEntity>();
    public DbSet<TeamMappingEntity> TeamMappings => Set<TeamMappingEntity>();
    public DbSet<MatchMappingEntity> MatchMappings => Set<MatchMappingEntity>();
    public DbSet<TrendResultEntity> TrendResults => Set<TrendResultEntity>();
    public DbSet<DailyPickEntity> DailyPicks => Set<DailyPickEntity>();
    public DbSet<PickOutcomeEntity> PickOutcomes => Set<PickOutcomeEntity>();
    public DbSet<BankrollStateEntity> BankrollStates => Set<BankrollStateEntity>();
    public DbSet<BankrollConfigEntity> BankrollConfigs => Set<BankrollConfigEntity>();

    public MatchEdgeDbContext(DbContextOptions<MatchEdgeDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HistoricalOddsEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.Source, x.SourceMatchId }).IsUnique();
            e.HasIndex(x => new { x.MatchDate, x.HomeTeamId, x.AwayTeamId });
            e.HasIndex(x => x.SofaScoreEventId);
            e.Property(x => x.HomeWinOdds).HasPrecision(10, 4);
            e.Property(x => x.DrawOdds).HasPrecision(10, 4);
            e.Property(x => x.AwayWinOdds).HasPrecision(10, 4);
        });

        modelBuilder.Entity<TeamMappingEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.Source, x.SourceTeamId }).IsUnique();
            e.HasIndex(x => x.SofaScoreTeamId);
        });

        modelBuilder.Entity<MatchMappingEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.Source, x.SourceMatchId }).IsUnique();
            e.HasIndex(x => x.SofaScoreEventId);
        });

        modelBuilder.Entity<TrendResultEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.ScrapedAt);
            e.HasIndex(x => x.FixtureApid);
            e.HasIndex(x => new { x.FixtureApid, x.Team, x.Market });
            e.HasIndex(x => x.Classification);
            e.HasIndex(x => x.IsSelected);
            e.Property(x => x.Odds).HasPrecision(10, 4);
            e.Property(x => x.Score).HasPrecision(10, 4);
            e.Property(x => x.Edge).HasPrecision(10, 4);
        });

        modelBuilder.Entity<DailyPickEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Date);
            e.HasIndex(x => new { x.Date, x.Team, x.Market }).IsUnique();
            e.HasIndex(x => x.TrendResultId);
            e.HasIndex(x => x.Status);
            e.Property(x => x.Score).HasPrecision(10, 4);
            e.Property(x => x.Edge).HasPrecision(10, 4);
            e.Property(x => x.Odds).HasPrecision(10, 4);
            e.Property(x => x.StakeUnits).HasPrecision(10, 4);
        });

        modelBuilder.Entity<PickOutcomeEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.DailyPickId);
            e.HasIndex(x => x.Won);
            e.HasIndex(x => x.MatchDate);
            e.Property(x => x.Odds).HasPrecision(10, 4);
            e.Property(x => x.Profit).HasPrecision(10, 4);
            e.Property(x => x.ProfitUnits).HasPrecision(10, 4);
            e.Property(x => x.BalanceAfter).HasPrecision(10, 4);
        });

        modelBuilder.Entity<BankrollStateEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.InitialBankroll).HasPrecision(10, 4);
            e.Property(x => x.CurrentBalance).HasPrecision(10, 4);
            e.Property(x => x.PeakBalance).HasPrecision(10, 4);
            e.Property(x => x.MaxDrawdown).HasPrecision(10, 4);
            e.Property(x => x.MaxDrawdownPercent).HasPrecision(10, 4);
            e.Property(x => x.TotalStaked).HasPrecision(10, 4);
            e.Property(x => x.TotalProfit).HasPrecision(10, 4);
        });

        modelBuilder.Entity<BankrollConfigEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.InitialBankroll).HasPrecision(10, 4);
            e.Property(x => x.KellyFraction).HasPrecision(10, 4);
            e.Property(x => x.MaxStakePerPick).HasPrecision(10, 4);
            e.Property(x => x.MaxDailyExposure).HasPrecision(10, 4);
            e.Property(x => x.MaxDrawdownPercent).HasPrecision(10, 4);
            e.Property(x => x.MinEdge).HasPrecision(10, 4);
        });
    }
}
