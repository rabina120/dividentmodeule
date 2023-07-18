using CDSMODULE.Helper;
using Entity.Common;
using Interface.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace CDSMODULE.Areas.Common.Controllers
{
    [Authorize]
    [Area("Common")]
    [AutoValidateAntiforgeryToken]
    public class CommonController : Controller
    {
        private readonly ILoggedinUser _IloggedinUser;
        private readonly ICommon _ICommon;
        private readonly IShareType _IShare;
        private readonly IWebHostEnvironment _hosting;
        public CommonController(ILoggedinUser IloggedinUser, ICommon ICommon, IShareType ishare, IWebHostEnvironment hosting)
        {
            this._IloggedinUser = IloggedinUser;
            this._ICommon = ICommon;
            this._IShare = ishare;
            this._hosting = hosting;
        }

        public IActionResult Index()
        {
            ViewBag.UserName = _IloggedinUser.GetUserNameToDisplay();
            return View();
        }

        #region OwnerType
        [HttpPost]
        public JsonResponse GetShareOwnerType()
        {
            return _ICommon.GetShareOwnerType();
        }
        #endregion 

        #region  Get Share Type
        [HttpPost]
        public JsonResponse GetShareTypes(string compcode)
        {
            return _IShare.GetShareTypes(compcode);
        }
        #endregion
        [HttpPost]
        public JsonResponse GetExcelSheetNames()
        {

            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                var currentOLEDBVersion = ExcelConfig.GetOLEDBVersion();
                IFormFile file = Request.Form.Files[0];
                string folderName = "UploadExcel\\Common";
                string webRootPath = _hosting.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                    ISheet sheet;
                    string fullPath = Path.Combine(newPath, file.FileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
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
                            if (!currentOLEDBVersion.Contains(12)) throw new Exception(ATTMessages.EXCEL_UPLOAD.XLSX_NOT_SUPPORTED);
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

                    return jsonResponse;
                }
            }
            catch (Exception ex)
            {
                jsonResponse.Message = ex.Message;
            }
            return jsonResponse;
        }


    }
}
