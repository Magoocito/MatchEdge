using System.Globalization;
using MatchEdge.Application.Configuration;
using MatchEdge.Application.UseCases.Lambda;
using MatchEdge.Application.UseCases.Probability;
using Microsoft.Extensions.Options;

namespace MatchEdge.Infrastructure.Services;

// P13: fila de la tabla Proxy-EV. OddsFm nulo (con Status) significa que el
// fixture no tiene línea en fm_market_odds: no se inventa cuota ni se toma otra
// casa, y Delta% queda sin calcular.
// P13 jugadores: Team/Player solo se rellenan en las filas de jugador (las de
// equipo ya nombran al sujeto dentro de la etiqueta del mercado).
public sealed record P13MarketRow(
    string Market,
    double? PModel,
    double? FairOdds,
    string? OddsFm,
    double? DeltaPct,
    string? Status,
    string? Team = null,
    string? Player = null);

// P13: sección por partido. HasData=false -> el fixture no tiene snapshot y no
// se pudo construir ningún modelo. PlayerRows es la tabla de mercados de
// jugador (vacía en los fixtures post-kickoff y en los sin señales).
// IsPostMatch=true -> detección automática de captura post-kickoff: la sección
// es solo registro histórico, sin Delta%, sin candidatos y sin jugadores.
public sealed record P13FixtureSection(
    string FixtureId,
    string Home,
    string Away,
    string? KickoffUtc,
    bool HasData,
    string Volume,
    double ThresholdPct,
    string Confidence,
    double? LambdaHome,
    double? LambdaAway,
    string? LambdaNote,
    IReadOnlyList<P13MarketRow> Rows,
    IReadOnlyList<P13MarketRow> PlayerRows,
    IReadOnlyList<P13MarketRow> Candidates,
    IReadOnlyList<string> Notes,
    bool IsPostMatch = false);

public sealed record P13Report(
    string GeneratedAtUtc,
    IReadOnlyList<P13FixtureSection> Fixtures,
    IReadOnlyList<string> Notes);

// P13: el orden de los 8 partidos y su volumen vienen del brief; el builder no
// los infiere de la base de datos. No hay flag manual de post-partido: ese
// estado se decide solo comparando MIN(source_timestamp_utc) de fm_snapshot
// con el kickoff del fixture (ver IsPostKickoff) y, si el kickoff no se puede
// leer del overview, la seccion se cierra igualmente (fail-closed).
public sealed record P13FixtureSpec(
    string FixtureId,
    string Home,
    string Away,
    string Volume);

// Lados del partido con sus filas FM (para las lambdas de mercados de conteo).
public sealed record FixtureSides(
    string? HomeName,
    string? AwayName,
    long? HomeApid,
    long? AwayApid,
    IReadOnlyList<FmTeamMatchRow> HomeRows,
    IReadOnlyList<FmTeamMatchRow> AwayRows);

// P13: builder NUEVO para Proxy-EV. No modifica FmConfluenceReportBuilder ni el
// pipeline P7-P11: toma su salida (FmReport) + fm_market_odds y calcula
// P_modelo / Cuota Justa / Delta% aparte, con su propio contrato de wording.
public static class P13ProxyEvReportBuilder
{
    // Brief P13: 1.15 de trabajo para UEFA (MatchModelOptions.HomeAdvantageFactor
    // = 1.35 está calibrado para Liga 1 Perú, no se reutiliza tal cual).
    public const double HomeAdvantageFactor = 1.15;

    public const double DixonColesRho = DixonColes.DefaultRho;

    // Matriz de marcadores 0-0 a 8-8 (el brief pide de 0-0 a 4-4; 8 cubre las
    // líneas de goles que existen en fm_market_odds sin truncar la cola).
    public const int MaxGoals = 8;

    // Brief P13 (extensión de jugadores): el ranking de candidatos por partido
    // es ÚNICO para equipo + jugador y sube de 2 a 10 filas combinadas.
    public const int CandidatesCap = 10;
    public const int StatWindow = 10;

    // Serie mínima del jugador para calcular Delta%: por debajo no hay lambda.
    public const int PlayerMinSample = 5;

    public const string NoOddsStatus = "SIN CUOTA FM - DELTA NO CALCULABLE";
    public const string NoSignalText = "SIN SEÑAL DE VALOR - SOLO OBSERVACIÓN";
    public const string NoDataText =
        "SIN DATA FM - snapshot no ejecutado (presupuesto de navegacion)";
    public const string MuestraInsufficientStatus = "MUESTRA INSUFICIENTE";

    // P13 GAP1: modo en que queda una seccion capturada despues de su kickoff.
    public const string PostMatchText =
        "POST-PARTIDO - SOLO REGISTRO HISTORICO, NO ACCIONABLE";
    public const string HomeAdvNote =
        "HomeAdv sin calibrar para UEFA - valor provisional 1.15";
    public const string NoModelNote = "mercado sin modelo propio";

    // Nota textual obligatoria al final de cada sección de jugadores.
    public const string PlayerModelNote =
        "Modelo de jugador sin ajuste por minutos esperados ni rival. " +
        "Delta% refleja solo la media histórica del jugador.";
    public const string PlayerPostCloseNote =
        "Mercados de jugadores no calculados: snapshot capturado post-cierre " +
        "(solo registro historico).";
    public const string PlayerNoSignalNote =
        "Sin señales de jugador con línea FM para este partido.";

    public const string OddsHeader = "Cuota FM (bookmaker)";

