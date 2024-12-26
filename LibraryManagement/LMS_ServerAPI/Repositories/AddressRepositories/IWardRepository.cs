using LMS_ServerAPI.Models;

namespace LMS_ServerAPI.Repositories.AddressRepositories
{
    public interface IWardRepository
    {
        Task<IEnumerable<Ward>> GetAllAsync();
        Task<Ward?> GetByIdAsync(int id);
        Task<IEnumerable<Ward>> GetWardsByDistrictIdAsync(int districtId);
        Task AddAsync(Ward ward);
        Task UpdateAsync(Ward ward);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }
}
