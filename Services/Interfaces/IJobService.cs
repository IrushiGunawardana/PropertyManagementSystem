using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.Dtos;
using PropertyManagementSystem.Models.Schema;

namespace PropertyManagementSystem.Services.Interfaces
{
    public interface IJobService
    {
        Task<List<JobTypesDto>> GetJobTypesAsync();

        Task<SingleJobDto> GetJobByIdAsync(string userId, string jobId);

        Task<List<JobListDto>> GetAllJobsAsync(string userId);

        Task<Models.Schema.Job> CreateJobAsync(string userId, CreateJobRequestDto model);

        Task<bool> isValid(string userId, CreateJobRequestDto model);
    }
}
