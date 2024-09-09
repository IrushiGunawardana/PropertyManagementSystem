using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.Dtos;
using PropertyManagementSystem.Services.Interfaces;
using System.Security.Claims;

namespace PropertyManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {

        private readonly IPropertyService _propertyService;

        // Constructor to inject the IPropertyService dependency
        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        // Endpoint to retrieve property details, requires authorization
        [Authorize]
        [HttpGet("getpropertydetails")]
        public async Task<IActionResult> GetDetails()
        {
            // Retrieves the user's ID from the authentication token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Calls the service to get properties associated with the user
            var properties = await _propertyService.GetPropertyAsync(userId);

            // Returns a successful response with the list of properties
            return Ok(new CommonResponseDto
            {
                message = "Properties successfully retrieved",
                data = properties
            });
        }
    }
}

