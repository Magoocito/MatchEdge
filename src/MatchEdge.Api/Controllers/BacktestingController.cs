using MatchEdge.Application.UseCases.Backtesting;
using MatchEdge.Infrastructure.Clients;
using Microsoft.AspNetCore.Mvc;

namespace MatchEdge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BacktestingController : ControllerBase
{
    private readonly IBacktestingService _backtestingService;
    private readonly BacktestingJobStore _jobStore;
    private readonly ILogger<BacktestingController> _logger;

    public BacktestingController(
        IBacktestingService backtestingService,
        BacktestingJobStore jobStore,
        ILogger<BacktestingController> logger)
    {
        _backtestingService = backtestingService;
        _jobStore = jobStore;
        _logger = logger;
    }

    [HttpPost("run")]
    public IActionResult Run([FromBody] BacktestRequest request)
    {
        var job = _jobStore.CreateJob();

        _ = Task.Run(async () =>
        {
            job.Status = "Running";
            _logger.LogInformation("Backtesting job {JobId} started", job.JobId);

            try
            {
                var progress = new Progress<BacktestProgress>(p =>
                {
                    job.ProcessedMatches = p.ProcessedMatches;
                    job.TotalMatches = p.TotalMatches;
                    job.CurrentMatch = p.CurrentMatch;
                });

                var (summary, details) = await _backtestingService.RunAsync(
                    request.TournamentId,
                    request.FromDate,
                    request.ToDate,
                    request.ExperimentalGamma,
                    request.CalibrationAsOf,
                    request.IncludeB2,
                    request.SeasonLookback,
                    progress);

                job.Summary = summary;
                job.Details = details;
                job.TotalMatches = details.Count;
                job.ProcessedMatches = details.Count;
                job.Status = "Completed";
                job.CompletedAt = DateTime.UtcNow;

                _logger.LogInformation("Backtesting job {JobId} completed. {Count} matches",
                    job.JobId, details.Count);
            }
            catch (Exception ex)
            {
                job.Status = "Failed";
                job.ErrorMessage = ex.Message;
                job.CompletedAt = DateTime.UtcNow;
                _logger.LogError(ex, "Backtesting job {JobId} failed", job.JobId);
            }
        });

        return Ok(new
        {
            jobId = job.JobId,
            status = job.Status,
            message = "Backtesting started. Poll GET /api/backtesting/status/{jobId} for progress."
        });
    }

    [HttpGet("status/{jobId}")]
    public IActionResult Status(string jobId)
    {
        var job = _jobStore.GetJob(jobId);
        if (job == null)
            return NotFound(new { error = $"Job {jobId} not found" });

        return Ok(new
        {
            jobId = job.JobId,
            status = job.Status,
            totalMatches = job.TotalMatches,
            processedMatches = job.ProcessedMatches,
            currentMatch = job.CurrentMatch,
            startedAt = job.StartedAt,
            completedAt = job.CompletedAt,
            elapsed = job.CompletedAt.HasValue
                ? (job.CompletedAt.Value - job.StartedAt).TotalSeconds
                : (DateTime.UtcNow - job.StartedAt).TotalSeconds,
            errorMessage = job.ErrorMessage
        });
    }

    [HttpGet("result/{jobId}")]
    public IActionResult Result(string jobId)
    {
        var job = _jobStore.GetJob(jobId);
        if (job == null)
            return NotFound(new { error = $"Job {jobId} not found" });

        if (job.Status == "Running")
            return Ok(new { status = "Running", message = "Still processing..." });

        if (job.Status == "Failed")
            return StatusCode(500, new { status = "Failed", error = job.ErrorMessage });

        return Ok(new
        {
            summary = job.Summary,
            matchCount = job.Details?.Count ?? 0,
            details = job.Details?.Take(20).ToList()
        });
    }

    [HttpGet("jobs")]
    public IActionResult ListJobs()
    {
        var jobs = _jobStore.GetAllJobs();
        return Ok(jobs.Select(j => new
        {
            jobId = j.JobId,
            status = j.Status,
            totalMatches = j.TotalMatches,
            startedAt = j.StartedAt,
            completedAt = j.CompletedAt
        }));
    }
}

public class BacktestRequest
{
    public int TournamentId { get; set; } = 406;
    public DateTime FromDate { get; set; } = DateTime.UtcNow.AddMonths(-3);
    public DateTime ToDate { get; set; } = DateTime.UtcNow;
    public double ExperimentalGamma { get; set; } = 1.58;
    public DateTime CalibrationAsOf { get; set; } = DateTime.UtcNow;
    public bool IncludeB2 { get; set; } = true;
    public int SeasonLookback { get; set; } = 2;
}
