using LMS_ServerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS_ServerAPI.Repositories.AuthorRepositories
{
	public class AuthorRepository : IAuthorRepository
	{
		private readonly LibraryDbContext _context;
		public AuthorRepository(LibraryDbContext context) { _context = context; }
		public async Task<IEnumerable<Author>> GetAll()
		{
			try
			{
				
				return await _context.Authors.ToListAsync();
			}
			catch (Exception ex) 
			{
				return null!;
			}
		}
	}
}
