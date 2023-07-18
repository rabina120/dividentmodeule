using CDSMODULE.Helper;
using Entity.Common;
using Entity.ShareHolder;
using Interface.Common;
using Interface.Security;
using Interface.ShareHolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CDSMODULE.Areas.HolderManagement.Controllers
{
    [Authorize]
    [Area("HolderManagement")]
    [AutoValidateAntiforgeryToken]
    public class ShareHolderRelativeEntryController : Controller
    {
        private readonly ILoggedinUser _IloggedinUser;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IShareHolderRelativeEntry _shareHolderRelativeEntry;
        private readonly IAudit _audit;

        public ShareHolderRelativeEntryController(ILoggedinUser _loggedInUser, ICheckUserAccess checkUserAccess, IShareHolderRelativeEntry shareHolderRelativeEntry, IAudit audit)
        {
            this._IloggedinUser = _loggedInUser;
            this._checkUserAccess = checkUserAccess;
            this._shareHolderRelativeEntry = shareHolderRelativeEntry;
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
        public JsonResponse GetShHolder(string CompCode, string SelectedAction, string ShHolderNo)
        {
            JsonResponse response = _shareHolderRelativeEntry.GetShHolder(CompCode, SelectedAction, ShHolderNo, _IloggedinUser.GetUserNameToDisplay(), _IloggedinUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [HttpPost]
        public JsonResponse GetRelativeShHolder(string CompCode, string SelectedAction, string ShHolderNo)
        {
            JsonResponse response = _shareHolderRelativeEntry.GetRelativeShHolder(CompCode, SelectedAction, ShHolderNo, _IloggedinUser.GetUserNameToDisplay(), _IloggedinUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse GetMaxSN(string CompCode, string SelectedAction, string ShHolderNo)
        {
            JsonResponse response = _shareHolderRelativeEntry.GetMaxSN(CompCode, SelectedAction, ShHolderNo, _IloggedinUser.GetUserNameToDisplay());
            if (response.HasError)
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse SaveShHolderRelative(string CompCode, int ShHolderNo, string SN, ATTShHolder relativeShholder, string SelectedAction)
        {
            JsonResponse response = _shareHolderRelativeEntry.SaveRelativeEntry(CompCode, ShHolderNo, SN, relativeShholder, SelectedAction, _IloggedinUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
            if (response.HasError)
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse GetRelativeShHolderForUpdateDelete(string CompCode, string SelectedAction, string ShHolderNo)
        {
            JsonResponse response = _shareHolderRelativeEntry.GetRelativeShHolderForUpdateDelete(CompCode, SelectedAction, ShHolderNo, _IloggedinUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
            if (response.HasError)
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
    }
}
