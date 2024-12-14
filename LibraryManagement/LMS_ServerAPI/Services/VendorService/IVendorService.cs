using LMS_ServerAPI.Models;

namespace LMS_ServerAPI.Services.VendorService
{
    public interface IVendorService
    {
        Task<IEnumerable<Vendor>> GetVendors();
    }
}
