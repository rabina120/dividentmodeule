using CDSMODULE.Helper;
using Entity.Common;

using Interface.Common;
using Interface.Security;
using Interface.ShareHolder;
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

namespace Shareplus.Areas.HolderManagement.Controllers
{
    [Authorize]
    [Area("HolderManagement")]
    [AutoValidateAntiforgeryToken]
    public class UpdateDemateHolderController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IUpdateDemateHolder _UpdateDemateHolder;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        private IWebHostEnvironment Environment;
        private IConfiguration Configuration;
        IOptions<ReadConfig> connectionString;

        public UpdateDemateHolderController(IOptions<ReadConfig> _connectionString, IWebHostEnvironment Environment, IConfiguration Configuration, ILoggedinUser _loggedInUser, IUpdateDemateHolder updateDemateHolder, ICheckUserAccess checkUserAccess, IAudit audit)
        {
            this.connectionString = _connectionString;
            this.Environment = Environment;
            this.Configuration = Configuration;
            this._loggedInUser = _loggedInUser;
            _UpdateDemateHolder = updateDemateHolder;
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
        public JsonResponse GetSheetNames()
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                var currentOLEDBVersion = ExcelConfig.GetOLEDBVersion();
                IFormFile file = Request.Form.Files[0];
                string folderName = "UploadExcel\\UpdateDemateHolderExcel";
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

        [HttpPost]
        //public JsonResponse UploadSheet(IFormFile postedFile, int SheetId)
        //{
        //    JsonResponse jsonResponse = new JsonResponse();

        //    if (postedFile == null)
        //    {
        //        jsonResponse.IsSuccess = false;
        //        jsonResponse.Message = "Invalid file or sheet ID.";
        //        return jsonResponse;
        //    }

        //    try
        //    {
        //        // Create a Folder
        //        string path = Path.Combine(this.Environment.WebRootPath, "UploadExcel\\UpdateDemateHolderExcel");
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }

        //        // Save the uploaded Excel file
        //        string fileName = Path.GetFileName(postedFile.FileName);
        //        string filePath = Path.Combine(path, fileName);
        //        postedFile.CopyTo(new FileStream(filePath, FileMode.Create));

        //        string extension = Path.GetExtension(postedFile.FileName);
        //        string conString = extension == ".xls"
        //            ? this.Configuration.GetConnectionString("ExcelConStringV4")
        //            : this.Configuration.GetConnectionString("ExcelConStringV12");

        //        conString = string.Format(conString, filePath);
        //        // using FileStream fs = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        //        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //        {
        //            IExcelDataReader reader = extension == ".xls"
        //                ? ExcelReaderFactory.CreateBinaryReader(stream)
        //                : ExcelReaderFactory.CreateOpenXmlReader(stream);

        //            if (reader == null)
        //            {
        //                jsonResponse.IsSuccess = false;
        //                jsonResponse.Message = "Failed to create Excel reader.";
        //                return jsonResponse;
        //            }

        //            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        //            var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
        //            {
        //                ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
        //                {
        //                    UseHeaderRow = true
        //                }
        //            });

        //            DataTable dt = ds.Tables[SheetId];

        //            jsonResponse.IsSuccess = true;
        //            jsonResponse.TotalRecords = dt.Rows.Count;

        //            // Verify required columns
        //            List<string> columnNames = new List<string>()
        //            {
        //                "boid_no", "holder_name", "gurdian_name", "nominee_name", "father_or_husband_name",
        //                "gender", "dob", "citizen_no", "nrn_id_no", "grandfather_or_spouse_name", "account_status",
        //                "pan_no", "tax_deduction_status", "nationality", "bank_code", "bank_branch", "bank_name",
        //                "bank_acc_type", "bank_acc_no", "total_holding", "pledge_balance", "free_balance"
        //            };

        //            foreach (DataColumn column in dt.Columns)
        //            {
        //                column.ColumnName = column.ColumnName.Trim().ToUpper();
        //            }

        //            foreach (string column in columnNames)
        //            {
        //                if (!dt.Columns.Contains(column))
        //                {
        //                    throw new Exception($"Excel Upload Must Have {column}");
        //                }
        //            }

        //            jsonResponse = _UpdateDemateHolder.UploadHolderDetails(dt, _loggedInUser.GetUserNameToDisplay());
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        jsonResponse.IsSuccess = false;
        //        jsonResponse.Message = ex.Message;
        //        _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), ex);
        //    }

        //    return jsonResponse;
        //}

