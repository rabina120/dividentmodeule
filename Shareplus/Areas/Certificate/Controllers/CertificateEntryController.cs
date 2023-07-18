using CDSMODULE.Helper;
using Entity.Common;
using Interface.Certificate;
using Interface.Common;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CDSMODULE.Areas.Certificate.Controllers
{
    [Authorize]
    [Area("Certificate")]
    [AutoValidateAntiforgeryToken]
    public class CertificateEntryController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        private readonly ICertificateEntry _certificateEntry;
        private readonly ICommon _common;

        public CertificateEntryController(ILoggedinUser _loggedInUser, ICheckUserAccess checkUserAccess, IAudit audit, ICertificateEntry certificateEntry, ICommon common)
        {
            this._loggedInUser = _loggedInUser;
            this._checkUserAccess = checkUserAccess;
            _audit = audit;
            _certificateEntry = certificateEntry;
            _common = common;
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
        public JsonResponse GetShHolderInformation(string ShHolderNo, string CompCode, string SelectedAction)
        {
            JsonResponse response = _certificateEntry.GetShHolderInformation(ShHolderNo, CompCode, SelectedAction, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }

        [HttpPost]
        public JsonResponse SearchCertificate(string ShHolderNoFrom, string ShHolderNoTo, string CertificateNoFrom, string CertificateNoTo, string SerialNoFrom, string SerialNoTo, string ShareKitttaFrom, string ShareKitttaTo, string CompCode)
        {
            JsonResponse response = _certificateEntry.SearchCertificate(ShHolderNoFrom, ShHolderNoTo, CertificateNoFrom, CertificateNoTo, SerialNoFrom, SerialNoTo, ShareKitttaFrom, ShareKitttaTo, CompCode, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
        [HttpPost]
        public JsonResponse LoadCertStatuses()
        {
            JsonResponse response = _common.GetCertificateStatus("1,2,3,4,5,6,7");
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
        [HttpPost]
        public JsonResponse LoadShOwnerTypes()
        {
            JsonResponse response = _common.GetShOwnerTypes(true);
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
        [HttpPost]
        public JsonResponse LoadShareTypes()
        {
            JsonResponse response = _common.GetAllShareTypes();
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
        [HttpPost]
        public JsonResponse SaveCertificate(string CompCode, string SelectedAction, string ShHolderNo, string CertificateNo, string ShareType, string ShareKitta,
            string CertificateIssuedDate, string CertificateType, string StartSerialNo, string EndSerialNo, string ShOwnerType)
        {
            JsonResponse response = _certificateEntry.SaveCertificate(CompCode, SelectedAction, ShHolderNo, CertificateNo, ShareType, ShareKitta, CertificateIssuedDate, CertificateType,
                StartSerialNo, EndSerialNo, ShOwnerType, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
    }
}
