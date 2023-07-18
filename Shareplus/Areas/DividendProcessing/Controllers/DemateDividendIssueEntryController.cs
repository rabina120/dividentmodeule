using CDSMODULE.Helper;
using Entity.Common;

using Interface.Common;
using Interface.DividendProcessing;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CDSMODULE.Areas.DividendProcessing.Controllers
{
    [Authorize]
    [Area("DividendProcessing")]
    [AutoValidateAntiforgeryToken]
    public class DemateDividendIssueEntryController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IDemateDividend _demateDividend;
        private readonly ICashDemateDividendIssueEntry _cashDemateDividendIssueEntry;
        private readonly IPaymentCenter _paymentCenter;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;

        public DemateDividendIssueEntryController(ILoggedinUser _loggedInUser, IDemateDividend demateDividend,
            IPaymentCenter paymentCenter, ICashDemateDividendIssueEntry cashDemateDividendIssueEntry, ICheckUserAccess checkUserAccess, IAudit audit)
        {
            this._loggedInUser = _loggedInUser;
            _demateDividend = demateDividend;
            _paymentCenter = paymentCenter;
            _cashDemateDividendIssueEntry = cashDemateDividendIssueEntry;
            this._checkUserAccess = checkUserAccess;
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
        public JsonResponse GetAllDemateDividends(string CompCode)
        {
            JsonResponse response = _demateDividend.GetDemateDividendTableList(CompCode);
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [HttpPost]
        public JsonResponse GetDemateDividendInformation(string CompCode, string DivCode, string shholderno, string actionType, string SelectedAction)
        {
            JsonResponse response = _cashDemateDividendIssueEntry.GetDemateDividendInformation(CompCode, DivCode, shholderno, actionType, SelectedAction, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse SaveCashDemateDividend(string DivCode, string CompCode, string centerid, string remarks, string bankName, string accountNo, string compcode, string warrantNo, string boidno, string selectedAction, string wissueddate, string creditedDt, string IsPaidBy)
        {
            JsonResponse response = _cashDemateDividendIssueEntry.SaveCashDemateDividend(DivCode, CompCode, centerid, remarks, bankName, accountNo, compcode, warrantNo, boidno, selectedAction, wissueddate, creditedDt, _loggedInUser.GetUserNameToDisplay(), IsPaidBy, _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
    }
}
