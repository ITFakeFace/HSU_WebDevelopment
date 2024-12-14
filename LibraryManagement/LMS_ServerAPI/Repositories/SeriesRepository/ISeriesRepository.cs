using LMS_ServerAPI.Models;

namespace LMS_ServerAPI.Repositories.SeriesRepository
{
    public interface ISeriesRepository
    {
        Task<IEnumerable<Series>> GetAll();

    }
}
