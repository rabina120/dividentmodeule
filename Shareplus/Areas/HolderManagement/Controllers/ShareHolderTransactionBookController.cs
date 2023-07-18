using CDSMODULE.Helper;
using DocumentFormat.OpenXml.Spreadsheet;
using Entity.Common;
using Entity.Reports;
using Interface.Common;
using Interface.Reports;
using Interface.Security;
using Interface.ShareHolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repository.Reports;
using System;
using static Entity.Reports.ATTGenericReport;

namespace Shareplus.Areas.HolderManagement.Controllers
{
    [Authorize]
    [Area("HolderManagement")]
    [AutoValidateAntiforgeryToken]
    public class ShareHolderTransactionBookController : Controller
    {
        private readonly ILoggedinUser _IloggedinUser;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        private readonly IShareTransactionBook _shTranBook;
        private readonly IGenericReport _genericReport;

        public ShareHolderTransactionBookController(ILoggedinUser _loggedInUser, ICheckUserAccess checkUserAccess, IAudit audit, IShareTransactionBook shTranBook, IGenericReport genericReport)
        {
            _IloggedinUser = _loggedInUser;
            _checkUserAccess = checkUserAccess;
            _audit = audit;
            _shTranBook = shTranBook;
            _genericReport = genericReport;
        }
        public IActionResult Index()
        {
            ViewBag.UserName = _IloggedinUser.GetUserNameToDisplay();
            string UserId = _IloggedinUser.GetUserId();
            JsonResponse res = _audit.auditSave(_IloggedinUser.GetUserNameToDisplay(), "Opened " + this.ControllerContext.RouteData.Values["controller"].ToString() + " Form", this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress());
            if (res.IsSuccess)
            {
                if (_checkUserAccess.CheckIfAccessible(UserId, this.ControllerContext))
                    return View();
                return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }

        [HttpPost]
        public object GetShareHolderTransactionBook(string CompCode, string SHNumber)
        {
            JsonResponse response = new JsonResponse();
            response = _shTranBook.GetShareHolderTransactionBook(CompCode, SHNumber, _IloggedinUser.GetUserName(), _IloggedinUser.GetUserIPAddress());
            return JsonConvert.SerializeObject(response);
        }
        [HttpPost]
        public object GetShareTypes(string CompCode, string SHNumber, string ShareType)
        {
            JsonResponse response = new JsonResponse();
            response = _shTranBook.GetShareTypes(CompCode, SHNumber, ShareType, _IloggedinUser.GetUserName(), _IloggedinUser.GetUserIPAddress());
            return JsonConvert.SerializeObject(response);
        }
        public object GetPurchaseSalesReport(string CompCode, string SHNumber, string ShareType, string CompName, string HolderName, string FileType)
        {
            JsonResponse response;
            //JsonResponse resp = new JsonResponse();
            response = _shTranBook.GetPurchaseSalesReport(CompCode, SHNumber, ShareType, _IloggedinUser.GetUserName(), _IloggedinUser.GetUserIPAddress(), FileType);
            string PSReportName = (ShareType == "P") ? "Purchase" : "Sales";

            if (response.IsSuccess && FileType == "P")
            {
                string[] reportTitles = { CompCode, CompName,  PSReportName + " Report of Holder " + HolderName };
                response = _genericReport.GenerateReport(ATTGenericReport.ReportName.SharePurchaseSalesReport, response, reportTitles);
                if (response.IsSuccess)
                    response.Message = CompCode + "_" + PSReportName + "_Report_of_Holder_" + HolderName + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".pdf";
            }
            else if (response.IsSuccess && FileType == "E")
            {
                GenericExcelReport report = new GenericExcelReport();
                var sheetName = PSReportName + "_Report";
                response = report.GenerateExcelReport(response, sheetName, null, CompName, CompCode, ShareType);

                if (response.IsSuccess)
                    response.Message = CompCode + "_" + PSReportName + "_Report_of_Holder_" + HolderName + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
            }
            return JsonConvert.SerializeObject(response);
        }
    }
}
