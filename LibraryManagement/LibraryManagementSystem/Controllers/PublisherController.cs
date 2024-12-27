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
    public class PublisherController : Controller
    {
        private readonly LibraryDbContext _context;

        public PublisherController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Publisher
        public async Task<IActionResult> Index(string Id, string Name, string Status, int pageNumber = 1, int pageSize = 10)
        {
            var publisher = _context.Publishers.AsQueryable();
            if (!string.IsNullOrEmpty(Id))
            {
                int parsedId;
                if (int.TryParse(Id, out parsedId))
                {
                    publisher = publisher.Where(a => a.Id == parsedId);
                }
            }
            if (!string.IsNullOrEmpty(Name))
                publisher = publisher.Where(a => a.Name.Contains(Name));

            if (!string.IsNullOrEmpty(Status))
            {
                // Chuyển đổi chuỗi Status thành số để so sánh
                if (int.TryParse(Status, out int statusValue))
                {
                    publisher = publisher.Where(a => a.Status == statusValue);
                }
            }

            var paginatedPublisher = await publisher
                .OrderBy(a => a.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(a => a.Books)
                .ToListAsync();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)publisher.Count() / pageSize);
            ViewBag.TotalPublisher = (double)publisher.Count();

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


            return View(paginatedPublisher);

        }



        [HttpPost]
        [Route("Publisher/ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var publisher = await _context.Publishers.FirstOrDefaultAsync(a => a.Id == id);

            if (publisher != null)
            {
                // Nếu trạng thái là null, mặc định chuyển thành 1 (Kích hoạt)
                publisher.Status = publisher.Status == null || publisher.Status == 0 ? 1 : 0;

                _context.Publishers.Update(publisher);
                await _context.SaveChangesAsync();

                // Trả về trạng thái mới dưới dạng JSON
                return Json(new { success = true, status = publisher.Status });
            }

            return Json(new { success = false, message = "Không tìm thấy mục." });
        }

        [Route("Publisher/DetailPublisher")]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers
                .Include(a => a.Books) // Nạp các Book liên quan
                .FirstOrDefaultAsync(m => m.Id == id);
            if (publisher == null)
            {
                return NotFound();
            }


            return View("DetailPublisher", publisher);
        }


        // GET: Publisher/CreatePublisher
        [Route("Publisher/CreatePublisher")]
        public IActionResult Create()
        {
            return View("CreatePublisher"); // Trả về view cụ thể
        }

        // POST: Publisher/CreatePublisher
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Publisher/CreatePublisher")]
        public async Task<IActionResult> Create([Bind("Id,Name,Status")] Publisher Publisher)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(Publisher);
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
            return View("CreatePublisher", Publisher);
        }


        // GET: Publisher/EditPublisher/5
        [Route("Publisher/EditPublisher")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Publisher = await _context.Publishers.FindAsync(id);
            if (Publisher == null)
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
                Publisher.Status
                );
            return View("EditPublisher", Publisher);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Publisher/EditPublisher")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Status")] Publisher Publisher)
        {
            if (id != Publisher.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Publisher);
                    await _context.SaveChangesAsync();
                    // Sau khi lưu thành công, chuyển hướng đến trang Index
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublisherExists(Publisher.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // Ghi log lỗi khi có lỗi đồng thời (concurrency error)
                        ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật. Vui lòng thử lại.");
                        Console.WriteLine("Concurrency error: " + Publisher.Id); // Ghi log lỗi
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
                Publisher.Status
                );
            return View("EditPublisher", Publisher);
        }

        // GET: Publisher/DeletePublisher/5
        [Route("Publisher/DeletePublisher")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers
                .Include(a => a.Books) // Nạp các Book liên quan
                .FirstOrDefaultAsync(m => m.Id == id);
            if (publisher == null)
            {
                return NotFound();
            }

            // Kiểm tra nếu tác giả có sách liên kết
            if (publisher.Books.Any())
            {
                ViewBag.ErrorMessage = $"Không thể xóa NXB '{publisher.Name}' vì có sách liên kết:";
            }

            return View("DeletePublisher", publisher);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Publisher/DeletePublisher")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var publisher = await _context.Publishers
                .Include(a => a.Books) // Nạp các Book liên quan
                .FirstOrDefaultAsync(a => a.Id == id);

            if (publisher != null)
            {
                if (publisher.Books.Any()) // Kiểm tra nếu có liên kết với Book
                {
                    publisher.Status = 0; // Cập nhật trạng thái thành 0
                    _context.Publishers.Update(publisher);
                }
                else
                {
                    _context.Publishers.Remove(publisher); // Xóa nếu không có liên kết
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




        private bool PublisherExists(int id)
        {
            return _context.Publishers.Any(e => e.Id == id);
        }
    }
}
