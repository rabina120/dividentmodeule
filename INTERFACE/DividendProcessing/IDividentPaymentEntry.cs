
using Entity.Common;

namespace Interface.DividendProcessing
{
    public interface IDividentPaymentEntry
    {
        JsonResponse GetMaxSeqno(string tablename, string centerid);
        JsonResponse GetDividendInformation(string CompCode, string DivCode, string shholderno, string a, string action, string UserName, string IPAddress);
        JsonResponse SaveDividentPaymentEntry(string DivCode, string CompCode, string bankName, string accountNo, string centerid, string PayUser, string telno, string cashOrCheque,string warrantNo, string shholderno, string action, string creditedDt, string wissueddate,string IssueRemarks, string IPAddress);
    }
}
