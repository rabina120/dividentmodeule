using CDSMODULE.Helper;
using Entity.Common;
using ENTITY.DemateDividend;
using Interface.Common;
using Interface.Security;
using INTERFACE.DividendManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Shareplus.Areas.DividendManagement.Controllers
{
    [Authorize]
    [Area("DividendManagement")]
    [AutoValidateAntiforgeryToken]
    public class PoolAccountSplitPostingController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private IWebHostEnvironment Environment;
        private IConfiguration Configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IOptions<ReadConfig> connectionString;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IPoolAccountSplit _poolAccSplit;
        private readonly IAudit _audit;

        public PoolAccountSplitPostingController(
            IOptions<ReadConfig> _connectionString,
            IWebHostEnvironment Environment,
            IConfiguration Configuration,
            ILoggedinUser loggedinUser,
            ICheckUserAccess checkUserAccess,
            IAudit audit,
            IPoolAccountSplit poolAccSplit,
            IWebHostEnvironment webHostEnvironment
            )
        {
            this.connectionString = _connectionString;
            this.Environment = Environment;
            this.Configuration = Configuration;
            _loggedInUser = loggedinUser;
            this._loggedInUser = loggedinUser;
            this._checkUserAccess = checkUserAccess;
            _audit = audit;
            _poolAccSplit = poolAccSplit;
            _webHostEnvironment = webHostEnvironment;

        }
        public IActionResult Index()
        {
            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();

            string UserId = _loggedInUser.GetUserId();
            JsonResponse res = _audit.auditSave(_loggedInUser.GetUserId(), "Opened " + this.ControllerContext.RouteData.Values["controller"].ToString() + " Form", this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress());
            if (res.IsSuccess)
            {
                if (_checkUserAccess.CheckIfAccessible(UserId, this.ControllerContext))
                    return View();
                return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }

        [HttpPost]
        public object GetSplitPostingList(string CompCode, string DivCode)
        {
            JsonResponse response = new JsonResponse();
            response = _poolAccSplit.GetSplitPostingList(CompCode, DivCode, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress(), null);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public object GetSplitPostingDetailList(string CompCode, string DivCode, int? ParentId)
        {
            JsonResponse response = new JsonResponse();
            response = _poolAccSplit.GetSplitPostingList(CompCode, DivCode, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress(), ParentId);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public object SubmitForPosting(List<ATTSplit> PostingList, string PostingDate, string Remarks, string Action)
        {
            JsonResponse response = new JsonResponse();
            response = _poolAccSplit.SubmitForPostring(PostingList, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress(),Action, PostingDate, Remarks);
            return JsonConvert.SerializeObject(response);
        }
    }
}
