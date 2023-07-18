using Entity.Common;
using Interface.Security;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CDSMODULE.Areas.Common.Controllers
{
    [Area("Common")]
    [AutoValidateAntiforgeryToken]
    public class ChangePasswordController : Controller
    {
        private readonly ILoginUser _loginUser;

        public ChangePasswordController(ILoginUser loginUser)
        {
            _loginUser = loginUser;
        }

        public IActionResult Index(string UserName = null)
        {
            ViewBag.CurrentUserName = UserName;

            return View();
        }

        [HttpPost]
        public string ChangeUserPassword(string UserName, string Password, string NewPassword, string PasswordChangeAlertDate)
        {
            Password = Crypto.OneWayEncryter(Password);
            NewPassword = Crypto.OneWayEncryter(NewPassword);
            return JsonConvert.SerializeObject(_loginUser.ChangePassword(UserName, UserName, Password, NewPassword, PasswordChangeAlertDate, Request.HttpContext.Connection.RemoteIpAddress.ToString()));
        }
    }
}
