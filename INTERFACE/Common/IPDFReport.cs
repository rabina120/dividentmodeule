using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;


namespace Interface.Common
{
    public interface IPDFReport
    {
        Document InitDocReport(string RootPath, string filename);
        PdfPTable GenTable(List<string> Headers, List<string> Columns, IEnumerable<object> list);
    }
}
