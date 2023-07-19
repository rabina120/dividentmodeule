
using CDSMODULE.Helper;
using ClosedXML.Excel;
using Entity.CDS;
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
using System.Text;
using System.Text.RegularExpressions;

namespace CDSMODULE.Areas.CDSExportImport.Controllers
{
    [Authorize]
    [Area("CDSExportImport")]
    [AutoValidateAntiforgeryToken]
    public class CreateIPFController : Controller
    {

        private readonly ILoggedinUser _loggedInUser;
        private readonly IDividend _dividend;
        private readonly ICashDividendEntry _cashDividendEntry;
        private readonly ICheckUserAccess _checkUserAccess;
        private IWebHostEnvironment Environment;
        private IConfiguration Configuration;
        private IAudit _audit;

        IOptions<ReadConfig> connectionString;
        public CreateIPFController(IOptions<ReadConfig> _connectionString, IAudit audit, IWebHostEnvironment Environment, IConfiguration Configuration, ILoggedinUser _loggedInUser, IDividend dividend, ICashDividendEntry cashDividendEntry, ICheckUserAccess checkUserAccess)
        {
            this.connectionString = _connectionString;
            this.Environment = Environment;
            this.Configuration = Configuration;
            this._loggedInUser = _loggedInUser;
            _dividend = dividend;
            _cashDividendEntry = cashDividendEntry;
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
                string folderName = "UploadExcel\\IPF";
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
                                sheet = workbook.GetSheetAt(0);
                            }

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
                                sheet = workbook.GetSheetAt(0); //get first sheet from workbook
                            }


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
            string fileName = "DummyDataIPFCreate.xlsx";
            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet =
                workbook.Worksheets.Add("IPF Format");
                worksheet.Cell(1, 1).Value = "boid";
                worksheet.Cell(1, 2).Value = "rtarefno";
                worksheet.Cell(1, 3).Value = "isin";
                worksheet.Cell(1, 4).Value = "current_quan";
                worksheet.Cell(1, 5).Value = "frozen_quan";
                worksheet.Cell(1, 6).Value = "lock_in_quan";
                worksheet.Cell(1, 7).Value = "lock_code";
                worksheet.Cell(1, 8).Value = "lock_in_reason";
                worksheet.Cell(1, 9).Value = "lock_in_expiry";
                worksheet.Cell(1, 10).Value = "dr/cr";
                worksheet.Cell(1, 11).Value = "benefit_isin";
                worksheet.Cell(1, 12).Value = "curr_quan";
                worksheet.Cell(1, 13).Value = "frozen";
                worksheet.Cell(1, 14).Value = "lock_in";
                worksheet.Cell(1, 15).Value = "lock_code";
                worksheet.Cell(1, 16).Value = "lock_in_reason";
                worksheet.Cell(1, 17).Value = "expiry";
                worksheet.Cell(1, 18).Value = "db/cr";
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }
        }
        #endregion

        [HttpPost]
        public JsonResponse Upload(IFormFile postedFile, int SheetId, int StartRow, int totRow)

        {

            JsonResponse jsonResponse = new JsonResponse();
            if (postedFile != null && SheetId != null)
            {
                try
                {
                    //Create a Folder.
                    string path = Path.Combine(this.Environment.WebRootPath, "UploadExcel\\IPF");
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
                            throw (new Exception("Cannot Read Excel File"));

                        var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });
                        //SheetId = SheetId + 1;
                        DataTable dt = ds.Tables[SheetId];

                        List<ATTCreateIPF> lstexcel = new List<ATTCreateIPF>();
                        //  const string format = "{0,-10} {1,10}";
                        for (int Rows = StartRow; Rows <= totRow; Rows++)
                        {

                            ATTCreateIPF objexcel = new ATTCreateIPF();
                            objexcel.BOID = (dt.Rows[Rows - 1]["boid"]).ToString();
                            objexcel.Isin = (dt.Rows[Rows - 1]["isin"]).ToString();
                            objexcel.current_quan = String.Format("{0:000000000000.000}", int.Parse((dt.Rows[Rows - 1]["current_quan"]).ToString()));
                            objexcel.froz_quan = String.Format("{0:000000000000.000}", int.Parse((dt.Rows[Rows - 1]["frozen_quan"]).ToString()));
                            objexcel.lock_in_quan = String.Format("{0:000000000000.000}", int.Parse((dt.Rows[Rows - 1]["lock_in_quan"]).ToString()));
                            objexcel.lock_code = String.Format("{0:00}", int.Parse((string.IsNullOrEmpty((dt.Rows[Rows - 1]["lock_code"]).ToString()) ? "0" : (dt.Rows[Rows - 1]["lock_code"]).ToString())));
                            objexcel.lock_in_reason = (((dt.Rows[Rows - 1]["lock_in_reason"]).ToString()).PadLeft(50)).ToString();
                            objexcel.lock_in_expiry = String.Format("{0:00000000}", int.Parse((string.IsNullOrEmpty((dt.Rows[Rows - 1]["lock_in_expiry"]).ToString()) ? "0" : (dt.Rows[Rows - 1]["lock_in_expiry"]).ToString())));
                            objexcel.dbcr = (dt.Rows[Rows - 1]["dr/cr"]).ToString();
                            objexcel.benefit_isin1 = (dt.Rows[Rows - 1]["benefit_isin"]).ToString();
                            objexcel.current_quan1 = String.Format("{0:000000000000.000}", int.Parse((dt.Rows[Rows - 1]["curr_quan"]).ToString() == "" ? "0" : (dt.Rows[Rows - 1]["curr_quan"]).ToString()));
                            objexcel.frozen1 = String.Format("{0:000000000000.000}", int.Parse((dt.Rows[Rows - 1]["frozen"]).ToString()));
                            objexcel.lock_in1 = String.Format("{0:000000000000.000}", int.Parse((dt.Rows[Rows - 1]["lock_in"]).ToString()));
                            objexcel.lock_code1 = String.Format("{0:00}", int.Parse((string.IsNullOrEmpty((dt.Rows[Rows - 1]["lock_code_1"]).ToString()) ? "0" : (dt.Rows[Rows - 1]["lock_code_1"]).ToString())));
                            objexcel.lock_in_reason1 = (((dt.Rows[Rows - 1]["lock_in_reason_1"]).ToString()).PadLeft(50)).ToString();
                            objexcel.expiry1 = String.Format("{0:00000000}", int.Parse((string.IsNullOrEmpty((dt.Rows[Rows - 1]["expiry"]).ToString()) ? "0" : (dt.Rows[Rows - 1]["expiry"]).ToString())));
                            objexcel.dbcr1 = (dt.Rows[Rows - 1]["db/cr"]).ToString();
                            objexcel.rtarefno = (((dt.Rows[Rows - 1]["rtarefno"]).ToString()).PadLeft(16)).ToString();
                            lstexcel.Add(objexcel);

                        }
                        jsonResponse.ResponseData = lstexcel;
                        jsonResponse.IsSuccess = true;
                    }
                }

                catch (Exception ex)
                {
                    _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), ex);
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;
                }

            }
            return jsonResponse;
        }

        public static Object DeSerialize(string json, Type ObjectType)
        {
            try
            {

                Object objDeserialized = Newtonsoft.Json.JsonConvert.DeserializeObject(json, ObjectType);

                return objDeserialized;
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        public object CreateFile(string Excel, string TotRecord, string TotFrzeQty, string TotCurrQty, string TotLockQty)
        {
            string file = "";
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                string msg = "IPF File Created!!!";


                List<ATTCreateIPF> lst = DeSerialize(Excel, typeof(List<ATTCreateIPF>)) as List<ATTCreateIPF>;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(TotRecord + TotCurrQty + TotFrzeQty + TotLockQty);

                foreach (var s in lst)
                {
                    sb.AppendLine(((
                        s.BOID.ToString()).PadLeft(16)).ToString() + 
                        ((s.rtarefno.ToString().Trim()).PadLeft(16)).ToString() + 
                        ((s.Isin.ToString().Trim().Trim()).PadLeft(10)).ToString() + 
                        s.current_quan.Trim() + 
                        s.froz_quan.Trim() + 
                        s.lock_in_quan.Trim() + 
                        s.lock_code.Trim() +
                        ((s.lock_in_reason.ToString().Trim()).PadLeft(50)).ToString() + 
                        s.lock_in_expiry.Trim() + 
                        s.dbcr.Trim() + s.benefit_isin1.Trim() + 
                        s.current_quan1.Trim() + 
                        s.frozen1.Trim() + s.lock_in1.Trim() + s.
                        lock_code1.Trim() + 
                        ((s.lock_in_reason1.ToString().Trim()).PadLeft(50)).ToString() + 
                        s.expiry1.Trim() + s.dbcr1.Trim());
                }

                var data = sb.ToString();



                //var path = LogFile();
                string folderName = "FileExport";
                string webRootPath = Environment.WebRootPath;
                string path = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string newpath = Environment.WebRootPath + "\\FileExport\\" + DateTime.Now.ToString("dd-MM-yyyy hh_mm_s_tt") + "Report.ipf";

                using (FileStream fs = new FileStream(newpath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose))
                {

                    var sr = new StreamWriter(fs);
                    sr.WriteLine(sb);
                    sr.Flush();
                    sr.Close();
                    file = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(sb.ToString()));
                }

                jsonResponse.IsSuccess = true;
                jsonResponse.ResponseData = file;
                jsonResponse.Message = msg;
            }
            catch (Exception ee)
            {
                _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), ee);
                jsonResponse.IsSuccess = false;
                jsonResponse.Message = ee.Message; ;
            }
            return jsonResponse;
        }
        public object CreateCVFFile(string Excel1, string TotRecord)
        {
            string file = "";
            JsonResponse jsonResponse = new JsonResponse();
            string msg = "CVF File Created!!!";
            try
            {
                List<ATTCreateIPF> lst = DeSerialize(Excel1, typeof(List<ATTCreateIPF>)) as List<ATTCreateIPF>;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(TotRecord);
                foreach (var s in lst)
                {
                    sb.AppendLine(((s.BOID.ToString().Trim()).PadLeft(16)).ToString());
                }

                var data = sb.ToString();

                string folderName = "FileExport";
                string webRootPath = Environment.WebRootPath;
                string path = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string newpath = Environment.WebRootPath + "\\FileExport\\" + DateTime.Now.ToString("dd-MM-yyyy hh_mm_s_tt") + "Report.cvf";

                using (FileStream fs = new FileStream(newpath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose))
                {
                    var sr = new StreamWriter(fs);
                    sr.WriteLine(sb);
                    sr.Flush();
                    sr.Close();
                    file = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(sb.ToString()));
                }

                jsonResponse.IsSuccess = true;
                jsonResponse.ResponseData = file;
                jsonResponse.Message = msg;
            }


            catch (Exception ee)
            {
                jsonResponse.IsSuccess = false;
                jsonResponse.Message = ee.Message;
                _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), ee);

            }
            return jsonResponse;
        }
    }
}
