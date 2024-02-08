using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SpiritualNetwork.API.AppContext;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;
using System.Net.Mail;
using System.Security.Cryptography.Xml;
using System.Text.Json;

namespace SpiritualNetwork.API.Services
{
    public class PostService : IPostService
    {
        private readonly IAttachmentService _attachmentService;
        private readonly INotificationService _notificationService;
        private readonly IRepository<UserPost> _userPostRepository;
        private IRepository<PostFiles> _postFiles;
        private readonly IRepository<Entities.File> _fileRepository;
        private readonly IRepository<Reaction> _reactionRepository;
        private readonly IRepository<BlockedPosts> _blockedPost;
        private readonly AppDbContext _context;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserSubcription> _userSubcriptionRepo;
        private readonly IPollService _pollService;
        private readonly IProfileService _profileService;

        public PostService(IAttachmentService attachmentService,
            IRepository<UserSubcription> userSubcriptionRepo,
            INotificationService notificationService,
            IRepository<UserPost> userPostRepository,
            IRepository<PostFiles> postFiles,
            IRepository<Entities.File> filerepository,
            IRepository<Reaction> reactionRepository,
            IRepository<BlockedPosts> blockedPostRepository,
            IRepository<User> userRepository,
            IPollService pollService,
            AppDbContext context,
            IProfileService profileService)
        {
            _blockedPost = blockedPostRepository;
            _userSubcriptionRepo = userSubcriptionRepo;
            _userPostRepository = userPostRepository;
            _notificationService = notificationService;
            _reactionRepository = reactionRepository;
            _fileRepository = filerepository;
            _context = context;
            _attachmentService = attachmentService;
            _userPostRepository = userPostRepository;
            _postFiles = postFiles;
            _userRepository = userRepository;
            _pollService = pollService;
            _profileService = profileService;
        }

