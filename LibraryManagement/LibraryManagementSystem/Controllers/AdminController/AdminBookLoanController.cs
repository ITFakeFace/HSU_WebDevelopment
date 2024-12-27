using LibraryManagementSystem.DTO.BookLoanDTO;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers.AdminController
{
    [Authorize(Roles = "ADMINISTRATOR,LIBRARIAN")]
    public class AdminBookLoanController : Controller
    {
        private readonly LibraryDbContext _ctx;

        public AdminBookLoanController(LibraryDbContext ctx)
        {
            this._ctx = ctx;
        }

        public async Task<IActionResult> Index()
        {
            var loans = await _ctx.BookLoans.Include(bl => bl.BookNavigation).Include(bl => bl.UserNavigation).Include(bl => bl.LibraryNavigation).ToListAsync();
            return View(loans);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Users"] = await _ctx.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "CUSTOMER"))
                .ToListAsync();
            ViewData["Books"] = await _ctx.Books.ToListAsync();
            ViewData["Libraries"] = await _ctx.Libraries.ToListAsync();
            return View();
        }

        public async Task<IActionResult> Create(CreateBookLoanDto loanDto)
        {
            var user = await _ctx.Users.Where(u => u.Id == loanDto.User).FirstOrDefaultAsync();
            // Check User đã mượn sách và tồn tại
            if (user == null)
            {
                ViewData["Error"] = "Không tìm thấy User";
                return View(loanDto);
            }
            else if (user.BookLoans.Last() != null && user.BookLoans.Last().IsReturned == 0)
            {
                ViewData["Error"] = "User đã mượn sách";
                return View(loanDto);
            }
            // Check Library Phải có sách và còn sách
            var library = await _ctx.Libraries.Where(lib => lib.Id == loanDto.Library).FirstOrDefaultAsync();
            if (library == null)
            {
                ViewData["Error"] = "Chi nhánh không tồn tại";
                return View(loanDto);
            }
            else
            {
                var booksInBranch = await _ctx.BookInBranches.Where(bib => bib.Library == library.Id).ToListAsync();
                bool hasBook = false;
                // Check Sách phải có ở chi nhánh và phải còn số lượng > 0
                foreach (var bookInBranch in booksInBranch)
                {
                    if (bookInBranch.Book == loanDto.Book && bookInBranch.Amount > 0)
                    {
                        hasBook = true;
                    }
                }
                if (!hasBook)
                {
                    ViewData["Error"] = "Sách không có ở chi nhánh hoặc đã hết";
                    return View(loanDto);
                }
            }

            var loan = new BookLoan
            {
                User = loanDto.User,
                Library = loanDto.Library,
                Book = loanDto.Book,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now.AddDays(7),
                IsReturned = 0,
                Status = 1,
            };
            _ctx.BookLoans.Add(loan);
            await _ctx.SaveChangesAsync();

            return View(loanDto);
        }
    }
}
