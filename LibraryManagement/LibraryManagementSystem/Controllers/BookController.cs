using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using LibraryManagementSystem.DTO;
using LibraryManagementSystem.DTO.BookDTO;
using System;
using System.Text.Json.Serialization.Metadata;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers
{
    public class BookController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly LibraryDbContext _context;

        public BookController(LibraryDbContext context, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET list book
        public async Task<IActionResult> Index()
        {
            return _context.Books != null ?
                View(await _context.Books
                .Include(o => o.Authors)
                .Include(o => o.BookLoans)
                .ToListAsync()) :
                Problem("Book not found.");
        }

        public async Task<IActionResult> Detail(int? Id)
        {
            if (Id == null || _context.Books == null)
            {
                return RedirectToAction("Index", "Book");
            }
            var book = await _context.Books
                .Include(a => a.Authors)
                .Include(a => a.BookInBranches)
                    .ThenInclude(bib => bib.LibraryNavigation)
                .Include(a => a.Categories.OrderBy(category => category.Name))
                .Include(a => a.PublisherNavigation)
                .Include(a => a.SeriesNavigation)
                .Include(a => a.BookImgs)
                .Include(a=> a.VendorNavigation)
                .FirstOrDefaultAsync(b => b.Id == Id);
            var user = await _userManager.GetUserAsync(User);
            var roles = await _signInManager.UserManager.GetRolesAsync(user);
            if (book == null || (book.Status == 0 && !roles.Contains("ADMINISTRATOR")))
            {
                return RedirectToAction("Index", "Book");
            }
            return View(book);
        }




        public async Task<IActionResult> Search(string name, string language, int? vendor, int? Publisher, int? publishYear, string version, int? series, int? status)
        {

            if (string.IsNullOrEmpty(name) &&
                string.IsNullOrEmpty(language) &&
                vendor == null &&
                Publisher == null &&
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


        [method: HttpPost]
        public IActionResult Create(CreateBookDTO createBookDTO)
        {
            if ((Regex.IsMatch(createBookDTO.ISBN!, @"^\d{3}-\d-\d{2}-\d{5}-\d$")))
            {
                ViewBag["Error"] = "ISBN Wrong format";
                return View();
            }

            try
            {
                Author author = _context.Authors.FirstOrDefault(e => e.Id == createBookDTO.AuthorId)!;
                Publisher publisher = _context.Publishers.FirstOrDefault(e => e.Id == createBookDTO.AuthorId)!;
                Vendor vendor = _context.Vendors.FirstOrDefault(e => e.Id == createBookDTO.VendorId)!;
                Book book = new Book()
                {
                    Name = createBookDTO.Title,
                    Authors = new List<Author> { author },
                    PublisherNavigation = publisher,
                    Description = createBookDTO.Description,
                    PublishYear = createBookDTO.PublishYear,
                    PageNumber = createBookDTO.PageNumber,
                    Language = createBookDTO.Language,
                    Version = createBookDTO.Version,
                    Series = createBookDTO.SeriesId,
                    Vendor = createBookDTO.VendorId,
                };
                _context.Books.AddAsync(book);
                _context.SaveChanges();
                return Redirect("/");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return View();

        }
        public IActionResult Create()
        {

            return View();

        }

        public IActionResult UploadImage()
        {
            return View();
        }
        

        [HttpGet]
        public IActionResult GetBookImage(int bookId)
        {
            var bookImg = _context.BookImgs.FirstOrDefault(b => b.Book == bookId);

            if (bookImg == null || bookImg.Image == null)
            {
                // Return a placeholder image if no image exists
                var placeholderPath = Path.Combine(Directory.GetCurrentDirectory(), "");
                return PhysicalFile(placeholderPath, "image/png");
            }

            return File(bookImg.Image, "image/jpeg"); // Adjust MIME type as needed
        }

    }
}
