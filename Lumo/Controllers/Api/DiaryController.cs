using Lumo.DTOs.DiaryEntry;
using Lumo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Lumo.Helpers;
namespace Lumo.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DiaryController : ControllerBase
    {
        private readonly IDiaryService _service;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DiaryMapper _mapper; 

        public DiaryController(IDiaryService service, UserManager<ApplicationUser> userManager, DiaryMapper mapper)
        {
            _service = service;
            _userManager = userManager;
            _mapper = mapper; 
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = _userManager.GetUserId(User);
            var entries = await _service.GetUserEntriesAsync(userId);
            var result = entries.Select(e => _mapper.MapToReadDto(e)).ToList();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = _userManager.GetUserId(User);
            var entry = await _service.GetEntryByIdAsync(userId, id);
            if (entry == null) return NotFound();
            return Ok(_mapper.MapToReadDto(entry));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDiaryEntryDto dto)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var updated = await _service.UpdateEntryAsync(id, userId, dto);
            if (updated == null) return NotFound();

            return Ok(_mapper.MapToReadDto(updated));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDiaryEntryDto dto)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                var entryDate = dto.EntryDate.Date;
                if (await _service.HasEntryForDateAsync(userId, entryDate))
                    return BadRequest(new { message = "Masz już wpis w pamiętniku dla tej daty." });

                var entry = await _service.CreateEntryAsync(userId, dto);

                return Ok(_mapper.MapToReadDto(entry));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("favorites")]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var entries = await _service.GetUserEntriesAsync(userId);

            var favorites = entries
                .Where(e => e.IsFavorite)
                .OrderByDescending(e => e.EntryDate)
                .Select(e => _mapper.MapToReadDto(e))
                .ToList();

            return Ok(favorites);
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
