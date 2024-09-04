using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.Configurations;
using PropertyManagementSystem.Dtos;
using PropertyManagementSystem.Services.Interfaces;

namespace PropertyManagementSystem.Services
{
    public class ServiceProviderService : IServiceProviderService
    {
        private readonly ApplicationDbContext _context;

        public ServiceProviderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceProviderDetailsDto>> GetServiceProvideryAsync(string jobType)
        {

            var serviceproviders = await _context.ServiceProviderJobTypes
                 .Where(Type => Type.JobTypeId.ToString().Equals(jobType))
                 .Select(jt => new ServiceProviderDetailsDto
                 {
                     Id = jt.ServiceProvider.Id,
                     UserId = jt.ServiceProvider.UserId,
                     CompanyName = jt.ServiceProvider.CompanyName,
                     Email = jt.ServiceProvider.Email,
                 }).ToListAsync();

            return serviceproviders;
        }
    }
}


