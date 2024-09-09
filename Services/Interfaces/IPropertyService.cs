using PropertyManagementSystem.Dtos;
using PropertyManagementSystem.Models.Schema;

namespace PropertyManagementSystem.Services.Interfaces
{
    public interface IPropertyService
    {
        Task<Guid> CreatePropertyAsync(string address);

        Task<List<PropertyListDto>> GetPropertyAsync(String userId);
    }
}
