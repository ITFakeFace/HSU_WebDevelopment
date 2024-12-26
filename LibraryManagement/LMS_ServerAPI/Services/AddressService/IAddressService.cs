using LMS_ServerAPI.Models;

namespace LMS_ServerAPI.Services.AddressService
{
    public interface IAddressService
    {
        Task<IEnumerable<Address>> GetAllAddressesAsync();
        Task<Address?> GetAddressByIdAsync(int id);
        Task<IEnumerable<Street>> GetStreetsByWardIdAsync(int wardId);
        Task<IEnumerable<Ward>> GetWardsByDistrictIdAsync(int districtId);
        Task<IEnumerable<District>> GetDistrictsByCityIdAsync(int cityId);
        Task<IEnumerable<City>> GetAllCitiesAsync();
        Task AddAddressAsync(Address address);
        Task UpdateAddressAsync(Address address);
        Task DeleteAddressAsync(int id);
    }
}
