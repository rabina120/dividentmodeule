using CDSMODULE.Helper;
using Entity.Common;
using Entity.Esewa;
using Interface.Common;
using Interface.Esewa;
using Interface.Security;
using Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
    public class EsewaTransactionProcessingController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly ILogDetails _logDetails;
        private readonly IAudit _audit;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IEsewaTransactionProcessing _batchProcessing;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IEService _eService;
        private IConfiguration _configuration;

        public EsewaTransactionProcessingController(ILoggedinUser _loggedInUser, ILogDetails logDetails, IConfiguration configuration,
            IAudit audit, ICheckUserAccess checkUserAccess, IEsewaTransactionProcessing batchProcessing, IWebHostEnvironment hostingEnvironment, IEService eService)
        {
            this._loggedInUser = _loggedInUser;
            _logDetails = logDetails;
            _audit = audit;
            _checkUserAccess = checkUserAccess;
            _batchProcessing = batchProcessing;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _eService = eService;
        }
        public IActionResult Index()
        {
            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();
            ViewBag.UserRole = _loggedInUser.GetUserType();
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
            JsonResponse response = _batchProcessing.GetDividendList(CompCode, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                _audit.errorSave(_loggedInUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        //checking the status of the batch
        [HttpPost]
        public JsonResponse CheckBatchStatus(string CompCode, string DivCode,string BatchID)
        {
            JsonResponse res = _batchProcessing.CheckBatchStatus(CompCode, DivCode, BatchID, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
            if (res.HasError)
                res = _audit.errorSave(_loggedInUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), new Exception(res.Message));
            return res;
        }
        ///Transcation Batch Processing
        [HttpPost]
        public JsonResponse TransactionProcessing(string CompCode, string DivCode, string BatchID, string BankID)
        {
            JsonResponse jsonResponse = new JsonResponse();
            jsonResponse = _batchProcessing.TransactionProcessing(DivCode, CompCode, BatchID, BankID, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
            return jsonResponse;
        }

        [HttpPost]
        public JsonResponse GetSourceBankList(string Compcode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            jsonResponse = _batchProcessing.GetSourceBanks( Compcode,  _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
            return jsonResponse;
        }
        [HttpPost]
        public async Task<IActionResult> GetData(string CompCode, string DivCode, string BatchID, string BatchStatus)
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




    }
}
