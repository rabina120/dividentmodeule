using CDSMODULE.Helper;
using Entity.Common;

using Interface.Common;
using Interface.DividendManagement;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Reports;
using System;

namespace CDSMODULE.Areas.DividendManagement.Controllers
{
    [Authorize]
    [Area("DividendManagement")]
    [AutoValidateAntiforgeryToken]
    public class BulkInsertPostingController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IAudit _audit;
        private ICheckUserAccess _checkUserAccess;
        private readonly ICashDividendBulkEntryPosting _cashDividendBulkEntryPosting;

        public BulkInsertPostingController(ILoggedinUser _loggedInUser, IAudit audit, ICheckUserAccess checkUserAccess, ICashDividendBulkEntryPosting cashDividendBulkEntryPosting)
        {
            this._loggedInUser = _loggedInUser;
            _audit = audit;
            _checkUserAccess = checkUserAccess;
            _cashDividendBulkEntryPosting = cashDividendBulkEntryPosting;
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
        public JsonResponse GetDividendList(string CompCode, string BonusType, string ShareType)
        {
            JsonResponse response = _cashDividendBulkEntryPosting.GetDividendList(CompCode, BonusType, ShareType);

            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse GetSelectedDividendDetails(string CompCode, string BonusType, string ShareType, string DivCode)
        {
            JsonResponse response = _cashDividendBulkEntryPosting.GetSelectedDividendDetails(CompCode, BonusType, ShareType, DivCode);

            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse GetSelectedDividendDetailsExcel(string CompCode, string BonusType, string ShareType, string DivCode, string CompEnName)
        {
            JsonResponse response = _cashDividendBulkEntryPosting.GenerateData(CompCode, BonusType, ShareType, DivCode, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (!response.HasError)
            {
                if (response.IsSuccess)
                {
                    GenericExcelReport report = new GenericExcelReport();
                    string tablename = response.ResponseData2.ToString();
                    response = report.GenerateExcelReport(response, "Sheet1", tablename + "'s Data", CompEnName, CompCode, ShareType);

                    if (response.IsSuccess)
                        response.Message = CompCode + "_" + tablename + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";

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
        public JsonResponse Save(string CompCode, string BonusType, string ShareType, string DivCode, string AcceptOrReject)
        {
            JsonResponse response = _cashDividendBulkEntryPosting.Save(CompCode, BonusType, ShareType, DivCode, AcceptOrReject, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
    }
}
