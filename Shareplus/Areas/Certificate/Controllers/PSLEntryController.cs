

using CDSMODULE.Helper;
using Entity.Certificate;
using Entity.Common;

using Interface.Certificate;
using Interface.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CDSMODULE.Areas.Certificate.Controllers
{
    [Authorize]
    [Area("Certificate")]
    [AutoValidateAntiforgeryToken]
    public class PSLEntryController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IPSLEntry _pSLEntry;
        [System.Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ICheckUserAccess _checkUserAccess;

        [System.Obsolete]
        public PSLEntryController(ILoggedinUser _loggedInUser, IPSLEntry pSLEntry, IHostingEnvironment hostingEnvironment, ICheckUserAccess checkUserAccess)
        {
            this._loggedInUser = _loggedInUser;
            this._pSLEntry = pSLEntry;
            this._hostingEnvironment = hostingEnvironment;
            this._checkUserAccess = checkUserAccess;
        }

        public IActionResult Index()
        {
            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();

            if (_checkUserAccess.CheckIfAccessible(_loggedInUser.GetUserId(), this.ControllerContext))
                return View();
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }

        [HttpPost]
        public JsonResponse GetShholderDetailsByShHolderNo(string CompCode, int ShholderNo, string SelectedAction, int pslno)
        {
            return _pSLEntry.GetShholderDetailsByShHolderNo(CompCode, ShholderNo, SelectedAction, pslno, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
        }

        [HttpPost]
        public JsonResponse GetholderinfoBysearch(string CompCode)
        {
            return _pSLEntry.GethlderinfoBysearch(CompCode, _loggedInUser.GetUserNameToDisplay());
        }
        [HttpPost]
        public JsonResponse Getstatus(string Trantype)
        {
            return _pSLEntry.Getstatus(Trantype);
        }
        [HttpPost]
        public JsonResponse SavePslBatchEntry(List<ATTPSLEntry> PSLEntry, string CompCode, int ShholderNo, string Code, string Remark, string Transdate, string SelectedAction, string Pleggeamount, string Status, int PSLNo, float charge)

        {

            return _pSLEntry.SavePslBatchEntry(PSLEntry, CompCode, ShholderNo, Code, Remark, Transdate, _loggedInUser.GetUserNameToDisplay(), SelectedAction, Pleggeamount, Status, PSLNo, Request.HttpContext.Connection.RemoteIpAddress.ToString(), charge);
        }

        [HttpPost]
        public JsonResponse InsertCertnoInfo(List<ATTPSLEntry> aTTPSLEntry)
        {
            return _pSLEntry.InsertCertnoInfo(aTTPSLEntry);
        }

        [HttpPost]
        public JsonResponse GetAllPledgeAt()
        {
            return _pSLEntry.GetAllPledgeAt();
        }

        [HttpPost]
        public JsonResponse GetHolderByQuery(string CompCode, string FirstName, string LastName, string FatherName, string GrandFatherName)
        {
            return _pSLEntry.GetHolderByQuery(CompCode, FirstName, LastName, FatherName, GrandFatherName, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
        }

    }

}
