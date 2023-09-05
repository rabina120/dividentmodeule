using CDSMODULE.Helper;
using Entity.Reports;
using Interface.Common;
using Interface.Reports;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CDSMODULE.Areas.Reports.Controllers
{
    [Area("Reports")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class DailyReportController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IGenericReport _genericReport;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IDailyReport _dailyReport;
        private readonly IAudit _audit;
        private readonly ICommon _common;

        public DailyReportController(ILoggedinUser _loggedInUser, IAudit audit,
           IWebHostEnvironment hostingEnvironment, IGenericReport genericReport, ICheckUserAccess checkUserAccess, IDailyReport dailyReport, ICommon common)
        {
            this._loggedInUser = _loggedInUser;
            _hostingEnvironment = hostingEnvironment;
            _genericReport = genericReport;
            _checkUserAccess = checkUserAccess;
            _dailyReport = dailyReport;
            _audit = audit;
            _common = common;
        }

        public IActionResult Index()
        {
            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();
            string UserId = _loggedInUser.GetUserId();
            Entity.Common.JsonResponse res = _audit.auditSave(_loggedInUser.GetUserNameToDisplay(), "Opened " + this.ControllerContext.RouteData.Values["controller"].ToString() + " Form", this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress());
            if (res.IsSuccess)
            {
                if (_checkUserAccess.CheckIfAccessible(UserId, this.ControllerContext))
                    return View();
                return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });

        }
        [HttpPost]
        public Entity.Common.JsonResponse GenerateReport(string DailyReportDate, string CompCode)
        {
            Entity.Common.JsonResponse response = new Entity.Common.JsonResponse();
            string[] reportTitles = { "001", "Standard Chartered Bank", "' " + DailyReportDate + " ' Audit Report" };
            response = _dailyReport.GenerateReport(_loggedInUser.GetUserNameToDisplay(), CompCode, DailyReportDate, _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            else
            {
                response = _genericReport.GenerateReport(ATTGenericReport.ReportName.DailyReport, response, reportTitles);
                if (response.IsSuccess)
                {
                    response.ResponseData = _common.SaveGetPdfReport(response.ResponseData);
                    response.Message = "AuditReport-" + DailyReportDate + ".pdf";
                }
                    
            }

            return response;
        }
    }
}
