using Lumo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lumo.Controllers
{
    public class DiaryController : Controller
    {
        private readonly DiaryService _service;
        private readonly UserManager<ApplicationUser> _userManager;
        // GET: /Diary
        public DiaryController(DiaryService service, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            // Na razie pusta lista
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
            // Przekazujemy wpis do widoku, żeby wypełnić formularz danymi
            return View(entry);
        }
    }
}
