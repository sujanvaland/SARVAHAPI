using SpiritualNetwork.Entities;

namespace SpiritualNetwork.API.Model
{
    public class ChatHistoryReq
    {
        public int UserId { get; set; }
        public int IsGroup { get; set; }
    }

    public class ChatHistoryResponse
    {

        public ChatProfile UserProfile { get; set; }
        public List<ChatHistory> ChatMessages { get; set; }

    }

    public class ChatProfile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string About { get; set; }
        public DateTime createdDate { get; set; }
        public string profileImg { get; set; }
        public bool IsPremium { get; set; }
        public int NoOfFollowers { get; set; }
        public int NoOfFollowing { get; set; }
        public bool IsGroup { get; set; }
    }

    public class ChatHistory : BaseEntity
    {
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public int ReceiverId { get; set; }
        public string Message { get; set; }
        public string ReplyTo { get; set; }
        public string ReplyMessage { get; set; }
        public int? IsDelivered { get; set; }
        public int? IsRead { get; set; }
        public int? AttachmentId { get; set; }
        public int? DeleteForUserId1 { get; set; }
        public int? DeleteForUserId2 { get; set; }
        public string ThumbnailUrl { get; set; }
        public string ActualUrl { get; set; }
    }
}
