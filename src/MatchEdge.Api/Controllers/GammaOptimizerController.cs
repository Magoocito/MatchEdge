using MatchEdge.Application.UseCases.Backtesting;
using Microsoft.AspNetCore.Mvc;

namespace MatchEdge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GammaOptimizerController : ControllerBase
{
    private readonly IGammaOptimizer _gammaOptimizer;
    private readonly BacktestingJobStore _jobStore;
    private readonly ILogger<GammaOptimizerController> _logger;

    public GammaOptimizerController(
        IGammaOptimizer gammaOptimizer,
        BacktestingJobStore jobStore,
        ILogger<GammaOptimizerController> logger)
    {
        _gammaOptimizer = gammaOptimizer;
        _jobStore = jobStore;
        _logger = logger;
    }

    [HttpPost("run")]
    public IActionResult Run([FromBody] GammaOptimizationRequest request)
    {
        var job = _jobStore.CreateJob();

        _ = Task.Run(async () =>
        {
            job.Status = "Running";
            _logger.LogInformation("Gamma optimization job {JobId} started", job.JobId);

            try
            {
                var progress = new Progress<BacktestProgress>(p =>
                {
                    job.ProcessedMatches = p.ProcessedMatches;
                    job.TotalMatches = p.TotalMatches;
                    job.CurrentMatch = p.CurrentMatch;
                });

                var result = await _gammaOptimizer.FindOptimalGammaAsync(
                    request.TournamentId,
                    request.FromDate,
                    request.ToDate,
                    request.GammaMin,
                    request.GammaMax,
                    request.Step,
                    request.SeasonLookback,
                    progress);

                job.GammaResult = result;
                job.TotalMatches = result.Training.GridResults.Count + 3 + 1; // pilot + train + val
                job.ProcessedMatches = job.TotalMatches;
                job.Status = "Completed";
                job.CompletedAt = DateTime.UtcNow;

                _logger.LogInformation("Gamma optimization job {JobId} completed. Optimal gamma={Gamma}, overfitting={Overfit}",
                    job.JobId, result.Training.OptimalGamma, result.Validation.OverfittingDetected);
            }
            catch (Exception ex)
            {
                job.Status = "Failed";
                job.ErrorMessage = ex.Message;
                job.CompletedAt = DateTime.UtcNow;
                _logger.LogError(ex, "Gamma optimization job {JobId} failed", job.JobId);
            }
        });

        return Ok(new
        {
            jobId = job.JobId,
            status = job.Status,
            message = "Gamma optimization started. Poll GET /api/backtesting/status/{jobId} for progress."
        });
    }
}
