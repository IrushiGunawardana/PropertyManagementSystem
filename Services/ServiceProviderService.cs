using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.Configurations;
using PropertyManagementSystem.Dtos;
using PropertyManagementSystem.Services.Interfaces;

namespace PropertyManagementSystem.Services
{
    // Service class responsible for managing service provider data
    public class ServiceProviderService : IServiceProviderService
    {
        // Injecting ApplicationDbContext dependency to interact with the database
        private readonly ApplicationDbContext _context;

        // Constructor to initialize the service with the database context
        public ServiceProviderService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Method to get service providers based on a job type
        public async Task<List<ServiceProviderDetailsDto>> GetServiceProvideryAsync(string jobType)
        {
            // Querying the database to find service providers related to a specific job type
            var serviceproviders = await _context.ServiceProviderJobTypes
                .Where(Type => Type.JobTypeId.ToString().Equals(jobType)) // Filter based on the job type ID
                .Select(jt => new ServiceProviderDetailsDto // Map the results to ServiceProviderDetailsDto
                {
                    Id = jt.ServiceProvider.Id,
                    UserId = jt.ServiceProvider.UserId,
                    CompanyName = jt.ServiceProvider.CompanyName,
                    Email = jt.ServiceProvider.Email,
                }).ToListAsync(); // Execute the query and return as a list

            return serviceproviders; // Return the list of service providers
        }
    }
}
