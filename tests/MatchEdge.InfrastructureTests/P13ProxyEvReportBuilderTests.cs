using System.Text.Json;
using System.Text.RegularExpressions;
using MatchEdge.Infrastructure.Services;
using Xunit;

namespace MatchEdge.InfrastructureTests;

// P13: tests propios del builder separado Proxy-EV (delta modelo-mercado).
// No reutiliza los de P11: validan el contrato del brief P13 (header Delta%,
// estado SIN CUOTA FM, ranking unico de equipo + jugador con tope de 10,
// umbral por volumen, HomeAdv provisional, ausencia de las palabras del regex
// F2 en el markdown), la extension de mercados de jugadores (ventana last10,
// muestra minima, fila exacta de cuota) y la deteccion automatica de
// post-partido del GAP1 (kickoff del overview vs primera captura, sin flag,
// con fail-closed cuando no hay overview que leer).
public class P13ProxyEvReportBuilderTests
{
    private const string FixtureWithOdds = "33662307";
    private const string FixtureNoOdds = "33662309";
    private const string FixtureNoSnapshot = "33662315";

    // Mismo patron que FmDeterministicTests.Forbidden (F2 del contrato P11).
    private static readonly Regex Forbidden = new(
        @"recomend|apuesta|\bedge\b|value bet|elegid|banker|sub[ -]?hero|\bpick\b|\bev\b",
        RegexOptions.IgnoreCase);

    private static readonly P13FixtureSpec[] Specs =
    {
        new(FixtureWithOdds, "Portugal", "Wales", "alto"),
        new(FixtureNoOdds, "Serbia", "Greece", "bajo"),
        new(FixtureNoSnapshot, "France", "Italy", "medio")
    };

    [Fact]
    public void ThresholdAndVolume_FollowBriefTables()
    {
        Assert.Equal(6d, P13ProxyEvReportBuilder.ThresholdFor("alto"));
        Assert.Equal(5d, P13ProxyEvReportBuilder.ThresholdFor("medio"));
        Assert.Equal(5d, P13ProxyEvReportBuilder.ThresholdFor("bajo"));

        Assert.Equal("Alta", P13ProxyEvReportBuilder.ConfidenceFor("alto"));
        Assert.Equal("Media", P13ProxyEvReportBuilder.ConfidenceFor("medio"));
        Assert.Equal("Baja", P13ProxyEvReportBuilder.ConfidenceFor("bajo"));
    }

    // El estado sin cuota no puede usar la literal "EV%": esa palabra rompe el
    // regex F2 si tmp/p13_uefa_*.md se escanea con el mismo patron.
    [Fact]
    public void NoOddsStatus_AndHeaders_DoNotMatchForbiddenPattern()
    {
            Assert.DoesNotMatch(Forbidden, P13ProxyEvReportBuilder.NoOddsStatus);
            Assert.DoesNotMatch(Forbidden, P13ProxyEvReportBuilder.NoSignalText);
            Assert.DoesNotMatch(Forbidden, P13ProxyEvReportBuilder.NoDataText);
            Assert.DoesNotMatch(Forbidden, P13ProxyEvReportBuilder.MuestraInsufficientStatus);
            Assert.DoesNotMatch(Forbidden, P13ProxyEvReportBuilder.PlayerModelNote);
            Assert.DoesNotMatch(Forbidden, P13ProxyEvReportBuilder.PlayerPostCloseNote);
            Assert.DoesNotMatch(Forbidden, P13ProxyEvReportBuilder.PlayerNoSignalNote);
            Assert.DoesNotMatch(Forbidden, P13ProxyEvReportMarkdown.Title);
        Assert.Contains("| Delta% |", RenderedHeaderLine());
        Assert.DoesNotContain("EV%", RenderedHeaderLine());
    }

    [Fact]
    public async Task Report_Markdown_MatchesBriefHeadersAndStaysNeutral()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            var outcomes = new FmOutcomeStore($"Data Source={dbPath}");
            await SeedAsync(store, FixtureWithOdds, withSignals: true, withOdds: true);
            await SeedAsync(store, FixtureNoOdds, withSignals: true, withOdds: false);
            await SeedPreMatchOverviewAsync(store, FixtureWithOdds);
            await SeedPreMatchOverviewAsync(store, FixtureNoOdds);

            var report = await P13ProxyEvReportBuilder.BuildAsync(Specs, store, outcomes);
            var md = P13ProxyEvReportMarkdown.Render(report);

            Assert.Contains(
                "| Mercado | P_modelo | Cuota Justa | Cuota FM (bookmaker) | Delta% |", md);
            Assert.Contains(
                "| Partido | Mercado candidato | P_modelo | Cuota Justa | Cuota FM | Delta% " +
                "| Confianza | Snapshot | Volumen |", md);
            Assert.Contains(P13ProxyEvReportBuilder.HomeAdvNote, md);
            Assert.Contains("Dixon-Coles rho = 0.1", md);
            Assert.Contains("## Filtros live F1/F2", md);
            Assert.Contains(P13ProxyEvReportBuilder.NoDataText, md);
            Assert.Contains(P13ProxyEvReportBuilder.NoSignalText, md);

