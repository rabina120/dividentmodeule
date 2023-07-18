
using Entity.Common;

namespace INTERFACE.Reports
{
    public interface IUserReport
    {
        JsonResponse GetSecurityMatrixReport(int roleid, string username, string ip);
        JsonResponse GetSecurityMatrixReportForExcel(int roleid, string username, string ip);
        JsonResponse GetUserDetailsAuditReport(string reportType, string fromDate, string ToDate, string userID, string _loggedInUser, string IpAddress);
        JsonResponse GenerateReport(string UserName, string DateFrom, string DateTo, string IPAddress, string CurrentUserName, string ReportType);
    }
}
