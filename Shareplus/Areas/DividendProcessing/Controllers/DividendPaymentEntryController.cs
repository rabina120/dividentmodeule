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
    public class DividendPaymentEntryController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IDividend _dividend;
        private readonly IDividentPaymentEntry _dividentPaymentEntry;
        private readonly IPaymentCenter _paymentCenter;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;

        public DividendPaymentEntryController(ILoggedinUser _loggedInUser, IDividend dividend, IDividentPaymentEntry dividentPaymentEntry, IPaymentCenter paymentCenter, ICheckUserAccess checkUserAccess, IAudit audit)
        {
            this._loggedInUser = _loggedInUser;
            _dividend = dividend;
            _dividentPaymentEntry = dividentPaymentEntry;
            _paymentCenter = paymentCenter;
            _checkUserAccess = checkUserAccess;
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
        public JsonResponse GetAllDividends(string CompCode)
        {
            JsonResponse response = _dividend.GetDividendTableList(CompCode);
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }

        [HttpPost]
        public JsonResponse GetDividendInformation(string CompCode, string DivCode, string shholderno, string a, string ac)
        {
            //    a is for based on like shholder and warrant no
            //    ac is for the selected action add or delete
            JsonResponse response = _dividentPaymentEntry.GetDividendInformation(CompCode, DivCode, shholderno, a, ac, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }

        [HttpPost]
        public JsonResponse GetAllPaymentCenter()
        {
            JsonResponse response = _paymentCenter.GetPaymentCenter();
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }

        [HttpPost]
        public JsonResponse GetMaxSeqNo(string tablename, string centerid)
        {
            JsonResponse response = _dividentPaymentEntry.GetMaxSeqno(tablename, centerid);
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }

        [HttpPost]
        public JsonResponse SaveDividentPaymentEntry(string DivCode, string CompCode, string bankName, string accountNo, string centerid, string PayUser, string telno, string cashOrCheque,string warrantNo, string shholderno, string selectedAction,string creditedDt, string wissueddate, string IssueRemarks)
        {
            JsonResponse response = _dividentPaymentEntry.SaveDividentPaymentEntry(DivCode, CompCode,bankName,accountNo, centerid, _loggedInUser.GetUserNameToDisplay(), telno, cashOrCheque, warrantNo, shholderno, selectedAction, creditedDt, wissueddate, IssueRemarks, _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
    }
}
