using System.IdentityModel.Tokens.Jwt;         
using System.Security.Claims;                  
using System.Text;                             
using Microsoft.AspNetCore.Authorization;      
using Microsoft.AspNetCore.Identity;          
using Microsoft.AspNetCore.Mvc;                
using Microsoft.EntityFrameworkCore;           
using Microsoft.IdentityModel.Tokens;         
using PropertyManagementSystem.Models.Authentication;  
using PropertyManagementSystem.Dtos;           
using PropertyManagementSystem.Models.Schema;  
using PropertyManagementSystem.Services.Interfaces;  

namespace PropertyManagementSystem.Controllers
{
    
    [ApiController]

    [Route("api/account")]

    // Controller responsible for handling user-related actions, such as registration and login.
    public class UserController : ControllerBase
    {
        // Private field for user service dependency that handles user management tasks like registration and login.
        private readonly IUserService _userService;

        // Constructor to inject the IUserService, which will be used to perform user-related operations.
        public UserController(IUserService userService)
        {
            _userService = userService;  // Assign the injected IUserService instance to the private field.
        }

        // HTTP POST endpoint for registering a new user. The endpoint receives a RegisterRequestDto model from the request body.
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto model)
        {
            // Calls the RegisterAsync method from the IUserService to create a new user.
            var result = await _userService.RegisterAsync(model);

            // If the registration is successful, return an HTTP 200 OK status with a success message.
            if (result.Succeeded)
            {
                return Ok(new { Message = "User registered successfully" });
            }

            // If the registration fails, return an HTTP 400 Bad Request status with the error messages.
            return BadRequest(result.Errors);
        }

        // HTTP POST endpoint for user login. Receives a LoginRequestDto object with the user's credentials.
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            // Calls the LoginAsync method from the IUserService to validate the user's credentials and generate a JWT token if valid.
            var token = await _userService.LoginAsync(model);

            // If the login is successful and a token is generated, return an HTTP 200 OK status with the token.
            if (token != null)
            {
                return Ok(new { token });  // Return the JWT token to the client.
            }

            // If the login fails (e.g., invalid credentials), return an HTTP 401 Unauthorized status.
            return Unauthorized();
        }
    }
}
