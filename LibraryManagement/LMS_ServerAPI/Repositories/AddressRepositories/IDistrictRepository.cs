using LMS_ServerAPI.Models;

namespace LMS_ServerAPI.Repositories.AddressRepositories
{
    public interface IDistrictRepository
    {
        Task<IEnumerable<District>> GetAllAsync();
        Task<District?> GetByIdAsync(int id);
        Task<IEnumerable<District>> GetDistrictsByCityIdAsync(int cityId);
        Task AddAsync(District district);
        Task UpdateAsync(District district);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }
}
