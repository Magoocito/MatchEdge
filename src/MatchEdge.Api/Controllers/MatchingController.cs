using MatchEdge.Infrastructure.Data.Entities;
using MatchEdge.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace MatchEdge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchingController : ControllerBase
{
    private readonly IOddsMatchingService _matchingService;
    private readonly ILogger<MatchingController> _logger;

    public MatchingController(IOddsMatchingService matchingService, ILogger<MatchingController> logger)
    {
        _matchingService = matchingService;
        _logger = logger;
    }

    [HttpGet("teams")]
    public IActionResult GetTeamMappings()
    {
        var mappings = _matchingService.GetAllTeamMappings();
        return Ok(new { count = mappings.Count, mappings });
    }

    [HttpPost("teams")]
    public IActionResult CreateTeamMapping([FromBody] CreateTeamMappingRequest request)
    {
        var existing = _matchingService.FindTeamMapping(request.Source, request.SourceTeamName);
        if (existing != null)
            return Conflict(new { error = "Mapping already exists", existingId = existing.Id });

        _matchingService.SaveTeamMapping(
            request.Source, request.SourceTeamId, request.SourceTeamName,
            request.SofaScoreTeamId, request.SofaScoreTeamName, request.Confidence);

        _logger.LogInformation("Created team mapping: {Source} {SourceName} → SofaScore {SofaScoreName}",
            request.Source, request.SourceTeamName, request.SofaScoreTeamName);

        return Ok(new { message = "Team mapping created" });
    }

    [HttpDelete("teams/{id}")]
    public IActionResult DeleteTeamMapping(int id)
    {
        _matchingService.DeleteTeamMapping(id);
        return Ok(new { message = "Team mapping deleted" });
    }

    [HttpGet("matches")]
    public IActionResult GetMatchMappings()
    {
        var mappings = _matchingService.GetAllMatchMappings();
        return Ok(new { count = mappings.Count, mappings });
    }

    [HttpPost("matches")]
    public IActionResult CreateMatchMapping([FromBody] CreateMatchMappingRequest request)
    {
        var existing = _matchingService.FindMatchMapping(request.Source, request.SourceMatchId);
        if (existing != null)
            return Conflict(new { error = "Mapping already exists", existingId = existing.Id });

        _matchingService.SaveMatchMapping(
            request.Source, request.SourceMatchId, request.SofaScoreEventId,
            request.MatchDate, request.Confidence);

        _logger.LogInformation("Created match mapping: {Source} #{SourceMatchId} → SofaScore #{SofaScoreEventId}",
            request.Source, request.SourceMatchId, request.SofaScoreEventId);

        return Ok(new { message = "Match mapping created" });
    }

    [HttpDelete("matches/{id}")]
    public IActionResult DeleteMatchMapping(int id)
    {
        _matchingService.DeleteMatchMapping(id);
        return Ok(new { message = "Match mapping deleted" });
    }

    [HttpGet("normalize")]
    public IActionResult NormalizeTeamName([FromQuery] string name)
    {
        var normalized = _matchingService.NormalizeTeamName(name);
        return Ok(new { original = name, normalized });
    }
}

public record CreateTeamMappingRequest(
    string Source,
    int SourceTeamId,
    string SourceTeamName,
    int SofaScoreTeamId,
    string SofaScoreTeamName,
    double Confidence = 1.0);

public record CreateMatchMappingRequest(
    string Source,
    int SourceMatchId,
    int SofaScoreEventId,
    DateTime MatchDate,
    double Confidence = 1.0);
