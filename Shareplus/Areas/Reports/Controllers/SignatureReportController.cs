using CDSMODULE.Helper;
using Entity.Common;
using Interface.Common;
using Interface.Reports;
using Interface.Security;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Repository.Reports;
using System;

namespace CDSMODULE.Areas.Reports.Controllers
{
    [Authorize]
    [Area("Reports")]
    [AutoValidateAntiforgeryToken]
    public class SignatureReportController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private IWebHostEnvironment _webHostEnvironment;
        private readonly IOptions<ReadConfig> _connectionString;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly ISignatureReport _signatureReport;
        private readonly IAudit _audit;
        public SignatureReportController(IOptions<ReadConfig> connectionString, IWebHostEnvironment webHostEnvironment,
             ILoggedinUser _loggedInUser, ICheckUserAccess checkUserAccess, ISignatureReport signatureReport, IAudit audit)
        {
            this._connectionString = connectionString;
            this._webHostEnvironment = webHostEnvironment;
            this._loggedInUser = _loggedInUser;
            this._checkUserAccess = checkUserAccess;
            this._signatureReport = signatureReport;
            _audit = audit;
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
        public JsonResponse GenerateReport(string CompCode, string SelectedAction, string DateFrom = null, string DateTo = null, string HolderFrom = null, string HolderTo = null)
        {
            JsonResponse response= new JsonResponse();
            var dataResponse= _signatureReport.GenerateReport(CompCode, _loggedInUser.GetUserNameToDisplay(), SelectedAction, _loggedInUser.GetUserIPAddress(), DateFrom, DateTo, HolderFrom, HolderTo);
            //if (SelectedAction=="E")
            //{
            //    GenericExcelReport report = new GenericExcelReport();
            //    response = report.GenerateExcelReport(response, "DuplicateCertificateList", "DCL", CompEnName, CompCode, "");

            //    if (response.IsSuccess)
            //        response.Message = CompCode + "_" + "DuplicateCertificateList" + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
            //}
            //else
            //{
            //    string[] reportTitles = { CompCode, CompEnName, type + Enum.GetName(Title.GetType(), Title) };

                
            //        response = _genericReport.GenerateReport(Title, response, reportTitles);
            //        if (response.IsSuccess)
            //        {
            //            response.Message = CompCode + "_" + type + Enum.GetName(Title.GetType(), Title) + "_" + DateTime.Now.ToString("yyyy_mm_dd") + ".pdf";
            //        }
                
            //}
            return response;
        }
    }
}
