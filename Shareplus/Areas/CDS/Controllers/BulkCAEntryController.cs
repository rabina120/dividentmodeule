

using CDSMODULE.Helper;
using ClosedXML.Excel;
using Entity.CDS;
using Entity.Common;
using Entity.Reports;
using ExcelDataReader;
using Interface.CDS;
using Interface.Common;
using Interface.Reports;
using Interface.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Repository.Reports;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CDSMODULE.Areas.CDS.Controllers
{
    [Authorize]
    [Area("CDS")]
    [AutoValidateAntiforgeryToken]
    public class BulkCAEntryController : Controller
    {
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly ILoggedinUser _loggedInUser;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IBulkCAEntry _bulkCAEntry;
        private IConfiguration _configuration;
        private IAudit _audit;
        private readonly IGenericReport _genericReport;

        public BulkCAEntryController(ICheckUserAccess checkUserAccess, ILoggedinUser _loggedInUser, IWebHostEnvironment webHostEnvironment, IConfiguration configuration, IBulkCAEntry bulkCAEntry, IAudit audit, IGenericReport genericReport)
        {
            _checkUserAccess = checkUserAccess;
            this._loggedInUser = _loggedInUser;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _bulkCAEntry = bulkCAEntry;
            _audit = audit;
            _genericReport = genericReport;
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

        #region Excel Upload 
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
        public JsonResponse GetSheetNames()
        {

            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                var currentOLEDBVersion = ExcelConfig.GetOLEDBVersion();
                IFormFile file = Request.Form.Files[0];
                string folderName = "UploadExcel\\BulkCA";
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
        [HttpGet]
        public ActionResult DownloadExcelDocument()
        {
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "DummyExcel BulkCA.xls";
            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet =
                workbook.Worksheets.Add("Bulk CA");
                worksheet.Cell(1, 1).Value = "shholderno";
                worksheet.Cell(1, 2).Value = "certno";
                worksheet.Cell(1, 3).Value = "Kitta";
                worksheet.Cell(1, 4).Value = "boid";
                worksheet.Cell(1, 5).Value = "isin";
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }
        }
        #endregion
        public JsonResponse Upload(IFormFile postedFile, int SheetId, int StartRow, int EndRow, string CompCode, string CertDetailId, string ShOwnerType)

        {

            JsonResponse jsonResponse = new JsonResponse();
            if (postedFile != null && SheetId != null)
            {
                try
                {
                    //Create a Folder.
                    string path = Path.Combine(_webHostEnvironment.WebRootPath, "UploadExcel\\BulkCA");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    //Save the uploaded Excel file.
                    string fileName = Path.GetFileName(postedFile.FileName);
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
                    using (var stream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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
                        //SheetId = SheetId + 1;
                        DataTable dt = ds.Tables[SheetId];
                        var newDt = dt.AsEnumerable()
                            .GroupBy(r => r.Field<dynamic>("certno"))
                            .Select(g => g.First())
                            .CopyToDataTable();

                        if (dt.Rows.Count == newDt.Rows.Count)
                            dt = newDt;
                        else
                        {
                            jsonResponse.Message = ATTMessages.DUPLICATE_RECORDS_FOUND + "\n Duplicate Certificate No .";
                            return jsonResponse;
                        }

                        List<ATTBulkCAEntry> lstexcel = new List<ATTBulkCAEntry>();
                        if (StartRow == 0)
                            StartRow = 1;
                        if (EndRow == 0)
                        {
                            EndRow = dt.Rows.Count;
                        }
                        for (int Rows = StartRow; Rows <= EndRow; Rows++)
                        {

                            ATTBulkCAEntry objexcel = new ATTBulkCAEntry();
                            objexcel.ShHolderNo = (dt.Rows[Rows - 1]["shholderno"]).ToString();
                            objexcel.CertNo = (dt.Rows[Rows - 1]["certno"]).ToString();
                            objexcel.BOID = (dt.Rows[Rows - 1]["boid"]).ToString();
                            objexcel.DPID = (dt.Rows[Rows - 1]["boid"]).ToString().Substring(0, 8);
                            objexcel.ISIN_NO = (dt.Rows[Rows - 1]["isin"]).ToString();
                            lstexcel.Add(objexcel);

                        }
                        jsonResponse = _bulkCAEntry.GetCompleteDataFromExcel(lstexcel, _loggedInUser.GetUserNameToDisplay(), CompCode, CertDetailId, ShOwnerType, Request.HttpContext.Connection.RemoteIpAddress.ToString());

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
        public JsonResponse SaveBulkCAEntry(List<ATTBulkCAEntry> aTTBulkCAEntries, string CompCode, string TransactionDate, string CertDetail)
        {
            JsonResponse response = _bulkCAEntry.SaveBulkCAEntry(aTTBulkCAEntries, CompCode, TransactionDate, CertDetail, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [HttpPost]
        public async Task<IActionResult> GetData(string CompanyCode, int? Cret_Id, int? ShOwnerType)
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
            ATTDataTableResponse<ATTBulkCAEntry> returnData = new ATTDataTableResponse<ATTBulkCAEntry>();

            returnData = await _bulkCAEntry.GetData(request, CompanyCode, Cret_Id, ShOwnerType);
            var jsonData = new { draw = returnData.Draw, recordsFiltered = returnData.RecordsFiltered, recordsTotal = returnData.RecordsTotal, data = returnData.Data };
            return Ok(jsonData);
        }


        [HttpPost]
        public JsonResponse GenerateReport(string CompanyCode, int? Cret_Id, int? ShOwnerType, string ReportType,string CompEnName)
        {
            JsonResponse response = new JsonResponse();
            var ReportName = "Bulk_CA_Details_Report";
            string[] reportTitles = { CompanyCode, CompEnName, ReportName };
            response = _bulkCAEntry.GenerateReport(CompanyCode, Cret_Id, ShOwnerType, ReportType);
            var ReportNameToDatabase = ATTGenericReport.ReportName.BulkCA;
            List<ATTShareHolderReportTotalBasedOn> aTTTotalBasedOns = new List<ATTShareHolderReportTotalBasedOn>();
            aTTTotalBasedOns.Add(new ATTShareHolderReportTotalBasedOn()
            {
                TotalBasedOn="kitta"
            });
               

            if (ReportType == "P")
            {
                response = _genericReport.GenerateReport(ReportNameToDatabase, response, reportTitles, isNepali: false, isTotal: true, aTTTotalBasedOns);
            }
            else
            {
                GenericExcelReport report = new GenericExcelReport();
                response = report.GenerateExcelReport(response, ReportName, ReportType, CompEnName, CompanyCode, "");

                if (response.IsSuccess)
                    response.Message = CompanyCode + "_" + ReportName + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
            }
            if (response.IsSuccess)
            {
                if (ReportType != "E") { 
                    response.Message = ReportName + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".pdf";
                }
                else
                {
                    response.Message = ReportName + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
                }
            }
            return response;
        }
    }
}
