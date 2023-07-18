using CDSMODULE.Helper;
using Entity.Certificate;
using Entity.Common;
using Interface.Certificate;
using Interface.Common;
using Interface.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CDSMODULE.Areas.Certificate.Controllers
{
    [Authorize]
    [Area("Certificate")]
    [AutoValidateAntiforgeryToken]
    public class CertificateSplitController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly ICertificateSplit _certificateSplit;
        private readonly ICertificateReports _certificateReports;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICheckUserAccess _checkUserAccess;
        public CertificateSplitController(ILoggedinUser _loggedInUser, ICertificateSplit certificateSplit,
            IWebHostEnvironment hostingEnvironment, ICheckUserAccess checkUserAccess, ICertificateReports certificateReports)
        {
            this._loggedInUser = _loggedInUser;
            this._certificateSplit = certificateSplit;
            this._hostingEnvironment = hostingEnvironment;
            this._checkUserAccess = checkUserAccess;
            this._certificateReports = certificateReports;
        }

        public IActionResult Index()
        {

            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();
            if (_checkUserAccess.CheckIfAccessible(_loggedInUser.GetUserId(), this.ControllerContext))
                return View();
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }
        [HttpPost]
        public JsonResponse GetCertificateDetailsByCertificateNo(string CompCode, string CertificateNo, string ActionType)
        {
            return _certificateSplit.GetCertificateDetailsByCertificateNo(CompCode, CertificateNo, ActionType, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
        }

        [HttpPost]
        public JsonResponse CheckCertificateNo(string CompCode, int? CertificateNo)
        {
            return _certificateSplit.CheckCertificateNo(CompCode, CertificateNo);
        }

        [HttpPost]
        public JsonResponse SaveCertificateSplit(string CompCode, string CertificateNo, string srnofrom, string srnoto, List<ATTCertificateSplit> aTTCertificates, string shholderno, string Splitdate, int shownertype, int sharetype, string remarks, string SelectedAction, string PageNo, string SplitNo)
        {
            string UserName = _loggedInUser.GetUserNameToDisplay();
            return _certificateSplit.SaveCertificateSplit(CompCode, CertificateNo, srnofrom, srnoto, aTTCertificates, shholderno, Splitdate, shownertype, sharetype, remarks, SelectedAction, PageNo, SplitNo, UserName, _loggedInUser.GetUserIPAddress());
        }

        [HttpPost]
        public JsonResponse CreateReport(string CompCode, string CertificateNo)
        {
            JsonResponse response = new JsonResponse();
            JsonResponse returnedResponse = new JsonResponse();

            response = _certificateSplit.CreateReport(CompCode, CertificateNo, _loggedInUser.GetUserNameToDisplay());



            if (response.IsSuccess)
            {
                returnedResponse = _certificateReports.CertificateSplitReportForSingle((List<ATTCertificateSplit>)response.ResponseData, _hostingEnvironment.WebRootPath);
            }

            else
            {
                returnedResponse = response;
            }
            return returnedResponse;


        }



    }
}
