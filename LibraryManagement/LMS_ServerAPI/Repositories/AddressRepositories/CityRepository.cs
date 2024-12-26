using LMS_ServerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS_ServerAPI.Repositories.AddressRepositories
{
    public class CityRepository : ICityRepository
    {
        private readonly LibraryDbContext _context;

        public CityRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<City>> GetAllAsync()
        {
            return await _context.Set<City>().ToListAsync();
        }

        public async Task<City?> GetByIdAsync(int id)
        {
            return await _context.Set<City>().FindAsync(id);
        }

        public async Task<City?> GetCityByNameAsync(string name)
        {
            return await _context.Set<City>()
                                 .FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task AddAsync(City city)
        {
            await _context.Set<City>().AddAsync(city);
        }

        public async Task UpdateAsync(City city)
        {
            _context.Set<City>().Update(city);
        }

        public async Task DeleteAsync(int id)
        {
            var city = await GetByIdAsync(id);
            if (city != null)
            {
                _context.Set<City>().Remove(city);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
