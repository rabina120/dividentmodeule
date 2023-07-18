using CDSMODULE.Helper;
using Entity.Common;
using Entity.ShareHolder;
using Interface.Certificate;
using Interface.Common;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;



namespace CDSMODULE.Areas.Certificate.Controllers
{
    [Area("Certificate")]
    [AutoValidateAntiforgeryToken]
    public class CertificateDistributionEntryController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;

        private readonly ICERTIFICATEDISTRIBUTIONENTRY _distributionEntry;
        private readonly ICERTIFICATE _CERTIFICATE;
        private readonly ICheckUserAccess _checkUserAccess;

        public CertificateDistributionEntryController(ICheckUserAccess checkUserAccess, ILoggedinUser _loggedInUser, ICERTIFICATEDISTRIBUTIONENTRY distributionEntry, ICERTIFICATE CERTIFICATE)
        {
            this._loggedInUser = _loggedInUser;
            _checkUserAccess = checkUserAccess;
            _CERTIFICATE = CERTIFICATE;

            _distributionEntry = distributionEntry;


        }

        public IActionResult Index()
        {

            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();
            if (_checkUserAccess.CheckIfAccessible(_loggedInUser.GetUserId(), this.ControllerContext))
                return View();
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });

        }


        [HttpPost]
        public JsonResponse GetSholderInformation(string CompCode, string ShholderNo)
        {
            return _distributionEntry.GET_SHHOLDER_DISTRIBUTE(CompCode, ShholderNo, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());

        }




        [HttpPost]
        public JsonResponse GET_SHHOLDER_CERTDISTRIBUTE(string compcode, int ShholderNo, string SelectedAction)
        {
            return _distributionEntry.GET_SHHOLDER_CERTDISTRIBUTE(compcode, ShholderNo, SelectedAction);
        }


        [HttpPost]
        public JsonResponse SaveDistributionCertificate(List<ATTCertDet> CertificateList, string CompCode, string certno, string SelectedAction, string DistDate)
        {
            return _distributionEntry.SaveDistributionCertificate(CertificateList, CompCode, certno, SelectedAction, DistDate, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
        }
    }

}
