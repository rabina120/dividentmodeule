using CDSMODULE.Helper;
using Entity.Common;
using Entity.ShareHolder;
using Interface.Security;
using Interface.ShareHolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CDSMODULE.Areas.HolderManagement.Controllers
{
    [Area("HolderManagement")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class CertificateController : Controller
    {
        private readonly ICertDet _iCertDet;
        private readonly IAudit _audit;
        private readonly ILoggedinUser _loggedInUser;
        public CertificateController(ICertDet iCertDet, IAudit audit, ILoggedinUser _loggedInUser)
        {
            _iCertDet = iCertDet;
            _audit = audit;
            this._loggedInUser = _loggedInUser;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResponse GetCertDet(int ShHolderNo, string CompCode)
        {
            JsonResponse response = _iCertDet.GetCertDet(ShHolderNo, CompCode);
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [HttpPost]
        public JsonResponse GetCertStatuses()
        {
            JsonResponse response = _iCertDet.GetCertStatuses();
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPut]
        public JsonResponse UpdateCertificates(int shholderno, List<ATTCertDet> lisOfCertificates)
        {
            JsonResponse response = _iCertDet.UpdateCertificate(shholderno, lisOfCertificates);
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
    }
}
