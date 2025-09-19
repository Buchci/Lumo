using Lumo.Models;
using Lumo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Lumo.DTOs.Tag;

namespace Lumo.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly TagService _service;
        private readonly UserManager<ApplicationUser> _userManager;

        public TagController(TagService service, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        // GET: pobranie wszystkich tagów dla użytkownika + globalne
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = _userManager.GetUserId(User);
            var tags = await _service.GetUserTagsAsync(userId);

            // mapowanie do DTO
            var result = tags.Select(t => new TagReadDto
            {
                Id = t.Id,
                CustomName = t.CustomName,
                IsGlobal = t.IsGlobal
            }).ToList();

            return Ok(result);
        }

        // POST: dodanie nowego tagu
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTagDto dto)
        {
            var userId = _userManager.GetUserId(User);
            var tag = await _service.CreateTagAsync(userId, dto.ResourceKey, dto.CustomName, dto.IsGlobal);
            return Ok(tag);
        }

        // PUT: edycja istniejącego tagu
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTagDto dto)
        {
            var userId = _userManager.GetUserId(User);
            var updatedTag = await _service.UpdateTagAsync(id, userId, dto.CustomName);
            if (updatedTag == null) return NotFound();
            return Ok(updatedTag);
        }

        // DELETE: usunięcie tagu
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var deleted = await _service.DeleteTagAsync(id, userId);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }

}
