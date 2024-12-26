using LMS_ServerAPI.Models;

namespace LMS_ServerAPI.Repositories.AddressRepositories
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>> GetAllAsync();
        Task<City?> GetByIdAsync(int id);
        Task<City?> GetCityByNameAsync(string name);
        Task AddAsync(City city);
        Task UpdateAsync(City city);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }
}
