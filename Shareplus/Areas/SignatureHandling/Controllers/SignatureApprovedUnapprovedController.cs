using CDSMODULE.Helper;
using Entity.Common;
using Interface.Common;
using Interface.Security;
using Interface.Signature;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;


namespace CDSMODULE.Areas.SignatureHandling.Controllers
{
    [Authorize]
    [Area("SignatureHandling")]
    [AutoValidateAntiforgeryToken]
    public class SignatureApprovedUnapprovedController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private IWebHostEnvironment _webHostEnvironment;
        private readonly IOptions<ReadConfig> _connectionString;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly ISignatureApprovedUnapproved _signatureApprovedUnapproved;
        private readonly IAudit _audit;


        public SignatureApprovedUnapprovedController(IOptions<ReadConfig> connectionString, IWebHostEnvironment webHostEnvironment, ILoggedinUser _loggedInUser, ICheckUserAccess checkUserAccess, ISignatureApprovedUnapproved signatureApprovedUnapproved, IAudit audit)
        {
            this._connectionString = connectionString;
            this._webHostEnvironment = webHostEnvironment;
            this._loggedInUser = _loggedInUser;
            this._checkUserAccess = checkUserAccess;
            this._signatureApprovedUnapproved = signatureApprovedUnapproved;
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
        public JsonResponse GetAllSignatureList(string CompCode)
        {
            JsonResponse response = _signatureApprovedUnapproved.GetAllSignatureList(CompCode, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);

            return response;
        }
        [HttpPost]
        public JsonResponse GetSingleSignature(string CompCode, string ShHolderNo)
        {
            JsonResponse response = _signatureApprovedUnapproved.GetSingleSignature(CompCode, ShHolderNo, _loggedInUser.GetUserIPAddress());

            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);

            return response;
        }
        [HttpPost]
        public JsonResponse GetUnApproveHolderDetail(string CompCode, string ShHolderNo)
        {
            JsonResponse response = _signatureApprovedUnapproved.GetUnApproveHolderDetail(CompCode, ShHolderNo, _loggedInUser.GetUserIPAddress(),_loggedInUser.GetUserNameToDisplay());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);

            return response;
        }

        [HttpPost]
        public JsonResponse SaveApprove(string CompCode, List<string> ShHolderNos, bool hasPassword, string ScannedBy, string ApprovedDate, string SelectedAction, string Password = null)
        {
            if (hasPassword)
            {
                Password = Crypto.OneWayEncryter(Password);
            }
            JsonResponse response = _signatureApprovedUnapproved.SaveApprove(CompCode, ShHolderNos, ScannedBy, ApprovedDate, _loggedInUser.GetUserNameToDisplay(), SelectedAction, hasPassword, _loggedInUser.GetUserIPAddress(), Password);
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);

            return response;
        }
        [HttpPost]
        public JsonResponse SaveUnapprove(string CompCode, string ShHolderNo, string SelectedAction)
        {
            JsonResponse response = _signatureApprovedUnapproved.SaveUnapprove(CompCode, ShHolderNo, _loggedInUser.GetUserNameToDisplay(), SelectedAction, _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);

            return response;
        }
    }
}
