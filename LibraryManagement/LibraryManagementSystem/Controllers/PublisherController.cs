using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;

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

        // Index: List Publishers
        public async Task<IActionResult> Index(string? Name, int? Status, int pageNumber = 1, int pageSize = 10)
        {
            var publishers = _context.Publishers.AsQueryable();

            // Lọc theo tên nếu có
            if (!string.IsNullOrEmpty(Name))
                publishers = publishers.Where(p => p.Name.Contains(Name));

            // Lọc theo trạng thái nếu có
            if (Status.HasValue)
                publishers = publishers.Where(p => p.Status == Status);

            var paginatedPublishers = await publishers
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Truyền giá trị lọc vào ViewData
            ViewData["Name"] = Name;
            ViewData["Status"] = Status?.ToString();
            ViewData["PageNumber"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)publishers.Count() / pageSize);

            return View(paginatedPublishers);
        }


        // Create: GET
        public IActionResult Create()
        {
            return View();
        }

        // Create: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, Name, Parent, Status")] Publisher publisher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(publisher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(publisher);
        }

        // Edit: GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null) return NotFound();

            return View(publisher);
        }

        // Edit: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Name, Parent, Status")] Publisher publisher)
        {
            if (id != publisher.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(publisher);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublisherExists(publisher.Id)) return NotFound();
                    else throw;
                }
            }
            return View(publisher);
        }

        // Delete: GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var publisher = await _context.Publishers
                .Include(p => p.Books)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (publisher == null) return NotFound();
            return View(publisher);
        }

        // Delete: POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher != null)
            {
                _context.Publishers.Remove(publisher);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Change Status
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null) return NotFound();

            publisher.Status = (publisher.Status == 1) ? 0 : 1;

            _context.Update(publisher);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool PublisherExists(int id)
        {
            return _context.Publishers.Any(e => e.Id == id);
        }
    }
}
