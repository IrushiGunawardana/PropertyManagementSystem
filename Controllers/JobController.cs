using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementSystem.Dtos;
using PropertyManagementSystem.Services.Interfaces;
using System.Security.Claims;

namespace PropertyManagementSystem.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;

        // Constructor that injects the IJobService dependency
        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        // Authorizes access to the endpoint and retrieves all jobs for the current user
        [Authorize]
        [HttpGet("getalljobs")]
        public async Task<IActionResult> GetAllJobs()
        {
            // Retrieve the user ID from the claims in the JWT token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Calls the service to get all jobs associated with the current user
            var JobTypes = await _jobService.GetAllJobsAsync(userId);

            // Returns a success response with all jobs retrieved
            return Ok(new CommonResponseDto
            {
                message = "All Job retrieved successfully",
                data = JobTypes
            });
        }

        // Authorizes access and retrieves job details by job ID for the current user
        [Authorize]
        [HttpGet("getjobdetails/{jobid}")]
        public async Task<IActionResult> GetJobDetails(string jobid)
        {
            // Retrieve the user ID from the claims in the JWT token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Calls the service to get job details by job ID
            var jobDetails = await _jobService.GetJobByIdAsync(userId, jobid);

            // Returns a success response with the job details
            return Ok(new CommonResponseDto
            {
                message = "Job details retrieved successfully",
                data = jobDetails
            });
        }

        // Authorizes access and creates a new job based on the data provided in the request body
        [Authorize]
        [HttpPost("createnewjob")]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobRequestDto model)
        {
            // Retrieve the user ID from the claims in the JWT token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Checks if the job request is valid based on custom logic in the service
            if (!await _jobService.isValid(userId, model))
            {
                // If invalid, return a BadRequest response
                return (IActionResult)Results.BadRequest();
            }

            // Calls the service to create the new job and retrieves the created job
            Models.Schema.Job job = await _jobService.CreateJobAsync(userId, model);

            // Returns a success response with the job details of the newly created job
            return Ok(new CommonResponseDto
            {
                message = "Job posted successfully",
                data = new
                {
                    job.Id,
                    job.ServiceProviderId,
                    job.JobNumber,
                    job.JobType.Name,
                    job.Description
                }
            });
        }

        // Authorizes access and retrieves all job types
        [Authorize]
        [HttpGet("getjobtypes")]
        public async Task<IActionResult> GetJobTypes()
        {
            // Calls the service to get all job types
            var JobTypes = await _jobService.GetJobTypesAsync();

            // Returns a success response with the list of job types
            return Ok(new CommonResponseDto
            {
                message = "Job types retrieved successfully",
                data = JobTypes
            });
        }
    }
}
