using Lumo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Lumo.DTOs.DiaryEntry;

namespace Lumo.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
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
            var userId = _userManager.GetUserId(User);
            var entry = await _service.CreateEntryAsync(userId, dto);
            return Ok(entry);
        }
    }
}
