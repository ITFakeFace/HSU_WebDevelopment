using LMS_ServerAPI.Models;
namespace LMS_ServerAPI.Repositories.CategoryRepository
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAll();
    }
}
