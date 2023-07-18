using CDSMODULE.Helper;
using Entity.Common;

using Interface.Common;
using Interface.DividendManagement;
using Interface.Reports;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CDSMODULE.Areas.DividendManagement.Controllers
{

    [Authorize]
    [Area("DividendManagement")]
    [AutoValidateAntiforgeryToken]
    public class HoldersHistoryController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IHoldersHistory _holdersHistory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAudit _audit;
        private readonly IGenericReport _genricReport;

        public HoldersHistoryController(ILoggedinUser _loggedInUser, ICheckUserAccess checkUserAccess, IWebHostEnvironment webHostEnvironment, IHoldersHistory holdersHistory, IAudit audit, IGenericReport genericReport)
        {
            this._loggedInUser = _loggedInUser;
            this._checkUserAccess = checkUserAccess;
            this._holdersHistory = holdersHistory;
            this._webHostEnvironment = webHostEnvironment;
            _audit = audit;
            _genricReport = genericReport;
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

        [HttpPost]
        public JsonResponse GetHolderHistoryData(string CompCode, string DivType, string ShareType, string ShHolderNo, int occupation)
        {

            JsonResponse response = _holdersHistory.GetHoldersHistory(CompCode, DivType, ShareType, ShHolderNo, occupation, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
       
    }
}

