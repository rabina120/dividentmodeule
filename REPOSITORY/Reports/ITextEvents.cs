using Entity.Reports;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Repository.Reports
{
    public class ITextEvents : PdfPageEventHelper
    {
        private string CompName, CompCode, ReportType;
        private List<ATTTableHeader> tableHeaders;
        public ITextEvents(string CompCode, string CompName, string ReportType, List<ATTTableHeader> tableHeaders,string WebRootPath)
        {
            this.CompCode = CompCode;
            this.CompName = CompName;
            this.ReportType = ReportType;
            this.tableHeaders = tableHeaders;
            this.rootpath = WebRootPath;
        }

        public ITextEvents(string v, string compEnName, List<ATTTableHeader> tableHeaders1)
        {
            this.v = v;
            this.compEnName = compEnName;
            this.tableHeaders1 = tableHeaders1;
        }

        PdfContentByte cb;

        // we will put the final number of pages in a template
        PdfTemplate headerTemplate, footerTemplate;

        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;

        // This keeps track of the creation time
        DateTime PrintTime = DateTime.Now;


        #region Fields
        private string _header;
        private string v;
        private string compEnName;
        private List<ATTTableHeader> tableHeaders1;
        private string rootpath;
        #endregion

        #region Properties
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }
        #endregion
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                PrintTime = DateTime.Now;
                bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;

                headerTemplate = cb.CreateTemplate(100, 100);
                footerTemplate = cb.CreateTemplate(50, 50);

            }
            catch (DocumentException de)
            {

            }
            catch (System.IO.IOException ioe)
            {

            }
        }

        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnEndPage(writer, document);
            iTextSharp.text.Font baseFontSmallBold = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            iTextSharp.text.Font baseFontSmall = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
            iTextSharp.text.Font baseFontNormal = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 12f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
            iTextSharp.text.Font baseFontBig = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            Phrase p1Header = new Phrase(CompName + " | " + ReportType, baseFontNormal);



            //Create PdfTable object
            PdfPTable pdfTab = new PdfPTable(3);
            Image logo = iTextSharp.text.Image.GetInstance(Path.Combine(rootpath, "img", "pdfreport.png"));
            //logo.ScaleToFit(40, 30);
            logo.SpacingBefore = 0;
            logo.ScalePercent(40);
            //We will have to create separate cells to include image logo and 2 separate strings
            //Row 1
            PdfPCell pdfCell = new PdfPCell(logo);

            PdfPCell pdfCell1 = new PdfPCell(p1Header);
            PdfPCell pdfCell2 = new PdfPCell(p1Header);
            PdfPCell pdfCell3 = new PdfPCell(new Phrase("Date:" + PrintTime.ToShortDateString(), baseFontSmall));
            String text = "Page No :" + writer.PageNumber;

            float[] columnDefinitionSize = tableHeaders.Select(x => x.ColumnDefinition).ToArray();
            PdfPTable table = new PdfPTable(columnDefinitionSize)
            {
                WidthPercentage = 100,
                TotalWidth = document.PageSize.Width - 30f
            };

            foreach (ATTTableHeader tableHeader in tableHeaders)
            {
                PdfPCell pdfPCell = new PdfPCell(new Phrase(tableHeader.ColumnName, baseFontSmallBold));
                pdfPCell.Border = 0;
                pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                
                table.AddCell(pdfPCell);
            }

            table.WriteSelectedRows(0, -1, 20, document.PageSize.Height - 75, writer.DirectContent);


            //Add paging to footer
            {
                cb.BeginText();
                cb.SetFontAndSize(bf, 8);
                cb.SetTextMatrix(document.PageSize.Width / 2, document.PageSize.GetBottom(10));
                cb.ShowText(text);
                cb.EndText();
                float len = bf.GetWidthPoint(text, 12);
                cb.AddTemplate(footerTemplate, document.PageSize.Width / 2, document.PageSize.GetBottom(10));
            }

            //Row 2
            PdfPCell pdfCell4 = new PdfPCell();//(new Phrase(ReportType, baseFontSmall));


            //set the alignment of all three cells and set border to 0
            pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfCell1.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfCell2.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfCell3.HorizontalAlignment = Element.ALIGN_RIGHT;
            pdfCell4.HorizontalAlignment = Element.ALIGN_CENTER;

            pdfCell.VerticalAlignment = Element.ALIGN_TOP;
            pdfCell1.VerticalAlignment = Element.ALIGN_TOP;
            pdfCell3.VerticalAlignment = Element.ALIGN_TOP;
            pdfCell4.VerticalAlignment = Element.ALIGN_TOP;

            pdfCell4.Colspan = 3;

            pdfCell.Border = 0;
            pdfCell1.Border = 0;
            pdfCell2.Border = 0;
            pdfCell3.Border = 0;
            pdfCell4.Border = 0;

            //add all three cells into PdfTable
            pdfTab.AddCell(pdfCell);
            //pdfTab.AddCell(pdfCell1);
            pdfTab.AddCell(pdfCell2);
            pdfTab.AddCell(pdfCell3);
            pdfTab.AddCell(pdfCell4);

            pdfTab.TotalWidth = document.PageSize.Width - 30f;
            pdfTab.WidthPercentage = 100;
            //pdfTab.HorizontalAlignment = Element.ALIGN_CENTER;    

            //call WriteSelectedRows of PdfTable. This writes rows from PdfWriter in PdfTable
            //first param is start row. -1 indicates there is no end row and all the rows to be included to write
            //Third and fourth param is x and y position to start writing
            pdfTab.WriteSelectedRows(0, -1, 20, document.PageSize.Height - 30, writer.DirectContent);
            //set pdfContent value
            //Move the pointer and draw line to separate header section from rest of page
            cb.MoveTo(20, document.PageSize.Height - 60);
            cb.LineTo(document.PageSize.Width - 10, document.PageSize.Height - 60);
            cb.Stroke();
            //Move the pointer and draw line to separate footer section from rest of page
            cb.MoveTo(20, document.PageSize.GetBottom(30));
            cb.LineTo(document.PageSize.Width - 10, document.PageSize.GetBottom(30));
            cb.Stroke();
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);


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
