using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;
using System.Text.Json;

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
        public async Task<JobPost> SaveUpdateJobPost(JobPostReq req, int userId)
        {
            if (req.JobId == 0)
            {
                JobPost jobPost = new JobPost();
                jobPost.Id = 0;
                jobPost.CreatedBy = userId;
                jobPost.JobTitle = req.JobTitle;
                jobPost.CompanyInfo = req.CompanyInfo;
                jobPost.JobDescription = req.JobDescription;
                jobPost.RequiredQualification = req.RequiredQualification;
                jobPost.NoOfVaccancy = req.NoOfVaccancy;
                jobPost.ApplicationDeadline = req.ApplicationDeadline;
                jobPost.SkillsRequired = req.SkillsRequired;
                jobPost.MinSalary = req.MinSalary;
                jobPost.MaxSalary = req.MaxSalary;
                await _jobPostRepository.InsertAsync(jobPost);

                return jobPost;
            }
            else
            {
                var job = await _jobPostRepository.Table.Where(x => x.Id == req.JobId).FirstOrDefaultAsync();
                job.CreatedBy = userId;
                job.JobTitle = req.JobTitle;
                job.CompanyInfo = req.CompanyInfo;
                job.JobDescription = req.JobDescription;
                job.RequiredQualification = req.RequiredQualification;
                job.NoOfVaccancy = req.NoOfVaccancy;
                job.ApplicationDeadline = req.ApplicationDeadline;
                job.SkillsRequired = req.SkillsRequired;
                job.MinSalary = req.MinSalary;
                job.MaxSalary = req.MaxSalary;
                await _jobPostRepository.UpdateAsync(job);

                return job;
            }
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

        public async Task<JsonResponse> GetAllJobs(getJobReq req)
        {
            DateTime fromdate = DateTime.Today.Date, todate = DateTime.Today.Date;
            if (req.TimePeriod == 1)
            {
                fromdate = DateTime.Today.Date;
                todate = DateTime.Today.Date;
            }
            else if (req.TimePeriod == 2)
            {
                DateTime today = DateTime.Today;
                DateTime endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

                fromdate = today;
                todate = endOfMonth;
            }
            else if (req.TimePeriod == 3)
            {
                DateTime today = DateTime.Today;
                DateTime firstofNextMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month)).AddDays(1);
                today = firstofNextMonth;
                DateTime lastofNextMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
                fromdate = firstofNextMonth;
                todate = lastofNextMonth;
            }

            var filterJob = _jobPostRepository.Table;
            if (req.TimePeriod > 0)
            {
                filterJob = filterJob.Where(x => x.CreatedDate >= fromdate && x.CreatedDate <= todate);
            }
            if (req.SearchText != null && filterJob.Count() > 0)
            {
                filterJob = filterJob.Where(x => x.CompanyInfo.ToLower().Contains(req.SearchText.ToLower()) ||
                                            x.RequiredQualification.ToLower().Contains(req.SearchText.ToLower()) ||
                                            x.JobTitle.ToLower().Contains(req.SearchText.ToLower()));
            }
            if (req.MinSalary > 0 && req.MaxSalary > 0)
            {
                filterJob = filterJob.Where(x => x.MinSalary >= req.MinSalary && x.MaxSalary <= req.MaxSalary);
            }

            //if (req.Skills != null)
            //{
            //    string[] skillsArray = req.Skills.Split(',');
            //    filterJob = filterJob.Where(x => x.SkillsRequired != null &&
            //        x.SkillsRequired.Split(',').Any(skill => skillsArray.Contains(skill)));
            //}

            var query = await (from jp in filterJob
                               join ud in _userRepository.Table on jp.CreatedBy equals ud.Id
                               select new GetAllJobsResponse
                               {
                                   Id = jp.Id,
                                   JobTitle = jp.JobTitle,
                                   CompanyName = jp.CompanyInfo,
                                   JobDescription = jp.JobDescription,
                                   RequiredQualification = jp.RequiredQualification,
                                   NumberOfVacancies = jp.NoOfVaccancy,
                                   ApplicationDeadline = jp.ApplicationDeadline,
                                   SkillsRequired = jp.SkillsRequired,
                                   PostedBy = ud.FirstName + " " + ud.LastName,
                                   MinSalary = jp.MinSalary,
                                   MaxSalary = jp.MaxSalary
                               }).ToListAsync();
            foreach (var item in query)
            {
                item.ApplicationReceived = _applicationRepository.Table.Where(x => x.JobId == item.Id 
                                            && x.IsDeleted == false).Count(); 
            }

            return new JsonResponse(200, true, "Success", query);
        }

        public async Task<JsonResponse> GetJobById(int JobId)
        {
            var query = await (from jp in _jobPostRepository.Table.Where(x=> x.Id == JobId && x.IsDeleted == false)
                               join ud in _userRepository.Table on jp.CreatedBy equals ud.Id
                               select new GetAllJobsResponse
                               {
                                   Id = jp.Id,
                                   JobTitle = jp.JobTitle,
                                   CompanyName = jp.CompanyInfo,
                                   JobDescription = jp.JobDescription,
                                   RequiredQualification = jp.RequiredQualification,
                                   NumberOfVacancies = jp.NoOfVaccancy,
                                   ApplicationDeadline = jp.ApplicationDeadline,
                                   SkillsRequired = jp.SkillsRequired,
                                   PostedBy = ud.FirstName + " " + ud.LastName,
                                   MinSalary = jp.MinSalary,
                                   MaxSalary = jp.MaxSalary
                               }).FirstOrDefaultAsync();

            query.ApplicationReceived = _applicationRepository.Table.Where(x => x.JobId == JobId
                                            && x.IsDeleted == false).Count();


            return new JsonResponse(200, true, "Success", query);
        }

    }
}
