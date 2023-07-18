
using Entity.Common;

namespace Interface.DividendProcessing
{
    public interface IDemateDividentPaymentEntry
    {
        JsonResponse GetMaxDemateSeqno(string tablename, string centerid);
        JsonResponse GetDemateDividendInformation(string CompCode, string DivCode, string BoidNo, string a, string action,string username,string IPAddress);
        JsonResponse SaveDemateDividendPaymentEntry(string DivCode, string CompCode,string bankName, string accountNo, string centerid, string Payment, string PayUser,
            string remarks, string warrantNo, string boidno, string selectedAction, string creditedDt, string wissueddate, string username,string IPAddress);
    }
}
