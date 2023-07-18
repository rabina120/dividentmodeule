using CDSMODULE.Helper;
using Entity.Common;
using Entity.DakhilTransfer;
using Interface.Common;
using Interface.DakhilTransfer;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;

namespace CDSMODULE.Areas.DakhilTransfer.Controllers
{
    [Authorize]
    [Area("DakhilTransfer")]
    [AutoValidateAntiforgeryToken]
    public class DakhilIndividualTransferEntryController : Controller
    {

        private readonly ILoggedinUser _loggedInUser;
        private IWebHostEnvironment _webHostEnvironment;
        private readonly IOptions<ReadConfig> _connectionString;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IDakhilIndividualTransferEntry _dakhilIndividualTransferEntry;
        private readonly IAudit _audit;

        public DakhilIndividualTransferEntryController(IOptions<ReadConfig> connectionString, IWebHostEnvironment webHostEnvironment, ILoggedinUser _loggedInUser, ICheckUserAccess checkUserAccess, IDakhilIndividualTransferEntry dakhilIndividualTransferEntry, IAudit audit)
        {
            this._connectionString = connectionString;
            this._webHostEnvironment = webHostEnvironment;
            this._loggedInUser = _loggedInUser;
            this._dakhilIndividualTransferEntry = dakhilIndividualTransferEntry;
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
        public JsonResponse GetCertificateInformation(string CompCode, string CertificateNo, string SelectedAction)
        {
            JsonResponse response = _dakhilIndividualTransferEntry.GetCertificateInformation(CompCode, CertificateNo, SelectedAction, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }

        [HttpPost]
        public JsonResponse GetBrokerList(string CompCode)
        {
            JsonResponse response = _dakhilIndividualTransferEntry.GetBrokerList(CompCode);
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
        [HttpPost]
        public JsonResponse GetBuyerInformation(string CompCode, string BHolderNo)
        {
            JsonResponse response = _dakhilIndividualTransferEntry.GetBuyerInformation(CompCode, BHolderNo, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
        [HttpPost]
        public JsonResponse SaveDakhilTransfer(ATTDakhilTransfer DakhilTransferData, string SelectedAction)
        {
            JsonResponse response = _dakhilIndividualTransferEntry.SaveDakhilTransfer(DakhilTransferData, SelectedAction, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
    }
}
