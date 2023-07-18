using CDSMODULE.Helper;
using Entity.Common;
using ENTITY.Certificate;
using Interface.Certificate;
using Interface.Common;
using Interface.Security;
using Interface.ShareHolder;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Shareplus.Areas.ParameterSetup.Controllers
{
    [Area("ParameterSetup")]
    [AutoValidateAntiforgeryToken]
    public class PrintSetupController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IHolderInformation _holderInformation;
        private readonly ICertificateList _certificateList;
        private readonly IAudit _audit;
        private readonly IPrint _print;
        public PrintSetupController(IAudit audit, ICheckUserAccess checkUserAccess, ILoggedinUser _loggedInUser, IHolderInformation holderInformation, ICertificateList certificateList, IPrint print)
        {
            _checkUserAccess = checkUserAccess;
            this._loggedInUser = _loggedInUser;
            _audit = audit;
            _holderInformation = holderInformation;
            _certificateList = certificateList;
            _print = print;
        }
        public IActionResult Index()
        {
            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();

            string UserId = _loggedInUser.GetUserId();
            JsonResponse res = _audit.auditSave(_loggedInUser.GetUserId(), "Opened " + ControllerContext.RouteData.Values["controller"].ToString() + " Form", ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress());
            if (res.IsSuccess)
            {
                if (_checkUserAccess.CheckIfAccessible(UserId, ControllerContext))
                    return View();
                return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }


        [HttpPost]
        public object GetAllPrintFields()
        {
            JsonResponse response = new JsonResponse();
            response = _print.GetAllPrintFields();
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public object GetAllPrintFieldsWithCoordinates(string CompCode)
        {
            JsonResponse response = new JsonResponse();
            response = _print.GetAllPrintFieldsWithCoordinates(CompCode);
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public object SaveCompanyCoordinates(List<ATTCompanyCoordinates> cordList, string CompCode)
        {
            JsonResponse response = new JsonResponse();
            response = _print.SaveCompanyCoordinates(cordList, CompCode);
            return JsonConvert.SerializeObject(response);
        }


    }
}
