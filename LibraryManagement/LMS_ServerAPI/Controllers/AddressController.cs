using LMS_ServerAPI.Helpers;
using LMS_ServerAPI.Models;
using LMS_ServerAPI.Services.AddressService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LMS_ServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _service;
        public AddressController(IAddressService service)
        {
            this._service = service;
        }

        [HttpGet]
        public async Task<string> GetAddresses()
        {
            return JsonConvert.SerializeObject(new ResponseHandler<IEnumerable<Address>>
            {
                IsSuccess = true,
                Message = "Success",
                StatusCode = "200",
                Data = await _service.GetAllAddressesAsync(),
            });
        }

        [HttpGet("{id}")]
        public async Task<string> GetAddressById(int id)
        {
            var address = await _service.GetAddressByIdAsync(id);
            if (address == null)
            {
                return JsonConvert.SerializeObject(new ResponseHandler<Address?>
                {
                    IsSuccess = false,
                    Message = "Address not found.",
                    StatusCode = "404",
                    Data = null
                });
            }

            return JsonConvert.SerializeObject(new ResponseHandler<Address>
            {
                IsSuccess = true,
                Message = "Address retrieved successfully.",
                StatusCode = "200",
                Data = address
            });
        }

        [HttpGet("streets/ward/{wardId}")]
        public async Task<string> GetStreetsByWardId(int wardId)
        {
            var streets = await _service.GetStreetsByWardIdAsync(wardId);
            return JsonConvert.SerializeObject(new ResponseHandler<IEnumerable<Street>>
            {
                IsSuccess = true,
                Message = "Streets retrieved successfully.",
                StatusCode = "200",
                Data = streets
            });
        }

        [HttpGet("wards/district/{districtId}")]
        public async Task<string> GetWardsByDistrictId(int districtId)
        {
            var wards = await _service.GetWardsByDistrictIdAsync(districtId);
            return JsonConvert.SerializeObject(new ResponseHandler<IEnumerable<Ward>>
            {
                IsSuccess = true,
                Message = "Wards retrieved successfully.",
                StatusCode = "200",
                Data = wards
            });
        }

        [HttpGet("districts/city/{cityId}")]
        public async Task<string> GetDistrictsByCityId(int cityId)
        {
            var districts = await _service.GetDistrictsByCityIdAsync(cityId);
            return JsonConvert.SerializeObject(new ResponseHandler<IEnumerable<District>>
            {
                IsSuccess = true,
                Message = "Districts retrieved successfully.",
                StatusCode = "200",
                Data = districts
            });
        }

        [HttpGet("cities")]
        public async Task<string> GetAllCities()
        {
            var cities = await _service.GetAllCitiesAsync();
            return JsonConvert.SerializeObject(new ResponseHandler<IEnumerable<City>>
            {
                IsSuccess = true,
                Message = "Cities retrieved successfully.",
                StatusCode = "200",
                Data = cities
            });
        }



    }
}
