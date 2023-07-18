using CDSMODULE.Helper;
using Entity.Common;
using Entity.Esewa;

using Interface.Common;
using Interface.DividendProcessing;
using Interface.Esewa;
using Interface.Security;
using Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CDSMODULE.Areas.ESewaTransaction.Controllers
{
    [Authorize]
    [Area("ESewaTransaction")]
    [AutoValidateAntiforgeryToken]
    public class TransactionStatusController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IConfiguration _configuration;
        private readonly IAudit _audit;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IDividend _dividend;
        private readonly ITransactionStatus _transactionStatus;
        private readonly IEService _eService;


        public TransactionStatusController(ILoggedinUser _loggedInUser, IConfiguration configuration, IAudit audit, ICheckUserAccess checkUserAccess
            , IDividend dividend, ITransactionStatus transactionStatus, IEService eService)
        {
            this._loggedInUser = _loggedInUser;
            _configuration = configuration;
            _audit = audit;
            _checkUserAccess = checkUserAccess;
            _dividend = dividend;
            _transactionStatus = transactionStatus;
            _eService = eService;
        }
        public IActionResult Index()
        {
            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();
            string UserId = _loggedInUser.GetUserId(); 
            JsonResponse res = _audit.auditSave(_loggedInUser.GetUserName(), "Opened " + this.ControllerContext.RouteData.Values["controller"].ToString() + " Form", this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress());
            if (res.IsSuccess)
            {
                if (_checkUserAccess.CheckIfAccessible(UserId, this.ControllerContext))
                    return View();
                return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }
        [HttpPost]
        public JsonResponse GetDividendList(string CompCode)
        {
            JsonResponse response = _transactionStatus.GetDividendList(CompCode, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                _audit.errorSave(_loggedInUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse GetBatchList(string CompCode, string DivCode)
        {
            JsonResponse response = _transactionStatus.GetBatchList(CompCode, DivCode, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                _audit.errorSave(_loggedInUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public async Task<IActionResult> GetAccountValidatedData(string CompCode, string DivCode, string BatchID, string BatchStatus)
        {
            var request = new ATTDataTableRequest();

            request.Draw = Convert.ToInt32(Request.Form["draw"].FirstOrDefault());
            request.Start = Convert.ToInt32(Request.Form["start"].FirstOrDefault());
            request.Length = Convert.ToInt32(Request.Form["length"].FirstOrDefault());
            request.Search = new ATTDataTableSearch()
            {
                Value = Request.Form["search[value]"].FirstOrDefault()
            };
            request.Order = new ATTDataTableOrder[] {
                new ATTDataTableOrder()
                {
                    Dir = Request.Form["order[0][dir]"].FirstOrDefault(),
                    Column = Convert.ToInt32(Request.Form["order[0][column]"].FirstOrDefault()),
                    ColumnName =Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault(),

                    }
                };
            ATTDataTableResponse<ATTBatchProcessing> returnData = new ATTDataTableResponse<ATTBatchProcessing>();

            returnData = await _eService.GetBatchProcessingAsync(request, CompCode, DivCode, BatchID, BatchStatus);
            var jsonData = new { draw = returnData.Draw, recordsFiltered = returnData.RecordsFiltered, recordsTotal = returnData.RecordsTotal, data = returnData.Data };
            return Ok(jsonData);
        }

        [HttpPost]
        public JsonResponse UpdateTransactionStatus(string CompCode, string BatchNo, string DivCode, string BankUserName, string BankPassword)
        {
            JsonResponse response = _transactionStatus.UpdateTransactionStatus(CompCode, BatchNo, DivCode, BankUserName, BankPassword, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                _audit.errorSave(_loggedInUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;

        }
    }
}
