using CDSMODULE.Helper;
using Entity.Common;
using Interface.Common;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CDSMODULE.Areas.Security.Controllers
{
    [Area("Security")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class RoleDefineController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IRoleDefine _roleDefine;
        public readonly ICheckUserAccess _checkUserAccess;
        public readonly IAudit _audit;

        public RoleDefineController(ILoggedinUser loggedInUser, IRoleDefine roleDefine, ICheckUserAccess checkUserAccess, IAudit audit)
        {
            _loggedInUser = loggedInUser;
            _roleDefine = roleDefine;
            _checkUserAccess = checkUserAccess;
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
        public JsonResponse SaveRole(string RoleName)
        {
            JsonResponse response = _roleDefine.SaveRole(RoleName, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }

    }
}
