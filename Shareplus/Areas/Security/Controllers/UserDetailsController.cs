using CDSMODULE.Helper;
using Entity.Common;
using Entity.Reports;
using Entity.Security;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
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
    public class UserDetailsController : Controller
    {
        private readonly ILoginUser loginUser;
        private readonly ILoggedinUser IloggedinUser;
        private readonly IUserDetails IuserDetails;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        private readonly IUserReport _userReport;
        private readonly IGenericReport _genericReport;
        public UserDetailsController(ILoginUser _loginUser, ILoggedinUser _IloggedinUser, IUserDetails userDetails, ICheckUserAccess checkUserAccess, IAudit audit, IUserReport userReport, IGenericReport genericReport)
        {
            this.loginUser = _loginUser;
            this.IloggedinUser = _IloggedinUser;
            this.IuserDetails = userDetails;
            _checkUserAccess = checkUserAccess;
            _audit = audit;
            _userReport = userReport;
            _genericReport = genericReport;
        }
        public IActionResult Index()
        {
            ViewBag.UserName = IloggedinUser.GetUserNameToDisplay();
            string UserId = IloggedinUser.GetUserId(); 
            Entity.Common.JsonResponse res = _audit.auditSave(IloggedinUser.GetUserNameToDisplay(), "Opened " + this.ControllerContext.RouteData.Values["controller"].ToString() + " Form", this.ControllerContext.RouteData.Values["controller"].ToString(), IloggedinUser.GetUserIPAddress());
            if (res.IsSuccess)
            {
                if (_checkUserAccess.CheckIfAccessible(UserId, this.ControllerContext))
                    return View();
                return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }

        public Entity.Common.JsonResponse GetAllRoles()
        {
            Entity.Common.JsonResponse response = IuserDetails.GetAllRoles();
            if (response.HasError)
            {
                response = _audit.errorSave(IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }

        #region API Calls
        [HttpPost]
        public string GetAllUsers()
        {
            return JsonConvert.SerializeObject(new { data = IuserDetails.GetAllUsers(IloggedinUser.GetUserNameToDisplay(), IloggedinUser.GetUserIPAddress()) });
        }

        [HttpPost]
        public Entity.Common.JsonResponse DisableUserById(string id)
        {
            return IuserDetails.EnableDisableUserById(id, IloggedinUser.GetUserNameToDisplay());
        }
        [HttpPost]
        public Entity.Common.JsonResponse EnableUserById(string id)
        {
            return IuserDetails.EnableDisableUserById(id, IloggedinUser.GetUserNameToDisplay(),true);
        }

        [HttpPost]
        public Entity.Common.JsonResponse EditUserById(string id)
        {
            JsonResponse res = IuserDetails.EditUserById(id);
            if (res.IsSuccess)
            {
                res.IsValid = IloggedinUser.IsLDAP();
                _audit.auditSave(IloggedinUser.GetUserNameToDisplay(), "GetUserDetails of ID: " + id + " For Edit", this.ControllerContext.RouteData.Values["controller"].ToString(), IloggedinUser.GetUserIPAddress());
            }
            return res;

        }


        [HttpPost]
        public Entity.Common.JsonResponse UpdateUserDetails(string id, ATTUserProfile UserProfile)
        {
            if (UserProfile.Password != null) UserProfile.Password = Crypto.OneWayEncryter(UserProfile.Password);
            return IuserDetails.UpdateUserDetails(IloggedinUser.GetUserNameToDisplay(), id, UserProfile, IloggedinUser.GetUserIPAddress());
        }

        [HttpPost]
        public Entity.Common.JsonResponse GetUserRights(string UserID)
        {
            Entity.Common.JsonResponse response = IuserDetails.GetUserRights(UserID, IloggedinUser.GetUserNameToDisplay(), IloggedinUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
        [HttpPost]
        public Entity.Common.JsonResponse GetAuditReport(string reportType, string fromDate, string toDate, string userId)
        {
            Entity.Common.JsonResponse response = _userReport.GetUserDetailsAuditReport(reportType, fromDate, toDate, userId, IloggedinUser.GetUserNameToDisplay(), IloggedinUser.GetUserIPAddress());
            string[] reportTitles = { IloggedinUser.GetConnectedCompany().CompCode, IloggedinUser.GetConnectedCompany().CompEnName, "User Role Audit Report" };

            if (response.HasError)
            {
                response = _audit.errorSave(IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), IloggedinUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            if (response.IsSuccess)
            {
                if (reportType == "excel")
                {
                    GenericExcelReport Excelreport = new GenericExcelReport();
                    response = Excelreport.GenerateExcelReport(response, "UserRoleAuditReport - " + DateTime.Now.ToString("yyyy - MM - dd"), "", "SCB", "001", null);
                    if (response.IsSuccess)
                        response.Message = "UserRoleAuditReport - " + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";

                }
                else if (reportType == "pdf")
                {
                    response = _genericReport.GenerateReport(ATTGenericReport.ReportName.UserRoleUpdateReport, response, reportTitles);

                    if (response.IsSuccess)
                    {

                        response.Message = "SecurityMatrixReport-" + DateTime.Now.ToString("yyyy-MM-dd") + ".pdf";

                    }
                }
            }
            return response;
        }
        #endregion
    }
}
