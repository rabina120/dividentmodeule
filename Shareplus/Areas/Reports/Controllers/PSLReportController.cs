using CDSMODULE.Helper;
using Entity.Reports;
using Interface.Common;
using Interface.Parameter;
using Interface.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Repository.Reports;
using System;
using JsonResponse = Entity.Common.JsonResponse;

namespace CDSMODULE.Areas.Reports.Controllers
{
    [Authorize]
    [Area("Reports")]
    [AutoValidateAntiforgeryToken]
    public class PSLReportController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IPSLReport _pSLReport;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IDPSetup _dPSetup;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IGenericReport _genericReport;
        private readonly ICommon _common;

        public PSLReportController(ILoggedinUser _loggedInUser, IPSLReport pSLReport, IGenericReport genericReport, ICheckUserAccess checkUserAccess, IDPSetup dPSetup, IWebHostEnvironment webHostEnvironment, ICommon common)
        {

            this._loggedInUser = _loggedInUser;
            this._pSLReport = pSLReport;
            this._dPSetup = dPSetup;
            this._webHostEnvironment = webHostEnvironment;
            this._checkUserAccess = checkUserAccess;
            _genericReport = genericReport;
            _common = common;
        }
        public IActionResult Index()
        {
            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();

            if (_checkUserAccess.CheckIfAccessible(_loggedInUser.GetUserId(), this.ControllerContext))
                return View();
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }
        [HttpPost]
        public JsonResponse GetAllPledgeAt()
        {
            return _pSLReport.GetAllPledgeAt();
        }
        [HttpPost]
        public JsonResponse GenerateReport(string CompCode, string CompEnName, string PCode, string TranType, string PSLDateFrom, string PSLDateTo, string HolderNoFrom, string HolderNoTo, string CertNoFrom, string CertNoTo, string AppStatus, string OrderBy, string ShareType, string ReportType)
        {
            JsonResponse response = new JsonResponse();
            JsonResponse returnedResponse = new JsonResponse();

            var UserName = _loggedInUser.GetUserNameToDisplay();
            var IpAddress = _loggedInUser.GetUserIPAddress();
            var EntryDateTime = DateTime.Now.ToString();

            response = _pSLReport.GenerateReport(CompCode, PCode, TranType, PSLDateFrom, PSLDateTo, HolderNoFrom, HolderNoTo, CertNoFrom, CertNoTo, AppStatus, OrderBy, ShareType, ReportType, UserName, IpAddress, EntryDateTime);

            GenericExcelReport _excelReport = new GenericExcelReport();
            var Title = "PSL Report For ";
            string type = string.Empty;
            string appStatus = string.Empty ;
            if (ReportType == "U") type = "UnClearPSL";
            if (ReportType == "C") type = "ClearPSL";
            if (AppStatus == "P") appStatus = "Posted";
            if (AppStatus == "U") appStatus = "Unposted";
            if (AppStatus == "R") appStatus = "Rejected";
            
            if (response.HasError)
            {
                //_audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            else
            {
                response = _excelReport.GenerateExcelReport(response, Title + type + " - " + appStatus, appStatus, CompEnName, CompCode, null);
                if (response.IsSuccess)
                    response.Message = CompCode + "_" + Title + type + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
            }

            return response;
        }
        public JsonResponse GenerateReoprtPdf(string CompCode, string CompEnName, string PCode, string TranType, string PSLDateFrom, string PSLDateTo, string HolderNoFrom, string HolderNoTo, string CertNoFrom, string CertNoTo, string AppStatus, string OrderBy, string ShareType, string ReportType)
        {
            JsonResponse response = new JsonResponse();

            
            string type = string.Empty;
            string appStatus = string.Empty;
            if (ReportType == "U") type = "";
            if (ReportType == "C") type = "";
            if (AppStatus == "P") appStatus = "Posted";
            if (AppStatus == "U") appStatus = "Unposted";
            if (AppStatus == "R") appStatus = "Rejected";

            

            var UserName = _loggedInUser.GetUserNameToDisplay();
            var IpAddress = _loggedInUser.GetUserIPAddress();
            var EntryDateTime = DateTime.Now.ToString();

            response = _pSLReport.GenerateReportPdf(CompCode, PCode, TranType, PSLDateFrom, PSLDateTo, HolderNoFrom, HolderNoTo, CertNoFrom, CertNoTo, AppStatus, OrderBy, ShareType, ReportType, UserName, IpAddress, EntryDateTime);

            if (ReportType == "U")
            {
                var Title = ATTGenericReport.ReportName.PSLReportUnclearPosted;
                if (appStatus == "Posted")
                {
                    Title = ATTGenericReport.ReportName.PSLReportUnclearPosted;
                }
                else if (appStatus == "Unposted")
                {
                    Title = ATTGenericReport.ReportName.PSLReportUnclearUnposted;
                    
                }
                else
                {
                    Title = ATTGenericReport.ReportName.PSLReportUnclearRejected;
                }
               
                string[] reportTitles = { CompCode, CompEnName, type + Enum.GetName(Title.GetType(), Title) };
                if (response.HasError)
                {
                    //_audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
                    response.IsSuccess = false;
                    response.Message = "Failed";
                }
                else
                {
                    response = _genericReport.GenerateReport(Title, response, reportTitles);
                    if (response.IsSuccess)
                        response.Message = CompCode + "_" + type + Enum.GetName(Title.GetType(), Title) + "_" + DateTime.Now.ToString("yyyy_mm_dd") + ".pdf";
                }

            }
            if (ReportType == "C")
            {
                var Title = ATTGenericReport.ReportName.PSLReportClearPosted;
                if (appStatus == "Posted")
                {
                    Title = ATTGenericReport.ReportName.PSLReportClearPosted;
                }
                else if (appStatus == "Unposted")
                {
                    Title = ATTGenericReport.ReportName.PSLReportClearUnposted;
                }
                else
                {
                    Title = ATTGenericReport.ReportName.PSLReportClearRejected;
                }
                
                string[] reportTitles = { CompCode, CompEnName, type + Enum.GetName(Title.GetType(), Title) };

                if (response.HasError)
                {
                    //_audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
                    response.IsSuccess = false;
                    response.Message = "Failed";
                }
                else
                {
                    response = _genericReport.GenerateReport(Title, response, reportTitles);
                    if (response.IsSuccess)
                    {
                        response.Message = CompCode + "_" + type + Enum.GetName(Title.GetType(), Title) + "_" + DateTime.Now.ToString("yyyy_mm_dd") + ".pdf";
                    }
                }
            }
            response.ResponseData = _common.SaveGetPdfReport(response.ResponseData);

            return response;
        }
    }
}
