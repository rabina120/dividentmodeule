
using CDSMODULE.Helper;
using Entity.Common;
using Entity.ShareHolder;
using Interface.CDS;
using Interface.Common;
using Interface.Parameter;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CDSMODULE.Areas.CDS.Controllers
{
    [Authorize]
    [Area("CDS")]
    [AutoValidateAntiforgeryToken]
    public class DeMaterializeEntryController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IDematerializeEntry _dematerializeEntry;
        private readonly IDPSetup _idpSetup;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;

        public DeMaterializeEntryController(ILoggedinUser _loggedInUser, IDematerializeEntry dematerializeEntry, IDPSetup idpSetup, ICheckUserAccess checkUserAccess, IAudit audit)
        {
            this._loggedInUser = _loggedInUser;
            _dematerializeEntry = dematerializeEntry;
            _idpSetup = idpSetup;
            this._checkUserAccess = checkUserAccess;
            _audit = audit;
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
        public JsonResponse GetAllParaCompChild(string CompCode)
        {
            JsonResponse response = _dematerializeEntry.GetAllParaCompChild(CompCode);
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserId(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse GetShHolderInformation(string CompCode, string ShHolderNo, string Occupation)
        {
            JsonResponse response = _dematerializeEntry.GetShHolderInformation(CompCode, ShHolderNo, Occupation, _loggedInUser.GetUserId(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserId(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse GetMaxRegNo(string CompCode)
        {
            JsonResponse response = _dematerializeEntry.GetMaxRegNo(CompCode);
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserId(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [HttpPost]
        public JsonResponse GetDP()
        {
            JsonResponse response = _idpSetup.LoadDPDetailList();
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserId(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [HttpPost]
        public JsonResponse GetCertificateDetails(string CompCode, string DemateType, string ShOwnerType, string HolderNo, string SrNoFrom, string SrNoTo)
        {
            JsonResponse response = _dematerializeEntry.GetCertificateDetails(CompCode, DemateType, ShOwnerType, HolderNo, SrNoFrom, SrNoTo, _loggedInUser.GetUserId(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserId(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        public JsonResponse GetDataFromCertificateDetail(string CompCode)
        {
            JsonResponse response = _dematerializeEntry.GetDataFromCertificateDetail(CompCode);
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserId(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        public JsonResponse GetMaxDemateRegNo(string CompCode)
        {
            JsonResponse response = _dematerializeEntry.GetMaxDemateRegNo(CompCode);
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserId(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [HttpPost]
        public JsonResponse GetStartSrNoEndSrNo(string CompCode, string BonusIssueCode)
        {
            JsonResponse response = _dematerializeEntry.GetStartSrNoEndSrNo(CompCode, BonusIssueCode);
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserId(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [RequestSizeLimit(2147483648)]
        [HttpPost]
        public JsonResponse SaveDematerializeCertificate(List<ATTCertDet> CertificateList, string CompCode, string DemateRegNo, string ShHolderNo, string EntryDate, string DemateReqDate, string DrnNo, string DPCode, string Remarks, string RegNO, string ISINNo, string BonusCode, string BOID, string SelectedAction)
        {
            JsonResponse response = _dematerializeEntry.SaveDematerializeCertificate(CertificateList, CompCode, DemateRegNo, ShHolderNo, EntryDate, DemateReqDate, BOID, DrnNo, DPCode, Remarks, RegNO, ISINNo, BonusCode, SelectedAction, _loggedInUser.GetUserId(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserId(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [HttpPost]
        public JsonResponse GetSignature(string CompCode, string HolderNo)
        {
            JsonResponse response = _dematerializeEntry.GetSignature(CompCode, HolderNo);
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserId(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [HttpPost]
        public JsonResponse GetHolderByQuery(string CompCode, string FirstName, string Occupation)
        {
            JsonResponse response = _dematerializeEntry.GetHolderByQuery(CompCode, FirstName, Occupation);
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserId(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [HttpPost]
        public JsonResponse GetDematedCertificateList(string CompCode, string HolderNo)
        {
            JsonResponse response = _dematerializeEntry.GetDematedCertificateList(CompCode, HolderNo);
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserId(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse GetDematedCertificateDetails(string CompCode, string DemateRegNo)
        {
            JsonResponse response = _dematerializeEntry.GetDematedCertificateDetails(CompCode, DemateRegNo);
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserId(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
    }
}
