namespace SpiritualNetwork.API.Model
{
    public class GetAllCandidate
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string ResumeUrl { get; set; }
        public string? Skills { get; set; }
        public string? ProfileImg { get; set; }
        public int? TotalExperience { get; set; }
        public string? Specialization { get; set; }
        public string? Tags { get; set; }
        public string? PhoneNumber { get; set; }
        public int? TotalJobsApplied { get; set; }
    }

    public class GetAllRecuiter
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string? Skills { get; set; }
        public string? ProfileImg { get; set; }
        public int? TotalExperience { get; set; }
        public string? Specialization { get; set; }
        public string? Tags { get; set; }
        public string? PhoneNumber { get; set; }
        public int? TotalJobsPost { get; set; }
    }
}
