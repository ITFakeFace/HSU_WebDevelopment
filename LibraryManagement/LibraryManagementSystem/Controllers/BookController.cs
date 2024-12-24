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

        public BookController(LibraryDbContext context, SignInManager<User> signInManager, UserManager<User> userManager,BookImagesService bookImagesService)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _bookImgService = bookImagesService;

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
            if (imageFile != null && imageFile.Length > 0)
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
                var existingBook = _context.Books.FirstOrDefault(b => b.Id == bookId);

                if (existingBook != null)
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
            }


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
