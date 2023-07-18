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
    [Area("DividendProcessing")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class DemateDividendPaymentEntryController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IDemateDividentPaymentEntry _demateDividentPaymentEntry;
        private readonly IDemateDividend _demateDividend;
        private readonly IPaymentCenter _paymentCenter;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        public DemateDividendPaymentEntryController(ILoggedinUser loggedInUser, IDemateDividentPaymentEntry demateDividentPaymentEntry,
            IPaymentCenter paymentCenter, IDemateDividend demateDividend, ICheckUserAccess checkUserAccess, IAudit audit)
        {
            this._loggedInUser = loggedInUser;
            _demateDividentPaymentEntry = demateDividentPaymentEntry;
            _paymentCenter = paymentCenter;
            _demateDividend = demateDividend;
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
        public JsonResponse GetDemateDividendInformation(string CompCode, string DivCode, string shholderno, string a, string ActionType)
        {
            JsonResponse response = _demateDividentPaymentEntry.GetDemateDividendInformation(CompCode, DivCode, shholderno, a, ActionType,_loggedInUser.GetUserNameToDisplay(),_loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse SaveDemateDividendPaymentEntry(string DivCode, string CompCode,string bankName,string accountNo, string centerid, string Payment, string PayUser, string remarks, string warrantNo, string boidno, string selectedAction,string creditedDt, string wissueddate)
        {
            JsonResponse response = _demateDividentPaymentEntry.SaveDemateDividendPaymentEntry(DivCode, CompCode, bankName,accountNo, centerid, Payment, PayUser, remarks, warrantNo, boidno, selectedAction, creditedDt, wissueddate, _loggedInUser.GetUserNameToDisplay(),_loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
    }
}
