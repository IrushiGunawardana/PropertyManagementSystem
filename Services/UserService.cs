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
    // Implements the IUserService interface, which handles user registration, login, and token generation.
    public class UserService : IUserService
    {
        // Dependencies for user management, configuration, database context, and property services.
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IPropertyService _propertyService;

        // Constructor for injecting necessary services into the UserService.
        public UserService(UserManager<User> userManager, IConfiguration configuration, ApplicationDbContext context, IPropertyService propertyService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            _propertyService = propertyService;
        }

        public async Task<(string AccessToken, string RefreshToken)> LoginAsync(LoginRequestDto model)
        {
            // Find user by email (which is used as a username in this case).
            var user = await _userManager.FindByNameAsync(model.Email);

            // Check if the user exists and the password is correct.
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // Generate the access token.
                var accessToken = GenerateJwtToken(user, isRefreshToken: false);

                // Generate the refresh token.
                var refreshToken = GenerateJwtToken(user, isRefreshToken: true);

                return (accessToken, refreshToken);
            }

            // Return null if login failed.
            return (null, null);
        }


        // Private method to generate a JWT token for a user. This includes user and manager-specific claims.
        private string GenerateJwtToken(User user , bool isRefreshToken)
        {
            // Retrieve the PropertyManager associated with the user.
            var manager = _context.PropertyManagers.Where(pm => pm.UserId == user.Id).FirstOrDefault();

            // Create a set of claims for the JWT token, including user information and manager details.
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),  // User ID.
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique token ID.
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()), // Token issued at timestamp.
                new Claim("UserId", user.Id.ToString()),  // Custom claim for user ID.
                new Claim("UserName", user.UserName),  // Custom claim for username.
                new Claim("FirstName", manager.FirstName),  // Custom claim for manager's first name.
                new Claim("LastName", manager.LastName),  // Custom claim for manager's last name.
                new Claim("email", manager.Email),  // Custom claim for manager's email.
                new Claim("TokenType", isRefreshToken ? "RefreshToken" : "AccessToken")
            };

            // Symmetric security key based on the JWT secret key defined in configuration.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSetting:securityKey"]));
            // Signing credentials using HMAC SHA256 algorithm.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Set expiration time: 60 minutes for access token, 7 days for refresh token
            var expiresValue = isRefreshToken ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddMinutes(60);

            // Create a token descriptor, which includes claims, expiration time, issuer, audience, and signing credentials.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresValue, 
                Issuer = _configuration["JWTSetting:Issuer"],  // JWT issuer.
                Audience = _configuration["JWTSetting:Audience"],  // JWT audience.
                SigningCredentials = creds  // Token signing credentials.
            };

            // Create and return the JWT token using the token handler.
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<string> RefreshTokenAsync(string refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JWTSetting:securityKey"]);

            try
            {
                // Validate token signature and extract principal
                var principal = tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = validatedToken as JwtSecurityToken;

                // Check if this is a refresh token
                var tokenType = principal.Claims.FirstOrDefault(x => x.Type == "TokenType")?.Value;
                if (tokenType != "RefreshToken")
                    throw new SecurityTokenException("Invalid token type");

                // Check token expiration (if in the last hour, allow refresh)
                var expiryDateUnix = long.Parse(principal.Claims.First(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expiryDateTime = DateTimeOffset.FromUnixTimeSeconds(expiryDateUnix).UtcDateTime;
                var currentUtc = DateTime.UtcNow;

                if (expiryDateTime > currentUtc)
                {
                    // Token is valid for refresh. Generate a new access token.
                    var userId = principal.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
                    var user = await _userManager.FindByIdAsync(userId);

                    if (user != null)
                    {
                        var newAccessToken = GenerateJwtToken(user, isRefreshToken: false);
                        return newAccessToken;
                    }
                }

                return null; // Token expired
            }
            catch
            {
                return null; // Token is invalid
            }
        }


        // Handles the registration of a new user with role-based logic for property managers, owners, tenants, and service providers.
        public async Task<IdentityResult> RegisterAsync(RegisterRequestDto model)
        {
            // Create a property asynchronously and get the property ID (using a dummy GUID for now).
            Guid propertyId = Guid.Parse("E2A3A3A6-3EC3-4F24-8679-02382E816856");

            // Create a new user with the provided username and password (the password is hashed internally).
            var user = new User { UserName = model.UserName };
            var result = await _userManager.CreateAsync(user, model.Password);

            // If the user was created successfully, assign them a specific role and create role-related entities.
            if (result.Succeeded)
            {
                // Role-based logic: check which role the user has selected and create the appropriate entity.
                switch (model.Role.ToLower())
                {
                    case "propertymanager":
                        // Create a PropertyManager entity if the role is PropertyManager.
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
                        // Create a PropertyOwner entity if the role is PropertyOwner.
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
                        // Create a PropertyTenant entity if the role is PropertyTenant.
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
                        // Create a ServiceProvider entity if the role is ServiceProvider.
                        var serviceProvider = new Models.Schema.ServiceProvider
                        {
                            UserId = user.Id,
                            CompanyName = model.CompanyName,
                            Email = model.Email
                        };
                        _context.ServiceProviders.Add(serviceProvider);
                        break;

                    default:
                        // Throw an exception if the specified role is invalid.
                        throw new ArgumentException("Invalid role specified.");
                }

                // Save the changes to the database.
                await _context.SaveChangesAsync();
            }

            // Return the result of the user creation operation.
            return result;
        }
    }
}
