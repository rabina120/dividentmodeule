using CDSMODULE.Helper;
using Entity.Common;
using Entity.Dividend;

using Interface.Common;
using Interface.DividendProcessing;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CDSMODULE.Areas.DividendProcessing.Controllers
{
    [Authorize]
    [Area("DividendProcessing")]
    [AutoValidateAntiforgeryToken]
    public class DividendIssuePostingController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly ICashIssueDividendPosting IcashIssueDividendPosting;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        public DividendIssuePostingController(ILoggedinUser _loggedInUser, ICashIssueDividendPosting _IcashIssueDividendPosting, ICheckUserAccess checkUserAccess, IAudit audit)
        {
            this._loggedInUser = _loggedInUser;
            this.IcashIssueDividendPosting = _IcashIssueDividendPosting;
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
        public JsonResponse GetCashDividendForApproval(string CompCode, string FromDate, string ToDate, string Divcode)
        {
            JsonResponse response = IcashIssueDividendPosting.GetCashDividendForApproval(CompCode, FromDate, ToDate, Divcode, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }

        [RequestSizeLimit(2147483648)]
        [HttpPost]
        public JsonResponse PostCashDividendRequest(string aTTCashDividends, ATTCashDividend RecordDetails, string ActionType)
        {

            List < ATTCashDividend > aTTCashDividends1 = new List < ATTCashDividend >();
            aTTCashDividends1 = JsonConvert.DeserializeObject<List<ATTCashDividend>>(aTTCashDividends); 

            string UserName = _loggedInUser.GetUserNameToDisplay();
            JsonResponse response = IcashIssueDividendPosting.PostCashDividendRequest(aTTCashDividends1, RecordDetails, ActionType, UserName, Request.HttpContext.Connection.RemoteIpAddress.ToString());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
    }
}
