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

            var result = entries.Select(e => new DiaryEntryReadDto
            {
                Id = e.Id,
                Title = e.Title,
                Content = e.Content,
                EntryDate = e.EntryDate,
                MoodRating = e.MoodRating,
                IsFavorite = e.IsFavorite,
                Tags = e.Tags.Select(t => t.CustomName ?? string.Empty).ToList()
            }).ToList();

            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = _userManager.GetUserId(User);
            var entry = await _service.GetEntryByIdAsync(userId, id);
            if (entry == null) return NotFound();
            return Ok(entry);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDiaryEntryDto dto)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var updated = await _service.UpdateEntryAsync(id, userId, dto);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDiaryEntryDto dto)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "User not logged in." });

                var entryDate = dto.EntryDate.Date;
                if (entryDate == default)
                    return BadRequest(new { message = "Data wpisu (EntryDate) jest wymagana." });

                bool alreadyExists = await _service.HasEntryForDateAsync(userId, entryDate);
                if (alreadyExists)
                    return BadRequest(new { message = "Masz już wpis w pamiętniku dla tej daty." });

                var entry = await _service.CreateEntryAsync(userId, dto);

                // Map to DTO before returning
                var result = new DiaryEntryReadDto
                {
                    Id = entry.Id,
                    Title = entry.Title,
                    Content = entry.Content,
                    EntryDate = entry.EntryDate,
                    MoodRating = entry.MoodRating,
                    IsFavorite = entry.IsFavorite,
                    Tags = entry.Tags.Select(t => t.CustomName ?? string.Empty).ToList()
                };

                return Ok(result);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _service.DeleteEntryAsync(id, userId);
            if (!success)
                return NotFound();

            return NoContent();
        }

    }
}
