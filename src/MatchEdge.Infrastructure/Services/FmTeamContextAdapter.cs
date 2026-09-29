using MatchEdge.Application.UseCases.Context;
using MatchEdge.Domain.Models;

namespace MatchEdge.Infrastructure.Services;

// P13: adaptador FM -> TeamContextStatistics. EnhancedLambdaCalculator consume
// TeamContextStatistics/SofaScore a través de TeamContextStatisticsService; las
// filas de fm_team_matches tienen la misma semántica (localía + goles
// marcados/ recibidos), así que se adapta aquí sin tocar su lógica interna.
public static class FmTeamContextAdapter
{
    // Una fila de fm_team_matches se guarda desde la perspectiva del equipo:
    // con location=home los goles del equipo son hgoals y los recibidos agoals;
    // con location=away se invierten.
    public static TeamContextStatistics ToContext(
        IReadOnlyList<FmTeamMatchRow> rows, DateTime? asOfUtc = null)
    {
        var home = 0;
        var away = 0;
        var homeScored = 0;
        var homeConceded = 0;
        var awayScored = 0;
        var awayConceded = 0;
        var skipped = 0;

        foreach (var row in rows)
        {
            if (asOfUtc is DateTime cut && row.TsUtc >= cut) continue;
            if (row.HGoals is null || row.AGoals is null)
            {
                skipped++;
                continue;
            }

            var isHome = string.Equals(row.Location, "home", StringComparison.OrdinalIgnoreCase);
            var scored = isHome ? row.HGoals.Value : row.AGoals.Value;
            var conceded = isHome ? row.AGoals.Value : row.HGoals.Value;

            if (isHome)
            {
                home++;
                homeScored += scored;
                homeConceded += conceded;
            }
            else
            {
                away++;
                awayScored += scored;
                awayConceded += conceded;
            }
        }

        return new TeamContextStatistics(
            AttackHome: home > 0 ? (double)homeScored / home : 0d,
            DefenseHome: home > 0 ? (double)homeConceded / home : 0d,
            AttackAway: away > 0 ? (double)awayScored / away : 0d,
            DefenseAway: away > 0 ? (double)awayConceded / away : 0d,
            HomeMatchesCount: home,
            AwayMatchesCount: away,
            SkippedMatchesCount: skipped);
    }

    // Totales de temporada (sin split) para el fallback del propio
    // EnhancedLambdaCalculator vía IHistoricalTeamStatisticsProvider.
    public static TeamStatistics ToTeamStatistics(
        IReadOnlyList<FmTeamMatchRow> rows, DateTime? asOfUtc = null)
    {
        var scored = 0;
        var conceded = 0;
        var matches = 0;
        var skipped = 0;

        foreach (var row in rows)
        {
            if (asOfUtc is DateTime cut && row.TsUtc >= cut) continue;
            if (row.HGoals is null || row.AGoals is null)
            {
                skipped++;
                continue;
            }

            var isHome = string.Equals(row.Location, "home", StringComparison.OrdinalIgnoreCase);
            scored += isHome ? row.HGoals.Value : row.AGoals.Value;
            conceded += isHome ? row.AGoals.Value : row.HGoals.Value;
            matches++;
        }

        return new TeamStatistics
        {
            Matches = matches,
            GoalsScored = scored,
            GoalsConceded = conceded
        };
    }
}
