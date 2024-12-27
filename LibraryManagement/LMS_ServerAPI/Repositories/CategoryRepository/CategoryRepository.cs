using LMS_ServerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS_ServerAPI.Repositories.CategoryRepository
{
    public class CategoryRepository : ICategoryRepository
    {
        LibraryDbContext _context;
        public CategoryRepository(LibraryDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Category>> GetAll()
        {
            try
            {
                var categories = await _context.Categories.ToListAsync();  // Lấy tất cả dữ liệu
                return categories;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null!;
            }
        }
    }
}
