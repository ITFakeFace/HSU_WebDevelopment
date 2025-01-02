using LMS_ServerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS_ServerAPI.Repositories.AgeRepositories
{
	public class AgeRepository : IAgeRepository
	{
		private readonly LibraryDbContext _context;
		public AgeRepository(LibraryDbContext context) { _context = context; }
		public async Task<IEnumerable<Age>> GetAll()
		{
			try
			{
				
				return await _context.Ages.ToListAsync();
			}
			catch (Exception ex) 
			{
				return null!;
			}
		}
	}
}