        public async Task<UserPost> GetUserPostByPostId(int PostId)
        {
            try
            {
                var data = await _userPostRepository.Table.Where(x => x.Id == PostId).FirstOrDefaultAsync();
                return data;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> PostMentionList(MentionListReq req, int loginUserid)
        {
            try 
            {
                List<MentionBoxModel> mentionList = new List<MentionBoxModel>();
                foreach (var member in req.userName)
                {
                    var user = await _profileService.GetUserInfoBox(member, loginUserid);
                    MentionBoxModel infoBox = new MentionBoxModel();
                    infoBox.Id = user.Id;
                    infoBox.FullName = user.FirstName + " " + user.LastName;
                    infoBox.About = user.About == null ? "" : user.About;
                    infoBox.UserName = user.UserName;
                    infoBox.ProfileImgUrl = user.ProfileImg == null ? "" : user.ProfileImg;
                    infoBox.isPremium = user.IsPremium;
                    infoBox.isfollowing = user.IsFollowedByLoginUser;
                    infoBox.isPeer = user.IsFollowingLoginUser;
                    mentionList.Add(infoBox);
                }
                return new JsonResponse(200, true, "Success", mentionList);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> GetAllPostsAsync(int Id, int PageNo, int? ProfileUserId, string? Type)
        {
            try
            {
                if(ProfileUserId > 0)
                {
                    SqlParameter userparam = new SqlParameter("@UserId", Id);
                    SqlParameter pageparam = new SqlParameter("@PageNo", PageNo);
                    SqlParameter profileUserIdparam = new SqlParameter("@ProfileUserId", ProfileUserId);
                    var Result = await _context.PostResponses
                        .FromSqlRaw("GetProfileTimeLine @UserId,@PageNo,@ProfileUserId", userparam, pageparam, profileUserIdparam)
                        .ToListAsync();
                    return new JsonResponse(200, true, "Success", Result);
                }
                else
                {
                    SqlParameter userparam = new SqlParameter("@UserId", Id);
                    SqlParameter pageparam = new SqlParameter("@PageNo", PageNo);
                    SqlParameter typeparam = new SqlParameter("@Type", Type);

                    var Result = await _context.PostResponses
                        .FromSqlRaw("GetTimeLine @UserId,@PageNo,@Type", userparam, pageparam, typeparam)
                        .ToListAsync();
                    return new JsonResponse(200, true, "Success", Result);
                }

               
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> GetPostById(int loginUserId,int postId)
        {
            try
            {
                SqlParameter postparam = new SqlParameter("@postId", postId);
                SqlParameter userparam = new SqlParameter("@UserId", loginUserId);
                var Result = await _context.PostResponses
                    .FromSqlRaw("GetPostById @postId,@UserId", postparam, userparam)
                    .ToListAsync();

                return new JsonResponse(200, true, "Success", Result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> RePost(int PostId, int UserId)
        {
            try
            {
                var data = await _userPostRepository.Table
                    .Where(x => x.IsDeleted == false && x.Id == PostId)
                    .FirstOrDefaultAsync();

                var filedata = await _postFiles.Table.Where(x => x.PostId == PostId).ToListAsync();

                UserPost userPost = new UserPost();
                userPost.UserId = UserId;
                userPost.PostMessage = data.PostMessage;
                userPost.Type = "repost";
                userPost.ParentId = PostId;
                await _userPostRepository.InsertAsync(userPost);

                NotificationRes notification = new NotificationRes();
                notification.PostId = PostId;
                notification.ActionByUserId = UserId;
                notification.ActionType = "repost";
                notification.RefId1 = data.UserId.ToString();
                notification.RefId2 = "";
                notification.Message = "";
                await _notificationService.SaveNotification(notification);

                List<PostFiles> postFiles = new List<PostFiles>();

                foreach (var item in filedata)
                {
                    PostFiles pf = new PostFiles();
                    pf.PostId = userPost.Id;
                    pf.FileId = item.FileId;

                    postFiles.Add(pf);
                }
              
                await _postFiles.InsertRangeAsync(postFiles);

                await UpdateCount(PostId, "reshare", 1);
                return new JsonResponse(200, true, "Success", userPost);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> UpdateCount(int PostId, string Type, int dir)
        {
            var data = await _userPostRepository.Table
                .Where(x => x.Id == PostId)
                .FirstOrDefaultAsync();

            var postData = JsonSerializer.Deserialize<Post>(data.PostMessage);

            if (Type == "like")
            {
                if(dir == 0)
                {
                    postData.noOfLikes -= postData.noOfLikes > 0 ? 1 : 0;
                }
                else if(dir == 1)
                {
                    postData.noOfLikes += 1;
                }
            }
            if(Type == "reshare")
            {
                if (dir == 0 && postData.noOfRepost > 0)
                {
                    postData.noOfRepost -= 1;
                }
                else if(dir == 1)
                {
                    postData.noOfRepost += 1;
                }
            }
            if (Type == "comment")
            {
                if (dir == 0)
                {
                    postData.noOfComment -= postData.noOfComment > 0 ? 1 : 0;
                }
                else if (dir == 1)
                {
                    postData.noOfComment += 1;
                }
            }
            data.PostMessage = JsonSerializer.Serialize(postData);

            await _userPostRepository.UpdateAsync(data);

            return new JsonResponse(200, true, "Success", data);
        }

        public async Task<JsonResponse> InsertPost(IFormCollection form, int UserId, string Username)
        {
            try
            {
                var user = _userRepository.GetById(UserId);

                var permiumcheck = _userSubcriptionRepo.Table.Where(x => x.UserId == UserId &&
                                   x.PaymentStatus == "completed" && x.IsDeleted == false).FirstOrDefault();

                var str = form.ToList()[0].Value;
                var postData = JsonSerializer.Deserialize<Post>(str);
                if(postData == null)
                    return new JsonResponse(200, false, "Fail", "Bad Request");

                int pollId = 0;
                if (postData.poll != null )
                {
                    var poll = new Poll();
                    var polldata = JsonSerializer.Deserialize<PollRequest>(postData.poll);
                    poll.PollTitle = postData.textMsg;
                    poll.Choice1 = polldata.choice1;
                    poll.Choice2 = polldata.choice2;
                    poll.Choice3 = polldata.choice3;
                    poll.Choice4 = polldata.choice4;
                    poll.Day = Convert.ToInt32(polldata.day);
                    poll.Hour = Convert.ToInt32(polldata.hour);
                    poll.Minute = Convert.ToInt32(polldata.minute);
                    poll.CreatedBy = Convert.ToInt32(polldata.createdBy);
                    var pollresult = await _pollService.SavePoll(poll);
                    postData.pollId = pollresult.Id;
                    postData.poll = null;
                }
                UserPost userPost = new UserPost();
                userPost.ParentId = postData.parentId;
                userPost.UserId = user.Id;
                userPost.PostMessage = "";
                userPost.Type = postData.type;
                
                await _userPostRepository.InsertAsync(userPost);

                if (permiumcheck != null)
                {
                    postData.isPaid = true;
                }
                else { postData.isPaid = false; }

                postData.id = userPost.Id;
                postData.createdBy = user.FirstName + " " + user.LastName;
                postData.userName = user.UserName;
                postData.profileImg = user.ProfileImg;
                postData.noOfComment = 0;
                postData.noOfLikes = 0;
                postData.noOfRepost = 0;
                postData.noOfViews = 0;
                postData.createdOn = DateTime.UtcNow.ToString();
                UploadPostResponse uploadPostResponse = new UploadPostResponse();
                uploadPostResponse.Post = userPost;

                if (postData.type == "comment")
                {
                    var parentPost = _userPostRepository.GetById(postData.parentId);
                    var postMessage = JsonSerializer.Deserialize<Post>(parentPost.PostMessage);
                    postMessage.noOfComment += 1;
                    var postMessageStr = JsonSerializer.Serialize(postMessage);
                    parentPost.PostMessage = postMessageStr;
                    _userPostRepository.Update(parentPost);
                }
                if (form.Files.Count == 0)
                {
                    userPost.PostMessage = JsonSerializer.Serialize(postData);
                    await _userPostRepository.UpdateAsync(userPost);
                }   

                if (form.Files.Count > 0)
                {
                    List<IFormFile> formFiles = new List<IFormFile>();
                    foreach (var item in form.Files)
                    {
                        formFiles.Add(item);
                    }
                    var uploadedfiles = await _attachmentService.InsertAttachment(formFiles);
                    uploadPostResponse.Files = uploadedfiles;
                    List<PostFiles> postfiles = new List<PostFiles>();

                    
                    postData.imgUrl = new List<string>();
                    postData.thumbnailUrl = new List<string>();
                    postData.videoUrl = new List<string>();
                    bool hasImageFile = false;
                    bool hasVideoFile = false;
                    foreach (var item in uploadedfiles)
                    {
                        PostFiles post = new PostFiles();
                        post.PostId = userPost.Id;
                        post.FileId = item.Id;
                       
                        postfiles.Add(post);
                        if(item.FileExtension.ToLower() == ".jpg" || item.FileExtension.ToLower() == ".jpeg" 
                            || item.FileExtension.ToLower() == ".png" || item.FileExtension.ToLower() == ".gif"
                            || item.FileExtension.ToLower() == ".svg" || item.FileExtension.ToLower() == ".webp"
                            || item.FileExtension.ToLower() == ".bmp" || item.FileExtension.ToLower() == ".tiff")
                        {
                            hasImageFile = true;
                            postData.imgUrl.Add(item.ActualUrl);
                            postData.thumbnailUrl.Add(item.ThumbnailUrl);
                        }
                        if (item.FileExtension.ToLower() == ".mp4" || item.FileExtension.ToLower() == ".avi"
                            || item.FileExtension.ToLower() == ".mov" || item.FileExtension.ToLower() == ".wmv"
                            || item.FileExtension.ToLower() == ".flv" || item.FileExtension.ToLower() == ".mkv"
                            || item.FileExtension.ToLower() == ".webm" || item.FileExtension.ToLower() == ".mpeg"
                            || item.FileExtension.ToLower() == ".mpg" || item.FileExtension.ToLower() == ".3gp")
                        {
                            hasVideoFile = true;
                            postData.videoUrl.Add(item.ActualUrl);
                        }
                    }
                    await _postFiles.InsertRangeAsync(postfiles);
                    userPost.PostMessage = JsonSerializer.Serialize(postData);
                    await _userPostRepository.UpdateAsync(userPost);

                }
                else
                {
                    uploadPostResponse.Files = new List<Entities.File>();
                }



                NotificationRes notification = new NotificationRes();
                notification.PostId = postData.id;
                notification.ActionByUserId = UserId;
                notification.ActionType = postData.type;
                notification.RefId1 = postData.parentId.ToString();
                notification.RefId2 = "";
                notification.Message = "";
                await _notificationService.SaveNotification(notification);
                return new JsonResponse(200, true, "Success", uploadPostResponse);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<JsonResponse> DeletePostAsync(int PostId)
        {
            try
            {
                var userpost = await _userPostRepository.GetByIdAsync(PostId);

                var postfile = await _postFiles.Table
                    .Where(x => x.IsDeleted == false && x.PostId == PostId)
                    .ToListAsync();

                var reactions = await _reactionRepository.Table
                    .Where(x => x.IsDeleted ==false && x.PostId == PostId)
                    .ToListAsync();  

                if (userpost != null)
                {
                    await _userPostRepository.DeleteAsync(userpost);
                    _postFiles.DeleteHardRange(postfile);
                    _reactionRepository.DeleteHardRange(reactions);
                    // await _postFiles.DeleteRangeAsync(postfile);
                    //await _reactionRepository.DeleteRangeAsync(reactions);
                    return new JsonResponse(200, true, "Success",userpost);
                }

                return new JsonResponse(200, true, "Fail", null);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> BlockUnBlockPosts(int PostId, int UserId)
        {
            var data = await _blockedPost.Table
                .Where(post => post.UserId == UserId && post.PostId == PostId)
                .FirstOrDefaultAsync();

            if(data == null)
            {
                BlockedPosts blockedPosts = new BlockedPosts();
                blockedPosts.UserId = UserId;
                blockedPosts.PostId = PostId;
                await _blockedPost.InsertAsync(blockedPosts);
                return new JsonResponse(200,true,"Success",blockedPosts);
            }
            else
            {
                _blockedPost.DeleteHard(data);
                return new JsonResponse(200,true,"Success",data);
            }
        }

        public void UpdatePost()
        {
            var data = _userPostRepository.Table.ToList();
            List<UserPost> posts = new List<UserPost>();
            foreach (var postItem in data)
            {
                var user = _userRepository.GetById(postItem.UserId);
                var permiumcheck =  _userSubcriptionRepo.Table.Where(x => x.UserId == postItem.UserId &&
                                    x.PaymentStatus == "completed" && x.IsDeleted == false).FirstOrDefault();
                var postMessage = JsonSerializer.Deserialize<Post>(postItem.PostMessage);
                postMessage.createdBy = user.FirstName + " " + user.LastName;
                postMessage.userName = user.UserName;
                postMessage.profileImg = user.ProfileImg;
                postMessage.type = postItem.Type;
                if (permiumcheck != null)
                {
                    postMessage.isPaid = true;
                }
                else { postMessage.isPaid = false; }
                postItem.PostMessage = JsonSerializer.Serialize(postMessage);
                posts.Add(postItem);
            }
            _userPostRepository.UpdateRange(posts);
        }

        public async Task<UserPost> ChangeWhoCanReply(int postId,int whoCanReply)
        {
            var data = await _userPostRepository.Table
                .Where(x => x.Id == postId)
                .FirstOrDefaultAsync();

            var postData = JsonSerializer.Deserialize<Post>(data.PostMessage);
            postData.whoCanReply = whoCanReply;
            var postMessageStr = JsonSerializer.Serialize(postData);
            data.PostMessage = postMessageStr;
            await _userPostRepository.UpdateAsync(data);
            return data;
        }

        public async Task<UserPost> PinUnpinPost(int postId)
        {
            var data = await _userPostRepository.Table
                .Where(x => x.Id == postId)
                .FirstOrDefaultAsync();

            var postData = JsonSerializer.Deserialize<Post>(data.PostMessage);
            postData.isPinPost = postData.isPinPost == 1 ? 0 : 1;
            var postMessageStr = JsonSerializer.Serialize(postData);
            data.PostMessage = postMessageStr;
            await _userPostRepository.UpdateAsync(data);
            return data;
        }
    }
}
