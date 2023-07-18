using CDSMODULE.Helper;
using Entity.CDS;
using Entity.Common;
using Interface.CDS;
using Interface.Common;
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
    public class DeMaterializePostingController : Controller
    {

        private readonly ILoggedinUser _loggedInUser;
        private readonly IDematerializePosting dematerializePosting;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;

        public DeMaterializePostingController(IDematerializePosting dematerializePosting, ILoggedinUser _loggedInUser, ICheckUserAccess checkUserAccess, IAudit audit)
        {
            this._loggedInUser = _loggedInUser;
            this.dematerializePosting = dematerializePosting;
            this._checkUserAccess = checkUserAccess;
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
        public JsonResponse GetParaCompChildList(string CompCode)
        {
            JsonResponse response = dematerializePosting.GetParaCompChildList(CompCode);
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;

        }

        [HttpPost]
        public JsonResponse GetDeMaterializeData(string CompCode, string FromDate, string ToDate, string RegNoFrom, string RegNoTo, string ISINNO, string CheckCA)
        {
            JsonResponse response = dematerializePosting.GetDeMaterializeData(CompCode, FromDate, ToDate, RegNoFrom, RegNoTo, ISINNO, CheckCA, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [HttpPost]
        public JsonResponse GetSingleDematerializeData(string CompCode, string DemateRegno, string RegNo, string ISINNo, string Remarks, string DRNNo)
        {
            JsonResponse response = dematerializePosting.GetSingleDeMaterializeData(CompCode, DemateRegno, RegNo, ISINNo, Remarks, DRNNo, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [HttpPost]
        public JsonResponse PostDeMaterializeEntry(List<CertificateDemateDetails> certificateDemate, CertificateDemateDetails RecordDetails, string ActionType)
        {
            JsonResponse response = dematerializePosting.PostDeMaterializeEntry(certificateDemate, RecordDetails, ActionType, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

    }
}
