using Entity.Common;

namespace Interface.Reports
{
    public interface IEsewaStatusReport
    {
        JsonResponse GetAllDividends(string CompCode);
        JsonResponse GetAllBatch(string compCode, string divCode);
        JsonResponse GenerateReportData(string CompCode, string DivCode, string Batch, string ReportType, string ReportSubType,string exportTo, string UserName,string IpAddress);
    }
}
