using MatchEdge.Application.UseCases.Backtesting;
using MatchEdge.Application.UseCases.OddsImport;
using Microsoft.AspNetCore.Mvc;

namespace MatchEdge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BacktestingController : ControllerBase
{
    private readonly IBacktestingService _backtestingService;
    private readonly BacktestingJobStore _jobStore;
    private readonly ICsvOddsParser _csvOddsParser;
    private readonly IHistoricalOddsService _historicalOddsService;
    private readonly ILogger<BacktestingController> _logger;

    public BacktestingController(
        IBacktestingService backtestingService,
        BacktestingJobStore jobStore,
        ICsvOddsParser csvOddsParser,
        IHistoricalOddsService historicalOddsService,
        ILogger<BacktestingController> logger)
    {
        _backtestingService = backtestingService;
        _jobStore = jobStore;
        _csvOddsParser = csvOddsParser;
        _historicalOddsService = historicalOddsService;
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
            details = job.Details,
            gammaResult = job.GammaResult
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

    [HttpPost("odds/upload")]
    public IActionResult UploadOdds(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "No file uploaded or file is empty" });

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { error = "File must be a CSV" });

        string csvContent;
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            csvContent = reader.ReadToEnd();
        }

        var odds = _csvOddsParser.Parse(csvContent);
        if (odds.Count == 0)
            return BadRequest(new { error = "No valid odds data found in CSV" });

        _historicalOddsService.Load(odds);

        _logger.LogInformation("Loaded {Count} odds from CSV upload", odds.Count);

        return Ok(new
        {
            message = $"Successfully loaded {odds.Count} odds records",
            matchCount = odds.Count,
            dateRange = new
            {
                from = odds.Min(o => o.MatchDate).ToString("yyyy-MM-dd"),
                to = odds.Max(o => o.MatchDate).ToString("yyyy-MM-dd")
            }
        });
    }

    [HttpGet("odds")]
    public IActionResult GetOdds([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] int? tournamentId)
    {
        var odds = tournamentId.HasValue
            ? _historicalOddsService.GetByTournament(tournamentId.Value)
            : fromDate.HasValue && toDate.HasValue
                ? _historicalOddsService.GetByDateRange(fromDate.Value, toDate.Value)
                : _historicalOddsService.GetAll();

        return Ok(new
        {
            count = odds.Count,
            odds = odds
        });
    }
}
