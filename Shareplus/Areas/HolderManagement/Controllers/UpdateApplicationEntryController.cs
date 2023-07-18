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
    public class UpdateApplicationEntryController : Controller
    {
        private readonly ILoggedinUser _IloggedinUser;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IEntryUpdateApplication _entryUpdateApplication;
        public readonly IAudit _audit;

        public UpdateApplicationEntryController(ILoggedinUser _loggedInUser, ICheckUserAccess checkUserAccess,
            IEntryUpdateApplication entryUpdateApplication, IAudit audit)
        {
            this._IloggedinUser = _loggedInUser;
            this._checkUserAccess = checkUserAccess;
            this._entryUpdateApplication = entryUpdateApplication;
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
        public JsonResponse GetInformationForApplication(string CompCode, string SelectedAction, string ShHolderNo, string ApplicationNo = null)
        {
            JsonResponse response = _entryUpdateApplication.GetInformationForApplication(CompCode, ShHolderNo, SelectedAction, _IloggedinUser.GetUserNameToDisplay(), _IloggedinUser.GetUserIPAddress(), ApplicationNo);
            if (response.HasError)
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse GetInformationFromApplicationNo(string CompCode, string SelectedAction, string ApplicationNo)
        {
            JsonResponse response = _entryUpdateApplication.GetInformationFromApplicationNo(CompCode, SelectedAction, _IloggedinUser.GetUserNameToDisplay(), _IloggedinUser.GetUserIPAddress(), ApplicationNo);
            if (response.HasError)
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [HttpPost]
        public JsonResponse SaveApplicationForUpdate(string CompCode, string ShHolderNo, string ApplicationDate, string Action, string SelectedAction, string ApplicationNo = null)
        {
            JsonResponse response = _entryUpdateApplication.SaveApplicationForUpdate(CompCode, ShHolderNo, ApplicationDate, Action, SelectedAction, _IloggedinUser.GetUserNameToDisplay(), _IloggedinUser.GetUserIPAddress(), ApplicationNo);
            if (response.HasError)
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
    }
}
