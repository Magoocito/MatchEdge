using MatchEdge.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchEdge.Infrastructure.Data;

public class MatchEdgeDbContext : DbContext
{
    public DbSet<HistoricalOddsEntity> HistoricalOdds => Set<HistoricalOddsEntity>();
    public DbSet<TeamMappingEntity> TeamMappings => Set<TeamMappingEntity>();
    public DbSet<MatchMappingEntity> MatchMappings => Set<MatchMappingEntity>();

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
    }
}
