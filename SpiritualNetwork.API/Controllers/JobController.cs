using Microsoft.AspNetCore.Authorization;
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

        [HttpPost(Name = "SaveUpdateRecuiterProfile")]
        public async Task<JsonResponse> SaveUpdateRecuiterProfile(Recuiter req)
        {
            try
            {
                var response = await _jobService.SaveUpdateRecuiterProfile(req);
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
        public async Task<JsonResponse> SaveUpdateJobPost(JobPostReq req)
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
        public async Task<JsonResponse> SaveApplication(JobApplyReq req)
        {
            try
            {
                var response = await _jobService.SaveApplication(req.JobId, user_unique_id);
                return new JsonResponse(200, true, "Success", response);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "GetAllJobs")]
        public async Task<JsonResponse> GetAllJobs(getJobReq req)
        {
            try
            {
                return await _jobService.GetAllJobs(req,15,user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "GetJobById")]
        public async Task<JsonResponse> GetJobById(getJobIdReq req)
        {
            try
            {
                return await _jobService.GetJobById(req.JobId,user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "ToggleBookmark")]
        public async Task<JsonResponse> ToggleBookmark(ReactionReq req)
        {
            try
            {
                return await _jobService.ToggleBookmark(req.PostId, user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "GetAllBookmarkJobs")]
        public async Task<JsonResponse> GetAllBookmarkJobs()
        {
            try
            {
                return await _jobService.GetAllBookmarkJobs(user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
        [HttpPost(Name = "GetAllApplications")]
        public async Task<JsonResponse> GetAllApplications(getJobIdReq req)
        {
            try
            {
                return await _jobService.GetAllApplications(req.JobId, user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
        [HttpGet(Name = "GetResume")]
        public async Task<JsonResponse> GetResume()
        {
            try
            {
                return await _jobService.GetResume(user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
    }
}
