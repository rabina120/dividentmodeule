

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
    public class CertificateReversalPostingController : Controller
    {

        private readonly ICheckUserAccess _checkUserAccess;
        private readonly ILoggedinUser _loggedInUser;
        private readonly IReversalPosting _reversalPosting;
        private readonly IAudit _audit;

        public CertificateReversalPostingController(ICheckUserAccess checkUserAccess, ILoggedinUser _loggedInUser, IReversalPosting reversalPosting, IAudit audit)
        {
            this._checkUserAccess = checkUserAccess;
            this._loggedInUser = _loggedInUser;
            this._reversalPosting = reversalPosting;
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
        public JsonResponse GetDataForPosting(string CompCode, string FromDate, string ToDate)
        {
            JsonResponse response = _reversalPosting.GetDataForPosting(CompCode,FromDate, ToDate, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse ViewSingleRematerializeDetail(string CompCode, string RevTranNo, string RegNo, string DrnNo)
        {
            JsonResponse response = _reversalPosting.ViewSingleRematerializeDetail(CompCode, RevTranNo, RegNo, DrnNo, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse PostData(string Compcode, string SelectedAction, string Remarks, string PostingDate, List<ATTReversalCertificate> ReversalCertificates)
        {
            JsonResponse response = _reversalPosting.PostData(Compcode, SelectedAction, Remarks, PostingDate, ReversalCertificates, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
    }
}
