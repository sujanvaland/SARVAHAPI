﻿using SpiritualNetwork.API.Model;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface IJobService
    {
        public Task<JobExperience> SaveUpdateExperience(JobExperience req);
        Task<JsonResponse> DeleteExperience(int Id);
        public Task<JobPost> SaveUpdateJobPost(JobPostReq req, int userId);
        public Task<JsonResponse> DeleteJobPost(int Id);
        public Task<Application> SaveApplication(int JobId, int userId);
        public Task<JsonResponse> GetAllJobs(getJobReq req);
    }
}
