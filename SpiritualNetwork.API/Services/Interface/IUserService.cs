using SpiritualNetwork.API.Model;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface IUserService
    {
        public Task<JsonResponse> SignUp(SignupRequest signupRequest);
        public Task<JsonResponse> SignIn(string username, string password);
        public Task<JsonResponse> ForgotPasswordRequest(string email);
        public Task<JsonResponse> ValidateOTP(string encryptedotp, string encrypteduserid);
        public Task<PreRegisteredUser> PreSignUp(PreSignupRequest req);
        public Task<JsonResponse> CheckUsername(string username);
        Task<JsonResponse> VerifyEmail(string encryptedotp, string encrypteduserid);
        void BlockMuteUser(int userId, int loginUserId, string type);
        public Task<User> GetUserByName(string Username);
        public Task<JsonResponse> OnlineOfflineUsers(int UserId, string? ConnectionId);
        public Task<JsonResponse> GetOnlineUsers();
        public Task<JsonResponse> ChangePassword(ChangePasswordReq req,int userId);
        public Task<JsonResponse> DeactivateUserAccount(int timeperiod, int userId);
        public Task<JsonResponse> BlockedUsersList(int userId);

    }
}
