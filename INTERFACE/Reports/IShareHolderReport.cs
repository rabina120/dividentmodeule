using Entity.Common;
using Entity.Reports;

namespace Interface.Reports
{
    public interface IShareHolderReport
    {
        JsonResponse GenerateReport(string UserName, string CompCode, string ExportReportType, string IPAddress, ATTShareHolderReportData ShareHolderReportData);
        JsonResponse ShholderLockUnlock(string compCode, string dataType, string statusType, string dateFrom, string dateTo, string holderNoFrom, string holderNoTo, string reportType, string v1, string v2);
    }
}
