using Lumo.DTOs.DiaryEntry;
using Lumo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lumo.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DiaryController : ControllerBase
    {
        private readonly DiaryService _service;
        private readonly UserManager<ApplicationUser> _userManager;

        public DiaryController(DiaryService service, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = _userManager.GetUserId(User);
            var entries = await _service.GetUserEntriesAsync(userId);
            return Ok(entries);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDiaryEntryDto dto)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                // <- TU DODAJ PUNKT 2 i 3
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not logged in." });
                }

                var entry = await _service.CreateEntryAsync(userId, dto);
                return Ok(entry);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                    details = ex.StackTrace
                });
            }
        }
    }
}
