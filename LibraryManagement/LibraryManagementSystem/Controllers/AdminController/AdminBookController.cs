using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagementSystem.Controllers.AdminController
{
    //[Authorize(Roles = "ADMINISTRATOR")]
    public class AdminBookController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly LibraryDbContext _context;

        public AdminBookController(LibraryDbContext context, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string bookname,
      string? isbn,
      string? language,
      int? vendor,
      int? publisher,
      int? publishYearFrom,
      int? publishYearTo,
      string version,
      int? series,
      string? status,
      string authors,
      List<int> categoryIds)
        {
            var query = _context.Books
                .Include(b => b.PublisherNavigation)
                .Include(b => b.VendorNavigation)
                .Include(b => b.BookImgs)
                .Include(b => b.Categories)
                .Include(b => b.Authors)
                .AsQueryable();
            if (!string.IsNullOrEmpty(bookname))
                query = query.Where(a => a.Name.Contains(bookname));
            if (!string.IsNullOrEmpty(isbn))
                query = query.Where(a => a.Isbn.Contains(isbn));
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
            if (!string.IsNullOrEmpty(status))
            {
                int parsedStatus;
                if (int.TryParse(status, out parsedStatus))
                {
                    query = query.Where(a => a.Status == parsedStatus);
                }
            }
            if (!string.IsNullOrEmpty(version))
            {
                query = query.Where(b => b.Version.Contains(version));
            }

            if (series.HasValue)
            {
                query = query.Where(b => b.Series == series.Value);
            }
            if (publisher.HasValue)
            {
                query = query.Where(b => b.Publisher == publisher.Value);
            }
            if (!string.IsNullOrEmpty(language))
                query = query.Where(a => a.Language.Contains(language));
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
            ViewBag.StatusOptions = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Chọn trạng thái" },
                    new SelectListItem { Value = "1", Text = "Hiện" },
                    new SelectListItem { Value = "0", Text = "Ẩn" },
                },
                "Value",
                "Text",
                status
                );
            if (categoryIds != null && categoryIds.Any())
            {
                query = query.Where(b => categoryIds.All(c => b.Categories.Select(cat => cat.Id).Contains(c)));
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

        public async Task<IActionResult> Deactive(int? Id)
        {
            if (Id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == Id);
            if (book != null)
            {
                if (book.Status == 0)
                {
                    book.Status = 1;
                    _context.SaveChanges();
                }
                else
                {
                    book.Status = 0;
                    _context.SaveChanges();
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? Id)
        {
            var book = _context.Books.FirstOrDefault(m => m.Id == Id);
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

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

            return RedirectToAction("Index");
        }
    }


}
