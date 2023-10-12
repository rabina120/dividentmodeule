using CDSMODULE.Helper;
using Entity.Common;
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
using JsonResponse = Entity.Common.JsonResponse;

namespace CDSMODULE.Areas.Reports.Controllers
{
    [Area("Reports")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class ShareHolderReportController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IGenericReport _genericReport;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        private readonly ICommon _common;
        private readonly IShareHolderReport _shareHolderReport;

        public ShareHolderReportController(ILoggedinUser _loggedInUser, IAudit audit,
          IWebHostEnvironment hostingEnvironment, IGenericReport genericReport, ICheckUserAccess checkUserAccess, ICommon common, IShareHolderReport shareHolderReport)
        {
            this._loggedInUser = _loggedInUser;
            _hostingEnvironment = hostingEnvironment;
            _genericReport = genericReport;
            _checkUserAccess = checkUserAccess;
            _audit = audit;
            _common = common;
            _shareHolderReport = shareHolderReport;
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
        public JsonResponse GetAllDistrict()
        {
            JsonResponse response = _common.GetDistricts();

            if (response.HasError)
                _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [HttpPost]
        public JsonResponse GetAllOccupation()
        {
            JsonResponse response = _common.GetOccupations();
            if (response.HasError)
                _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]

        public JsonResponse GetAllShOwnerType()
        {
            JsonResponse response = _common.GetShOwnerTypes();
            if (response.HasError)
                _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        public JsonResponse GetAllShOwnerSubType()
        {
            JsonResponse response = _common.GetShOwnerSubTypes();
            if (response.HasError)
                _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse GenerateReport(string ExcelReportType, ATTCompany Company, ATTShareHolderReportData ReportData)
        {
            string CompCode = Company.CompCode;
            string CompEnName = Company.CompEnName;
            JsonResponse response = new JsonResponse();
            var ReportName = "";
            switch (ReportData.ReportType)
            {
                case "ATL":
                    ReportName = "Address_Table_List_Report";
                    break;
                case "SHL":
                    ReportName = "Share_Holder_List_Report";
                    break;
                case "SHLN":
                    ReportName = "Share_Holder_List_Nepali_Report";
                    break;
                case "HAL":
                    ReportName = "Holder_Attendance_Report";
                    break;
                case "SHDL":
                    ReportName = "Share_Holder_Details_List_Report";
                    break;
                case "SHLZK":
                    ReportName = "Share_Holder_List_Zero_Kitta_Report";
                    break;
                case "FL":
                    ReportName = "Fraction_List_Report";
                    break;
                case "AFL":
                    ReportName = "All_Fraction_List_Report";
                    break;

            }
            string[] reportTitles = { CompCode, CompEnName, ReportName };
            response = _shareHolderReport.GenerateReport(_loggedInUser.GetUserNameToDisplay(), CompCode, ExcelReportType, _loggedInUser.GetUserIPAddress(), ReportData);
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            else
            {
                if (response.IsSuccess)
                {
                    var ReportNameToDatabase = ATTGenericReport.ReportName.ShareHolderAddressTableListReport;
                    List<ATTShareHolderReportTotalBasedOn> aTTTotalBasedOns = new List<ATTShareHolderReportTotalBasedOn>();

                    if (ExcelReportType == "P")
                    {
                        switch (ReportData.ReportType)
                        {
                            case "ATL":
                                ReportNameToDatabase = ATTGenericReport.ReportName.ShareHolderAddressTableListReport;
                                response = _genericReport.GenerateReport(ReportNameToDatabase, response, reportTitles);
                                //response.ResponseData = _common.SaveGetPdfReport(response.ResponseData);
                                break;
                            case "SHL":
                                ReportNameToDatabase = ATTGenericReport.ReportName.ShareHolderShareHolderListInEnglish;
                                aTTTotalBasedOns.Add(new ATTShareHolderReportTotalBasedOn()
                                {
                                    TotalBasedOn = "totalkitta"
                                    
                                });
                                response = _genericReport.GenerateReport(ReportNameToDatabase, response, reportTitles, isNepali: false, isTotal: true, aTTTotalBasedOns);
                                //response.ResponseData = _common.SaveGetPdfReport(response.ResponseData);
                                break;
                            case "SHLN":
                                ReportNameToDatabase = ATTGenericReport.ReportName.ShareHolderShareHolderListInNepali;
                                response = _genericReport.GenerateReport(ReportNameToDatabase, response, reportTitles, isNepali: true);
                                //response.ResponseData = _common.SaveGetPdfReport(response.ResponseData);
                                break;
                            case "HAL":
                                ReportNameToDatabase = ATTGenericReport.ReportName.ShareHolderShareHolderAttendanceList;
                                response = _genericReport.GenerateReport(ReportNameToDatabase, response, reportTitles);
                                //response.ResponseData = _common.SaveGetPdfReport(response.ResponseData);
                                break;
                            case "SHDL":
                                ReportNameToDatabase = ATTGenericReport.ReportName.ShareHolderShareHoldersDetailsList;
                                response = _genericReport.GenerateReport(ReportNameToDatabase, response, reportTitles);
                                //response.ResponseData = _common.SaveGetPdfReport(response.ResponseData);
                                break;
                            case "SHLZK":
                                ReportNameToDatabase = ATTGenericReport.ReportName.ShareHolderShareHolderslistInEnglishZeroKitta;
                                aTTTotalBasedOns.Add(new ATTShareHolderReportTotalBasedOn()
                                {
                                    TotalBasedOn = "totalkitta"
                                   
                                });
                                response = _genericReport.GenerateReport(ReportNameToDatabase, response, reportTitles, isNepali: false, isTotal: true, aTTTotalBasedOns);
                                //response.ResponseData = _common.SaveGetPdfReport(response.ResponseData);
                                break;
                            case "FL":
                                ReportNameToDatabase = ATTGenericReport.ReportName.ShareHolderFractionList;
                                aTTTotalBasedOns.Add(new ATTShareHolderReportTotalBasedOn()
                                {
                                    TotalBasedOn = "totalkitta",
                                    Location = "6"
                                });
                                aTTTotalBasedOns.Add(new ATTShareHolderReportTotalBasedOn()
                                {
                                    TotalBasedOn = "fraction",
                                    Location = "7"
                                });
                                response = _genericReport.GenerateReport(ReportNameToDatabase, response, reportTitles, isNepali: false, isTotal: true, aTTTotalBasedOns);
                                //response.ResponseData = _common.SaveGetPdfReport(response.ResponseData);
                                break;
                            case "AFL":
                                ReportNameToDatabase = ATTGenericReport.ReportName.AllShareHolderFractionList;
                                aTTTotalBasedOns.Add(new ATTShareHolderReportTotalBasedOn()
                                {
                                    TotalBasedOn = "totalkitta",
                                    Location = "6"
                                });
                                aTTTotalBasedOns.Add(new ATTShareHolderReportTotalBasedOn()
                                {
                                    TotalBasedOn = "fraction",
                                    Location = "7"
                                });
                                response = _genericReport.GenerateReport(ReportNameToDatabase, response, reportTitles, isNepali: false, isTotal: true, aTTTotalBasedOns);
                                //response.ResponseData = _common.SaveGetPdfReport(response.ResponseData);
                                break;

                        }
                    }
                    else
                    {
                        GenericExcelReport report = new GenericExcelReport();
                        response = report.GenerateExcelReport(response, ReportName, ReportData.ReportType, CompEnName, CompCode, "");

                        if (response.IsSuccess)
                            response.Message = CompCode + "_" + ReportName + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
                    }
                    //response = _genericReport.GenerateReport(ReportNameToDatabase, response, reportTitles);
                    if (response.IsSuccess)
                        if (ExcelReportType != "E")
                            response.Message = ReportName + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".pdf";
                        else
                            response.Message = ReportName + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
                }
            }
            //response = _dailyReport.GenerateReport(_loggedInUser.GetUserNameToDisplay(), CompCode, DailyReportDate, _loggedInUser.GetUserIPAddress());
            //if (response.HasError)
            //{
            //    response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            //}
            //else
            //{
            //    response = _genericReport.GenerateReport(ATTGenericReport.ReportName.DailyReport, response, reportTitles);
            //    if (response.IsSuccess)
            //        response.Message = "AuditReport-" + DailyReportDate + ".pdf";
            //}

            return response;
        }



    }
}
