using LMS_ServerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS_ServerAPI.Repositories.AuthorRepositories
{
	public interface IAuthorRepository
	{
		Task<IEnumerable<Author>> GetAll();
	}
}
