using LMS_ServerAPI.Models;

namespace LMS_ServerAPI.Services.AgeService
{
    public interface IAgeService
    {
        Task<IEnumerable<Age>> GetAges();
    }
}
