

using CDSMODULE.Helper;
using Entity.CDS;
using Entity.Common;
using Entity.Reports;
using Interface.CDS;
using Interface.Common;
using Interface.Security;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Repository.Reports;
using System;
using System.Collections.Generic;
using System.IO;

namespace CDSMODULE.Areas.CDS.Controllers
{

    [Authorize]
    [Area("CDS")]
    [AutoValidateAntiforgeryToken]
    public class CertificateReversalEntryController : Controller
    {
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly ILoggedinUser _loggedInUser;
        private readonly IReversalEntry _reversalEntry;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IAudit _audit;

        public CertificateReversalEntryController(ICheckUserAccess checkUserAccess, ILoggedinUser _loggedInUser, IReversalEntry reversalEntry, IWebHostEnvironment hostingEnvironment, IAudit audit)
        {
            this._checkUserAccess = checkUserAccess;
            this._loggedInUser = _loggedInUser;
            this._reversalEntry = reversalEntry;
            this._hostingEnvironment = hostingEnvironment;
            _audit = audit;
        }

        public IActionResult Index()
        {
            ViewBag.username = _loggedInUser.GetUserNameToDisplay();

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
        public JsonResponse GetShHolderInformation(string CompCode, string ShHolderNo)
        {
            JsonResponse response = _reversalEntry.GetShholderInformation(CompCode, _loggedInUser.GetUserNameToDisplay(), ShHolderNo, _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }


        [HttpPost]
        public JsonResponse GetReversalCertifcateList(string CompCode, string SelectedAction, string CertNo = null)
        {
            JsonResponse response = _reversalEntry.GetReversalCertificateList(CompCode, _loggedInUser.GetUserNameToDisplay(), SelectedAction, CertNo);
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [HttpPost]
        public JsonResponse SaveReversalCertificate(string CompCode, string SelectedAction, string Remarks, string ApprovedDate, List<CertificateDemateDetails> certificates, string DRNNO, string ShHolderNo)
        {
            JsonResponse response = _reversalEntry.SaveReversalCertificate(CompCode, _loggedInUser.GetUserNameToDisplay(), SelectedAction, Remarks, ApprovedDate, certificates, DRNNO, ShHolderNo, _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        [HttpPost]
        public JsonResponse GetDataFromDRN(string CompCode, string ShholderNo, string DRNNO)
        {
            JsonResponse response = _reversalEntry.GetDataFromDRN(CompCode, _loggedInUser.GetUserNameToDisplay(), ShholderNo, DRNNO, _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }

        [HttpPost]
        public JsonResponse GenerateReport(string CompCode, string DateFrom, string DateTo, string CompEnName)
        {
            JsonResponse jsonResponse = new JsonResponse();
            jsonResponse = _reversalEntry.GenerateReport(CompCode, _loggedInUser.GetUserNameToDisplay(), DateFrom, DateTo);

            if (jsonResponse.HasError)
                jsonResponse = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)jsonResponse.ResponseData);
            else
            {
                if (jsonResponse.ResponseData != null && Convert.ToInt32(jsonResponse.TotalRecords) != 0)
                {
                    jsonResponse.IsSuccess = true;
                    string folderName = "PDFReports";
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string newPath = Path.Combine(webRootPath, folderName);
                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }
                    FileStream fs = new FileStream(_hostingEnvironment.WebRootPath + "\\PDFReports\\" + DateTime.Now.ToString("dd-MM-yyyy hh_mm_s_tt") + "Report.pdf", FileMode.Create, FileAccess.ReadWrite,
                    FileShare.None, 4096, FileOptions.DeleteOnClose);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        List<ATTTableHeader> tableHeaders = new List<ATTTableHeader>();
                        ATTTableHeader tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Rev No.";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Reg No.";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "H No.";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Name";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Cert No";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "SrNoFrom";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "SrNoTo";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "TotalShares";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "TrDate";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "ReverseDate";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "ReversedBy";
                        tableHeaders.Add(tableHeader);

                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Remarks";
                        tableHeaders.Add(tableHeader);


                        Document document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 30);
                        document.Open();
                        document.SetMargins(30, 30, 90, 30);

                        PdfWriter writer = PdfWriter.GetInstance(document, ms);
                        writer.PageEvent = new ITextEvents(CompCode, CompEnName, "Company : " + CompEnName + " Code : " + CompCode, tableHeaders, webRootPath);

                        Phrase phrase = null;
                        PdfPCell cell = null;
                        PdfPTable table = null;
                        BaseColor color = null;
                        document.Open();

                        //document.SetMargins(30, 30, 90, 30);


                        Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, 10);
                        Font font2 = FontFactory.GetFont(FontFactory.TIMES, 8);
                        Font fontBlue = FontFactory.GetFont(FontFactory.TIMES, 8, Font.NORMAL, BaseColor.BLUE);
                        Font fontNormal = FontFactory.GetFont(FontFactory.TIMES, 7, Font.NORMAL, BaseColor.BLACK);
                        Font fontBold = FontFactory.GetFont(FontFactory.TIMES, 8, Font.BOLD, BaseColor.BLACK);
                        Font fontHeadings = FontFactory.GetFont(FontFactory.TIMES, 9, Font.UNDERLINE, BaseColor.BLACK);


                        color = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#A9A9A9"));

                        List<ATTReversalCertificate> aTTCertDetails = (List<ATTReversalCertificate>)jsonResponse.ResponseData;
                        table = new PdfPTable(2);

                        table.TotalWidth = document.PageSize.Width - 30f;
                        table.LockedWidth = true;
                        document.Add(table);

                        //document.SetMargins(10, 10, 90, 30);

                        float[] columnDefinitionSize = { 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F };

                        table = new PdfPTable(columnDefinitionSize)
                        {
                            WidthPercentage = 100,

                        };

                        cell = new PdfPCell
                        {
                            BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)

                        };


                        aTTCertDetails.ForEach(x =>
                        {
                            CreateCell(table, x.rev_tran_no.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, x.regno.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, x.shholderno.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, x.name, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, x.certno.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, x.srnofrom.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, x.srnoto.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, x.shkitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, x.tr_date.ToString().Substring(0, 10), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, x.rev_date.ToString().Substring(0, 10), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, x.rev_by, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, x.rev_remarks, fontNormal, horizontalAlignment: PdfPCell.ALIGN_RIGHT);

                        });


                        document.Add(table);
                        table = new PdfPTable(1);
                        Paragraph lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_LEFT, 1)));
                        lineSeparator.SetLeading(1F, 1F);
                        document.Add(lineSeparator);
                        document.Add(table);

