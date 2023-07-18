﻿using CDSMODULE.Helper;
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
    public class ShareTransferController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private IWebHostEnvironment _webHostEnvironment;
        private readonly IOptions<ReadConfig> _connectionString;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IDakhilShareTransfer _dakhilShareTransfer;
        private readonly IAudit _audit;

        public ShareTransferController(IOptions<ReadConfig> connectionString, IWebHostEnvironment webHostEnvironment, ILoggedinUser _loggedInUser, ICheckUserAccess checkUserAccess, IDakhilShareTransfer dakhilShareTransfer, IAudit audit)
        {
            this._connectionString = connectionString;
            this._webHostEnvironment = webHostEnvironment;
            this._loggedInUser = _loggedInUser;
            this._checkUserAccess = checkUserAccess;
            this._dakhilShareTransfer = dakhilShareTransfer;
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
        public JsonResponse GetShareTransferList(string CompCode, string RegNoFrom, string RegNoTo, string DateFrom, string DateTo)
        {
            JsonResponse response = _dakhilShareTransfer.GetShareTransferList(CompCode, RegNoFrom, RegNoTo, DateFrom, DateTo, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
        [HttpPost]
        public JsonResponse GetIndividualShareTransferList(string CompCode, string RegNo, string BHolderNo)
        {
            JsonResponse response = _dakhilShareTransfer.GetIndividualShareTransferList(CompCode, RegNo, BHolderNo, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
        [HttpPost]
        public JsonResponse SaveShareTransfer(string CompCode, List<ATTShareDakhilTransfer> aTTShareDakhilTransfers, string TransferedDate, string SelectedAction, string FolioNo = null, string BatchNo = null)
        {
            JsonResponse response = _dakhilShareTransfer.SaveShareTransfer(CompCode: CompCode, aTTShareDakhilTransfers: aTTShareDakhilTransfers, UserName: _loggedInUser.GetUserNameToDisplay(), IPAddress: Request.HttpContext.Connection.RemoteIpAddress.ToString(), TransferedDate: TransferedDate, SelectedAction: SelectedAction, FolioNo: FolioNo, BatchNo: BatchNo);
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
    }
}
