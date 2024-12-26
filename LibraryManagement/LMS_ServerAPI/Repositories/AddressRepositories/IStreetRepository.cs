using LMS_ServerAPI.Models;

namespace LMS_ServerAPI.Repositories.AddressRepositories
{
    public interface IStreetRepository
    {
        Task<IEnumerable<Street>> GetAllAsync();
        Task<Street?> GetByIdAsync(int id);
        Task<IEnumerable<Street>> GetStreetsByWardIdAsync(int wardId);
        Task AddAsync(Street street);
        Task UpdateAsync(Street street);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }
}
