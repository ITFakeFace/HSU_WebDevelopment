using LMS_ServerAPI.Helpers;
using LMS_ServerAPI.Models;
using LMS_ServerAPI.Services.CategoryService;
using LMS_ServerAPI.Services.SeriesService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LMS_ServerAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class CategoryController : Controller
    {
        ICategoryService _CategoryService;
        public CategoryController(ICategoryService CategoryService)
        {
            _CategoryService = CategoryService;
        }

        [HttpGet]
        public async Task<string> getAll()
        {
            var Series = await _CategoryService.GetCategories();
            var statusCode = (Series == null) ? "500" : "200"; // Tính toán statusCode ở đây

            return JsonConvert.SerializeObject(new ResponseHandler<IEnumerable<Category>>
            {
                IsSuccess = true,
                Data = Series,
                StatusCode = statusCode,
                Message = (statusCode == "200") ? "OK" : "Failed" // Sử dụng statusCode thay vì StatusCode
            });


        }
    }
}
