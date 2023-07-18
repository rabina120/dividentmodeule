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
    [Area("ParameterSetup")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class BrokerSetupController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IBrokerSetup brokerSetup;
        private readonly IAudit _audit;
        private readonly ICheckUserAccess _checkUserAccess;

        public BrokerSetupController(ILoggedinUser _loggedInUser, IBrokerSetup _brokerSetup, IAudit audit, ICheckUserAccess checkUserAccess)
        {
            this._loggedInUser = _loggedInUser;
            this.brokerSetup = _brokerSetup;
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
        public JsonResponse GetBrokerCode()
        {

            JsonResponse response = brokerSetup.GetBrokerCode();
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);

            return response;
        }

        [HttpPost]
        public JsonResponse GetBrokerDetail(string Bcode)
        {
            JsonResponse response = brokerSetup.GetBrokerDetail(Bcode, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [HttpPost]
        public JsonResponse SaveBrokerDetails(ATTBroker aTTBroker, string ActionType)
        {
            JsonResponse response = brokerSetup.SaveBrokerDetails(aTTBroker, ActionType, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
    }
}
