using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.Configurations;
using PropertyManagementSystem.Dtos;
using PropertyManagementSystem.Models.Schema;
using PropertyManagementSystem.Services.Interfaces;

namespace PropertyManagementSystem.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly ApplicationDbContext _context;
        public PropertyService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> CreatePropertyAsync(string address)
        {
            var property = new Property { Address = address };
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();
            return property.Id;
        }

        public async Task<List<PropertyListDto>> GetPropertyAsync(String userId)
        {
            var propertyManageList = await _context.PropertyManagers.Where(pm => pm.UserId.ToString() == userId).ToListAsync();

            if (propertyManageList.Count == 0)
            {
                throw new Exception("No Property Manger Found !");
            }
            List<Guid> propertyIds = propertyManageList.Select(pm => pm.PropertyId).ToList();

            var properties = await _context.Properties
             .Where(p => propertyIds.Contains(p.Id))
             .Select(p => new PropertyListDto
             {
                 Id = p.Id,
                 Address = p.Address,
                 ownersDetails = p.PropertyOwners.Select(owner => new OwnerDto { UserId = owner.UserId, FirstName = owner.FirstName, LastName = owner.LastName }).ToList(),
                 tenantsDetails = p.PropertyTenants.Select(tenant => new TenantDto { UserId = tenant.UserId, FirstName = tenant.FirstName, LastName = tenant.LastName }).ToList()
                 
             })
             .ToListAsync();

            return properties;
        }
    }
}
