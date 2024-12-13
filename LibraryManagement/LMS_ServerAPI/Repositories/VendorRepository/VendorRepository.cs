using LMS_ServerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS_ServerAPI.Repositories.VendorRepository
{
    public class VendorRepository : IVendorRepository
    {
        LibraryDbContext _context;
        public VendorRepository(LibraryDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Vendor>> GetAll()
        {
            try
            {
                var vendors = await _context.Vendors.ToListAsync();  // Lấy tất cả dữ liệu
                return vendors;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null!;
            }
        }
    }
}
