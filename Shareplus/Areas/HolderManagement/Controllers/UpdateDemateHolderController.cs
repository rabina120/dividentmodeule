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
using System;
using System.Data;
using System.IO;

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
        public JsonResponse UploadFile()
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                DataTable dataTable = new DataTable();
                IFormFile file = Request.Form.Files[0];
                string folderName = "UploadExcel\\UpdateDemateHolder";
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
