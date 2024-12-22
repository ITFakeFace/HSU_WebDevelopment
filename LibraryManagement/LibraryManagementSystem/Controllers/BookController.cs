using LibraryManagementSystem.DTO;
using LibraryManagementSystem.DTO.BookDTO;
using LibraryManagementSystem.Helper;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO;
using System.Linq;
using System.Linq;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                .Include(a => a.BookImgs)
                .Include(a => a.SeriesNavigation)
                .Include(a => a.VendorNavigation)
                .FirstOrDefaultAsync(b => b.Id == Id);
            var user = await _userManager.GetUserAsync(User);
            var roles = await _signInManager.UserManager.GetRolesAsync(user);
            if (book == null || (book.Status == 0 && !roles.Contains("ADMINISTRATOR")))
            {
                return RedirectToAction("Index", "Book");
            }
            return View(book);
        }
        public bool IncludeAll(List<Author>? source, List<string> dest)
        {
            if (source == null || dest == null)
            {
                return false;
            }

            return dest.All(author => source.Any(a => a.Name.Contains(author)));
        }

        public async Task<IActionResult> Search(
      string name,
      string language,
      int? vendor,
      int? publisher,
      int? publishYearFrom,
      int? publishYearTo,
      string version,
      int? series,
      int? status,
      string authors,
      List<int> categoryIds)
        {
            var query = _context.Books
                .Include(b => b.PublisherNavigation)
                .Include(b => b.VendorNavigation)
                .Include(b => b.BookImgs)
                .Include(b => b.Categories)
                .Include(b => b.Authors)
                 .Where(b => b.Authors.All(a => a.Status == 1))
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(b => b.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(language) && language != "ALL")
            {
                query = query.Where(b => b.Language == language);
            }

            if (vendor.HasValue)
            {
                query = query.Where(b => b.Vendor == vendor.Value);
            }

            if (publishYearFrom.HasValue && publishYearTo.HasValue)
            {
                query = query.Where(b => b.PublishYear >= publishYearFrom.Value && b.PublishYear <= publishYearTo.Value);
            }
            else if (publishYearFrom.HasValue)
            {
                query = query.Where(b => b.PublishYear >= publishYearFrom.Value);
            }
            else if (publishYearTo.HasValue)
            {
                query = query.Where(b => b.PublishYear <= publishYearTo.Value);
            }

            if (!string.IsNullOrEmpty(version))
            {
                query = query.Where(b => b.Version.Contains(version));
            }

            if (series.HasValue)
            {
                query = query.Where(b => b.Series == series.Value);
            }

            // Default to only show books with Status = 1
            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }
            else
            {
                query = query.Where(b => b.Status == 1);
            }

            if (categoryIds != null && categoryIds.Any())
            {
                query = query.Where(b => categoryIds.All(c => b.Categories.Select(cat => cat.Id).Contains(c)));
            }

            var books = await query.ToListAsync();
            var result = new List<Book>();

            if (!string.IsNullOrEmpty(authors))
            {
                var authorList = authors.Split(',').Select(a => a.Trim()).ToList();
                foreach (var book in books)
                {
                    if (IncludeAll(book.Authors.ToList(), authorList))
                    {
                        result.Add(book);
                    }
                }
                books = result;
            }

            if (!books.Any())
            {
                ViewBag.Message = "Không tìm thấy sách nào!";
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();

            return View(books);
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

        [HttpPost]
        public IActionResult UploadImage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile imageFile, int bookId)
        {
            if (bookId <= 0)
            {
                TempData["Message"] = "Invalid Book ID.";
                return RedirectToAction("Index");
            }

            // Check if the book exists
            var book = await _context.Books
                .Include(b => b.BookImgs) // Include related images
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null)
            {
                TempData["Message"] = "Book not found.";
                return RedirectToAction("Index");
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await imageFile.CopyToAsync(ms);
                var imageBytes = ms.ToArray();

                // Check if an image already exists for this book
                var existingBookImg = book.BookImgs.FirstOrDefault();
                if (existingBookImg != null)
                {
                    // Update existing image
                    existingBookImg.Image = imageBytes;
                    _context.BookImgs.Update(existingBookImg);
                }
                else
                {
                    // Add new image
                    var newBookImg = new BookImg
                    {
                        Book = bookId,
                        Image = imageBytes
                    };
                    _context.BookImgs.Add(newBookImg);
                }

                TempData["Message"] = $"Updated image for book ID '{bookId}'.";
                await _context.SaveChangesAsync();
            }
            else
            {
                TempData["Message"] = "No image file selected.";
            }

            return RedirectToAction("Index", "Book");
        }

        [HttpGet]
        public IActionResult GetBookImage(int bookId)
        {
            var bookImg = _context.BookImgs.FirstOrDefault(b => b.Book == bookId);

            if (bookImg == null || bookImg.Image == null)
            {
                // Return a placeholder image if no image exists
                var placeholderPath = Path.Combine(Directory.GetCurrentDirectory(), "~\\assets\\image\\book\\No_Image_Available.jpg");
                return PhysicalFile(placeholderPath, "image/png");
            }

            return File(bookImg.Image, "image/jpeg"); // Adjust MIME type as needed
        }

        public async Task<IActionResult> Index(
      string name,
      string language,
      int? vendor,
      int? publisher,
      int? publishYearFrom,
      int? publishYearTo,
      string version,
      int? series,
      int? status,
      string authors,
      List<int> categoryIds)
        {
            var query = _context.Books
                .Include(b => b.PublisherNavigation)
                .Include(b => b.VendorNavigation)
                .Include(b => b.BookImgs)
                .Include(b => b.Categories)
                .Include(b => b.Authors)
                 .Where(b => b.Authors.All(a => a.Status == 1))
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(b => b.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(language) && language != "ALL")
            {
                query = query.Where(b => b.Language == language);
            }

            if (vendor.HasValue)
            {
                query = query.Where(b => b.Vendor == vendor.Value);
            }

            if (publishYearFrom.HasValue && publishYearTo.HasValue)
            {
                query = query.Where(b => b.PublishYear >= publishYearFrom.Value && b.PublishYear <= publishYearTo.Value);
            }
            else if (publishYearFrom.HasValue)
            {
                query = query.Where(b => b.PublishYear >= publishYearFrom.Value);
            }
            else if (publishYearTo.HasValue)
            {
                query = query.Where(b => b.PublishYear <= publishYearTo.Value);
            }

            if (!string.IsNullOrEmpty(version))
            {
                query = query.Where(b => b.Version.Contains(version));
            }

            if (series.HasValue)
            {
                query = query.Where(b => b.Series == series.Value);
            }

            // Default to only show books with Status = 1
            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }
            else
            {
                query = query.Where(b => b.Status == 1);
            }

            if (categoryIds != null && categoryIds.Any())
            {
                query = query.Where(b => categoryIds.All(c => b.Categories.Select(cat => cat.Id).Contains(c)));
            }

            var books = await query.ToListAsync();
            var result = new List<Book>();

            if (!string.IsNullOrEmpty(authors))
            {
                var authorList = authors.Split(',').Select(a => a.Trim()).ToList();
                foreach (var book in books)
                {
                    if (IncludeAll(book.Authors.ToList(), authorList))
                    {
                        result.Add(book);
                    }
                }
                books = result;
            }

            if (!books.Any())
            {
                ViewBag.Message = "Không tìm thấy sách nào!";
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();

            return View(books);
        }

    }
}
