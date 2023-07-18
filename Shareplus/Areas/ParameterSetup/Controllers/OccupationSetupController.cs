using CDSMODULE.Helper;
using Entity.Common;
using Entity.ShareHolder;
using Interface.Common;
using Interface.Parameter;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CDSMODULE.Areas.ParameterSetup.Controllers
{
    [Area("ParameterSetup")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class OccupationSetupController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IOccupationSetup occupationSetup;
        private readonly IAudit _audit;
        private readonly ICheckUserAccess _checkUserAccess;
        public OccupationSetupController(ILoggedinUser _loggedInUser, IOccupationSetup _occupationSetup, IAudit audit, ICheckUserAccess checkUserAccess)
        {
            this._loggedInUser = _loggedInUser;
            this.occupationSetup = _occupationSetup;
            _audit = audit;
            _checkUserAccess = checkUserAccess;
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
        public JsonResponse GetOccupationCode()
        {
            return occupationSetup.GetOccupationCode();
        }

        [HttpPost]
        public JsonResponse GetOccupationDetails(string OccupationId)
        {
            JsonResponse response = occupationSetup.GetOccupationDetails(OccupationId, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }

        [HttpPost]
        public JsonResponse LoadOccupationList()
        {
            JsonResponse response = occupationSetup.LoadOccupationList(_loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }

        [HttpPost]
        public JsonResponse SaveOccupationDetails(ATTOccupation aTTOccupation, string ActionType)
        {
            JsonResponse response = occupationSetup.SaveOccupationDetails(aTTOccupation, ActionType, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
    }
}
