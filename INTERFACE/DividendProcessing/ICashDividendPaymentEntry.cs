
using Entity.Common;

namespace Interface.DividendProcessing
{
    public interface ICashDividendPaymentEntry
    {
        //JsonResponse GetMaxSeqno(string tablename, string centerid);
        JsonResponse GetDividendPaymentInformation(string CompCode, string DivCode, string shholderno, string a, string action);
        JsonResponse SaveCashDividendPayment(string tablename, string wamtpaiddt, string batchno, string compcode, string shholderno, string selectedAction, string username);
    }
}
