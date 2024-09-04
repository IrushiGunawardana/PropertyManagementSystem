using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PropertyManagementSystem.Configurations;
using PropertyManagementSystem.Dtos;
using PropertyManagementSystem.Models.Schema;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PropertyManagementSystem.Services.Interfaces
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IPropertyService _propertyService;

        public UserService(UserManager<User> userManager, IConfiguration configuration, ApplicationDbContext context, IPropertyService propertyService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            _propertyService = propertyService;
        }

        public async Task<string> LoginAsync(LoginRequestDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return GenerateJwtToken(user);
            }

            return null;
        }
        //private string GenerateJwtToken(User user)
        //{
        //    var claims = new[]
        //    {
        //    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //    new Claim(JwtRegisteredClaimNames.Iat,  DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
        //    new Claim("UserId", user.Id.ToString()),
        //    new Claim("UserName", user.UserName)
        //};

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSetting:securityKey"]));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        issuer: _configuration["JWTSetting:Issuer"],
        //        audience: _configuration["JWTSetting:Audience"],
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddMinutes(60),
        //        signingCredentials: creds);

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        private string GenerateJwtToken(User user)
        {

          var manager =  _context.PropertyManagers.Where(pm => pm.UserId == user.Id).FirstOrDefault();
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
        new Claim("UserId", user.Id.ToString()),
        new Claim("UserName", user.UserName),
        new Claim("FirstName", manager.FirstName),
         new Claim("LastName", manager.LastName),
         new Claim("email", manager.Email),
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSetting:securityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = _configuration["JWTSetting:Issuer"],
                Audience = _configuration["JWTSetting:Audience"],
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        //    private string GenerateJwtToken(User user)
        //    {
        //        var claims = new[]
        //        {
        //    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
        //    new Claim("UserId", user.Id.ToString()),
        //    new Claim("UserName", user.UserName)
        //};

        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSetting:securityKey"]));
        //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //        var tokenDescriptor = new SecurityTokenDescriptor
        //        {
        //            Subject = new ClaimsIdentity(claims),
        //            Expires = DateTime.UtcNow.AddMinutes(60),
        //            Issuer = _configuration["JWTSetting:Issuer"],
        //            Audience = _configuration["JWTSetting:Audience"],
        //            SigningCredentials = creds
        //        };

        //        return new JwtSecurityTokenHandler().CreateEncodedJwt(tokenDescriptor);
        //    }


        public async Task<IdentityResult> RegisterAsync(RegisterRequestDto model)
        {
            //Guid propertyId = await _propertyService.CreatePropertyAsync(model.Address);

            Guid propertyId = Guid.Parse("E2A3A3A6-3EC3-4F24-8679-02382E816856");

            //Passing the password for hashing using createAsync

            var user = new User { UserName = model.UserName  };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {

                switch (model.Role.ToLower())
                {
                    case "propertymanager":
                        var propertyManager = new PropertyManager
                        {
                            UserId = user.Id,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            PropertyId = propertyId
                        };
                        _context.PropertyManagers.Add(propertyManager);
                        break;

                    case "propertyowner":
                        var propertyOwner = new PropertyOwner
                        {
                            UserId = user.Id,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            PropertyId = propertyId
                        };
                        _context.PropertyOwners.Add(propertyOwner);
                        break;

                    case "propertytenant":
                        var propertyTenant = new PropertyTenant
                        {
                            UserId = user.Id,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            PropertyId = propertyId
                        };
                        _context.PropertyTenants.Add(propertyTenant);
                        break;

                    case "serviceprovider":
                        var serviceProvider = new Models.Schema.ServiceProvider
                        {
                            UserId = user.Id,
                            CompanyName = model.CompanyName,
                            Email = model.Email
                        };
                        _context.ServiceProviders.Add(serviceProvider);
                        break;

                    default:
                        throw new ArgumentException("Invalid role specified.");
                }

                await _context.SaveChangesAsync();
            }

            return result;
        }


    }
}
