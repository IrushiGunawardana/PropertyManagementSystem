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

        // Constructor to inject ApplicationDbContext dependency
        public JobService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all jobs posted by a specific user
        public async Task<List<JobListDto>> GetAllJobsAsync(string userId)
        {
            // Finds the property manager associated with the user
            var propertyManager = await _context.PropertyManagers.FirstOrDefaultAsync(pm => pm.UserId.ToString() == userId);

            // If no property manager is found, throw an exception
            if (propertyManager == null)
            {
                throw new Exception("No Property Found for this !");
            }

            // Retrieves all jobs associated with the user and maps to JobListDto
            var jobs = await _context.Jobs
             .Where(j => j.PostedByUserId.ToString() == userId)
             .Select(j => new JobListDto
             {
                 id = j.Id,
                 JobNumber = j.JobNumber,
                 Description = j.Description,
                 PostedDate = j.PostedOn
             })
             .ToListAsync();

            return jobs;
        }

        // Retrieves the details of a job by job ID for a specific user
        public async Task<SingleJobDto> GetJobByIdAsync(string userId, string jobId) 
        {
            // Finds the job based on job ID and user ID
            var job = _context.Jobs
             .Where(j => j.Id.ToString() == jobId && j.PostedByUserId.ToString() == userId).FirstOrDefault();
            //j => j.Id.ToString refer     dont convert the value in the queires , declare and use

            // If the job is not found, throw an exception
            if (job == null)
            {
                throw new Exception("Job Id not found for authorized user");
            }

            // Retrieves associated property, owners, tenants, job type, and service provider information
            var property = await _context.Properties   
                .Where(property => property.Id == job.PropertyId).FirstOrDefaultAsync();

            var owners = await _context.PropertyOwners
                .Where(owners => owners.PropertyId == job.PropertyId)
                .Select(owner => new SingleOwnerDtoResponse
                {
                    Id = owner.Id,
                    UserId = owner.UserId,
                    FirstName = owner.FirstName,
                    LastName = owner.LastName
                }).ToListAsync();

            var tenants = await _context.PropertyTenants
                .Where(tenant => tenant.PropertyId == job.PropertyId)
                .Select(tenant => new SingleTenantDtoResponse
                {
                    Id = tenant.Id,
                    UserId = tenant.UserId,
                    FirstName = tenant.FirstName,
                    LastName = tenant.LastName
                }).ToListAsync();

            var jobType = await _context.JobTypes.Where(jobtype => jobtype.Id == job.TypeId).FirstOrDefaultAsync();
            var provider = await _context.ServiceProviders.Where(provider => provider.Id == job.ServiceProviderId).FirstOrDefaultAsync();

            // Returns a SingleJobDto containing all relevant job details
            return new SingleJobDto
            {
                Id = job.Id,
                JobNumber = job.JobNumber,
                property = new SinglePropertyDtoResponse { Id = property.Id, Address = property.Address },
                OwnerDetails = owners,
                TenantDetails = tenants,
                Description = job.Description,
                JobType = new JobTypeDtoResponse { Id = jobType.Id, Name = jobType.Name },
                Provider = new ProviderDtoResponse { Id = provider.Id, UserId = provider.UserId, CompanyName = provider.CompanyName, Email = provider.Email },
            };
        }

        // Creates a new job and saves it to the database
        public async Task<Models.Schema.Job> CreateJobAsync(string userId, CreateJobRequestDto model)
        {
            // Creates a new Job object and generates a unique job number
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

            // Adds the new job to the context and saves changes
            _context.Jobs.Add(JobPost);
            await _context.SaveChangesAsync();

            return JobPost;
        }

        // Generates a unique six-digit job number for each job
        public async Task<int> GenerateUniqueJobNumberAsync()
        {
            int jobNumber;
            Random random = new Random();
            do
            {
                // Generates a random job number between 100000 and 999999
                jobNumber = random.Next(100000, 999999);
            }
            // Ensures the generated number is unique by checking the database
            while (await _context.Jobs.AnyAsync(j => j.JobNumber == jobNumber));

            return jobNumber;
        }

        // Validates the job request by ensuring the property manager, property, provider, and job type are valid
        public async Task<bool> isValid(string userId, CreateJobRequestDto model)
        {
            // Validates the property manager based on user ID
            var propertyManager = await _context.PropertyManagers.FirstOrDefaultAsync(pm => pm.UserId.ToString() == userId);

            if (propertyManager == null)
            {
                return false;
            }

            // Validates the property associated with the property manager
            var property = await _context.Properties.Where(p => p.Id == propertyManager.PropertyId).FirstOrDefaultAsync();

            if (property == null)
            {
                return false;

            }

            // Validates the service provider
            var provider = await _context.ServiceProviders.Where(provider => provider.Id == model.ServiceProviderId).FirstOrDefaultAsync();

            if (provider == null)
            {
                return false;
            }

            // Validates the job type
            var jobtype = await _context.JobTypes.Where(job => job.Id == model.Type).FirstOrDefaultAsync();

            if (jobtype == null)
            {
                return false;
            }

            return true;
        }

        // Retrieves a list of available job types
        public async Task<List<JobTypesDto>> GetJobTypesAsync()
        {
            // Maps job types to JobTypesDto and returns the list
            var jobTypes = await _context.JobTypes
                .Select(type => new JobTypesDto
                {
                    Id = type.Id,
                    Name = type.Name,
                }).ToListAsync();

            return jobTypes;
        }
    }
}
