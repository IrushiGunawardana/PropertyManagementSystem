using Microsoft.AspNetCore.Identity;
using PropertyManagementSystem.Dtos;

namespace PropertyManagementSystem.Services.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterAsync(RegisterRequestDto model);
      
        Task<(string AccessToken, string RefreshToken)> LoginAsync(LoginRequestDto model); // Return a tuple with both tokens
        Task<string> RefreshTokenAsync(string refreshToken);  // Add RefreshTokenAsync method definition
    }
}
