using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities.CommonModel;
using SpiritualNetwork.Entities;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.Common;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using Azure;

namespace SpiritualNetwork.API.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IRepository<PasswordResetRequest> _passwordResetRequestRepository;
        private readonly INotificationService _notificationService;
        private readonly IGlobalSettingService _globalSettingService;
        private readonly IRepository<PreRegisteredUser> _preRegisteredUserRepository;
        private readonly IRepository<UserFollowers> _userFollowersRepository;
        private readonly IRepository<UserMuteBlockList> _userMuteBlockListRepository;
        private readonly IRepository<OnlineUsers> _onlineUsers;
        private readonly IProfileService _profileService;

        public UserService(
            IRepository<OnlineUsers> onlineUsers,
            IRepository<PreRegisteredUser> preregistereduserrepository,
            IRepository<User> userRepository,
            IMapper mapper,
            IConfiguration configuration,
            IRepository<PasswordResetRequest> passwordResetRequestRepository,
            INotificationService notificationService, IGlobalSettingService globalSettingService,
            IRepository<UserFollowers> userFollowersRepository,
            IRepository<UserMuteBlockList> userMuteBlockListRepository,
            IProfileService profileService
            )
        {
            _onlineUsers = onlineUsers;
            _preRegisteredUserRepository = preregistereduserrepository;
            _passwordResetRequestRepository = passwordResetRequestRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
            _notificationService = notificationService;
            _globalSettingService = globalSettingService;
            _userFollowersRepository = userFollowersRepository;
            _userMuteBlockListRepository = userMuteBlockListRepository;
            _profileService = profileService;
        }

        public async Task<JsonResponse> OnlineOfflineUsers(int UserId, string? ConnectionId)
        {
            try
            {

               var data = await _onlineUsers.Table
                    .Where(x => x.IsDeleted == false && x.UserId == UserId)
                    .FirstOrDefaultAsync();

                if(!ConnectionId.IsNullOrEmpty() && data != null)
                {
                    data.ConnectionId = ConnectionId;
                    await _onlineUsers.UpdateAsync(data);
                    return new JsonResponse(200, true, "Success", data);
                }

                OnlineUsers onlineUsers = new OnlineUsers();
                onlineUsers.UserId = UserId;
                onlineUsers.ConnectionId = ConnectionId;

                await _onlineUsers.InsertAsync(onlineUsers);

                return new JsonResponse(200, true, "Success", onlineUsers);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<User> Authenticate(string username, string password)
        {
            try
            {
                var data = await _userRepository.Table
                    .Where(x => x.UserName.ToLower() == username.ToLower()
                    || x.Email.ToLower() == username.ToLower()).FirstOrDefaultAsync();

                if (data != null)
                {
                    if (PasswordHelper.VerifyPassword(password, data.Password))
                    {
                        return data;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<bool> IsUserExist(string username)
        {
            var data = await _userRepository.Table.Where(x => x.UserName == username || x.Email.ToLower() == username.ToLower()).FirstOrDefaultAsync();
            return (data != null) ? true : false;
        }

        public async Task<JsonResponse> SignIn(string username, string password)
        {
            if (!await IsUserExist(username))
            {
                return new JsonResponse(204, true, "Not Exist", null);
            }
            else
            {
                User user = await Authenticate(username, password);
                if (user != null)
                {
                    var profileModal = _profileService.GetUserProfile(user);
                    var authClaims = new List<Claim>
                    {
                        new Claim("Username", username),
                        new Claim("Id", user.Id.ToString())
                    };
                    var authSigninKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddDays(1),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256Signature)
                        );

                    LoginResponse loginResponse = new LoginResponse()
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        Profile = profileModal
                    };
                    
                    return new JsonResponse(200, true, "Success", loginResponse);
                }
                else
                {
                    LoginResponse loginResponse = new LoginResponse()
                    {
                        Token = "",
                        Profile = null
                    };

                    return new JsonResponse(200, true, "UnAuthenticated", loginResponse);
                }
            }
        }

        string GenerateRandomPassword(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder password = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                password.Append(chars[random.Next(chars.Length)]);
            }

            return password.ToString();
        }

        public async Task<JsonResponse> ForgotPasswordRequest(string email)
        {
            var user = _userRepository.Table.Where(x => x.Email == email).FirstOrDefault();
            if (user == null)
            {
                return new JsonResponse(200, false, "Bad Request", null);
            }

            PasswordResetRequest passwordResetRequest = new PasswordResetRequest();
            passwordResetRequest.UserId = user.Id;
            passwordResetRequest.OTP = StringHelper.GenerateRandomNumber;
            passwordResetRequest.ActivationDate = DateTime.Now;
            passwordResetRequest.ExpirtionDate = DateTime.Now.AddMinutes(15);
            passwordResetRequest.IsUsed = false;
            await _passwordResetRequestRepository.InsertAsync(passwordResetRequest);

            // string encryptedotp = CommonHelper.EncryptString(passwordResetRequest.OTP.ToString());
            // string encrypteduserid = CommonHelper.EncryptString(passwordResetRequest.UserId.ToString());

            var byteotp = System.Text.Encoding.UTF8.GetBytes(Convert.ToString(passwordResetRequest.OTP));
            string encryptedotp = Convert.ToBase64String(byteotp);
            var byteuserid = System.Text.Encoding.UTF8.GetBytes(Convert.ToString(passwordResetRequest.UserId));
            string encrypteduserid = Convert.ToBase64String(byteuserid);

            EmailRequest emailRequest = new EmailRequest();
            emailRequest.USERNAME = user.UserName;
            emailRequest.CONTENT1 = "Oops, it happens to the best of us! If you've forgotten your password, don't worry. We're here to help you regain access to your " + await _globalSettingService.GetValue("SiteName") + " account.";
            emailRequest.CONTENT2 = "If you have any questions, we're here to help. Just reach out.";
            emailRequest.CTALINK = await _globalSettingService.GetValue("SiteUrl") + "/forgotPassword/" + encryptedotp + "/" + encrypteduserid;
            emailRequest.CTATEXT = "Click here to reset your password";
            emailRequest.ToEmail = user.Email;
            emailRequest.Subject = "Password Reset Request : " + await _globalSettingService.GetValue("SiteName");

            SMTPDetails smtpDetails = new SMTPDetails();
            smtpDetails.Username = await _globalSettingService.GetValue("SMTPUsername");
            smtpDetails.Host = await _globalSettingService.GetValue("SMTPHost");
            smtpDetails.Password = await _globalSettingService.GetValue("SMTPPassword");
            smtpDetails.Port = await _globalSettingService.GetValue("SMTPPort");
            smtpDetails.SSLEnable = await _globalSettingService.GetValue("SMTPSSLEnable");
            var body = EmailHelper.SendEmailRequest(emailRequest, smtpDetails);
            return new JsonResponse(200, true, "Success", null);
        }

        public async Task<JsonResponse> ValidateOTP(string encryptedotp, string encrypteduserid)
        {
            if (!string.IsNullOrEmpty(encryptedotp) && !string.IsNullOrEmpty(encrypteduserid))
            {
                var resetpasswordreq = await _passwordResetRequestRepository.Table
                    .Where(x => x.UserId.ToString() == encrypteduserid && x.OTP == encryptedotp && x.IsUsed == false).FirstOrDefaultAsync();

                resetpasswordreq.IsUsed = true;

                await _passwordResetRequestRepository.UpdateAsync(resetpasswordreq);

                if (resetpasswordreq != null)
                {

                    if (DateTime.Now < resetpasswordreq.ActivationDate || DateTime.Now > resetpasswordreq.ExpirtionDate)
                    {
                        return new JsonResponse(200, false, "Link Expired", null);
                    }
                    else
                    {
                        var newpassword = GenerateRandomPassword(10);

                        var user = _userRepository.GetById(Convert.ToInt32(encrypteduserid));

                        user.Password = PasswordHelper.EncryptPassword(newpassword.ToString());

                        await _userRepository.UpdateAsync(user);

                        EmailRequest emailRequest = new EmailRequest();
                        emailRequest.USERNAME = user.UserName;
                        emailRequest.CONTENT1 = "Your new password, please use below pasword to login to " + await _globalSettingService.GetValue("SiteName") + " account.";
                        emailRequest.CONTENT2 = "New Password: " + newpassword;
                        emailRequest.CTALINK = await _globalSettingService.GetValue("SiteUrl") + "/Login";
                        emailRequest.CTATEXT = "Click here to login";
                        emailRequest.ToEmail = user.Email;
                        emailRequest.Subject = "Welcome to " + await _globalSettingService.GetValue("SiteName");

                        SMTPDetails smtpDetails = new SMTPDetails();
                        smtpDetails.Username = await _globalSettingService.GetValue("SMTPUsername");
                        smtpDetails.Host = await _globalSettingService.GetValue("SMTPHost");
                        smtpDetails.Password = await _globalSettingService.GetValue("SMTPPassword");
                        smtpDetails.Port = await _globalSettingService.GetValue("SMTPPort");
                        smtpDetails.SSLEnable = await _globalSettingService.GetValue("SMTPSSLEnable");
                        var body = EmailHelper.SendEmailRequest(emailRequest, smtpDetails);

                        return new JsonResponse(200, true, "Success", null);
                    }


                }
            }
            return new JsonResponse(200, true, "Something went wrong", null);
        }

        public async Task<JsonResponse> SignUp(SignupRequest signupRequest)
        {
            try
            {
                signupRequest.UserName = signupRequest.UserName.TrimEnd().TrimStart().ToLower();
                if (signupRequest.UserName.Contains(" "))
                {
                    return new JsonResponse(200, false, "Space not allowed in username", null);
                }

                var query = _userRepository.Table;
                if (!String.IsNullOrEmpty(signupRequest.PhoneNumber))
                {
                    query = query.Where(x=>x.PhoneNumber == signupRequest.PhoneNumber);
                }

                var data = await query.Where(x => x.IsDeleted == false &&
                (x.UserName.ToLower().Trim() == signupRequest.UserName.ToLower().Trim()
                || x.Email.ToLower().Trim() == signupRequest.Email.ToLower().Trim()
                )).FirstOrDefaultAsync();
                
                if (data != null)
                {
                    return new JsonResponse(200, false, "Username or Email already exists", null);
                }

                if (data == null)
                {
                    User user = _mapper.Map<User>(signupRequest);
                    user.InviterId = 0;
                    user.Password = PasswordHelper.EncryptPassword(signupRequest.Password);

                    user.PaymentStatus = "";
                    user.PaymentRef1 = "";
                    user.PaymentRef2 = "";
                    user.Status = "";
                    await _userRepository.InsertAsync(user);

                    PasswordResetRequest passwordResetRequest = new PasswordResetRequest();
                    passwordResetRequest.UserId = user.Id;
                    passwordResetRequest.OTP = StringHelper.GenerateRandomNumber;
                    passwordResetRequest.ActivationDate = DateTime.Now;
                    passwordResetRequest.ExpirtionDate = DateTime.Now.AddMinutes(15);
                    passwordResetRequest.IsUsed = false;
                    await _passwordResetRequestRepository.InsertAsync(passwordResetRequest);

                    // string encryptedotp = CommonHelper.EncryptString(passwordResetRequest.OTP.ToString());
                    // string encrypteduserid = CommonHelper.EncryptString(passwordResetRequest.UserId.ToString());
                    try 
                    { 
                        var byteotp = System.Text.Encoding.UTF8.GetBytes(Convert.ToString(passwordResetRequest.OTP));
                        string encryptedotp = Convert.ToBase64String(byteotp);
                        var byteuserid = System.Text.Encoding.UTF8.GetBytes(Convert.ToString(passwordResetRequest.UserId));
                        string encrypteduserid = Convert.ToBase64String(byteuserid);

                        EmailRequest emailRequest = new EmailRequest();
                        emailRequest.USERNAME = signupRequest.UserName;
                        emailRequest.CONTENT1 = "Welcome aboard! We're delighted to have you as a part of our " + await _globalSettingService.GetValue("SiteName") + " family. Get ready for an exciting journey with us!";
                        emailRequest.CONTENT2 = "If you have any questions, we're here to help. Just reach out.";
                        emailRequest.CTALINK = await _globalSettingService.GetValue("SiteUrl") + "/Verifyemail/" + encryptedotp + "/" + encrypteduserid;
                        emailRequest.CTATEXT = "Verify Email";
                        emailRequest.ToEmail = signupRequest.Email;
                        emailRequest.Subject = "Welcome to " + await _globalSettingService.GetValue("SiteName");

                        SMTPDetails smtpDetails = new SMTPDetails();
                        smtpDetails.Username = await _globalSettingService.GetValue("SMTPUsername");
                        smtpDetails.Host = await _globalSettingService.GetValue("SMTPHost");
                        smtpDetails.Password = await _globalSettingService.GetValue("SMTPPassword");
                        smtpDetails.Port = await _globalSettingService.GetValue("SMTPPort");
                        smtpDetails.SSLEnable = await _globalSettingService.GetValue("SMTPSSLEnable");

                        var body = EmailHelper.SendEmailRequest(emailRequest, smtpDetails);

                        /* var Inviter_User = await _userRepository.Table.Where(x => x.UserName == signupRequest.InviterName)
                                 .FirstOrDefaultAsync();

                         var totalCount = await _userRepository.Table
                                         .Where(x => x.InviterId == Inviter_User.Id && x.IsDeleted == false)
                                         .CountAsync();

                         await _notificationService.SendEmailNotification("newreferral", Inviter_User);*/
                    }
                    catch (Exception ex)
                    {
                        return new JsonResponse(200, true, "Success", user);
                    }

                    return new JsonResponse(200, true, "Success", user);
                    
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PreRegisteredUser> PreSignUp(PreSignupRequest req)
        {
            try
            {
                var res = await _preRegisteredUserRepository.Table
                    .Where(x => x.UserName == req.UserName || 
                    x.Email.ToLower().Trim() == req.Email.ToLower().Trim())
                    .FirstOrDefaultAsync();

                if (res == null)
                {
                    PreRegisteredUser preRegisteredUser = new PreRegisteredUser();
                    preRegisteredUser.UserName = req.UserName;
                    preRegisteredUser.FirstName = req.FirstName;
                    preRegisteredUser.LastName = req.LastName;
                    preRegisteredUser.Email = req.Email;
                    preRegisteredUser.IpAddress = req.IpAddress;

                    await _preRegisteredUserRepository.InsertAsync(preRegisteredUser);

                    return preRegisteredUser;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    
        public async Task<JsonResponse> CheckUsername(string username)
        {
            try
            {
                var data = await _userRepository.Table
                    .Where(x => x.UserName.Trim() == username)
                    .FirstOrDefaultAsync();

                if (data == null)
                {
                    return new JsonResponse(200, true, "Success", false);
                }
                else
                {
                    return new JsonResponse(200, true, "Success", true);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> VerifyEmail(string encryptedotp, string encrypteduserid)
        {
            if (!string.IsNullOrEmpty(encryptedotp) && !string.IsNullOrEmpty(encrypteduserid))
            {
                var resetpasswordreq = await _passwordResetRequestRepository.Table
                    .Where(x => x.UserId.ToString() == encrypteduserid && x.OTP == encryptedotp && x.IsUsed == false).FirstOrDefaultAsync();

                resetpasswordreq.IsUsed = true;

                await _passwordResetRequestRepository.UpdateAsync(resetpasswordreq);

                if (resetpasswordreq != null)
                {

                    if (DateTime.Now < resetpasswordreq.ActivationDate || DateTime.Now > resetpasswordreq.ExpirtionDate)
                    {
                        return new JsonResponse(200, false, "Link Expired", null);
                    }
                    else
                    {
                        var user = _userRepository.GetById(Convert.ToInt32(encrypteduserid));
                        user.IsEmailVerified = 1;
                        await _userRepository.UpdateAsync(user);

                        EmailRequest emailRequest = new EmailRequest();
                        emailRequest.USERNAME = user.UserName;
                        emailRequest.CONTENT1 = "Thankyou for verifying your Email with " + await _globalSettingService.GetValue("SiteName") + " account.";
                        emailRequest.CONTENT2 = "We wish you good luck in your spirtual journey";
                        emailRequest.CTALINK = await _globalSettingService.GetValue("SiteUrl") + "/Login";
                        emailRequest.CTATEXT = "Click here to login";
                        emailRequest.ToEmail = user.Email;
                        emailRequest.Subject = "Email Verified " + await _globalSettingService.GetValue("SiteName");

                        SMTPDetails smtpDetails = new SMTPDetails();
                        smtpDetails.Username = await _globalSettingService.GetValue("SMTPUsername");
                        smtpDetails.Host = await _globalSettingService.GetValue("SMTPHost");
                        smtpDetails.Password = await _globalSettingService.GetValue("SMTPPassword");
                        smtpDetails.Port = await _globalSettingService.GetValue("SMTPPort");
                        smtpDetails.SSLEnable = await _globalSettingService.GetValue("SMTPSSLEnable");
                        var body = EmailHelper.SendEmailRequest(emailRequest, smtpDetails);

                        return new JsonResponse(200, true, "Success", true);
                    }


                }
            }
            return new JsonResponse(200, true, "Something went wrong", null);
        }
    
        public void FollowUnFollowUser(int userId,int loginUserId)
        {
            var exists = _userFollowersRepository.Table.Where(x => x.UserId == loginUserId && x.FollowToUserId == userId).FirstOrDefault();
            if (exists == null)
            {
                UserFollowers follower = new UserFollowers();
                follower.UserId = loginUserId;
                follower.FollowToUserId = userId;
                _userFollowersRepository.Insert(follower);

                NotificationRes notification = new NotificationRes();
                notification.PostId = 0;
                notification.ActionByUserId = loginUserId;
                notification.ActionType = "follow";
                notification.RefId1 = userId.ToString();
                notification.RefId2 = "";
                notification.Message = "";
                _notificationService.SaveNotification(notification);
            }
            else
            {
                _userFollowersRepository.DeleteHard(exists);
            }
        }

        public void BlockMuteUser(int userId, int loginUserId,string type)
        {
            var query = _userMuteBlockListRepository.Table.Where(x => x.UserId == loginUserId);

            if (type == "mute")
            {
                var exist = _userMuteBlockListRepository.Table
                    .Where(x=> x.UserId == loginUserId && x.MuteedUserId == userId).FirstOrDefault();
                if(exist == null)
                {
                    UserMuteBlockList mutedUser = new UserMuteBlockList();
                    mutedUser.UserId = loginUserId;
                    mutedUser.MuteedUserId = userId;
                    _userMuteBlockListRepository.Insert(mutedUser);
                }
                else
                {
                    _userMuteBlockListRepository.DeleteHard(exist);
                }
            }
            else
            {
                var exist = _userMuteBlockListRepository.Table.Where(x => x.UserId == loginUserId && x.BlockedUserId == userId).FirstOrDefault();
                if (exist == null)
                {
                    UserMuteBlockList blockedUser = new UserMuteBlockList();
                    blockedUser.UserId = loginUserId;
                    blockedUser.BlockedUserId = userId;
                    _userMuteBlockListRepository.Insert(blockedUser);
                }
                else
                {
                    _userMuteBlockListRepository.DeleteHard(exist);
                }
            }
        }
        
        public async Task<User> GetUserByName(string Username)
        {
            var data = await _userRepository.Table.Where(x => x.IsDeleted ==  false 
            && (x.UserName == Username || x.Email == Username))
                .FirstOrDefaultAsync();

            return data;
        }

        public async Task<JsonResponse> GetOnlineUsers()
        {
            try
            {
                var data = await (from onlineuser in _onlineUsers.Table.Where(x => x.IsDeleted == false)
                             join
                             user in _userRepository.Table.Where(x => x.IsDeleted == false)
                             on onlineuser.UserId equals user.Id
                             select new
                             {
                                 Id = onlineuser.Id,
                                 UserId = onlineuser.UserId,
                                 ConnectionId = onlineuser.ConnectionId,
                                 Username = user.UserName,
                                 FirstName = user.FirstName,
                                 LastName = user.LastName
                             }).ToListAsync();

                return new JsonResponse(200,true,"Success",data);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> ChangePassword(ChangePasswordReq req, int UserId)
        {
            try
            {
                var user = await _userRepository.Table.Where(x => x.Id == UserId &&
                x.IsDeleted == false).FirstOrDefaultAsync();
                 
                if (PasswordHelper.VerifyPassword(req.CurrentPassword, user.Password)) {
                    user.Password = PasswordHelper.EncryptPassword(req.NewPassword);
                    await _userRepository.UpdateAsync(user);
                    return new JsonResponse(200, true, "Password Changed Successfully!", null);
                }
                else
                {
                    return new JsonResponse(200, false, "Incorrect Current Password!", null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> DeactivateUserAccount(int timeperiod, int UserId)
        {
            try
            {
                var user = await _userRepository.Table.Where(x => x.Id == UserId &&
                x.IsDeleted == false).FirstOrDefaultAsync();

                user.IsDeleted = true;
                if(timeperiod == 1)
                {
                    user.ReActivationDate = DateTime.Now.AddDays(30);
                }
                else
                {
                    user.ReActivationDate = DateTime.Now.AddMonths(12);
                }
                
                await _userRepository.UpdateAsync(user);
                return new JsonResponse(200, true, "Account Deactivated Successfully!", null);
            }
            catch(Exception ex)
            {
                return new JsonResponse(200, false, "Something went wrong", null);
            }
        }

        public async Task<JsonResponse> BlockedUsersList(int UserId)
        {
            try
            {
                var user = await (from b in _userMuteBlockListRepository.Table
                                  join u in _userRepository.Table on b.BlockedUserId equals u.Id
                                  where b.UserId == UserId
                                  select new
                                  {
                                      u.Id,
                                      u.FirstName,
                                      u.LastName,
                                      u.ProfileImg,
                                      u.UserName
                                  }).ToListAsync();

                return new JsonResponse(200, true, "Success", user);

            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Something went wrong", null);
            }
        }

    }
}
