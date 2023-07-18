using CDSMODULE.Helper;
using Entity.Common;
using Interface.Common;
using Interface.Esewa;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CDSMODULE.Areas.ESewaTransaction.Controllers
{
    [Authorize]
    [Area("ESewaTransaction")]
    [AutoValidateAntiforgeryToken]
    public class BankDetailsController : Controller
    {

        private readonly ILoggedinUser _loggedInUser;
        private readonly IConfiguration _configuration;
        private readonly IAudit _audit;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IBatchProcessing _batchProcessing;

        public BankDetailsController(ILoggedinUser _loggedInUser, IConfiguration configuration, IAudit audit, ICheckUserAccess checkUserAccess, IBatchProcessing batchProcessing)
        {
            this._loggedInUser = _loggedInUser;
            _configuration = configuration;
            _audit = audit;
            _checkUserAccess = checkUserAccess;
            _batchProcessing = batchProcessing;
        }
        public IActionResult Index()
        {
            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();
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
        public JsonResponse GetBanks()
        {
            return _batchProcessing.GetBanks(_loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress(), this.ControllerContext.RouteData.Values["controller"].ToString());
        }

        [HttpPost]
        public JsonResponse GetBanksFromEsewa()
        {
            return _batchProcessing.UpdateBankDetailsFromEsewa(_loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
        }

        [HttpPost]
        public JsonResponse UpdateBankDetails(string bankcode, string swiftcode)
        {
            return _batchProcessing.UpdateBankDetailsToSystem(swiftcode, bankcode, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
        }
    }
}
