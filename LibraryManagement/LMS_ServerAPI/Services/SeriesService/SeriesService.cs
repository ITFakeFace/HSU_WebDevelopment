using LMS_ServerAPI.Models;
using LMS_ServerAPI.Repositories.SeriesRepository;
using LMS_ServerAPI.Services.SeriesService;

namespace LMS_ServerAPI.Services.SeriesService
{
    public class SeriesService : ISeriesService
    {
       
            private readonly ISeriesRepository _repo;
            public SeriesService(ISeriesRepository repo)
            {
                _repo = repo;
            }

            public async Task<IEnumerable<Series>> GetSeries()
            {
                try
                {
                    var Series = await _repo.GetAll();
                    return Series;
                }
                catch
                {
                    return null!;
                }

            }

        
    }
}
