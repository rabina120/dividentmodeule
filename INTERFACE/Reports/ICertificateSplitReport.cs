using Entity.Common;
using Entity.Reports;

namespace Interface.Reports

{
    public interface ICertificateSplitReport
    {

        JsonResponse GenerateReport(ATTCERTIFICATEREPORT ReportData, string ExportReportType, string FromSystem, string Username, string IPAddress);
    }
}
