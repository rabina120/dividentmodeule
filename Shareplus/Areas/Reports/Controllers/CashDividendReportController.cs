using CDSMODULE.Helper;

using Entity.Reports;
using Interface.Common;
using Interface.Reports;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Repository.Reports;
using System;
using System.Collections.Generic;
using static Entity.Reports.ATTGenericReport;
using JsonResponse = Entity.Common.JsonResponse;

namespace CDSMODULE.Areas.Reports.Controllers
{
    [Authorize]
    [Area("Reports")]
    [AutoValidateAntiforgeryToken]
    public class CashDividendReportController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly ICashDividendReport _cashDividendReport;
        private readonly IAudit _audit;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICheckUserAccess _checkUserAccess;

        private readonly IGenericReport _genericReport;
        private readonly ICommon _common;


        public CashDividendReportController(ILoggedinUser _loggedInUser, ICashDividendReport cashDividendReport, IWebHostEnvironment hostingEnvironment, ICheckUserAccess checkUserAccess, IAudit audit, IGenericReport genericReport, ICommon common)
        {
            this._loggedInUser = _loggedInUser;
            _cashDividendReport = cashDividendReport;
            _hostingEnvironment = hostingEnvironment;
            _checkUserAccess = checkUserAccess;
            _audit = audit;
            _genericReport = genericReport;
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
                    return View();
                return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }


        //For Generating Report
        [HttpPost]
        public JsonResponse GenerateDataForReport(string CompCode, string DivCode, string FiscalYr, string CompEnName, string SelectedReportType,string undoType, string seqNoFrom, string seqNoTo, string KittaFrom, string KittaTo, string DateFrom, string DateTo, string PaymentType, string Posted, string PaymentCenter, string BatchNo, bool WithBankDetails, string ShareType, string ExportFileType, string SelectedReportName,string FiscalYear,int? Occupation)
        {
            JsonResponse response = new JsonResponse();
            SelectedReportName = SelectedReportName.Replace(" ", string.Empty);

            

            if (!SelectedReportName.Contains("SummaryReportOfIssuedAndPaid"))
            {
                if (!SelectedReportName.Contains("UndoReport"))
                {
                    SelectedReportName = SelectedReportName + (Posted == "Posted" ? "Posted" : "Unposted");
                }           
            }

            response = _audit.auditSave(_loggedInUser.GetUserNameToDisplay(), "Report Generated of : " + SelectedReportName, this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress());
            if (response.IsSuccess)
            {
                response = _cashDividendReport.GenerateDataForReport(CompCode, DivCode, SelectedReportType,undoType, seqNoFrom, seqNoTo, KittaFrom, KittaTo, DateFrom, DateTo, PaymentType, Posted, PaymentCenter, BatchNo, WithBankDetails, ShareType,Occupation, ExportFileType, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress(), SelectedReportName);
                if (!response.HasError)
                {
                    if (response.IsSuccess)
                    {
                        String title = "";
                        switch (SelectedReportType)
                        {
                            case "LIDW":
                                title = "Issued";
                                break;
                            case "LUDW":
                                title = "Unisued";
                                break;
                            case "LPDW":
                                title = "Paid";
                                break;
                            case "LUPDW":
                                title = "Unpaid";
                                break;
                            case "LIUDW":
                                title = "Issued and Unpaid";
                                break;
                            case "SRIP":
                                title = "Summary";
                                break;
                            case "UNDO":
                                title = "Undo";
                                Posted = "";
                                break;
                            default:
                                // code block
                                break;
                        }

                        string holderType = "";
                        if (ShareType == "P")
                        {
                            holderType = "Physical";
                        }
                        else
                        {
                            holderType = "Demat";
                        }
                        if (ExportFileType == "E") //excel
                        {
                            GenericExcelReport report = new GenericExcelReport();
                            response = report.GenerateExcelReport(response,  "List of "+ Posted + " " + title+" Report for " + holderType+" " +  " FY- " + FiscalYear, SelectedReportType, CompEnName, CompCode, ShareType);

                            if (response.IsSuccess)
                                response.Message = CompCode + "_" + SelectedReportName + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
                        }
                        else //pdf
                        {
                            response.withBankDetails = WithBankDetails;
                            string[] reportTitles = { CompCode, CompEnName,"List of "+ Posted + " " + title+" Report for " + holderType+" " +  " FY- " + FiscalYear };
                            List<ATTShareHolderReportTotalBasedOn> reportTotals = new List<ATTShareHolderReportTotalBasedOn>();
                            reportTotals.Add(new ATTShareHolderReportTotalBasedOn()
                            {
                                TotalBasedOn = "totshkitta"

                            });
                            reportTotals.Add(new ATTShareHolderReportTotalBasedOn()
                            {
                                TotalBasedOn = "netamt"

                            });
                            response = _genericReport.GenerateReport((ReportName)Enum.Parse(typeof(ReportName), SelectedReportName), response, reportTitles,false,true, reportTotals);
                            if (response.IsSuccess)
                            {
                                response.ResponseData = _common.SaveGetPdfReport(response.ResponseData);
                                response.Message = CompCode + "_" + SelectedReportName + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".pdf";
                            }

                        }
                    }
                }
                else
                {
                    response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
                    response.IsSuccess = false;
                }

            }

            return response;
        }



    }
}
