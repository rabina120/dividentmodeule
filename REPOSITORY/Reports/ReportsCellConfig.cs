using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Repository.Reports
{
    public static class ReportsCellConfig
    {
        public static void CreateCell(PdfPTable table, string text, Font font, int verticalAllignment = PdfPCell.ALIGN_BOTTOM, int horizontalAlignment = PdfPCell.ALIGN_RIGHT, bool header = true)
        {
            table.AddCell(PhraseCellNoBorder(new Phrase(new Chunk(text, font)), verticalAllignment: verticalAllignment, horizontalAlignment: horizontalAlignment, header));
        }

        public static PdfPCell PhraseCellNoBorder(Phrase phrase, int verticalAllignment = PdfPCell.ALIGN_BOTTOM, int horizontalAlignment = PdfPCell.ALIGN_RIGHT, bool header = true)
        {
            PdfPCell cell = new PdfPCell(phrase);
            cell.VerticalAlignment = verticalAllignment;
            cell.HorizontalAlignment = horizontalAlignment;
            cell.BorderColor = BaseColor.WHITE;
            if (header)
            {
                cell.PaddingBottom = 4f;
                cell.PaddingTop = 4f;

            }

            return cell;
        }
        public static PdfPCell PhraseCell(Phrase phrase, int verticalAllignment = PdfPCell.ALIGN_BOTTOM, int horizontalAlignment = PdfPCell.ALIGN_RIGHT)
        {
            PdfPCell cell = new PdfPCell(phrase);
            cell.VerticalAlignment = verticalAllignment;
            cell.HorizontalAlignment = horizontalAlignment;
            cell.PaddingBottom = 4f;
            cell.PaddingTop = 4f;
            return cell;
        }
    }
}
