﻿using CDSMODULE.Helper;
using ClosedXML.Excel;
using Entity.Common;
using Entity.Esewa;
using ExcelDataReader;
using Interface.Common;
using Interface.Esewa;
using Interface.Security;
using Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace CDSMODULE.Areas.ESewaTransaction.Controllers
{
    [Authorize]
    [Area("ESewaTransaction")]
    [AutoValidateAntiforgeryToken]
    public class BatchProcessingController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly ILogDetails _logDetails;
        private readonly IAudit _audit;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IBatchProcessing _batchProcessing;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IEService _eService;
        private IConfiguration _configuration;

        public BatchProcessingController(ILoggedinUser _loggedInUser, ILogDetails logDetails, IConfiguration configuration,
            IAudit audit, ICheckUserAccess checkUserAccess, IBatchProcessing batchProcessing, IWebHostEnvironment hostingEnvironment, IEService eService)
        {
            this._loggedInUser = _loggedInUser;
            _logDetails = logDetails;
            _audit = audit;
            _checkUserAccess = checkUserAccess;
            _batchProcessing = batchProcessing;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _eService = eService;
        }
        public IActionResult Index()
        {
            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();
            ViewBag.UserRole = _loggedInUser.GetUserType();
            string UserId = _loggedInUser.GetUserId(); 
            JsonResponse res = _audit.auditSave(_loggedInUser.GetUserName(), "Opened " + this.ControllerContext.RouteData.Values["controller"].ToString() + " Form", this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress());
            if (res.IsSuccess)
            {
                if (_checkUserAccess.CheckIfAccessible(UserId, this.ControllerContext))
                    return View();
                return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }


        //checking the status of the batch
        [HttpPost]
        public JsonResponse CheckBatchStatus(string CompCode, string DivCode, string BatchId)
        {
            JsonResponse res = _batchProcessing.CheckBatchStatus(CompCode, DivCode, BatchId, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
            if (res.HasError)
                res = _audit.errorSave(_loggedInUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)res.ResponseData);
            return res;
        }


        //for get list of active batch
        [HttpPost]
        public JsonResponse GetAllActiveBatch(string CompCode, String DivCode)
        {
            JsonResponse res = _batchProcessing.GetAllActiveBatch(CompCode, DivCode, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
            if (res.HasError)
                res = _audit.errorSave(_loggedInUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)res.ResponseData);
            return res;
        }
        //for creating a new batch
        [HttpPost]
        public JsonResponse CreateBatch(string CompCode, string DivCode)
        {
            JsonResponse res = _batchProcessing.CreateBatch(CompCode, DivCode, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
            if (res.HasError)
                res = _audit.errorSave(_loggedInUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)res.ResponseData);
            return res;
        }

        [HttpGet]
        public ActionResult DownloadExcelDocument()
        {
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "DummyDataCDSImport.xlsx";
            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet =
                workbook.Worksheets.Add("DummyData");
                worksheet.Cell(1, 1).Value = "fullname";
                worksheet.Cell(1, 2).Value = "boid";
                worksheet.Cell(1, 3).Value = "faname";
                worksheet.Cell(1, 4).Value = "grfaname";
                worksheet.Cell(1, 5).Value = "address";
                worksheet.Cell(1, 6).Value = "bankcode";
                worksheet.Cell(1, 7).Value = "bankaccno";
                worksheet.Cell(1, 8).Value = "bankname";
                worksheet.Cell(1, 9).Value = "citno";
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }
        }
        #region Excel Upload 
        //getting the name of the sheets 
        [HttpPost]
        public JsonResponse GetSheetNames(string CompCode, string DivCode, string BatchID)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                var currentOLEDBVersion = ExcelConfig.GetOLEDBVersion();

                IFormFile file = Request.Form.Files[0];
                string folderName = "UploadExcel\\CDSImport";
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                    ISheet sheet;
                    string fullPath = Path.Combine(newPath, "CompCode_" + CompCode + "_DivCode_" + DivCode + "_BatchId_" + BatchID + "_" + DateTime.Now.ToString("yyyy_MM_dd") + ".xlsx");
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
                            //if (!currentOLEDBVersion.Contains(12)) throw new Exception(ATTMessages.EXCEL_UPLOAD.XLSX_NOT_SUPPORTED);
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

        
        static double GetOLEDBVersion()
        {
            var reader = OleDbEnumerator.GetRootEnumerator();

            var OLEDBVersion = string.Empty;
            while (reader.Read())
            {
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.GetName(i) == "SOURCES_NAME")
                    {
                        OLEDBVersion = reader.GetValue(i).ToString();
                    }
                }

            }
            reader.Close();
            Double.TryParse(Regex.Match(OLEDBVersion, @"\d+\.*\d*").Value, out double version);
            return version;
        }
        [HttpPost]
        #endregion

        [HttpPost]
        //uploading to Database
        public async Task<JsonResponse> UploadSheet(int SheetId, string CompCode, string DivCode, string BatchID)
        {
            JsonResponse jsonResponse = new JsonResponse();

            IFormFile postedFile = Request.Form.Files[0];
            DataTable dt1 = new DataTable();


            if (SheetId != null)
            {
                try
                {
                    // Create a Folder.
                    string path = Path.Combine(this._hostingEnvironment.WebRootPath, "UploadExcel\\CDSImport");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    //Save the uploaded Excel file.
                    string fileName = "CompCode_" + CompCode + "_DivCode_" + DivCode + "_BatchId_" + BatchID + "_" + DateTime.Now.ToString("yyyy_MM_dd") + ".xlsx";
                    string excelFilePath = Path.Combine(path, fileName);
                    string extension = Path.GetExtension(postedFile.FileName);

                    //Read the connection string for the Excel file.
                    string conString = string.Empty;
                    if (extension == ".xls")
                    {
                        //This will read the Excel 97-2000 formats 
                        conString = this._configuration.GetConnectionString("ExcelConStringV4");
                    }
                    else
                    {
                        ////This will read 2007 Excel format 
                        conString = this._configuration.GetConnectionString("ExcelConStringV12");
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
                        
                        DataTable dt = ds.Tables[SheetId];
                        dt.Columns.Add(new DataColumn("compcode", typeof(string)));
                        dt.Columns.Add(new DataColumn("divcode", typeof(string)));
                        dt.Columns.Add(new DataColumn("batchid", typeof(string)));
                        dt.Columns.Add(new DataColumn("entryuser", typeof(string)));
                        dt.Columns.Add(new DataColumn("entrydate", typeof(string)));

                        foreach (DataRow row in dt.Rows)
                        {
                            row["compcode"] = CompCode.Substring(0, 3);
                            row["divcode"] = DivCode.Substring(0, 3);
                            row["batchid"] = BatchID;
                            row["entryuser"] = _loggedInUser.GetUserName();
                            row["entrydate"] = DateTime.Now.ToString("yyyy-MM-dd");
                        }
                        //caliing repoo
                        jsonResponse = _batchProcessing.UploadCDSData(dt, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress(), CompCode, DivCode, BatchID);
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
            return jsonResponse;
        }

        //validte cds data
        [HttpPost]
        public JsonResponse ValidateCDSData(string CompCode, string DivCode, string BatchID)
        {
            JsonResponse res = _batchProcessing.ValidateCDSData(CompCode, DivCode, BatchID, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
            if (res.HasError)
                res = _audit.errorSave(_loggedInUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)res.ResponseData);
            return res;
        }

        //update bank details

        [HttpPost]
        public JsonResponse UpdateBankDetails()
        {

            var jsonResponse = _batchProcessing.UpdateBankDetailsFromEsewa(_loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
            return jsonResponse;

        }

        public ATTBanks GetBanksfromEsewa()
        {
            string EsewaUrl = _configuration.GetSection("EsewaUrl").Value;
            var UserName = _configuration.GetSection("RSAKeys").GetSection("UserName").Value;
            var Password = _configuration.GetSection("RSAKeys").GetSection("Password").Value;
            var client = new RestClient($"{EsewaUrl}/api/banks");
            var request = new RestRequest(Method.GET);
            var byteArray = Encoding.ASCII.GetBytes($"{UserName}:{Password}");
            var clientAuthrizationHeader = new AuthenticationHeaderValue("Basic",
                                                          Convert.ToBase64String(byteArray));

            var auth = clientAuthrizationHeader.ToString();

            request.AddHeader("Authorization", auth);
            request.AddHeader("Content-Type", "application/json");
            IRestResponse response = client.Execute(request);
            ATTBanks aTTBanks = JsonConvert.DeserializeObject<ATTBanks>(response.Content);
            return aTTBanks;
        }
        //VALIDATE ACCOUNT DETAILS FROM API
        [HttpPost]
        public JsonResponse ValidateAccountDetails(string CompCode, string DivCode, string BatchID, string BankUserName, string BankPassword)
        {
            JsonResponse jsonResponse = new JsonResponse();
            jsonResponse = _batchProcessing.ValidateAccountDetailsBatch(DivCode, CompCode, BatchID, BankUserName, BankPassword, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
            return jsonResponse;
        }


        

        [HttpPost]
        public async Task<IActionResult> GetData(string CompCode, string DivCode, string BatchID, string BatchStatus)
        {
            var request = new ATTDataTableRequest();

            request.Draw = Convert.ToInt32(Request.Form["draw"].FirstOrDefault());
            request.Start = Convert.ToInt32(Request.Form["start"].FirstOrDefault());
            request.Length = Convert.ToInt32(Request.Form["length"].FirstOrDefault());
            request.Search = new ATTDataTableSearch()
            {
                Value = Request.Form["search[value]"].FirstOrDefault()
            };
            request.Order = new ATTDataTableOrder[] {
                new ATTDataTableOrder()
                {
                    Dir = Request.Form["order[0][dir]"].FirstOrDefault(),
                    Column = Convert.ToInt32(Request.Form["order[0][column]"].FirstOrDefault()),
                    ColumnName =Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault(),

                    }
                };
            ATTDataTableResponse<ATTBatchProcessing> returnData = new ATTDataTableResponse<ATTBatchProcessing>();

            returnData = await _eService.GetBatchProcessingAsync(request, CompCode, DivCode, BatchID, BatchStatus);
            var jsonData = new { draw = returnData.Draw, recordsFiltered = returnData.RecordsFiltered, recordsTotal = returnData.RecordsTotal, data = returnData.Data };
            return Ok(jsonData);
        }




    }
}