    public static readonly IReadOnlyList<string> GlobalNotes = new[]
    {
        "Delta% = (P_modelo x Cuota FM) - 1, en porcentaje. Cuota FM = mejor cuota " +
        "disponible entre las casas del fixture en fm_market_odds (casa indicada en la " +
        "columna); sin fila, la fila queda " + NoOddsStatus + ".",
        "Mercados de goles (1X2, doble oportunidad, BTTS, Asian Handicap y " +
        "over/under de goles): matriz de Poisson bivariado 0-8 con correccion " +
        "Dixon-Coles rho=0.1 sobre 0-0/1-0/0-1/1-1 y masa normalizada.",
        "Resto de mercados de conteo (corners, tarjetas, tiros, faltas, duelos): " +
        "Poisson con lambda = media FM de la ventana last10 (si no hay last10, la " +
        "media de toda la serie) del reporte P11. Los total_* se arman con la media " +
        "del local en casa + la del visitante fuera, porque el reporte P11 marca esa " +
        "base como no reproducible.",
        HomeAdvNote + ".",
        "Poisson se usa como estructura, no como verdad: no corrige tarjetas rojas, " +
        "rotaciones ni incentivos.",
        "Pinnacle no existe en este P13: toda comparacion es contra Cuota FM real; " +
        "no se usa cuota de cierre estimada ni se rellena con otra casa.",
        "Sin datos de lesiones, clima ni motivacion: no se asume nada sin fuente " +
        "UEFA.com oficial.",
        "Los filtros live F1/F2 son especificacion para el momento del partido; " +
        "este documento no contiene datos en vivo.",
        "El emparejamiento de cuotas de equipo es mercado + linea + lado sobre " +
        "fm_market_odds; la columna subject_name se ignora porque en los mercados " +
        "home_/away_ etiqueta lineas con el nombre de los dos equipos. En los " +
        "mercados de jugador es al reves: solo vale la fila exacta por jugador + " +
        "mercado + linea + lado, sin reutilizar la cuota del equipo.",
        "Mercados de jugadores: Poisson simple con lambda = media de la ventana " +
        "last10 del historial del jugador en fm_signal; si esa ventana no llega a " +
        PlayerMinSample + " observaciones se usa la media de toda la serie y, si " +
        "tampoco llega, la fila queda " + MuestraInsufficientStatus + " y queda " +
        "fuera del ranking.",
        "El ranking de candidatos es unico por partido (equipo + jugador) con tope " +
        "de " + CandidatesCap + " filas y el mismo umbral de Delta% por volumen; " +
        "los jugadores no aplican el filtro de P_modelo de los mercados de equipo.",
        "Los mercados home_*/away_* usan la media de la sede correspondiente (venue) " +
        "de fm_team_matches; la ventana last10 own_windows del reporte P11 mezcla " +
        "localías, asi que queda como respaldo.",
        "Esas lambdas de conteo son historicas y SIN ajuste por rival: no incorporan " +
        "la fortaleza defensiva del adversario ni la dificultad del cruce, asi que un " +
        "Delta% grande en mercados de conteo refleja sobre todo ese hueco estructural " +
        "del modelo (los goles si van con ataque x defensa via HomeAwaySplit)."
    };

    public static async Task<P13Report> BuildAsync(
        IReadOnlyList<P13FixtureSpec> specs,
        FmSnapshotStore store,
        FmOutcomeStore outcomeStore,
        CancellationToken ct = default)
    {
        var sections = new List<P13FixtureSection>(specs.Count);
        foreach (var spec in specs)
        {
            ct.ThrowIfCancellationRequested();

            // P13 GAP1: kickoff del fixture desde el overview persistido + timing
            // de captura. La deteccion de post-partido es automatica: compara la
            // primera captura con el kickoff y no depende de ningun flag manual.
            var timing = await store.GetSnapshotTimingAsync(spec.FixtureId, ct);
            var kickoff = await ReadFixtureKickoffUtcAsync(timing, ct);

            var input = await FmConfluenceReportLoader.LoadAsync(
                store, outcomeStore, spec.FixtureId, ct);
            if (kickoff is DateTime ko && IsPostKickoff(ko, timing.FirstSnapshotUtc))
            {
                sections.Add(BuildPostMatch(spec, input, timing, ko));
                continue;
            }

            // P13 GAP1 fail-closed: sin kickoff legible (fixture sin tab
            // 'overview' persistido o sin startDate en el JSON-LD) no se puede
            // descartar que la primera captura sea posterior al partido, asi que
            // con datos de captura la seccion queda en modo registro (sin
            // modelo, sin candidatos y sin jugadores) en vez de fallar abierto.
            // Es el caso del matchday siguiente: fixtures nuevos capturados
            // antes de tener su overview.
            if (kickoff is null && input is not null)
            {
                sections.Add(BuildPostMatch(spec, input, timing, null));
                continue;
            }

            sections.Add(input is null
                ? BuildWithoutData(spec)
                : await BuildSectionAsync(
                    spec, FmConfluenceReportBuilder.Build(spec.FixtureId, input),
                    input, store, kickoff, ct));
        }

        return new P13Report(
            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm 'UTC'", CultureInfo.InvariantCulture),
            sections,
            GlobalNotes);
    }

    private static P13FixtureSection BuildWithoutData(P13FixtureSpec spec) =>
        new(
            spec.FixtureId, spec.Home, spec.Away, null, false,
            spec.Volume, ThresholdFor(spec.Volume), ConfidenceFor(spec.Volume),
            null, null, null,
            Array.Empty<P13MarketRow>(), Array.Empty<P13MarketRow>(),
            Array.Empty<P13MarketRow>(),
            new[] { NoDataText });

