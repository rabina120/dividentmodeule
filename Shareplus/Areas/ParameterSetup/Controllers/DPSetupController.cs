

using CDSMODULE.Helper;
using Entity.Common;
using Entity.Parameter;
using Interface.Common;
using Interface.Parameter;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CDSMODULE.Areas.ParameterSetup.Controllers
{
    [Authorize]
    [Area("ParameterSetup")]
    [AutoValidateAntiforgeryToken]
    public class DPSetupController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IDPSetup dpsetup;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;

        public DPSetupController(ILoggedinUser _loggedInUser, IDPSetup _dpsetup, IAudit audit, ICheckUserAccess checkUserAccess)
        {
            this._loggedInUser = _loggedInUser;
            this.dpsetup = _dpsetup;
            _checkUserAccess = checkUserAccess;
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
        public JsonResponse GetDpCode()
        {
            JsonResponse response = dpsetup.GetDPCode();
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [HttpPost]
        public JsonResponse LoadDPDetailList()
        {
            JsonResponse response = dpsetup.LoadDPDetailList(_loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse GetDPDetails(string DPCode)
        {
            JsonResponse response = dpsetup.GetDPDetails(DPCode, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse SaveDPDetails(ATTDPSetup aTTDPSetup, string Actiontype)
        {
            JsonResponse response = dpsetup.SaveDPDetails(aTTDPSetup, Actiontype, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
    }
}
