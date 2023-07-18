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
using System.Collections.Generic;

namespace CDSMODULE.Areas.DakhilTransfer.Controllers
{
    [Authorize]
    [Area("DakhilTransfer")]
    [AutoValidateAntiforgeryToken]
    public class DakhilManyToOneTransferEntryController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private IWebHostEnvironment _webHostEnvironment;
        private readonly IOptions<ReadConfig> _connectionString;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IDakhilManyToOneTransferEntry _dakhilManyToOneTransferEntry;
        private readonly IAudit _audit;

        public DakhilManyToOneTransferEntryController(IOptions<ReadConfig> connectionString, IWebHostEnvironment webHostEnvironment, ILoggedinUser _loggedInUser, ICheckUserAccess checkUserAccess, IDakhilManyToOneTransferEntry dakhilManyToOneTransferEntry, IAudit audit)
        {
            this._connectionString = connectionString;
            this._webHostEnvironment = webHostEnvironment;
            this._loggedInUser = _loggedInUser;
            this._checkUserAccess = checkUserAccess;
            this._dakhilManyToOneTransferEntry = dakhilManyToOneTransferEntry;
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
        public JsonResponse GetBrokerList(string CompCode)
        {
            JsonResponse response = _dakhilManyToOneTransferEntry.GetBrokerList(CompCode);
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
        [HttpPost]
        public JsonResponse GetBuyerInformation(string CompCode, string BHolderNo)
        {
            JsonResponse response = _dakhilManyToOneTransferEntry.GetBuyerInformation(CompCode, BHolderNo, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
        [HttpPost]
        public JsonResponse GetSellerCertificateInformation(string CompCode, string CertificateNo)
        {
            JsonResponse response = _dakhilManyToOneTransferEntry.GetSellerCertificateInformation(CompCode, CertificateNo, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
        [HttpPost]
        public JsonResponse GetMaxRegNo(string CompCode)
        {
            JsonResponse response = _dakhilManyToOneTransferEntry.GetMaxRegNo(CompCode);
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
        [HttpPost]
        public JsonResponse SaveBatchDakhilTransfer(string CompCode, string BuyerHolderNo, List<ATTDakhilSellerInformation> sellers, int? Letter, int? TradeType,
            string Broker, string DakhilDate)
        {
            JsonResponse response = _dakhilManyToOneTransferEntry.SaveBatchDakhilTransfer(CompCode, BuyerHolderNo, sellers, Letter, TradeType, Broker, DakhilDate, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
    }
}
