using CDSMODULE.Helper;
using Entity.Certificate;
using Entity.Common;
using Entity.Reports;
using Interface.Certificate;
using Interface.Common;
using Interface.Reports;
using Interface.Security;
using Microsoft.AspNetCore.Mvc;
using Repository.Reports;
using System;
using System.Collections.Generic;
using static Entity.Reports.ATTGenericReport;

namespace CDSMODULE.Areas.Certificate.Controllers
{
    [Area("Certificate")]
    [AutoValidateAntiforgeryToken]
    public class CertificateListController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly ICertificateList _listCertificate;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        private readonly IGenericReport _genericReport;
        private readonly ICommon _common;

        public CertificateListController(ILoggedinUser _loggedInUser, ICertificateList listC, IAudit audit, ICheckUserAccess checkUserAccess, IGenericReport genericReport, ICommon common)
        {
            this._loggedInUser = _loggedInUser;
            _listCertificate = listC;
            _audit = audit;
            _checkUserAccess = checkUserAccess;
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
        [HttpPost]
        public JsonResponse DisplayDuplicateLists(string CompCode, string CompEnName, string OrderBy, string Listtype)
        {
            JsonResponse response = _listCertificate.DisplayCertificateList(CompCode, _loggedInUser.GetUserNameToDisplay(), OrderBy, Listtype, _loggedInUser.GetUserIPAddress());
            if (!response.HasError)
            {
                if (response.IsSuccess)
                {
                    GenericExcelReport report = new GenericExcelReport();
                    response = report.GenerateExcelReport(response, "DuplicateCertificateList", "DCL", CompEnName, CompCode, "");

                    if (response.IsSuccess)
                        response.Message = CompCode + "_" + "DuplicateCertificateList" + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";

                }
            }
            else
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
                response.IsSuccess = false;
            }
            return response;
        }


        [HttpPost]
        public JsonResponse AllCertiicateList(ATTDuplicateCertificate ReportDataForAllCertificate, string OrderBy, string ShareOwnerType)
        {
            JsonResponse response = _listCertificate.AllCertificateList(ReportDataForAllCertificate, OrderBy, ShareOwnerType);
            if (!response.HasError)
            {
                if (response.IsSuccess)
                {
                    GenericExcelReport report = new GenericExcelReport();
                    response = report.GenerateExcelReport(response, "AllCertificateList", "ACL", ReportDataForAllCertificate.CompEnName, ReportDataForAllCertificate.CompCode, "");

                    if (response.IsSuccess)
                        response.Message = ReportDataForAllCertificate.CompCode + "_" + "AllCertificateList" + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";

                }
            }
            else
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
                response.IsSuccess = false;
            }
            return response;
        }

        [HttpPost]
        public JsonResponse AllCertiicateListReport(ATTDuplicateCertificate ReportDataForAllCertificate, string OrderBy, string ShareOwnerType)
        {
            JsonResponse response = _listCertificate.AllCertificateListForPDF(ReportDataForAllCertificate, OrderBy, ShareOwnerType);
            if (!response.HasError)
            {
                if (response.IsSuccess)
                {
                    List<ATTShareHolderReportTotalBasedOn> aTTTotalBasedOns = new List<ATTShareHolderReportTotalBasedOn>();
                    aTTTotalBasedOns.Add(new ATTShareHolderReportTotalBasedOn()
                    {
                        TotalBasedOn = "SHKITTA"
                        
                    });
                    string[] reportTitles = { ReportDataForAllCertificate.CompCode, ReportDataForAllCertificate.CompEnName, ReportName.AllCertificateList.ToString() };
                    response = _genericReport.GenerateReport(ReportName.AllCertificateList, response, reportTitles, false, true, aTTTotalBasedOns,true);

                    if (response.IsSuccess)
                    {
                        //response.ResponseData = _common.SaveGetPdfReport(response.ResponseData);
                        response.Message = ReportDataForAllCertificate.CompCode + "_" + "AllCertificateList" + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".pdf";
                    }

                }
            }
            else
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
                response.IsSuccess = false;
            }
            return response;
        }

        [HttpPost]
        public JsonResponse DistributedUnDistributedList(ATTDuplicateCertificate ReportDataDistributedCertificatesList, string OrderBy, string Listtype, string sharetype, string SelectedAction)
        {
            JsonResponse response = new JsonResponse();
            response = _listCertificate.DistributedUnDistributedList(ReportDataDistributedCertificatesList, OrderBy, Listtype, sharetype, SelectedAction);
            if (!response.HasError)
            {
                if (response.IsSuccess)
                {
                    GenericExcelReport report = new GenericExcelReport();
                    response = report.GenerateExcelReport(response, "DistributedUndistributedCertificateList", "DUCL", ReportDataDistributedCertificatesList.CompEnName, ReportDataDistributedCertificatesList.CompCode, "");

                    if (response.IsSuccess)
                        response.Message = ReportDataDistributedCertificatesList.CompEnName + "_" + "DistributedUndistributedCertificateList" + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";

                }
            }
            else
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
                response.IsSuccess = false;
            }
            return response;
        }
        [HttpPost]
        public JsonResponse DistributedUnDistributedListReport(ATTDuplicateCertificate ReportDataDistributedCertificatesList, string OrderBy, string Listtype, string sharetype, string SelectedAction)
        {
            JsonResponse response = new JsonResponse();
            response = _listCertificate.DistributedUnDistributedListForPDF(ReportDataDistributedCertificatesList, OrderBy, Listtype, sharetype, SelectedAction);
            if (!response.HasError)
            {
                if (response.IsSuccess)
                {
                    List<ATTShareHolderReportTotalBasedOn> totalcolumn = new List<ATTShareHolderReportTotalBasedOn>();
                    totalcolumn.Add(new ATTShareHolderReportTotalBasedOn()
                    {
                        TotalBasedOn = "SHKITTA",
                        Location = "4"
                    });
                    string reporttype = string.Empty;
                    if (Listtype == "P")
                    {
                        reporttype = "(POSTED)";

                    }
                    else if (Listtype == "U")
                    {
                        reporttype = "(UNPOSTED)";
                    }
                    string[] reportTitles = { ReportDataDistributedCertificatesList.CompCode, ReportDataDistributedCertificatesList.CompEnName, (SelectedAction == "D" ? "Distributed Certificate List" : "UnDistributed Certificate List")+reporttype };
                    response = _genericReport.GenerateReport(ReportName.DistributedUndistributedCertificateList, response, reportTitles, false, true, totalcolumn);

                    if (response.IsSuccess)
                    {
                        //response.ResponseData = _common.SaveGetPdfReport(response.ResponseData);
                        response.Message = ReportDataDistributedCertificatesList.CompEnName + "_" + SelectedAction == "D" ? "DistributedCertificateList" : "UnDistributedCertificateList" + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".pdf";
                    }
                }
            }
            else
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
                response.IsSuccess = false;
            }
            return response;
        }
    }
}

