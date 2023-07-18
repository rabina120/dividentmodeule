
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
    public class CompanySetupController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly ICompanySetup companySetup;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;


        public CompanySetupController(ILoggedinUser _loggedInUser, ICompanySetup _companySetup, IAudit audit, ICheckUserAccess checkUserAccess)
        {
            this._loggedInUser = _loggedInUser;
            this.companySetup = _companySetup;
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
        public JsonResponse GetCompanyCode()
        {
            JsonResponse response = companySetup.GetCompanyCode();
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);

            return response;
        }

        [HttpPost]
        public JsonResponse GetCompanyDetails(string CompCode)
        {
            JsonResponse response = companySetup.GetCompanyDetails(CompCode);
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);

            return response;
        }

        [HttpPost]
        public JsonResponse SaveCompanyDetails(ATTCompanySetup aTTCompanySetup, string ActionType)
        {
            JsonResponse response = companySetup.SaveCompanyDetails(aTTCompanySetup, ActionType, _loggedInUser.GetUserNameToDisplay());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);

            return response;
        }
    }
}
