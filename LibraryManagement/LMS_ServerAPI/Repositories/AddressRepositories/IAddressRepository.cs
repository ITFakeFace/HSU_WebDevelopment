using LMS_ServerAPI.Models;

namespace LMS_ServerAPI.Repositories.AddressRepositories
{
    public interface IAddressRepository
    {
        Task<IEnumerable<Address>> GetAllAsync();
        Task<Address?> GetByIdAsync(int id);
        Task<IEnumerable<Address>> GetAddressesByStreetIdAsync(int streetId);
        Task AddAsync(Address address);
        Task UpdateAsync(Address address);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }
}
