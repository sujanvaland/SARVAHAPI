using AutoMapper;
using Microsoft.Extensions.Configuration;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Common;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;
using System.Text.Json;
using SpiritualNetwork.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RestSharp;

namespace SpiritualNetwork.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IGlobalSettingService _globalSettingService;
        private readonly IRepository<EmailTemplate> _emailTemplateRepository;
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IRepository<Reaction> _reactionRepository;
        private readonly IRepository<UserFollowers> _userFollowers;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<OnlineUsers> _onlineUserRepository;
        private readonly IRepository<UserPost> _userPostRepository;
        private readonly IRepository<UserNotification> _userNotificationRepository;
        private readonly IHubContext<NotificationHub, INotificationHub> _notificationHub;
        private readonly IRestClient _client;


        private readonly IMapper _mapper;

        public NotificationService(IRepository<EmailTemplate> emailTemplateRepository,
        IGlobalSettingService globalSettingService, IRepository<Notification> notificationRepository, IMapper mapper, IRepository<Reaction> reactionRepository, IRepository<UserFollowers> userFollowers, IRepository<User> userRepository, IRepository<OnlineUsers> onlineUserRepository,
        IHubContext<NotificationHub, INotificationHub> notificationHub,
        IRepository<UserPost> userPostRepository, IRepository<UserNotification> userNotificationRepository, IRestClient client)
        {
            _emailTemplateRepository = emailTemplateRepository;
            _globalSettingService = globalSettingService;
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _reactionRepository = reactionRepository;
            _userFollowers = userFollowers;
            _userRepository = userRepository;
            _onlineUserRepository = onlineUserRepository;
            _notificationHub = notificationHub;
            _userPostRepository = userPostRepository;
            _userNotificationRepository = userNotificationRepository;
            _client = client;
        }

        public async Task SendEmailNotification(string emailType,User user)
        {
            var emailTemplate = _emailTemplateRepository.Table.Where(x => x.EmailType == emailType).FirstOrDefault();
            EmailRequest emailRequest = new EmailRequest();
            emailRequest.SITETITLE = await _globalSettingService.GetValue("SITENAME");
            emailRequest.USERNAME = user.UserName;
            emailRequest.CONTENT1 = emailTemplate.Content1;
            emailRequest.CONTENT2 = emailTemplate.Content2;
            emailRequest.CTALINK = emailTemplate.CTALink;
            emailRequest.CTATEXT = emailTemplate.CTAText;
            emailRequest.ToEmail = user.Email;
            emailRequest.Subject = emailTemplate.Subject + await _globalSettingService.GetValue("SITENAME");
            emailRequest.SUPPORTEMAIL = await _globalSettingService.GetValue("SupportEmail");

            SMTPDetails smtpDetails = new SMTPDetails();
            smtpDetails.Username = await _globalSettingService.GetValue("SMTPUsername");
            smtpDetails.Host = await _globalSettingService.GetValue("SMTPHost");
            smtpDetails.Password = await _globalSettingService.GetValue("SMTPPassword");
            smtpDetails.Port = await _globalSettingService.GetValue("SMTPPort");
            smtpDetails.SSLEnable = await _globalSettingService.GetValue("SMTPSSLEnable");
            EmailHelper.SendEmailRequest(emailRequest, smtpDetails);
        }

        public async Task<JsonResponse> SaveNotification(NotificationRes Res)
        {
            Notification notification = _mapper.Map<Notification>(Res);
             _notificationRepository.Insert(notification);

            if(notification.ActionType == "repost" || notification.ActionType == "like" 
                || notification.ActionType == "follow")
            {
                UserNotification userNotification = new UserNotification();
                userNotification.NotificationId = notification.Id;
                userNotification.UserId = int.Parse(notification.RefId1);
                await _userNotificationRepository.InsertAsync(userNotification);
            }
            if(notification.ActionType == "comment")
            {

                var parentPost = _userPostRepository.GetById(int.Parse(notification.RefId1));
                UserNotification userNotification = new UserNotification();
                userNotification.NotificationId = notification.Id;
                userNotification.UserId = parentPost.UserId;
                await _userNotificationRepository.InsertAsync(userNotification);
            }

            if (notification.ActionType == "post")
            {
                var followerConnection = (from uf in _userFollowers.Table
                                          join u in _userRepository.Table on uf.UserId equals u.Id
                                          where uf.FollowToUserId == notification.ActionByUserId
                                          select u.Id).ToList();
                foreach (var userId in followerConnection)
                {
                    UserNotification userNotification = new UserNotification();
                    userNotification.NotificationId = notification.Id;
                    userNotification.UserId = userId;
                    await _userNotificationRepository.InsertAsync(userNotification);
                }

                var followToconnection = (from uf in _userFollowers.Table
                                  join u in _userRepository.Table on uf.UserId equals u.Id
                                  join ou in _onlineUserRepository.Table on uf.UserId equals ou.UserId
                                  where uf.FollowToUserId == notification.ActionByUserId
                                  select ou.ConnectionId).ToList();

                var userconnenction = _onlineUserRepository.Table.Where(x => x.UserId == notification.ActionByUserId).FirstOrDefault();
                if(userconnenction != null)
                {
                    followToconnection.Add(userconnenction.ConnectionId);
                }

                var postobj = _userPostRepository.Table.Where(x => x.Id == notification.PostId).FirstOrDefault();
                Res.PostId = postobj.Id;
                Res.connectionIds = followToconnection;
                Res.Message = JsonSerializer.Serialize(postobj);
                string strmessage = JsonSerializer.Serialize(Res);
                await SendNotification(Res,strmessage);

                if (postobj != null)
                {
                    Post PostMessage = JsonSerializer.Deserialize<Post>(postobj.PostMessage);
                    var MentionList = PostMessage.mentions.Select(x => x.userId).ToList();
                    var tagList = PostMessage.tagUser.Select(x => x.id).ToList();
                    Res.connectionIds = null;
                    Res.Message = "";
                    if (MentionList != null )
                    {
                        Notification MentionNotitficaion = _mapper.Map<Notification>(Res);
                        MentionNotitficaion.ActionType = "mention";
                        await _notificationRepository.InsertAsync(MentionNotitficaion);
                        foreach (var id in MentionList)
                        {
                            UserNotification userNotification = new UserNotification();
                            userNotification.NotificationId = MentionNotitficaion.Id;
                            userNotification.UserId = id;
                            await _userNotificationRepository.InsertAsync(userNotification);
                        }
                    }
                    if (tagList != null)
                    {   
                        Notification MentionNotitficaion = _mapper.Map<Notification>(Res);
                        MentionNotitficaion.Id = 0;
                        MentionNotitficaion.ActionType = "tag";
                        await _notificationRepository.InsertAsync(MentionNotitficaion);
                        foreach (var id in tagList)
                        {
                            UserNotification userNotification = new UserNotification();
                            userNotification.NotificationId = MentionNotitficaion.Id;
                            userNotification.UserId = id;
                            await _userNotificationRepository.InsertAsync(userNotification);
                        }
                    }
                }
            }

            if (notification.ActionType == "newchatmessage" || notification.ActionType == "newgroupmessage")
            {
                string strmessage = JsonSerializer.Serialize(Res);
                await SendNotification(Res, strmessage);
            }
            return new JsonResponse(200, true, "Saved Success", null);
        }

        public async Task SendNotification(NotificationRes request,string strmessage)
        {
            //if(request.ActionType == "post")
            //{
            //    foreach (var connectionId in request.connectionIds)
            //    {
            //        await _notificationHub.Clients.Client(connectionId).OnNewPost(strmessage);
            //    }
            //}
            //else if (request.ActionType == "newchatmessage" || request.ActionType == "newgroupmessage")
            //{
            //    foreach (var connectionId in request.connectionIds)
            //    {
            //        await _notificationHub.Clients.Client(connectionId).SendChatMessage(strmessage);
            //    }
            //    if (request.connectionIds.Count() == 0)
            //    {
            //        await _notificationHub.Clients.All.SendChatMessage(strmessage);
            //    }
            //}

            string apiUrl = "https://localhost:4200";
            var _client = new RestClient(apiUrl);
           
            // Example: Make a POST request with JSON payload
            var postRequest = new RestRequest("/notify", Method.Post);
            postRequest.AddJsonBody(request);
            var responseData = _client.Execute(postRequest);
        }

        public async Task<JsonResponse> UserNotification(int userId,int PageNo, int Size)
        {
            var allNotification = from UN in _userNotificationRepository.Table
                                  join N in _notificationRepository.Table on UN.NotificationId equals N.Id into noti
                                  from N in noti.DefaultIfEmpty()
                                  join UD in _userRepository.Table on N.ActionByUserId equals UD.Id into user
                                  from UD in user.DefaultIfEmpty()
                                  join UP in _userPostRepository.Table on N.PostId equals UP.Id into userPost
                                  from UP in userPost.DefaultIfEmpty()
                                  join RUP in _userPostRepository.Table on UP.ParentId equals RUP.Id into reUserPost
                                  from RUP in reUserPost.DefaultIfEmpty()
                                  join PUD in _userRepository.Table on RUP.UserId equals PUD.Id into userPostDetails
                                  from PUD in userPostDetails.DefaultIfEmpty()
                                  where UN.UserId == userId && UN.IsDeleted == false && N.ActionByUserId != userId
                                  select new userNotificationRes
                                  {
                                      UserDetail = _mapper.Map<UserDetails>(UD),
                                      Type = N.ActionType,
                                      PostId = N.PostId,
                                      ParentPostId = RUP.Id,
                                      RepostUserDetail = _mapper.Map<UserDetails?>(PUD),
                                  };

            var chatHistory = await allNotification.Take(Size).Skip((PageNo - 1) * Size).ToListAsync();
            return new JsonResponse(200, true, " Success", chatHistory);

        }
    }
}
