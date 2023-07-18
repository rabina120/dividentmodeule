using Entity.Certificate;
using Entity.Common;
using Entity.Reports;
using Interface.Reports;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Options;
using Repository.Reports;
using System;
using System.Collections.Generic;
using System.IO;

namespace REPOSITORY.Reports
{
    public class CertificateReports : ICertificateReports
    {

        IOptions<ReadConfig> _connectionString;

        public CertificateReports(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;

        }


        public JsonResponse CertificateConsolidateReport(ATTConsolidateReport ReportData, List<ATTConsolidateReport> ConsolidateList, string rootPath)

        {
            JsonResponse jsonResponseToReturn = new JsonResponse();
            string folderName = "PDFReports";
            string webRootPath = rootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            FileStream fs = new FileStream(rootPath + "\\PDFReports\\" + DateTime.Now.ToString("dd-MM-yyyy hh_mm_s_tt") + "Report.pdf", FileMode.Create, FileAccess.ReadWrite,
            FileShare.None, 4096, FileOptions.DeleteOnClose);

            using (MemoryStream ms = new MemoryStream())
            {

                List<ATTTableHeader> tableHeaders = new List<ATTTableHeader>();
                ATTTableHeader tableHeader = new ATTTableHeader();

                tableHeader = new ATTTableHeader();
                tableHeader.ColumnDefinition = 1F;
                tableHeader.ColumnName = "Split No";
                tableHeaders.Add(tableHeader);
                tableHeader = new ATTTableHeader();
                tableHeader.ColumnDefinition = 1F;
                tableHeader.ColumnName = "Shholderno";
                tableHeaders.Add(tableHeader);
                tableHeader = new ATTTableHeader();
                tableHeader.ColumnDefinition = 1F;
                tableHeader.ColumnName = "Full Name";
                tableHeaders.Add(tableHeader);
                tableHeader = new ATTTableHeader();
                tableHeader.ColumnDefinition = 1F;
                tableHeader.ColumnName = "Seqno";
                tableHeaders.Add(tableHeader);
                tableHeader = new ATTTableHeader();
                tableHeader.ColumnDefinition = 1F;
                tableHeader.ColumnName = "Certno";
                tableHeaders.Add(tableHeader);
                tableHeader = new ATTTableHeader();
                tableHeader.ColumnDefinition = 1F;
                tableHeader.ColumnName = "kitta";
                tableHeaders.Add(tableHeader);
                tableHeader = new ATTTableHeader();
                tableHeader.ColumnDefinition = 1F;
                tableHeader.ColumnName = "SrNoFrom";
                tableHeaders.Add(tableHeader);
                tableHeader = new ATTTableHeader();
                tableHeader.ColumnDefinition = 1F;
                tableHeader.ColumnName = "SrNoTo";
                tableHeaders.Add(tableHeader);
                tableHeader = new ATTTableHeader();

                tableHeader.ColumnDefinition = 1F;
                tableHeader.ColumnName = "split dt";
                tableHeaders.Add(tableHeader);
                tableHeader = new ATTTableHeader();
                tableHeader.ColumnDefinition = 1F;
                tableHeader.ColumnName = "split remarks";
                tableHeaders.Add(tableHeader);
                Document document = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                string Header = "";

                if (ReportData.DataType == "P")
                {
                    Header = "Posted Certificate List";
                }
                else if (ReportData.DataType == "U")
                {
                    Header = "Unposted Certificate List";
                }
                else
                {
                    Header = "Rejected Certificate List";
                }


                writer.PageEvent = new ITextEvents(ReportData.CompCode.ToString(), ReportData.CompEnName, Header, tableHeaders, webRootPath);

                Phrase phrase = null;
                PdfPCell cell = null;
                PdfPTable table = null;
                BaseColor color = null;
                document.Open();

                document.SetMargins(25, 30, 90, 30);


                Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, 10);
                Font font2 = FontFactory.GetFont(FontFactory.TIMES, 8);
                Font fontBlue = FontFactory.GetFont(FontFactory.TIMES, 8, Font.NORMAL, BaseColor.BLUE);
                Font fontNormal = FontFactory.GetFont(FontFactory.TIMES, 8, Font.NORMAL, BaseColor.BLACK);
                Font fontBold = FontFactory.GetFont(FontFactory.TIMES, 8, Font.BOLD, BaseColor.BLACK);
                Font fontHeadings = FontFactory.GetFont(FontFactory.TIMES, 9, Font.UNDERLINE, BaseColor.BLACK);


                color = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#A9A9A9"));



                table = new PdfPTable(2);

                table.TotalWidth = document.PageSize.Width - 30f;
                table.LockedWidth = true;
                table.AddCell(PhraseCellNoBorder(new Paragraph(string.Empty)));
                table.AddCell(PhraseCellNoBorder(new Paragraph(string.Empty)));
                table.SpacingAfter = 40f;
                document.Add(table);

                float[] columnDefinitionSize = { 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F };
                table = new PdfPTable(columnDefinitionSize)
                {
                    WidthPercentage = 100
                };

                cell = new PdfPCell
                {
                    BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)
                };



