using CDSMODULE.Helper;
using Entity.Common;
using Entity.Reports;
using Interface.Common;
using Interface.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;


namespace CDSMODULE.Areas.Reports.Controllers
{
    [Authorize]
    [Area("Reports")]
    [AutoValidateAntiforgeryToken]
    public class ConsolidateReportController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IConsolidateReport _consolidateReport;
        private readonly ICertificateReports _certificateReports;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICheckUserAccess _checkUserAccess;


        public ConsolidateReportController(ILoggedinUser _loggedInUser, IWebHostEnvironment hostingEnvironment,
            IConsolidateReport consolidateReport, ICheckUserAccess checkUserAccess, ICertificateReports certificateReports)
        {
            this._loggedInUser = _loggedInUser;
            _hostingEnvironment = hostingEnvironment;
            _consolidateReport = consolidateReport;
            this._checkUserAccess = checkUserAccess;
            _certificateReports = certificateReports;

        }

        public IActionResult Index()
        {
            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();

            if (_checkUserAccess.CheckIfAccessible(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext))
                return View();
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });

        }
        [HttpPost]
        public JsonResponse GenerateReport(string ReportData1, string ExportReportType)
        {
            JsonResponse jsonResponse = new JsonResponse();
            JsonResponse jsonResponseToReturn = new JsonResponse();
            ATTConsolidateReport ReportData = JsonSerializer.Deserialize<ATTConsolidateReport>(ReportData1);//(ReportData1, typeof(ATTConsolidateReport)) as ATTConsolidateReport;
            jsonResponse = _consolidateReport.GenerateReport(ReportData, ExportReportType, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
            List<ATTConsolidateReport> ConsolidateList = (List<ATTConsolidateReport>)jsonResponse.ResponseData;

            if (jsonResponse.IsSuccess)
            {
                _certificateReports.CertificateConsolidateReport(ReportData, ConsolidateList, _hostingEnvironment.WebRootPath);
            }

            else
            {
                jsonResponseToReturn = jsonResponse;
            }



            return jsonResponseToReturn;
        }

    }
}