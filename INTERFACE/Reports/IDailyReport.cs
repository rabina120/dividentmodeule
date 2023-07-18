using Entity.Common;

namespace Interface.Reports
{
    public interface IDailyReport
    {
        JsonResponse GenerateReport(string UserName, string CompCode, string DailyReportDate, string IPAddress);
    }
}