                for (int i = 0; i < ConsolidateList.Count; i++)
                {

                    string name = ConsolidateList[i].FName + " " + ConsolidateList[i].LName;
                    //CreateCell(table, (i + 1).ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                    //CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                    CreateCell(table, ConsolidateList[i].split_no.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                    CreateCell(table, ConsolidateList[i].ShholderNo.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                    CreateCell(table, name.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                    //ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                    //ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                    //ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                    //ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                    //ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                    CreateCell(table, ConsolidateList[i].seqno, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                    CreateCell(table, ConsolidateList[i].certno.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER); ;
                    CreateCell(table, ConsolidateList[i].kitta, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                    CreateCell(table, ConsolidateList[i].srnofrom, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                    CreateCell(table, ConsolidateList[i].srnoto, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                    CreateCell(table, ConsolidateList[i].split_dt.ToString("MM/dd/yyyy"), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);

                    CreateCell(table, ConsolidateList[i].split_remarks, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);




                    //for (int j = 0; j < ConsolidateList.Count; j++)
                    //{
                    //    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);
                    //    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                    //    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                    //    ReportsCellConfig.CreateCell(table, ConsolidateList[j].seqno, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                    //    ReportsCellConfig.CreateCell(table, ConsolidateList[j].certno.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                    //    ReportsCellConfig.CreateCell(table, ConsolidateList[j].kitta, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                    //    ReportsCellConfig.CreateCell(table, ConsolidateList[j].srnofrom, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                    //    ReportsCellConfig.CreateCell(table, ConsolidateList[j].srnoto, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                    //    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                    //    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);



                    //}

                }




                table.SpacingBefore = 20f;

                document.Add(table);
                table = new PdfPTable(1);
                Paragraph lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_LEFT, 1)));
                lineSeparator.SetLeading(0.5F, 0.5F);
                document.Add(lineSeparator);
                table = new PdfPTable(columnDefinitionSize)
                {
                    WidthPercentage = 100
                };