    // P13 GAP1: modo post-partido automatico. La primera captura del fixture es
    // posterior a su kickoff (o no se puede descartar, ver el fail-closed de
    // BuildAsync), asi que el informe queda como registro historico: sin Delta%,
    // sin candidatos y sin mercados de jugador, con la evidencia de captura
    // (ventana, snapshots, senales y filas de cuota) en las notas.
    private static P13FixtureSection BuildPostMatch(
        P13FixtureSpec spec,
        FmReportInput? input,
        FmSnapshotTiming timing,
        DateTime? kickoff)
    {
        var notes = new List<string> { PostMatchText };

        if (kickoff is DateTime k)
            notes.Add($"Captura posterior al kickoff {FormatKickoff(k)} (primera captura " +
                      $"{FormatKickoff(timing.FirstSnapshotUtc)}): sin Delta%, sin candidatos y " +
                      "sin mercados de jugador; solo registro historico.");
        else
            notes.Add("Sin snapshot overview persistido no se puede determinar el kickoff: " +
                      "la seccion queda en modo fail-closed, sin Delta%, sin candidatos y sin " +
                      "mercados de jugador, hasta poder comparar la captura con el kickoff.");

        if (timing.FirstSnapshotUtc is DateTime first && timing.LastSnapshotUtc is DateTime last)
            notes.Add($"Captura: {FormatKickoff(first)} - {FormatKickoff(last)} " +
                      $"({timing.SnapshotCount} snapshots) · Senales: " +
                      $"{input?.Signals.Count ?? 0} · filas fm_market_odds: " +
                      $"{input?.Odds.Count ?? 0}.");

        notes.Add(PlayerPostCloseNote);

        return new P13FixtureSection(
            spec.FixtureId, spec.Home, spec.Away, FormatKickoff(kickoff), true,
            spec.Volume, ThresholdFor(spec.Volume), ConfidenceFor(spec.Volume),
            null, null, null,
            Array.Empty<P13MarketRow>(), Array.Empty<P13MarketRow>(),
            Array.Empty<P13MarketRow>(), notes, IsPostMatch: true);
    }

    // P13 GAP1: kickoff del fixture leido del snapshot overview persistido
    // (JSON-LD SportsEvent.startDate de la pagina del partido). Sin overview no
    // hay kickoff: BuildAsync no puede comparar la captura con el partido y
    // cierra la seccion (fail-closed) en vez de calcular candidatos a ciegas.
    private static readonly System.Text.RegularExpressions.Regex KickoffPattern = new(
        "\"startDate\"\\s*:\\s*\"(?<ts>\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}" +
        "(?:\\.\\d+)?Z)\"",
        System.Text.RegularExpressions.RegexOptions.CultureInvariant);

    public static DateTime? ParseKickoffUtc(string? overviewHtml) =>
        overviewHtml is null
            ? null
            : ParseUtc(KickoffPattern.Match(overviewHtml).Groups["ts"].Value);

    // Comparacion del GAP1: primera captura (MIN source_timestamp_utc) > kickoff.
    public static bool IsPostKickoff(DateTime? kickoffUtc, DateTime? firstSnapshotUtc) =>
        kickoffUtc is DateTime kickoff &&
        firstSnapshotUtc is DateTime first &&
        first > kickoff;

