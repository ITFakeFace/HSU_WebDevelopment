using LMS_ServerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS_ServerAPI.Repositories.AddressRepositories
{
    public class StreetRepository : IStreetRepository
    {
        private readonly LibraryDbContext _context;

        public StreetRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Street>> GetAllAsync()
        {
            return await _context.Set<Street>().ToListAsync();
        }

        public async Task<Street?> GetByIdAsync(int id)
        {
            return await _context.Set<Street>().FindAsync(id);
        }

        public async Task<IEnumerable<Street>> GetStreetsByWardIdAsync(int wardId)
        {
            return await _context.Set<Street>()
                                 .Where(s => s.Ward == wardId)
                                 .ToListAsync();
        }

        public async Task AddAsync(Street street)
        {
            await _context.Set<Street>().AddAsync(street);
        }

        public async Task UpdateAsync(Street street)
        {
            _context.Set<Street>().Update(street);
        }

        public async Task DeleteAsync(int id)
        {
            var street = await GetByIdAsync(id);
            if (street != null)
            {
                _context.Set<Street>().Remove(street);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