                cell = new PdfPCell
                {
                    BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)
                };
                CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);

                document.Add(table);


                document.Close();
                document.CloseDocument();
                document.Dispose();
                writer.Close();
                writer.Dispose();
                string file = Convert.ToBase64String(ms.ToArray());
                jsonResponseToReturn.IsSuccess = true;
                jsonResponseToReturn.ResponseData = file;
                //jsonResponseToReturn.Message =  "_ConsolidateReport.pdf";
                jsonResponseToReturn.Message = "Company_" + ReportData.CompEnName + "_Code_" + ReportData.CompEnName + Header + ".pdf";
                fs.Close();
                fs.Dispose();
            }
            return jsonResponseToReturn;
        }

        public JsonResponse CertificateSplitReport(ATTCERTIFICATEREPORT ReportData, JsonResponse data, string rootPath)
        {
            JsonResponse jsonResponse = new JsonResponse();
            if (data.IsSuccess)
            {
                string folderName = "PDFReports";
                string webRootPath = rootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {

                    Directory.CreateDirectory(newPath);

                }
                FileStream fs = new FileStream(rootPath + "\\PDFReports\\" + DateTime.Now.ToString("dd-MM-yyyy hh_mm_s_tt") + "Report.pdf", FileMode.Create, FileAccess.ReadWrite,
                FileShare.None, 4096, FileOptions.DeleteOnClose);

                using (MemoryStream ms = new MemoryStream())
                {

                    List<ATTTableHeader> tableHeaders = new List<ATTTableHeader>();
                    ATTTableHeader tableHeader = new ATTTableHeader();

                    tableHeader = new ATTTableHeader();
                    tableHeader.ColumnDefinition = 1F;
                    tableHeader.ColumnName = "Split No";
                    tableHeaders.Add(tableHeader);
                    tableHeader = new ATTTableHeader();
                    tableHeader.ColumnDefinition = 1F;
                    tableHeader.ColumnName = "Cert No";
                    tableHeaders.Add(tableHeader);
                    tableHeader = new ATTTableHeader();
                    tableHeader.ColumnDefinition = 1F;
                    tableHeader.ColumnName = "Shholder No";
                    tableHeaders.Add(tableHeader);
                    tableHeader = new ATTTableHeader();
                    tableHeader.ColumnDefinition = 1F;
                    tableHeader.ColumnName = "Name";
                    tableHeaders.Add(tableHeader);
                    tableHeader = new ATTTableHeader();
                    tableHeader.ColumnDefinition = 1F;
                    tableHeader.ColumnName = "Seq No";
                    tableHeaders.Add(tableHeader);
                    tableHeader = new ATTTableHeader();
                    tableHeader.ColumnDefinition = 1F;
                    tableHeader.ColumnName = "Share No";
                    tableHeaders.Add(tableHeader);
                    tableHeader = new ATTTableHeader();
                    tableHeader.ColumnDefinition = 1F;
                    tableHeader.ColumnName = "SrNo From";
                    tableHeaders.Add(tableHeader);
                    tableHeader = new ATTTableHeader();
                    tableHeader.ColumnDefinition = 1F;
                    tableHeader.ColumnName = "SrNo To";
                    tableHeaders.Add(tableHeader);
                    tableHeader = new ATTTableHeader();
                    tableHeader.ColumnDefinition = 1F;
                    tableHeader.ColumnName = "New CertNo";
                    tableHeaders.Add(tableHeader);
                    tableHeader = new ATTTableHeader();
                    tableHeader.ColumnDefinition = 1.25F;
                    tableHeader.ColumnName = "Split Date";
                    tableHeaders.Add(tableHeader);
                    tableHeader = new ATTTableHeader();
                    tableHeader.ColumnDefinition = 1F;
                    tableHeader.ColumnName = "Split Remarks";
                    tableHeaders.Add(tableHeader);
                    tableHeader = new ATTTableHeader();
                    tableHeader.ColumnDefinition = 1.25F;
                    tableHeader.ColumnName = "Prepared by:";
                    tableHeaders.Add(tableHeader);
                    tableHeader = new ATTTableHeader();
                    tableHeader.ColumnDefinition = 1.25F;
                    tableHeader.ColumnName = "Authorized by:";
                    tableHeaders.Add(tableHeader);
                    Document document = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30);
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    string Header = "";

                    if (ReportData.DataType == "P")
                    {
                        Header = "Posted Certificate List";
                    }
                    else
                    {
                        Header = "Unposted Certificate List";
                    }

                    writer.PageEvent = new ITextEvents(ReportData.CompCode.ToString(), ReportData.CompEnName, Header, tableHeaders, webRootPath);

                    Phrase phrase = null;
                    PdfPCell cell = null;
                    PdfPTable table = null;
                    BaseColor color = null;
                    document.Open();

                    document.SetMargins(25, 30, 90, 30);

                    Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, 10);
                    Font font2 = FontFactory.GetFont(FontFactory.TIMES, 8);
                    Font fontBlue = FontFactory.GetFont(FontFactory.TIMES, 8, Font.NORMAL, BaseColor.BLUE);
                    Font fontNormal = FontFactory.GetFont(FontFactory.TIMES, 9, Font.NORMAL, BaseColor.BLACK);
                    Font fontBold = FontFactory.GetFont(FontFactory.TIMES, 8, Font.BOLD, BaseColor.BLACK);
                    Font fontHeadings = FontFactory.GetFont(FontFactory.TIMES, 11, Font.UNDERLINE, BaseColor.BLACK);

                    color = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#A9A9A9"));

                    List<ATTCERTIFICATEREPORT> demateRemateList = (List<ATTCERTIFICATEREPORT>)data.ResponseData;
                    table = new PdfPTable(2);

                    table.TotalWidth = document.PageSize.Width - 30f;
                    table.LockedWidth = true;
                    table.AddCell(PhraseCellNoBorder(new Paragraph(String.Empty)));
                    table.AddCell(PhraseCellNoBorder(new Paragraph(String.Empty)));
                    table.SpacingAfter = 40f;
                    document.Add(table);

                    float[] columnDefinitionSize = { 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1.25F, 1F, 1.25F, 1.25F };
                    table = new PdfPTable(columnDefinitionSize)
                    {

                        WidthPercentage = 100

                    };

                    cell = new PdfPCell
                    {
                        BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)

                    };


                    for (int i = 0; i < demateRemateList.Count; i++)
                    {
                        CreateCell(table, demateRemateList[i].Split_NO.ToString() ?? "--", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        CreateCell(table, demateRemateList[i].CertNo.ToString() ?? "--", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        CreateCell(table, demateRemateList[i].ShholderNo.ToString() ?? "--", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        CreateCell(table, demateRemateList[i].name.ToString() ?? "--", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER); ;
                        CreateCell(table, demateRemateList[i].SeqNo.ToString() ?? "--", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);

                        CreateCell(table, demateRemateList[i].Kitta.ToString() ?? "--", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);

                        CreateCell(table, demateRemateList[i].SrNoFrom.ToString() ?? "--", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        CreateCell(table, demateRemateList[i].SrNoTo.ToString() ?? "--", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        CreateCell(table, demateRemateList[i].certno_new.ToString() ?? "--", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        CreateCell(table, demateRemateList[i].split_dt.Substring(0, 10) ?? "--", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        CreateCell(table, (demateRemateList[i].app_remarks ?? "--").ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        CreateCell(table, demateRemateList[i].split_by.ToString() ?? "--", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        CreateCell(table, (demateRemateList[i].ApprovedBy ?? "--").ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);

                    }
                    table.SpacingBefore = 20f;

                    document.Add(table);
                    table = new PdfPTable(1);
                    Paragraph lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_LEFT, 1)));
                    lineSeparator.SetLeading(0.5F, 0.5F);
                    document.Add(lineSeparator);
                    table = new PdfPTable(columnDefinitionSize)
                    {
                        WidthPercentage = 100
                    };

                    cell = new PdfPCell
                    {
                        BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)

                    };
                    CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                    CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                    CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                    CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                    CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                    CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                    CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                    CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                    CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                    CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                    CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                    CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                    CreateCell(table, "", fontHeadings, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                    document.Add(table);

                    document.Close();
                    document.CloseDocument();
                    document.Dispose();
                    writer.Close();
                    writer.Dispose();
                    string file = Convert.ToBase64String(ms.ToArray());
                    jsonResponse.IsSuccess = true;
                    jsonResponse.ResponseData = file;
                    jsonResponse.Message = "_SplitReport.pdf";
                    fs.Close();
                    fs.Dispose();




                }

            }
            return jsonResponse;

        }

        public JsonResponse CertificateSplitReportForSingle(List<ATTCertificateSplit> returnedList, string rootPath)

        {
            JsonResponse returnedResponse = new JsonResponse();
            string folderName = "PDFReports";
            string webRootPath = rootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            FileStream fs = new FileStream(rootPath + "\\PDFReports\\" + DateTime.Now.ToString("dd-MM-yyyy hh_mm_s_tt") + "Report.pdf", FileMode.Create, FileAccess.ReadWrite,
            FileShare.None, 4096, FileOptions.DeleteOnClose);

            using (MemoryStream ms = new MemoryStream())
            {
                List<ATTTableHeader> tableHeaders = new List<ATTTableHeader>();

                ATTTableHeader tableHeader = new ATTTableHeader();

                tableHeader = new ATTTableHeader();
                tableHeader.ColumnDefinition = 1f;
                tableHeader.ColumnName = "CertificateNo";
                tableHeaders.Add(tableHeader);
                tableHeader = new ATTTableHeader();
                tableHeader.ColumnDefinition = 1f;
                tableHeader.ColumnName = "HolderNo";
                tableHeaders.Add(tableHeader);
                tableHeader = new ATTTableHeader();
                tableHeader.ColumnDefinition = 1f;
                tableHeader.ColumnName = "Splitted Date";
                tableHeaders.Add(tableHeader);
                tableHeader = new ATTTableHeader();
                tableHeader.ColumnDefinition = 1f;
                tableHeader.ColumnName = "ShareKitta";
                tableHeaders.Add(tableHeader);
                tableHeader = new ATTTableHeader();
                tableHeader.ColumnDefinition = 1f;
                tableHeader.ColumnName = "From";
                tableHeaders.Add(tableHeader);
                tableHeader = new ATTTableHeader();
                tableHeader.ColumnDefinition = 1f;
                tableHeader.ColumnName = "To";
                tableHeaders.Add(tableHeader);


                Document document = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                writer.PageEvent = new ITextEvents(returnedList[0].compcode, returnedList[0].CertNo.ToString(), "", tableHeaders, webRootPath);

                Phrase phrase = null;
                PdfPCell cell = null;
                PdfPTable table = null;
                BaseColor color = null;
                document.Open();

                document.SetMargins(30, 30, 90, 30);


                Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, 13);
                Font font2 = FontFactory.GetFont(FontFactory.TIMES, 10);
                Font fontBlue = FontFactory.GetFont(FontFactory.TIMES, 10, Font.NORMAL, BaseColor.BLUE);
                Font fontNormal = FontFactory.GetFont(FontFactory.TIMES, 10, Font.NORMAL, BaseColor.BLACK);
                Font fontBold = FontFactory.GetFont(FontFactory.TIMES, 10, Font.BOLD, BaseColor.BLACK);
                Font fontHeadings = FontFactory.GetFont(FontFactory.TIMES, 11, Font.UNDERLINE, BaseColor.BLACK);


                color = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#A9A9A9"));


                table = new PdfPTable(2);

                table.TotalWidth = document.PageSize.Width - 30f;
                table.LockedWidth = true;
                table.AddCell(PhraseCellNoBorder(new Paragraph(string.Empty)));
                table.AddCell(PhraseCellNoBorder(new Paragraph(string.Empty)));
                table.SpacingAfter = 40f;
                document.Add(table);

                document.SetMargins(30, 30, 90, 30);

                float[] columnDefinitionSize = { 1F, 1F, 1F, 1F, 1F, 1F };
                table = new PdfPTable(columnDefinitionSize)
                {
                    WidthPercentage = 100
                };

                cell = new PdfPCell
                {
                    BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)
                };


                returnedList.ForEach(x =>
                {
                    CreateCell(table, x.CertNo.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                    CreateCell(table, x.ShHolderNo.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                    CreateCell(table, x.TranDt.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                    CreateCell(table, x.ShKitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                    CreateCell(table, x.SrNoFrom.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                    CreateCell(table, x.SrNoTo.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                });

                table.SpacingBefore = 20f;

                document.Add(table);
                table = new PdfPTable(1);
                Paragraph lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_LEFT, 1)));
                lineSeparator.SetLeading(0.5F, 0.5F);
                document.Add(lineSeparator);


                document.Close();
                document.CloseDocument();
                document.Dispose();
                writer.Close();
                writer.Dispose();
                String file = Convert.ToBase64String(ms.ToArray());
                returnedResponse.IsSuccess = true;
                returnedResponse.ResponseData = file;
                returnedResponse.Message = "Company_" + returnedList[0].compcode + "_CertificateSplitReport.pdf";
                fs.Close();
                fs.Dispose();
                return returnedResponse;
            }
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

