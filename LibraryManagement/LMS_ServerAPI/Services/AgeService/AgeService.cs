using LMS_ServerAPI.Models;
using LMS_ServerAPI.Repositories.AgeRepositories;

namespace LMS_ServerAPI.Services.AgeService
{

    public class AgeService : IAgeService
    {
        private readonly IAgeRepository _repo;
        public AgeService(IAgeRepository repo)
        {
            _repo = repo;
        }

		public async Task<IEnumerable<Age>> GetAges()
		{
            try
            {
                var ages =  await _repo.GetAll();
                return ages;
            }
            catch 
            {
                return null!;
            }

		}

	}
}
