using Microsoft.AspNetCore.Mvc;

namespace Lumo.Controllers
{
    public class DiaryController : Controller
    {
        // GET: /Diary
        public IActionResult Index()
        {
            // Na razie pusta lista
            var model = new List<Lumo.Models.DiaryEntry>();
            return View(model);
        }
    }
}
