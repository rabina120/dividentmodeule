using Entity.Common;

namespace Interface.Reports
{
    public interface IEsewaReports
    {
        JsonResponse GetAllReports();

        JsonResponse GetAllBatchList(string CompCode, string DivCode);
        JsonResponse GenerateReport(string CompCode, string DivCode, string BatchNo, string ReportType);

        JsonResponse LoadEsewaReportByHolder(string CompCode, string DivCode, string CompEnName, string BatchNo, string Holder);
    }
}
