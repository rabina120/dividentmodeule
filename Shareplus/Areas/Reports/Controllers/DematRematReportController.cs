using CDSMODULE.Helper;
using Entity.Common;
using Entity.Reports;
using Interface.Common;
using Interface.Parameter;
using Interface.Reports;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Repository.Reports;
using System;


namespace CDSMODULE.Areas.Reports.Controllers
{
    [Authorize]
    [Area("Reports")]
    [AutoValidateAntiforgeryToken]
    public class DematRematReportController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IDPSetup _dPSetup;
        private readonly IDemateRemateReport _demateRemateReport;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAudit _audit;
        private readonly IGenericReport _genericReport;


        public DematRematReportController(ILoggedinUser _loggedInUser, IDemateRemateReport demateRemateReport, ICheckUserAccess checkUserAccess,
            IDPSetup dPSetup, IWebHostEnvironment webHostEnvironment, IAudit audit, IGenericReport genericReport)
        {
            this._loggedInUser = _loggedInUser;
            this._checkUserAccess = checkUserAccess;
            this._dPSetup = dPSetup;
            this._demateRemateReport = demateRemateReport;
            this._webHostEnvironment = webHostEnvironment;
            _audit = audit;
            _genericReport = genericReport;
        }
        public IActionResult Index()
        {
            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();
            string UserId = _loggedInUser.GetUserId(); 
            JsonResponse res = _audit.auditSave(_loggedInUser.GetUserNameToDisplay(), "Opened " + this.ControllerContext.RouteData.Values["controller"].ToString() + " Form", this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress());
            if (res.IsSuccess)
            {
                if (_checkUserAccess.CheckIfAccessible(UserId, this.ControllerContext))
                    return View();
                return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }
        [HttpPost]
        public JsonResponse GETDP()
        {
            return _dPSetup.LoadDPDetailList();
        }
        [HttpPost]
        public JsonResponse GetAllParaCompChild(string CompCode)
        {
            JsonResponse response = _demateRemateReport.GetAllParaCompChild(CompCode);
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [HttpPost]
        public JsonResponse GetDataFromCertificateDetail(string CompCode)
        {
            JsonResponse response = _demateRemateReport.GetDataFromCertificateDetail(CompCode);
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse GenerateReportExcel(ATTReportTypeForDemateRemate ReportData, string ExportReportType)
        {
            JsonResponse jsonResponse = new JsonResponse();


            return jsonResponse;
        }


        [HttpPost]
        public JsonResponse GenerateReport(ATTReportTypeForDemateRemate ReportData, string ExportReportType)
        {
            JsonResponse jsonResponse = new JsonResponse();

            if (ExportReportType == "R") //for pdf report
            {
                string reportType,secondaryReportType,status="";
                if(ReportData.DataType == "P")
                {
                    status = "POSTED";
                }
                else if (ReportData.DataType == "U")
                {
                    status = "UNPOSTED";
                }
                else if (ReportData.DataType == "R")
                {
                    status = "REJECTED";
                }
                var Title = ATTGenericReport.ReportName.DematDetailReport;
                if (ReportData.ReportType == "D")
                {
                    reportType = "Demat";
                    if (ReportData.SecondaryReportType == "D")
                    {
                        secondaryReportType = "Detail";
                        Title = ATTGenericReport.ReportName.DematDetailReport;
                    }
                    else if (ReportData.SecondaryReportType == "S")
                    {
                        secondaryReportType = "Summary";
                        Title = ATTGenericReport.ReportName.DematSummaryReport;
                    }
                    else
                    {
                        secondaryReportType = "DRN";
                        Title = ATTGenericReport.ReportName.DematDrnReport;
                    }
                }
                else
                {
                    reportType = "Remat";
                    if (ReportData.SecondaryReportType == "D")
                    {
                        secondaryReportType = "Detail";
                        Title = ATTGenericReport.ReportName.RematDetailReport;
                    }
                    else if (ReportData.SecondaryReportType == "S")
                    {
                        secondaryReportType = "Summary";
                        Title = ATTGenericReport.ReportName.RematSummaryReport;
                    }
                    else
                    {
                        secondaryReportType = "DRN";
                        Title = ATTGenericReport.ReportName.RematDrnReport;
                    }
                }

                string[] reportTitles = { ReportData.CompCode, ReportData.CompEnName, Enum.GetName(Title.GetType(), Title  ) + " : " + status};

                jsonResponse = _demateRemateReport.GenerateReport(ReportData, ExportReportType, _loggedInUser.GetUserNameToDisplay());
                if (jsonResponse.HasError)
                    jsonResponse = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)jsonResponse.ResponseData);
                else
                {
                    jsonResponse = _genericReport.GenerateReport(Title , jsonResponse, reportTitles);
                    if (jsonResponse.IsSuccess)
                        jsonResponse.Message =  Enum.GetName(Title.GetType(), Title) + DateTime.Now.ToString("yyyy_mm_dd") + ".pdf";
                }
            }
            else //for excel report
            {
                jsonResponse = _demateRemateReport.GenerateReportExcel(ReportData, ExportReportType, _loggedInUser.GetUserNameToDisplay());
                if (jsonResponse.HasError)
                    jsonResponse = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)jsonResponse.ResponseData);
                else
                {


                    GenericExcelReport _excelReport = new GenericExcelReport();
                    var Title = "Report-for";
                    string pslStatus = string.Empty;
                    string type = string.Empty;
                    string dematType = string.Empty;
                    if (ReportData.DataType == "P") pslStatus = "-Posted";
                    if (ReportData.DataType == "U") pslStatus = "-Unposted";
                    if (ReportData.DataType == "R") pslStatus = "-Rejected";


                    if (ReportData.ReportType == "Rev") type = "_Reversal";
                    if (ReportData.ReportType == "D")
                    {
                        if (ReportData.DemateType == "DR") type = "_Demate_DR ";
                        else type = "_Demat_CR ";
                    }
                    else type = "_Demat ";
                    string reportOption = string.Empty;
                    if (ReportData.SecondaryReportType == "D") reportOption = "_Detail-List ";
                    if (ReportData.SecondaryReportType == "S") reportOption = "_Summary-List ";
                    if (ReportData.SecondaryReportType == "DN") reportOption = "_DRN-No ";

                    jsonResponse = _excelReport.GenerateExcelReport(jsonResponse, Title + type + reportOption + pslStatus , pslStatus, ReportData.CompEnName, ReportData.CompCode, null);
                    if (jsonResponse.IsSuccess)
                        jsonResponse.Message = ReportData.CompCode + "_" + Title + type + reportOption + pslStatus + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
                }
                
            }
            return jsonResponse;
        }
    }
}
