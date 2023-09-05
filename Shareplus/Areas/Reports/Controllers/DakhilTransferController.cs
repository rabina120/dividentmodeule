using CDSMODULE.Helper;
using Entity.Common;
using Entity.Reports;
using Interface.Common;
using Interface.DakhilTransfer;
using Interface.Reports;
using Interface.Security;
using INTERFACE.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repository.Reports;
using System;

namespace Shareplus.Areas.Reports.Controllers
{
    [Area("Reports")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class DakhilTransferController : Controller
    {
       
        private readonly ILoggedinUser _loggedInUser;
        private readonly IDakhilTransferReport _generateReport;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        //private readonly GenericExcelReport _excelReport;
        private readonly IGenericReport _genericReport;
        private readonly IDakhilManyToOneTransferEntry _dakhilTran;
        private readonly ICommon _common;
        public DakhilTransferController(ILoggedinUser _loggedInUser, IAudit audit, IGenericReport genericReport,
           IWebHostEnvironment hostingEnvironment, IDakhilTransferReport generateReport, ICheckUserAccess checkUserAccess, IDakhilManyToOneTransferEntry dakhilTran, ICommon common)
        {
            this._loggedInUser = _loggedInUser;
            _hostingEnvironment = hostingEnvironment;
            _generateReport = generateReport;
            this._checkUserAccess = checkUserAccess;
            _audit = audit;
            _genericReport = genericReport;
            _dakhilTran = dakhilTran;
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
            else RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }
        [HttpPost]
        public object GetBrokerList(string CompCode)
        {
            JsonResponse resp = new JsonResponse();
            resp = _dakhilTran.GetBrokerList(CompCode);
            return JsonConvert.SerializeObject(resp);
        }

        [HttpPost]
        public JsonResponse GenerateReportForPDF(string Compcode, string SelectedAction, string ReportType, string FromDate,
            string ToDate, string RegnoFrom, string RegnoTo, string TranKittaFrom, string TranKittaTo, string BHolderNoFrom,
           string BHolderNoTo, string SHolderNoFrom, string SHolderNoTo, string Broker,string CompEnName)
        {
            JsonResponse response = new JsonResponse();
            var Title = GetReportTitle(SelectedAction);
            string type = string.Empty;
            if (ReportType == "S") type = "SummaryOf";
            if (ReportType == "D") type = "DetailOf";
            response = _generateReport.GetReportDataForPDF(Compcode, SelectedAction, ReportType, FromDate,
             ToDate, RegnoFrom, RegnoTo, TranKittaFrom, TranKittaTo, BHolderNoFrom,
            BHolderNoTo, SHolderNoFrom,SHolderNoTo, Broker, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            string[] reportTitles = { Compcode, CompEnName, type+Enum.GetName(Title.GetType(), Title) };
            ATTShareHolderReportTotalBasedOn totalBasedOn = new ATTShareHolderReportTotalBasedOn()
            {
                TotalBasedOn=Title==ATTGenericReport.ReportName.DakhilKittaReport?"TotalKitta":"Trankitta"
            };
            if (response.HasError)
            {
                _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(),(Exception) response.ResponseData);
            }
            else
            {
                response = _genericReport.GenerateReport(Title, response, reportTitles);
                if (response.IsSuccess)
                {
                    response.ResponseData = _common.SaveGetPdfReport(response.ResponseData);
                    response.Message = Compcode + "_" + type + Enum.GetName(Title.GetType(), Title) + "_" + DateTime.Now.ToString("yyyy_mm_dd") + ".pdf";
                }
            }
        
            return response;

        }
        [HttpPost]
        public JsonResponse GenerateReportForExcel(string Compcode, string SelectedAction, string ReportType, string FromDate,
           string ToDate, string RegnoFrom, string RegnoTo, string TranKittaFrom, string TranKittaTo, string BHolderNoFrom,
           string BHolderNoTo, string SHolderNoFrom,string SHolderNoTo, string Broker,string CompEnName)
        {
            JsonResponse response = new JsonResponse();
            GenericExcelReport _excelReport = new GenericExcelReport();
            var Title = GetReportTitle(SelectedAction);
            string type = string.Empty;
            if(ReportType=="S") type="SummaryOf";
            if(ReportType=="D") type="DetailOf";
            response = _generateReport.GetReportDataForExcel(Compcode, SelectedAction, ReportType, FromDate,
             ToDate, RegnoFrom, RegnoTo, TranKittaFrom, TranKittaTo, BHolderNoFrom,
            BHolderNoTo, SHolderNoFrom, SHolderNoTo, Broker, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            else
            {
                 response = _excelReport.GenerateExcelReport(response,type + Enum.GetName(Title.GetType(), Title), "P", CompEnName, Compcode, null);
                if (response.IsSuccess)
                    response.Message = Compcode + "_" + type+Enum.GetName(Title.GetType(), Title) + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
            }

            return response;

        }

        private ATTGenericReport.ReportName GetReportTitle(string SelectedCode)
        {
             ATTGenericReport.ReportName title =  new ATTGenericReport.ReportName();
            switch(SelectedCode)
            {
                case "DL":
                    title = ATTGenericReport.ReportName.DakhilList;
                        break;
                case "TL":
                    title = ATTGenericReport.ReportName.TransferList;

                    break;
                case "DK":
                    title = ATTGenericReport.ReportName.DakhilKittaReport;

                    break;
                case "KH":
                    title = ATTGenericReport.ReportName.DakhilKharejBook;

                    break;
                case "BL":
                    title = ATTGenericReport.ReportName.DakhilBuyerList;

                    break;
                case "SL":
                    title = ATTGenericReport.ReportName.DakhilSellerList;

                    break;
                 default:
                    title = new ATTGenericReport.ReportName();

                    break;
            }

            return title;
        }
    }
}
