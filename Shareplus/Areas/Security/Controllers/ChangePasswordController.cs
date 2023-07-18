using CDSMODULE.Helper;
using Entity.Common;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace CDSMODULE.Areas.Security.Controllers
{
    [Authorize]
    [Area("Security")]
    [AutoValidateAntiforgeryToken]
    public class ChangePasswordController : Controller
    {
        private readonly ILoginUser _loginUser;
        private readonly ILoggedinUser IloggedinUser;

        public ChangePasswordController(ILoginUser loginUser, ILoggedinUser _IloggedinUser)
        {
            this._loginUser = loginUser;
            this.IloggedinUser = _IloggedinUser;
        }
        public IActionResult Index()
        {

            ViewBag.UserName = IloggedinUser.GetUserNameToDisplay();
            return View();
        }

        [HttpPost]
        public string ChangeUserPassword(string Password, string NewPassword, string PasswordChangeAlertDate)
        {
            var UserName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            Password = Crypto.OneWayEncryter(Password);
            NewPassword = Crypto.OneWayEncryter(NewPassword);
            return JsonConvert.SerializeObject(_loginUser.ChangePassword(IloggedinUser.GetUserNameToDisplay(), UserName, Password, NewPassword, PasswordChangeAlertDate, Request.HttpContext.Connection.RemoteIpAddress.ToString()));
        }
    }
}
