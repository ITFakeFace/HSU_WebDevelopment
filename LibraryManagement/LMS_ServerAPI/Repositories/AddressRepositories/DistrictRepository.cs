using LMS_ServerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS_ServerAPI.Repositories.AddressRepositories
{
    public class DistrictRepository : IDistrictRepository
    {
        private readonly LibraryDbContext _context;

        public DistrictRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<District>> GetAllAsync()
        {
            return await _context.Set<District>().ToListAsync();
        }

        public async Task<District?> GetByIdAsync(int id)
        {
            return await _context.Set<District>().FindAsync(id);
        }

        public async Task<IEnumerable<District>> GetDistrictsByCityIdAsync(int cityId)
        {
            return await _context.Set<District>()
                                 .Where(d => d.City == cityId)
                                 .ToListAsync();
        }

        public async Task AddAsync(District district)
        {
            await _context.Set<District>().AddAsync(district);
        }

        public async Task UpdateAsync(District district)
        {
            _context.Set<District>().Update(district);
        }

        public async Task DeleteAsync(int id)
        {
            var district = await GetByIdAsync(id);
            if (district != null)
            {
                _context.Set<District>().Remove(district);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
