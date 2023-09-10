using CDSMODULE.Helper;
using ClosedXML.Excel;
using Entity.Common;
using ExcelDataReader;
using Interface.Common;
using Interface.Security;
using INTERFACE.DividendManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Shareplus.Areas.DividendManagement.Controllers
{
    [Authorize]
    [Area("DividendManagement")]
    [AutoValidateAntiforgeryToken]
    public class PoolAccountSplitController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private IWebHostEnvironment Environment;
        private IConfiguration Configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IOptions<ReadConfig> connectionString;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IPoolAccountSplit _poolAccSplit;
        private readonly IAudit _audit;

        public PoolAccountSplitController(
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
        public object GetHolderInfoForSplit(string CompCode, string BOID, string DivCode, string Action)
        {
            JsonResponse response = new JsonResponse();
            var UserName = _loggedInUser.GetUserName();
            var IPAddress = _loggedInUser.GetUserIPAddress();
            response = _poolAccSplit.GetHolderInfoForSplit(BOID, UserName, CompCode, IPAddress, DivCode, Action);
            return JsonConvert.SerializeObject(response);
        }
        [HttpGet]
        public ActionResult DownloadExcelDocument()
        {
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "DummyExcel PoolAccountSplit.xlsx";
            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("DataFormat");
                worksheet.Cell(1, 1).Value = "Boid";
                worksheet.Cell(1, 2).Value = "name";
                worksheet.Cell(1, 3).Value = "faname";
                worksheet.Cell(1, 4).Value = "grfaname";
                worksheet.Cell(1, 5).Value = "totalkitta";
                worksheet.Cell(1, 6).Value = "tfrackitta";
                worksheet.Cell(1, 7).Value = "actualbonus";
                //worksheet.Cell(1, 8).Value = "abwithprevfrac";
                //worksheet.Cell(1, 9).Value = "issuebonus";
                worksheet.Cell(1, 8).Value = "remfrac";
                worksheet.Cell(1, 9).Value = "divamount";
                worksheet.Cell(1, 10).Value = "divtax";
                worksheet.Cell(1, 11).Value = "bonustax";
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }
        }

        [HttpPost]
        public JsonResponse GetSheetNames(string CompCode, string DivCode, string BOID)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                var currentOLEDBVersion = ExcelConfig.GetOLEDBVersion();

                IFormFile file = Request.Form.Files[0];
                string folderName = "UploadExcel\\PoolAccountSplit";
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
                    string fullPath = Path.Combine(newPath, "CompCode_" + CompCode + "_DivCode_" + DivCode + "_BOID_" + BOID + "_" + DateTime.Now.ToString("yyyy_MM_dd") + sFileExtension);
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
                    return jsonResponse;
                }
            }
            catch (Exception ex)
            {
                jsonResponse.Message = ex.Message;
            }
            return jsonResponse;
        }

        [HttpPost]
        //uploading to Database
        public JsonResponse UploadSheet(int SheetId, string CompCode, string DivCode, string BOID, string ActionType)
        {
            JsonResponse jsonResponse = new JsonResponse();
            if(ActionType == "A")
            {
                IFormFile postedFile = Request.Form.Files[0];
                DataTable dt1 = new DataTable();
                if (SheetId != null)
                {
                    try
                    {
                        // Create a Folder.
                        string path = Path.Combine(this._webHostEnvironment.WebRootPath, "UploadExcel\\PoolAccountSplit");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        //Save the uploaded Excel file.
                        string extension = Path.GetExtension(postedFile.FileName);
                        string fileName = "CompCode_" + CompCode + "_DivCode_" + DivCode + "_BOID_" + BOID + "_" + DateTime.Now.ToString("yyyy_MM_dd") + extension;
                        //Path.Combine(newPath, "CompCode_" + CompCode + "_DivCode_" + DivCode + "_BOID_" + BOID + "_" + DateTime.Now.ToString("yyyy_MM_dd") + ".xlsx");
                        string excelFilePath = Path.Combine(path, fileName);

                        //Read the connection string for the Excel file.
                        string conString = string.Empty;
                        if (extension == ".xls")
                        {
                            //This will read the Excel 97-2000 formats 
                            conString = this.Configuration.GetConnectionString("ExcelConStringV4");
                        }
                        else
                        {
                            ////This will read 2007 Excel format 
                            conString = this.Configuration.GetConnectionString("ExcelConStringV12");
                        }
                        using (var stream = new FileStream(excelFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                            IExcelDataReader reader = null;
                            if (excelFilePath.EndsWith(".xls"))
                            {
                                reader = ExcelReaderFactory.CreateBinaryReader(stream);
                            }
                            else if (excelFilePath.EndsWith(".xlsx"))
                            {
                                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                            }

                            if (reader == null)
                                return null;

                            var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true
                                }
                            });

                            #region to insert
                            //Insert the Data read from the Excel file to Database Table.
                            conString = Configuration.GetConnectionString("DefaultConnection");
                            DataTable dt = ds.Tables[SheetId];
                            foreach (DataRow row in dt.Rows)
                            {
                                if (row["Boid"] == null || row["name"].ToString() == "")
                                {
                                    throw new Exception("Boid and Name cannot be empty!!!");
                                }
                                else
                                {
                                    row["tfrackitta"] = row["tfrackitta"] == null ? 0.00 : row["tfrackitta"];
                                    row["actualbonus"] = row["actualbonus"] == null ? 0.00 : row["actualbonus"];
                                    row["remfrac"] = row["remfrac"] == null ? 0.00 : row["remfrac"];
                                    row["divamount"] = row["divamount"] == null ? 0.00 : row["divamount"];
                                    row["divtax"] = row["divtax"] == null ? 0.00 : row["divtax"];
                                    row["bonustax"] = row["bonustax"] == null ? 0.00 : row["bonustax"];
                                }
                            }
                            //dt.Columns.Add(new DataColumn("compcode", typeof(string)));
                            //dt.Columns.Add(new DataColumn("divcode", typeof(string)));
                            //dt.Columns.Add(new DataColumn("boid_old", typeof(string)));
                            //dt.Columns.Add(new DataColumn("warrantno_new", typeof(string)));
                            //dt.Columns.Add(new DataColumn("warrantno", typeof(string)));
                            //dt.Columns.Add(new DataColumn("split_id", typeof(string)));
                            //dt.Columns.Add(new DataColumn("entryuser", typeof(string)));
                            //dt.Columns.Add(new DataColumn("entrydate", typeof(string)));

                            //foreach (DataRow row in dt.Rows)
                            //{
                            //    row["compcode"] = CompCode.Substring(0, 3);
                            //    row["divcode"] = DivCode.Substring(0, 3);
                            //    row["boid_old"] = BOID;
                            //    row["entryuser"] = _loggedInUser.GetUserName();
                            //    row["entrydate"] = DateTime.Now.ToString("yyyy-MM-dd");

                            //}
                            //caliing repoo
                            jsonResponse = _poolAccSplit.UploadPASData(dt, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress(), CompCode, DivCode, BOID, ActionType);
                            if (jsonResponse.HasError)
                                _audit.errorSave(_loggedInUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)jsonResponse.ResponseData);
                            return jsonResponse;
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = ex.Message;
                        _audit.errorSave(_loggedInUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), ex);
                    }
                }
            }
            else
            {
                try
                {
                    jsonResponse = _poolAccSplit.UploadPASData(null, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress(), CompCode, DivCode, BOID, ActionType);
                    if (jsonResponse.HasError)
                        _audit.errorSave(_loggedInUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)jsonResponse.ResponseData);
                    return jsonResponse;
                }
                catch(Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;
                    _audit.errorSave(_loggedInUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), ex);
                }
            }
            


            
            return jsonResponse;
        }
    }
}
