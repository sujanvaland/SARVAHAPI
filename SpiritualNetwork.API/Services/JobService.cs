using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.API.AppContext;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SpiritualNetwork.API.Services
{
    public class JobService : IJobService
    {
        private readonly IRepository<User> _userRepository; 
        private readonly IRepository<JobExperience> _jobExperienceRepository;
        private readonly IRepository<JobPost> _jobPostRepository;
        private readonly IRepository<Application> _applicationRepository;
        private readonly IRepository<Reaction> _reactionRepository;
        private readonly IRepository<Recuiter> _recuiterRepository;
        private readonly IRepository<Entities.File> _fileRepository;

        private readonly AppDbContext _context;

        public JobService(IRepository<User> userRepository,
            IRepository<JobExperience> jobExperienceRepository,
            IRepository<JobPost> jobPostRepository,
            IRepository<Application> applicationRepository,
            IRepository<Reaction> reactionRepository,
            AppDbContext context,
            IRepository<Recuiter> recuiterRepository,
            IRepository<Entities.File> fileRepository)
        {
            _userRepository = userRepository;
            _jobExperienceRepository = jobExperienceRepository;
            _jobPostRepository = jobPostRepository;
            _applicationRepository = applicationRepository;
            _reactionRepository = reactionRepository;
            _context = context;
            _recuiterRepository = recuiterRepository;
            _fileRepository = fileRepository;
        }

        public async Task<Recuiter> SaveUpdateRecuiterProfile(Recuiter req)
        {
            if (req.Id == 0)
            {
                await _recuiterRepository.InsertAsync(req);
            }
            else
            {
                await _recuiterRepository.UpdateAsync(req);
            }

            return req;
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
        public async Task<JsonResponse> GetAllJobs(getJobReq req,int size, int UserId)
        {
            DateTime fromdate = Convert.ToDateTime("1/1/1753"), todate = Convert.ToDateTime("1/1/1753");
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

            SqlParameter _FromDate = new SqlParameter("@FromDate", fromdate);
            SqlParameter _ToDate = new SqlParameter("@ToDate", todate);
            SqlParameter _SearchText = new SqlParameter("@SearchText", req.SearchText);
            SqlParameter _MinSalary = new SqlParameter("@MinSalary", req.MinSalary);
            SqlParameter _MaxSalary = new SqlParameter("@MaxSalary", req.MaxSalary);
            SqlParameter _PageSize = new SqlParameter("@PageSize", size);
            SqlParameter _PageNumber = new SqlParameter("@PageNumber", req.PageNo);
            SqlParameter _userId = new SqlParameter("@userId", UserId);
            SqlParameter _userType = new SqlParameter("@userType", req.UserType);


            var Result = await _context.GetAllJobsResponse
                .FromSqlRaw("GetFilteredJobs  @FromDate,@ToDate,@SearchText,@MinSalary,@MaxSalary,@PageSize,@PageNumber,@userId,@userType"
                , _FromDate, _ToDate, _SearchText, _MinSalary, _MaxSalary, _PageSize, _PageNumber, _userId, _userType)
                .ToListAsync();

            return new JsonResponse(200, true, "Success", Result);
        }
        public async Task<JsonResponse> GetJobById(int JobId,int userId)
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
                                   MaxSalary = jp.MaxSalary,
                               }).FirstOrDefaultAsync();

            var application = _applicationRepository.Table.Where(x => x.JobId == JobId
                                            && x.IsDeleted == false);
            query.IsApplied = application.Any(x => x.CandidateId == userId) ? 1 : 0 ;
            query.ApplicationReceived = application.Count();

            return new JsonResponse(200, true, "Success", query);
        }
        public async Task<JsonResponse> ToggleBookmark(int postid, int userid)
        {
            try
            {
                var data = await _reactionRepository.Table
                    .Where(x => x.IsDeleted == false &&
                    x.UserId == userid &&
                    x.PostId == postid)
                    .FirstOrDefaultAsync();

                if (data != null)
                {
                    _reactionRepository.DeleteHard(data);
                    return new JsonResponse(200, true, "Success", data);
                }

                Reaction reaction = new Reaction();
                reaction.PostId = postid;
                reaction.UserId = userid;
                reaction.Type = "bookmark";

                await _reactionRepository.InsertAsync(reaction);

                return new JsonResponse(200, true, "Success", reaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResponse> GetAllBookmarkJobs(int userId)
        {
            var query = await (from r in _reactionRepository.Table.Where(x => x.UserId == userId    
                               &&  x.Type == "bookmark" && x.IsDeleted == false)
                               join jp in _jobPostRepository.Table on r.PostId equals jp.Id
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
                                   MaxSalary = jp.MaxSalary,
                                   IsBookmarked = 1
                               }).ToListAsync();

            foreach (var job in query)
            {
                job.ApplicationReceived = _applicationRepository.Table.Where(x=> x.JobId == job.Id).Count();
            }
            return new JsonResponse(200, true, "Success", query);
        }
        
        public async Task<JsonResponse> GetAllApplications (int JobId, int UserId)
        {
            var check = await _jobPostRepository.Table.Where(x => x.Id == JobId
                        && x.IsDeleted == false && x.CreatedBy == UserId).CountAsync() > 0;

            if (!check)
            {
                return new JsonResponse(200, false, "You Are Not recruiter for this Job", null);
            }

            var ap = await _applicationRepository.Table.Where(x => x.JobId == JobId).ToListAsync();

            var List = await (from jp in _applicationRepository.Table.Where(x => x.JobId == JobId && x.IsDeleted == false
                               && x.Status == "applied")
                               join ud in _userRepository.Table on jp.CandidateId equals ud.Id
                               join r in _fileRepository.Table on ud.ResumeId equals r.Id
                               where ud.IsDeleted == false 
                               select new GetAllApplicationsResponse
                               {
                                   UserId = ud.Id,
                                   FirstName = ud.FirstName,
                                   LastName = ud.LastName,
                                   UserName = ud.UserName,
                                   ResumeId = ud.ResumeId,
                                   ResumeUrl = r.ActualUrl,
                                   ApplicationDate = jp.AppliedOn,
                               }).ToListAsync();

            return new JsonResponse(200, true, "Success", List);
        }

        public async Task<JsonResponse> GetResume(int ResumeId)
        {
            var query = await _fileRepository.Table.Where(x=> x.Id  == ResumeId).FirstOrDefaultAsync();
            if (query == null)
            {
                return new JsonResponse(200, false, "Not Found", null);
            }
            return new JsonResponse(200, true, "Success", query);
        }
    }
}
