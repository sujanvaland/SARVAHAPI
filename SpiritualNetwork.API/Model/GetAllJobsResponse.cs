namespace SpiritualNetwork.API.Model
{
    public class GetAllJobsResponse
    {
        public int Id { get; set; } 
        public string? JobTitle { get; set; }
        public string? CompanyName { get; set; }
        public string? JobDescription { get; set; }
        public string? RequiredQualification { get; set; }
        public int? NumberOfVacancies { get; set; }
        public DateTime? ApplicationDeadline { get; set; }
        public string? SkillsRequired { get; set; }
        public string? PostedBy { get; set; }
        public int? ApplicationReceived { get; set; }
        public int? MinSalary { get; set; }
        public int? MaxSalary { get; set; }
        public int? IsBookmarked { get; set;}
        public int IsApplied { get; set;}   
        public int TotalCount { get; set; }
        public int RowNum { get; set; }
        public int TotalSize { get; set;}

    }

    public class getJobReq
    {
        public string? SearchText { get; set; }
        public int? TimePeriod { get; set; }
        public string? Skills { get; set; }
        public int? MinSalary { get; set; }
        public int? MaxSalary { get; set; }
        public int? PageNo { get; set; }
        public string? UserType { get; set; }

    }
    public class getJobIdReq
    {
        public int JobId { get; set; }

    }



}
