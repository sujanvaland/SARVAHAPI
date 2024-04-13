using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;
using System.Runtime.CompilerServices;

namespace SpiritualNetwork.API.Services
{
    public class ProfileService : IProfileService
    {

        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Books> _bookRepository;
        private readonly IRepository<Movies> _movieRepository;
        private readonly IRepository<Gurus> _guruRepository;
        private readonly IRepository<Practices> _practiceRepository;
        private readonly IRepository<Experience> _experienceRepository;
        private readonly IRepository<OnlineUsers> _onlineUsers;
        private readonly IRepository<UserProfileSuggestion> _profilesuggestionRepo;
        private readonly IRepository<JobExperience> _jobExperienceRepository;
        private readonly IRepository<Certificate> _certificateRepository;
        private readonly IRepository<UserSubcription> _userSubcriptionRepo;
        private readonly IMapper _mapper;

        public ProfileService(IRepository<User> userRepository, 
            IRepository<Books> bookRepository, 
            IRepository<Movies> movieRepository, 
            IRepository<Gurus> guruRepository, 
            IRepository<Practices> practiceRepository, 
            IRepository<Experience> experienceRepository,
            IRepository<OnlineUsers> onlineUsers,
            IRepository<UserProfileSuggestion> profilesuggestionRepo,
            IRepository<UserSubcription> userSubcriptionRepo,
            IRepository<JobExperience> jobExperienceRepository,
            IRepository<Certificate> certificateRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _bookRepository = bookRepository;
            _movieRepository = movieRepository;
            _guruRepository = guruRepository;
            _practiceRepository = practiceRepository;
            _experienceRepository = experienceRepository;
            _onlineUsers = onlineUsers;
            _profilesuggestionRepo = profilesuggestionRepo;
            _userSubcriptionRepo = userSubcriptionRepo;
            _jobExperienceRepository = jobExperienceRepository;
            _certificateRepository = certificateRepository;
            _mapper = mapper;
        }

