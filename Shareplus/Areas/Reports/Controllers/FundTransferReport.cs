using CDSMODULE.Helper;
using Entity.Common;
using Entity.Esewa;
using Entity.Reports;
using Interface.Common;
using Interface.Reports;
using Interface.Security;
using INTERFACE.FundTransfer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Repository.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDSMODULE.Areas.Reports.Controllers
{

    [Authorize]
    [Area("Reports")]
    [AutoValidateAntiforgeryToken]
    public class FundTransferReport : Controller
    {

        private readonly ILoggedinUser _loggedInUser;
        private readonly ILogDetails _logDetails;
        private readonly IAudit _audit;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IEsewaStatusReport _esewaReports;
        private readonly IGenericReport _genericReport;
        private readonly IEService _eService;


        public FundTransferReport(ILoggedinUser _loggedInUser, ILogDetails logDetails,
            IAudit audit, ICheckUserAccess checkUserAccess, IWebHostEnvironment hostingEnvironment, IEsewaStatusReport esewaReports
            , IGenericReport genericReport, IEService eService)
        {
            this._loggedInUser = _loggedInUser;
            _logDetails = logDetails;
            _audit = audit;
            _checkUserAccess = checkUserAccess;
            _hostingEnvironment = hostingEnvironment;
            _esewaReports = esewaReports;
            _genericReport = genericReport;
            _eService = eService;
        }
        public IActionResult Index()
        {
            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();
            ViewBag.UserRole = _loggedInUser.GetUserType();
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
        public JsonResponse GetAllDividends(string CompCode)
        {
            JsonResponse response = _esewaReports.GetAllDividends(CompCode);

            if (response.HasError)
                _audit.errorSave(_loggedInUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;

        }
        [HttpPost]
        public JsonResponse GetAllBatch(string CompCode,string DivCode)
        {
            JsonResponse response = _esewaReports.GetAllBatch(CompCode,DivCode);

            if (response.HasError)
                _audit.errorSave(_loggedInUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;

        }
        [HttpPost]
        public JsonResponse GenerateReport(string CompCode,string CompEnName,string FiscalYear, string DivCode,string Batch,string ReportType,string ReportSubType,string exportTo)
        {
            JsonResponse response = _esewaReports.GenerateReportData(CompCode, DivCode,Batch,ReportType,ReportSubType, exportTo, _loggedInUser.GetUserNameToDisplay(),_loggedInUser.GetUserIPAddress());
            
            //JsonResponse res=_genericReport.GenerateReport()

            if (response.HasError)
            {

                _audit.errorSave(_loggedInUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
                return response;

            }
            else if(response.IsSuccess)
            {
                

                    var report = GetReportName(ReportType, ReportSubType);
                    string reportName = report.ToString() + "Report";
                    string title = reportName;
                    if (!(FiscalYear == null)) title += "|FY- " + FiscalYear;
                    if (!(Batch == null)) title += "|Batch: " + Batch;
                    string[] reportTitles = { CompCode, CompEnName, title };
                if (exportTo == "P")
                {

                    List<ATTShareHolderReportTotalBasedOn> reportTotals = new List<ATTShareHolderReportTotalBasedOn>();

                    reportTotals.Add(new ATTShareHolderReportTotalBasedOn()
                    {
                        TotalBasedOn = "totalamt"

                    });
                    var response1 = _genericReport.GenerateReport(report, response, reportTitles, false, false, reportTotals);
                    if (response1.IsSuccess)
                    {
                        response1.Message = CompEnName + "_" + title + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".pdf";
                        return response1;
                    }
                }
                else
                {
                    GenericExcelReport excelReport = new GenericExcelReport();
                     var Excelresponse = excelReport.GenerateExcelReport(response, reportName+"_Batch_"+Batch+"FY_"+FiscalYear, "", CompEnName, CompCode, "");

                    if (Excelresponse.IsSuccess)
                    {

                        Excelresponse.Message = CompEnName + "_" + title + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
                    }
                    return Excelresponse;
                }
                
            }
            return response;

        }
        [HttpPost]
        public async Task<IActionResult> ViewData(string CompCode, string DivCode, string BatchID, string ReportType, string ReportSubType)
        {
            var request = new ATTDataTableRequest();

            request.Draw = Convert.ToInt32(Request.Form["draw"].FirstOrDefault());
            request.Start = Convert.ToInt32(Request.Form["start"].FirstOrDefault());
            request.Length = Convert.ToInt32(Request.Form["length"].FirstOrDefault());
            request.Search = new ATTDataTableSearch()
            {
                Value = Request.Form["search[value]"].FirstOrDefault()
            };
            request.Order = new ATTDataTableOrder[] {
                new ATTDataTableOrder()
                {
                    Dir = Request.Form["order[0][dir]"].FirstOrDefault(),
                    Column = Convert.ToInt32(Request.Form["order[0][column]"].FirstOrDefault()),
                    ColumnName =Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault(),

                    }
                };
            ATTDataTableResponse<ATTBatchProcessing> returnData = new ATTDataTableResponse<ATTBatchProcessing>();

            returnData = await _eService.GetBatchReportAsync(request, CompCode, DivCode, BatchID, ReportType,ReportSubType);
            var jsonData = new { draw = returnData.Draw, recordsFiltered = returnData.RecordsFiltered, recordsTotal = returnData.RecordsTotal, data = returnData.Data };
            return Ok(jsonData);
        }
        private static ATTGenericReport.ReportName GetReportName(string ReportType,string ReportSubType)
        {
            var reportName = new ATTGenericReport.ReportName();
            switch (ReportType)
            {
                case "AV":
                    {

                        switch (ReportSubType)
                        {
                            case "V":
                                reportName= ATTGenericReport.ReportName.ValidAccount;
                                break;
                            case "NV":
                                reportName= ATTGenericReport.ReportName.InValidAccount;
                                break;
                        }
                        break;
                    }
                case "TP":
                    {

                        switch (ReportSubType)
                        {
                            case "P":
                                reportName = ATTGenericReport.ReportName.TransactionProcessed;
                                break;
                            case "F":
                                reportName = ATTGenericReport.ReportName.TransactionFailedToProcess;
                                break;
                        }
                        break;
                    }
                case "TS":
                    {

                        switch (ReportSubType)
                        {
                            case "P":
                                reportName = ATTGenericReport.ReportName.TransactionStatusProcessing;
                                break;
                            case "S":
                                reportName = ATTGenericReport.ReportName.TransactionStatusSuccess;
                                break;
                            case "F":
                                reportName = ATTGenericReport.ReportName.TransactionStatusFailed;
                                break;
                        }
                        break;
                    }
                default:
                     reportName= new ATTGenericReport.ReportName();
                    break;
            }
            return reportName;
        }
    }
}
