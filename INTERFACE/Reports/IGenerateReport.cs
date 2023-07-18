using Entity.Common;

namespace Interface.Reports
{
    public interface IGenerateReport
    {
        JsonResponse GenerateReport(string CompCode, string ReportType, string UserName, string IPAddress);
        JsonResponse GenerateReportDemateHolderList(string CompCode, string DataUploadDate, string UserName, string IPAddress);
        JsonResponse GenerateReportDemateRemateList(string CompCode, string TransferedDtFrom, string TransferedDtTo, string UserName, string IPAddress);
    }
}
