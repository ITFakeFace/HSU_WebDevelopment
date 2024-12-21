using Microsoft.AspNetCore.Mvc;
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
        private readonly LibraryDbContext _context;

        public BookController(LibraryDbContext context)
        {
            _context = context;
        }

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
    

        public bool IncludeAll(List<Author>? source, List<string> dest)
        {
            if (source == null || dest == null)
            {
                return false;
            }

            return dest.All(author => source.Any(a => a.Name.Contains(author)));
        }

        public bool IncludeAll(List<Category>? source, List<string> dest)
        {
            if (source == null || dest == null)
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
                foreach(var book in books)
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
