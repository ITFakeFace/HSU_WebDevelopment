using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using Microsoft.IdentityModel.Tokens;

namespace LibraryManagementSystem.Controllers
{
    public class AuthorController : Controller
    {
        private readonly LibraryDbContext _context;

        public AuthorController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Authors
        public async Task<IActionResult> Index(string Id, string Name, string Gender, string Nation, string Status, int pageNumber = 1, int pageSize = 10)
        {
            var authors = _context.Authors.AsQueryable();
            if (!string.IsNullOrEmpty(Id))
            {
                int parsedId;
                if (int.TryParse(Id, out parsedId))
                {
                    authors = authors.Where(a => a.Id == parsedId);
                }
            }
            if (!string.IsNullOrEmpty(Name))
                authors = authors.Where(a => a.Name.Contains(Name));
            if (!string.IsNullOrEmpty(Gender))
            {
                // Chuyển đổi chuỗi Gender thành số để so sánh
                if (int.TryParse(Gender, out int genderValue))
                {
                    authors = authors.Where(a => a.Gender == genderValue);
                }
            }
            if (!string.IsNullOrEmpty(Nation))
                authors = authors.Where(a => a.Nation.Contains(Nation));
            if (!string.IsNullOrEmpty(Status))
            {
                // Chuyển đổi chuỗi Status thành số để so sánh
                if (int.TryParse(Status, out int statusValue))
                {
                    authors = authors.Where(a => a.Status == statusValue);
                }
            }

            var paginatedAuthors = await authors
                .OrderBy(a => a.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)authors.Count() / pageSize);
            ViewBag.TotalBooks = (double)authors.Count();

            ViewData["Id"] = Id;
            ViewData["Name"] = Name;
            ViewData["Gender"] = Gender;
            ViewData["Nation"] = Nation;
            ViewData["Status"] = Status;

            ViewBag.NationOptions = new SelectList(
                new List<SelectListItem>
                {
                            new SelectListItem { Value = "", Text = "Chọn quốc tịch" },
                            new SelectListItem { Value = "00", Text = "Không rõ" },
                            new SelectListItem { Value = "VN", Text = "Vietnam" },
                            new SelectListItem { Value = "EN", Text = "England" },
                            new SelectListItem { Value = "US", Text = "United States" },
                            new SelectListItem { Value = "FR", Text = "France" },
                            new SelectListItem { Value = "JP", Text = "Japan" },
                            new SelectListItem { Value = "KR", Text = "South Korea" },
                            new SelectListItem { Value = "CN", Text = "China" },
                            new SelectListItem { Value = "DE", Text = "Germany" },
                            new SelectListItem { Value = "AU", Text = "Australia" },
                            new SelectListItem { Value = "IT", Text = "Italy" },
                            new SelectListItem { Value = "ES", Text = "Spain" },
                            new SelectListItem { Value = "CA", Text = "Canada" },
                            new SelectListItem { Value = "IN", Text = "India" },
                            new SelectListItem { Value = "BR", Text = "Brazil" }
                },
                "Value",
                "Text",
                Nation
                );

