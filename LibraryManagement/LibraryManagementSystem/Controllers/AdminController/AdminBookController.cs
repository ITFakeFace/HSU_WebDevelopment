using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers.AdminController
{
    [Authorize(Roles = "ADMINISTRATOR")]
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

        public async Task<IActionResult> Index()
        {
            return _context.Books != null ?
                View(await _context.Books
                .Include(o => o.Authors)
                .Include(o => o.BookLoans)
                .ToListAsync()) :
                Problem("Book not found.");
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
            return RedirectToAction("Index","Book");
        }

        public async Task<IActionResult> Delete(int? Id)
        {
            var book = _context.Books.FirstOrDefault(m => m.Id == Id);
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
            }
            return RedirectToAction("Index", "Book");
        }

        public async Task<IActionResult> UploadImage(IFormFile imageFile, int bookId)
        {
            if (bookId <= 0)
            {
                TempData["Message"] = "Invalid Book ID.";
                return RedirectToAction("Index", "Book");
            }

            // Check if the book exists
            var book = await _context.Books
                .Include(b => b.BookImgs) // Include related images
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null)
            {
                TempData["Message"] = "Book not found.";
                return RedirectToAction("Index", "Book");
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
    }


}
