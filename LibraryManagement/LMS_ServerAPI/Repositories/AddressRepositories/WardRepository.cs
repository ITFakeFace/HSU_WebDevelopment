using LMS_ServerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS_ServerAPI.Repositories.AddressRepositories
{
    public class WardRepository : IWardRepository
    {
        private readonly LibraryDbContext _context;

        public WardRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ward>> GetAllAsync()
        {
            return await _context.Set<Ward>().ToListAsync();
        }

        public async Task<Ward?> GetByIdAsync(int id)
        {
            return await _context.Set<Ward>().FindAsync(id);
        }

        public async Task<IEnumerable<Ward>> GetWardsByDistrictIdAsync(int districtId)
        {
            return await _context.Set<Ward>()
                                 .Where(w => w.District == districtId)
                                 .ToListAsync();
        }

        public async Task AddAsync(Ward ward)
        {
            await _context.Set<Ward>().AddAsync(ward);
        }

        public async Task UpdateAsync(Ward ward)
        {
            _context.Set<Ward>().Update(ward);
        }

        public async Task DeleteAsync(int id)
        {
            var ward = await GetByIdAsync(id);
            if (ward != null)
            {
                _context.Set<Ward>().Remove(ward);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
