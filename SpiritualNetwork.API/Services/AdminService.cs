using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.API.AppContext;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services
{
    public class AdminService : IAdminService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<JobExperience> _jobExperienceRepository;
        private readonly IRepository<JobPost> _jobPostRepository;
        private readonly IRepository<Application> _applicationRepository;
        private readonly IRepository<Reaction> _reactionRepository;
        private readonly IRepository<Recuiter> _recuiterRepository;
        private readonly IRepository<Entities.File> _fileRepository;
        private readonly AppDbContext _context;

        public AdminService(IRepository<User> userRepository,
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

        //public async Task<JsonResponse> GetAllCandidate(int UserId)
        //{
        //    var check = await _userRepository.Table.Where(x => x.Id == UserId).FirstOrDefaultAsync();

        //    if(check.LoginType == "admin")
        //    {
        //        return new JsonResponse(200, false, "Only Admin Can Operate this", null);
        //    }

        //    var query = await (from u in _userRepository.Table.Where(x => x.IsDeleted == false)
        //                       join )
        //}
    }
}
