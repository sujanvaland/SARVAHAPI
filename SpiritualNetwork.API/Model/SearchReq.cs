using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Model
{
    public class SearchReq
    {
        public string Name { get; set; }
    }


    public class SearchReqByPage
    {
        public string Name { get; set; }
        public int PageNo { get; set; }
        public int Records { get; set; }

    }

    public class SearchUserResModel
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImg { get; set; }
        public bool? Online { get; set; }
        public string? UniqueId { get; set; }
        public bool? IsInvited { get; set; }

    }

}
