using Lumo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Lumo.Services;
namespace Lumo.Controllers
{
    [Authorize]
    public class DiaryController : Controller
    {
        private readonly IDiaryService _service;
        private readonly UserManager<ApplicationUser> _userManager;
        public DiaryController(IDiaryService service, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var model = new List<Lumo.Models.DiaryEntry>();
            return View(model);
        }
        public IActionResult Favorites()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var entry = await _service.GetEntryByIdAsync(userId, id);

            if (entry == null) return NotFound();
            ViewBag.AllTags = await _service.GetAllTagsAsync(userId);
            // Przekazujemy wpis do widoku
            return View(entry);
        }
    }
}
