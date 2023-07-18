using CDSMODULE.Helper;
using Entity.Common;
using Entity.Reports;
using Interface.Common;
using Interface.Reports;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CDSMODULE.Areas.Reports.Controllers
{
    [Area("Reports")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class DemateRemateTransferController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IGenerateReport _generateReport;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IGenericReport _genericReport;
        private readonly IAudit _audit;


        public DemateRemateTransferController(ILoggedinUser _loggedInUser, IAudit audit, IWebHostEnvironment hostingEnvironment, IGenerateReport generateReport, ICheckUserAccess checkUserAccess, IGenericReport genericReport)
        {
            this._loggedInUser = _loggedInUser;
            _hostingEnvironment = hostingEnvironment;
            _generateReport = generateReport;
            this._checkUserAccess = checkUserAccess;
            _genericReport = genericReport;
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
                    return View();
                return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });

        }

        [HttpPost]
        public JsonResponse GenerateReport(string CompCode, string CompEnName, string TransferedDtFrom, string TransferedDtTo)
        {
            JsonResponse jsonResponse = new JsonResponse();
            JsonResponse jsonResponseToReturn = new JsonResponse();

            jsonResponse = _generateReport.GenerateReportDemateRemateList(CompCode, TransferedDtFrom, TransferedDtTo, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (jsonResponse.HasError)
            {
                jsonResponseToReturn = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)jsonResponse.ResponseData);

            }
            else
            {
                if (jsonResponse.IsSuccess)
                {
                    string[] reportTitles = { CompCode, CompEnName, " Demate Remate Transfer List Report From: " + TransferedDtFrom + " To :" + TransferedDtTo };
                    jsonResponseToReturn = _genericReport.GenerateReport(ATTGenericReport.ReportName.DemateRemateReport, jsonResponse, reportTitles);
                    if (jsonResponseToReturn.IsSuccess)
                        jsonResponseToReturn.Message = "Company_" + CompEnName + "_Code_" + CompCode + "_DemateRemateTransferReport_" + "From: " + TransferedDtFrom + " To :" + TransferedDtTo + ".pdf";

                }
                else
                {
                    jsonResponseToReturn = jsonResponse;
                }
            }

            return jsonResponseToReturn;
        }
    }
}