            Assert.DoesNotMatch(Forbidden, md);
            Assert.DoesNotMatch(Forbidden, JsonSerializer.Serialize(report));
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    [Fact]
    public async Task Report_Sections_CarryModelRowsOddsAndCandidatesRules()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            var outcomes = new FmOutcomeStore($"Data Source={dbPath}");
            await SeedAsync(store, FixtureWithOdds, withSignals: true, withOdds: true);
            await SeedAsync(store, FixtureNoOdds, withSignals: true, withOdds: false);
            await SeedPreMatchOverviewAsync(store, FixtureWithOdds);
            await SeedPreMatchOverviewAsync(store, FixtureNoOdds);

            var report = await P13ProxyEvReportBuilder.BuildAsync(Specs, store, outcomes);

            var withOdds = report.Fixtures.Single(s => s.FixtureId == FixtureWithOdds);
            Assert.True(withOdds.HasData);
            Assert.Equal(6d, withOdds.ThresholdPct);
            Assert.Equal("Alta", withOdds.Confidence);
            Assert.NotNull(withOdds.LambdaHome);
            Assert.NotNull(withOdds.LambdaAway);
            Assert.True(withOdds.LambdaHome > 0);
            Assert.True(withOdds.LambdaAway > 0);
            Assert.Contains(withOdds.Notes, n => n.Contains(P13ProxyEvReportBuilder.HomeAdvNote));

            // Mercados obligatorios del brief siempre presentes.
            foreach (var label in new[]
                     {
                         "1X2 - local", "1X2 - empate", "1X2 - visitante",
                         "Doble oportunidad - 1X", "Doble oportunidad - X2",
                         "Doble oportunidad - 12", "Over/Under 2.5 goles - over",
                         "Over/Under 2.5 goles - under", "BTTS - si", "BTTS - no",
                         "Asian Handicap -0.25 local", "Asian Handicap +0.25 visitante"
                     })
                Assert.Contains(withOdds.Rows, r => r.Market == label);

            // La cuota FM es la MEJOR de las casas del fixture (2.10 > 1.91).
            var saves = Assert.Single(withOdds.Rows,
                r => r.Market.StartsWith("home_saves", StringComparison.Ordinal));
            Assert.NotNull(saves.OddsFm);
            Assert.StartsWith("2.10 (", saves.OddsFm);
            Assert.NotNull(saves.DeltaPct);

            // Delta% = (P_modelo x Cuota) - 1, en porcentaje.
            var odds = double.Parse(saves.OddsFm![.. saves.OddsFm.IndexOf(' ')],
                System.Globalization.CultureInfo.InvariantCulture);
            Assert.Equal(((saves.PModel!.Value * odds) - 1d) * 100d,
                saves.DeltaPct!.Value, 9);

            // Candidatos: ranking unico equipo + jugador, maximo CandidatesCap,
            // ordenados y con las dos condiciones del brief para el equipo.
            // El tope subio de 2 a 10 con la extension de mercados de jugadores
            // (el encargo pide ranking combinado equipo + jugador de 10 filas);
            // el test de abajo verifica que hay fixture con mas de 2 candidatos
            // combinados, para que el tope nuevo no se cumpla de casualidad.
            Assert.Equal(10, P13ProxyEvReportBuilder.CandidatesCap);
            Assert.NotEmpty(withOdds.Candidates);
            Assert.True(withOdds.Candidates.Count <= P13ProxyEvReportBuilder.CandidatesCap);
            Assert.True(withOdds.Candidates.Zip(withOdds.Candidates.Skip(1))
                .All(p => p.First.DeltaPct >= p.Second.DeltaPct));
            Assert.All(withOdds.Candidates, c =>
            {
                Assert.True(c.PModel > 0.50);
                Assert.True(c.DeltaPct >= withOdds.ThresholdPct);
                Assert.NotNull(c.OddsFm);
            });
            // Sin senales de jugador en este fixture: la seccion queda vacia con nota.
            Assert.Empty(withOdds.PlayerRows);
            Assert.Contains(withOdds.Notes,
                n => n == P13ProxyEvReportBuilder.PlayerNoSignalNote);
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    [Fact]
    public async Task Report_WithoutOddsRows_UsesSinCuotaStatusAndNoCandidates()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            var outcomes = new FmOutcomeStore($"Data Source={dbPath}");
            await SeedAsync(store, FixtureNoOdds, withSignals: true, withOdds: false);
            await SeedPreMatchOverviewAsync(store, FixtureNoOdds);

            var report = await P13ProxyEvReportBuilder.BuildAsync(
                new[] { Specs[1] }, store, outcomes);
            var section = Assert.Single(report.Fixtures);

            Assert.True(section.HasData);
            Assert.Equal(5d, section.ThresholdPct);
            Assert.Equal("Baja", section.Confidence);
            Assert.NotEmpty(section.Rows);
            Assert.All(section.Rows, r =>
            {
                Assert.Null(r.OddsFm);
                Assert.Null(r.DeltaPct);
                Assert.Equal(P13ProxyEvReportBuilder.NoOddsStatus, r.Status);
            });
            Assert.Empty(section.Candidates);

            var md = P13ProxyEvReportMarkdown.Render(report);
            Assert.Contains(P13ProxyEvReportBuilder.NoOddsStatus, md);
            Assert.Contains(P13ProxyEvReportBuilder.NoSignalText, md);
            Assert.DoesNotMatch(Forbidden, md);
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    [Fact]
    public async Task Report_WithoutSnapshot_MarksNoDataAndSkipsTable()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            var outcomes = new FmOutcomeStore($"Data Source={dbPath}");

