using CDSMODULE.Helper;
using Entity.Common;
using Entity.Reports;
using Interface.Common;
using Interface.Reports;
using Interface.Security;
using INTERFACE.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repository.Reports;
using System;

namespace CDSMODULE.Areas.Security.Controllers
{
    [Area("Security")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class UserRightController : Controller
    {
        private readonly ILoginUser _loginUser;
        private readonly ILoggedinUser IloggedInUser;
        private readonly IUserMenu IuserMenu;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        private readonly IGenericReport _genericReport;
        private readonly IUserDetails _iuserDetails;
        private readonly IUserReport _userReport;


        public UserRightController(ILoginUser _loginUser, ILoggedinUser _IloggedinUser, IUserMenu _IuserMenu, ICheckUserAccess checkUserAccess, IAudit audit, IGenericReport genericReport, IUserDetails userDetails, IUserReport userReport)
        {
            this._loginUser = _loginUser;
            this.IloggedInUser = _IloggedinUser;
            this.IuserMenu = _IuserMenu;
            this._checkUserAccess = checkUserAccess;
            _audit = audit;
            _genericReport = genericReport;
            _iuserDetails = userDetails;
            _userReport = userReport;
        }

        public IActionResult Index()
        {
            ViewBag.UserName = IloggedInUser.GetUserNameToDisplay();
            string UserId = IloggedInUser.GetUserId(); 
            JsonResponse res = _audit.auditSave(IloggedInUser.GetUserNameToDisplay(), "Opened " + this.ControllerContext.RouteData.Values["controller"].ToString() + " Form", this.ControllerContext.RouteData.Values["controller"].ToString(), IloggedInUser.GetUserIPAddress());
            if (res.IsSuccess)
            {
                if (_checkUserAccess.CheckIfAccessible(UserId, this.ControllerContext))
                    return View();
                return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }

        [HttpPost]
        public JsonResponse GetMenuList()
        {
            JsonResponse response = IuserMenu.GetMenuList();
            if (response.HasError)
            {
                response = _audit.errorSave(IloggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), IloggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }

        [HttpPost]
        public string AddRights(string userId, string[] menuList, string addUpdate)
        {
            JsonResponse response = IuserMenu.AddRights(userId, menuList, addUpdate, IloggedInUser.GetUserNameToDisplay());
            if (response.HasError)
            {
                response = _audit.errorSave(IloggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), IloggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }

            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public string AddRightsByRole(string RoleId, string[] menuList, string addUpdate)
        {
            JsonResponse response = IuserMenu.AddRightsByRole(RoleId, menuList, addUpdate, IloggedInUser.GetUserNameToDisplay(), IloggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(IloggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), IloggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }

            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public JsonResponse RoleReport(int roleId, string actionType)
        {
            JsonResponse response = new JsonResponse();
            string[] reportTitles = { IloggedInUser.GetConnectedCompany().CompCode, IloggedInUser.GetConnectedCompany().CompEnName, "Security Matrix Report" };
            if (actionType == "pdf")
            {
                response = _userReport.GetSecurityMatrixReport(roleId, IloggedInUser.GetUserNameToDisplay(), IloggedInUser.GetUserIPAddress());

                response = _genericReport.GenerateReport(ATTGenericReport.ReportName.SecurityMatrixForSCB, response, reportTitles);

                if (response.IsSuccess)
                {

                    response.Message = "SecurityMatrixReport-" + DateTime.Now.ToString("yyyy-MM-dd") + ".pdf";

                }
            }
            else if (actionType == "excel")
            {
                response = _userReport.GetSecurityMatrixReportForExcel(roleId, IloggedInUser.GetUserNameToDisplay(), IloggedInUser.GetUserIPAddress());

                if (response.IsSuccess)
                {

                    GenericExcelReport Excelreport = new GenericExcelReport();
                    response = Excelreport.GenerateExcelReport(response, "SecurityMatrixReport - " + DateTime.Now.ToString("yyyy - MM - dd"), "", "SCB", "001", null);
                    response.Message = "SecurityMatrixReport - " + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";

                }
            }

            else
            {
                response.Message = "Select a Report Type";
            }
            return response;
        }

    }
}
