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
        public Task<JsonResponse> GetAllJobs(getJobReq req, int size);
        public Task<JsonResponse> ToggleBookmark(int postid, int userid);
        public Task<JsonResponse> GetAllBookmarkJobs(int userId);
        public Task<JsonResponse> GetJobById(int JobId, int userId);
        public Task<Recuiter> SaveUpdateRecuiterProfile(Recuiter req);
        public Task<JsonResponse> GetAllApplications(int UserId, int JobId);
        public Task<JsonResponse> GetResume(int ResumeId);
    }
}
