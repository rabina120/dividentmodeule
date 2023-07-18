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
    public class UndoDividendPaymentController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        private readonly IUndoDividendPayment _undoDividendPayment;
        public UndoDividendPaymentController(ILoggedinUser _loggedInUser, ICheckUserAccess checkUserAccess, IAudit audit, IUndoDividendPayment undoDividendPayment)
        {
            this._loggedInUser = _loggedInUser;
            this._checkUserAccess = checkUserAccess;
            _audit = audit;
            _undoDividendPayment = undoDividendPayment;
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
        public JsonResponse GetDividendInformation(string CompCode, string DivCode, string shholderno, string based, string undoType)
        {
            JsonResponse response = _undoDividendPayment.GetDividendInformation(CompCode, DivCode, shholderno, based, undoType, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());

            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse SaveCashDividend(string DivCode, string CompCode, string undoType,string warrantno, string shholderno)
        {
            JsonResponse response = _undoDividendPayment.SaveCashDividend(DivCode, CompCode, undoType, warrantno, shholderno, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
    }
}