            var report = await P13ProxyEvReportBuilder.BuildAsync(
                new[] { Specs[2] }, store, outcomes);
            var section = Assert.Single(report.Fixtures);

            Assert.False(section.HasData);
            Assert.Empty(section.Rows);
            Assert.Empty(section.Candidates);
            Assert.Contains(P13ProxyEvReportBuilder.NoDataText, section.Notes);
            Assert.Equal(5d, section.ThresholdPct);
            Assert.Equal("Media", section.Confidence);

            var md = P13ProxyEvReportMarkdown.Render(report);
            Assert.Contains(P13ProxyEvReportBuilder.NoDataText, md);
            Assert.Contains("| France vs Italy | " + P13ProxyEvReportBuilder.NoDataText, md);
            Assert.DoesNotMatch(Forbidden, md);
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    private static string RenderedHeaderLine() =>
        "| Mercado | P_modelo | Cuota Justa | Cuota FM (bookmaker) | Delta% |";

    // ---- jugadores -------------------------------------------------------------

    [Fact]
    public void PlayerLambda_PrefersLast10_ThenAll_ThenNothing()
    {
        var last10 = new FmWindowStats("10", 10, 7, 0.7, 1.2, 1.0, 0.0, 3.0,
            FmWindowCalculator.StatusOk);
        var all = new FmWindowStats("all", 24, 14, 0.58, 1.51, 1.0, 0.0, 4.0,
            FmWindowCalculator.StatusOk);
        Assert.Equal(1.2, P13ProxyEvReportBuilder.PlayerLambda(new[] { last10, all }));

        // last10 por debajo de la muestra minima: se cae a toda la serie.
        var shortLast10 = new FmWindowStats("10", 4, 2, 0.5, 2.5, 2.0, 1.0, 5.0,
            FmWindowCalculator.StatusOk);
        var fullSeries = new FmWindowStats("all", 7, 4, 0.57, 1.4, 1.0, 0.0, 3.0,
            FmWindowCalculator.StatusOk);
        Assert.Equal(1.4, P13ProxyEvReportBuilder.PlayerLambda(
            new[] { shortLast10, fullSeries }));

        // Ninguna ventana llega a la muestra minima: sin lambda.
        var insufficient = new FmWindowStats("all", 3, null, null, null, null, null,
            null, FmWindowCalculator.StatusInsufficient);
        Assert.Null(P13ProxyEvReportBuilder.PlayerLambda(new[] { insufficient }));
    }

    [Fact]
    public async Task Player_Last10Window_UsesExactOddsAndRendersColumns()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            var outcomes = new FmOutcomeStore($"Data Source={dbPath}");
            await SeedAsync(store, FixtureWithOdds, withSignals: true, withOdds: true);

            // 12 observaciones: la media de las 10 mas recientes (1.30) difiere de
            // la de toda la serie (1.9166...), asi que el test ve que ventana uso.
            var values = new double[] { 2, 1, 1, 1, 3, 2, 1, 0, 1, 1, 5, 5 };
            await SeedPlayersAsync(store, FixtureWithOdds,
                new[] { PlayerSignal("K. Demo", "shots", 0.5, values) },
                new[] { PlayerOdds("K. Demo", "shots", 0.5, 1.90) });
            await SeedPreMatchOverviewAsync(store, FixtureWithOdds);

            var report = await P13ProxyEvReportBuilder.BuildAsync(
                new[] { Specs[0] }, store, outcomes);
            var section = Assert.Single(report.Fixtures);
            var row = Assert.Single(section.PlayerRows);

            Assert.Equal("Portugal", row.Team);
            Assert.Equal("K. Demo", row.Player);
            Assert.Equal("shots @ 0.5 (over)", row.Market);
            Assert.Null(row.Status);
            Assert.NotNull(row.OddsFm);
            Assert.StartsWith("1.90 (", row.OddsFm);

            var last10 = values.Take(10).Average();
            var all = values.Average();
            Assert.NotEqual(last10, all, 9);
            Assert.Equal(OverProbability(last10, 0.5), row.PModel!.Value, 9);
            Assert.Equal(1d / row.PModel!.Value, row.FairOdds!.Value, 9);
            Assert.Equal(((row.PModel!.Value * 1.90) - 1d) * 100d,
                row.DeltaPct!.Value, 9);
            Assert.Contains(section.Candidates, c => c.Player == "K. Demo");

            var md = P13ProxyEvReportMarkdown.Render(report);
            Assert.Contains(
                "| Equipo | Jugador | Mercado | P_modelo | Cuota Justa | " +
                "Cuota FM (casa) | Delta% |", md);
            Assert.Contains("| Portugal | K. Demo | shots @ 0.5 (over) |", md);
            Assert.Contains(P13ProxyEvReportBuilder.PlayerModelNote, md);
            Assert.DoesNotMatch(Forbidden, md);
            Assert.DoesNotMatch(Forbidden, JsonSerializer.Serialize(report));
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    [Fact]
    public async Task Player_SeriesBelowMinSample_UsesMuestraInsuficiente()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            var outcomes = new FmOutcomeStore($"Data Source={dbPath}");
            await SeedAsync(store, FixtureWithOdds, withSignals: true, withOdds: false);

