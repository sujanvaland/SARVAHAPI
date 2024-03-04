using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities.CommonModel;
using System.Net;
using System.Net.Http;

namespace SpiritualNetwork.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ApiBaseController
    {
        private readonly ILogger<UserController> logger;
        private IUserService _userService;

        public UserController(ILogger<UserController> logger, 
            IUserService userService)
        {
            this.logger = logger;
            this._userService = userService;
        }

        [AllowAnonymous]
        [HttpPost(Name = "CheckUsername")]
        public async Task<JsonResponse> CheckUsername(CheckUsernameRequest req)
        {
            try
            {
                var response = await _userService.CheckUsername(req.Username);

                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost(Name = "PreSignUp")]
        public async Task<JsonResponse> PreSignUp(PreSignupRequest presignupRequest)
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                presignupRequest.IpAddress = ip;
                var response = await _userService.PreSignUp(presignupRequest);
                
                return new JsonResponse(200, true, "Success", response);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost(Name = "SignUp")]
        public async Task<JsonResponse> SignUp(SignupRequest signupRequest)
        {
            try
            {
                var response = await _userService.SignUp(signupRequest);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost(Name = "SignIn")]
        public async Task<JsonResponse> SignIn(LoginRequest loginRequest)
        {
            try
            {
                return await _userService.SignIn(loginRequest.Username, loginRequest.Password);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost(Name = "GetUserByName")]
        public async Task<JsonResponse> GetUserByName(GetUserByNameReq getUserByNameReq)
        {
            try
            {
                var user = await _userService.GetUserByName(getUserByNameReq.UserName);
                return new JsonResponse(200, true, "Success", user);
            }
            catch(Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet(Name = "ForgotPassword")]
        public async Task<JsonResponse> ForgotPassword(string Email)
        {
            try
            {
                return await _userService.ForgotPasswordRequest(Email);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost(Name = "ValidateOTP")]
        public async Task<JsonResponse> ValidateOTP(ValidateOTP req)
        {
            try
            {
                return await _userService.ValidateOTP(req.EncryptedOtp, req.EncryptedUserId);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost(Name = "VerifyEmail")]
        public async Task<JsonResponse> VerifyEmail(ValidateOTP req)
        {
            try
            {
                return await _userService.VerifyEmail(req.EncryptedOtp, req.EncryptedUserId);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet(Name = "DownloadImage")]
        public async Task<IActionResult> DownloadImage(string url)
        {
            try
            {
                var image = System.IO.File.OpenRead(url);
                return File(image, "image/jpeg");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error downloading image: {ex.Message}");
            }
        }

        [HttpGet(Name = "OnlineOfflineUsers")]
        public async Task<JsonResponse> OnlineOfflineUsers(string? connectionid)
        {
            try
            {
                return await _userService.OnlineOfflineUsers(user_unique_id,connectionid);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "GetOnlineUsers")]
        public async Task<JsonResponse> GetOnlineUsers()
        {
            try
            {
                return await _userService.GetOnlineUsers();
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "ChangePassword")]
        public async Task<JsonResponse> ChangePassword(ChangePasswordReq req)
        {
            try
            {
                var response = await _userService.ChangePassword(req,user_unique_id);

                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "DeactivateUserAccount")]
        public async Task<JsonResponse> DeactivateUserAccount(int timeperiod)
        {
            try
            {
                var response = await _userService.DeactivateUserAccount(timeperiod, user_unique_id);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
    }
}
