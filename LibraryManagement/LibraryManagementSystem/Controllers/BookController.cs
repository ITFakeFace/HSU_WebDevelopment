using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    public class BookController : Controller
    {
        private readonly LibraryDbContext _context;

        public BookController(LibraryDbContext context)
        {
            _context = context;
        }
        
        // GET list book
        public async Task<IActionResult> Index()
        {
            return _context.Books != null ?
                View(await _context.Books.ToListAsync()) :
                Poblem("Book not found.");
        }

        public async Task<IActionResult> Details(int? Id)
        {
            if (Id == null || _context.Books == null)
            {
                return Notfound();
            }
            var book = await _context.Books
                .FirstOrDefaultAsync(b=>b.Id == Id);
            return View();
        }
    }
}
