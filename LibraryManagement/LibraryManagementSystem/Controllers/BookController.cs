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
using LibraryManagementSystem.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Newtonsoft.Json;
using LibraryManagementSystem.Helper;

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
                return false;
            }

            return dest.All(author => source.Any(a => a.Name.Contains(author)));
        }

        public async Task<IActionResult> Search(string name, string language, int? vendor, int? publisher, int? publishYearFrom, int? publishYearTo, string version, int? series, int? status, string authors, List<int> categoryIds)
        {
            var query = _context.Books
                .Include(b => b.PublisherNavigation)
                .Include(b => b.VendorNavigation)
                .Include(b => b.BookImgs)
                .Include(b => b.Categories)
                .Include(b => b.Authors)
                .AsQueryable();

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

            if (series != null)
            {
                query = query.Where(b => b.Series == series);
            }

            if (status != null)
            {
                query = query.Where(b => b.Status == status);
            }

            if (categoryIds != null && categoryIds.Any())
            {
                query = query.Where(b => categoryIds.All(c => b.Categories.Select(cat => cat.Id).Contains(c)));
            }


            var books = await query.ToListAsync();
            List<Book> result = new List<Book>();

            if (!string.IsNullOrEmpty(authors))
            {
                var authorList = authors.Split(",").Select(a => a.Trim()).ToList();
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
        public async Task<IActionResult> UploadImage(IFormFile imageFile, int bookId)
        {
            if (bookId <= 0)
            {
                TempData["Message"] = "Invalid Book ID.";
                return RedirectToAction("Index");
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await imageFile.CopyToAsync(ms);
                var imageBytes = ms.ToArray();

                var existingBook = _context.Books.FirstOrDefault(b => b.Id == bookId);

                if (existingBook != null)
                {
                    existingBook.Image = imageBytes;
                    _context.Books.Update(existingBook);
                    TempData["Message"] = $"Updated image for book ID '{bookId}'.";
                }
                else
                {
                    TempData["Message"] = $"Book with ID '{bookId}' not found.";
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                TempData["Message"] = "No image file selected.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetBookImage(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book?.Image != null)
            {
                var base64Image = Convert.ToBase64String(book.Image);
                var imgSrc = $"data:image/jpeg;base64,{base64Image}";
                return Content(imgSrc);
            }

            return NotFound("Image not found");
        }

        public async Task<IActionResult> Index(string name, string language, int? vendor, int? publisher, int? publishYearFrom, int? publishYearTo, string version, int? series, int? status, string authors, List<int> categoryIds)
        {
            var query = _context.Books
                 .Include(b => b.PublisherNavigation)
                 .Include(b => b.VendorNavigation)
                 .Include(b => b.BookImgs)
                 .Include(b => b.Categories)
                 .Include(b => b.Authors)
                 .AsQueryable();

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

            if (series != null)
            {
                query = query.Where(b => b.Series == series);
            }

            if (status != null)
            {
                query = query.Where(b => b.Status == status);
            }

            if (categoryIds != null && categoryIds.Any())
            {
                query = query.Where(b => categoryIds.All(c => b.Categories.Select(cat => cat.Id).Contains(c)));
            }


            var books = await query.ToListAsync();
            List<Book> result = new List<Book>();

            if (!string.IsNullOrEmpty(authors))
            {
                var authorList = authors.Split(",").Select(a => a.Trim()).ToList();
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
