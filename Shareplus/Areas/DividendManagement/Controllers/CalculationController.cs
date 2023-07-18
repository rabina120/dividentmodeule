using ClosedXML.Excel;
using Entity.Common;

using ENTITY.Dividend;
using ExcelDataReader;
using INTERFACE.DividendManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Shareplus.Areas.DividendManagement.Controllers
{
    [Authorize]
    [Area("DividendManagement")]
    [AutoValidateAntiforgeryToken]
    public class CalculationController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private IConfiguration Configuration;
        private readonly ICalculationRepo _calculationRepo;
        public CalculationController(IWebHostEnvironment webHostEnvironment, IConfiguration _Configuration, ICalculationRepo calculationRepo)
        {
            _webHostEnvironment = webHostEnvironment;
            Configuration = _Configuration;
            _calculationRepo = calculationRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string SaveCalculation(ATTCalculation data,string selectedOption)
        {
            JsonResponse response = new JsonResponse();
            response = _calculationRepo.SaveCalculation(data,selectedOption);
            return JsonConvert.SerializeObject(response);
        }
        [HttpPost]
        public string GetAllCalculationData(string selectedOption,string CompanyId,int? pageNo,int? pageSize)
        {
            JsonResponse response = new JsonResponse();
           response.ResponseData = _calculationRepo.GetAllCalclationData(selectedOption,CompanyId,pageNo,pageSize,out int TotalRecords);
            response.TotalRecords = TotalRecords;
            return JsonConvert.SerializeObject(response);
        }


        [HttpGet]
        public ActionResult DownloadExcelDocument(string selectedoption)
        {
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "DummyExcelCalculation.xlsx";
            
            using (var workbook = new XLWorkbook())
            {
                if (selectedoption == "P")
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Add("DataFormat");
                    worksheet.Cell(1, 1).Value = "shHolderNo";
                    worksheet.Cell(1, 2).Value = "HolderName";
                    worksheet.Cell(1, 3).Value = "Address";
                    worksheet.Cell(1, 4).Value = "TotShKitta";
                    worksheet.Cell(1, 5).Value = "FractionKitta";
                    worksheet.Cell(1, 6).Value = "Total";
                }
                else
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Add("DataFormat");
                    worksheet.Cell(1, 1).Value = "BO_idno";
                    worksheet.Cell(1, 2).Value = "fullname";
                    worksheet.Cell(1, 3).Value = "Address";
                    worksheet.Cell(1, 4).Value = "TotShKitta";
                    worksheet.Cell(1, 5).Value = "FractionKitta";
                    worksheet.Cell(1, 6).Value = "Total";
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
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        [RequestSizeLimit(10L * 1024L * 1024L * 1024L)]
        public JsonResponse GetSheetNames(string CompCode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                List<double> currentOLEDBVersion = ExcelConfig.GetOLEDBVersion();

                IFormFile file = Request.Form.Files[0];
                string folderName = "UploadExcel\\Calculation";
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
                    string fullPath = Path.Combine(newPath, "CompCode_" + CompCode + "_" + DateTime.Now.ToString("yyyy_MM_dd") + sFileExtension);
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
        public JsonResponse UploadSheet(int SheetId, string CompCode,string selectedOption)
        {
            JsonResponse jsonResponse = new JsonResponse();

            IFormFile postedFile = Request.Form.Files[0];
            DataTable dt1 = new DataTable();


            if (SheetId != null)
            {
                try
                {
                    // Create a Folder.
                    string path = Path.Combine(this._webHostEnvironment.WebRootPath, "UploadExcel\\Calculation");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    //Save the uploaded Excel file.
                    string extension = Path.GetExtension(postedFile.FileName);
                    string fileName = "CompCode_" + CompCode + "_" + DateTime.Now.ToString("yyyy_MM_dd") + extension;
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

                        #region to insert by Procedure 
                        //Insert the Data read from the Excel file to Database Table.
                        //conString = Configuration.GetConnectionString("DefaultConnection");
                        //DataTable dt = ds.Tables[SheetId];
                        //List<ATTCalculation> lst = new List<ATTCalculation>();
                        //foreach (DataRow row in dt.Rows)
                        //{
                        //    ATTCalculation obj = new ATTCalculation();
                        //    if (selectedOption == "P")
                        //    {
                        //        obj.CompanyId = CompCode;
                        //        obj.HolderNo = Convert.ToInt32(row["HolderNo"]);
                        //        obj.HolderName = row["HolderName"].ToString();
                        //        obj.Address = row["Address"].ToString();
                        //        obj.TotalKitta = row["TotalKitta"].ToString();
                        //        obj.FractionKitta = decimal.Parse(row["FractionKitta"].ToString(), System.Globalization.NumberStyles.Float);
                        //        obj.Total = decimal.Parse(row["Total"].ToString(), System.Globalization.NumberStyles.Float);
                        //    }
                        //    else
                        //    {
                        //        obj.CompanyId = CompCode;
                        //        obj.Boid = row["Boid"].ToString();
                        //        obj.HolderName = row["HolderName"].ToString();
                        //        obj.Address = row["Address"].ToString();
                        //        obj.TotalKitta = row["TotalKitta"].ToString();
                        //        obj.FractionKitta = decimal.Parse(row["FractionKitta"].ToString(), System.Globalization.NumberStyles.Float);
                        //        obj.Total = decimal.Parse(row["Total"].ToString(), System.Globalization.NumberStyles.Float);
                        //    }
                        //    lst.Add(obj);
                        //}

                        //JsonResponse response = new JsonResponse();
                        //response = _calculationRepo.SaveCalculationFromExcel(lst,selectedOption);

                        //jsonResponse.ResponseData = lst;
                        //jsonResponse.IsSuccess = true;
                        ////return jsonResponse;
                        #endregion


                        #region Bulkcopy

                        //Insert the Data read from the Excel file to Database Table.

                        DataTable dt = ds.Tables[SheetId];
                        dt.Columns.Add(new DataColumn("compcode", typeof(string)));

                        foreach (DataRow row in dt.Rows)
                        {
                            row["compcode"] = CompCode.Substring(0, 3);
                        }

                        var sourcearrayNames = (from DataColumn x
                          in dt.Columns.Cast<DataColumn>()
                                                select x.ColumnName).ToArray();

                        jsonResponse = _calculationRepo.BulkCopyCalculationData(ds, dt, selectedOption);
                        #endregion
                    }

                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;
                    //_audit.errorSave(_loggedInUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), ex);
                }
            }
            return jsonResponse;
        }


        [HttpPost]
        public string Calculate( string compcode, string selectedOption,string Bonus,string Divident /*,string args*/)
        {
            JsonResponse response = new JsonResponse();
           // List<ATTCalculation> data = JsonConvert.DeserializeObject<List<ATTCalculation>>(args);
            response = _calculationRepo.Calculate(selectedOption,Bonus,Divident, compcode);
            return JsonConvert.SerializeObject(response);
        }


    }
}
