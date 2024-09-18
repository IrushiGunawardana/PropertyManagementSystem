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
        // Inject IServiceProviderService for business logic related to service providers
        private readonly IServiceProviderService _serviceProviderService;

        public ServiceProviderController(IServiceProviderService serviceProviderService)
        {
            _serviceProviderService = serviceProviderService; // Assign the injected service to a private field
        }

        // GET endpoint to retrieve service provider details based on job type
        // Authorization is required to access this endpoint
        [Authorize]
        [HttpGet("getserviceproviderdetails")] 
        public async Task<IActionResult> GetDetails([FromQuery] string jobType)
        {
            
            // Retrieve service provider details asynchronously by calling the service layer
            var serviceProviderDetails = await _serviceProviderService.GetServiceProvideryAsync(Guid.Parse(jobType));

            // Return an Ok response with a CommonResponseDto that contains the message and data
            return Ok(new CommonResponseDto
            {
                message = "Service providers successfully retrieved",
                data = serviceProviderDetails
            });
        }
    }
}
