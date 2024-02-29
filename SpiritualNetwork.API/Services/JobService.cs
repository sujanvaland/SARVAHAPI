﻿using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services
{
    public class JobService : IJobService
    {
        private readonly IRepository<User> _userRepository; 
        private readonly IRepository<JobExperience> _jobExperienceRepository;
        private readonly IRepository<JobPost> _jobPostRepository;
        private readonly IRepository<Application> _applicationRepository;



        public JobService(IRepository<User> userRepository,
            IRepository<JobExperience> jobExperienceRepository,
            IRepository<JobPost> jobPostRepository,
            IRepository<Application> applicationRepository)
        {
            _userRepository = userRepository;
            _jobExperienceRepository = jobExperienceRepository;
            _jobPostRepository = jobPostRepository;
            _applicationRepository = applicationRepository;
        }

        public async Task<JobExperience> SaveUpdateExperience(JobExperience req)
        {
            if (req.Id == 0)
            {
                await _jobExperienceRepository.InsertAsync(req);
            }
            else
            {
                await _jobExperienceRepository.UpdateAsync(req);
            }

            return req;
        }

        public async Task<JsonResponse> DeleteExperience(int Id)
        {
            var experience = await _jobExperienceRepository.Table.Where(x => x.Id == Id).FirstOrDefaultAsync();
            await _jobExperienceRepository.DeleteAsync(experience);
            return new JsonResponse(200, true, "Success", null);
        }
        public async Task<JobPost> SaveUpdateJobPost(JobPost req)
        {
            if (req.Id == 0)
            {
                await _jobPostRepository.InsertAsync(req);
            }
            else
            {
                await _jobPostRepository.UpdateAsync(req);
            }

            return req;
        }
        public async Task<JsonResponse> DeleteJobPost(int Id)
        {
            var job = await _jobPostRepository.Table.Where(x => x.Id == Id).FirstOrDefaultAsync();
            await _jobPostRepository.DeleteAsync(job);
            return new JsonResponse(200, true, "Success", null);
        }

        public async Task<Application> SaveApplication(int JobId, int userId)
        {
            var application = await _userRepository.Table.Where(x => x.Id == userId).FirstOrDefaultAsync();

            Application apply = new Application();
            apply.JobId = JobId;
            apply.CandidateId = userId;
            apply.AppliedOn = DateTime.Now;
            apply.ResumeId = application.ResumeId;
            apply.Status = "applied";

            await _applicationRepository.InsertAsync(apply);

            return apply;
        }


    }
}