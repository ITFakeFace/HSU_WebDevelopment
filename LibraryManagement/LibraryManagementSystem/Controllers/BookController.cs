using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class BookController : Controller
    {
        private readonly LibraryDbContext _context;

        public BookController(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books
                .Include(b => b.PublisherNavigation)
                .Include(b => b.VendorNavigation)
                .Include(b => b.BookImgs)
                .ToListAsync();
            return View(books); 
        }

        public async Task<IActionResult> Search(string name, string language, int? vendor,int? Publisher , int? publishYear, string version, int? series, int? status)
        {
           
            if (string.IsNullOrEmpty(name) &&
                string.IsNullOrEmpty(language) &&
                vendor == null &&
                Publisher==null &&
                publishYear == null &&
                string.IsNullOrEmpty(version) &&
                series == null &&
                status == null)
            {
                ViewBag.Message = "nhập nhanh ";
                return View(new List<Book>());
            }

            var query = _context.Books
                .Include(b => b.PublisherNavigation)
                .Include(b => b.VendorNavigation)
                .Include(b => b.BookImgs)
                .AsQueryable();

            // Lọc theo từng tiêu chí
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(b => b.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(language) && language != "ALL")
            {
                query = query.Where(b => b.Language == language);
            }

            if (vendor != null)
            {
                query = query.Where(b => b.Vendor == vendor);
            }
            if (publishYear != null)
            {
                query = query.Where(b => b.PublishYear == publishYear);
            }
            if (publishYear != null)
            {
                query = query.Where(b => b.PublishYear == publishYear);
            }

            if (!string.IsNullOrEmpty(version))
            {
                query = query.Where(b => b.Version.Contains(version));
            }

            if (series != null)
            {
                query = query.Where(b => b.Series == series);
            }

            if (status != null)
            {
                query = query.Where(b => b.Status == status);
            }

            var result = await query.ToListAsync();

            if (!result.Any())
            {
                ViewBag.Message = "fuck";
            }

            return View(result);
        }

    }
}
