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
    public class SummaryReportController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IGenerateReport _generateReport;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;

        public SummaryReportController(ILoggedinUser _loggedInUser, IGenerateReport generateReport,
            IWebHostEnvironment hostingEnvironment, ICheckUserAccess checkUserAccess, IAudit audit)
        {
            this._loggedInUser = _loggedInUser;
            _generateReport = generateReport;
            _hostingEnvironment = hostingEnvironment;
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
        public JsonResponse GenerateReport(string CompCode, string CompEnName, string ReportType)
        {
            JsonResponse jsonResponse = new JsonResponse();
            JsonResponse jsonResponseToReturn = new JsonResponse();

            jsonResponse = _generateReport.GenerateReport(CompCode, ReportType, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
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
                        Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, 10);
                        Font font2 = FontFactory.GetFont(FontFactory.TIMES, 8);
                        Font fontBlue = FontFactory.GetFont(FontFactory.TIMES, 8, Font.NORMAL, BaseColor.BLUE);
                        Font fontNormal = FontFactory.GetFont(FontFactory.TIMES, 8, Font.NORMAL, BaseColor.BLACK);
                        Font fontBold = FontFactory.GetFont(FontFactory.TIMES, 8, Font.BOLD, BaseColor.BLACK);
                        Font fontHeadings = FontFactory.GetFont(FontFactory.TIMES, 9, Font.UNDERLINE, BaseColor.BLACK);

                        BaseColor color = null;

                        color = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#A9A9A9"));

                        if (ReportType == "S")
                        {
                            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                            PdfWriter writer = PdfWriter.GetInstance(document, ms);


                            Phrase phrase = null;
                            PdfPCell cell = null;
                            PdfPTable table = null;
                            document.Open();

                            ATTExportSummaryReport summaryReport = (ATTExportSummaryReport)jsonResponse.ResponseData;

                            table = new PdfPTable(2);
                            table.TotalWidth = 500f;
                            table.LockedWidth = true;

                            table.AddCell(PhraseCellNoBorder(new Phrase(new Chunk("Company : " + CompEnName + " Code : " + CompCode + " Summary Report \n\n", FontFactory.GetFont("TIMES", 12, Font.BOLD, BaseColor.RED))), horizontalAlignment: PdfPCell.ALIGN_RIGHT));
                            table.AddCell(PhraseCellNoBorder(new Phrase(new Chunk(DateTime.Now.ToString("dd/MM/yyyy") + "\n\n", FontFactory.GetFont("TIMES", 8, Font.BOLD, BaseColor.BLACK)))));
                            document.Add(table);
                            Paragraph lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_CENTER, 1)));
                            lineSeparator.SetLeading(0.5F, 0.5F);
                            document.Add(lineSeparator);


                            table = new PdfPTable(1);
                            document.Add(table);
                            table = new PdfPTable(1);


                            float[] columnDefinitionSize = { 3F, 2F, 2F };
                            table = new PdfPTable(columnDefinitionSize)
                            {
                                WidthPercentage = 100
                            };

                            cell = new PdfPCell
                            {
                                BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)
                            };

                            //first row
                            //headings
                            table = new PdfPTable(columnDefinitionSize)
                            {
                                WidthPercentage = 100
                            };
                            CreateCell(table, "Certificate Report:", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Total Number:", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Total Kitta:", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            document.Add(table);



                            //phrases front
                            table = new PdfPTable(columnDefinitionSize)
                            {
                                WidthPercentage = 100
                            };
                            CreateCell(table, "Normal Certificate:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.NormalCertificate.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.NormalKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Certificate Under Pledge:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.PledgeCertificate.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.PledgeKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Certificate Under Suspend:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.SuspendCertificate.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.SuspendKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Certificate Under Lost:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.LostCertificate.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.LostKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Demate Certificate:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.DemateCertificate.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.DemateKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Demate Certificate Under Process:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.DemateUnderProcessCertificate.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.DemateUnderProcessKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);

                            CreateCell(table, "Total : ", fontBlue, horizontalAlignment: PdfPCell.ALIGN_RIGHT);
                            CreateCell(table, (summaryReport.NormalCertificate + summaryReport.PledgeCertificate + summaryReport.SuspendCertificate + summaryReport.LostCertificate + summaryReport.DemateCertificate + summaryReport.DemateUnderProcessCertificate).ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, (summaryReport.NormalKitta + summaryReport.PledgeKitta + summaryReport.SuspendKitta + summaryReport.LostKitta + summaryReport.DemateKitta + summaryReport.DemateUnderProcessKitta).ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            document.Add(table);
                            lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_CENTER, 1)));
                            lineSeparator.SetLeading(0.5F, 0.5F);
                            document.Add(lineSeparator);

                            //Second Part
                            table = new PdfPTable(columnDefinitionSize)
                            {
                                WidthPercentage = 100
                            };
                            CreateCell(table, "\nCertificates Under Consolidated:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.ConsolidatedCertificate.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.ConsolidatedKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Certificate Splitted:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.SplittedCertificate.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.SplittedKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Duplicate Certificates:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.DuplicateCertificate.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.DuplicateKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);


                            document.Add(table);
                            lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_CENTER, 1)));
                            lineSeparator.SetLeading(0.5F, 0.5F);
                            document.Add(lineSeparator);


                            //Third Part
                            table = new PdfPTable(columnDefinitionSize)
                            {
                                WidthPercentage = 100
                            };
                            CreateCell(table, "\n Promoter Certificates:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.PromoterCertificate.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.PromoterKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Staff Certificate:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.StaffCertificate.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.StaffKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Public Certificate:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.PublicCertificate.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.PublicKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);


                            CreateCell(table, "Total : ", fontBlue, horizontalAlignment: PdfPCell.ALIGN_RIGHT);
                            CreateCell(table, (summaryReport.PromoterCertificate + summaryReport.StaffCertificate + summaryReport.PublicCertificate).ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, (summaryReport.PromoterKitta + summaryReport.StaffKitta + summaryReport.PublicKitta).ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            document.Add(table);
                            lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_CENTER, 1)));
                            lineSeparator.SetLeading(0.5F, 0.5F);
                            document.Add(lineSeparator);


                            //Fourth Part
                            table = new PdfPTable(columnDefinitionSize)
                            {
                                WidthPercentage = 100
                            };
                            CreateCell(table, "\n Untransfered Certificates:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.UntransferedCertificate.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, " ", fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Transfered Certificate:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.TransferedCertificate.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, " ", fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);

                            CreateCell(table, "Total : ", fontBlue, horizontalAlignment: PdfPCell.ALIGN_RIGHT);
                            CreateCell(table, (summaryReport.UntransferedCertificate + summaryReport.TransferedCertificate).ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, " ", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            document.Add(table);
                            lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_CENTER, 1)));
                            lineSeparator.SetLeading(0.5F, 0.5F);
                            document.Add(lineSeparator);

                            //Fifth Part
                            table = new PdfPTable(columnDefinitionSize)
                            {
                                WidthPercentage = 100
                            };
                            CreateCell(table, "\n Ordinary Certificates:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.OrdinaryCertificate.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.OrdinaryKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Preferencial Certificates:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.PreferencialCertificate.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.PreferencialKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Bonus Certificates:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.BonusCertificate.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.BonusKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Right Share Certificates:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.RightShareCertificate.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.RightShareKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);

                            CreateCell(table, "Total : ", fontBlue, horizontalAlignment: PdfPCell.ALIGN_RIGHT);
                            CreateCell(table, (summaryReport.OrdinaryCertificate + summaryReport.BonusCertificate + summaryReport.PreferencialCertificate + summaryReport.RightShareCertificate).ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, (summaryReport.OrdinaryKitta + summaryReport.BonusKitta + summaryReport.PreferencialKitta + summaryReport.RightShareKitta).ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            document.Add(table);
                            lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_CENTER, 1)));
                            lineSeparator.SetLeading(0.5F, 0.5F);
                            document.Add(lineSeparator);

                            //Sixth Part
                            table = new PdfPTable(columnDefinitionSize)
                            {
                                WidthPercentage = 100
                            };
                            CreateCell(table, "\n Distributed Certificates:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.DistCertificate.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.DistCertificateKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Undistributed Certificate:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.UnDistCertificate.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.UnDistCertificateKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);

                            CreateCell(table, "Total : ", fontBlue, horizontalAlignment: PdfPCell.ALIGN_RIGHT);
                            CreateCell(table, (summaryReport.DistCertificate + summaryReport.UnDistCertificate).ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, (summaryReport.DistCertificateKitta + summaryReport.UnDistCertificateKitta).ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            document.Add(table);
                            lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_CENTER, 1)));
                            lineSeparator.SetLeading(0.5F, 0.5F);
                            document.Add(lineSeparator);

                            //Seventh Part
                            table = new PdfPTable(columnDefinitionSize)
                            {
                                WidthPercentage = 100
                            };
                            CreateCell(table, "\n Holder Reports:", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, " ", fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, " ", fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Total No. Of Share Holders:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.TotalShareHolder.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.MKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Total No. Of Minor Share Holders:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.TotalMinorShareHolder.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.MinorKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Total Fraction Shares:", fontBlue, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, summaryReport.TotalFrac.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);

                            CreateCell(table, "Total Share Holders : ", fontBlue, horizontalAlignment: PdfPCell.ALIGN_RIGHT);
                            CreateCell(table, (summaryReport.TotalShareHolder + summaryReport.TotalMinorShareHolder).ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Total Kitta: " + (summaryReport.MKitta + summaryReport.TotalFrac).ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            document.Add(table);
                            lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_CENTER, 1)));
                            lineSeparator.SetLeading(0.5F, 0.5F);
                            document.Add(lineSeparator);
                            table = new PdfPTable(columnDefinitionSize)
                            {
                                WidthPercentage = 100
                            };
                            CreateCell(table, " ", fontBlue, horizontalAlignment: PdfPCell.ALIGN_RIGHT);
                            CreateCell(table, " ", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Frac Kitta: " + summaryReport.TotalFrac.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, " ", fontBlue, horizontalAlignment: PdfPCell.ALIGN_RIGHT);
                            CreateCell(table, " ", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Total Kitta With Frac:  " + Math.Round(Convert.ToDouble((summaryReport.MKitta + summaryReport.MinorKitta + summaryReport.TotalFrac)), 2).ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            document.Add(table);
                            lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_CENTER, 1)));
                            lineSeparator.SetLeading(0.5F, 0.5F);
                            document.Add(lineSeparator);


                            document.Close();
                            document.CloseDocument();
                            document.Dispose();
                            writer.Close();
                            writer.Dispose();
                            string file = Convert.ToBase64String(ms.ToArray());
                            jsonResponseToReturn.IsSuccess = true;
                            jsonResponseToReturn.ResponseData = file;
                            jsonResponseToReturn.Message = "Company_" + CompEnName + "_Code_" + CompCode + "_SummaryReport.pdf";
                            fs.Close();
                            fs.Dispose();
                        }
                        else
                        {
                            List<ATTTableHeader> tableHeaders = new List<ATTTableHeader>();
                            ATTTableHeader tableHeader = new ATTTableHeader();
                            tableHeader.ColumnDefinition = 0.25f;
                            tableHeader.ColumnName = "S.N.";
                            tableHeaders.Add(tableHeader);
                            tableHeader = new ATTTableHeader();
                            tableHeader.ColumnDefinition = 1f;
                            tableHeader.ColumnName = "Share Kitta";
                            tableHeaders.Add(tableHeader);
                            tableHeader = new ATTTableHeader();
                            tableHeader.ColumnDefinition = 1f;
                            tableHeader.ColumnName = "No. of Holders";
                            tableHeaders.Add(tableHeader);
                            tableHeader = new ATTTableHeader();
                            tableHeader.ColumnDefinition = 1f;
                            tableHeader.ColumnName = "Total Kitta";
                            tableHeaders.Add(tableHeader);
                            tableHeader = new ATTTableHeader();
                            tableHeader.ColumnDefinition = 1f;
                            tableHeader.ColumnName = "Holding %";
                            tableHeaders.Add(tableHeader);

                            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                            PdfWriter writer = PdfWriter.GetInstance(document, ms);
                            writer.PageEvent = new ITextEvents(CompCode, CompEnName, "Kitta Wise Report", tableHeaders, webRootPath);

                            Phrase phrase = null;
                            PdfPCell cell = null;
                            PdfPTable table = null;
                            document.Open();

                            document.SetMargins(30, 30, 90, 30);
                            List<ATTKittaWiseReport> kittaWiseReports = (List<ATTKittaWiseReport>)jsonResponse.ResponseData;



                            table = new PdfPTable(2);
                            table.TotalWidth = document.PageSize.Width - 30f;
                            table.LockedWidth = true;
                            table.AddCell(PhraseCellNoBorder(new Paragraph(string.Empty)));
                            table.AddCell(PhraseCellNoBorder(new Paragraph(string.Empty)));
                            table.SpacingAfter = 40f;
                            document.Add(table);

                            float[] columnDefinitionSize = { 0.25F, 1F, 1F, 1F, 1F };
                            table = new PdfPTable(columnDefinitionSize)
                            {
                                WidthPercentage = 100,
                                TotalWidth = document.PageSize.Width - 40f,
                                LockedWidth = true,

                            };

                            cell = new PdfPCell
                            {
                                BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)
                            };

                            double? holdingSum = 0;
                            int? holderNoSum = 0;
                            for (int i = 0; i < kittaWiseReports.Count; i++)
                            {
                                CreateCell(table, (i + 1).ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                CreateCell(table, kittaWiseReports[i].ShareKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                CreateCell(table, kittaWiseReports[i].NumberOfHolder.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                CreateCell(table, kittaWiseReports[i].TotalKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                CreateCell(table, kittaWiseReports[i].HoldingPercentage.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                holdingSum += Math.Round(Convert.ToDouble(kittaWiseReports[i].HoldingPercentage), 5);
                                holderNoSum += kittaWiseReports[i].NumberOfHolder;
                            }
                            table.SpacingBefore = 20f;
                            document.Add(table);
                            table = new PdfPTable(1);
                            Paragraph lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_CENTER, 1)));
                            lineSeparator.SetLeading(0.5F, 0.5F);
                            document.Add(lineSeparator);

                            table = new PdfPTable(columnDefinitionSize)
                            {
                                WidthPercentage = 100
                            };

                            CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                            CreateCell(table, "Total :", fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, holderNoSum.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, kittaWiseReports[0].TTotalKitta.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                            CreateCell(table, holdingSum.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);

                            document.Add(table);

                            document.Close();
                            document.CloseDocument();
                            document.Dispose();
                            writer.Close();
                            writer.Dispose();
                            String file = Convert.ToBase64String(ms.ToArray());
                            jsonResponseToReturn.IsSuccess = true;
                            jsonResponseToReturn.ResponseData = file;
                            jsonResponseToReturn.Message = "Company_" + CompEnName + "_Code_" + CompCode + "_KittaWiseReport.pdf";
                            fs.Close();
                            fs.Dispose();
                        }



                        return jsonResponseToReturn;
                    }
                }
                else
                {
                    jsonResponseToReturn = jsonResponse;
                }
            }


            return jsonResponseToReturn;
        }


        private static void DrawLine(PdfWriter writer, float x1, float y1, float x2, float y2, BaseColor color)
        {
            PdfContentByte contentByte = writer.DirectContent;
            contentByte.SetColorStroke(color);
            contentByte.MoveTo(x1, y1);
            contentByte.LineTo(x2, y2);
            contentByte.Stroke();
        }
        private static PdfPCell PhraseCellNoBorder(Phrase phrase, int verticalAllignment = PdfPCell.ALIGN_BOTTOM, int horizontalAlignment = PdfPCell.ALIGN_RIGHT)
        {
            PdfPCell cell = new PdfPCell(phrase);
            cell.VerticalAlignment = verticalAllignment;
            cell.HorizontalAlignment = horizontalAlignment;
            cell.BorderColor = BaseColor.WHITE;
            cell.PaddingBottom = 4f;
            cell.PaddingTop = 6f;
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

