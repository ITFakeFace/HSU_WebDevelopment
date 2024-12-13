using LMS_ServerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS_ServerAPI.Repositories.PublisherRepository
{
    public class PublisherRepository : IPublisherRepository
    {

		private readonly LibraryDbContext _context;
		public PublisherRepository(LibraryDbContext context) { _context = context; }

		public async Task<IEnumerable<Publisher>> GetAll()
		{
			try
			{
				var publishers = await _context.Publishers
					   .Include(p => p.ParentNavigation)  // Bao gồm thông tin ParentPublisher
					   .ToListAsync();  // Lấy tất cả dữ liệu
				return publishers;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return null!;
			}
		}

		public async Task<IEnumerable<Publisher>> GetChild(int id)
		{
			try
			{
				var publishers = await _context.Publishers
					   .Include(p => p.ParentNavigation)  // Bao gồm thông tin ParentPublisher
					   .Where(p => p.Parent == id)
					   .ToListAsync();  // Lấy tất cả dữ liệu
				return publishers;
			}
			catch (Exception ex) 
			{
				Console.WriteLine(ex.ToString());
				return null!;
			}
		}

		public async Task<IEnumerable<Publisher>> GetNoParent()
		{
			try
			{
				var publishers = await _context.Publishers
					   .Include(p => p.ParentNavigation)  // Bao gồm thông tin ParentPublisher
					   .Where(p => p.Parent == null)
					   .ToListAsync();  // Lấy tất cả dữ liệu
				return publishers;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return null!;
			}
		}
	}
}