            ViewBag.GenderOptions = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Chọn giới tính" },
                    new SelectListItem { Value = "1", Text = "Nam" },
                    new SelectListItem { Value = "0", Text = "Nữ" },
                },
                "Value",
                "Text",
                Gender
                );

            ViewBag.StatusOptions = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Chọn trạng thái" },
                    new SelectListItem { Value = "1", Text = "Đang hoạt động" },
                    new SelectListItem { Value = "0", Text = "Ngừng hoạt động" }
                },
                "Value",
                "Text",
                Status
                );

            return View(paginatedAuthors);

        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            return View("CreateAuthor"); // Trả về view cụ thể
        }

        // POST: Authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Gender,Nation,Status")] Author Author)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(Author);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi nếu xảy ra vấn đề lưu dữ liệu
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu. Vui lòng thử lại.");
                    Console.WriteLine(ex.Message); // Hoặc ghi log lỗi
                }
            }

            // Hiển thị các lỗi trong ModelState trên giao diện
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage); // Log lỗi (nếu cần)
            }

            // Trả lại form với thông tin đã nhập và thông báo lỗi
            return View("CreateAuthor", Author);
        }


        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Author = await _context.Authors.FindAsync(id);
            if (Author == null)
            {
                return NotFound();
            }

            ViewBag.NationOptions = new SelectList(
                new List<SelectListItem>
                {
                            new SelectListItem { Value = "00", Text = "Select Country" },
                            new SelectListItem { Value = "VN", Text = "Vietnam" },
                            new SelectListItem { Value = "EN", Text = "England" },
                            new SelectListItem { Value = "US", Text = "United States" },
                            new SelectListItem { Value = "FR", Text = "France" },
                            new SelectListItem { Value = "JP", Text = "Japan" },
                            new SelectListItem { Value = "KR", Text = "South Korea" },
                            new SelectListItem { Value = "CN", Text = "China" },
                            new SelectListItem { Value = "DE", Text = "Germany" },
                            new SelectListItem { Value = "AU", Text = "Australia" },
                            new SelectListItem { Value = "IT", Text = "Italy" },
                            new SelectListItem { Value = "ES", Text = "Spain" },
                            new SelectListItem { Value = "CA", Text = "Canada" },
                            new SelectListItem { Value = "IN", Text = "India" },
                            new SelectListItem { Value = "BR", Text = "Brazil" }
                },
                "Value",
                "Text",
                Author.Nation
                );

            ViewBag.GenderOptions = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Text = "Nam" },
                    new SelectListItem { Value = "0", Text = "Nữ" },
                    new SelectListItem { Value = "", Text = "Không tiết lộ" }
                },
                "Value",
                "Text",
                Author.Gender
                );

            ViewBag.StatusOptions = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Không rõ" },
                    new SelectListItem { Value = "1", Text = "Đang hoạt động" },
                    new SelectListItem { Value = "0", Text = "Ngừng hoạt động" }
                },
                "Value",
                "Text",
                Author.Status
                );
            return View("EditAuthor", Author);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Gender,Nation,Status")] Author Author)
        {
            if (id != Author.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Author);
                    await _context.SaveChangesAsync();
                    // Sau khi lưu thành công, chuyển hướng đến trang Index
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(Author.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // Ghi log lỗi khi có lỗi đồng thời (concurrency error)
                        ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật. Vui lòng thử lại.");
                        Console.WriteLine("Concurrency error: " + Author.Id); // Ghi log lỗi
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý bất kỳ lỗi nào khác khi lưu dữ liệu
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu. Vui lòng thử lại.");
                    Console.WriteLine(ex.Message); // Hoặc ghi log chi tiết lỗi
                }
                //return View("EditAuthor",Author);
            }
            ViewBag.CountryOptions = new SelectList(
                new List<SelectListItem>
                {
                            new SelectListItem { Value = "00", Text = "Select Country" },
                            new SelectListItem { Value = "VN", Text = "Vietnam" },
                            new SelectListItem { Value = "EN", Text = "England" },
                            new SelectListItem { Value = "US", Text = "United States" },
                            new SelectListItem { Value = "FR", Text = "France" },
                            new SelectListItem { Value = "JP", Text = "Japan" },
                            new SelectListItem { Value = "KR", Text = "South Korea" },
                            new SelectListItem { Value = "CN", Text = "China" },
                            new SelectListItem { Value = "DE", Text = "Germany" },
                            new SelectListItem { Value = "AU", Text = "Australia" },
                            new SelectListItem { Value = "IT", Text = "Italy" },
                            new SelectListItem { Value = "ES", Text = "Spain" },
                            new SelectListItem { Value = "CA", Text = "Canada" },
                            new SelectListItem { Value = "IN", Text = "India" },
                            new SelectListItem { Value = "BR", Text = "Brazil" }
                },
                "Value",
                "Text",
                Author.Nation
                );

            ViewBag.GenderOptions = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Text = "Nam" },
                    new SelectListItem { Value = "0", Text = "Nữ" },
                    new SelectListItem { Value = "", Text = "Không tiết lộ" }
                },
                "Value",
                "Text",
                Author.Gender
                );

            ViewBag.StatusOptions = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Không rõ" },
                    new SelectListItem { Value = "1", Text = "Đang hoạt động" },
                    new SelectListItem { Value = "0", Text = "Ngừng hoạt động" }
                },
                "Value",
                "Text",
                Author.Status
                );
            return View("EditAuthor", Author);
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return View("DeleteAuthor", author);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books) // Nạp các Book liên quan
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author != null)
            {
                if (author.Books.Any()) // Kiểm tra nếu có liên kết với Book
                {
                    author.Status = 0; // Cập nhật trạng thái thành 0
                    _context.Authors.Update(author);
                }
                else
                {
                    _context.Authors.Remove(author); // Xóa nếu không có liên kết
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}
