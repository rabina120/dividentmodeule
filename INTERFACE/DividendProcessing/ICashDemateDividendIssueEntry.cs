
using Entity.Common;

namespace Interface.DividendProcessing
{
    public interface ICashDemateDividendIssueEntry
    {
        JsonResponse GetMaxDemateSeqno(string tablename, string centerid);
        JsonResponse GetDemateDividendInformation(string CompCode, string DivCode, string BoidNo, string a, string action, string UserName, string IP);
        JsonResponse SaveCashDemateDividend(string DivCode, string CompCode, string centerid, string remarks, string bankName, string accountNo, string compcode, string warrantNo, string boidno, string selectedAction,  string wissueddate, string creditedDt, string UserName, string IsPaidBy, string IP);
    }
}
