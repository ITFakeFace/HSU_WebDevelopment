using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class PublisherController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