            // 4 observaciones: ni last10 ni all llegan a la muestra minima.
            await SeedPlayersAsync(store, FixtureWithOdds,
                new[] { PlayerSignal("R. Corto", "tackles", 0.5, new double[] { 2, 1, 3, 1 }) },
                new[] { PlayerOdds("R. Corto", "tackles", 0.5, 2.00) });
            await SeedPreMatchOverviewAsync(store, FixtureWithOdds);

            var report = await P13ProxyEvReportBuilder.BuildAsync(
                new[] { Specs[0] }, store, outcomes);
            var section = Assert.Single(report.Fixtures);
            var row = Assert.Single(section.PlayerRows);

            Assert.Equal(P13ProxyEvReportBuilder.MuestraInsufficientStatus, row.Status);
            Assert.Null(row.PModel);
            Assert.Null(row.FairOdds);
            Assert.Null(row.OddsFm);
            Assert.Null(row.DeltaPct);
            Assert.DoesNotContain(section.Candidates, c => c.Player is not null);

            var md = P13ProxyEvReportMarkdown.Render(report);
            Assert.Contains(P13ProxyEvReportBuilder.MuestraInsufficientStatus, md);
            Assert.DoesNotMatch(Forbidden, md);
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    [Fact]
    public async Task Player_WithoutExactOddsRow_DoesNotInheritTeamOdds()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            var outcomes = new FmOutcomeStore($"Data Source={dbPath}");
            // El seed deja una fila de EQUIPO en home_saves @ 1.5 (over), 2.10.
            await SeedAsync(store, FixtureWithOdds, withSignals: true, withOdds: true);
            await SeedPlayersAsync(store, FixtureWithOdds,
                new[] { PlayerSignal("T. SinCuota", "home_saves", 1.5,
                    new double[] { 2, 1, 2, 2, 1, 3 }) },
                Array.Empty<FmOddsDraft>());
            await SeedPreMatchOverviewAsync(store, FixtureWithOdds);

            var report = await P13ProxyEvReportBuilder.BuildAsync(
                new[] { Specs[0] }, store, outcomes);
            var section = Assert.Single(report.Fixtures);
            var row = Assert.Single(section.PlayerRows);

            Assert.NotNull(row.PModel);
            Assert.Null(row.OddsFm);
            Assert.Null(row.DeltaPct);
            Assert.Equal(P13ProxyEvReportBuilder.NoOddsStatus, row.Status);
            Assert.DoesNotContain(section.Candidates, c => c.Player is not null);

            var md = P13ProxyEvReportMarkdown.Render(report);
            Assert.Contains(P13ProxyEvReportBuilder.NoOddsStatus, md);
            Assert.DoesNotMatch(Forbidden, md);
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    [Fact]
    public async Task Player_CandidatesShareTheTenSlotsWithTeamRows()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            var outcomes = new FmOutcomeStore($"Data Source={dbPath}");
            await SeedAsync(store, FixtureWithOdds, withSignals: true, withOdds: true);

            var signals = new List<FmSignalDraft>();
            var odds = new List<FmOddsDraft>();
            for (var i = 1; i <= 9; i++)
            {
                var name = $"J. Demo{i}";
                signals.Add(PlayerSignal(name, "shots", 0.5,
                    new double[] { 1, 1, 2, 2, 2, 1 }));
                odds.Add(PlayerOdds(name, "shots", 0.5, 1.50));
            }
            await SeedPlayersAsync(store, FixtureWithOdds, signals, odds);
            await SeedPreMatchOverviewAsync(store, FixtureWithOdds);

            var report = await P13ProxyEvReportBuilder.BuildAsync(
                new[] { Specs[0] }, store, outcomes);
            var section = Assert.Single(report.Fixtures);

            Assert.Equal(9, section.PlayerRows.Count(
                r => r.DeltaPct >= section.ThresholdPct));
            // 9 jugadores + al menos 1 candidato de equipo -> tope combinado.
            // Con el tope viejo de 2 este fixture se quedaba en 2 filas; ahora
            // hay que llenar los 10 slots y el test comprueba que supera 2.
            Assert.True(section.Candidates.Count > 2);
            Assert.Equal(P13ProxyEvReportBuilder.CandidatesCap,
                section.Candidates.Count);
            Assert.Contains(section.Candidates, c => c.Player is not null);
            Assert.Contains(section.Candidates, c => c.Player is null);
            Assert.True(section.Candidates.Zip(section.Candidates.Skip(1))
                .All(p => p.First.DeltaPct >= p.Second.DeltaPct));
            Assert.All(section.Candidates, c =>
            {
                Assert.NotNull(c.OddsFm);
                Assert.True(c.DeltaPct >= section.ThresholdPct);
            });
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    // P13 GAP1: si la primera captura del fixture es posterior a su kickoff el
    // reporte entra solo en modo post-partido. No hay flag manual: el spec se
    // construye con los 4 campos del brief y el estado sale de los datos.
    [Fact]
    public async Task Report_AfterKickoff_IsPostMatchWithoutManualFlag()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        var overview = Path.Combine(Path.GetTempPath(), $"fmov_{Guid.NewGuid():N}.html");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            var outcomes = new FmOutcomeStore($"Data Source={dbPath}");
            await SeedAsync(store, FixtureWithOdds, withSignals: true, withOdds: true);
            await SeedPlayersAsync(store, FixtureWithOdds,
                new[] { PlayerSignal("K. Demo", "shots", 0.5,
                    new double[] { 2, 1, 1, 1, 3, 2 }) },
                new[] { PlayerOdds("K. Demo", "shots", 0.5, 1.90) });

