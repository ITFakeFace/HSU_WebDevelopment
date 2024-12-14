using LMS_ServerAPI.Helpers;
using LMS_ServerAPI.Models;
using LMS_ServerAPI.Services.AuthorService;
using LMS_ServerAPI.Services.PublisherService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LMS_ServerAPI.Controllers
{
	[ApiController]
	[Route("/api/[controller]/[action]")]
	public class PublisherController : Controller
	{
		IPublisherService _publisherService;
		public PublisherController(IPublisherService publisherService)
		{
			_publisherService = publisherService;
		}

		[HttpGet]
		public async Task<string> getAll()
		{
			var publishers = await _publisherService.GetPublishers();
			var statusCode = (publishers == null) ? "500" : "200"; // Tính toán statusCode ở đây
			var response = new ResponseHandler<IEnumerable<Publisher>>
			{
				IsSuccess = true,
				Data = publishers,
				StatusCode = statusCode,
				Message = (statusCode == "200") ? "OK" : "Failed" // Sử dụng statusCode thay vì StatusCode
			};
			return JsonConvert.SerializeObject(response, new JsonSerializerSettings
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			});

		}

		[HttpGet]
		public async Task<string> getnoparent()
		{
			var publishers = await _publisherService.GetPublishersNoParent();
			var statusCode = (publishers == null) ? "500" : "200"; // Tính toán statusCode ở đây
			var response = new ResponseHandler<IEnumerable<Publisher>>
			{
				IsSuccess = true,
				Data = publishers,
				StatusCode = statusCode,
				Message = (statusCode == "200") ? "OK" : "Failed" // Sử dụng statusCode thay vì StatusCode
			};
			return JsonConvert.SerializeObject(response, new JsonSerializerSettings
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			});

		}
		[HttpGet("{id}")]
		public async Task<string> getchilds(string id)
		{
			var publishers = await _publisherService.GetPublishersChild(id);
			var statusCode = (publishers == null) ? "500" : "200"; // Tính toán statusCode ở đây
			var response = new ResponseHandler<IEnumerable<Publisher>>
			{
				IsSuccess = true,
				Data = publishers,
				StatusCode = statusCode,
				Message = (statusCode == "200") ? "OK" : "Failed" // Sử dụng statusCode thay vì StatusCode
			};
			return JsonConvert.SerializeObject(response, new JsonSerializerSettings
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			});

		}
	}
	
}
