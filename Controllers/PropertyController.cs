

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

        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [Authorize]
        [HttpGet("details")]
        public async Task<IActionResult> GetDetails()
        {
            //propertyid
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var properties = await _propertyService.GetPropertyAsync(userId);
            return Ok(new CommonResponseDto
            {
                message = "Properties successfully retrieved",
                data = properties
            });

        }

    }
}
