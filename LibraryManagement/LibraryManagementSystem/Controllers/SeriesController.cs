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
    [Authorize(Roles = "ADMINISTRATOR,LIBRARIAN")]
    public class SeriesController : Controller
    {
        private readonly LibraryDbContext _context;

        public SeriesController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Series
        public async Task<IActionResult> Index(string Id, string Name, string Status, int pageNumber = 1, int pageSize = 10)
        {
            var series = _context.Series.AsQueryable();
            if (!string.IsNullOrEmpty(Id))
            {
                int parsedId;
                if (int.TryParse(Id, out parsedId))
                {
                    series = series.Where(a => a.Id == parsedId);
                }
            }
            if (!string.IsNullOrEmpty(Name))
                series = series.Where(a => a.Name.Contains(Name));
            
            if (!string.IsNullOrEmpty(Status))
            {
                // Chuyển đổi chuỗi Status thành số để so sánh
                if (int.TryParse(Status, out int statusValue))
                {
                    series = series.Where(a => a.Status == statusValue);
                }
            }

            var paginatedSeries = await series
                .OrderBy(a => a.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(a => a.Books)
                .ToListAsync();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)series.Count() / pageSize);
            ViewBag.TotalSeries = (double)series.Count();

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

           
            return View(paginatedSeries);

        }

       

        [HttpPost]
        [Route("Series/ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var serie = await _context.Series.FirstOrDefaultAsync(a => a.Id == id);

            if (serie != null)
            {
                // Nếu trạng thái là null, mặc định chuyển thành 1 (Kích hoạt)
                serie.Status = serie.Status == null || serie.Status == 0 ? 1 : 0;

                _context.Series.Update(serie);
                await _context.SaveChangesAsync();

                // Trả về trạng thái mới dưới dạng JSON
                return Json(new { success = true, status = serie.Status });
            }

            return Json(new { success = false, message = "Không tìm thấy mục." });
        }

        [Route("Vendor/DetailVendor")]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendor = await _context.Vendors
                .Include(a => a.Books) // Nạp các Book liên quan
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vendor == null)
            {
                return NotFound();
            }


            return View("DetailVendor", vendor);
        }

        // GET: Series/CreateSeries
        [Route("Series/CreateSeries")]
        public IActionResult Create()
        {
            return View("CreateSeries"); // Trả về view cụ thể
        }

        // POST: Series/CreateSeries
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Series/CreateSeries")]
        public async Task<IActionResult> Create([Bind("Id,Name,Status")] Series Series)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(Series);
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
            return View("CreateSeries", Series);
        }


        // GET: Series/EditSeries/5
        [Route("Series/EditSeries")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Series = await _context.Series.FindAsync(id);
            if (Series == null)
            {
                return NotFound();
            }


            ViewBag.StatusOptions = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Không rõ" },
                    new SelectListItem { Value = "1", Text = "Đang hoạt động" },
                    new SelectListItem { Value = "0", Text = "Ngừng hoạt động" }
                },
                "Value",
                "Text",
                Series.Status
                );
            return View("EditSeries", Series);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Series/EditSeries")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Status")] Series Series)
        {
            if (id != Series.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Series);
                    await _context.SaveChangesAsync();
                    // Sau khi lưu thành công, chuyển hướng đến trang Index
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SeriesExists(Series.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // Ghi log lỗi khi có lỗi đồng thời (concurrency error)
                        ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật. Vui lòng thử lại.");
                        Console.WriteLine("Concurrency error: " + Series.Id); // Ghi log lỗi
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
                    new SelectListItem { Value = "", Text = "Không rõ" },
                    new SelectListItem { Value = "1", Text = "Đang hoạt động" },
                    new SelectListItem { Value = "0", Text = "Ngừng hoạt động" }
                },
                "Value",
                "Text",
                Series.Status
                );
            return View("EditSeries", Series);
        }

        // GET: Series/DeleteSeries/5
        [Route("Series/DeleteSeries")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var series = await _context.Series
                .Include(a => a.Books) // Nạp các Book liên quan
                .FirstOrDefaultAsync(m => m.Id == id);
            if (series == null)
            {
                return NotFound();
            }

            // Kiểm tra nếu tác giả có sách liên kết
            if (series.Books.Any())
            {
                ViewBag.ErrorMessage = $"Không thể xóa bộ sách '{series.Name}' vì có sách liên kết:";
            }

            return View("DeleteSeries", series);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Series/DeleteSeries")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var series = await _context.Series
                .Include(a => a.Books) // Nạp các Book liên quan
                .FirstOrDefaultAsync(a => a.Id == id);

            if (series != null)
            {
                if (series.Books.Any()) // Kiểm tra nếu có liên kết với Book
                {
                    series.Status = 0; // Cập nhật trạng thái thành 0
                    _context.Series.Update(series);
                }
                else
                {
                    _context.Series.Remove(series); // Xóa nếu không có liên kết
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




        private bool SeriesExists(int id)
        {
            return _context.Series.Any(e => e.Id == id);
        }
    }
}
