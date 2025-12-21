using Lumo.Models;
using Lumo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatisticsController : ControllerBase
{
    private readonly StatisticsService _service;
    private readonly UserManager<ApplicationUser> _userManager;

    public StatisticsController(
        StatisticsService service,
        UserManager<ApplicationUser> userManager)
    {
        _service = service;
        _userManager = userManager;
    }

    // GET /api/statistics
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var stats = await _service.GetUserStatisticsAsync(userId);
        return Ok(stats);
    }
}
