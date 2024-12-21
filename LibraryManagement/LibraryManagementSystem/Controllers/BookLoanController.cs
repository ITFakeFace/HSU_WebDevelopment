using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

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
        public async Task<IActionResult> Index(string  id,string? fullname, string? status, string? isbn, int pagenumber = 1)
        {
            var pagesize = 10;
            var bookloan = _context.BookLoans.AsQueryable();
            if (!string.IsNullOrEmpty(id))
            {
                int parsedId;
                if (int.TryParse(id, out parsedId))
                {
                    bookloan = bookloan.Where(a => a.Id == parsedId);
                }
            }
            if (!string.IsNullOrEmpty(fullname))
                bookloan = bookloan.Where(a => a.UserNavigation.Fullname.Contains(fullname));
            if (!string.IsNullOrEmpty(isbn))
                bookloan = bookloan.Where(a => a.BookNavigation.Isbn.Contains(isbn));
            if (!string.IsNullOrEmpty(status))
            {
                int parsedStatus;
                if (int.TryParse(status, out parsedStatus))
                {
                    bookloan = bookloan.Where(a => a.IsReturned == parsedStatus);
                }
            }
                    var pagedBookloan = await bookloan
            .OrderBy(a => a.UserNavigation.UserName).Reverse()
                .Include(o => o.UserNavigation)
                .Include(o => o.BookNavigation)
                .Skip((pagenumber - 1) * pagesize)
                .Take(pagesize)
                .ToListAsync();

            ViewBag.PageNumber = pagenumber;
            ViewBag.PageSize = pagesize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)bookloan.Count() / pagesize);
            ViewBag.TotalBookloan = (double)bookloan.Count();

            ViewData["Id"] = id;
            ViewData["Fullname"] = fullname;
            ViewData["Status"] = status;
            ViewData["Isbn"] = isbn;

            ViewBag.StatusOptions = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Chọn trạng thái" },
                    new SelectListItem { Value = "1", Text = "Đã trả" },
                    new SelectListItem { Value = "0", Text = "Chưa trả" },
                },
                "Value",
                "Text",
                status
                );

            return View(pagedBookloan);
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
