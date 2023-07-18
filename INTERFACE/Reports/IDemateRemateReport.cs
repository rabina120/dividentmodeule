using Entity.Common;
using Entity.Reports;

namespace Interface.Reports
{
    public interface IDemateRemateReport
    {
        JsonResponse GetAllParaCompChild(string CompCode);
        JsonResponse GetDataFromCertificateDetail(string CompCode);
        JsonResponse GenerateReport(ATTReportTypeForDemateRemate aTTReportType, string ExportReportType, string Username);
        JsonResponse GenerateReportExcel(ATTReportTypeForDemateRemate aTTReportType, string ExportReportType, string Username);
    }
}
