using Microsoft.AspNetCore.Mvc;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JobController : ApiBaseController
    {
        private readonly IJobService _jobService;

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpPost(Name = "SaveUpdateExperience")]
        public async Task<JsonResponse> SaveUpdateExperience(JobExperience req)
        {
            try
            {
                var response = await _jobService.SaveUpdateExperience(req);
                return new JsonResponse(200, true, "Success", response);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "DeleteExperience")]
        public async Task<JsonResponse> DeleteExperience(int Id)
        {
            try
            {
                var response = await _jobService.DeleteExperience(Id);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "SaveUpdateJobPost")]
        public async Task<JsonResponse> SaveUpdateJobPost(JobPostReq req, int userId)
        {
            try
            {
                var response = await _jobService.SaveUpdateJobPost(req, user_unique_id);
                return new JsonResponse(200, true, "Success", response);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "DeleteJobPost")]
        public async Task<JsonResponse> DeleteJobPost(int Id)
        {
            try
            {
                return await _jobService.DeleteJobPost(Id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }


        [HttpPost(Name = "SaveApplication")]
        public async Task<JsonResponse> SaveApplication(int jobId)
        {
            try
            {
                var response = await _jobService.SaveApplication(jobId,user_unique_id);
                return new JsonResponse(200, true, "Success", response);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
    }
}
