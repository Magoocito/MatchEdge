using MatchEdge.Application.Clients;
using MatchEdge.Application.Clients.FootyMetrics;
using MatchEdge.Application.Configuration;
using MatchEdge.Application.Services;
using MatchEdge.Application.UseCases.Calibration;
using MatchEdge.Application.UseCases.Context;
using MatchEdge.Application.UseCases.Lambda;
using MatchEdge.Application.UseCases.Predictions;
using MatchEdge.Application.UseCases.Probability;
using MatchEdge.Application.UseCases.Statistics;
using MatchEdge.Application.UseCases.Teams;
using MatchEdge.Application.UseCases.Backtesting;
using MatchEdge.Application.UseCases.Historical;
using MatchEdge.Application.UseCases.ValueBetting;
using MatchEdge.Application.UseCases.OddsImport;
using MatchEdge.Infrastructure.Clients;
using MatchEdge.Infrastructure.Configuration;
using MatchEdge.Infrastructure.Data;
using MatchEdge.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting.WindowsServices;

var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
};

var builder = WebApplication.CreateBuilder(options);

builder.Host.UseWindowsService(options =>
{
    options.ServiceName = "MatchEdge";
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5272);
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<MatchEdgeDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddMemoryCache();
builder.Services.AddScoped<ISeasonService, SofaScoreBrowserSeasonService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<SofaScoreBrowserClient>();
builder.Services.AddScoped<ISofaScoreClient>(sp =>
    new CachedSofaScoreClient(
        sp.GetRequiredService<SofaScoreBrowserClient>(),
        sp.GetRequiredService<IMemoryCache>(),
        sp.GetRequiredService<MatchCacheTtlResolver>(),
        sp.GetRequiredService<IOptions<SofaScoreCacheOptions>>(),
        sp.GetRequiredService<ILogger<CachedSofaScoreClient>>()));
builder.Services.AddScoped<MatchCacheTtlResolver>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IHomeAdvantageCalibrationService, HomeAdvantageCalibrationService>();
builder.Services.AddScoped<IMultiSeasonHomeAdvantageCalibrationService, MultiSeasonHomeAdvantageCalibrationService>();
builder.Services.AddScoped<TeamContextStatisticsCalculator>();
builder.Services.AddScoped<ITeamContextStatisticsService, TeamContextStatisticsService>();
builder.Services.AddScoped<IMatchLambdaCalculator, MatchLambdaCalculator>();
builder.Services.AddScoped<IProbabilityEngine, PoissonProbabilityEngine>();
builder.Services.AddScoped<IMatchPredictionService, MatchPredictionService>();
builder.Services.AddScoped<IHttpRequestExecutor, HttpRequestExecutor>();
builder.Services.AddSingleton<PlaywrightBrowserManager>();
builder.Services.AddSingleton<SofaScoreBrowserCollector>();
builder.Services.AddSingleton<FootyMetricsBrowserManager>();
builder.Services.AddSingleton<FootyMetricsBrowserCollector>();
builder.Services.AddSingleton<BacktestingJobStore>();
builder.Services.AddScoped<ISofaScoreBrowserCollector>(sp => sp.GetRequiredService<SofaScoreBrowserCollector>());
builder.Services.AddScoped<IValueBetCalculator, ValueBetCalculator>();
builder.Services.AddScoped<IBacktestingService, BacktestingService>();
builder.Services.AddScoped<ICalibrationCurveCalculator, CalibrationCurveCalculator>();
builder.Services.AddScoped<IGammaOptimizer, GammaOptimizer>();
builder.Services.AddScoped<IHistoricalMatchEnumerator, HistoricalMatchEnumerator>();
builder.Services.AddScoped<IHistoricalTeamStatisticsProvider, HistoricalTeamStatisticsProvider>();
builder.Services.AddScoped<ICsvOddsParser, CsvOddsParser>();
builder.Services.AddScoped<IHistoricalOddsService, SqlHistoricalOddsService>();
builder.Services.AddScoped<OddsMatchingService>();
builder.Services.AddScoped<IOddsMatchingService>(sp => sp.GetRequiredService<OddsMatchingService>());
builder.Services.AddScoped<IMatchMappingProvider>(sp => sp.GetRequiredService<OddsMatchingService>());
builder.Services.AddScoped<ITeamMappingProvider>(sp => sp.GetRequiredService<OddsMatchingService>());

builder.Services.Configure<SofaScoreOptions>(
    builder.Configuration.GetSection("SofaScore"));

builder.Services.Configure<MatchModelOptions>(
    builder.Configuration.GetSection("MatchModel"));

builder.Services.Configure<SofaScoreCacheOptions>(
    builder.Configuration.GetSection("SofaScoreCache"));

builder.Services.Configure<FootyMetricsOptions>(
    builder.Configuration.GetSection("FootyMetrics"));

builder.Services.AddSingleton<FootyMetricsDomScraper>();
builder.Services.AddScoped<FootyMetricsBrowserClient>();
builder.Services.AddScoped<IFootyMetricsClient>(sp => sp.GetRequiredService<FootyMetricsBrowserClient>());
builder.Services.AddScoped<ITrendPersistenceService, TrendPersistenceService>();
builder.Services.AddScoped<ITrendBacktestingService, TrendBacktestingService>();
builder.Services.AddScoped<IBankrollManager, BankrollManager>();
builder.Services.AddScoped<IOrchestratorService, OrchestratorService>();
builder.Services.AddHostedService<PipelineWorker>();

var app = builder.Build();

// Auto-migrate database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MatchEdgeDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
