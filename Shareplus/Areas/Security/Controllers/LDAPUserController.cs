using CDSMODULE.Helper;
using Entity.Common;
using Entity.Security;
using Interface.Common;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;

namespace CDSMODULE.Areas.Security.Controllers
{
    [Authorize]
    [Area("Security")]
    [AutoValidateAntiforgeryToken]
    public class LDAPUserController : Controller
    {
        private readonly ILoginUser _loginUser;
        private readonly ILoggedinUser _IloggedinUser;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        private readonly IConfiguration _congifuration;

        public LDAPUserController(ILoginUser loginUser, ILoggedinUser _IloggedinUser, ICheckUserAccess checkUserAccess, IAudit audit, IConfiguration congifuration)
        {
            this._loginUser = loginUser;
            this._IloggedinUser = _IloggedinUser;
            _checkUserAccess = checkUserAccess;
            _audit = audit;
            _congifuration = congifuration;
        }
        public IActionResult Index()
        {
            ViewBag.UserName = _IloggedinUser.GetUserNameToDisplay();
            string UserId = _IloggedinUser.GetUserId();
            JsonResponse res = _audit.auditSave(_IloggedinUser.GetUserNameToDisplay(), "Opened " + this.ControllerContext.RouteData.Values["controller"].ToString() + " Form", this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress());
            if (res.IsSuccess)
            {
                string isLDAP = _congifuration.GetSection("LDAPAuthentication").GetSection("Enabled").Value;
                if (isLDAP.ToLower() != "true")
                    return RedirectToAction("Index", "User", new { area = "Security" });
                if (_checkUserAccess.CheckIfAccessible(UserId, this.ControllerContext))
                    return View();
                return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }
        [HttpPost]
        public string CreateUser(ATTUserProfile aTTUserProfile)
        {
            JsonResponse jsonResponse = _loginUser.LdapAuthentication(aTTUserProfile.UserId.ToString(), aTTUserProfile.Password);
            if (!jsonResponse.IsSuccess) { return JsonConvert.SerializeObject(jsonResponse); }

            aTTUserProfile.CreatedBy = _IloggedinUser.GetUserNameToDisplay();
            aTTUserProfile.Password = Crypto.OneWayEncryter(aTTUserProfile.Password);
            JsonResponse response = _loginUser.CreateUser(aTTUserProfile, _IloggedinUser.GetUserNameToDisplay(), _IloggedinUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public string UpdateUser(ATTUserProfile aTTUserProfile)
        {
            JsonResponse jsonResponse = new JsonResponse();
            jsonResponse = _loginUser.UpdateUser(aTTUserProfile, _IloggedinUser.GetUserNameToDisplay(), _IloggedinUser.GetUserIPAddress());
            if (jsonResponse.HasError) _audit.errorSave(_IloggedinUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)jsonResponse.ResponseData);
            return JsonConvert.SerializeObject(jsonResponse);
        }
        [HttpPost]
        public string GetUserType()
        {
            return JsonConvert.SerializeObject(_loginUser.GetUserType());

        }
        [HttpPost]
        public string GetUserStatus()
        {
            return JsonConvert.SerializeObject(_loginUser.GetUserStatus());

        }
        [HttpPost]
        public string GetUserRole()
        {
            return JsonConvert.SerializeObject(_loginUser.GetUserRole());

        }

    }
}
