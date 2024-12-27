using LibraryManagementSystem.DTO.LibraryDTO;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LibraryManagementSystem.Controllers.AdminController
{
    [Authorize(Roles = "ADMINISTRATOR")]
    public class AdminLibraryController : Controller
    {
        private readonly LibraryDbContext _ctx;
        private readonly AddressService _addressService;
        public AdminLibraryController(LibraryDbContext ctx, AddressService addressService)
        {
            this._ctx = ctx;
            this._addressService = addressService;
        }
        public async Task<IActionResult> Index()
        {
            var libs = await _ctx.Libraries.ToListAsync();
            List<LibraryDto> libsList = new List<LibraryDto>();
            foreach (var lib in libs)
            {
                libsList.Add(new LibraryDto
                {
                    Id = lib.Id,
                    Name = lib.Name,
                    Address = await _addressService.GetAddressString(lib.Address),
                    OpenFrom = lib.OpenFrom,
                    OpenTo = lib.OpenTo,
                    Phone = lib.Phone,
                    Status = lib.Status,
                });
            }
            return View(libsList);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Cities"] = await _ctx.Cities.ToListAsync();
            ViewData["Managers"] = await _ctx.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "ADMINISTRATOR" || ur.Role.Name == "LIBRARIAN"))
                .ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateLibraryDto libraryDto)
        {
            Console.WriteLine(JsonConvert.SerializeObject(libraryDto));
            Library lib = new Library
            {
                Name = libraryDto.Name,
                Phone = libraryDto.Phone,
                OpenFrom = libraryDto.OpenFrom,
                OpenTo = libraryDto.OpenTo,
                Status = 0,
                Manager = libraryDto.Manager,
            };
            Address addr = await _addressService.CreateAddress(
                libraryDto.City,
                libraryDto.District,
                libraryDto.Ward,
                libraryDto.Street,
                libraryDto.Address
            );
            lib.Address = addr.Id;

            _ctx.Add(lib);
            await _ctx.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
