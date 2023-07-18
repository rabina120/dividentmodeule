
using Entity.Common;

namespace Interface.Reports
{
    public interface IPSLReport
    {
        JsonResponse GetAllPledgeAt();
        //JsonResponse GenerateReport(ATTPSLReport ReportData, string ExportReportType);
        //JsonResponse GenerateReport(ATTPSLReport aTTPSLReport, string ExportReportType, string Username);
        //JsonResponse GenerateReport(string CompCode, string PCode, string TranType, string DateFrom, string DateTo, string HolderNoFrom, string HolderNoTo, string CertNoFrom, string CertNoTo, string AppStatus, string OrderBy, string ShareType, string UserName, string ReportType, string ExportReportType);
        JsonResponse GenerateReport(string CompCode, string PCode, string TranType, string PSLDateFrom, string PSLDateTo, string HolderNoFrom, string HolderNoTo, string CertNoFrom, string CertNoTo, string AppStatus, string OrderBy, string ShareType, string ReportType, string UserName, string IpAddress, string EntryDateTime);
        JsonResponse GenerateReportPdf(string CompCode, string PCode, string TranType, string PSLDateFrom, string PSLDateTo, string HolderNoFrom, string HolderNoTo, string CertNoFrom, string CertNoTo, string AppStatus, string OrderBy, string ShareType, string ReportType, string UserName, string IpAddress, string EntryDateTime);
    }
}
