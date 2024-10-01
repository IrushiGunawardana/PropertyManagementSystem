using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.Configurations;
using PropertyManagementSystem.Dtos;
using PropertyManagementSystem.Models.Schema;
using PropertyManagementSystem.Services.Interfaces;

namespace PropertyManagementSystem.Services
{
    // Service class to manage properties, implements the IPropertyService interface
    public class PropertyService : IPropertyService
    {
        // Dependency Injection of ApplicationDbContext to interact with the database
        private readonly ApplicationDbContext _context;

        public PropertyService(ApplicationDbContext context)
        {
            _context = context; // Assign the injected context to the private field
        }

        // Method to create a new property asynchronously
        public async Task<Guid> CreatePropertyAsync(string address)
        {
            // Create a new Property entity and add it to the context
            var property = new Property { Address = address };
            _context.Properties.Add(property);

            // Save changes to the database asynchronously
            await _context.SaveChangesAsync();

            // Return the ID of the newly created property2
            return property.Id;
        }

        // Method to get a list of properties managed by a specific user (Property Manager)
        public async Task<List<PropertyListDto>> GetPropertyAsync(Guid userId)
        {
            // Retrieve the list of PropertyManagers based on the provided userId
            var propertyManageList = await _context.PropertyManagers.Where(pm => pm.UserId == userId).ToListAsync();

            // Throw an exception if no PropertyManager is found for the given userId
            if (propertyManageList.Count == 0)
            {
                throw new Exception("No Property Manager Found!");
            }

            // Extract the list of Property IDs managed by the user
            List<Guid> propertyIds = propertyManageList.Select(pm => pm.PropertyId).ToList();

            // Retrieve properties based on the extracted Property IDs and project the result to PropertyListDto
            var properties = await _context.Properties
             .Where(p => propertyIds.Contains(p.Id))
             .Select(p => new PropertyListDto
             {
                 // Map properties to PropertyListDto
                 Id = p.Id,
                 Address = p.Address,
                 // Map owners to OwnerDto list
                 ownersDetails = p.PropertyOwners.Select(owner => new OwnerDto { UserId = owner.UserId, FirstName = owner.FirstName, LastName = owner.LastName }).ToList(),
                 // Map tenants to TenantDto list
                 tenantsDetails = p.PropertyTenants.Select(tenant => new TenantDto { UserId = tenant.UserId, FirstName = tenant.FirstName, LastName = tenant.LastName }).ToList()

             })
             .ToListAsync();

            // Return the list of properties as PropertyListDto
            return properties;
        }
    }
}
