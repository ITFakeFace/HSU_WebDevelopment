using Microsoft.AspNetCore.Mvc;
using LMS_ServerAPI.Services.AgeService;
using LMS_ServerAPI.Helpers;
using LMS_ServerAPI.Models;
using Newtonsoft.Json;
namespace LMS_ServerAPI.Controllers
{
	[ApiController]
	[Route("/api/[controller]/[action]")]
	public class AgeController : Controller
	{
		IAgeService _ageService;
		public AgeController(IAgeService ageService) 
		{
			_ageService = ageService;
		}

		[HttpGet]	
		public async Task<string >getAll()
		{
			var ages = await _ageService.GetAges();
			var statusCode = (ages == null) ? "500" : "200"; // Tính toán statusCode ở đây

			return JsonConvert.SerializeObject(new ResponseHandler<IEnumerable<Age>>
			{
				IsSuccess = true,
				Data = ages,
				StatusCode = statusCode,
				Message = (statusCode == "200") ? "OK" : "Failed" // Sử dụng statusCode thay vì StatusCode
			});

		}
	}
}
