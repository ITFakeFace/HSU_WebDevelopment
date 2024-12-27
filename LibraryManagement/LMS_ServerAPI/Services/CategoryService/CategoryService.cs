using LMS_ServerAPI.Models;
using LMS_ServerAPI.Repositories.CategoryRepository;

namespace LMS_ServerAPI.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
       
        private readonly ICategoryRepository _repo;
        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Category>> GetCategories()
            {
                try
                {
                    var categories = await _repo.GetAll();
                    return categories;
                }
                catch
                {
                    return null!;
                }

            }

        
    }
}
