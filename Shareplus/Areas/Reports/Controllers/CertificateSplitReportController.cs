using CDSMODULE.Helper;
using Entity.Common;
using Entity.Reports;

using Interface.Common;
using Interface.Reports;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CDSMODULE.Areas.Reports.Controllers
{
    [Authorize]
    [Area("Reports")]
    [AutoValidateAntiforgeryToken]
    public class CertificateSplitReportController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly ICertificateSplitReport _certificateSplitReport;
        private readonly ICertificateReports _certificateReport;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        private readonly ICommon _common;
        public CertificateSplitReportController(ILoggedinUser _loggedInUser, IWebHostEnvironment hostingEnvironment
            , ICertificateSplitReport certificateSplitReport, ICheckUserAccess checkUserAccess, IAudit audit, ICertificateReports certificateReports, ICommon common)
        {

            this._loggedInUser = _loggedInUser;
            _certificateSplitReport = certificateSplitReport;
            _hostingEnvironment = hostingEnvironment;
            this._checkUserAccess = checkUserAccess;
            _audit = audit;
            _certificateReport = certificateReports;
            _common = common;
        }
        public IActionResult Index()
        {

            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();
            string UserId = _loggedInUser.GetUserId(); 
            JsonResponse res = _audit.auditSave(_loggedInUser.GetUserNameToDisplay(), "Opened " + this.ControllerContext.RouteData.Values["controller"].ToString() + " Form", this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress());
            if (res.IsSuccess)
            {
                if (_checkUserAccess.CheckIfAccessible(UserId, this.ControllerContext))
                {
                    return View();
                }
                return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }

        [HttpPost]

        public JsonResponse GenerateReport(string ReportData1, string ExportReportType, string FromSystem)
        {
            JsonResponse jsonResponse = new JsonResponse();
            JsonResponse jsonResponseToReturn = new JsonResponse();
            ATTCERTIFICATEREPORT ReportData = JsonSerializer.Deserialize<ATTCERTIFICATEREPORT>(ReportData1);//(ReportData1, typeof(ATTCERTIFICATEREPORT)) as ATTCERTIFICATEREPORT;
            jsonResponse = _certificateSplitReport.GenerateReport(ReportData, ExportReportType, FromSystem, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());

            if (jsonResponse.IsSuccess)
            {
                jsonResponseToReturn = _certificateReport.CertificateSplitReport(ReportData,jsonResponse, _hostingEnvironment.WebRootPath);
                
                jsonResponseToReturn.ResponseData = _common.SaveGetPdfReport(jsonResponseToReturn.ResponseData);
            }

            else
            {

                jsonResponseToReturn = jsonResponse;
            }

            return jsonResponseToReturn;
        }




    }
}
