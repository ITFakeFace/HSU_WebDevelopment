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
    }
}
