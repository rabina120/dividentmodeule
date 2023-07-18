using CDSMODULE.Helper;
using Entity.Common;
using Entity.Reports;
using Interface.Common;
using Interface.Reports;
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

namespace CDSMODULE.Areas.Reports.Controllers
{
    [Area("Reports")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class DemateHolderListController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IGenerateReport _generateReport;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;


        public DemateHolderListController(ILoggedinUser _loggedInUser, IAudit audit,
            IWebHostEnvironment hostingEnvironment, IGenerateReport generateReport, ICheckUserAccess checkUserAccess)
        {
            this._loggedInUser = _loggedInUser;
            _hostingEnvironment = hostingEnvironment;
            _generateReport = generateReport;
            this._checkUserAccess = checkUserAccess;
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
        public JsonResponse GenerateReport(string CompCode, string CompEnName, string DataUploadDate)
        {
            JsonResponse jsonResponse = new JsonResponse();
            JsonResponse jsonResponseToReturn = new JsonResponse();

            jsonResponse = _generateReport.GenerateReportDemateHolderList(CompCode, DataUploadDate, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());

            if (jsonResponse.HasError)
            {
                jsonResponseToReturn = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)jsonResponse.ResponseData);

            }
            else
            {
                if (jsonResponse.IsSuccess)
                {
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
                        //tableHeader.ColumnDefinition = 0.5f;
                        //tableHeader.ColumnName = "S.N.";
                        //tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "HolderNo";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 2f;
                        tableHeader.ColumnName = "BOID";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Name";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Fathers's Name";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "GrandFather's Name";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Address";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "City";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "State";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Contact";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Kitta";
                        tableHeaders.Add(tableHeader);

                        Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                        PdfWriter writer = PdfWriter.GetInstance(document, ms);
                        writer.PageEvent = new ITextEvents(CompCode, CompEnName, "Company : " + CompEnName + " Code : " + CompCode + " Demate Holder List Report UpTo: " + DataUploadDate, tableHeaders, webRootPath);

                        Phrase phrase = null;
                        PdfPCell cell = null;
                        PdfPTable table = null;
                        BaseColor color = null;
                        document.Open();

                        document.SetMargins(30, 30, 90, 30);


                        Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8);
                        Font font2 = FontFactory.GetFont(FontFactory.TIMES, 8);
                        Font fontBlue = FontFactory.GetFont(FontFactory.TIMES, 8, Font.NORMAL, BaseColor.BLUE);
                        Font fontNormal = FontFactory.GetFont(FontFactory.TIMES, 8, Font.NORMAL, BaseColor.BLACK);
                        Font fontBold = FontFactory.GetFont(FontFactory.TIMES, 8, Font.BOLD, BaseColor.BLACK);
                        Font fontHeadings = FontFactory.GetFont(FontFactory.TIMES, 9, Font.UNDERLINE, BaseColor.BLACK);


                        color = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#A9A9A9"));


                        List<ATTDemateHolderList> demateHolderList = (List<ATTDemateHolderList>)jsonResponse.ResponseData;
                        table = new PdfPTable(2);

                        table.TotalWidth = document.PageSize.Width - 30f;
                        table.LockedWidth = true;
                        table.AddCell(PhraseCellNoBorder(new Paragraph(string.Empty)));
                        table.AddCell(PhraseCellNoBorder(new Paragraph(string.Empty)));
                        table.SpacingAfter = 40f;
                        document.Add(table);

                        document.SetMargins(30, 30, 90, 30);

                        float[] columnDefinitionSize = { 1F, 2F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F };
                        table = new PdfPTable(columnDefinitionSize)
                        {
                            WidthPercentage = 100
                        };

                        cell = new PdfPCell
                        {
                            BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)
                        };


                        int? totalDematedKitta = 0;
                        demateHolderList.ForEach(x =>
                        {
                            CreateCell(table, x.ShHolderNo, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, x.bo_acct_no, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, x.first_name == "" ? "---" : x.first_name, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, x.faname, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, x.grfaname, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, x.bo_add_1, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, x.bo_addr_city, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, x.bo_addr_state, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, x.bo_tel_no1, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, x.totalkitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            totalDematedKitta += x.totalkitta;
                        });

                        table.SpacingBefore = 20f;

                        document.Add(table);
                        table = new PdfPTable(1);
                        Paragraph lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_LEFT, 1)));
                        lineSeparator.SetLeading(0.5F, 0.5F);
                        document.Add(lineSeparator);
                        CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        CreateCell(table, "Total Kitta:", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        CreateCell(table, totalDematedKitta.ToString(), fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        document.Add(table);

                        document.Close();
                        document.CloseDocument();
                        document.Dispose();
                        writer.Close();
                        writer.Dispose();
                        String file = Convert.ToBase64String(ms.ToArray());
                        jsonResponseToReturn.IsSuccess = true;
                        jsonResponseToReturn.ResponseData = file;
                        jsonResponseToReturn.Message =  CompEnName  + "_DemateHolderListReport.pdf";
                        fs.Close();
                        fs.Dispose();
                    }
                }

                else
                {
                    jsonResponseToReturn = jsonResponse;
                }

            }



            return jsonResponseToReturn;
        }





        private static PdfPCell PhraseCellNoBorder(Phrase phrase, int verticalAllignment = PdfPCell.ALIGN_BOTTOM, int horizontalAlignment = PdfPCell.ALIGN_RIGHT)
        {
            PdfPCell cell = new PdfPCell(phrase);
            cell.VerticalAlignment = verticalAllignment;
            cell.HorizontalAlignment = horizontalAlignment;
            cell.BorderColor = BaseColor.WHITE;
            cell.PaddingBottom = 4f;
            cell.PaddingTop = 4f;
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

        private static void CreateCell(PdfPTable table, string text, Font font, int verticalAllignment = PdfPCell.ALIGN_BOTTOM, int horizontalAlignment = PdfPCell.ALIGN_RIGHT)
        {
            table.AddCell(PhraseCellNoBorder(new Phrase(new Chunk(text, font)), verticalAllignment: verticalAllignment, horizontalAlignment: horizontalAlignment));
        }
    }
}

