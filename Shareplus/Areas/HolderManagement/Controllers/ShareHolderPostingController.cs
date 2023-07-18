using CDSMODULE.Helper;
using Entity.Common;
using Entity.ShareHolder;
using Interface.Common;
using Interface.Security;
using Interface.ShareHolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CDSMODULE.Areas.HolderManagement.Controllers
{
    [Authorize]
    [Area("HolderManagement")]
    [AutoValidateAntiforgeryToken]
    public class ShareHolderPostingController : Controller
    {
        private readonly IShareHolderPosting _IshareHolderPosting;
        private readonly ILoggedinUser _IloggedinUser;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        public ShareHolderPostingController(ILoggedinUser _loggedInUser, IShareHolderPosting shareHolderPosting, ICheckUserAccess checkUserAccess, IAudit audit)
        {
            this._IloggedinUser = _loggedInUser;
            this._IshareHolderPosting = shareHolderPosting;
            this._checkUserAccess = checkUserAccess;
            _audit = audit;
        }
        public IActionResult Index()
        {
            ViewBag.UserName = _IloggedinUser.GetUserNameToDisplay();
            string UserId = _IloggedinUser.GetUserId();
            JsonResponse res = _audit.auditSave(_IloggedinUser.GetUserNameToDisplay(), "Opened " + this.ControllerContext.RouteData.Values["controller"].ToString() + " Form", this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress());
            if (res.IsSuccess)
            {
                if (_checkUserAccess.CheckIfAccessible(UserId, this.ControllerContext))
                    return View();
                return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });

        }

        public JsonResponse GetHolderForApproval(string CompCode, string SelectedAction,string FromDate,string ToDate)
        {
            JsonResponse response = _IshareHolderPosting.GetHolderForApproval(CompCode, SelectedAction,FromDate,ToDate, _IloggedinUser.GetUserNameToDisplay(), _IloggedinUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }


        [HttpPost]
        public JsonResponse PostShholderInfo(List<ATTShHolder> aTTShHolder, string CompCode, string SelectedRecordType)
        {
            string Username = _IloggedinUser.GetUserNameToDisplay();
            JsonResponse response = _IshareHolderPosting.PostShholderInfo(aTTShHolder, CompCode, Username, SelectedRecordType, Request.HttpContext.Connection.RemoteIpAddress.ToString());
            if (response.HasError)
            {
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }

    }
}

