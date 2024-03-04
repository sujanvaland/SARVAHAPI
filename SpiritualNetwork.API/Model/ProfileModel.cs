using SpiritualNetwork.Entities;
using System.ComponentModel.DataAnnotations;

namespace SpiritualNetwork.API.Model
{
    public class ProfileModel : User
    {
        public bool IsPremium { get; set; }
        public OnlineUsers ConnectionDetail { get; set; }
        public int NoOfFollowers { get; set; }
        public int NoOfFollowing { get; set; }
        public int IsFollowedByLoginUser { get; set; }
        public int? IsFollowingLoginUser { get; set; }
        public List<JobExperience> Experience { get; set; }
    }

    public class ProfileReqest 
    {
        public string? About { get; set; }
        public DateTime? DOB { get; set; }
        [MaxLength(10)]
        public string? Gender { get; set; }
        [MaxLength(200)]
        public string? Location { get; set; }
        [MaxLength(200)]
        public string? Profession { get; set; }
        [MaxLength(200)]
        public string? Organization { get; set; }
        [MaxLength(100)]
        public string? Title { get; set; }
        [MaxLength(200)]
        public string? FacebookLink { get; set; }
        [MaxLength(200)]
        public string? LinkedinLink { get; set; }
        [MaxLength(200)]
        public string? Skills { get; set; }
        [MaxLength(1000)]
        public string? ProfileImg { get; set; }
        [MaxLength(1000)]
        public string? BackgroundImg { get; set; }
        [MaxLength(1000)]
        public string? Tags { get; set; }
        [MaxLength(200)]
        public string? SchoolName { get; set; }
        [MaxLength(200)]
        public string? BoardName { get; set; }
        [MaxLength(200)]
        public string? CollegeName { get; set; }
        [MaxLength(200)]
        public string? Degree { get; set; }
        [MaxLength(200)]
        public int? YearOfPassing { get; set; }
        public int? TotalExperience { get; set; }
        [MaxLength(200)]
        public string? University { get; set; }
        [MaxLength(200)]
        public string? HighestQualification { get; set; }
        [MaxLength(200)]
        public string? Course { get; set; }
        [MaxLength(200)]
        public string? Specialization { get; set; }
        public DateTime? StartingYear { get; set; }
        public DateTime? PassingYear { get; set; }
        [MaxLength(200)]
        public string? Grades { get; set; }
        public List<JobExperience> Experience { get; set; }

    }

    public class Education
    {
        public string? University { get; set; }
        public string? Qualification { get; set; }
        public string? Course { get; set; }
        public string? Specialization { get; set; }
        public DateTime? StartingYear { get; set; }
        public DateTime? PassingYear { get; set; }
        public int? Grades { get; set; }

    }

    public class Experiences
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [MaxLength(200)]
        public string? Company { get; set; }
        [MaxLength(200)]
        public string? JoinedDate { get; set; }
        [MaxLength(200)]
        public string? EndDate { get; set; }
        [MaxLength(200)]
        public string? Designation { get; set; }
        [MaxLength(200)]
        public string? Responsibility { get; set; }
    }

    public class UserFollowersModel
    {
        public UserFollowersModel()
        {
            Followers = new List<ProfileModel>();
            Following = new List<ProfileModel>();
            Mutual = new List<ProfileModel>();
        }
        public List<ProfileModel> Followers { get; set; }
        public List<ProfileModel> Following { get; set; }
        public List<ProfileModel> Mutual { get; set; }

    }

    public class Mentions
    {
        public int userId { get; set; }
        public string name { get; set; }
        public string link { get; set; }
        public string avatar { get; set; }
    }

    public class TagUser
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string userName { get; set; }

    }
}
