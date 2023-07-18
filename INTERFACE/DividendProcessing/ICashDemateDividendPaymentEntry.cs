
using Entity.Common;

namespace Interface.DividendProcessing

{
    public interface ICashDemateDividendPaymentEntry
    {
        JsonResponse GetMaxDematePaymentSeqno(string tablename, string centerid);
        JsonResponse GetDemateDividendPaymentInformation(string CompCode, string DivCode, string shholderno, string a, string action);
        JsonResponse SaveCashDemateDividendPayment(string tablename, string wamtpaiddt, string batchno, string compcode, string shholderno, string selectedAction, string username);
    }
}
