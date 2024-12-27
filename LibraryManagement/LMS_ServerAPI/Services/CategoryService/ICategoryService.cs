using LMS_ServerAPI.Models;

namespace LMS_ServerAPI.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetCategories();
    }
}
