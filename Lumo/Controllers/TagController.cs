using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lumo.Controllers
{
    [Authorize]
    public class TagController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}