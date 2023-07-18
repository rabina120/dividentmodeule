using Entity.Common;

namespace Interface.Reports
{
    public interface IDemateDividendReport
    {
        public JsonResponse GetRecordForExcel(string compcode, string divcode, string selectedReportType, string dateFrom, string dateTo, string posted);
        public JsonResponse GenerateData(string CompCode, string DivCode, string SelectedReportType, string DateFrom, string DateTo, string Posted);
    }
}
