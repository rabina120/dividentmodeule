using CDSMODULE.Helper;
using Entity.Common;
using Entity.Reports;
using Interface.Common;
using Interface.Reports;
using Interface.Security;
using INTERFACE.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Repository.Reports;
using System;

namespace CDSMODULE.Areas.Security.Controllers
{
    [Authorize]
    [Area("Security")]
    [AutoValidateAntiforgeryToken]
    public class UserReportController : Controller
    {
        private readonly ILoggedinUser _IloggedinUser;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IUserReport _userReport;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IGenericReport _aTTGenericReport;
        private readonly IAudit _audit;


        public UserReportController(ILoggedinUser IloggedinUser, ICheckUserAccess checkUserAccess, IUserReport userReport, IWebHostEnvironment webHostEnvironment, IGenericReport aTTGenericReport, IAudit audit)
        {
            _IloggedinUser = IloggedinUser;
            _checkUserAccess = checkUserAccess;
            _userReport = userReport;
            _webHostEnvironment = webHostEnvironment;
            _aTTGenericReport = aTTGenericReport;
            _audit = audit;
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
        public JsonResponse GenerateReport(string FromDate, string ToDate, string ReportType, string UserName = null)
        {
            string UserNameR;
            JsonResponse response = new JsonResponse();
            if (UserName == null)
            {
                UserNameR = "All User";
            }
            else
            {
                UserNameR = UserName;
            }
                

            string[] reportTitles = { " ", "PSMS ", UserNameR + "'s Audit Report" };
            response = _userReport.GenerateReport(UserName, FromDate, ToDate, _IloggedinUser.GetUserIPAddress(), _IloggedinUser.GetUserNameToDisplay(), ReportType);

            if (response.HasError)
            {
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            else
            {
                if (ReportType == "R")
                {
                    response = _aTTGenericReport.GenerateReport(ATTGenericReport.ReportName.UserAuditReport, response, reportTitles);
                    if (response.IsSuccess)
                        response.Message = UserNameR + "-AuditReport-" + DateTime.Now.ToString("yyyy-MM-dd") + ".pdf";

                }
                else
                {
                    GenericExcelReport report = new GenericExcelReport();
                    response = report.GenerateExcelReport(response, UserNameR + " - AuditReport - " + DateTime.Now.ToString("yyyy - MM - dd"), "", "SCB", "001", null);
                    response.Message = UserNameR + "-AuditReport-" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
                }
            }
            return response;

        }

        [HttpPost]
        public JsonResponse GetUserRole()
        {
            JsonResponse response = new JsonResponse();
            if (_IloggedinUser.GetUserRole() != "19")
            {
                response.IsSuccess = true;
                response.ResponseData = _IloggedinUser.GetUserNameToDisplay();
            }
            return response;
        }


    }
}
