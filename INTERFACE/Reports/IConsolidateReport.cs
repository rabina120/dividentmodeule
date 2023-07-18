using Entity.Common;
using Entity.Reports;

namespace Interface.Reports
{
    public interface IConsolidateReport
    {
        JsonResponse GenerateReport(ATTConsolidateReport ReportData, string ExportReportType, string UserName, string IP);
    }
}
