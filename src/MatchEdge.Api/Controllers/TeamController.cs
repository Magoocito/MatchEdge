using MatchEdge.Application.Services;
using MatchEdge.Domain.Teams;
using MatchEdge.Domain.Tournaments;
using Microsoft.AspNetCore.Mvc;

namespace MatchEdge.Api.Controllers;

[Route("api")]
[ApiController]
public class TeamController : ControllerBase
{
    private readonly ITeamService _teamService;

    public TeamController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpGet("leagues")]
    public IActionResult GetLeagues()
    {
        var leagues = Enum.GetValues(typeof(Tournament))
            .Cast<Tournament>()
            .Select(t => new League
            {
                Id = (int)t,
                Name = t.ToString()
            })
            .ToList();

        return Ok(leagues);
    }

    [HttpGet("teams")]
    public async Task<IActionResult> GetTeams([FromQuery] int tournamentId)
    {
        if (!Enum.IsDefined(typeof(Tournament), tournamentId))
        {
            return BadRequest($"Tournament {tournamentId} is not supported. Use /api/leagues to see available tournaments.");
        }

        var teams = await _teamService.GetTeamsAsync(tournamentId);

        if (teams == null || !teams.Any())
        {
            return NotFound();
        }

        return Ok(teams);
    }
}

public class League
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
