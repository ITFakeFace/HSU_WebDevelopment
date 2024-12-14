using LMS_ServerAPI.Models;

namespace LMS_ServerAPI.Repositories.PublisherRepository
{
	public interface IPublisherRepository
	{
		Task<IEnumerable<Publisher>> GetAll();
		Task<IEnumerable<Publisher>> GetNoParent();
		Task<IEnumerable<Publisher>> GetChild(int id);

	}
}
