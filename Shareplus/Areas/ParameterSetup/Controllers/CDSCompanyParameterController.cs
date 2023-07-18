using CDSMODULE.Helper;

using Entity.Common;

using Interface.Common;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;


using System.Collections.Generic;

using Newtonsoft.Json;
using INTERFACE.Parameter;
using ENTITY.Parameter;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace Shareplus.Areas.DividendManagement.Controllers
{
    [Authorize]
    [Area("ParameterSetup")]
    [AutoValidateAntiforgeryToken]
    public class CDSCompanyParameterController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private IWebHostEnvironment Environment;
        private IConfiguration Configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IOptions<ReadConfig> connectionString;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        //rivate readonly IPledgeRelease _pledgeRelease;
        private readonly ICDSCompanyParameter _cdsCompanyParameter;

        public CDSCompanyParameterController(
            IOptions<ReadConfig> _connectionString,
            IWebHostEnvironment Environment,
            IConfiguration Configuration,
            ILoggedinUser loggedinUser,
            ICheckUserAccess checkUserAccess,
            IAudit audit,
            IWebHostEnvironment webHostEnvironment,
          //  IPledgeRelease pledgeRelease
          ICDSCompanyParameter cdsCompanyParameter)

        {
            this.connectionString = _connectionString;
            this.Environment = Environment;
            this.Configuration = Configuration;
            _loggedInUser = loggedinUser;
            this._loggedInUser = loggedinUser;
            this._checkUserAccess = checkUserAccess;
            _audit = audit;
            _webHostEnvironment = webHostEnvironment;
            // _pledgeRelease = pledgeRelease;
            _cdsCompanyParameter = cdsCompanyParameter;


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

        //uploading to Database

        public JsonResponse GetSheetNames(string CompCode, string DivCode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                List<double> currentOLEDBVersion = ExcelConfig.GetOLEDBVersion();

                IFormFile file = Request.Form.Files[0];
                string folderName = "UploadExcel\\PledgeRelease";
                string webRootPath = _webHostEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                    ISheet sheet;
                    string fullPath = Path.Combine(newPath, "CompCode_" + CompCode + "_DivCode_" + DivCode + "_" + DateTime.Now.ToString("yyyy_MM_dd") + sFileExtension);
                    System.IO.File.Delete(fullPath);
                    using (var stream = new FileStream(fullPath, FileMode.CreateNew))
                    {
                        file.CopyTo(stream);
                        stream.Position = 0;
                        IWorkbook workbook = null;
                        if (sFileExtension == ".xls")
                        {
                            workbook = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                            int name = workbook.NumberOfSheets;
                            List<string> sheetName = new List<string>();
                            for (int i = 1; i <= name; i++)
                            {
                                sheetName.Add(workbook.GetSheetName(i - 1));

                            }
                            if (sheetName.Count > 0)
                            {
                                jsonResponse.IsSuccess = true;
                                jsonResponse.ResponseData = sheetName;
                            }
                            sheet = workbook.GetSheetAt(0);
                        }
                        else
                        {
                            //if (currentOLEDBVersion <= 12.0) throw new Exception(ATTMessages.EXCEL_UPLOAD.XLSX_NOT_SUPPORTED);
                            workbook = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                            int name = workbook.NumberOfSheets;
                            List<string> sheetName = new List<string>();
                            for (int i = 1; i <= name; i++)
                            {
                                sheetName.Add(workbook.GetSheetName(i - 1));

                            }
                            if (sheetName.Count > 0)
                            {
                                jsonResponse.IsSuccess = true;
                                jsonResponse.ResponseData = sheetName;
                            }
                            sheet = workbook.GetSheetAt(0); //get first sheet from workbook
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                jsonResponse.IsSuccess = false;
                jsonResponse.HasError = true;
                jsonResponse.ResponseData = ex;
            }
            return jsonResponse;
        }


        [HttpPost]
        public object SaveCDSCompanyParameterList(string compCode, List<ATTCDSCompanyParameter> ParamList)
        {
            JsonResponse response = new JsonResponse();
            // response = _pledgeRelease.SaveCDSCompanyParameterList(compCode, isinNo, shholdernNo, shownerType, description);
            response = _cdsCompanyParameter.SaveCDSCompanyParameterList(compCode, ParamList, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            return JsonConvert.SerializeObject(response);
        }

        public object GetCDSCompanyParameter(string compcode)
        {
            JsonResponse response = new JsonResponse();

            response = _cdsCompanyParameter.GetCDSCompanyParameter(compcode);
            return (response);
        }

        public object UpdateCDSCompanyParameter(string compCode, ATTCDSCompanyParameter cdsParameter)
        {
            JsonResponse response = new JsonResponse();
            //response = _cdsCompanyParameter.UpdateCDSCompanyParameter(cdsParameter);
            return (response);
        }

    }
}
