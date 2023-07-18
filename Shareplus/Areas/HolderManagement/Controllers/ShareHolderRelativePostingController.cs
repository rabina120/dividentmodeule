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
    public class ShareHolderRelativePostingController : Controller
    {

        private readonly ILoggedinUser _IloggedinUser;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IShareHolderRelativePosting _shareHolderRelativePosting;
        private readonly IAudit _audit;

        public ShareHolderRelativePostingController(ILoggedinUser _loggedInUser, ICheckUserAccess checkUserAccess, IShareHolderRelativePosting shareHolderRelativePosting, IAudit audit)
        {
            this._IloggedinUser = _loggedInUser;
            this._checkUserAccess = checkUserAccess;
            this._shareHolderRelativePosting = shareHolderRelativePosting;
            this._audit = audit;
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


        [HttpPost]
        public JsonResponse GetHolderForPosting(string CompCode, string fromDate, string toDate)
        {
            JsonResponse response = _shareHolderRelativePosting.GetHolderForPosting(CompCode,fromDate, toDate, _IloggedinUser.GetUserNameToDisplay(), _IloggedinUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [HttpPost]
        public JsonResponse SaveShHolderRelative(string CompCode, List<ATTShHolderForRelative> attShHolderForRelatives, string SelectedAction, string ApprovedDate)
        {
            JsonResponse response = _shareHolderRelativePosting.SaveHolderPosting(CompCode, attShHolderForRelatives, SelectedAction, ApprovedDate, _IloggedinUser.GetUserNameToDisplay(), _IloggedinUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

    }
}