        public async Task<JsonResponse> UpdateProfile(ProfileReqest profileReq, int UserId)
        {

            try
            {
                var profileData = await _userRepository.Table.Where(x => x.Id == UserId 
                                    && x.IsDeleted == false).FirstOrDefaultAsync();
                //profileData = _mapper.Map<User>(profileData);
                profileData.About = profileReq.About;
                profileData.DOB = profileReq.DOB;
                profileData.Gender = profileReq.Gender;
                profileData.Location = profileReq.Location;
                profileData.Profession = profileReq.Profession;
                profileData.Organization = profileReq.Organization;
                profileData.Title = profileReq.Title;
                profileData.FacebookLink = profileReq.FacebookLink;
                profileData.LinkedinLink = profileReq.LinkedinLink;
                profileData.Skills = profileReq.Skills;
                profileData.ProfileImg = profileReq.ProfileImg;
                profileData.BackgroundImg = profileReq.BackgroundImg;
                profileData.Tags = profileReq.Tags;
                profileData.TotalExperience = profileReq.TotalExperience;
                profileData.University = profileReq.University;
                profileData.HighestQualification = profileReq.HighestQualification;
                profileData.Course = profileReq.Course;
                profileData.Specialization = profileReq.Specialization;
                profileData.StartingYear = profileReq.StartingYear;
                profileData.PassingYear = profileReq.PassingYear;
                profileData.Grades = profileReq.Grades;
                profileData.ResumeId = profileReq.ResumeId;

                await _userRepository.UpdateAsync(profileData);
                
                foreach (var item in profileReq.Experience)
                {
                    if (item.Id == 0)
                    {
                        item.UserId = UserId;
                        await _jobExperienceRepository.InsertAsync(item);
                    }
                    else
                    {
                        await _jobExperienceRepository.UpdateAsync(item);
                    }
                }

                foreach (var item in profileReq.Certificate)
                {
                    if (item.Id == 0)
                    {
                        item.UserId = UserId;
                        await _certificateRepository.InsertAsync(item);
                    }
                    else
                    {
                        await _certificateRepository.UpdateAsync(item);
                    }
                }


                return new JsonResponse(200, true, "Profile Updated Successfully", profileData);

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        public async Task<ProfileModel> GetUserProfileById(int Id)
        {
            try
            {
                var user = await _userRepository.Table.Where(x => x.Id == Id).FirstOrDefaultAsync();
                var permiumcheck = await _userSubcriptionRepo.Table.Where(x => x.UserId == Id &&
                                    x.PaymentStatus == "completed" && x.IsDeleted == false).FirstOrDefaultAsync();
                ProfileModel profileModel = _mapper.Map<ProfileModel>(user);
                if (permiumcheck != null)
                {
                    profileModel.IsPremium = true;
                }
                else { profileModel.IsPremium = false; }
                profileModel.ConnectionDetail = _onlineUsers.GetById(user.Id);
                return profileModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ProfileModel> GetUserProfileByUsername(string username)
        {
            try
            {
                var user = await _userRepository.Table.Where(x => x.UserName == username).FirstOrDefaultAsync();
                var permiumcheck = await _userSubcriptionRepo.Table.Where(x => x.UserId == user.Id &&
                                    x.PaymentStatus == "completed" && x.IsDeleted == false).FirstOrDefaultAsync();
               
                ProfileModel profileModel = _mapper.Map<ProfileModel>(user);
                if (permiumcheck != null)
                {
                    profileModel.IsPremium = true;
                }
                else { profileModel.IsPremium = false; }
                profileModel.ConnectionDetail = _onlineUsers.GetById(user.Id);

                profileModel.Experience = _jobExperienceRepository.Table.Where(x=> x.UserId == profileModel.Id).ToList();
                profileModel.Certificate = _certificateRepository.Table.Where(x=> x.UserId == profileModel.Id).ToList();
                return profileModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ProfileModel> GetUserInfoBox(string username, int UserId)
        {
            try
            {
                var user = await _userRepository.Table.Where(x => x.UserName == username).FirstOrDefaultAsync();
                var permiumcheck = await _userSubcriptionRepo.Table.Where(x => x.UserId == user.Id &&
                                   x.PaymentStatus == "completed" && x.IsDeleted == false).FirstOrDefaultAsync();

                ProfileModel profileModel = _mapper.Map<ProfileModel>(user);
                if (permiumcheck != null)
                {
                    profileModel.IsPremium = true;
                }
                else { profileModel.IsPremium = false; }
                profileModel.ConnectionDetail = _onlineUsers.GetById(user.Id);
                return profileModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ProfileModel> GetUserInfoBoxByUserId(int UserId, int LoginUserId)
        {
            try
            {
                var user = await _userRepository.Table.Where(x => x.Id == UserId).FirstOrDefaultAsync();
                ProfileModel profileModel = _mapper.Map<ProfileModel>(user);
                profileModel.IsPremium = false;
                profileModel.ConnectionDetail = _onlineUsers.GetById(user.Id);
                return profileModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ProfileModel GetUserProfile(User user)
        {
            try
            {
                
                ProfileModel profileModel = _mapper.Map<ProfileModel>(user);
                profileModel.IsPremium = false;
                profileModel.ConnectionDetail = _onlineUsers.GetById(user.Id);
                return profileModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ProfileModel>> GetUserProfile(List<User> users)
        {
            try
            {
                List<ProfileModel> profiles = new List<ProfileModel>();
                foreach ( var user in users)
                {
                    ProfileModel profileModel = _mapper.Map<ProfileModel>(user);
                    profileModel.IsPremium = false;
                    profileModel.ConnectionDetail = _onlineUsers.GetById(user.Id);
                    profiles.Add(profileModel);
                }
                return profiles;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> GetBooksSuggestion(int userId)
        {
            try
            {
                var result = await (from u in _profilesuggestionRepo.Table
                                    join b in _bookRepository.Table
                                    on u.SuggestedId equals b.Id
                                    where u.UserId == userId && u.IsDeleted == false 
                                    && b.IsDeleted == false  && u.Type == "book"
                                    select new
                                    {
                                        u.Id,
                                        u.SuggestedId,
                                        b.Author,
                                        b.BookImg,
                                        b.BookName,
                                        u.IsRead
                                    }).ToListAsync();

                return new JsonResponse(200, true, "success", result);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> GetMoviesSuggestion(int userId)
        {
            try
            {
                // var result = await _movieRepository.Table.Where(x => x.IsDeleted == false).ToListAsync();
                var result = await (from u in _profilesuggestionRepo.Table
                                    join m in _movieRepository.Table
                                    on u.SuggestedId equals m.Id
                                    where u.UserId == userId && u.IsDeleted == false
                                    && m.IsDeleted == false && u.Type == "movie"
                                    select new
                                    {
                                        u.Id,
                                        u.SuggestedId,
                                        m.MovieName,
                                        m.MovieImg,
                                        u.IsRead
                                    }).ToListAsync();

                return new JsonResponse(200, true, "success", result);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> GetGurusSuggestion(int userId)
        {
            try
            {
                //var result = await _guruRepository.Table.Where(x => x.IsDeleted == false).ToListAsync();

                var result = await (from u in _profilesuggestionRepo.Table
                                    join g in _guruRepository.Table
                                    on u.SuggestedId equals g.Id
                                    where u.UserId == userId && u.IsDeleted == false
                                    && g.IsDeleted == false && u.Type == "guru"
                                    select new
                                    {
                                        u.Id,
                                        u.SuggestedId,
                                        g.GuruImg,
                                        g.GuruName,
                                        u.IsRead
                                    }).ToListAsync();

                return new JsonResponse(200, true, "success", result);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> GetPracticeSuggestion(int userId)
        {
            try
            {
                // var result = await _practiceRepository.Table.Where(x => x.IsDeleted == false).ToListAsync();

                var result = await (from u in _profilesuggestionRepo.Table
                                    join p in _practiceRepository.Table
                                    on u.SuggestedId equals p.Id
                                    where u.UserId == userId && u.IsDeleted == false
                                    && p.IsDeleted == false && u.Type == "practice"
                                    select new
                                    {
                                        u.Id,
                                        u.SuggestedId,
                                        p.PracticeImg,
                                        p.PracticeName,
                                        u.IsRead
                                    }).ToListAsync();

                return new JsonResponse(200, true, "success", result);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> GetExperienceSuggestion(int userId)
        {
            try
            {
                //var result = await _experienceRepository.Table.Where(x => x.IsDeleted == false).ToListAsync();

                var result = await (from u in _profilesuggestionRepo.Table
                                    join e in _experienceRepository.Table
                                    on u.SuggestedId equals e.Id
                                    where u.UserId == userId && u.IsDeleted == false
                                    && e.IsDeleted == false && u.Type == "practice"
                                    select new
                                    {
                                        u.Id,
                                        u.SuggestedId,
                                        e.ExperienceImg,
                                        e.ExperienceName,
                                        u.IsRead
                                    }).ToListAsync();

                return new JsonResponse(200, true, "success", result);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> SearchSuggestion(SearchProfileSuggestion req)
        {
            try
            {
                if (req.Type == "book")
                {
                    var book = await _bookRepository.Table.Where(x => x.Author.Contains(req.Name) || 
                                x.BookName.Contains(req.Name) && x.IsDeleted == false).
                                Select(x=>  new SuggestRes
                                {
                                   Id = x.Id,
                                   Img = x.BookImg,
                                   Name = x.BookName,
                                   Author = x.Author

                                }).ToListAsync();
                    return new JsonResponse(200, true, "success", book);
                }
                if (req.Type == "movie")
                {
                    var movie = await _movieRepository.Table.Where(x => x.MovieName.Contains(req.Name) 
                                && x.IsDeleted == false).Select(x => new SuggestRes
                                {
                                    Id = x.Id,
                                    Img = x.MovieImg,
                                    Name = x.MovieName,
                                    Author = ""

                                }).ToListAsync();
                    return new JsonResponse(200, true, "success", movie);
                }
                if (req.Type == "guru")
                {
                    var guru = await _guruRepository.Table.Where(x => x.GuruName.Contains(req.Name) 
                                && x.IsDeleted == false).Select(x => new SuggestRes
                                {
                                    Id = x.Id,
                                    Img = x.GuruImg,
                                    Name = x.GuruName,
                                    Author = ""

                                }).ToListAsync();
                    return new JsonResponse(200, true, "success", guru);
                }
                if (req.Type == "practice")
                {
                    var practice = await _practiceRepository.Table.Where(x => x.PracticeName.Contains(req.Name) 
                                    && x.IsDeleted == false).Select(x => new SuggestRes
                                    {
                                        Id = x.Id,
                                        Img = x.PracticeImg,
                                        Name = x.PracticeName,
                                        Author = ""

                                    }).ToListAsync();
                    return new JsonResponse(200, true, "success", practice);
                }
                if (req.Type == "experience")
                {
                    var experience = await _experienceRepository.Table.Where(x => x.ExperienceName.Contains(req.Name)
                                      && x.IsDeleted == false).Select(x => new SuggestRes
                                      {
                                          Id = x.Id,
                                          Img = x.ExperienceImg,
                                          Name = x.ExperienceName,
                                          Author = ""

                                      }).ToListAsync();
                    return new JsonResponse(200, true, "success", experience);
                }

                return new JsonResponse(200, true, "Not Found", null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> AddSuggestion(BookMarkRes res, int UserId)
        {
            try
            {
                var check = await _profilesuggestionRepo.Table.Where(x => x.Type == res.Type
                            && x.UserId == UserId && x.SuggestedId == res.Id).FirstOrDefaultAsync();

                if (check != null)
                {
                    if (check.IsDeleted)
                    {
                        check.IsRead = false;
                        check.IsDeleted = false;
                        _profilesuggestionRepo.Update(check);
                        return new JsonResponse(200, true, "Saved Success", null);
                    }
                    else
                    {
                        return new JsonResponse(200, true, "You Have Already Saved this "+check.Type, null);
                    }

                }

                UserProfileSuggestion list = new UserProfileSuggestion();
                list.UserId = UserId;
                list.SuggestedId = res.Id;
                list.Type = res.Type;
                list.IsRead = false;
                await _profilesuggestionRepo.InsertAsync(list);
                return new JsonResponse(200, true, "Saved Success", null);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> DeleteProfileSuggestion(int Id)
        {
            try
            {
                var data = _profilesuggestionRepo.Table.Where(x => x.Id == Id).FirstOrDefault();
                await _profilesuggestionRepo.DeleteAsync(data);
                return new JsonResponse(200, true, "Success", null);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> UpdateIsReadSuggestion(int Id)
        {
            try
            {
                var data = await _profilesuggestionRepo.Table.Where(x => x.Id == Id).FirstOrDefaultAsync();
                if (data.IsRead)
                {
                    data.IsRead = false;
                    _profilesuggestionRepo.Update(data);
                    return new JsonResponse(200, true, "Mark Read", data);
                }
                if (!data.IsRead)
                {

                    data.IsRead = true;
                    _profilesuggestionRepo.Update(data);
                    return new JsonResponse(200, true, "Mark UnRead", data);

                }
                return new JsonResponse(200, true, "Success", null);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
