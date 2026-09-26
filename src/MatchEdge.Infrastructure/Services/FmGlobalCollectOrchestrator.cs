using System.Diagnostics;

namespace MatchEdge.Infrastructure.Services;

// P10 (item 5 de P2): POST /api/fm/collect ejecuta el collect G2 por equipo
// para N equipos. Vive en Infrastructure (y no en el controller de la Api)
// para que MatchEdge.InfrastructureTests - que solo referencia este proyecto -
// pueda cubrir validacion, aislamiento de errores y orden sin levantar un web
// host; el controller aporta el fetch/parse/upsert reales como delegates, asi
// que sigue habiendo UNA sola implementacion de esa logica (sin copias).
public sealed record FmGlobalCollectBatch(object? Response, int Fetches, int Errors);

public sealed record FmGlobalTeamOutcome(
    long TeamApid, object? Collect, object? Positions, string? Error);

public sealed record FmGlobalCollectOutcome(
    IReadOnlyList<FmGlobalTeamOutcome> Teams,
    int Fetches,
    int PositionFetches,
    int Errors,
    long ElapsedMs);

public static class FmGlobalCollectOrchestrator
{
    // Mismo conjunto que FmTeamController.AllowedLocations.
    public static readonly string[] AllowedLocations = { "home", "away" };

    // position-stats solo acepta period en {5,10,15,20} (whitelist P8 J3).
    private static readonly int[] PositionsPeriods = { 5, 10, 15, 20 };

    // Validacion del body: unica fuente de 400 del endpoint global.
    // (El collect por equipo conserva su comportamiento heredado: period <= 0
    // se normaliza a 15 en vez de rechazarse.)
    public static string? Validate(
        long[]? teams, int period, string[]? locations, bool includePositions)
    {
        if (teams is not { Length: > 0 })
            return "teams is required and must not be empty";
        foreach (var team in teams)
            if (team <= 0)
                return $"invalid team apid: {team}";

        if (period <= 0)
            return "period must be greater than 0";

        if (locations is not { Length: > 0 })
            return "locations is required (home, away)";
        var invalid = locations
            .Where(l => Array.IndexOf(AllowedLocations, l) < 0)
            .ToArray();
        if (invalid.Length > 0)
            return $"invalid location(s): {string.Join(",", invalid)}";

        if (includePositions && Array.IndexOf(PositionsPeriods, period) < 0)
            return $"period {period} is not accepted by position-stats " +
                   $"(allowed: {string.Join(", ", PositionsPeriods)})";

        return null;
    }

    // Secuencial a proposito: un equipo entero antes de empezar el siguiente,
    // sin paralelismo ni throttle propios (los 2-4 s entre fetches viven en el
    // loop del collect por equipo). Un equipo que lanza no aborta a los demas:
    // su entrada lleva el error y las demas siguen.
    public static async Task<FmGlobalCollectOutcome> RunAsync(
        IReadOnlyList<long> teams,
        int period,
        bool includePositions,
        Func<long, CancellationToken, Task<FmGlobalCollectBatch>> collectTeam,
        Func<long, CancellationToken, Task<FmGlobalCollectBatch>>? collectPositions,
        CancellationToken ct)
    {
        var entries = new List<FmGlobalTeamOutcome>(teams.Count);
        var fetches = 0;
        var positionFetches = 0;
        var errors = 0;
        var sw = Stopwatch.StartNew();

        foreach (var team in teams)
        {
            ct.ThrowIfCancellationRequested();

            FmGlobalCollectBatch? collect = null;
            string? error = null;
            try
            {
                collect = await collectTeam(team, ct);
                fetches += collect.Fetches;
                errors += collect.Errors;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                errors++;
            }

            object? positions = null;
            // B3: positions despues del teams/table del MISMO equipo (necesita
            // fm_team_matches/fm_player_matches poblados); se salta si el
            // collect del equipo lanzo.
            if (includePositions && collectPositions is not null && error is null)
            {
                try
                {
                    var pos = await collectPositions(team, ct);
                    positions = pos.Response;
                    positionFetches += pos.Fetches;
                    errors += pos.Errors;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    positions = new { teamApid = team, error = ex.Message };
                    errors++;
                }
            }

            entries.Add(new FmGlobalTeamOutcome(team, collect?.Response, positions, error));
        }

        sw.Stop();
        return new FmGlobalCollectOutcome(
            entries, fetches, positionFetches, errors, sw.ElapsedMilliseconds);
    }
}
