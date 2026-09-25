using MatchEdge.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MatchEdge.Infrastructure.Services;

public class PipelineWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PipelineWorker> _logger;

    private static readonly TimeSpan PipelineInterval = TimeSpan.FromHours(1);
    private static readonly int PipelineStartHour = 10;
    private static readonly int PipelineEndHour = 22;

    public PipelineWorker(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<PipelineWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var enabled = !string.Equals(
            _configuration["Pipeline:LegacyWorker:Enabled"],
            "false", StringComparison.OrdinalIgnoreCase);
        if (!enabled)
        {
            _logger.LogInformation(
                "PipelineWorker disabled via Pipeline:LegacyWorker:Enabled=false.");
            return;
        }

        _logger.LogInformation(
            "PipelineWorker started. Interval: {Interval}, " +
            "Window: {Start}:00 - {End}:00 UTC",
            PipelineInterval, PipelineStartHour, PipelineEndHour);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.UtcNow;

                if (now.Hour >= PipelineStartHour && now.Hour < PipelineEndHour)
                {
                    _logger.LogInformation(
                        "Running scheduled pipeline at {Time}", now);

                    using var scope = _serviceProvider.CreateScope();
                    var orchestrator = scope.ServiceProvider
                        .GetRequiredService<IOrchestratorService>();

                    var result = await orchestrator.RunDailyPipelineAsync(
                        maxFixtures: 50,
                        topPicksPerFixture: 10,
                        topPicksOverall: 30,
                        ct: stoppingToken);

                    _logger.LogInformation(
                        "Pipeline result: {Success} | {Picks} picks | " +
                        "{Duration:F1}s",
                        result.Success, result.TotalPicksScored,
                        result.Duration.TotalSeconds);
                }
                else
                {
                    _logger.LogDebug(
                        "Outside pipeline window ({Hour}:00 UTC), skipping",
                        now.Hour);
                }

                await Task.Delay(PipelineInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PipelineWorker error");

                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        _logger.LogInformation("PipelineWorker stopped");
    }
}
