

using CDSMODULE.Helper;
using Entity.Certificate;
using Entity.Common;
using Interface.Certificate;
using Interface.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CDSMODULE.Areas.Certificate.Controllers
{
    [Authorize]
    [Area("Certificate")]
    [AutoValidateAntiforgeryToken]
    public class CertificateConsolidateController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly ICertificateConsolidate _certificateConsolidate;
        private readonly ICheckUserAccess _checkUserAccess;
        public CertificateConsolidateController(ILoggedinUser _loggedInUser, ICertificateConsolidate certificateConsolidate, ICheckUserAccess checkUserAccess)
        {
            this._loggedInUser = _loggedInUser;
            this._certificateConsolidate = certificateConsolidate;
            this._checkUserAccess = checkUserAccess;
        }

        public IActionResult Index()

        {
            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();

            if (_checkUserAccess.CheckIfAccessible(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext))
                return View();
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }

        [HttpPost]
        public JsonResponse GetShholderDetailsByShHolderNo(string CompCode, string ShholderNo, string SelectedAction)
        {
            return _certificateConsolidate.GetShholderDetailsByShHolderNo(CompCode, ShholderNo, SelectedAction, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
        }


        [HttpPost]
        public JsonResponse GetCertificateDetails(string CompCode, string ShholderNo, string CertificateNo, string SelectedAction)
        {
            return _certificateConsolidate.GetCertificateDetails(CompCode, ShholderNo, CertificateNo, SelectedAction, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
        }
        [HttpPost]
        public JsonResponse SaveCertificateConsolidate(string CompCode, string ShholderNo, List<ATTCertificateConsolidate> aTTCertificateConsolidate, string CertificateNo, string Splitdate, string remarks, string SelectedAction)
        {
            string UserName = _loggedInUser.GetUserNameToDisplay();
            return _certificateConsolidate.SaveCertificateConsolidate(CompCode, ShholderNo, aTTCertificateConsolidate, CertificateNo, Splitdate, remarks, SelectedAction, UserName, Request.HttpContext.Connection.RemoteIpAddress.ToString());
        }
    }
}
