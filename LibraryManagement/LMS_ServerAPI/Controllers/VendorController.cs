using LMS_ServerAPI.Helpers;
using LMS_ServerAPI.Models;
using LMS_ServerAPI.Models.APIHelpers;
using LMS_ServerAPI.Services.VendorService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LMS_ServerAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class VendorController
    {
        IVendorService _vendorService;
        public VendorController(IVendorService vendorService)
        {
            _vendorService = vendorService;
        }
        [HttpGet]
        public async Task<string> getAll()
        {
            var vendors = await _vendorService.GetVendors();
            var response = new ResponseHandler<IEnumerable<Vendor>>
            {
                IsSuccess = true,
                Data = vendors,
                StatusCode = "200",
                Message = "OK"// Sử dụng statusCode thay vì StatusCode
            };
            return JsonConvert.SerializeObject(response);
        }
    }
}
