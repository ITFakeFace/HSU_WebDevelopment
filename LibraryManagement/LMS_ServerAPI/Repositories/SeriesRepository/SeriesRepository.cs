using LMS_ServerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS_ServerAPI.Repositories.SeriesRepository
{
    public class SeriesRepository : ISeriesRepository
    {
        private readonly LibraryDbContext _context;
        public SeriesRepository(LibraryDbContext context) { _context = context; }
        public async Task<IEnumerable<Series>> GetAll()
        {
            try
            {

                return await _context.Series.ToListAsync();
            }
            catch (Exception ex)
            {
                return null!;
            }
        }
    }
}
