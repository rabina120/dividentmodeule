using CDSMODULE.Helper;
using Entity.Common;
using Interface.Common;
using Interface.Security;
using Interface.ShareHolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CDSMODULE.Areas.HolderManagement.Controllers
{
    [Area("HolderManagement")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class ShareHolderLockUnlockEntryController : Controller
    {
        private readonly ILoggedinUser _IloggedinUser;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        private readonly IShHolderLockUnlockEntry _shHolderLockUnlockEntry;


        public ShareHolderLockUnlockEntryController(ILoggedinUser _loggedInUser, ICheckUserAccess checkUserAccess, IShHolderLockUnlockEntry shHolderLockUnlockEntry, IAudit audit)
        {
            this._IloggedinUser = _loggedInUser;
            this._checkUserAccess = checkUserAccess;
            this._shHolderLockUnlockEntry = shHolderLockUnlockEntry;
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
        public JsonResponse GetHolderForLockUnlock(string CompCode, string ShHolderNo, string RecordType, string SelectedAction)
        {
            JsonResponse response = _shHolderLockUnlockEntry.GetHolderForLockUnlock(CompCode, ShHolderNo, RecordType, SelectedAction, _IloggedinUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
            if (response.HasError)
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);

            return response;
        }

        [HttpPost]
        public JsonResponse GetMaxLockId(string CompCode)
        {
            JsonResponse response = _shHolderLockUnlockEntry.GetMaxLockId(CompCode);
            if (response.HasError)
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);

            return response;
        }
        [HttpPost]
        public JsonResponse GetRecordShHolderLuckDetail(string CompCode, string SelectedAction = null, string RecordType = null)
        {
            JsonResponse response = _shHolderLockUnlockEntry.GetRecordShHolderLuckDetail(CompCode, _IloggedinUser.GetUserNameToDisplay(), _IloggedinUser.GetUserIPAddress(), SelectedAction, RecordType);
            if (response.HasError)
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);

            return response;
        }

        [HttpPost]
        public JsonResponse SaveHolderLockUnlock(string CompCode, string ShHolderNo, string RecordType, string SelectedAction,
            string LockId, string LockDate, string LockRemarks, string UnlockDate , string UnlockRemarks)
        {
            JsonResponse response = _shHolderLockUnlockEntry.SaveHolderLockUnlock(CompCode, ShHolderNo, RecordType, SelectedAction, LockId, LockDate,
                LockRemarks, _IloggedinUser.GetUserNameToDisplay(), _IloggedinUser.GetUserIPAddress(),UnlockDate,UnlockRemarks);
            if (response.HasError)
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);

            return response;
        }
        [HttpPost]
        public JsonResponse GetHolderByLockId(string CompCode, string LockId, string RecordType, string SelectedAction)
        {
            JsonResponse response = _shHolderLockUnlockEntry.GetHolderByLockId(CompCode, LockId, RecordType, SelectedAction, _IloggedinUser.GetUserNameToDisplay(), _IloggedinUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;

        }
    }
}
