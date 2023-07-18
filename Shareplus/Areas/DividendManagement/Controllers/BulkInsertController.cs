using CDSMODULE.Helper;
using ClosedXML.Excel;
using Entity.Common;
using Entity.Dividend;
using ExcelDataReader;

using Interface.Common;
using Interface.DividendManagement;
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
using System.IO;
using System.Linq;

namespace CDSMODULE.Areas.DividendManagement.Controllers
{
    [Authorize]
    [Area("DividendManagement")]
    [AutoValidateAntiforgeryToken]
    public class BulkInsertController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IDividend _dividend;
        private readonly IBulkInsert _BulkInsert;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        private IWebHostEnvironment Environment;
        private IConfiguration Configuration;
        IOptions<ReadConfig> connectionString;

        public BulkInsertController(IOptions<ReadConfig> _connectionString, IWebHostEnvironment Environment, IConfiguration Configuration, ILoggedinUser _loggedInUser, IDividend dividend, IBulkInsert bulkedinsert, ICheckUserAccess checkUserAccess, IAudit audit)
        {
            this.connectionString = _connectionString;
            this.Environment = Environment;
            this.Configuration = Configuration;
            this._loggedInUser = _loggedInUser;
            _dividend = dividend;
            _BulkInsert = bulkedinsert;
            _checkUserAccess = checkUserAccess;
            _audit = audit;
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
        [HttpPost]
        public JsonResponse GetExcelSheetNames()
        {

            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                var currentOLEDBVersion = ExcelConfig.GetOLEDBVersion();
                IFormFile file = Request.Form.Files[0];
                string folderName = "UploadExcel\\BulkInsert";
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

        [HttpPost]
        public JsonResponse GetAllDividends(string CompCode, string DivType)
        {
            JsonResponse response = _BulkInsert.GetDividendTableList(CompCode, DivType);
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
        [HttpGet]
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
                        worksheet.Cell(1, 1).Value = "shholderno";
                        worksheet.Cell(1, 2).Value = "fname";
                        worksheet.Cell(1, 3).Value = "lname";
                        worksheet.Cell(1, 4).Value = "faname";
                        worksheet.Cell(1, 5).Value = "occupation";
                        worksheet.Cell(1, 6).Value = "totshkitta";
                        worksheet.Cell(1, 7).Value = "tfrackitta";
                        worksheet.Cell(1, 8).Value = "total";
                        worksheet.Cell(1, 9).Value = "actualbonus";
                        worksheet.Cell(1, 10).Value = "abwithpfrac";
                        worksheet.Cell(1, 11).Value = "issuebonus";
                        worksheet.Cell(1, 12).Value = "remfrac";
                        worksheet.Cell(1, 13).Value = "warrantamt";
                        worksheet.Cell(1, 14).Value = "bonustax";
                        worksheet.Cell(1, 15).Value = "taxDamt";
                        worksheet.Cell(1, 16).Value = "bonusadj";
                        worksheet.Cell(1, 17).Value = "prevadj";
                        worksheet.Cell(1, 18).Value = "warrantno";
                        break;
                    case "02":
                        worksheet.Cell(1, 1).Value = "warrantno";
                        worksheet.Cell(1, 2).Value = "BO_idno";
                        worksheet.Cell(1, 3).Value = "fullname";
                        worksheet.Cell(1, 4).Value = "faname";
                        worksheet.Cell(1, 5).Value = "grfaname";
                        worksheet.Cell(1, 6).Value = "address";
                        worksheet.Cell(1, 7).Value = "occupation";
                        worksheet.Cell(1, 8).Value = "warrantamt";
                        worksheet.Cell(1, 9).Value = "taxdamt";
                        worksheet.Cell(1, 10).Value = "btaxamt";
                        worksheet.Cell(1, 11).Value = "bonusadj";
                        worksheet.Cell(1, 12).Value = "PrevAdj";
                        worksheet.Cell(1, 13).Value = "totshkitta";
                        worksheet.Cell(1, 14).Value = "payableamt"; 
                        break;
                    case "03":
                        worksheet.Cell(1, 1).Value = "shholderno";
                        worksheet.Cell(1, 2).Value = "fname";
                        worksheet.Cell(1, 3).Value = "lname";
                        worksheet.Cell(1, 4).Value = "occupation";
                        worksheet.Cell(1, 5).Value = "PrevTotKitta";
                        worksheet.Cell(1, 6).Value = "PrevFrac";
                        worksheet.Cell(1, 7).Value = "total";
                        worksheet.Cell(1, 8).Value = "actualbonus";
                        worksheet.Cell(1, 9).Value = "abwithpfrac";
                        worksheet.Cell(1, 10).Value = "issuebonus";
                        worksheet.Cell(1, 11).Value = "remfrac";
                        worksheet.Cell(1, 12).Value = "bonustax";
                        worksheet.Cell(1, 13).Value = "bonusadj";
                        worksheet.Cell(1, 14).Value = "certno";
                        worksheet.Cell(1, 15).Value = "srnofrom";
                        worksheet.Cell(1, 16).Value = "srnoto";
                        break;
                    default:
                        worksheet.Cell(1, 1).Value = "boid";
                        worksheet.Cell(1, 2).Value = "name";
                        worksheet.Cell(1, 3).Value = "address";
                        worksheet.Cell(1, 4).Value = "faname";
                        worksheet.Cell(1, 4).Value = "occupation";
                        worksheet.Cell(1, 5).Value = "PrevTotKitta";
                        worksheet.Cell(1, 6).Value = "PrevFrac";
                        worksheet.Cell(1, 7).Value = "total";
                        worksheet.Cell(1, 8).Value = "actualbonus";
                        worksheet.Cell(1, 9).Value = "abwithpfrac";
                        worksheet.Cell(1, 10).Value = "issuebonus";
                        worksheet.Cell(1, 11).Value = "remfrac";
                        worksheet.Cell(1, 13).Value = "bonustax";
                        worksheet.Cell(1, 12).Value = "bonusadj";
                        worksheet.Cell(1, 14).Value = "certno";
                        worksheet.Cell(1, 15).Value = "srnofrom";
                        worksheet.Cell(1, 16).Value = "srnoto";
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
        public JsonResponse UploadSheet(IFormFile postedFile, int SheetId, string DivCode, string Agmno, string CompCode, string ActionType)
        {
            DataTable dt1 = new DataTable();
            JsonResponse jsonResponse = new JsonResponse();


            string tablename1 = string.Empty;
            if (ActionType == "01" || ActionType == "03")
            {
                tablename1 = "1";
            }
            if (ActionType == "02" || ActionType == "04")
            {
                tablename1 = "2";
            }
            if (postedFile != null)
            {
                try
                {
                    // Create a Folder.
                    string path = Path.Combine(this.Environment.WebRootPath, "UploadExcel\\BulkInsert");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    //Save the uploaded Excel file.
                    string fileName = Path.GetFileName(postedFile.FileName);
                    string excelFilePath = Path.Combine(path, fileName);
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

                        #region to insert
                        //Insert the Data read from the Excel file to Database Table.
                        
                        DataTable dt = ds.Tables[SheetId];
                        dt.Columns.Add(new DataColumn("compcode", typeof(string)));
                        dt.Columns.Add(new DataColumn("entryuser", typeof(string)));
                        dt.Columns.Add(new DataColumn("entrydate", typeof(string)));
                        dt.Columns.Add(new DataColumn("approveduser", typeof(string)));
                        dt.Columns.Add(new DataColumn("approveddate", typeof(string)));
                        dt.Columns.Add(new DataColumn("approved", typeof(string)));
                        dt.Columns.Add(new DataColumn("isLocked", typeof(bool)));
                        dt.Columns.Add(new DataColumn("Dpayable", typeof(bool)));
                        dt.Columns.Add(new DataColumn("WIssued", typeof(bool)));
                        dt.Columns.Add(new DataColumn("WPaid", typeof(bool)));
                        dt.Columns.Add(new DataColumn("ispool", typeof(bool)));
                        dt.Columns.Add(new DataColumn("divType", typeof(string)));
                        dt.Columns.Add(new DataColumn("AgmNo", typeof(string)));

                        foreach (DataRow row in dt.Rows)
                        {
                            row["compcode"] = CompCode.Substring(0, 3);
                            row["entryuser"] = _loggedInUser.GetUserNameToDisplay();
                            row["entrydate"] = DateTime.Now;
                            row["approveduser"] = _loggedInUser.GetUserNameToDisplay();
                            row["approveddate"] = DateTime.Now;
                            row["approved"] = "Y";
                            row["isLocked"] = "False";
                            row["Dpayable"] = "False";
                            row["WIssued"] = "False";
                            row["WPaid"] = "False";
                            row["ispool"] = "False";
                            row["divType"] = DivCode;
                            row["AgmNo"] = Agmno;
                        }

                        var sourcearrayNames = (from DataColumn x
                          in dt.Columns.Cast<DataColumn>()
                                                select x.ColumnName).ToArray();

                        jsonResponse = _BulkInsert.Issue(CompCode, DivCode, tablename1, ActionType, SheetId, ds, dt, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());

                        if (jsonResponse.HasError)
                        {
                            jsonResponse = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)jsonResponse.ResponseData);
                            return jsonResponse;
                        }

                        
                        #endregion
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
        public JsonResponse CheckRecord(IFormFile postedFile, int SheetId, string DivCode, string CompCode, string ActionType)
        {
            JsonResponse jsonResponse = new JsonResponse();
            if (postedFile != null && SheetId >= 0)
            {
                try
                {
                    //Create a Folder.
                    string path = Path.Combine(this.Environment.WebRootPath, "UploadExcel\\BulkInsert");
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
                        conString = this.Configuration.GetConnectionString("ExcelConStringV4");
                    }
                    else
                    {
                        ////This will read 2007 Excel format 
                        conString = this.Configuration.GetConnectionString("ExcelConStringV12");
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
                        //string[] columnNames = dt.Columns.List;

                        if (ActionType == "03" || ActionType == "04")
                        {
                            double totalkitta, tfrackitta, total, actualbonus, actualbonuswithprevfrac, issuebonus, remfrac;
                            int totalcount = 0;
                            ATTBONUSEXCELUPLOAD aTTBONUSEXCELUPLOAD = new ATTBONUSEXCELUPLOAD();
                            aTTBONUSEXCELUPLOAD.totalcount = dt.Rows.Count.ToString();
                            aTTBONUSEXCELUPLOAD.PrevTotKitta = int.Parse(dt.AsEnumerable().Sum(row => row.Field<double>("PrevTotKitta")).ToString()).ToString();
                            aTTBONUSEXCELUPLOAD.PrevFrac = dt.AsEnumerable().Sum(row => row.Field<double>("PrevFrac")).ToString();
                            aTTBONUSEXCELUPLOAD.total = dt.AsEnumerable().Sum(row => row.Field<double>("total")).ToString();
                            aTTBONUSEXCELUPLOAD.actualbonus = dt.AsEnumerable().Sum(row => row.Field<double>("actualbonus")).ToString();
                            aTTBONUSEXCELUPLOAD.actualbonuswithprevfrac = dt.AsEnumerable().Sum(row => row.Field<double>("abwithpfrac")).ToString();
                            aTTBONUSEXCELUPLOAD.issuebonus = int.Parse(dt.AsEnumerable().Sum(row => row.Field<double>("issuebonus")).ToString()).ToString();
                            aTTBONUSEXCELUPLOAD.remfrac = dt.AsEnumerable().Sum(row => row.Field<double>("remfrac")).ToString();
                            aTTBONUSEXCELUPLOAD.bonustax = dt.AsEnumerable().Sum(row => row.Field<double>("bonustax")).ToString();
                            jsonResponse.ResponseData = aTTBONUSEXCELUPLOAD;
                            jsonResponse.IsSuccess = true;
                        }
                        else
                        {
                            double WARRANTAMT, DPAYABLE, DivTax, TOTSHKITTA, netpay;
                            int TOTALCOUNT = 0;



                            WARRANTAMT = dt.AsEnumerable().Sum(row => row.Field<double>("warrantamt"));
                            TOTALCOUNT = dt.Rows.Count;
                            DivTax = dt.AsEnumerable().Sum(row => row.Field<double>("taxdamt"));
                            TOTSHKITTA = dt.AsEnumerable().Sum(row => row.Field<double>("totshkitta"));

                            ATTDIVIDENDEXCELUPLOAD OBJ = new ATTDIVIDENDEXCELUPLOAD();
                            OBJ.WARRANTAMT = WARRANTAMT;
                            OBJ.TOTALCOUNT = TOTALCOUNT;
                            OBJ.DivTax = DivTax;
                            OBJ.TOTSHKITTA = TOTSHKITTA;

                            jsonResponse.ResponseData = OBJ;
                            jsonResponse.IsSuccess = true;
                        }


                    }
                            jsonResponse.Message = "Please Check The Data For Further Processing.";
                    //jsonResponse.Message = "Excell Sheet Checked and Added";

                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;

                }

            }
            return jsonResponse;
        }
    }
}
