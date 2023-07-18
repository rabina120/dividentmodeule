using CDSMODULE.Helper;
using Entity.Common;
using Entity.Dividend;
using Interface.Common;
using Interface.DividendManagement;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Shareplus.Areas.DividendManagement.Controllers
{
    [Authorize]
    [Area("DividendManagement")]
    [AutoValidateAntiforgeryToken]
    public class BoidTransferPostingController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private IWebHostEnvironment Environment;
        private IConfiguration Configuration;
        private readonly IOptions<ReadConfig> connectionString;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        private readonly IBoToBoTransfer _boTransfer;

        public BoidTransferPostingController(
            IOptions<ReadConfig> _connectionString,
            IWebHostEnvironment Environment,
            IConfiguration Configuration,
            ILoggedinUser loggedinUser,
            ICheckUserAccess checkUserAccess,
            IAudit audit,
            IBoToBoTransfer boTransfer)
        {
            this.connectionString = _connectionString;
            this.Environment = Environment;
            this.Configuration = Configuration;
            _loggedInUser = loggedinUser;
            this._loggedInUser = loggedinUser;
            this._checkUserAccess = checkUserAccess;
            _audit = audit;
            _boTransfer = boTransfer;
        }
        public IActionResult Index()
        {
            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();

            string UserId = _loggedInUser.GetUserId();
            JsonResponse res = _audit.auditSave(_loggedInUser.GetUserNameToDisplay(), "Opened " + this.ControllerContext.RouteData.Values["controller"].ToString() + " Form", this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress());
            if (res.IsSuccess)
            {
                if (_checkUserAccess.CheckIfAccessible(UserId, this.ControllerContext))
                    return View();
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }

        public object GetHolderChangelistForPosting(ATTHolderForBoidChange data)
        {
            JsonResponse response = new JsonResponse();
            data.UserName = _loggedInUser.GetUserName();
            data.IPAddress = _loggedInUser.GetUserIPAddress();
            response = _boTransfer.GetHolderChangelistForPosting(data);
            return JsonConvert.SerializeObject(response);
        }

        public object VerifyRejectHolderList(ATTHolderForBoidChange data)
        {
            JsonResponse response = new JsonResponse();
            data.UserName = _loggedInUser.GetUserName();
            data.IPAddress = _loggedInUser.GetUserIPAddress();
            response = _boTransfer.VerifyRejectHolderList(data);
            return JsonConvert.SerializeObject(response);
        }
    }
}
