using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.Configurations;
using PropertyManagementSystem.Dtos;
using PropertyManagementSystem.Models.Schema;
using PropertyManagementSystem.Services.Interfaces;
using System.Net;
using System.Web.Http.Results;

namespace PropertyManagementSystem.Services
{
    public class JobService : IJobService
    {
        private readonly ApplicationDbContext _context;

        public JobService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<JobListDto>> GetAllJobsAsync(String userId)
        {
            var propertyManager = await _context.PropertyManagers.FirstOrDefaultAsync(pm => pm.UserId.ToString() == userId);

            if (propertyManager == null)
            {
                throw new Exception("No Property Found for this !");
            }

            var jobs = await _context.Jobs
             .Where(j => j.PostedByUserId.ToString() == userId)
             .Select(j => new JobListDto
             {
                 id = j.Id,
                 JobNumber = j.JobNumber ,
                 Description = j.Description,
                 PostedDate = j.PostedOn

             })
             .ToListAsync();

            return jobs;
        }

        public async Task<SingleJobDto> GetJobByIdAsync(String userId, string jobId)
        {

            var job = _context.Jobs
             .Where(j => j.Id.ToString() == jobId && j.PostedByUserId.ToString() == userId).FirstOrDefault();

            if (job == null)
            {
                throw new Exception("Job Id not found for authorized user");
            }

            var property = await _context.Properties
                .Where(property => property.Id == job.PropertyId).FirstOrDefaultAsync();

            var owners = await _context.PropertyOwners
                .Where(owners => owners.PropertyId == job.PropertyId)
                .Select(owner => new SingleOwnerDtoResponse
                {
                    Id= owner.Id,
                    UserId = owner.UserId,
                    FirstName = owner.FirstName,
                    LastName = owner.LastName
                }).ToListAsync();

            var tenants = await _context.PropertyTenants
                .Where(tenant => tenant.PropertyId == job.PropertyId)
                .Select(tenant=> new SingleTenantDtoResponse 
                {   Id = tenant.Id,
                    UserId = tenant.UserId,
                    FirstName = tenant.FirstName,
                    LastName = tenant.LastName 
                }).ToListAsync();


            var jobType = await _context.JobTypes.Where(jobtype => jobtype.Id == job.TypeId).FirstOrDefaultAsync();
            var provider = await _context.ServiceProviders.Where(provider => provider.Id == job.ServiceProviderId).FirstOrDefaultAsync();

            return new SingleJobDto
            {
                Id = job.Id,
                JobNumber = job.JobNumber,
                property = new SinglePropertyDtoResponse { Id = property.Id, Address = property.Address },
                OwnerDetails = owners,
                TenantDetails = tenants,
                Description = job.Description,
                JobType =  new JobTypeDtoResponse { Id = jobType.Id, Name = jobType.Name },
                Provider = new ProviderDtoResponse { Id = provider.Id, UserId= provider.UserId, CompanyName = provider.CompanyName, Email = provider.Email },

            };



        }
        public async Task<Models.Schema.Job> CreateJobAsync(string userId, CreateJobRequestDto model)
        {
           
        var JobPost = new Models.Schema.Job
            {
                Id = Guid.NewGuid(),
                JobNumber = await GenerateUniqueJobNumberAsync(),
                PropertyId = model.PropertyId,
                PostedByUserId = Guid.Parse(userId),
                PostedOn = DateTime.Now,
                Description = model.Description,
                TypeId = model.Type,
                ServiceProviderId = model.ServiceProviderId
            };


            _context.Jobs.Add(JobPost);
            await _context.SaveChangesAsync();

            return JobPost;
        }


        public async Task<int> GenerateUniqueJobNumberAsync()
        {
            int jobNumber;
            Random random = new Random();
            do
            {
                jobNumber = random.Next(100000, 999999);
            }
            while (await _context.Jobs.AnyAsync(j => j.JobNumber == jobNumber));

            return jobNumber;
        }
        public async Task<bool> isValid(string userId, CreateJobRequestDto model) {

            var propertyManager = await _context.PropertyManagers.FirstOrDefaultAsync(pm => pm.UserId.ToString() == userId);

            if (propertyManager == null)
            {
                return false;
            }

            var property = await _context.Properties.Where(p => p.Id == propertyManager.PropertyId).FirstOrDefaultAsync();

            if (property == null) {
                return false;
            }

            var provider = await _context.ServiceProviders.Where(provider => provider.Id == model.ServiceProviderId).FirstOrDefaultAsync();
            
            if (provider == null)
            {
                return false;
            }

            var jobtype = await _context.JobTypes.Where(job => job.Id == model.Type).FirstOrDefaultAsync();

            if (jobtype == null)
            {
                return false;
            }

            return true;
        }


        public async Task<List<JobTypesDto>> GetJobTypesAsync()
        {
            var jobTypes = await  _context.JobTypes
                .Select(type => new JobTypesDto
                {
                    Id = type.Id,
                    Name = type.Name,
                }).ToListAsync();

            return jobTypes;
        }
    }
}
