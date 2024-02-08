using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SpiritualNetwork.API.AppContext;
using SpiritualNetwork.API;

namespace SpiritualNetwork.API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class ApiBaseController : Controller
    {
        protected AppDbContext DbContext => (AppDbContext)HttpContext.RequestServices.GetService(typeof(AppDbContext));
        protected int user_unique_id;
        protected string user_email;
        protected string username;
        protected string user_role;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (User.HasClaim(c => c.Type == "Id"))
            {
                string userid = User.Claims.SingleOrDefault(c => c.Type == "Id").Value.ToString();
                user_unique_id = int.Parse(userid);
                GlobalVariables.LoginUserId = user_unique_id;
            }

            if (User.HasClaim(c => c.Type == "Email"))
            {
                var email = User.Claims.SingleOrDefault(c => c.Type == "Email");
                user_email = email.Value.ToString();
                GlobalVariables.LoginUserEmail = user_email;
            }

            if (User.HasClaim(c => c.Type == "UserName"))
            {
                var user = User.Claims.SingleOrDefault(c => c.Type == "UserName");
                username = user.Value.ToString();
                GlobalVariables.LoginUserName = username;
            }
        }
    }
}
