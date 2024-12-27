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
using NuGet.Packaging;
using LibraryManagementSystem.Services;
using Newtonsoft.Json;
using System.Text;
using System.Net.NetworkInformation;

namespace LibraryManagementSystem.Controllers
{
    public class BookController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly LibraryDbContext _context;
        private readonly BookImagesService _bookImgService;

        public BookController(LibraryDbContext context, SignInManager<User> signInManager, UserManager<User> userManager, BookImagesService bookImagesService)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _bookImgService = bookImagesService;

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
            return RedirectToAction("Index", "Book");
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
        public IActionResult Create()
        {
            return View();
        }

        [method: HttpPost]
        public async Task<IActionResult> Create(CreateBookDTO createBookDTO)
        {
            try
            {
                // Lấy các thông tin khác từ DTO
                List<Author> authors = _context.Authors.Where(e => createBookDTO.AuthorId.Contains(e.Id)).ToList();
                Publisher publisher = _context.Publishers.FirstOrDefault(e => e.Id == createBookDTO.PublisherID)!;
                Vendor vendor = _context.Vendors.FirstOrDefault(e => e.Id == createBookDTO.VendorId)!;

                // Tạo đối tượng Book
                Book book = new Book()
                {
                    Name = createBookDTO.Title,
                    Authors = authors,
                    Isbn = createBookDTO.ISBN,
                    PublisherNavigation = publisher,
                    Description = createBookDTO.Description,
                    PublishYear = createBookDTO.PublishYear,
                    PageNumber = createBookDTO.PageNumber,
                    Language = createBookDTO.Language,
                    Version = createBookDTO.Version,
                    Series = createBookDTO.SeriesId,
                    Vendor = createBookDTO.VendorId,
                };

                // Thêm Book vào cơ sở dữ liệu
                _context.Books.Add(book);
                await _context.SaveChangesAsync(); // Lưu book để lấy Id

                // Nếu có ảnh, lưu vào bảng BookImg
                // Xử lý ảnh thông qua BookImgService
                if (createBookDTO.BookImgs != null || createBookDTO.BookImgs?.Count() != 0)
                {
                    List<byte[]?> bookImg = await _bookImgService.ProcessBookImagesAsync(createBookDTO.BookImgs);
                    foreach (var img in bookImg)
                    {
                        BookImg imgTemp = new BookImg()
                        {
                            Book = book.Id,
                            Image = img
                        };
                        _context.BookImgs.Add(imgTemp);
                    }
                    await _context.SaveChangesAsync(); // Lưu thông tin ảnh vào cơ sở dữ liệu    

                }

                return RedirectToAction("Index"); // Hoặc trang bạn muốn chuyển hướng
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                TempData["Error"] = "Đã xảy ra lỗi khi tạo sách.";
                return View();
            }
        }

        public IActionResult Create()
        {

            return View();

        }

        public IActionResult UploadImage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile imageFile, int bookId)
        {
            //Console.WriteLine("Id của sách là: " + id);

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
                var placeholderPath = Path.Combine(Directory.GetCurrentDirectory(), "");
                return PhysicalFile(placeholderPath, "image/png");
            }

            return File(bookImg.Image, "image/jpeg"); // Adjust MIME type as needed
        }


        [method: HttpPost]
        public async Task<IActionResult> Update(UpdateBookDTO updateBookDTO)
        {
            try
            {
                var book = await _context.Books
                    .Include(b => b.Authors)
                    .Include(b => b.BookImgs)
                    .FirstOrDefaultAsync(b => b.Id == updateBookDTO.Id)!;
                _context.BookImgs.RemoveRange(book.BookImgs);


                book.BookImgs.Clear();

                await _context.SaveChangesAsync();

                // Lấy các thông tin liên quan khác
                var authors = _context.Authors.Where(e => updateBookDTO.AuthorId.Contains(e.Id)).ToList();
                var publisher = _context.Publishers.FirstOrDefault(e => e.Id == updateBookDTO.PublisherID);
                var vendor = _context.Vendors.FirstOrDefault(e => e.Id == updateBookDTO.VendorId);

                // Cập nhật thông tin sách
                book.Name = updateBookDTO.Title;
                book.Description = updateBookDTO.Description;
                book.PublishYear = updateBookDTO.PublishYear;
                book.PageNumber = updateBookDTO.PageNumber;
                book.Language = updateBookDTO.Language;
                book.Version = updateBookDTO.Version;
                book.Series = updateBookDTO.SeriesId;
                book.Vendor = updateBookDTO.VendorId;
                book.PublisherNavigation = publisher ?? book.PublisherNavigation;

                // Cập nhật tác giả (nếu cần)
                book.Authors.Clear(); // Xóa các tác giả cũ
                book.Authors.AddRange(authors); // Thêm tác giả mới

                // Lưu thay đổi vào cơ sở dữ liệu
                _context.Books.Update(book); // Đánh dấu sách là cần cập nhật
                await _context.SaveChangesAsync();

                if (updateBookDTO.NewBookImgs != null && updateBookDTO.NewBookImgs.Count() > 0)
                {
                    List<byte[]?> bookImg = await _bookImgService.ProcessBookImagesAsync(updateBookDTO.NewBookImgs);
                    foreach (var img in bookImg)
                    {
                        BookImg imgTemp = new BookImg()
                        {
                            Book = book.Id,
                            Image = img
                        };
                        _context.BookImgs.Add(imgTemp);
                    }
                }

                if (updateBookDTO.OldBookImgs != null)
                {
                    foreach (var img in updateBookDTO.OldBookImgs)
                    {
                        var temp = img.Substring("data:image/png;base64,".Length);
                        byte[] byteArray = Convert.FromBase64String(temp);

                        BookImg imgTemp = new BookImg()
                        {
                            Book = book.Id,
                            Image = byteArray
                        };
                        _context.BookImgs.Add(imgTemp);
                    }

                }
                await _context.SaveChangesAsync(); // Lưu thông tin ảnh vào cơ sở dữ liệu    


                return RedirectToAction("Detail", new { id = book!.Id }); // Hoặc trang bạn muốn chuyển hướng
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return View();
            }
        }


           }
}
