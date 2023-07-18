using CDSMODULE.Helper;
using Entity.Common;
using Entity.Dividend;

using Interface.Common;
using Interface.Parameter;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CDSMODULE.Areas.ParameterSetup.Controllers
{
    [Authorize]
    [Area("ParameterSetup")]
    [AutoValidateAntiforgeryToken]
    public class DividendPostingController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IDividendParameterPosting dividendParameterPosting;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;

        public DividendPostingController(ILoggedinUser _loggedInUser,
            IDividendParameterPosting dividendParameterPosting, ICheckUserAccess checkUserAccess, IAudit audit)
        {
            this._loggedInUser = _loggedInUser;
            this.dividendParameterPosting = dividendParameterPosting;
            this._checkUserAccess = checkUserAccess;
            _audit = audit;
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
        public JsonResponse GetDividendForApproval(string CompCode)
        {
            JsonResponse response = dividendParameterPosting.GetDividendForApproval(CompCode);
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }

        [HttpPost]
        public JsonResponse DividendRequestPosting(List<ATTDividend> aTTDividend, string CompCode, string ActionType)
        {
            var UserName = _loggedInUser.GetUserNameToDisplay();
            JsonResponse response = dividendParameterPosting.DividendRequestPosting(aTTDividend, CompCode, UserName, ActionType);
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }


    }
}
