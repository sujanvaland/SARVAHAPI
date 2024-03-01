using System.ComponentModel.DataAnnotations;

namespace SpiritualNetwork.Entities
{
    public class User : BaseEntity
    {
        [MaxLength(100)]
        public string UserName { get; set; }
        public int InviterId { get; set; }
        [MaxLength(200)]
        public string Password { get; set; }
        [MaxLength(100)]
        public string FirstName { get; set; }
        [MaxLength(100)]
        public string LastName { get; set; }
        [MaxLength(200)]
        public string Email { get; set; }
        public int? IsEmailVerified { get; set; }
        [MaxLength(15)]
        public string? PhoneNumber { get; set; }
        public int? IsPhoneVerified { get; set; }
        [MaxLength(10)]
        public string? Status { get; set; }
        [MaxLength(100)]
        public string? PaymentStatus { get; set; }
        [MaxLength(100)]
        public string PaymentRef1 { get; set; }
        [MaxLength(100)]
        public string? PaymentRef2 { get; set; }
        [MaxLength(200)]
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
        public string? LoginMethod { get; set;}

        [MaxLength(100)]
        public string? LoginType { get; set; }
        public DateTime? ReActivationDate { get; set; }

        [MaxLength(200)]
        public string? SchoolName { get; set; }
        [MaxLength(200)]
        public string? BoardName { get; set; }
        [MaxLength(200)]
        public string? CollegeName { get; set; }
        [MaxLength(200)]
        public string? University { get; set; }
        [MaxLength(200)]
        public string? Degree { get; set; }
        [MaxLength(200)]
        public string? Course { get; set; }
        public int? YearOfPassing { get; set; }
        public int? ResumeId { get; set; }
    }

    public class JobExperience : BaseEntity
    {
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

    public class JobPost : BaseEntity
    {
        [MaxLength(200)]
        public string? JobTitle { get; set; }
        [MaxLength(2000)]
        public string? CompanyInfo { get; set; }
        [MaxLength(2000)]
        public string? JobDescription { get; set; }
        [MaxLength(500)]
        public string? RequiredQualification { get; set; }
        public DateTime? ApplicationDeadline { get; set; }
        public int? NoOfVaccancy { get; set; }
        [MaxLength(200)]
        public string? SkillsRequired { get; set; }
        public int? MinSalary { get; set; }
        public int? MaxSalary { get; set; }
    }

    public class Application : BaseEntity
    {
        public int? JobId { get; set; }
        public int? ResumeId { get; set; }
        public int? CandidateId { get; set; }

        public DateTime? AppliedOn { get; set; }
        [MaxLength(100)]
        public string? Status { get; set; }

    }
}