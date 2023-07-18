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
    public class PaymentSetupController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IPaymentType paymentType;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;

        public PaymentSetupController(ILoggedinUser _loggedInUser, IPaymentType _paymentType, ICheckUserAccess checkUserAccess, IAudit audit)
        {
            this._loggedInUser = _loggedInUser;
            this.paymentType = _paymentType;
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
        public JsonResponse GetPaymentCode()
        {
            return paymentType.GetPaymentCode();
        }

        [HttpPost]
        public JsonResponse GetPaymentDetails(string CenterId)
        {
            JsonResponse response = paymentType.GetPaymentDetails(CenterId, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }

        [HttpPost]
        public JsonResponse SavePaymentDetails(ATTPamentType aTTPamentType, string ActionType)
        {
            JsonResponse response = paymentType.SavePaymentDetails(aTTPamentType, ActionType, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
    }
}
