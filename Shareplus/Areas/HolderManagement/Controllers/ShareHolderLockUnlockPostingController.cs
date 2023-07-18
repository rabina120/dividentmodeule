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
    [Area("HolderManagement")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class ShareHolderLockUnlockPostingController : Controller
    {
        private readonly ILoggedinUser _IloggedinUser;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IShHolderLockUnlockPosting _shHolderLockUnlockPosting;
        private readonly IAudit _audit;

        public ShareHolderLockUnlockPostingController(ILoggedinUser _loggedInUser, ICheckUserAccess checkUserAccess, IShHolderLockUnlockPosting shHolderLockUnlockPosting, IAudit audit)
        {
            this._IloggedinUser = _loggedInUser;
            this._checkUserAccess = checkUserAccess;
            this._shHolderLockUnlockPosting = shHolderLockUnlockPosting;
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

        [HttpPost]
        public JsonResponse GetLockUnlockData(string CompCode,string FromDate, string ToDate, string RecordType)
        {
            JsonResponse response = _shHolderLockUnlockPosting.GetLockUnlockData(CompCode,FromDate,ToDate, RecordType, _IloggedinUser.GetUserNameToDisplay(), _IloggedinUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [HttpPost]
        public JsonResponse PostLockUnlockData(string CompCode, string RecordType, List<ATTShHolderLockUnlock> ShHolderLocks,
            string Remarks, string SelectedAction, string PostingDate)
        {
            JsonResponse response = _shHolderLockUnlockPosting.PostLockUnlockData(CompCode, RecordType, ShHolderLocks, Remarks, SelectedAction, PostingDate, _IloggedinUser.GetUserNameToDisplay(), _IloggedinUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

    }
}
