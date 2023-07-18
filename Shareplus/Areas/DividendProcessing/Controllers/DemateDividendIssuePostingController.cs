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
    [Area("DividendProcessing")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class DemateDividendIssuePostingController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly ICashDemateIssuePosting cashDemateIssuePosting;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;

        public DemateDividendIssuePostingController(ILoggedinUser _loggedInUser,
            ICashDemateIssuePosting _cashDemateIssuePosting, ICheckUserAccess checkUserAccess, IAudit audit)
        {
            this._loggedInUser = _loggedInUser;
            this.cashDemateIssuePosting = _cashDemateIssuePosting;
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
        public JsonResponse GetCashDemateForApproval(string CompCode, string FromDate, string ToDate, string Divcode)
        {
            JsonResponse response = cashDemateIssuePosting.GetCashDemateForApproval(CompCode, FromDate, ToDate, Divcode, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [RequestSizeLimit(2147483648)]
        [HttpPost]
        public JsonResponse PostCashDemateRequest(string attDivMasterCDS, ATTDivMasterCDS RecordDetails, string ActionType)
        {
            List<ATTDivMasterCDS> attDivMasterCDS1 = new List<ATTDivMasterCDS>();
            attDivMasterCDS1 = JsonConvert.DeserializeObject<List<ATTDivMasterCDS>>(attDivMasterCDS);

            string UserName = _loggedInUser.GetUserNameToDisplay();
            JsonResponse response = cashDemateIssuePosting.PostCashDemateRequest(attDivMasterCDS1, RecordDetails, ActionType, UserName, Request.HttpContext.Connection.RemoteIpAddress.ToString());
            
            if(response.HasError)response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
    }
}
