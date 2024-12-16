using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "ADMINISTRATOR, LIBRARIAN")]
    public class BookLoanController : Controller
    {
        private readonly LibraryDbContext _context;
        public BookLoanController(LibraryDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return _context.BookLoans != null ?
                View(await _context.BookLoans
                .Include(o=>o.UserNavigation)
                .Include(o=>o.BookNavigation)
                .ToListAsync()) :
                Problem("Book not found.");
        }

        public async Task<IActionResult> DeactiveReturn(int? Id)
        {
            if (Id == null || _context.BookLoans == null)
            {
                return NotFound();
            }

            var bookloan = await _context.BookLoans
                .FirstOrDefaultAsync(m => m.Id == Id);
            if(bookloan !=null)
            {
                if(bookloan.IsReturned == 0)
                {
                    bookloan.IsReturned = 1;
                    _context.SaveChanges();
                }
                else
                {
                    bookloan.IsReturned = 0;
                    _context.SaveChanges();
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "BookLoan");
        }
    }
}