    private static async Task<DateTime?> ReadFixtureKickoffUtcAsync(
        FmSnapshotTiming timing, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(timing.OverviewRawPath)) return null;
        try
        {
            if (!File.Exists(timing.OverviewRawPath)) return null;
            return ParseKickoffUtc(
                await File.ReadAllTextAsync(timing.OverviewRawPath, ct));
        }
        catch (IOException) { return null; }
        catch (UnauthorizedAccessException) { return null; }
    }

    private static string? FormatKickoff(DateTime? value) =>
        value is DateTime dt
            ? dt.ToString("yyyy-MM-ddTHH:mm:ss'Z'", CultureInfo.InvariantCulture)
            : null;


    // ---------------------------------------------------------------------------
    // Modelo por partido
    // ---------------------------------------------------------------------------

    private static async Task<P13FixtureSection> BuildSectionAsync(
        P13FixtureSpec spec,
        FmReport report,
        FmReportInput input,
        FmSnapshotStore store,
        DateTime? fixtureKickoffUtc,
        CancellationToken ct)
    {
        var notes = new List<string>();
        var kickoff = ParseUtc(report.Fixture.KickoffUtc);
        // Kickoff del fixture para mostrar (overview persistido si existe; si no,
        // el que trae el reporte P11 desde las filas de fm_team_matches).
        var kickoffDisplay = fixtureKickoffUtc ?? kickoff;

        var homeApid = input.Sides?.HomeApid
            ?? input.FixtureRows.FirstOrDefault(r =>
                string.Equals(r.Location, "home", StringComparison.OrdinalIgnoreCase))?.TeamApid;
        var awayApid = input.Sides?.AwayApid
            ?? input.FixtureRows.FirstOrDefault(r =>
                string.Equals(r.Location, "away", StringComparison.OrdinalIgnoreCase))?.TeamApid;

        var homeRows = RowsFor(input, homeApid);
        var awayRows = RowsFor(input, awayApid);

        var homeContext = FmTeamContextAdapter.ToContext(homeRows, kickoff);
        var awayContext = FmTeamContextAdapter.ToContext(awayRows, kickoff);

        var modelOptions = Options.Create(new MatchModelOptions
        {
            HomeAdvantageFactor = HomeAdvantageFactor
        });
        var lambdaCalculator = new EnhancedLambdaCalculator(
            new MatchLambdaCalculator(modelOptions),
            new FmHistoricalTeamStatisticsProvider(store),
            modelOptions,
            applyGammaToSplit: true);

        var lambdas = await lambdaCalculator.CalculateAsync(
            homeContext, awayContext,
            (int)(homeApid ?? 0), (int)(awayApid ?? 0),
            tournamentId: 0,
            asOfDateTime: kickoff ?? DateTime.UtcNow);

        notes.Add($"lambda_home = {Num(lambdas.LambdaHome)} / lambda_away = " +
                  $"{Num(lambdas.LambdaAway)} (fuente: {lambdas.CalculationMethod}; " +
                  $"HomeAdv = {HomeAdvantageFactor.ToString("0.00", CultureInfo.InvariantCulture)}).");
        notes.Add($"HomeAdv: {HomeAdvNote}.");
        notes.Add("Correccion Dixon-Coles rho = " +
                  $"{DixonColesRho.ToString("0.0", CultureInfo.InvariantCulture)} " +
                  "aplicada a las celdas 0-0, 1-0, 0-1 y 1-1 de la matriz.");
        notes.Add($"Muestras por sede: {report.Fixture.Home ?? "local"} " +
                  $"{homeContext.HomeMatchesCount} en casa / {homeContext.AwayMatchesCount} " +
                  $"fuera; {report.Fixture.Away ?? "visitante"} " +
                  $"{awayContext.HomeMatchesCount} en casa / {awayContext.AwayMatchesCount} fuera.");

        var probs = new GoalProbs(BuildMatrix(lambdas.LambdaHome, lambdas.LambdaAway));
        var sides = new FixtureSides(
            report.Fixture.Home, report.Fixture.Away, homeApid, awayApid,
            homeRows, awayRows);
        var directions = DirectionByMarket(input);

        var rows = new List<P13MarketRow>();
        var skipped = new List<string>();
        var noSide = new List<string>();
        var covered = new HashSet<(string Market, double? Line, string Side)>(
            FixtureKeyComparer.Instance);

        // Los mercados obligatorios del brief van primero: si el reporte P11
        // trae la misma linea, gana la etiqueta legible del brief.
        rows.AddRange(MandatoryRows(probs, sides, input, covered, notes));

        foreach (var market in report.Markets)
        {
            var side = SideOf(market, directions);
            var p = ModelFor(market, probs, sides, directions);
            if (p is null)
            {
                skipped.Add(market.Market + (market.Line is null
                    ? string.Empty
                    : " @ " + Num(market.Line.Value)));
                continue;
            }

            if (side is null)
            {
                noSide.Add(market.Market + (market.Line is null
                    ? string.Empty
                    : " @ " + Num(market.Line.Value)));
                continue;
            }
            var key = (NormalizeKey(market.Market), market.Line, NormalizeSide(side));
            if (!covered.Add(key)) continue;

            rows.Add(ToRow(
                Label(market.Market, market.Line, side),
                p.Value,
                // La etiqueta subject de fm_market_odds es inconsistente entre
                // lineas (misimo mercado con el nombre de ambos equipos), asi que
                // el emparejamiento es mercado + linea + lado.
                BestOdds(input, market.Market, market.Line, NormalizeSide(side),
                    subject: null)));
        }

        if (skipped.Count > 0)
            notes.Add("Fuera de la tabla (" + NoModelNote + "): " +
                      string.Join(", ", skipped.Distinct(StringComparer.Ordinal)) + ".");
        if (noSide.Count > 0)
            notes.Add("Fuera de la tabla (sin lado en la senal FM): " +
                      string.Join(", ", noSide.Distinct(StringComparer.Ordinal)) + ".");

        // P13 jugadores: solo en captura pre-partido. La captura post-kickoff ya
        // salio por el modo automatico de BuildAsync, asi que aqui siempre hay
        // ventana de partido por delante y los mercados de jugador se calculan.
        var playerRows = BuildPlayerRows(spec, input, notes);
        if (playerRows.Count == 0) notes.Add(PlayerNoSignalNote);

        var threshold = ThresholdFor(spec.Volume);
        // Unico ranking por partido: mercados de equipo (con su filtro de
        // P_modelo > 50%) + mercados de jugador, hasta CandidatesCap filas.
        var candidates = rows
            .Where(r => r.OddsFm is not null
                        && r.PModel is double p
                        && p > 0.50
                        && r.DeltaPct is double d
                        && d >= threshold)
            .Concat(playerRows
                .Where(r => r.OddsFm is not null
                            && r.DeltaPct is double d
                            && d >= threshold))
            .OrderByDescending(r => r.DeltaPct)
            .ThenBy(r => r.Player, StringComparer.Ordinal)
            .ThenBy(r => r.Market, StringComparer.Ordinal)
            .Take(CandidatesCap)
            .ToList();

        return new P13FixtureSection(
            spec.FixtureId, spec.Home, spec.Away,
            kickoffDisplay is DateTime k ? FormatKickoff(k) : report.Fixture.KickoffUtc,
            true,
            spec.Volume, threshold, ConfidenceFor(spec.Volume),
            lambdas.LambdaHome, lambdas.LambdaAway, lambdas.CalculationMethod,
            rows, playerRows, candidates, notes);
    }

    // ---------------------------------------------------------------------------
    // Mercados de jugadores (extensión P13 sobre el mismo builder)
    // ---------------------------------------------------------------------------

    // Lambda del jugador: media de la ventana last10 si llega a PlayerMinSample
    // observaciones; si no, media de toda la serie; si tampoco, sin modelo (la
    // fila queda MuestraInsufficientStatus y fuera del ranking). last10 y all
    // salen de la misma serie, asi que el segundo paso solo cubre el caso en que
    // la ventana last10 no sea utilizable.
    public static double? PlayerLambda(IReadOnlyList<FmWindowStats> windows)
    {
        foreach (var key in new[] { "10", "all" })
        {
            var window = windows.FirstOrDefault(w =>
                string.Equals(w.Window, key, StringComparison.Ordinal));
            if (window is null) continue;
            if (window.Status != FmWindowCalculator.StatusOk) continue;
            if (window.N < PlayerMinSample) continue;
            if (window.Mean is double mean) return mean;
        }
        return null;
    }

    private static List<P13MarketRow> BuildPlayerRows(
        P13FixtureSpec spec, FmReportInput input, List<string> notes)
    {
        var result = new List<P13MarketRow>();
        var skipped = new List<string>();

        // Una fila por jugador + mercado + linea (misma deduplicacion del P11).
        var signals = input.Signals
            .Where(s => s.SubjectType is "player" && !string.IsNullOrEmpty(s.Market))
            .GroupBy(s => (s.Market!, s.Line, s.SubjectName))
            .Select(g => g.OrderBy(s => s.Id).First());

        foreach (var s in signals)
        {
            var side = (s.Direction ?? string.Empty).Trim().ToLowerInvariant();
            if (s.Line is null || side.Length == 0)
            {
                skipped.Add($"{s.SubjectName} - {s.Market}");
                continue;
            }

            var label = Label(s.Market, s.Line, side);
            var team = PlayerTeam(spec, s.VenueRole);
            var lambda = PlayerLambda(FmWindowCalculator.Compute(
                "player", s.RecentValuesJson, s.Line, s.Direction));
            if (lambda is null)
            {
                result.Add(new P13MarketRow(label, null, null, null, null,
                    MuestraInsufficientStatus, team, s.SubjectName));
                continue;
            }

            var over = side is not ("under" or "no");
            var p = PoissonSide(lambda.Value, s.Line.Value, over);
            result.Add(ToPlayerRow(
                label, p,
                BestPlayerOdds(input, s.Market!, s.Line, side, s.SubjectName),
                team, s.SubjectName));
        }

        if (skipped.Count > 0)
            notes.Add("Jugadores fuera de la tabla (sin linea o sin lado en la " +
                      "senal FM): " +
                      string.Join(", ", skipped.Distinct(StringComparer.Ordinal)) + ".");

        // Solo se reportan los lados con Delta% positivo; las filas sin cuota o
        // sin muestra se conservan para documentar el porque de la exclusion.
        return result
            .Where(r => r.Status is not null || r.DeltaPct is > 0)
            .OrderByDescending(r => r.DeltaPct)
            .ThenBy(r => r.Player, StringComparer.Ordinal)
            .ThenBy(r => r.Market, StringComparer.Ordinal)
            .ToList();
    }

    private static P13MarketRow ToPlayerRow(
        string label, double p, string? oddsFm, string? team, string player)
    {
        var fair = p > 0 ? 1d / p : (double?)null;
        if (oddsFm is null)
            return new P13MarketRow(label, p, fair, null, null, NoOddsStatus, team, player);

        var odds = ParseOdds(oddsFm);
        if (odds is null || p <= 0)
            return new P13MarketRow(label, p, fair, oddsFm, null, NoOddsStatus, team, player);

        var delta = ((p * odds.Value) - 1d) * 100d;
        return new P13MarketRow(label, p, fair, oddsFm, delta, null, team, player);
    }

    // Equipo del jugador por la sede de la senal FM; sin sede no se inventa.
    private static string? PlayerTeam(P13FixtureSpec spec, string? venueRole)
    {
        if (string.Equals(venueRole, "home", StringComparison.OrdinalIgnoreCase))
            return spec.Home;
        if (string.Equals(venueRole, "away", StringComparison.OrdinalIgnoreCase))
            return spec.Away;
        return null;
    }

    // Mercado -> probabilidad del modelo. null = el mercado no tiene modelo
    // propio y se documenta en la sección (no se rellena con nada inventado).
    private static double? ModelFor(
        FmReportMarket market, GoalProbs probs, FixtureSides sides,
        IReadOnlyDictionary<(string Market, double?), string> directions)
    {
        var name = market.Market?.Trim() ?? string.Empty;
        var line = market.Line;
        // La direccion (over/under, yes/no, most) viene de la senal FM; sin
        // senal se asume el lado "over" solo para evaluar la fila.
        var direction = SignalDirection(market, directions)?.Trim()
            .ToLowerInvariant();
        var under = direction is "under" or "no";
        var over = !under;
        var core = Core(name);

        if (core == "goals")
        {
            if (line is null) return null;
            if (name.StartsWith("home_", StringComparison.Ordinal))
                return probs.HomeGoalsOver(line.Value, over);
            if (name.StartsWith("away_", StringComparison.Ordinal))
                return probs.AwayGoalsOver(line.Value, over);
            return probs.TotalOver(line.Value, over);
        }

        if (name.Equals("btts", StringComparison.Ordinal))
        {
            var yes = probs.BttsYes();
            return under ? 1d - yes : yes;
        }

        if (name.StartsWith("total_", StringComparison.Ordinal))
        {
            if (line is null) return null;
            var lambda = TotalLambda(core, sides);
            if (lambda is null) return null;
            return PoissonSide(lambda.Value, line.Value, over);
        }

        if (line is null) return null;
        // home_*/away_*: la expectativa de este partido es la media de la sede
        // correspondiente. own_windows del reporte P11 mezcla localías, asi que
        // primero se usa venue (fm_team_matches) y solo despues esa ventana.
        var mean = VenueMean(name, market.SubjectRole, core, sides)
            ?? WindowMean(market);
        return mean is null ? null : PoissonSide(mean.Value, line.Value, over);
    }

    private static double? VenueMean(
        string name, string? role, string core, FixtureSides sides)
    {
        if (name.StartsWith("home_", StringComparison.Ordinal))
            return StatMean(sides.HomeRows, core, "home");
        if (name.StartsWith("away_", StringComparison.Ordinal))
            return StatMean(sides.AwayRows, core, "away");
        if (string.Equals(role, "home", StringComparison.OrdinalIgnoreCase))
            return StatMean(sides.HomeRows, core, "home");
        if (string.Equals(role, "away", StringComparison.OrdinalIgnoreCase))
            return StatMean(sides.AwayRows, core, "away");
        return null;
    }

    // ---------------------------------------------------------------------------
    // Mercados obligatorios (lista del prompt anterior): se evalúan aunque
    // fm_market_odds no tenga la línea - entonces quedan SIN CUOTA FM.
    // ---------------------------------------------------------------------------

    private static IEnumerable<P13MarketRow> MandatoryRows(
        GoalProbs probs,
        FixtureSides sides,
        FmReportInput input,
        HashSet<(string Market, double? Line, string Side)> covered,
        List<string> notes)
    {
        var result = new List<P13MarketRow>();

        void Add(string label, string key, double? line, string side, double? p)
        {
            if (p is null) return;
            var normalized = (NormalizeKey(key), line, NormalizeSide(side));
            if (!covered.Add(normalized)) return;
            result.Add(ToRow(label, p.Value,
                BestOdds(input, key, line, NormalizeSide(side), null)));
        }

        var pHome = probs.HomeWin();
        var pDraw = probs.Draw();
        var pAway = probs.AwayWin();
        var pYes = probs.BttsYes();

        Add("1X2 - local", "1x2", null, "home", pHome);
        Add("1X2 - empate", "1x2", null, "draw", pDraw);
        Add("1X2 - visitante", "1x2", null, "away", pAway);

        Add("Doble oportunidad - 1X", "double_chance", null, "home_draw", pHome + pDraw);
        Add("Doble oportunidad - X2", "double_chance", null, "draw_away", pDraw + pAway);
        Add("Doble oportunidad - 12", "double_chance", null, "home_away", pHome + pAway);

        Add("Over/Under 2.5 goles - over", "total_goals", 2.5, "over",
            probs.TotalOver(2.5, true));
        Add("Over/Under 2.5 goles - under", "total_goals", 2.5, "under",
            probs.TotalOver(2.5, false));

        Add("BTTS - si", "btts", null, "yes", pYes);
        Add("BTTS - no", "btts", null, "no", 1d - pYes);

        // Línea entera 0: -0.25 / +0.25 se comparan sobre el 0 (empate = empuje).
        Add("Asian Handicap -0.25 local", "ah_home", null, "home", pHome);
        Add("Asian Handicap +0.25 visitante", "ah_away", null, "away", pAway);

        var corners = TotalLambda("corners", sides);
        if (corners is null)
        {
            notes.Add("Over/Under 9.5 corners: sin serie FM de corners, sin modelo.");
        }
        else
        {
            Add("Over/Under 9.5 corners - over", "total_corners", 9.5, "over",
                PoissonSide(corners.Value, 9.5, true));
            Add("Over/Under 9.5 corners - under", "total_corners", 9.5, "under",
                PoissonSide(corners.Value, 9.5, false));
        }

        return result;
    }

    // ---------------------------------------------------------------------------
    // Matriz de goles
    // ---------------------------------------------------------------------------

    private sealed class GoalProbs
    {
        private readonly double[,] _m;

        public GoalProbs(double[,] matrix) => _m = matrix;

        public double HomeWin() => Sum((x, y) => x > y);
        public double Draw() => Sum((x, y) => x == y);
        public double AwayWin() => Sum((x, y) => x < y);
        public double BttsYes() => Sum((x, y) => x > 0 && y > 0);

        public double TotalOver(double line, bool over) =>
            Sum((x, y) => over ? x + y > line : x + y <= line);

        public double HomeGoalsOver(double line, bool over) =>
            Sum((x, y) => over ? x > line : x <= line);

        public double AwayGoalsOver(double line, bool over) =>
            Sum((x, y) => over ? y > line : y <= line);

        private double Sum(Func<int, int, bool> predicate)
        {
            var total = 0d;
            for (var x = 0; x <= MaxGoals; x++)
            for (var y = 0; y <= MaxGoals; y++)
                if (predicate(x, y)) total += _m[x, y];
            return Math.Clamp(total, 0d, 1d);
        }
    }

    private static double[,] BuildMatrix(double lambdaHome, double lambdaAway)
    {
        var engine = new PoissonProbabilityEngine();
        var matrix = new double[MaxGoals + 1, MaxGoals + 1];
        var total = 0d;

        for (var x = 0; x <= MaxGoals; x++)
        for (var y = 0; y <= MaxGoals; y++)
        {
            var p = engine.PoissonProbability(lambdaHome, x) *
                    engine.PoissonProbability(lambdaAway, y) *
                    DixonColes.Tau(x, y, lambdaHome, lambdaAway, DixonColesRho);
            var clamped = Math.Max(0d, p);
            matrix[x, y] = clamped;
            total += clamped;
        }

        if (total <= 0) return matrix;
        for (var x = 0; x <= MaxGoals; x++)
        for (var y = 0; y <= MaxGoals; y++)
            matrix[x, y] /= total;
        return matrix;
    }

    private static double PoissonSide(double lambda, double line, bool over)
    {
        if (lambda < 0) return over ? 1d : 0d;
        return new PoissonProbabilityEngine().GetOverUnderProbability(lambda, line, over);
    }

    // ---------------------------------------------------------------------------
    // Lambdas de mercados de conteo
    // ---------------------------------------------------------------------------

    private static double? WindowMean(FmReportMarket market)
    {
        var windows = market.TeamAttack?.OwnWindows;
        if (windows is null) return null;

        if (windows.TryGetValue("last10", out var last10) &&
            last10 is FmReportWindow w10 && w10.Mean is double m10)
            return m10;

        if (windows.TryGetValue("all", out var all) &&
            all is FmReportWindow wall && wall.Mean is double mall)
            return mall;

        return null;
    }

    private static double? TotalLambda(string core, FixtureSides sides)
    {
        var home = StatMean(sides.HomeRows, core, "home");
        var away = StatMean(sides.AwayRows, core, "away");
        if (home is null && away is null) return null;
        return (home ?? 0d) + (away ?? 0d);
    }

    private static double? StatMean(
        IReadOnlyList<FmTeamMatchRow> rows, string core, string? location)
    {
        var stat = TeamStatFor(core);
        if (stat is null || rows.Count == 0) return null;

        var values = new List<double>();
        foreach (var row in rows.OrderByDescending(r => r.TsUtc))
        {
            if (location is not null &&
                !string.Equals(row.Location, location, StringComparison.OrdinalIgnoreCase))
                continue;
            if (row.TeamStatsJson is null) continue;
            if (TryReadStat(row.TeamStatsJson, stat, out var v)) values.Add(v);
            if (values.Count >= StatWindow) break;
        }

        return values.Count == 0 ? null : values.Average();
    }

    // market (sin prefijo home_/away_/total_) -> team_stats_json. Espejo del
    // mapa privado de FmConfluenceReportBuilder, solo para el cálculo del P13.
    private static string? TeamStatFor(string core) => core switch
    {
        "goals" => "goals",
        "corners" => "corners",
        "shots" => "sh",
        "shots_on_target" => "sot",
        "saves" => "saves",
        "goalkeeper_saves" => "saves",
        "cards" => "cards",
        "tackles" => "tackles",
        "offsides" => "offsides",
        "fouls_committed" => "foulsC",
        "fouls_drawn" => "foulsD",
        "shots_created" => "shotsCreated",
        "score_assist" => "assists",
        "assists" => "assists",
        _ => null
    };

    private static bool TryReadStat(string json, string stat, out double value)
    {
        value = 0;
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty(stat, out var node)) return false;
            if (node.ValueKind == System.Text.Json.JsonValueKind.Number &&
                node.TryGetDouble(out var d))
            {
                value = d;
                return true;
            }
            if (node.ValueKind == System.Text.Json.JsonValueKind.String &&
                double.TryParse(node.GetString(), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out d))
            {
                value = d;
                return true;
            }
        }
        catch (System.Text.Json.JsonException)
        {
            return false;
        }
        return false;
    }

    // ---------------------------------------------------------------------------
    // Dirección de la señal y cuotas
    // ---------------------------------------------------------------------------

    // market + linea -> "over"/"under" de la señal (fm_signal es quien la tiene;
    // el contrato del reporte no la conserva).
    private static Dictionary<(string Market, double?), string> DirectionByMarket(
        FmReportInput input)
    {
        var map = new Dictionary<(string, double?), string>();
        foreach (var signal in input.Signals)
        {
            if (signal.SubjectType is not "team") continue;
            if (string.IsNullOrWhiteSpace(signal.Market)) continue;
            if (string.IsNullOrWhiteSpace(signal.Direction)) continue;
            var key = (signal.Market!, signal.Line);
            if (!map.ContainsKey(key)) map[key] = signal.Direction!;
        }
        return map;
    }

    private static string? SideOf(
        FmReportMarket market, IReadOnlyDictionary<(string Market, double?), string> directions)
    {
        var direction = SignalDirection(market, directions);
        if (!string.IsNullOrWhiteSpace(direction)) return direction!.Trim();

        var sides = market.MarketOddsFm?
            .Select(o => o.Side)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray() ?? Array.Empty<string>();
        return sides.Length == 1 ? sides[0] : null;
    }

    private static string? SignalDirection(
        FmReportMarket market,
        IReadOnlyDictionary<(string Market, double?), string>? directions)
    {
        if (directions is not null &&
            !string.IsNullOrWhiteSpace(market.Market) &&
            directions.TryGetValue((market.Market!, market.Line), out var found))
            return found;
        return null;
    }

    private static string? BestOdds(
        FmReportInput input, string? market, double? line, string side, string? subject)
    {
        if (string.IsNullOrWhiteSpace(market)) return null;

        FmReportOddsRow? best = null;
        foreach (var row in input.Odds)
        {
            if (!string.Equals(row.Source, "fm", StringComparison.OrdinalIgnoreCase)) continue;
            if (!string.Equals(row.Market, market, StringComparison.OrdinalIgnoreCase)) continue;
            if (!LineEquals(row.Line, line)) continue;
            if (!string.Equals(row.Side, side, StringComparison.OrdinalIgnoreCase)) continue;
            if (subject is not null && row.SubjectName is not null &&
                !string.Equals(row.SubjectName.Trim(), subject.Trim(),
                    StringComparison.OrdinalIgnoreCase)) continue;

            if (best is null || row.OddsValue > best.OddsValue ||
                (Math.Abs(row.OddsValue - best.OddsValue) < 1e-9 &&
                 string.CompareOrdinal(row.SourceTimestampUtc, best.SourceTimestampUtc) > 0))
                best = row;
        }

        if (best is null) return null;
        return FormatOdds(best);
    }

    // Mercados de jugador: fila EXACTA por jugador + mercado + linea + lado.
    // Sin fila exacta no se calcula Delta%: jamas se hereda la cuota del equipo
    // ni se interpola entre lineas.
    private static string? BestPlayerOdds(
        FmReportInput input, string market, double? line, string side, string subject)
    {
        FmReportOddsRow? best = null;
        foreach (var row in input.Odds)
        {
            if (!string.Equals(row.Source, "fm", StringComparison.OrdinalIgnoreCase)) continue;
            if (string.IsNullOrWhiteSpace(row.SubjectName)) continue;
            if (!string.Equals(row.SubjectName.Trim(), subject.Trim(),
                    StringComparison.OrdinalIgnoreCase)) continue;
            if (!string.Equals(row.Market, market, StringComparison.OrdinalIgnoreCase)) continue;
            if (!LineEquals(row.Line, line)) continue;
            if (!string.Equals(row.Side, side, StringComparison.OrdinalIgnoreCase)) continue;

            if (best is null || row.OddsValue > best.OddsValue ||
                (Math.Abs(row.OddsValue - best.OddsValue) < 1e-9 &&
                 string.CompareOrdinal(row.SourceTimestampUtc, best.SourceTimestampUtc) > 0))
                best = row;
        }

        return best is null ? null : FormatOdds(best);
    }

    private static string FormatOdds(FmReportOddsRow row)
    {
        var name = string.IsNullOrWhiteSpace(row.BookmakerName)
            ? row.Bookmaker
            : row.BookmakerName!;
        return $"{row.OddsValue.ToString("0.00", CultureInfo.InvariantCulture)} ({name})";
    }

    // ---------------------------------------------------------------------------
    // Utilidades
    // ---------------------------------------------------------------------------

    private static P13MarketRow ToRow(string label, double p, string? oddsFm)
    {
        var fair = p > 0 ? 1d / p : (double?)null;
        if (oddsFm is null)
            return new P13MarketRow(label, p, fair, null, null, NoOddsStatus);

        var odds = ParseOdds(oddsFm);
        if (odds is null || p <= 0)
            return new P13MarketRow(label, p, fair, oddsFm, null, NoOddsStatus);

        var delta = ((p * odds.Value) - 1d) * 100d;
        return new P13MarketRow(label, p, fair, oddsFm, delta, null);
    }

    private static double? ParseOdds(string oddsFm)
    {
        var cut = oddsFm.IndexOf(' ');
        var token = cut > 0 ? oddsFm[..cut] : oddsFm;
        return double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture,
            out var value) ? value : null;
    }

    private static IReadOnlyList<FmTeamMatchRow> RowsFor(FmReportInput input, long? apid) =>
        apid is long a && input.TeamRows.TryGetValue(a, out var rows)
            ? rows
            : Array.Empty<FmTeamMatchRow>();

    private static string Core(string market)
    {
        var m = market.Trim();
        if (m.StartsWith("home_", StringComparison.Ordinal)) return m["home_".Length..];
        if (m.StartsWith("away_", StringComparison.Ordinal)) return m["away_".Length..];
        if (m.StartsWith("total_", StringComparison.Ordinal)) return m["total_".Length..];
        return m;
    }

    private static string NormalizeKey(string market) => market.Trim().ToLowerInvariant();

    private static string NormalizeSide(string side) => side.Trim().ToLowerInvariant();

    private static string Label(string? market, double? line, string side)
    {
        var label = market ?? string.Empty;
        if (line is not null)
            label += " @ " + Num(line.Value);
        return $"{label} ({side})";
    }

    private static bool LineEquals(double? a, double? b) =>
        a is null ? b is null : b is not null && Math.Abs(a.Value - b.Value) < 1e-9;

    private static DateTime? ParseUtc(string? value) =>
        DateTime.TryParse(value, CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var dt)
            ? dt
            : null;

    public static double ThresholdFor(string volume) =>
        string.Equals(volume, "alto", StringComparison.OrdinalIgnoreCase) ? 6d : 5d;

    public static string ConfidenceFor(string volume) =>
        string.Equals(volume, "bajo", StringComparison.OrdinalIgnoreCase) ? "Baja"
            : string.Equals(volume, "alto", StringComparison.OrdinalIgnoreCase) ? "Alta"
            : "Media";

    private static string Num(double value) =>
        value.ToString("0.##", CultureInfo.InvariantCulture);

    private sealed class FixtureKeyComparer :
        IEqualityComparer<(string Market, double? Line, string Side)>
    {
        public static readonly FixtureKeyComparer Instance = new();

        public bool Equals((string Market, double? Line, string Side) x,
            (string Market, double? Line, string Side) y) =>
            string.Equals(x.Market, y.Market, StringComparison.OrdinalIgnoreCase) &&
            LineEquals(x.Line, y.Line) &&
            string.Equals(x.Side, y.Side, StringComparison.OrdinalIgnoreCase);

        public int GetHashCode((string Market, double? Line, string Side) obj) =>
            HashCode.Combine(
                obj.Market.ToLowerInvariant(),
                obj.Line,
                obj.Side.ToLowerInvariant());
    }
}
