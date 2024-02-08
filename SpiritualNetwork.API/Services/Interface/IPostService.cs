using SpiritualNetwork.API.Model;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface IPostService
    {
        public Task<JsonResponse> InsertPost(IFormCollection form, int Id, string Username);
        public Task<JsonResponse> GetAllPostsAsync(int Id, int PageNo,int? ProfileUserId,string? Type);
        public Task<JsonResponse> GetPostById(int loginUserId, int postId);
        public Task<JsonResponse> RePost(int PostId, int UserId);
        public Task<JsonResponse> UpdateCount(int PostId, string Type, int dir);
        public Task<JsonResponse> DeletePostAsync(int PostId);
        public Task<UserPost> GetUserPostByPostId(int PostId);
        public Task<JsonResponse> BlockUnBlockPosts(int PostId, int UserId);
        public Task<UserPost> PinUnpinPost(int PostId);
        public Task<JsonResponse> PostMentionList(MentionListReq req, int loginUserid);

        void UpdatePost();
        Task<UserPost> ChangeWhoCanReply(int postId, int whoCanReply);
    }
}
