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
    public class VendorController : Controller
    {
        private readonly LibraryDbContext _context;

        public VendorController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: vendors
        [Route("Vendor")]
        public async Task<IActionResult> Index(string Id, string Name, string Email, string Phone, string Status, int pageNumber = 1, int pageSize = 10)
        {
            var vendors = _context.Vendors.AsQueryable();
            if (!string.IsNullOrEmpty(Id))
            {
                int parsedId;
                if (int.TryParse(Id, out parsedId))
                {
                    vendors = vendors.Where(a => a.Id == parsedId);
                }
            }
            if (!string.IsNullOrEmpty(Name))
                vendors = vendors.Where(a => a.Name.Contains(Name));
            if (!string.IsNullOrEmpty(Email))
                vendors = vendors.Where(a => a.Email.Contains(Email));
            if (!string.IsNullOrEmpty(Phone))
                vendors = vendors.Where(a => a.Phone.Contains(Phone));
            if (!string.IsNullOrEmpty(Status))
            {
                // Chuyển đổi chuỗi Status thành số để so sánh
                if (int.TryParse(Status, out int statusValue))
                {
                    vendors = vendors.Where(a => a.Status == statusValue);
                }
            }

            var paginatedVendors = await vendors
                .OrderBy(a => a.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(a => a.Books)
                .ToListAsync();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)vendors.Count() / pageSize);
            ViewBag.TotalVendors = (double)vendors.Count();

            ViewData["Id"] = Id;
            ViewData["Name"] = Name;
            ViewData["Email"] = Email;
            ViewData["Phone"] = Phone;
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

            return View(paginatedVendors);

        }

        [HttpPost]
        [Route("Vendor/ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var vendor = await _context.Vendors.FirstOrDefaultAsync(a => a.Id == id);

            if (vendor != null)
            {
                // Nếu trạng thái là null, mặc định chuyển thành 1 (Kích hoạt)
                vendor.Status = vendor.Status == null || vendor.Status == 0 ? 1 : 0;

                _context.Vendors.Update(vendor);
                await _context.SaveChangesAsync();

                // Trả về trạng thái mới dưới dạng JSON
                return Json(new { success = true, status = vendor.Status });
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

        // GET: Vendors/Create
        [Route("Vendor/CreateVendor")]
        public IActionResult Create()
        {
            return View("CreateVendor"); // Trả về view cụ thể
        }

       

        // POST: Vendors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Vendor/CreateVendor")]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Phone,Status")] Vendor Vendor)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(Vendor);
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
            return View("CreateVendor", Vendor);
        }


        // GET: Vendors/Edit/5
        [Route("Vendor/EditVendor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Vendor = await _context.Vendors.FindAsync(id);
            if (Vendor == null)
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
                Vendor.Status
                );
            return View("EditVendor", Vendor);
        }

        // POST: Vendors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Vendor/EditVendor")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Phone,Status")] Vendor Vendor)
        {
            if (id != Vendor.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Vendor);
                    await _context.SaveChangesAsync();
                    // Sau khi lưu thành công, chuyển hướng đến trang Index
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendorExists(Vendor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // Ghi log lỗi khi có lỗi đồng thời (concurrency error)
                        ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật. Vui lòng thử lại.");
                        Console.WriteLine("Concurrency error: " + Vendor.Id); // Ghi log lỗi
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý bất kỳ lỗi nào khác khi lưu dữ liệu
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu. Vui lòng thử lại.");
                    Console.WriteLine(ex.Message); // Hoặc ghi log chi tiết lỗi
                }
                //return View("EditVendor",Vendor);
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
                Vendor.Status
                );
            return View("EditVendor", Vendor);
        }

        // GET: Vendors/Delete/5
        [Route("Vendor/DeleteVendor")]
        public async Task<IActionResult> Delete(int? id)
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

            // Kiểm tra nếu nhà cung cấp có sách liên kết
            if (vendor.Books.Any())
            {
                ViewBag.ErrorMessage = $"Không thể xóa nhà cung cấp '{vendor.Name}' vì có sách liên kết:";
            }

            return View("DeleteVendor", vendor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Vendor/DeleteVendor")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vendor = await _context.Vendors
                .Include(a => a.Books) // Nạp các Book liên quan
                .FirstOrDefaultAsync(a => a.Id == id);

            if (vendor != null)
            {
                if (vendor.Books.Any()) // Kiểm tra nếu có liên kết với Book
                {
                    vendor.Status = 0; // Cập nhật trạng thái thành 0
                    _context.Vendors.Update(vendor);
                }
                else
                {
                    _context.Vendors.Remove(vendor); // Xóa nếu không có liên kết
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




        private bool VendorExists(int id)
        {
            return _context.Vendors.Any(e => e.Id == id);
        }
    }
}
