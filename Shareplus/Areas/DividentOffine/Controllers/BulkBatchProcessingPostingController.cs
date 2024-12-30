using CDSMODULE.Helper;
using ClosedXML.Excel;
using Entity.Common;
using ExcelDataReader;
using Interface.Common;
using Interface.DividendProcessing;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

using System.IO;
using System.Text.RegularExpressions;


namespace CDSMODULE.Areas.DividentOffine.Controllers
{
    [Authorize]
    [Area("DividentOffine")]
    [AutoValidateAntiforgeryToken]
    public class BulkBatchProcessingPostingController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IDividend _dividend;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        private IWebHostEnvironment Environment;
        private IConfiguration Configuration;
        private IOptions<ReadConfig> connectionString;
        private readonly ICashDividendEntry _cashDividendEntry;

        public BulkBatchProcessingPostingController(IOptions<ReadConfig> _connectionString, IWebHostEnvironment Environment,
            IConfiguration Configuration, ILoggedinUser loggedInUser, IDividend dividend, ICheckUserAccess checkUserAccess,
            IAudit audit, ICashDividendEntry cashDividendEntry)
        {
            this.connectionString = _connectionString;
            this.Environment = Environment;
            this.Configuration = Configuration;
            _loggedInUser = loggedInUser;
            _dividend = dividend;
            _checkUserAccess = checkUserAccess;
            _audit = audit;
            _cashDividendEntry = cashDividendEntry;
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
        public ActionResult DownloadExcelDocument(string DivBasedOn)
        {
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "DataFormat.xlsx";
            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("DataFormat");
                switch (DivBasedOn)
                {
                    case "01":
                        fileName = "DummyExcel BulkIssue Physical.xlsx";
                        worksheet.Cell(1, 1).Value = "SHHOLDERNO";
                        worksheet.Cell(1, 2).Value = "WARRANTNO";
                        worksheet.Cell(1, 3).Value = "BANKACCNO";
                        worksheet.Cell(1, 4).Value = "BANKNAME";
                        worksheet.Cell(1, 5).Value = "BANKADD";
                        worksheet.Cell(1, 6).Value = "CREDITEDDT";
                        worksheet.Cell(1, 7).Value = "WARRANTAMT";
                        worksheet.Cell(1, 8).Value = "TAXAMOUNT";
                        worksheet.Cell(1, 9).Value = "NETAMOUNT";
                        break;
                    default:
                        fileName = "DummyExcel BulkIssue Demat.xlsx";
                        worksheet.Cell(1, 1).Value = "BOID";
                        worksheet.Cell(1, 2).Value = "WARRANTNO";
                        worksheet.Cell(1, 3).Value = "BANKACCNO";
                        worksheet.Cell(1, 4).Value = "BANKNAME";
                        worksheet.Cell(1, 5).Value = "BANKADD";
                        worksheet.Cell(1, 6).Value = "CREDITEDDT";
                        worksheet.Cell(1, 7).Value = "WARRANTAMT";
                        worksheet.Cell(1, 8).Value = "TAXAMOUNT";
                        worksheet.Cell(1, 9).Value = "NETAMOUNT";
                        break;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }
        }

        [HttpPost]
        public JsonResponse GetAllDividends(string CompCode)
        {
            JsonResponse response = _dividend.GetDividendTableList(CompCode);

            if (response.HasError)
                _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse GetSheetNames()
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                var currentOLEDBVersion = ExcelConfig.GetOLEDBVersion();
                IFormFile file = Request.Form.Files[0];
                string folderName = "UploadExcel\\BulkPayment";
                string webRootPath = Environment.WebRootPath;
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
        
        public JsonResponse UploadSheet(IFormFile postedFile, int SheetId, string DivType)
        {
            JsonResponse jsonResponse = new JsonResponse();

            if (postedFile != null && SheetId != null)
            {
                try
                {
                    // Create a Folder.
                    string path = Path.Combine(this.Environment.WebRootPath, "UploadExcel\\BulkPayment");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    // Save the uploaded Excel file.
                    string fileName = Path.GetFileName(postedFile.FileName);
                    string filePath = Path.Combine(path, fileName);
                    string extension = Path.GetExtension(postedFile.FileName);

                    string conString = string.Empty;
                    if (extension == ".xls")
                    {
                        conString = this.Configuration.GetConnectionString("ExcelConStringV4");
                    }
                    else
                    {
                        conString = this.Configuration.GetConnectionString("ExcelConStringV12");
                    }

                    conString = string.Format(conString, filePath);
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                        IExcelDataReader reader = null;
                        if (fileName.EndsWith(".xls"))
                        {
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        else if (fileName.EndsWith(".xlsx"))
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

                        DataTable dt = ds.Tables[SheetId];

                        jsonResponse.IsSuccess = true;
                        jsonResponse.TotalRecords = dt.Rows.Count;

                        // Verify required columns
                            List<string> columnNames = new List<string>()
                            {
                                "WARRANTNO",
                                "BANKNAME",
                                "BANKADD",
                                "BANKACCNO",
                                "CREDITEDDT",
                                "WARRANTAMT",                                
                                "TAXAMOUNT",
                                "NETAMOUNT"
                            };
                        if (DivType == "02") columnNames.Add("BOID");
                        else columnNames.Add("SHHOLDERNO");

                        foreach (DataColumn column in dt.Columns)
                        {
                            column.ColumnName = column.ColumnName.Trim().ToUpper();
                        }

                        foreach (string column in columnNames)
                        {
                            if (!dt.Columns.Contains(column))
                            {
                                throw new Exception($"Excel Upload Must Have {column}");
                            }
                        }

                        // Validate rows
                        string rowToCheckForNull = DivType == "02" ? "BOID" : "SHHOLDERNO";
                        foreach (DataRow row in dt.Rows)
                        {
                            if (row[rowToCheckForNull] == DBNull.Value)
                            {
                                throw new Exception($"{rowToCheckForNull} is Empty at Row No: {dt.Rows.IndexOf(row) + 1}");
                            }
                        }

                        // Calculate sum of WARRANTAMT
                        decimal totalWarrantAmt = 0;
                        foreach (DataRow row in dt.Rows)
                        {
                            if (decimal.TryParse(row["WARRANTAMT"].ToString(), out decimal warrantAmt))
                            {
                                totalWarrantAmt += warrantAmt;
                            }
                        }

                        jsonResponse.ResponseData = $"{totalWarrantAmt}";                       

                        

                        decimal totalTaxAmount = 0;
                        foreach (DataRow row in dt.Rows)
                        {
                            if (decimal.TryParse(row["TAXAMOUNT"].ToString(), out decimal taxamount))
                            {
                                totalTaxAmount += taxamount;
                            }
                        }

                        jsonResponse.TaxAmount = $"{totalTaxAmount}";

                        decimal totalNetAmount = 0;
                        foreach (DataRow row in dt.Rows)
                        {
                            if (decimal.TryParse(row["NETAMOUNT"].ToString(), out decimal netamount))
                            {
                                totalNetAmount += netamount;
                            }
                        }

                        jsonResponse.NetAmount = $"{totalNetAmount}";

                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;
                    _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), ex);
                }
            }
            return jsonResponse;
        }


        [HttpPost]
        public JsonResponse BulkDividendPayment(string CompCode, string DivCode, string DivType, string IssueDate, bool isIssue, bool isPay, string IssueRemarks, string FileName, int SheetId)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                //Create a Folder.
                string path = Path.Combine(this.Environment.WebRootPath, "UploadExcel\\BulkPayment");

                //Save the uploaded Excel file.
                string fileName = Path.GetFileName(FileName);
                string filePath = Path.Combine(path, fileName);
                string extension = Path.GetExtension(fileName);
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
                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);


                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    IExcelDataReader reader = null;
                    if (fileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (fileName.EndsWith(".xlsx"))
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

                    dt = ds.Tables[SheetId];
                }

                jsonResponse = _cashDividendEntry.BulkPayment(CompCode, DivCode, DivType, IssueDate, isIssue, isPay, IssueRemarks, dt, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            }
            catch (Exception ex)
            {
                jsonResponse.Message = ex.Message;
            }
            return jsonResponse;
        }
    }
}
