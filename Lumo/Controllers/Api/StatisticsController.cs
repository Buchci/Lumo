using Lumo.Models;
using Lumo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace Lumo.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _service;
        private readonly UserManager<ApplicationUser> _userManager;

        public StatisticsController(
            IStatisticsService service,
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
}