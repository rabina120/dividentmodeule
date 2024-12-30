using CDSMODULE.Helper;
using Entity.Common;
using Interface.Common;
using Interface.Security;
using INTERFACE.FundTransfer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;


namespace CDSMODULE.Areas.FundTransfer.Controllers
{
    [Authorize]
    [Area("FundTransfer")]
    [AutoValidateAntiforgeryToken]
    public class ChangePinController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly ILogDetails _logDetails;
        private readonly IAudit _audit;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IChangePin _changepin;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IEService _eService;
        private IConfiguration _configuration;
        private readonly ILoginUser _login;

        public ChangePinController(ILoggedinUser _loggedInUser, ILogDetails logDetails, IConfiguration configuration,
            IAudit audit, ICheckUserAccess checkUserAccess, IChangePin changepin, IWebHostEnvironment hostingEnvironment, IEService eService, ILoginUser login)
        {
            this._loggedInUser = _loggedInUser;
            _logDetails = logDetails;
            _audit = audit;
            _checkUserAccess = checkUserAccess;
            _changepin = changepin;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _eService = eService;
            _login = login;
        }
        public IActionResult Index()
        {
            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();
            ViewBag.UserRole = _loggedInUser.GetUserType();
            string UserId = _loggedInUser.GetUserId();
            JsonResponse res = _audit.auditSave(_loggedInUser.GetUserName(), "Opened " + this.ControllerContext.RouteData.Values["controller"].ToString() + " Form", this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress());
            if (res.IsSuccess)
            {
                if (_checkUserAccess.CheckIfAccessible(UserId, this.ControllerContext))
                    return View();
                return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }

        [HttpPost]
         public JsonResponse ChangePin(string OldPin ,string NewPin,string NewCurrentPin)
        {
            JsonResponse res = _changepin.ChangePin(OldPin,NewPin,NewCurrentPin, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
            if (res.HasError)
                res = _audit.errorSave(_loggedInUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)res.ResponseData);
            return res;
        }


    }
}
