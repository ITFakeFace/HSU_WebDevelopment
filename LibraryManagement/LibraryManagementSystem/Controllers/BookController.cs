using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using System;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.DTO.BookDTO;
using LibraryManagementSystem.DTO;
using System.Text.RegularExpressions;

namespace LibraryManagementSystem.Controllers
{
    public class BookController : Controller
    {
        private readonly LibraryDbContext _context;

        public BookController(LibraryDbContext context)
        {
            _context = context;
        }
        
        // GET list book
        public async Task<IActionResult> Index()
        {
            return _context.Books != null ?
                View(await _context.Books.ToListAsync()) :
                Problem("Book not found.");
        }

        public async Task<IActionResult> Details(int? Id)
        {
            if (Id == null || _context.Books == null)
            {
                return NotFound();
            }
            var book = await _context.Books
                .FirstOrDefaultAsync(b=>b.Id == Id);
            return View();
        }

        [method: HttpPost]
        public IActionResult Create(CreateBookDTO createBookDTO) 
        {
            if((Regex.IsMatch(createBookDTO.ISBN!, @"^\d{3}-\d-\d{2}-\d{5}-\d$"))) 
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
                    Authors = new List<Author> { author},
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
    }
}
