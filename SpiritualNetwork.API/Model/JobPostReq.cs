using System.ComponentModel.DataAnnotations;

namespace SpiritualNetwork.API.Model
{
    public class JobPostReq
    {
        public int JobId { get; set; }
        public string? JobTitle { get; set; }
        public string? CompanyInfo { get; set; }
        public string? JobDescription { get; set; }
        public string? RequiredQualification { get; set; }
        public DateTime? ApplicationDeadline { get; set; }
        public int? NoOfVaccancy { get; set; }
        public string? SkillsRequired { get; set; }
        public int? MinSalary { get; set; }
        public int? MaxSalary { get; set; }
    }
    
    public class JobApplyReq
    {
        public int JobId { get; set;}
    }
}
