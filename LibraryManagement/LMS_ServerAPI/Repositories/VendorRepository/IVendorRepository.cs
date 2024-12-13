using LMS_ServerAPI.Models;
namespace LMS_ServerAPI.Repositories.VendorRepository
{
    public interface IVendorRepository
    {
        Task<IEnumerable<Vendor>> GetAll();
    }
}
