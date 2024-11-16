using LMS_ServerAPI.Models;
using LMS_ServerAPI.Repositories.AddressRepositories;

namespace LMS_ServerAPI.Repositories.AddressRepositories
{
    public class AddressRepository : IAddressRepository
    {
        private LibraryDbContext _ctx;
        private IHostEnvironment _env;
    }
}
