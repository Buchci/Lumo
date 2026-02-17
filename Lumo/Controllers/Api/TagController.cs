using Lumo.DTOs.Tag;
using Lumo.Models;
using Lumo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Lumo.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _service;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStringLocalizer _localizer;

        public TagController(
            ITagService service,
            UserManager<ApplicationUser> userManager,
            IStringLocalizerFactory localizerFactory)
        {
            _service = service;
            _userManager = userManager;

            // "Tags" to nazwa  plików: Tags_EN.resx i Tags_PL.resx
            var assemblyName = typeof(Program).Assembly.GetName().Name!;
            _localizer = localizerFactory.Create("Tags", assemblyName);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = _userManager.GetUserId(User);
            var tags = await _service.GetUserTagsAsync(userId);

            var result = tags.Select(t => new TagReadDto
            {
                Id = t.Id,
                CustomName = t.CustomName
                             ?? (t.IsGlobal ? _localizer[t.ResourceKey!].Value : t.ResourceKey),
                IsGlobal = t.IsGlobal
            }).ToList();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTagDto dto)
        {
            var userId = _userManager.GetUserId(User);
            var tag = await _service.CreateTagAsync(userId, dto.ResourceKey, dto.CustomName, dto.IsGlobal);
            return Ok(tag);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTagDto dto)
        {
            var userId = _userManager.GetUserId(User);
            var updatedTag = await _service.UpdateTagAsync(id, userId, dto.CustomName);
            if (updatedTag == null) return NotFound();
            return Ok(updatedTag);
        }

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
