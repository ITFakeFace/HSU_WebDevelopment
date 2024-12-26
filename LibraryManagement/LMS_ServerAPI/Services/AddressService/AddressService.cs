using LMS_ServerAPI.Models;
using LMS_ServerAPI.Repositories.AddressRepositories;

namespace LMS_ServerAPI.Services.AddressService
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IStreetRepository _streetRepository;
        private readonly IWardRepository _wardRepository;
        private readonly IDistrictRepository _districtRepository;
        private readonly ICityRepository _cityRepository;

        public AddressService(
            IAddressRepository addressRepository,
            IStreetRepository streetRepository,
            IWardRepository wardRepository,
            IDistrictRepository districtRepository,
            ICityRepository cityRepository)
        {
            _addressRepository = addressRepository;
            _streetRepository = streetRepository;
            _wardRepository = wardRepository;
            _districtRepository = districtRepository;
            _cityRepository = cityRepository;
        }

        public async Task<IEnumerable<Address>> GetAllAddressesAsync()
        {
            return await _addressRepository.GetAllAsync();
        }

        public async Task<Address?> GetAddressByIdAsync(int id)
        {
            return await _addressRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Street>> GetStreetsByWardIdAsync(int wardId)
        {
            return await _streetRepository.GetStreetsByWardIdAsync(wardId);
        }

        public async Task<IEnumerable<Ward>> GetWardsByDistrictIdAsync(int districtId)
        {
            return await _wardRepository.GetWardsByDistrictIdAsync(districtId);
        }

        public async Task<IEnumerable<District>> GetDistrictsByCityIdAsync(int cityId)
        {
            return await _districtRepository.GetDistrictsByCityIdAsync(cityId);
        }

        public async Task<IEnumerable<City>> GetAllCitiesAsync()
        {
            return await _cityRepository.GetAllAsync();
        }

        public async Task AddAddressAsync(Address address)
        {
            await _addressRepository.AddAsync(address);
            await _addressRepository.SaveAsync();
        }

        public async Task UpdateAddressAsync(Address address)
        {
            await _addressRepository.UpdateAsync(address);
            await _addressRepository.SaveAsync();
        }

        public async Task DeleteAddressAsync(int id)
        {
            await _addressRepository.DeleteAsync(id);
            await _addressRepository.SaveAsync();
        }
    }
}
