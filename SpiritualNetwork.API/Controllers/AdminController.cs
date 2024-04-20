using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : ApiBaseController
    {
        private readonly IAdminService _adminService;
        private readonly IJobService _jobService;

        public AdminController(IAdminService adminService, IJobService jobService)
        {
            _adminService = adminService;
            _jobService = jobService;

        }
        [AllowAnonymous]
        [HttpGet(Name = "GetAllCandidate")]
        public async Task<JsonResponse> GetAllCandidate()
        {
            try
            {
                return await _adminService.GetAllCandidate(13);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet(Name = "GetAllRecuiter")]
        public async Task<JsonResponse> GetAllRecuiter()
        {
            try
            {
                return await _adminService.GetAllRecuiter(13);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "GetJob")]
        public async Task<JsonResponse> GetJob(getJobReq req)
        {
            try
            {
                return await _jobService.GetAllJobs(req, 15);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
    }
}
