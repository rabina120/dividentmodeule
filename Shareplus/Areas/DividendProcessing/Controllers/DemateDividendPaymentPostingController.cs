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
    public class DemateDividendPaymentPostingController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IDemateDividentPaymentPosting demateDividentPaymentPosting;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;

        public DemateDividendPaymentPostingController(ILoggedinUser _loggedInUser,
            IDemateDividentPaymentPosting _demateDividentPaymentPosting, ICheckUserAccess checkUserAccess, IAudit audit)
        {
            this._loggedInUser = _loggedInUser;
            this.demateDividentPaymentPosting = _demateDividentPaymentPosting;
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
        public JsonResponse GetDemateDividentForApproval(string CompCode,string FromDate, string ToDate, string Divcode)
        {

            JsonResponse response = demateDividentPaymentPosting.GetDemateDividentForApproval(CompCode, FromDate, ToDate, Divcode, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [RequestSizeLimit(2147483648)]
        [HttpPost]
        public JsonResponse PostDemateDividentPaymentPosting(string attDemateDividentPaymentPosting, string PostingRemarks,string PostingDate,string CompCode,string Divcode, string ActionType)
        {
            List<ATTDemateDividentPaymentPosting> attDemateDividentPaymentPosting1 = new List<ATTDemateDividentPaymentPosting>();
            attDemateDividentPaymentPosting1 = JsonConvert.DeserializeObject<List<ATTDemateDividentPaymentPosting>>(attDemateDividentPaymentPosting);

            string UserName = _loggedInUser.GetUserNameToDisplay();
            JsonResponse response = demateDividentPaymentPosting.PostDemateDividentPaymentPosting(attDemateDividentPaymentPosting1, PostingRemarks,PostingDate, ActionType,  CompCode,  Divcode, UserName, Request.HttpContext.Connection.RemoteIpAddress.ToString());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
    }
}
