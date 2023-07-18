

using CDSMODULE.Helper;
using Entity.Certificate;
using Entity.Common;
using Interface.Certificate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CDSMODULE.Areas.Certificate.Controllers
{
    [Authorize]
    [Area("Certificate")]
    [AutoValidateAntiforgeryToken]
    public class CertificateConsolidatePosting : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly ICertificateConsolidatePosting certificateConsolidatePosting;

        public CertificateConsolidatePosting(ILoggedinUser _loggedInUser, ICertificateConsolidatePosting certificateConsolidatePosting)
        {
            this._loggedInUser = _loggedInUser;
            this.certificateConsolidatePosting = certificateConsolidatePosting;

        }

        public IActionResult Index()
        {

            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();
            return View();
        }


        [HttpPost]
        public JsonResponse GetCertificateConsolidateCompanyData(string CompCode)
        {
            return certificateConsolidatePosting.GetCertificateConsolidateCompanyData(CompCode, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
        }

        [HttpPost]
        public JsonResponse GetCertificate(string CompCode, string SplitNo, string ShholderNo)
        {
            return certificateConsolidatePosting.GetCertificate(CompCode, SplitNo, ShholderNo, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
        }
        [HttpPost]
        public JsonResponse PostCertificateConsolidateEntry(List<ATTCertificateConsolidatePosting> aTTCertificateConsolidatePostings, ATTCertificateConsolidatePosting recorddetails, string SelectedAction)
        {
            return certificateConsolidatePosting.PostCertificateConsolidateEntry(aTTCertificateConsolidatePostings, recorddetails, SelectedAction, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
        }

    }
}
