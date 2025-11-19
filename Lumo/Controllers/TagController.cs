using Microsoft.AspNetCore.Mvc;

namespace Lumo.Controllers
{
    public class TagController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}