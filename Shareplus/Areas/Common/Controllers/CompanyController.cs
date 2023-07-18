using CDSMODULE.Helper;
using Entity.Common;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace CDSMODULE.Areas.Common.Controllers
{
    [Authorize]
    [Area("Common")]
    [AutoValidateAntiforgeryToken]
    public class CompanyController : Controller
    {
        private readonly ILoggedinUser _IloggedinUser;
        private readonly ICompanyDetails _ICompanyDetails;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        public CompanyController(ILoggedinUser IloggedinUser, ICompanyDetails IcompanyDetails, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            this._IloggedinUser = IloggedinUser;
            this._ICompanyDetails = IcompanyDetails;
            this._httpContextAccessor = httpContextAccessor;
            this._configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewBag.UserName = _IloggedinUser.GetUserNameToDisplay();
            return View();
        }


        #region GetCompanyDetails
        [HttpPost]
        public JsonResponse GetCompanyDetails()
        {
            return _ICompanyDetails.GetCompanyDetails(null);
        }


        public JsonResponse GetCompanyDetailsByCompanyCode(string CompCode)
        {
            return _ICompanyDetails.GetCompanyDetails(CompCode);
        }
        #endregion

        #region Get Compnay Information From Syatem
        [HttpPost]
        public JsonResponse GetConnectedCompany()
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                ATTCompany company = _httpContextAccessor.HttpContext.Session.GetConnectedCompany();
                jsonResponse.ResponseData = company;
                jsonResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                jsonResponse.IsSuccess = false;
                jsonResponse.Message = "Company Not Selected";
            }
            return jsonResponse;
        }
        #endregion

        #region Load Company Information to Session
        [HttpPost]
        public JsonResponse ConnectCompany(string Compcode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            jsonResponse = _ICompanyDetails.GetCompanyDetails(Compcode);
            var isLDAP = _IloggedinUser.IsLDAP();
            try
            {
                _httpContextAccessor.HttpContext.Session.ConnectCompany(((List<ATTCompany>)jsonResponse.ResponseData)[0]);
            }
            catch (Exception ex)
            {
                jsonResponse.Message = "Failed To Load Company Data";
            }
            jsonResponse.IsValid = isLDAP;
            return jsonResponse;
        }
        #endregion
        #region Disconnect Company
        [HttpPost]
        public JsonResponse DisconnectCompany()
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                _httpContextAccessor.HttpContext.Session.DisconnectCompany();
                jsonResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                jsonResponse.Message = "Failed To Disconnect Company";
            }
            return jsonResponse;
        }
        #endregion
    }
}
