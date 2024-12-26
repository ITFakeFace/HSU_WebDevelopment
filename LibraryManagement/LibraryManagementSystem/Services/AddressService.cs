using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services
{
    public class AddressService
    {
        private readonly LibraryDbContext _ctx;
        public AddressService(LibraryDbContext ctx)
        {
            this._ctx = ctx;
        }

        public async Task<string> GetAddressString(int? AddressId)
        {
            string address = "";
            if (AddressId == null)
                return address;

            //Get Address
            AddressType currentPointAddress = await _ctx.Addresses.Where(o => o.Id == AddressId).FirstOrDefaultAsync();
            address += currentPointAddress.Name;

            //Get Street
            currentPointAddress = await _ctx.Streets.Where(o => o.Id == ((Address)currentPointAddress).Street).FirstOrDefaultAsync();
            address += " Đường " + currentPointAddress.Name;

            //Get Ward
            currentPointAddress = await _ctx.Wards.Where(o => o.Id == ((Street)currentPointAddress).Ward).FirstOrDefaultAsync();
            address += ", P." + currentPointAddress.Name;

            //Get District
            currentPointAddress = await _ctx.Districts.Where(o => o.Id == ((Ward)currentPointAddress).District).FirstOrDefaultAsync();
            address += ", Q." + currentPointAddress.Name;

            //Get City
            currentPointAddress = await _ctx.Cities.Where(o => o.Id == ((District)currentPointAddress).City).FirstOrDefaultAsync();
            address += ", TP." + currentPointAddress.Name;

            return address;
        }

        public async Task<Address> CreateAddress(string cityName, string districtName, string wardName, string streetName, string addressName)
        {
            var city = await _ctx.Cities
                .Include(ct => ct.Districts) // Nạp Districts nếu có
                .ThenInclude(d => d.Wards) // Nạp Wards của mỗi District
                .FirstOrDefaultAsync(ct => ct.Name == cityName);

            if (city == null)
            {
                city = new City { Name = cityName };
                _ctx.Cities.Add(city);
            }

            var district = city.Districts?.FirstOrDefault(dis => dis.Name == districtName);
            if (district == null)
            {
                district = new District { Name = districtName, CityNavigation = city };
                _ctx.Districts.Add(district);
            }

            var ward = district.Wards?.FirstOrDefault(wrd => wrd.Name == wardName);
            if (ward == null)
            {
                ward = new Ward { Name = wardName, DistrictNavigation = district };
                _ctx.Wards.Add(ward);
            }

            var street = await _ctx.Streets.FirstOrDefaultAsync(str => str.Name == streetName && str.Ward == ward.Id);
            if (street == null)
            {
                street = new Street { Name = streetName, WardNavigation = ward };
                _ctx.Streets.Add(street);
            }

            var address = await _ctx.Addresses.FirstOrDefaultAsync(addr => addr.Name == addressName && addr.Street == street.Id);
            if (address == null)
            {
                address = new Address { Name = addressName, StreetNavigation = street };
                _ctx.Addresses.Add(address);
            }

            await _ctx.SaveChangesAsync();
            return address;
        }

    }
}