                        document.Close();
                        document.CloseDocument();
                        document.Dispose();
                        writer.Close();
                        writer.Dispose();
                        string file = Convert.ToBase64String(ms.ToArray());
                        jsonResponse.ResponseData = file;
                        jsonResponse.Message = "Company_" + CompEnName + "_Code_" + CompCode + "_ReversalCertificateReport.pdf";
                        fs.Close();
                        fs.Dispose();
                    }
                }
            }
            return jsonResponse;
        }

        private static void CreateCell(PdfPTable table, string text, Font font, int verticalAllignment = PdfPCell.ALIGN_BOTTOM, int horizontalAlignment = PdfPCell.ALIGN_RIGHT)
        {
            table.AddCell(PhraseCellNoBorder(new Phrase(new Chunk(text, font)), verticalAllignment: verticalAllignment, horizontalAlignment: horizontalAlignment));
        }

        private static PdfPCell PhraseCellNoBorder(Phrase phrase, int verticalAllignment = PdfPCell.ALIGN_BOTTOM, int horizontalAlignment = PdfPCell.ALIGN_RIGHT)
        {
            PdfPCell cell = new PdfPCell(phrase);
            cell.VerticalAlignment = verticalAllignment;
            cell.HorizontalAlignment = horizontalAlignment;
            cell.BorderColor = BaseColor.WHITE;
            cell.PaddingBottom = 0f;
            cell.PaddingTop = 50f;

            return cell;
        }
        private static PdfPCell PhraseCell(Phrase phrase, int verticalAllignment = PdfPCell.ALIGN_BOTTOM, int horizontalAlignment = PdfPCell.ALIGN_RIGHT)
        {
            PdfPCell cell = new PdfPCell(phrase);
            cell.VerticalAlignment = verticalAllignment;
            cell.HorizontalAlignment = horizontalAlignment;
            cell.PaddingBottom = 2f;
            cell.PaddingTop = 0f;
            return cell;
        }
    }
}