        [HttpPost]
        public JsonResponse UploadSheet(IFormFile postedFile, int sheetId)
        {
            JsonResponse jsonResponse = new JsonResponse();
            if (postedFile == null)
            {
                jsonResponse.IsSuccess = false;
                jsonResponse.Message = "Invalid file or no file uploaded.";
                return jsonResponse;
            }

            try
            {
                // Save uploaded file
                string path = Path.Combine(Environment.WebRootPath, "UploadExcel\\UpdateDemateHolderExcel");
                Directory.CreateDirectory(path);
                string filePath = Path.Combine(path, postedFile.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                    stream.Position = 0;

                    IWorkbook workbook;
                    string fileExtension = Path.GetExtension(postedFile.FileName).ToLower();
                    if (fileExtension == ".xls")
                    {
                        workbook = new HSSFWorkbook(stream);
                    }
                    else if (fileExtension == ".xlsx")
                    {
                        workbook = new XSSFWorkbook(stream);
                    }
                    else
                    {
                        throw new Exception("Unsupported file format. Only .xls and .xlsx are allowed.");
                    }

                    // Validate sheet ID
                    if (sheetId < 0 || sheetId >= workbook.NumberOfSheets)
                    {
                        throw new Exception("Invalid sheet ID.");
                    }

                    ISheet sheet = workbook.GetSheetAt(sheetId);

                    // Read sheet data into DataTable
                    DataTable dataTable = new DataTable();
                    IRow headerRow = sheet.GetRow(0);
                    foreach (ICell cell in headerRow.Cells)
                    {
                        dataTable.Columns.Add(cell.ToString().Trim().ToUpper());
                    }

                    for (int i = 1; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;

                        DataRow dataRow = dataTable.NewRow();
                        for (int j = 0; j < row.LastCellNum; j++)
                        {
                            dataRow[j] = row.GetCell(j)?.ToString();
                        }
                        dataTable.Rows.Add(dataRow);
                    }

                    // Validate required columns
                    List<string> requiredColumns = new List<string>
            {
                "BOID_NO", "HOLDER_NAME", "GURDIAN_NAME", "NOMINEE_NAME", "FATHER_OR_HUSBAND_NAME",
                "GENDER", "DOB", "CITIZEN_NO", "NRN_ID_NO", "GRANDFATHER_OR_SPOUSE_NAME",
                "ACCOUNT_STATUS", "PAN_NO", "TAX_DEDUCTION_STATUS", "NATIONALITY",
                "BANK_CODE", "BANK_BRANCH", "BANK_NAME", "BANK_ACC_TYPE", "BANK_ACC_NO",
                "TOTAL_HOLDING", "PLEDGE_BALANCE", "FREE_BALANCE"
            };

                    foreach (string column in requiredColumns)
                    {
                        if (!dataTable.Columns.Contains(column))
                        {
                            throw new Exception($"Missing required column: {column}");
                        }
                    }

                    // Upload data
                    jsonResponse = _UpdateDemateHolder.UploadHolderDetails(dataTable, _loggedInUser.GetUserNameToDisplay());
                    jsonResponse.IsSuccess = true;
                    jsonResponse.TotalRecords = dataTable.Rows.Count;
                }
            }
            catch (Exception ex)
            {
                jsonResponse.IsSuccess = false;
                jsonResponse.Message = $"Error: {ex.Message}";
                _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), ex);
            }

            return jsonResponse;
        }

        //public JsonResponse UploadSheet(IFormFile postedFile, int SheetId )
        //{
        //    JsonResponse jsonResponse = new JsonResponse();
        //    DataTable dataTable = new DataTable();
        //    IFormFile file = Request.Form.Files[0];
        //    //string folderName = "UploadExcel\\UpdateDemateHolder";
        //    if (postedFile != null && SheetId != null)
        //    {
        //        try
        //        {
        //            // Create a Folder.
        //            string path = Path.Combine(this.Environment.WebRootPath, "UploadExcel\\UpdateDemateHolder");
        //            if (!Directory.Exists(path))
        //            {
        //                Directory.CreateDirectory(path);
        //            }

        //            // Save the uploaded Excel file.
        //            string fileName = Path.GetFileName(postedFile.FileName);
        //            string filePath = Path.Combine(path, fileName);
        //            string extension = Path.GetExtension(postedFile.FileName);

        //            string conString = string.Empty;
        //            if (extension == ".xls")
        //            {
        //                conString = this.Configuration.GetConnectionString("ExcelConStringV4");
        //            }
        //            else
        //            {
        //                conString = this.Configuration.GetConnectionString("ExcelConStringV12");
        //            }

        //            conString = string.Format(conString, filePath);
        //            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //            {
        //                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        //                IExcelDataReader reader = null;
        //                if (fileName.EndsWith(".xls"))
        //                {
        //                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
        //                }
        //                else if (fileName.EndsWith(".xlsx"))
        //                {
        //                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        //                }

        //                if (reader == null)
        //                    return null;

        //                var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
        //                {
        //                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
        //                    {
        //                        UseHeaderRow = true
        //                    }
        //                });

        //                DataTable dt = ds.Tables[SheetId];

        //                jsonResponse.IsSuccess = true;
        //                jsonResponse.TotalRecords = dt.Rows.Count;

        //                // Verify required columns
        //                List<string> columnNames = new List<string>()
        //                    {
        //                        "id",
        //                        "boid_no",
        //                        "holder_name",
        //                        "gurdian_name",
        //                        "nominee_name",
        //                        "father_or_husband_name",
        //                        "gender",
        //                        "dob",
        //                        "citizen_no",
        //                        "nrn_id_no",
        //                        "grandfather_or_spouse_name",
        //                        "account_status",
        //                        "pan_no",
        //                        "tax_deduction_status",
        //                        "nationality",
        //                        "bank_code",
        //                        "bank_branch",
        //                        "bank_name",
        //                        "bank_acc_type",
        //                         "bank_acc_no",
        //                         "total_holding",
        //                         "pledge_balance",
        //                         "free_balance"
        //                    };


        //                foreach (DataColumn column in dt.Columns)
        //                {
        //                    column.ColumnName = column.ColumnName.Trim().ToUpper();
        //                }

        //                foreach (string column in columnNames)
        //                {
        //                    if (!dt.Columns.Contains(column))
        //                    {
        //                        throw new Exception($"Excel Upload Must Have {column}");
        //                    }
        //                }


        //                jsonResponse = _UpdateDemateHolder.UploadHolderDetails(dataTable, _loggedInUser.GetUserNameToDisplay());
        //            }
        //            jsonResponse.TotalRecords = dataTable.Rows.Count;
        //        }

        //        catch (Exception ex)
        //        {
        //            jsonResponse.IsSuccess = false;
        //            jsonResponse.Message = ex.Message;
        //            _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), ex);
        //        }
        //    }
        //    return jsonResponse;
        //}

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
        public JsonResponse UploadFile()
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                DataTable dataTable = new DataTable();
                IFormFile file = Request.Form.Files[0];
                string folderName = "UploadExcel\\UpdateDemateHolder1";
                string webRootPath = Environment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                if (file.Length > 0)
                {
                    string fullPath = Path.Combine(newPath, file.FileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        stream.Position = 0;
                    }

                    // Initialize a new StreamReader instance to read the text file
                    using (StreamReader reader = new StreamReader(fullPath))
                    {
                        // Read the first line of the file to extract the field names
                        string headerLine = reader.ReadLine();

                        // Split the header line using the "~" characters as a separator to get the field names
                        string[] fieldNames = headerLine.Split(new string[] { "~" }, StringSplitOptions.None);
                        int a = 1;
                        // Create columns dynamically in the DataTable for each field
                        foreach (string fieldName in fieldNames)
                        {
                            dataTable.Columns.Add("column" + a);
                            a++;
                        }
                        // Read the rest of the file and add rows to the DataTable
                        while (!reader.EndOfStream)
                        {
                            // Read a line from the file
                            string line = reader.ReadLine();

                            // Split the line using the "~" characters as a separator to get the field values
                            string[] fieldValues = line.Split(new string[] { "~" }, StringSplitOptions.None);

                            // Create a new row in the DataTable and add the field values as values
                            DataRow row = dataTable.NewRow();
                            for (int i = 0; i < fieldValues.Length; i++)
                            {
                                row[i] = fieldValues[i];
                            }
                            dataTable.Rows.Add(row);
                        }
                        // Add header row as a new row in the DataTable at the top
                        DataRow headerRow = dataTable.NewRow();
                        for (int i = 0; i < fieldNames.Length; i++)
                        {
                            headerRow[i] = fieldNames[i];
                        }
                        dataTable.Rows.InsertAt(headerRow, 0);
                    }

                    jsonResponse = _UpdateDemateHolder.UploadHolderDetails(dataTable, _loggedInUser.GetUserNameToDisplay());
                }
                jsonResponse.TotalRecords = dataTable.Rows.Count;
                //jsonResponse.IsSuccess = true;
                //jsonResponse.Message = "File Uploaded Sucessfully";
            }
            catch (Exception e)
            {
                jsonResponse.IsSuccess = false;
                jsonResponse.Message = e.Message;
            }


            return jsonResponse;
        }

        [HttpPost]
        public JsonResponse SaveRecord()
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                jsonResponse = _UpdateDemateHolder.SaveHolderDetails(_loggedInUser.GetUserNameToDisplay());
            }
            catch (Exception e)
            {
                jsonResponse.IsSuccess = false;
                jsonResponse.Message = e.Message;
            }
            return jsonResponse;
        }

    }
}
