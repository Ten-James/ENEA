using BusinessLogic.Interfaces;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Main_Api.Controllers.api;

[ApiController]
[Route("api/stats")]
public class StatController : ControllerBase
{
    private readonly ILogger<StatController> _logger;
    private readonly IStatService _statService;

    public StatController(IStatService statService, ILogger<StatController> logger)
    {
        _statService = statService;
        _logger = logger;
    }

    /// <summary>
    ///     Gets all statistics (Admin only).
    /// </summary>
    /// <returns>A dictionary of monthly statistics.</returns>
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Dictionary<string, MonthStatDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Dictionary<string, MonthStatDto>>> GetAllStatsForAdmin()
    {
        _logger.LogInformation("Admin requesting all stats.");
        var stats = await _statService.GetAllStatsAsync();
        return Ok(stats);
    }

    /// <summary>
    ///     Gets statistics for the currently logged-in user.
    /// </summary>
    /// <returns>A dictionary of the user's monthly statistics.</returns>
    [HttpGet("user")]
    [Authorize] // Requires any authenticated user
    [ProducesResponseType(typeof(Dictionary<string, MonthStatDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Dictionary<string, MonthStatDto>>> GetAllStatsForCurrentUser()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("Could not extract or parse user ID from token.");
            return BadRequest("Invalid user ID in token.");
        }

        _logger.LogInformation("User {UserId} requesting their stats.", userId);
        var stats = await _statService.GetAllStatsAsync(userId);
        return Ok(stats);
    }
}