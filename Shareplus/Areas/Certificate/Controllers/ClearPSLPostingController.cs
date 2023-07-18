﻿

using CDSMODULE.Helper;
using ClosedXML.Excel;
using Entity.Certificate;
using Entity.Common;
using Entity.Reports;
using Interface.Certificate;
using Interface.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CDSMODULE.Areas.Certificate.Controllers
{
    [Authorize]
    [Area("Certificate")]
    [AutoValidateAntiforgeryToken]
    public class ClearPSLPostingController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IClearPSLPosting clearPSLPosting;
        private readonly ICheckUserAccess _checkUserAccess;

        public ClearPSLPostingController(ILoggedinUser _loggedInUser, IClearPSLPosting clearPSLPosting,ICheckUserAccess checkUserAccess)
        {

            this._loggedInUser = _loggedInUser;
            this.clearPSLPosting = clearPSLPosting;
            this._checkUserAccess = checkUserAccess;

        }
        public IActionResult Index()
        {
            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();
            if (_checkUserAccess.CheckIfAccessible(_loggedInUser.GetUserId(), this.ControllerContext))
                return View();
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }
        [HttpPost]
        public JsonResponse GetClearPSLPostingCompanyData(string CompCode)
        {
            return clearPSLPosting.GetClearPSLPostingCompanyData(CompCode, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
        }

        [HttpPost]
        public JsonResponse GetSingleClearPSLData(string CompCode, string PSL_CLEAR_NO, int ShholderNo)
        {
            return clearPSLPosting.GetSingleClearPSLData(CompCode, PSL_CLEAR_NO, ShholderNo);
        }

        [HttpPost]

        public JsonResponse PostPSLClearPosting(List<ATTClearPSLPosting> aTTpSLClearPostings, ATTClearPSLPosting recorddetails, string SelectedAction)
        {
            return clearPSLPosting.PostPSLClearPosting(aTTpSLClearPostings, recorddetails, SelectedAction, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
        }
        [HttpGet]

        public JsonResponse ViewReport(string CompCode, string ReportType)
        {
            JsonResponse response = new JsonResponse();
            JsonResponse returnedResponse = new JsonResponse();

            response = clearPSLPosting.ViewReport(CompCode, ReportType, _loggedInUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
            List<ATTClearPSLViewReport> pslReport = new List<ATTClearPSLViewReport>();
            pslReport = (List<ATTClearPSLViewReport>)response.ResponseData;
            if (response.IsSuccess)
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("ClearPSLReport");

                    PropertyInfo[] properties = pslReport.First().GetType().GetProperties();
                    List<string> headerNames = properties.Select(prop => prop.Name).ToList();
                    for (int i = 0; i < headerNames.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = headerNames[i];

                    }
                    worksheet.Cell(2, 1).InsertData(pslReport);
                    //worksheet.Cells[pslReport.Count + 2, 9] = "Total";
                    //worksheet.Cell(pslReport.Count + 2, 9).Value = "Total";
                    //worksheet.Cell(pslReport.Count + 2, 10).Value = (pslReport.Sum(x => Convert.ToInt32(x.Kitta))).ToString();


                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        String file = Convert.ToBase64String(stream.ToArray());
                        returnedResponse.IsSuccess = true;
                        returnedResponse.ResponseData = file;
                        returnedResponse.Message = "_Code_" + CompCode + "ClearPSLReport.xlsx";
                        return returnedResponse;

                    }


                }
            }
            else
            {
                returnedResponse.Message = response.Message;
                return returnedResponse;
            }
        }
    }
}
