using LMS_ServerAPI.Helpers;
using LMS_ServerAPI.Models;
using LMS_ServerAPI.Services.SeriesService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LMS_ServerAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class SeriesController
    {
       
            ISeriesService _SeriesService;
            public SeriesController(ISeriesService SeriesService)
            {
                _SeriesService = SeriesService;
            }

            [HttpGet]
            public async Task<string> getAll()
            {
                var Series = await _SeriesService.GetSeries();
                var statusCode = (Series == null) ? "500" : "200"; // Tính toán statusCode ở đây

                return JsonConvert.SerializeObject(new ResponseHandler<IEnumerable<Series>>
                {
                    IsSuccess = true,
                    Data = Series,
                    StatusCode = statusCode,
                    Message = (statusCode == "200") ? "OK" : "Failed" // Sử dụng statusCode thay vì StatusCode
                });

            
        }
    }
}
