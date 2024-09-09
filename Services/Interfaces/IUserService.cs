using Microsoft.AspNetCore.Identity;
using PropertyManagementSystem.Dtos;

namespace PropertyManagementSystem.Services.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterAsync(RegisterRequestDto model);
        Task<string> LoginAsync(LoginRequestDto model);
    }
}
