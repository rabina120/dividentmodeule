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
namespace Shareplus.Areas.Reports.Controllers
{
    [Authorize]
    [Area("Reports")]
    [AutoValidateAntiforgeryToken]
    public class ShholderLockUnlockReportController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        private readonly IShareHolderReport _shareHolderReport;
        private readonly IGenericReport _genericReport;

        public ShholderLockUnlockReportController(ILoggedinUser _loggedInUser, IWebHostEnvironment hostingEnvironment
            , ICheckUserAccess checkUserAccess, IAudit audit, IShareHolderReport shareHolderReport, IGenericReport genericReport)
        {

            this._loggedInUser = _loggedInUser;
            _hostingEnvironment = hostingEnvironment;
            this._checkUserAccess = checkUserAccess;
            _audit = audit;
            this._shareHolderReport = shareHolderReport;
            this._genericReport = genericReport;
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
        public Entity.Common.JsonResponse ExportExcel(string CompCode,string CompEnName, string DataType,string StatusType,string DateFrom,string DateTo,string HolderNoFrom,string HolderNoTo,string ReportType)
        {
        JsonResponse response = new JsonResponse();
         response = _shareHolderReport.ShholderLockUnlock( CompCode,  DataType,StatusType,  DateFrom,  DateTo,  HolderNoFrom, HolderNoTo,ReportType,_loggedInUser.GetUserName(),_loggedInUser.GetUserIPAddress());

            //DataType = Y -> Lock
            //DataType = N -> Unlock
            //StatusType = Posted/Unposted


            if (response.IsSuccess)
            {
                if (ReportType=="P")
                {
                    var Title = ATTGenericReport.ReportName.ShareholderLockReportPosted;

                    if(DataType == "Y")
                    {
                        if (StatusType == "Posted")
                        {
                            Title = ATTGenericReport.ReportName.ShareholderLockReportPosted;
                        }
                        else if (StatusType == "Unposted")
                        {
                            Title = ATTGenericReport.ReportName.ShareholderLockReportUnposted;

                        }
                    }
                    else
                    {
                        if (StatusType == "Posted")
                        {
                            Title = ATTGenericReport.ReportName.ShareholderUnlockReportPosted;
                        }
                        else if (StatusType == "Unposted")
                        {
                            Title = ATTGenericReport.ReportName.ShareholderUnlockReportUnposted;
                        }
                    }

                    string[] reportTitles = { CompCode, CompEnName, Enum.GetName(Title.GetType(), Title) };

                    response = _genericReport.GenerateReport(Title, response, reportTitles);
                    if (response.IsSuccess)
                        response.Message = CompCode + "_" + Enum.GetName(Title.GetType(), Title) + "_" + DateTime.Now.ToString("yyyy_mm_dd") + ".pdf";

                }
                else
                {
                    string type;
                    if (DataType == "Y")
                    {
                        type = "Lock";
                    }
                    else
                    {
                        type = "Unlock";
                    }

                    GenericExcelReport report = new GenericExcelReport();
                    response = report.GenerateExcelReport(response, "ShareHolderReport"+  type+ " " + StatusType, StatusType, CompEnName, CompCode, type);
                    if (response.IsSuccess)
                        response.Message = CompCode + "_" + "Lock_Unlock_Report" + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
                }

            
            }
            
        return response;
        }

    }
}
