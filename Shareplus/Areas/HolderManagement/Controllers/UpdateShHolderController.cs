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
    [Area("HolderManagement")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class UpdateShHolderController : Controller
    {
        private readonly ILoggedinUser _IloggedinUser;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        private readonly IUpdateShHolder _updateShHolder;
        private readonly IHolderInformation _holderInformation;
        public UpdateShHolderController(ILoggedinUser _loggedInUser, IHolderInformation holderInformation, ICheckUserAccess checkUserAccess, IAudit audit, IUpdateShHolder updateShHolder)
        {
            this._IloggedinUser = _loggedInUser;
            this._checkUserAccess = checkUserAccess;
            _audit = audit;
            _updateShHolder = updateShHolder;
            _holderInformation = holderInformation;
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
        public JsonResponse GetApplicationInformation(string CompCode, string ApplicationNo)
        {
            JsonResponse response = _updateShHolder.GetApplicationInformation(CompCode, ApplicationNo, _IloggedinUser.GetUserNameToDisplay(), _IloggedinUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse GetHolderInformation(string ShHolderNo, string CompCode, string selectedAction)
        {
            JsonResponse res = _holderInformation.GetSHholder(ShHolderNo, CompCode, selectedAction, _IloggedinUser.GetUserNameToDisplay(), _IloggedinUser.GetUserIPAddress());
            if (res.HasError)
            {
                res = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)res.ResponseData);
            }
            return res;
        }
        [HttpPost]
        public JsonResponse SaveShHolder(ATTShHolder aTTShHolder, string ApplicationNo)
        {
            aTTShHolder.UserName = _IloggedinUser.GetUserNameToDisplay();
            JsonResponse res = _updateShHolder.SaveApplicationUpdate(aTTShHolder, ApplicationNo, _IloggedinUser.GetUserNameToDisplay(), _IloggedinUser.GetUserIPAddress());
            if (res.HasError)
            {
                res = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)res.ResponseData);
            }
            return res;
        }

    }
}
