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
    public class CertificateDistributionPostingController : Controller
    {

        private readonly ILoggedinUser _loggedInUser;
        private readonly ICertificateDistributionPosting certificateDistributionPosting;
        private readonly ICheckUserAccess _checkUserAccess;

        public CertificateDistributionPostingController(ILoggedinUser _loggedInUser, ICertificateDistributionPosting certificateDistributionPosting, ICheckUserAccess _checkUserAccess)
        {

            this._loggedInUser = _loggedInUser;
            this.certificateDistributionPosting = certificateDistributionPosting;
            this._checkUserAccess = _checkUserAccess;


        }

        public IActionResult Index()
        {
            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();
            if (_checkUserAccess.CheckIfAccessible(_loggedInUser.GetUserId(), this.ControllerContext))
                return View();
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }

        [HttpPost]
        public JsonResponse GetCertificateDistributionCompanyData(string CompCode, string startdate, string enddate)
        {
            return certificateDistributionPosting.GetCertificateDistributionCompanyData(CompCode, startdate, enddate, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
        }

        [HttpPost]
        public JsonResponse PostCertificateDistributionEntry(List<ATTCERTIFICATE> certificateDemate, ATTCERTIFICATE recordDetails, string ActionType)
        {
            return certificateDistributionPosting.PostCertificateDistributionEntry(certificateDemate, recordDetails, ActionType, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
        }


    }
}
