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
    public class AgeController : Controller
    {
        private readonly LibraryDbContext _context;

        public AgeController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Age
        [Route("Age")]
        public async Task<IActionResult> Index(string Id, int FromAge, int ToAge)
        {
            var ages = _context.Ages.AsQueryable();

            var paginatedAge = await ages
                .OrderBy(a => a.FromAge)
                .Include(b => b.Books)
                .ToListAsync();

            ViewBag.TotalAge = (double)ages.Count();

            return View(paginatedAge);

        }



        // GET: Age/CreateAge
        [Route("Age/CreateAge")]
        public IActionResult Create()
        {
            return View("CreateAge"); // Trả về view cụ thể
        }

        // POST: Age/CreateAge
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Age/CreateAge")]
        public async Task<IActionResult> Create([Bind("Id,FromAge,ToAge")] Age Age)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(Age);
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
            return View("CreateAge", Age);
        }


        // GET: Age/EditAge/5
        [Route("Age/EditAge")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Age = await _context.Ages.FindAsync(id);
            if (Age == null)
            {
                return NotFound();
            }

            return View("EditAge", Age);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Age/EditAge")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FromAge,ToAge")] Age Age)
        {
            if (id != Age.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Age);
                    await _context.SaveChangesAsync();
                    // Sau khi lưu thành công, chuyển hướng đến trang Index
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgeExists(Age.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // Ghi log lỗi khi có lỗi đồng thời (concurrency error)
                        ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật. Vui lòng thử lại.");
                        Console.WriteLine("Concurrency error: " + Age.Id); // Ghi log lỗi
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
            
            return View("EditAge", Age);
        }

        // GET: Age/DeleteAge/5
        [Route("Age/DeleteAge")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var age = await _context.Ages
                .Include(b => b.Books) // Nạp các Book liên quan
                .FirstOrDefaultAsync(m => m.Id == id);
            if (age == null)
            {
                return NotFound();
            }

            // Kiểm tra nếu tác giả có sách liên kết
            if (age.Books.Any())
            {
                ViewBag.ErrorMessage = $"Không thể xóa độ tuổi '{age.FromAge}' đến '{age.ToAge}' vì có sách liên kết:";
            }

            return View("DeleteAge", age);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Age/DeleteAge")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var age = await _context.Ages
                .Include(a => a.Books) // Nạp các Book liên quan
                .FirstOrDefaultAsync(a => a.Id == id);

            if (age != null)
            {
                if (age.Books.Any()) // Kiểm tra nếu có liên kết với Book
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _context.Ages.Remove(age); // Xóa nếu không có liên kết
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




        private bool AgeExists(int id)
        {
            return _context.Ages.Any(e => e.Id == id);
        }
    }
}