            // kickoff 1h antes de la primera captura -> post-kickoff automatico.
            var kickoff = DateTime.UtcNow.AddHours(-1);
            overview = await SeedOverviewKickoffAsync(
                store, FixtureWithOdds, kickoff, DateTime.UtcNow.AddMinutes(-30));

            var report = await P13ProxyEvReportBuilder.BuildAsync(
                new[] { Specs[0] }, store, outcomes);
            var section = Assert.Single(report.Fixtures);

            Assert.True(section.IsPostMatch);
            Assert.True(section.HasData);
            Assert.Empty(section.Rows);
            Assert.Empty(section.PlayerRows);
            Assert.Empty(section.Candidates);
            Assert.Null(section.LambdaHome);
            Assert.Contains(P13ProxyEvReportBuilder.PostMatchText, section.Notes);
            Assert.Contains(section.Notes, n => n.Contains("Captura posterior al kickoff"));
            Assert.Contains(section.Notes,
                n => n.Contains(P13ProxyEvReportBuilder.PlayerPostCloseNote));

            var md = P13ProxyEvReportMarkdown.Render(report);
            Assert.Contains(P13ProxyEvReportBuilder.PostMatchText, md);
            Assert.DoesNotContain("#### Mercados", md);
            Assert.DoesNotContain("| Equipo | Jugador | Mercado |", md);
            Assert.Contains($"| Portugal vs Wales | " +
                            $"{P13ProxyEvReportBuilder.PostMatchText}", md);
            Assert.DoesNotMatch(Forbidden, md);
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
            File.Delete(overview);
        }
    }

    // Mismo fixture, captura ANTES del kickoff: sigue en modo modelo y los
    // mercados de jugador se calculan (contraparte positiva del test anterior).
    [Fact]
    public async Task Report_BeforeKickoff_KeepsModelAndPlayers()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        var overview = Path.Combine(Path.GetTempPath(), $"fmov_{Guid.NewGuid():N}.html");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            var outcomes = new FmOutcomeStore($"Data Source={dbPath}");
            await SeedAsync(store, FixtureWithOdds, withSignals: true, withOdds: true);
            await SeedPlayersAsync(store, FixtureWithOdds,
                new[] { PlayerSignal("K. Demo", "shots", 0.5,
                    new double[] { 2, 1, 1, 1, 3, 2 }) },
                new[] { PlayerOdds("K. Demo", "shots", 0.5, 1.90) });

            // kickoff en el futuro -> la captura es pre-partido.
            var kickoff = DateTime.UtcNow.AddHours(1);
            overview = await SeedOverviewKickoffAsync(
                store, FixtureWithOdds, kickoff, DateTime.UtcNow);

            var report = await P13ProxyEvReportBuilder.BuildAsync(
                new[] { Specs[0] }, store, outcomes);
            var section = Assert.Single(report.Fixtures);

            Assert.False(section.IsPostMatch);
            Assert.NotEmpty(section.Rows);
            Assert.NotEmpty(section.PlayerRows);

            var md = P13ProxyEvReportMarkdown.Render(report);
            Assert.DoesNotContain($"| Portugal vs Wales | " +
                                  $"{P13ProxyEvReportBuilder.PostMatchText}", md);
            Assert.Contains("#### Mercados", md);
            Assert.DoesNotMatch(Forbidden, md);
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
            File.Delete(overview);
        }
    }

    // P13 GAP1 fail-closed: fixture con captura pero sin tab overview (sin
    // kickoff legible). No se puede comparar la primera captura con el partido,
    // asi que la seccion se cierra en modo registro en vez de calcular modelo,
    // candidatos y mercados de jugador a ciegas (caso del matchday siguiente,
    // con fixtures capturados antes de tener su overview).
    [Fact]
    public async Task Report_WithoutOverviewSnapshot_FailsClosedAsPostMatch()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            var outcomes = new FmOutcomeStore($"Data Source={dbPath}");
            await SeedAsync(store, FixtureWithOdds, withSignals: true, withOdds: true);
            await SeedPlayersAsync(store, FixtureWithOdds,
                new[] { PlayerSignal("K. Demo", "shots", 0.5,
                    new double[] { 2, 1, 1, 1, 3, 2 }) },
                new[] { PlayerOdds("K. Demo", "shots", 0.5, 1.90) });

            // A proposito: sin SeedOverviewKickoffAsync no hay kickoff que leer.
            var report = await P13ProxyEvReportBuilder.BuildAsync(
                new[] { Specs[0] }, store, outcomes);
            var section = Assert.Single(report.Fixtures);

            Assert.True(section.IsPostMatch);
            Assert.True(section.HasData);
            Assert.Null(section.KickoffUtc);
            Assert.Null(section.LambdaHome);
            Assert.Empty(section.Rows);
            Assert.Empty(section.PlayerRows);
            Assert.Empty(section.Candidates);
            Assert.Contains(P13ProxyEvReportBuilder.PostMatchText, section.Notes);
            Assert.Contains(section.Notes,
                n => n.Contains("Sin snapshot overview persistido"));
            Assert.Contains(section.Notes, n => n.Contains("fail-closed"));
            Assert.Contains(section.Notes,
                n => n.Contains(P13ProxyEvReportBuilder.PlayerPostCloseNote));

            var md = P13ProxyEvReportMarkdown.Render(report);
            Assert.Contains(P13ProxyEvReportBuilder.PostMatchText, md);
            Assert.Contains("Kickoff: n/d", md);
            Assert.Contains($"| Portugal vs Wales | " +
                            $"{P13ProxyEvReportBuilder.PostMatchText}", md);
            Assert.DoesNotContain("#### Mercados", md);
            Assert.DoesNotContain("| Equipo | Jugador | Mercado |", md);
            Assert.DoesNotMatch(Forbidden, md);
            Assert.DoesNotMatch(Forbidden, JsonSerializer.Serialize(report));
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
        }
    }

    [Fact]
    public void PostKickoffDetection_ComparesFirstSnapshotAgainstKickoff()
    {
        var kickoff = new DateTime(2026, 9, 28, 18, 45, 0, DateTimeKind.Utc);

        Assert.True(P13ProxyEvReportBuilder.IsPostKickoff(
            kickoff, kickoff.AddMinutes(1)));
        Assert.False(P13ProxyEvReportBuilder.IsPostKickoff(
            kickoff, kickoff.AddMinutes(-1)));
        // Igual no es post-partido y sin kickoff no se decide nada.
        Assert.False(P13ProxyEvReportBuilder.IsPostKickoff(kickoff, kickoff));
        Assert.False(P13ProxyEvReportBuilder.IsPostKickoff(null, kickoff));
        Assert.False(P13ProxyEvReportBuilder.IsPostKickoff(kickoff, null));
    }

    [Fact]
    public void ParseKickoffUtc_ReadsJsonLdStartDateFromOverview()
    {
        const string html =
            "<script type=\"application/ld+json\">{\"@context\":\"https://schema.org\"," +
            "\"@type\":\"SportsEvent\",\"name\":\"Turkiye vs Italy\"," +
            "\"startDate\":\"2026-09-28T18:45:00.000Z\"}</script>";

        Assert.Equal(
            new DateTime(2026, 9, 28, 18, 45, 0, DateTimeKind.Utc),
            P13ProxyEvReportBuilder.ParseKickoffUtc(html));
        Assert.Null(P13ProxyEvReportBuilder.ParseKickoffUtc("<html>sin kickoff</html>"));
        Assert.Null(P13ProxyEvReportBuilder.ParseKickoffUtc(null));
    }

    // P13 GAP1: el kickoff del fixture vive en el snapshot overview persistido.
    [Fact]
    public async Task PostKickoffSnapshot_DoesNotComputePlayers()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"fmtest_{Guid.NewGuid():N}.db");
        var overview = Path.Combine(Path.GetTempPath(), $"fmov_{Guid.NewGuid():N}.html");
        try
        {
            var store = new FmSnapshotStore($"Data Source={dbPath}");
            var outcomes = new FmOutcomeStore($"Data Source={dbPath}");
            await SeedAsync(store, FixtureWithOdds, withSignals: true, withOdds: true);
            await SeedPlayersAsync(store, FixtureWithOdds,
                new[] { PlayerSignal("K. Demo", "shots", 0.5,
                    new double[] { 2, 1, 1, 1, 3, 2 }) },
                new[] { PlayerOdds("K. Demo", "shots", 0.5, 1.90) });

            overview = await SeedOverviewKickoffAsync(
                store, FixtureWithOdds, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow);

            var report = await P13ProxyEvReportBuilder.BuildAsync(
                new[] { Specs[0] }, store, outcomes);
            var section = Assert.Single(report.Fixtures);

            Assert.Empty(section.PlayerRows);
            Assert.Contains(P13ProxyEvReportBuilder.PlayerPostCloseNote, section.Notes);

            var md = P13ProxyEvReportMarkdown.Render(report);
            Assert.Contains(P13ProxyEvReportBuilder.PlayerPostCloseNote, md);
            Assert.DoesNotContain("| Equipo | Jugador | Mercado |", md);
            Assert.DoesNotMatch(Forbidden, md);
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(dbPath);
            File.Delete(overview);
        }
    }

    // P(X > line) con X ~ Poisson(lambda): comprobacion independiente del motor.
    private static double OverProbability(double lambda, double line)
    {
        var p = 0d;
        for (var k = 0; k <= 60; k++)
            if (k > line) p += Math.Exp(-lambda) * Math.Pow(lambda, k) / Factorial(k);
        return p;
    }

    private static double Factorial(int k)
    {
        var f = 1d;
        for (var i = 2; i <= k; i++) f *= i;
        return f;
    }

    // ---- seed ----------------------------------------------------------------

    private static FmTeamMatchDraft TeamDraft(
        long teamApid, string fixtureId, DateTime ts, string location,
        long opponentApid, string opponent, int hg, int ag, string statsJson) =>
        new(teamApid, fixtureId, 900_000 + (Math.Abs(fixtureId.GetHashCode()) % 100_000),
            ts, location, opponentApid, opponent, null, "UEFA Nations League",
            hg, ag, statsJson, null, 15, "corners");

    private static string Stats(int saves, int corners) =>
        $"{{\"saves\":{saves},\"corners\":{corners},\"sh\":10,\"sot\":3,\"cards\":1," +
        "\"tackles\":12,\"offsides\":2,\"foulsC\":8,\"foulsD\":9}";

    private static string History(int entries, double value, int hits) =>
        "[" + string.Join(",", Enumerable.Range(0, entries).Select(i =>
            $"{{\"t\":\"2026-{(9 - i / 4):00}-{(20 - i * 2):00}T18:00:00.000Z\"," +
            $"\"vt\":{value},\"met\":{(i < hits ? "true" : "false")}}}")) + "]";

    private static async Task SeedAsync(
        FmSnapshotStore store, string fixtureId, bool withSignals, bool withOdds)
    {
        var kickoff = new DateTime(2026, 9, 28, 18, 45, 0, DateTimeKind.Utc);

        if (withSignals)
        {
            var snapshotId = await store.InsertSnapshotAsync(
                fixtureId, "team-trends",
                $"https://www.footymetrics.com/fixtures/{fixtureId}-x",
                DateTime.UtcNow, "p", "s", "fm-json-v1", "OK");

            await store.InsertSignalsAsync(
                snapshotId, fixtureId,
                new List<FmSignalDraft>
                {
                    new("team", "Portugal", "home_saves", 1.5, "over", 10, 10, 1.0,
                        null, null, null, History(10, 5, 10)),
                    new("team", "Wales", "away_corners", 3.5, "over", 8, 10, 0.8,
                        null, null, null, History(10, 6, 8)),
                    new("team", "Portugal", "total_goals", 2.5, "over", 6, 10, 0.6,
                        null, null, null, History(10, 4, 6))
                }, "all", "all", DateTime.UtcNow, """{"location":"all"}""");

            if (withOdds)
            {
                await store.InsertOddsAsync(fixtureId, new List<FmOddsDraft>
                {
                    // Dos casas para home_saves: el builder debe quedarse 2.10.
                    new("team", "Portugal", "home_saves", 1.5, "1", 1.91, "over"),
                    new("team", "Portugal", "home_saves", 1.5, "3", 2.10, "over"),
                    new("team", "Wales", "away_corners", 3.5, "3", 2.05, "over"),
                    new("team", "Portugal", "total_goals", 2.5, "4", 1.70, "over")
                }, snapshotId, DateTime.UtcNow);
            }
        }

        var ptHome = new List<FmTeamMatchDraft>
        {
            TeamDraft(18701, fixtureId, kickoff, "home", 18721, "Wales", 1, 0, Stats(5, 7)),
            TeamDraft(18701, "fx-pt-h1", kickoff.AddDays(-4), "home", 18721, "Wales", 2, 1, Stats(4, 8)),
            TeamDraft(18701, "fx-pt-h2", kickoff.AddDays(-8), "home", 18873, "Serbia", 1, 1, Stats(3, 5)),
            TeamDraft(18701, "fx-pt-h3", kickoff.AddDays(-12), "home", 18568, "Greece", 3, 0, Stats(6, 9)),
            TeamDraft(18701, "fx-pt-h4", kickoff.AddDays(-16), "home", 18721, "Wales", 2, 2, Stats(2, 6)),
            TeamDraft(18701, "fx-pt-h5", kickoff.AddDays(-20), "home", 18660, "Germany", 0, 1, Stats(1, 4)),
            TeamDraft(18701, "fx-pt-h6", kickoff.AddDays(-24), "home", 18873, "Serbia", 1, 0, Stats(7, 10))
        };
        var ptAway = new List<FmTeamMatchDraft>
        {
            TeamDraft(18701, "fx-pt-a1", kickoff.AddDays(-2), "away", 18721, "Wales", 1, 1, Stats(3, 6)),
            TeamDraft(18701, "fx-pt-a2", kickoff.AddDays(-6), "away", 18660, "Germany", 0, 2, Stats(2, 5)),
            TeamDraft(18701, "fx-pt-a3", kickoff.AddDays(-10), "away", 18873, "Serbia", 1, 0, Stats(5, 7)),
            TeamDraft(18701, "fx-pt-a4", kickoff.AddDays(-14), "away", 18568, "Greece", 2, 0, Stats(4, 8)),
            TeamDraft(18701, "fx-pt-a5", kickoff.AddDays(-18), "away", 18721, "Wales", 0, 0, Stats(1, 3)),
            TeamDraft(18701, "fx-pt-a6", kickoff.AddDays(-22), "away", 18660, "Germany", 1, 2, Stats(6, 9))
        };
        var waAway = new List<FmTeamMatchDraft>
        {
            TeamDraft(18721, fixtureId, kickoff, "away", 18701, "Portugal", 1, 0, Stats(1, 4)),
            TeamDraft(18721, "fx-wa-a1", kickoff.AddDays(-4), "away", 18660, "Germany", 0, 3, Stats(2, 7)),
            TeamDraft(18721, "fx-wa-a2", kickoff.AddDays(-8), "away", 18643, "Austria", 1, 1, Stats(3, 5)),
            TeamDraft(18721, "fx-wa-a3", kickoff.AddDays(-12), "away", 18654, "Ireland", 2, 0, Stats(2, 9)),
            TeamDraft(18721, "fx-wa-a4", kickoff.AddDays(-16), "away", 18657, "Israel", 1, 2, Stats(4, 6)),
            TeamDraft(18721, "fx-wa-a5", kickoff.AddDays(-20), "away", 18870, "Liechtenstein", 5, 0, Stats(1, 8)),
            TeamDraft(18721, "fx-wa-a6", kickoff.AddDays(-24), "away", 27065, "Lithuania", 3, 1, Stats(2, 5))
        };
        var waHome = new List<FmTeamMatchDraft>
        {
            TeamDraft(18721, "fx-wa-h1", kickoff.AddDays(-2), "home", 18643, "Austria", 1, 0, Stats(3, 6)),
            TeamDraft(18721, "fx-wa-h2", kickoff.AddDays(-6), "home", 18654, "Ireland", 0, 1, Stats(2, 4)),
            TeamDraft(18721, "fx-wa-h3", kickoff.AddDays(-10), "home", 18657, "Israel", 2, 2, Stats(5, 7)),
            TeamDraft(18721, "fx-wa-h4", kickoff.AddDays(-14), "home", 18870, "Liechtenstein", 4, 0, Stats(1, 3)),
            TeamDraft(18721, "fx-wa-h5", kickoff.AddDays(-18), "home", 27065, "Lithuania", 2, 1, Stats(4, 9)),
            TeamDraft(18721, "fx-wa-h6", kickoff.AddDays(-22), "home", 18660, "Germany", 1, 3, Stats(1, 5))
        };

        await store.UpsertTeamMatchesAsync(
            18701, "home", 15, "corners", ptHome, new List<FmPlayerMatchDraft>(),
            "teams/table", DateTime.UtcNow);
        await store.UpsertTeamMatchesAsync(
            18701, "away", 15, "corners", ptAway, new List<FmPlayerMatchDraft>(),
            "teams/table", DateTime.UtcNow);
        await store.UpsertTeamMatchesAsync(
            18721, "home", 15, "corners", waHome, new List<FmPlayerMatchDraft>(),
            "teams/table", DateTime.UtcNow);
        await store.UpsertTeamMatchesAsync(
            18721, "away", 15, "corners", waAway, new List<FmPlayerMatchDraft>(),
            "teams/table", DateTime.UtcNow);
    }

    // ---- seed de jugadores ------------------------------------------------------

    // Snapshot propio (tab player-trends): el loader une el ultimo snapshot de
    // cada tab, asi que las senales de jugador conviven con las de equipo.
    private static async Task<long> SeedPlayersAsync(
        FmSnapshotStore store, string fixtureId,
        IReadOnlyList<FmSignalDraft> signals,
        IReadOnlyList<FmOddsDraft> odds)
    {
        var snapshotId = await store.InsertSnapshotAsync(
            fixtureId, "player-trends",
            $"https://www.footymetrics.com/fixtures/{fixtureId}-p",
            DateTime.UtcNow, "p", "s", "fm-json-v1", "OK");

        if (signals.Count > 0)
            await store.InsertSignalsAsync(
                snapshotId, fixtureId, signals, "all", "all", DateTime.UtcNow,
                """{"location":"all"}""");
        if (odds.Count > 0)
            await store.InsertOddsAsync(fixtureId, odds, snapshotId, DateTime.UtcNow);
        return snapshotId;
    }

    // P13 GAP1 fail-closed: los tests de modelo, candidatos y mercados de
    // jugador siembran ademas un overview PRE-partido (kickoff posterior a la
    // primera captura), que es el estado real de un fixture capturado antes del
    // partido. Sin ese overview el builder cierra la seccion y no habria filas
    // que verificar.
    private static Task<string> SeedPreMatchOverviewAsync(
        FmSnapshotStore store, string fixtureId) =>
        SeedOverviewKickoffAsync(
            store, fixtureId, DateTime.UtcNow.AddHours(2), DateTime.UtcNow);

    // P13 GAP1: snapshot overview persistido con el kickoff del fixture dentro
    // (JSON-LD SportsEvent.startDate), que es como el builder decide pre/post.
    // La marca de tiempo del snapshot es la primera captura del fixture.
    private static async Task<string> SeedOverviewKickoffAsync(
        FmSnapshotStore store, string fixtureId, DateTime kickoffUtc,
        DateTime sourceTimestampUtc)
    {
        var path = Path.Combine(Path.GetTempPath(), $"fmov_{Guid.NewGuid():N}.html");
        var stamp = kickoffUtc.ToString(
            "yyyy-MM-ddTHH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
        await File.WriteAllTextAsync(path,
            "<script type=\"application/ld+json\">{" +
            "\"@context\":\"https://schema.org\",\"@type\":\"SportsEvent\"," +
            $"\"name\":\"{fixtureId} demo\",\"startDate\":\"{stamp}Z\"}}</script>");

        await store.InsertSnapshotAsync(
            fixtureId, "overview",
            $"https://www.footymetrics.com/fixtures/{fixtureId}-demo",
            sourceTimestampUtc, path, "s", "fm-html-v1", "OK");
        return path;
    }

    // values va de mas reciente a mas antiguo (mismo orden que fm_signal).
    private static FmSignalDraft PlayerSignal(
        string player, string market, double line, double[] values,
        string direction = "over", string venueRole = "home") =>
        new("player", player, market, line, direction,
            values.Length, values.Length, 0.5, null, null, null,
            PlayerHistory(values), null, null, null, venueRole);

    private static FmOddsDraft PlayerOdds(
        string player, string market, double line, double odds,
        string direction = "over") =>
        new("player", player, market, line, "3", odds, direction);

    private static string PlayerHistory(double[] values) =>
        "[" + string.Join(",", values.Select(v =>
            $"{{\"v\":{v.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture)}," +
            $"\"met\":true}}")) + "]";
}
