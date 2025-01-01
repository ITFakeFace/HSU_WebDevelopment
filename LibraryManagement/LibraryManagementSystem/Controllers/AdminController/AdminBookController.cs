using LibraryManagementSystem.DTO.BookDTO;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Packaging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LibraryManagementSystem.Controllers.AdminController
{
    [Authorize(Roles = "ADMINISTRATOR")]
    public class AdminBookController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly LibraryDbContext _context;
        BookImagesService _bookImagesService;

        public AdminBookController(LibraryDbContext context, SignInManager<User> signInManager, UserManager<User> userManager, BookImagesService bookImagesService)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _bookImagesService = bookImagesService;
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
            var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == Id);
            var images = await _context.BookImgs.Where(img => img.Book == book.Id).ToListAsync();
            if (images.Any())
            {
                foreach (var img in images)
                {
                    _context.BookImgs.Remove(img);
                }
            }
            var loans = await _context.BookLoans.Where(loan => loan.Book == book.Id).ToListAsync();
            if (loans.Any())
            {
                TempData["Error"] = "Đã có sách được mượn";
                book.Status = 0;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
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
                List<Category> categories = _context.Categories.Where(e => createBookDTO.CategoriesId.Contains(e.Id)).ToList();
                Publisher publisher = _context.Publishers.FirstOrDefault(e => e.Id == createBookDTO.PublisherID)!;

                Vendor vendor = _context.Vendors.FirstOrDefault(e => e.Id == createBookDTO.VendorId)!;

                // Tạo đối tượng Book
                Book book = new Book()
                {
                    Name = createBookDTO.Title,
                    Authors = authors,
                    Categories = categories,
                    Isbn = createBookDTO.ISBN,
                    PublisherNavigation = publisher,
                    Description = createBookDTO.Description,
                    PublishYear = createBookDTO.PublishYear,
                    PageNumber = createBookDTO.PageNumber,
                    Language = createBookDTO.Language,
                    Version = createBookDTO.Version,
                    Series = createBookDTO.SeriesId,
                    Vendor = createBookDTO.VendorId,
                    Status = 1
                };

                // Thêm Book vào cơ sở dữ liệu
                _context.Books.Add(book);
                await _context.SaveChangesAsync(); // Lưu book để lấy Id

                // Nếu có ảnh, lưu vào bảng BookImg
                // Xử lý ảnh thông qua BookImgService
                if (createBookDTO.BookImgs != null && createBookDTO.BookImgs?.Count() != 0)
                {
                    List<byte[]?> bookImg = await _bookImagesService.ProcessBookImagesAsync(createBookDTO.BookImgs);
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

        public IActionResult Update(int id)
        {
            Console.WriteLine("Id của sách là: " + id);

            // Tìm thông tin cơ bản của sách
            var book = _context.Books
                .Where(e => e.Id == id)
                .FirstOrDefault();

            // Tải các thông tin liên quan bằng các truy vấn riêng biệt
            var vendorNavigation = _context.Vendors
                .Where(v => v.Id == book.Vendor)
                .FirstOrDefault();

            var publisherNavigation = _context.Publishers
                .Where(p => p.Id == book.Publisher)
                .FirstOrDefault();

            var seriesNavigation = _context.Series
                .Where(s => s.Id == book.Series)
                .FirstOrDefault();

            var authors = _context.Authors
                .Where(a => a.Books.Any(b => b.Id == id))
                .ToList();

            var bookImgs = _context.BookImgs
                .Where(img => img.Book == id)
                .ToList();

            // Đưa dữ liệu vào ViewData
            ViewData["book"] = book;
            ViewData["vendorNavigation"] = vendorNavigation;
            ViewData["publisherNavigation"] = publisherNavigation;
            ViewData["seriesNavigation"] = seriesNavigation;
            ViewData["authors"] = authors;
            ViewData["bookImgs"] = bookImgs;

            return View();
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
                var categories = _context.Categories.Where(e => updateBookDTO.CategoriesId.Contains(e.Id)).ToList();
                var publisher = _context.Publishers.FirstOrDefault(e => e.Id == updateBookDTO.PublisherID);
                var vendor = _context.Vendors.FirstOrDefault(e => e.Id == updateBookDTO.VendorId);

                // Cập nhật thông tin sách
                book.Name = updateBookDTO.Title;
                book.Isbn = updateBookDTO.ISBN;
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
                // Cập nhật tác giả (nếu cần)
                book.Categories.Clear(); // Xóa các tác giả cũ
                book.Categories.AddRange(categories); // Thêm tác giả mới

                // Lưu thay đổi vào cơ sở dữ liệu
                _context.Books.Update(book); // Đánh dấu sách là cần cập nhật
                await _context.SaveChangesAsync();

                if (updateBookDTO.NewBookImgs != null && updateBookDTO.NewBookImgs.Count() > 0)
                {
                    List<byte[]?> bookImg = await _bookImagesService.ProcessBookImagesAsync(updateBookDTO.NewBookImgs);
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


                return RedirectToAction("Index"); // Hoặc trang bạn muốn chuyển hướng
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return View();
            }
        }
    }


}
