using LMS_ServerAPI.Models;

namespace LMS_ServerAPI.Services.SeriesService
{
    public interface ISeriesService
    {
        Task<IEnumerable<Series>> GetSeries();
    }
}
