using LMS_ServerAPI.Models;
using LMS_ServerAPI.Repositories.VendorRepository;

namespace LMS_ServerAPI.Services.VendorService
{
    public class VendorService : IVendorService
    {
        IVendorRepository _vendorRepository;
        public VendorService(IVendorRepository _repo) 
        {
            _vendorRepository = _repo;
        }

        public async Task<IEnumerable<Vendor>> GetVendors()
        {
            var vendors = await _vendorRepository.GetAll();
            return vendors;
        }
    }
}
