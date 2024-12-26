using LMS_ServerAPI.Models;
using LMS_ServerAPI.Repositories.AddressRepositories;
using Microsoft.EntityFrameworkCore;

namespace LMS_ServerAPI.Repositories.AddressRepositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly LibraryDbContext _context;

        public AddressRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Address>> GetAllAsync()
        {
            return await _context.Set<Address>().ToListAsync();
        }

        public async Task<Address?> GetByIdAsync(int id)
        {
            return await _context.Set<Address>().FindAsync(id);
        }

        public async Task<IEnumerable<Address>> GetAddressesByStreetIdAsync(int streetId)
        {
            return await _context.Set<Address>()
                                 .Where(a => a.Street == streetId)
                                 .ToListAsync();
        }

        public async Task AddAsync(Address address)
        {
            await _context.Set<Address>().AddAsync(address);
        }

        public async Task UpdateAsync(Address address)
        {
            _context.Set<Address>().Update(address);
        }

        public async Task DeleteAsync(int id)
        {
            var address = await GetByIdAsync(id);
            if (address != null)
            {
                _context.Set<Address>().Remove(address);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
