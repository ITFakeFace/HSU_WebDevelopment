using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace LibraryManagementSystem.Controllers
{
    public class BookController : Controller
    {
        private readonly LibraryDbContext _context;

        public BookController(LibraryDbContext context)
        {
            _context = context;
        }

        // Action cho việc tải lên ảnh
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

        // Action cho việc tải ảnh của sách
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

        // Action cho trang Index (không thay đổi)
        public async Task<IActionResult> Index(string name, string language, int? vendor, int? publishYear, string version, int? series, int? status, string authors)
        {
            var query = _context.Books
                .Include(b => b.PublisherNavigation)
                .Include(b => b.VendorNavigation)
                .Include(b => b.BookImgs)
                .Include(b => b.Authors)
                .Include(b => b.SeriesNavigation)
                .AsQueryable();

            // Lọc theo từng tiêu chí (nếu có tham số tìm kiếm)
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(b => b.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(language) && language != "ALL")
            {
                query = query.Where(b => b.Language == language);
            }

            if (!string.IsNullOrEmpty(authors))
            {
                query = query.Where(b => b.Authors.Any(a => a.Name.Contains(authors)));
            }

            if (vendor != null)
            {
                query = query.Where(b => b.Vendor == vendor);
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

            var books = await query.ToListAsync();

            // Hiển thị thông báo nếu không có kết quả
            if (!books.Any())
            {
                ViewBag.Message = "No books found!";
            }

            return View(books);
        }

        // Action tìm kiếm
        public async Task<IActionResult> Search(string name, string language, int? vendor, int? publisher, int? publishYear, string version, int? series, int? status, string authors)
        {
            var query = _context.Books
                .Include(b => b.PublisherNavigation)
                .Include(b => b.VendorNavigation)
                .Include(b => b.BookImgs)
                .Include(b => b.Authors)
                .AsQueryable();

            // Lọc theo từng tiêu chí (nếu có tham số tìm kiếm)
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(b => b.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(language) && language != "ALL")
            {
                query = query.Where(b => b.Language == language);
            }

            if (!string.IsNullOrEmpty(authors))
            {
                query = query.Where(b => b.Authors.Any(a => a.Name.Contains(authors)));
            }

            if (vendor != null)
            {
                query = query.Where(b => b.Vendor == vendor);
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

            var books = await query.ToListAsync();

            // Hiển thị thông báo nếu không có kết quả
            if (!books.Any())
            {
                ViewBag.Message = "No books found!";
            }

            return View(books);
        }
    }
}
