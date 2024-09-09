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

        // Handles the user login operation, validating the user's credentials and generating a JWT token.
        public async Task<string> LoginAsync(LoginRequestDto model)
        {
            // Find user by email (which is used as a username in this case).
            var user = await _userManager.FindByNameAsync(model.Email);
            // Check if the user exists and the password is correct.
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // Generate a JWT token if login is successful.
                return GenerateJwtToken(user);
            }

            // Return null if login failed.
            return null;
        }

        // Private method to generate a JWT token for a user. This includes user and manager-specific claims.
        private string GenerateJwtToken(User user)
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
            };

            // Symmetric security key based on the JWT secret key defined in configuration.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSetting:securityKey"]));
            // Signing credentials using HMAC SHA256 algorithm.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create a token descriptor, which includes claims, expiration time, issuer, audience, and signing credentials.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60),  // Token expiration time (60 minutes).
                Issuer = _configuration["JWTSetting:Issuer"],  // JWT issuer.
                Audience = _configuration["JWTSetting:Audience"],  // JWT audience.
                SigningCredentials = creds  // Token signing credentials.
            };

            // Create and return the JWT token using the token handler.
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
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
