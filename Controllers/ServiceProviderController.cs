using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.Dtos;
using PropertyManagementSystem.Models.Schema;
using PropertyManagementSystem.Services.Interfaces;
using System.Security.Claims;

namespace PropertyManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceProviderController : ControllerBase
    {
        private readonly IServiceProviderService _serviceProviderService;

        public ServiceProviderController(IServiceProviderService serviceProviderService)
        {
            _serviceProviderService = serviceProviderService;
        }

        //String-string
        [Authorize]
        [HttpGet("details")] //naming conventions
        public async Task<IActionResult> GetDetails([FromQuery] string jobType)//remove _type
        {
            var serviceProviderDetails = await _serviceProviderService.GetServiceProvideryAsync(jobType);
            return Ok(new CommonResponseDto
            {
                message = "Service providers successfully retrieved",
                data = serviceProviderDetails
            });
        }

    }
}
