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

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllJobs()
        {
            //
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var JobTypes = await _jobService.GetAllJobsAsync(userId);

            return Ok(new CommonResponseDto
            {
                message = "All Job retrieved successfully",
                data = JobTypes
            });

        }

        [Authorize]
        [HttpGet("details/{jobid}")]
        public async Task<IActionResult> GetJobDetails(string jobid)
        {
            //
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var jobDetails = await _jobService.GetJobByIdAsync(userId, jobid);

            return Ok(new CommonResponseDto
            {
                message = "Job details retrieved successfully",
                data = jobDetails
            });

        }


        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobRequestDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await _jobService.isValid(userId, model))
            {
                return (IActionResult)Results.BadRequest();
            }


            Models.Schema.Job job = await _jobService.CreateJobAsync(userId, model);
            return Ok(new CommonResponseDto
            {
                message = "Job posted successfully",
                data = new {
                    job.Id,
                    job.ServiceProviderId,
                    job.JobNumber,
                    job.JobType.Name,
                    job.Description}
            });
        }


        [Authorize]
        [HttpGet("types")]
        public async Task<IActionResult> GetJobTypes()
        {
            var JobTypes = await _jobService.GetJobTypesAsync();

            return Ok( new CommonResponseDto
            {
                message = "Job types retrieved successfully",
                data = JobTypes
            });
        }

  
    }
}
