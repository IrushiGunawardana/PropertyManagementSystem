using PropertyManagementSystem.Dtos;

namespace PropertyManagementSystem.Services.Interfaces
{
    public interface IServiceProviderService
    {
        Task<List<ServiceProviderDetailsDto>> GetServiceProvideryAsync(Guid jobType);
    }
}
