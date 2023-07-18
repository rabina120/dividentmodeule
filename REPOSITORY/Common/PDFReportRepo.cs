
using Interface.Common;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repository.Common
{
    public class PDFReportRepo : IPDFReport
    {
        public PdfPTable GenTable(List<string> Headers, List<string> Columns, IEnumerable<object> list)
        {
            PdfPTable table = new PdfPTable(5);
            //table.SetWidths(new int[16] { 5, 5, 5, 15, 15, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 });
            //table.HeaderRows = 1;
            table.WidthPercentage = 100f;

            foreach (string Header in Headers)
            {
                PdfPCell cell = new PdfPCell();
                cell.BackgroundColor = BaseColor.GRAY;
                cell.AddElement(new Chunk(Header));
                table.AddCell(cell);
            }

            var lst = list.ToList();
            for (int i = 0; i < lst.Count; i++)
            {
                object obj = lst[i];
                table.AddCell(new Phrase(Convert.ToString(i + 1)));
                foreach (string col in Columns)
                {
                    string td = Convert.ToString(obj.GetType().GetProperty(col).GetValue(obj, null));
                    table.AddCell(new Phrase(td));
                }
            }
            return table;
        }

        public Document InitDocReport(string RootPath, string filename)
        {
            Document document = new Document();
            document.SetPageSize(iTextSharp.text.PageSize.A4);
            document.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            document.Open();

            //report header

            Paragraph prglogo = new Paragraph();
            prglogo.Alignment = Element.ALIGN_CENTER;
            Image png = Image.GetInstance(RootPath + "\\image\\" + "logo.png");
            png.ScalePercent(30f);
            png.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
            document.Add(png);

            Paragraph prgHeadings = new Paragraph();
            prgHeadings.Alignment = Element.ALIGN_CENTER;
            prgHeadings.Add(new Chunk("Government of Nepal \nMinistry of Energy, Water Resources and Irrigation \nAlternative Energy Promotion Centre"));
            document.Add(prgHeadings);

            Paragraph prgHeading1 = new Paragraph();
            prgHeading1.Alignment = Element.ALIGN_CENTER;
            prgHeading1.Add(new Chunk("Khumaltar, Lalitpur - Nepal"));
            document.Add(prgHeading1);

            Paragraph prgHeading = new Paragraph();
            prgHeading.Alignment = Element.ALIGN_CENTER;
            prgHeading.Add(new Chunk("Household-Report- " + System.DateTime.Now.ToString("dd MMM, yyyy").TrimStart('0')));
            document.Add(prgHeading);

            Paragraph prgReport = new Paragraph();
            BaseFont btnReport = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, BaseFont.EMBEDDED);
            iTextSharp.text.Font fntTableHead = new iTextSharp.text.Font(btnReport, 9, 1, BaseColor.WHITE);
            prgReport.Alignment = Element.ALIGN_LEFT;
            return document;
        }


    }
}
