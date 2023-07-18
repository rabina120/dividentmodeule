

using CDSMODULE.Helper;
using Entity.Certificate;
using Entity.Common;
using Interface.Certificate;
using Interface.Common;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;


namespace CDSMODULE.Areas.Certificate.Controllers
{

    [Area("Certificate")]
    [AutoValidateAntiforgeryToken]
    public class ClearPSLEntryController : Controller
    {

        private readonly ILoggedinUser _loggedInUser;

        private readonly IClearPSLEntry _PSLEntry;

        private readonly ICheckUserAccess _checkUserAccess;

        public ClearPSLEntryController(ICheckUserAccess checkUserAccess, ILoggedinUser _loggedInUser, IClearPSLEntry PSLEntry)
        {
            this._loggedInUser = _loggedInUser;
            _checkUserAccess = checkUserAccess;
            _PSLEntry = PSLEntry;


        }




        public IActionResult Index()
        {

            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();
            if (_checkUserAccess.CheckIfAccessible(_loggedInUser.GetUserId(), this.ControllerContext))
                return View();
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }

        [HttpPost]
        public JsonResponse SearchHolderPSL(string CompCode, string ShholderNo, string SelectedAction)
        {
            return _PSLEntry.SearchHolderPSL(CompCode, ShholderNo, SelectedAction, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());

        }
        [HttpPost]
        public JsonResponse GetPSLInformation(string CompCode, string ShholderNo, string SelectedAction, string PSLNo)
        {
            return _PSLEntry.GetPSLInformation(CompCode, ShholderNo, SelectedAction, PSLNo, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
        }

        [HttpPost]

        public JsonResponse SavePSLClearEntry(List<ATTPSLEntryClear> ReportData, string CompCode, int ShholderNo, int pslno, int PSL_clear_No, string Charge, string ClearedDt, string Remark, string UserName, string Issuedup, string SelectedAction)

        {

            return _PSLEntry.SavePSLClearEntry(ReportData, CompCode, ShholderNo, pslno, PSL_clear_No, Charge, ClearedDt, Remark, _loggedInUser.GetUserNameToDisplay(), Issuedup, SelectedAction, Request.HttpContext.Connection.RemoteIpAddress.ToString());
        }






    }
}
