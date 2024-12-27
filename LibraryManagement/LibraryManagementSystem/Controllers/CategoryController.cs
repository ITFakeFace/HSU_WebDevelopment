using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "ADMINISTRATOR")]
    public class CategoryController : Controller
    {
        private readonly LibraryDbContext _context;

        public CategoryController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Category
        public async Task<IActionResult> Index(string Id, string Name, string Status, int pageNumber = 1, int pageSize = 10)
        {
            var category = _context.Categories.AsQueryable();
            if (!string.IsNullOrEmpty(Id))
            {
                int parsedId;
                if (int.TryParse(Id, out parsedId))
                {
                    category = category.Where(a => a.Id == parsedId);
                }
            }
            if (!string.IsNullOrEmpty(Name))
                category = category.Where(a => a.Name.Contains(Name));

            if (!string.IsNullOrEmpty(Status))
            {
                // Chuyển đổi chuỗi Status thành số để so sánh
                if (int.TryParse(Status, out int statusValue))
                {
                    category = category.Where(a => a.Status == statusValue);
                }
            }

            var paginatedCategory = await category
                .OrderBy(a => a.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(a => a.Books)
                .ToListAsync();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)category.Count() / pageSize);
            ViewBag.TotalCategory = (double)category.Count();

            ViewData["Id"] = Id;
            ViewData["Name"] = Name;
            ViewData["Status"] = Status;


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


            return View(paginatedCategory);

        }



        [HttpPost]
        [Route("Category/ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(a => a.Id == id);

            if (category != null)
            {
                // Nếu trạng thái là null, mặc định chuyển thành 1 (Kích hoạt)
                category.Status = category.Status == null || category.Status == 0 ? 1 : 0;

                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                // Trả về trạng thái mới dưới dạng JSON
                return Json(new { success = true, status = category.Status });
            }

            return Json(new { success = false, message = "Không tìm thấy mục." });
        }

        [Route("Category/DetailCategory")]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(a => a.Books) // Nạp các Book liên quan
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }


            return View("DetailCategory", category);
        }


        // GET: Category/CreateCategory
        [Route("Category/CreateCategory")]
        public IActionResult Create()
        {
            return View("CreateCategory"); // Trả về view cụ thể
        }

        // POST: Category/CreateCategory
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Category/CreateCategory")]
        public async Task<IActionResult> Create([Bind("Id,Name,Status")] Category Category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(Category);
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
            return View("CreateCategory", Category);
        }


        // GET: Category/EditCategory/5
        [Route("Category/EditCategory")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Category = await _context.Categories.FindAsync(id);
            if (Category == null)
            {
                return NotFound();
            }


            ViewBag.StatusOptions = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Text = "Đang hoạt động" },
                    new SelectListItem { Value = "0", Text = "Ngừng hoạt động" }
                },
                "Value",
                "Text",
                Category.Status
                );
            return View("EditCategory", Category);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Category/EditCategory")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Status")] Category Category)
        {
            if (id != Category.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Category);
                    await _context.SaveChangesAsync();
                    // Sau khi lưu thành công, chuyển hướng đến trang Index
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(Category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // Ghi log lỗi khi có lỗi đồng thời (concurrency error)
                        ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật. Vui lòng thử lại.");
                        Console.WriteLine("Concurrency error: " + Category.Id); // Ghi log lỗi
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

            ViewBag.StatusOptions = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Text = "Đang hoạt động" },
                    new SelectListItem { Value = "0", Text = "Ngừng hoạt động" }
                },
                "Value",
                "Text",
                Category.Status
                );
            return View("EditCategory", Category);
        }

        // GET: Category/DeleteCategory/5
        [Route("Category/DeleteCategory")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(a => a.Books) // Nạp các Book liên quan
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            // Kiểm tra nếu tác giả có sách liên kết
            if (category.Books.Any())
            {
                ViewBag.ErrorMessage = $"Không thể xóa thể loại '{category.Name}' vì có sách liên kết:";
            }

            return View("DeleteCategory", category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Category/DeleteCategory")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories
                .Include(a => a.Books) // Nạp các Book liên quan
                .FirstOrDefaultAsync(a => a.Id == id);

            if (category != null)
            {
                if (category.Books.Any()) // Kiểm tra nếu có liên kết với Book
                {
                    category.Status = 0; // Cập nhật trạng thái thành 0
                    _context.Categories.Update(category);
                }
                else
                {
                    _context.Categories.Remove(category); // Xóa nếu không có liên kết
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
