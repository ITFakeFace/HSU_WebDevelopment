using LMS_ServerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS_ServerAPI.Repositories.AgeRepositories
{
	public interface IAgeRepository
	{
		Task<IEnumerable<Age>> GetAll();
	}
}
