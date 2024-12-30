
using Entity.Common;
using System.Data;

namespace Interface.DividendProcessing
{
    public interface ICashDividendEntry
    {
        JsonResponse GetMaxSeqno(string tablename, string centerid);
        JsonResponse GetDividendInformation(string CompCode, string DivCode, string shholderno, string a, string action, string UserName, string IP);
        JsonResponse SaveCashDividend(string DivCode, string CompCode, string centerid, string bankName, string accountNo, string remarks, string telno, string cashOrCheque, string UserName, string warrantNo, string shholderno, string action, string creditedDt, string wissueddate, string IsPaidBy, string IP);

        JsonResponse BulkIssue(string CompCode, string DivCode, string DivType, string IssueDate, bool isIssue, bool isPay, string IssueRemarks, DataTable dataTable, string UserName, string IPAddress);
        JsonResponse BulkPayment(string CompCode, string DivCode, string DivType, string IssueDate, bool isIssue, bool isPay, string IssueRemarks, DataTable dataTable, string UserName, string IPAddress);
    }
}
