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
    public class DividendPaymentPostingController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IDividentPaymentEntryPosting IDividentPaymentEntryPosting;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        public DividendPaymentPostingController(ILoggedinUser _loggedInUser, IDividentPaymentEntryPosting _IDividentPaymentEntryPosting, ICheckUserAccess checkUserAccess, IAudit audit)
        {
            this._loggedInUser = _loggedInUser;
            this.IDividentPaymentEntryPosting = _IDividentPaymentEntryPosting;
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
        public JsonResponse GetDividentPaymentForApproval(string CompCode, string FromDate, string ToDate, string Divcode)
        {
            JsonResponse response = IDividentPaymentEntryPosting.GetDividentPaymentForApproval(CompCode,FromDate, ToDate, Divcode, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }

        [RequestSizeLimit(2147483648)]
        [HttpPost]
        public JsonResponse PostDividentPaymentRequest(string aTTDividentPaymentEntrys, ATTDividentPaymentEntry RecordDetails, string ActionType)
        {
            List<ATTDividentPaymentEntry> aTTDividentPaymentEntrys1 = new List<ATTDividentPaymentEntry>();
            aTTDividentPaymentEntrys1 = JsonConvert.DeserializeObject<List<ATTDividentPaymentEntry>>(aTTDividentPaymentEntrys);

            string UserName = _loggedInUser.GetUserNameToDisplay();
            JsonResponse response = IDividentPaymentEntryPosting.PostDividentPaymentRequest(aTTDividentPaymentEntrys1, RecordDetails, ActionType, UserName, _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
    }
}
