using Microsoft.AspNetCore.Mvc;
using LMS_ServerAPI.Services.AuthorService;
using LMS_ServerAPI.Helpers;
using LMS_ServerAPI.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
namespace LMS_ServerAPI.Controllers
{
	[ApiController]
	[Route("/api/[controller]/[action]")]
	public class AuthorController : Controller
	{
		IAuthorService _authorService;
		public AuthorController(IAuthorService authorService) 
		{
			_authorService = authorService;
		}

		[HttpGet]	
		public async Task<string >getAll()
		{
			var authors = await _authorService.GetAuthors();
			var statusCode = (authors == null) ? "500" : "200"; // Tính toán statusCode ở đây

			return JsonConvert.SerializeObject(new ResponseHandler<IEnumerable<Author>>
			{
				IsSuccess = true,
				Data = authors,
				StatusCode = statusCode,
				Message = (statusCode == "200") ? "OK" : "Failed" // Sử dụng statusCode thay vì StatusCode
			});

		}
	}
}
