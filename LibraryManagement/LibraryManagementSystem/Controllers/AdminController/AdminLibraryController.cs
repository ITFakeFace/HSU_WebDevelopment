using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers.AdminController
{
    [Authorize(Roles = "ADMINISTATOR")]
    public class AdminLibraryController : Controller
    {
        private readonly LibraryDbContext _ctx;
        public AdminLibraryController(LibraryDbContext ctx)
        {
            this._ctx = ctx;
        }
        public async Task<IActionResult> Index()
        {
            var libs = await _ctx.Libraries.ToListAsync();
            return View(libs);
        }
    }
}
