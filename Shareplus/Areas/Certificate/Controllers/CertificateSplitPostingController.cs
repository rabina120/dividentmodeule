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
    public class CertificateSplitPostingController : Controller
    {


        private readonly ILoggedinUser _loggedInUser;
        private readonly ICertificateSplitPosting certificateSplitPosting;
        private readonly ICheckUserAccess _checkUserAccess;

        public CertificateSplitPostingController(ILoggedinUser _loggedInUser, ICertificateSplitPosting certificateSplitPosting,ICheckUserAccess checkUserAccess)
        {
            this._loggedInUser = _loggedInUser;
            this.certificateSplitPosting = certificateSplitPosting;
            this._checkUserAccess = checkUserAccess;

        }
        public IActionResult Index()
        {

            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();
            if (_checkUserAccess.CheckIfAccessible(_loggedInUser.GetUserId(), this.ControllerContext))
                return View();
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }


        [HttpPost]
        public JsonResponse GetCertificateSplitCompanyData(string CompCode)
        {
            return certificateSplitPosting.GetCertificateSplitCompanyData(CompCode);
        }

        [HttpPost]
        public JsonResponse PostCertificateSplitEntry(List<ATTCERTIFICATE> certificateDemate, ATTCERTIFICATE recordDetails, string ActionType)
        {
            return certificateSplitPosting.PostCertificateSplitEntry(certificateDemate, recordDetails, ActionType, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
        }




    }
}
