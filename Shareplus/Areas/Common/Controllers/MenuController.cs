using CDSMODULE.Helper;
using Entity.Common;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CDSMODULE.Areas.Common.Controllers
{
    [Authorize]
    [Area("Common")]
    [AutoValidateAntiforgeryToken]
    public class MenuController : Controller
    {
        private readonly ILoggedinUser IloggedinUser;
        private readonly IUserMenu IuserMenu;
        private readonly IAudit _audit;

        public MenuController(ILoggedinUser _IloggedinUser, IUserMenu _IuserMenu, IAudit audit)
        {
            this.IloggedinUser = _IloggedinUser;
            this.IuserMenu = _IuserMenu;
            _audit = audit;
        }

        public JsonResponse Index()
        {
            var UserId = IloggedinUser.GetUserNameToDisplay();
            var RoleId = IloggedinUser.GetUserRole();
            JsonResponse json = IuserMenu.GetUserMenu(RoleId);
            json.IsValid = IloggedinUser.GetConnectedCompany().CompCode != null;
            json.IsToken = IloggedinUser.IsLDAP();
            return json;
        }

        [HttpPost]
        public JsonResponse GetMenuForUser(string RoleId)
        {
            JsonResponse response = IuserMenu.GetUserMenu(RoleId);
            if (response.HasError)
            {
                response = _audit.errorSave(IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }

        [HttpPost]
        public JsonResponse GetMenuList()
        {
            JsonResponse response = IuserMenu.GetMenuList();
            if (response.HasError)
            {
                response = _audit.errorSave(IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }

        [HttpPost]
        public JsonResponse GetMenuByRole(int RoleId)
        {
            JsonResponse response = IuserMenu.GetMenuByRole(RoleId, IloggedinUser.GetUserNameToDisplay(), IloggedinUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
    }
}
