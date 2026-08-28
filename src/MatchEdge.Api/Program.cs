using MatchEdge.Application.Clients;
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
using MatchEdge.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

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
builder.Services.AddSingleton<BacktestingJobStore>();
builder.Services.AddScoped<ISofaScoreBrowserCollector>(sp => sp.GetRequiredService<SofaScoreBrowserCollector>());
builder.Services.AddScoped<IValueBetCalculator, ValueBetCalculator>();
builder.Services.AddScoped<IBacktestingService, BacktestingService>();
builder.Services.AddScoped<ICalibrationCurveCalculator, CalibrationCurveCalculator>();
builder.Services.AddScoped<IGammaOptimizer, GammaOptimizer>();
builder.Services.AddScoped<IHistoricalMatchEnumerator, HistoricalMatchEnumerator>();
builder.Services.AddScoped<IHistoricalTeamStatisticsProvider, HistoricalTeamStatisticsProvider>();
builder.Services.AddScoped<ICsvOddsParser, CsvOddsParser>();
builder.Services.AddSingleton<IHistoricalOddsService, HistoricalOddsService>();

builder.Services.Configure<SofaScoreOptions>(
    builder.Configuration.GetSection("SofaScore"));

builder.Services.Configure<MatchModelOptions>(
    builder.Configuration.GetSection("MatchModel"));

builder.Services.Configure<SofaScoreCacheOptions>(
    builder.Configuration.GetSection("SofaScoreCache"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
