using CDSMODULE.Helper;
using Entity.Certificate;
using Entity.Common;
using Interface.Certificate;
using Interface.Common;
using Interface.Security;
using Interface.ShareHolder;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Shareplus.Areas.Certificate.Controllers
{
    [Area("Certificate")]
    [AutoValidateAntiforgeryToken]
    public class PrintController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IHolderInformation _holderInformation;
        private readonly ICertificateList _certificateList;
        private readonly IAudit _audit;
        private readonly IPrint _print;
        public PrintController(IAudit audit, ICheckUserAccess checkUserAccess, ILoggedinUser _loggedInUser, IHolderInformation holderInformation, ICertificateList certificateList, IPrint print)
        {
            _checkUserAccess = checkUserAccess;
            this._loggedInUser = _loggedInUser;
            this._audit = audit;
            this._holderInformation = holderInformation;
            this._certificateList = certificateList;
            _print = print;
        }
        public IActionResult Index()
        {
            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();

            string UserId = _loggedInUser.GetUserId();
            JsonResponse res = _audit.auditSave(_loggedInUser.GetUserId(), "Opened " + this.ControllerContext.RouteData.Values["controller"].ToString() + " Form", this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress());
            if (res.IsSuccess)
            {
                if (_checkUserAccess.CheckIfAccessible(UserId, this.ControllerContext))
                    return View();
                return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }

        [HttpPost]
        public JsonResponse GetHolderInformation(string ShHolderNo, string CompCode)
        {
            JsonResponse response = _holderInformation.GetSHholder(ShHolderNo, CompCode, null, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }

        [HttpPost]
        public JsonResponse GetAllCertificates(int ShHolderNo, string CompCode)
        {
            ATTDuplicateCertificate ReportData = new ATTDuplicateCertificate();
            ReportData.HolderNoFrom = ShHolderNo;
            ReportData.HolderNoTo = ShHolderNo;
            ReportData.CompCode = CompCode;
            JsonResponse response = new JsonResponse();
            response = _certificateList.AllCertificateListForPDF(ReportData, null, null);
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }

        [HttpPost]
        public object PrintCertificates(List<ATTDuplicateCertificate> list, string CompCode)
        {
            JsonResponse response = new JsonResponse();
            response = _print.PrintCertificates(list, CompCode, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }


    }
}
